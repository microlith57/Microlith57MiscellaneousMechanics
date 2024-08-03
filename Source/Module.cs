using System;
using Monocle;

namespace Celeste.Mod.Microlith57.IntContest;

public class Module : EverestModule {
    public static Module Instance { get; private set; }

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
        // TODO: apply any hooks that should always be active
    }

    public override void Unload() {
        // TODO: unapply any hooks applied in Load()
    }

    public override void LoadContent(bool firstLoad) {
        base.LoadContent(firstLoad);

        PlayerPlayback.P_Appear ??= new ParticleType
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
    }
}