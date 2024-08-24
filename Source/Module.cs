using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Celeste.Mod.GravityHelper.Components;
using Microsoft.Xna.Framework;
using Monocle;

using Celeste.Mod.Microlith57Misc.Entities;
using Celeste.Mod.Microlith57Misc.Entities.Recordings;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System.Reflection;

namespace Celeste.Mod.Microlith57Misc;

public class Module : EverestModule {

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public static Module Instance { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.


    public Module() {
        Instance = this;
#if DEBUG
        // debug builds use verbose logging
        Logger.SetLogLevel(nameof(Module), LogLevel.Verbose);
#else
        // release builds use info logging to reduce spam in log files
        Logger.SetLogLevel(nameof(Module), LogLevel.Info);
#endif
    }

    public override void Load() {
        IL.Celeste.Player.NormalUpdate += manipPlayerNormalUpdate;

        On.Celeste.Player.Update += hookPlayerUpdate;
        On.Celeste.Player.IsRiding_JumpThru += hookPlayerIsRiding;
        On.Celeste.Player.DustParticleFromSurfaceIndex += hookDustParticle;

        On.Celeste.Level.Update += hookLevelUpdate;
    }

    public override void Unload() {
        IL.Celeste.Player.NormalUpdate -= manipPlayerNormalUpdate;

        On.Celeste.Player.Update -= hookPlayerUpdate;
        On.Celeste.Player.IsRiding_JumpThru -= hookPlayerIsRiding;
        On.Celeste.Player.DustParticleFromSurfaceIndex -= hookDustParticle;

        On.Celeste.Level.Update -= hookLevelUpdate;
    }

    public override void LoadContent(bool firstLoad) {
        base.LoadContent(firstLoad);

        Recording.P_Appear ??= new ParticleType {
            FadeMode = ParticleType.FadeModes.Late,
            Size = 1f,
            Direction = 0f,
            DirectionRange = (float)Math.PI * 2f,
            SpeedMin = 5f,
            SpeedMax = 10f,
            LifeMin = 0.6f,
            LifeMax = 1.2f,
            SpeedMultiplier = 0.3f
        };

        AreaSwitch.P_FireInactive ??= new ParticleType(TouchSwitch.P_FireWhite) {
            Size = 0.25f
        };

        AreaSwitch.P_Spark ??= new ParticleType {
            Color = Color.White,
            Color2 = Color.White,
            ColorMode = ParticleType.ColorModes.Blink,
            FadeMode = ParticleType.FadeModes.Late,
            Size = 1f,
            LifeMin = 0.4f,
            LifeMax = 0.8f,
            SpeedMin = 10f,
            SpeedMax = 20f,
            DirectionRange = Calc.Circle,
            SpeedMultiplier = 0.1f,
            Acceleration = new Vector2(0f, 10f)
        };

        Box.P_Impact ??= new ParticleType(TheoCrystal.P_Impact) {
            Color = Color.White
        };
    }

    private static void manipPlayerNormalUpdate(ILContext il) {
        ILCursor cursor = new(il);

        cursor.GotoNext(instr => instr.MatchCallOrCallvirt<Player>("get_Holding"));
        cursor.GotoNext(instr => instr.MatchCallOrCallvirt<Tracker>("GetComponents"));

        // | foreach (var h in Scene.Tracker.GetComponents<Holdable>())
        cursor.GotoPrev(MoveType.AfterLabel,
                        instr => instr.MatchLdarg(0),
                        instr => instr.MatchCallOrCallvirt<Entity>("get_Scene"),
                        instr => instr.MatchCallOrCallvirt<Scene>("get_Tracker"));

        ILLabel label_else = cursor.DefineLabel();

        // + if (Box.TryPickupAny(this))
        cursor.EmitLdarg(0);
        cursor.Emit(OpCodes.Call, typeof(Box).GetMethod("TryPickupAny")!);
        cursor.Emit(OpCodes.Brfalse, label_else);

        // +   return 8;
        cursor.EmitLdcI4(8);
        cursor.Emit(OpCodes.Ret);

        // + }
        cursor.MarkLabel(label_else);

        return;
    }

    private static void hookPlayerUpdate(On.Celeste.Player.orig_Update orig, Player self) {
        bool invert = self.Get<GravityComponent>()?.ShouldInvert ?? false;

        Dictionary<BoxSurface, (bool, bool, bool)> boxesWithOrigCollidableStates = [];
        foreach (BoxSurface boxSurface in self.Scene.Tracker.GetComponents<BoxSurface>()) {
            boxesWithOrigCollidableStates.Add(boxSurface, (boxSurface.Collidable,
                                                           boxSurface.CollidableTop,
                                                           boxSurface.CollidableBot));

            if (self.Holding?.Entity == boxSurface.Entity)
                boxSurface.Collidable = false;
            else if (!invert)
                boxSurface.CollidableBot = false;
            else
                boxSurface.CollidableTop = false;
        }

        orig(self);

        foreach ((var surface, (var wasCollidable, var wasCollidableTop, var wasCollidableBot)) in boxesWithOrigCollidableStates) {
            surface.Collidable = wasCollidable;
            surface.CollidableTop = wasCollidableTop;
            surface.CollidableBot = wasCollidableBot;
        }
    }

    private static bool hookPlayerIsRiding(On.Celeste.Player.orig_IsRiding_JumpThru orig, Player self, JumpThru jumpthru) {
        bool invert = self.Get<GravityComponent>()?.ShouldInvert ?? false;

        if (self.Holding?.Entity is Box box && (jumpthru == box.Surface.SurfaceTop || jumpthru == box.Surface.SurfaceBot))
            return false;
        else if (jumpthru.Get<BoxSurface.BelongsToBox>() is { } belongsToBox &&
                 ((belongsToBox.IsTop && invert) || (belongsToBox.IsBot && !invert)))
            return false;
        else
            return orig(self, jumpthru);
    }

    private static ConditionalWeakTable<Platform, ParticleType> platformDustOverrides = [];
    public static void OverrideDust(Platform platform, ParticleType particle) => platformDustOverrides.AddOrUpdate(platform, particle);

    private static ParticleType hookDustParticle(On.Celeste.Player.orig_DustParticleFromSurfaceIndex orig, Player self, int index) {
        if (index == SurfaceIndex.Glitch) {
            bool invert = self.Get<GravityComponent>()?.ShouldInvert ?? false;
            var pos = self.Position + (invert ? -Vector2.UnitY : Vector2.UnitY);
            var platform = SurfaceIndex.GetPlatformByPriority(self.CollideAll<Platform>(pos));

            if (platform != null && platformDustOverrides.TryGetValue(platform, out var particle))
                return particle;
        }
        return orig(self, index);
    }

    private static void hookLevelUpdate(On.Celeste.Level.orig_Update orig, Level self) {
        Box.updatedThisFrame = false;
        orig(self);
    }

}