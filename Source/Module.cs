using MonoMod.ModInterop;

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

    public override void Load() => LifecycleMethods.OnLoad();
    public override void Unload() => LifecycleMethods.OnUnload();
    public override void LoadContent(bool firstLoad) {
        base.LoadContent(firstLoad);
        LifecycleMethods.OnLoadContent(firstLoad);
    }

    [OnLoad]
    internal static void ModInterop() {
        typeof(Imports.GravityHelper).ModInterop();
        Imports.GravityHelper.OnImport();

        typeof(Imports.FrostHelper).ModInterop();
    }

}
