using System;
using System.Collections.Generic;
using Celeste.Mod.GravityHelper;
using Celeste.Mod.GravityHelper.Components;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc;

internal static class Utils {

    private static bool checkedGravityHelper;
    private static bool GravityHelperLoaded {
        get {
            if (checkedGravityHelper || Everest.Loader.DependencyLoaded(new EverestModuleMetadata { Name = "GravityHelper", Version = new Version(1, 0, 0) })) {
                checkedGravityHelper = true;
                return true;
            }
            return false;
        }
    }

    public static bool CheckGravityHelper(string erroringEntity) {
        if (GravityHelperLoaded)
            return true;

        Audio.SetMusic(null);
        LevelEnter.ErrorMessage = "{big}Oops!{/big}{n}To use {# F94A4A}" + erroringEntity + "{#}, you need to have {# d678db}Gravity Helper{#} installed!";
        var session = Engine.Scene is Level level ? new Session(level.Session.Area) : new Session();
        LevelEnter.Go(session, fromSaveData: false);

        return false;
    }

    public static bool ShouldInvert(this Entity e) => GravityHelperLoaded && GravityHelperContainmentChamber.shouldInvert(e);

    public static void SetInverted(this Entity e, bool inverted, string entityNameJustInCaseHahaNoReason) {
        if (!GravityHelperLoaded) {
            if (inverted) {
                CheckGravityHelper(entityNameJustInCaseHahaNoReason);
            }
        } else {
            GravityHelperContainmentChamber.setInverted(e, inverted);
        }
    }

    public static Component? GravityComponentIfExists() => GravityHelperLoaded ? GravityHelperContainmentChamber.makeGravityComponent() : null;

    private static bool checkedHelpingHand;
    private static bool HelpingHandLoaded {
        get {
            if (checkedHelpingHand || Everest.Loader.DependencyLoaded(new EverestModuleMetadata { Name = "MaxHelpingHand", Version = new Version(1, 0, 0) })) {
                checkedHelpingHand = true;
                return true;
            }
            return false;
        }
    }
    public static bool CheckHelpingHand(string erroringEntity) {
        if (HelpingHandLoaded)
            return true;

        Audio.SetMusic(null);
        LevelEnter.ErrorMessage = "{big}Oops!{/big}{n}To use {# F94A4A}" + erroringEntity + "{#}, you need to have {# d678db}Maddie's Helping Hand{#} installed!";
        var session = Engine.Scene is Level level ? new Session(level.Session.Area) : new Session();
        LevelEnter.Go(session, fromSaveData: false);

        return false;
    }

    public static float SoftCap(this float num, float magnitude, float softness) {
        var excess = Math.Max(Math.Abs(num) - magnitude, 0f);
        return num - Math.Sign(num) * excess * (1f - softness);
    }

    public static Vector2 SoftCap(this Vector2 vec, float magnitude, float softness)
        => vec.SafeNormalize() * SoftCap(vec.Length(), magnitude, softness);

    public static IEnumerator<T> GetEnumerator<T>(this IEnumerator<T> enumerator) => enumerator;

}

internal static class GravityHelperContainmentChamber {

    internal static bool shouldInvert(Entity e) => e.Get<GravityComponent>() is GravityComponent g && g.ShouldInvert;
    internal static void setInverted(Entity e, bool inverted) => e.Get<GravityComponent>()!.SetGravity(inverted ? GravityType.Inverted : GravityType.Normal);
    internal static Component makeGravityComponent() => new GravityComponent();

}