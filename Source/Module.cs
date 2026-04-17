namespace Celeste.Mod.Microlith57Misc;

public class Module : EverestModule {
    public static Module Instance { get; private set; } = null!;

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

    public override void Load() => LifecycleMethods.OnLoad();
    public override void Unload() => LifecycleMethods.OnUnload();
    public override void LoadContent(bool firstLoad) {
        base.LoadContent(firstLoad);
        LifecycleMethods.OnLoadContent(firstLoad);
    }
}