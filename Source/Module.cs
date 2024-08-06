using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57.IntContest;

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
        On.Celeste.Player.Update += hookPlayerUpdate;
        On.Celeste.Player.IsRiding_JumpThru += hookPlayerIsRiding;
        On.Celeste.Player.DustParticleFromSurfaceIndex += hookDustParticle;
    }

    public override void Unload() {
        On.Celeste.Player.Update -= hookPlayerUpdate;
        On.Celeste.Player.IsRiding_JumpThru -= hookPlayerIsRiding;
        On.Celeste.Player.DustParticleFromSurfaceIndex -= hookDustParticle;
    }

    public override void LoadContent(bool firstLoad) {
        base.LoadContent(firstLoad);

        Recordings.Recording.P_Appear ??= new ParticleType {
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

    private static void hookPlayerUpdate(On.Celeste.Player.orig_Update orig, Player self) {
        if (self.Holding?.Entity is Box box) {
            bool was_collidable = box.Surface.Collidable;
            box.Surface.Collidable = false;

            orig(self);

            box.Surface.Collidable = was_collidable;
        } else
            orig(self);
    }

    private static bool hookPlayerIsRiding(On.Celeste.Player.orig_IsRiding_JumpThru orig, Player self, JumpThru jumpthru) {
        if (self.Holding?.Entity is Box box && jumpthru == box.Surface)
            return false;
        else
            return orig(self, jumpthru);
    }

    private static ConditionalWeakTable<Platform, ParticleType> platformDustOverrides = [];
    public static void OverrideDust(Platform platform, ParticleType particle) => platformDustOverrides.AddOrUpdate(platform, particle);

    private static ParticleType hookDustParticle(On.Celeste.Player.orig_DustParticleFromSurfaceIndex orig, Player self, int index) {
        if (index == SurfaceIndex.Glitch) {
            var platform = SurfaceIndex.GetPlatformByPriority(self.CollideAll<Platform>(self.Position + Vector2.UnitY));
            if (platform != null && platformDustOverrides.TryGetValue(platform, out var particle))
                return particle;
        }
        return orig(self, index);
    }

}