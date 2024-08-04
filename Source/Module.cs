using System;
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
    }

    public override void Unload() {
    }

    public override void LoadContent(bool firstLoad) {
        base.LoadContent(firstLoad);

        Recordings.Recording.P_Appear ??= new ParticleType
		{
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

        AreaSwitch.P_Spark ??= new ParticleType
		{
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
    }
}