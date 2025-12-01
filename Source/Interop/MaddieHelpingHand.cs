using System;
using Monocle;

namespace Celeste.Mod.Microlith57Misc;

public static partial class Utils {

    private static bool checkedHelpingHand;
    internal static bool HelpingHandLoaded {
        get {
            if (checkedHelpingHand || Everest.Loader.DependencyLoaded(new EverestModuleMetadata { Name = "MaxHelpingHand", Version = new Version(1, 0, 0) })) {
                checkedHelpingHand = true;
                return true;
            }
            return false;
        }
    }
    internal static bool CheckHelpingHand(string erroringEntity) {
        if (HelpingHandLoaded)
            return true;

        Audio.SetMusic(null);
        LevelEnter.ErrorMessage = "{big}Oops!{/big}{n}To use {# F94A4A}" + erroringEntity + "{#}, you need to have {# d678db}Maddie's Helping Hand{#} enabled!";
        var session = Engine.Scene is Level level ? new Session(level.Session.Area) : new Session();
        LevelEnter.Go(session, fromSaveData: false);

        return false;
    }

}
