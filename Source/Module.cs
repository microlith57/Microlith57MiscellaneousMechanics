using System;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.ModInterop;

using Celeste.Mod.Microlith57Misc.Entities;
using Celeste.Mod.Microlith57Misc.Components;
using Celeste.Mod.Microlith57Misc.Entities.Recordings;
using System.Runtime.CompilerServices;

namespace Celeste.Mod.Microlith57Misc;

public class Module : EverestModule {

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public static Module Instance { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public override Type SettingsType => typeof(Microlith57MiscSettings);
    public static Microlith57MiscSettings Settings => (Microlith57MiscSettings)Instance._Settings;

    public const string MOD_NAME = "Microlith57Misc";

    public Module() {
        Instance = this;
#if DEBUG
        // debug builds use verbose logging
        Logger.SetLogLevel(MOD_NAME, LogLevel.Verbose);
#else
        // release builds use info logging to reduce spam in log files
        Logger.SetLogLevel(MOD_NAME, LogLevel.Info);
#endif
    }

    public override void Load() {
        IL.Celeste.Player.NormalUpdate += HoldablePriorityController.manipPlayerNormalUpdate;
        IL.Monocle.Scene.BeforeUpdate += FreezeTimeActiveController.manipSceneBeforeUpdate;

        On.Celeste.Player.IsRiding_JumpThru += hookPlayerIsRiding;
        On.Celeste.Player.DustParticleFromSurfaceIndex += hookDustParticle;

        Everest.Events.Level.OnBeforeUpdate += Box.BeforeLevelUpdate;

        CappedStamina.Load();
        LightRenderHook.Load();
        DecalRegistryExt.CustomLight.Handler.Load();

        typeof(Imports.GravityHelper).ModInterop();
        Imports.GravityHelper.OnImport();

        typeof(Imports.FrostHelper).ModInterop();
    }

    public override void Unload() {
        IL.Celeste.Player.NormalUpdate -= HoldablePriorityController.manipPlayerNormalUpdate;
        IL.Monocle.Scene.BeforeUpdate -= FreezeTimeActiveController.manipSceneBeforeUpdate;

        On.Celeste.Player.IsRiding_JumpThru -= hookPlayerIsRiding;
        On.Celeste.Player.DustParticleFromSurfaceIndex -= hookDustParticle;

        Everest.Events.Level.OnBeforeUpdate -= Box.BeforeLevelUpdate;

        CappedStamina.Unload();
        LightRenderHook.Unload();
    }

    public override void LoadContent(bool firstLoad) {
        base.LoadContent(firstLoad);

#if FEATURE_FLAG_RECORDINGS
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
#endif

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

#if FEATURE_FLAG_BOX
        Box.P_Impact ??= new ParticleType(TheoCrystal.P_Impact) {
            Color = Color.White
        };
#endif
    }

    private static bool hookPlayerIsRiding(On.Celeste.Player.orig_IsRiding_JumpThru orig, Player self, JumpThru jumpthru) {
        bool invert = self.ShouldInvert();

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
            bool invert = self.ShouldInvert();
            var pos = self.Position + (invert ? -Vector2.UnitY : Vector2.UnitY);
            var platform = SurfaceIndex.GetPlatformByPriority(self.CollideAll<Platform>(pos));

            if (platform != null && platformDustOverrides.TryGetValue(platform, out var particle))
                return particle;
        }
        return orig(self, index);
    }

}
