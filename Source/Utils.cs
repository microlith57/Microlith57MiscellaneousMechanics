using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.EeveeHelper.Entities;
using Celeste.Mod.GravityHelper;
using Celeste.Mod.GravityHelper.Components;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc;

internal static class Utils {

    private static bool checkedGravityHelper;
    public static bool GravityHelperLoaded {
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
    public static bool HelpingHandLoaded {
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

    private static bool checkedEeveeHelper;
    public static bool EeveeHelperLoaded {
        get {
            if (checkedEeveeHelper || Everest.Loader.DependencyLoaded(new EverestModuleMetadata { Name = "EeveeHelper", Version = new Version(1, 0, 0) })) {
                checkedEeveeHelper = true;
                return true;
            }
            return false;
        }
    }

    public static IEnumerable<(Entity container, Holdable hold, Action<Vector2> speedSetter)> GetEeveeHelperHoldableContainers(this Tracker tracker) => EeveeHelperLoaded ? EeveeHelperContainmentChamber.getHoldableContainers(tracker) : [];

    // public static bool IsHoldableContainer(this Holdable hold) => EeveeHelperLoaded && EeveeHelperContainmentChamber.isHoldableContainer(hold);

    public static float SoftCap(this float num, float magnitude, float softness) {
        var excess = Math.Max(Math.Abs(num) - magnitude, 0f);
        return num - Math.Sign(num) * excess * (1f - softness);
    }

    public static Vector2 SoftCap(this Vector2 vec, float magnitude, float softness)
        => vec.SafeNormalize() * SoftCap(vec.Length(), magnitude, softness);

    public static IEnumerator<T> GetEnumerator<T>(this IEnumerator<T> enumerator) => enumerator;

    public static void Add(this Entity self, Components.Vector2Source source) {
        self.Add(source.X);
        self.Add(source.Y);
    }

    public static void Remove(this Entity self, Components.Vector2Source source) {
        self.Remove(source.X);
        self.Remove(source.Y);
    }

}

internal static class GravityHelperContainmentChamber {

    internal static bool shouldInvert(Entity e) => e.Get<GravityComponent>() is GravityComponent g && g.ShouldInvert;
    internal static void setInverted(Entity e, bool inverted) => e.Get<GravityComponent>()!.SetGravity(inverted ? GravityType.Inverted : GravityType.Normal);
    internal static Component makeGravityComponent() => new GravityComponent();

}

internal static class EeveeHelperContainmentChamber {

    internal static IEnumerable<(Entity container, Holdable hold, Action<Vector2> speedSetter)> getHoldableContainers(Tracker tracker)
        => (
            from e in tracker.GetEntities<HoldableContainer>()
            let c = e as HoldableContainer
            where c != null
            select ((Entity)c, c.Hold, (Action<Vector2>)((speed) => c.Speed = speed))
        );

}