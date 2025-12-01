using System;
using Monocle;

namespace Celeste.Mod.Microlith57Misc;

public static partial class Utils {

    private static bool checkedGravityHelper;
    internal static bool GravityHelperLoaded {
        get {
            if (checkedGravityHelper || Everest.Loader.DependencyLoaded(new EverestModuleMetadata { Name = "GravityHelper", Version = new Version(1, 0, 0) })) {
                checkedGravityHelper = true;
                return true;
            }
            return false;
        }
    }
    internal static bool CheckGravityHelper(string erroringEntity) {
        if (GravityHelperLoaded)
            return true;

        Audio.SetMusic(null);
        LevelEnter.ErrorMessage = "{big}Oops!{/big}{n}To use {# F94A4A}" + erroringEntity + "{#}, you need to have {# d678db}Gravity Helper{#} enabled!";
        var session = Engine.Scene is Level level ? new Session(level.Session.Area) : new Session();
        LevelEnter.Go(session, fromSaveData: false);

        return false;
    }

    public static bool ShouldInvert(this Actor self) {
        if (Imports.GravityHelper.IsActorInverted == null)
            return false;
        return Imports.GravityHelper.IsActorInverted(self);
    }

    public static void SetInverted(this Actor self, bool invert, string erroringEntity) {
        CheckGravityHelper(erroringEntity);
        Imports.GravityHelper.SetActorGravity(self, invert ? Imports.GravityHelper.GravityType.Inverted : Imports.GravityHelper.GravityType.Normal, 1f);
    }

    public static Component? GravityComponentIfExists() {
        if (!GravityHelperLoaded)
            return null;
        return GravityHelperContainmentChamber.makeGravityComponent();
    }
    
}

internal static class GravityHelperContainmentChamber {

    internal static Component makeGravityComponent() => new GravityHelper.Components.GravityComponent();

}
