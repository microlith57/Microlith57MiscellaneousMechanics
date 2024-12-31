using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.EeveeHelper.Entities;
using Celeste.Mod.GravityHelper;
using Celeste.Mod.GravityHelper.Components;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc;

public static class Utils {

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
        LevelEnter.ErrorMessage = "{big}Oops!{/big}{n}To use {# F94A4A}" + erroringEntity + "{#}, you need to have {# d678db}Gravity Helper{#} installed!";
        var session = Engine.Scene is Level level ? new Session(level.Session.Area) : new Session();
        LevelEnter.Go(session, fromSaveData: false);

        return false;
    }

    internal static bool ShouldInvert(this Entity e) => GravityHelperLoaded && GravityHelperContainmentChamber.shouldInvert(e);

    internal static void SetInverted(this Entity e, bool inverted, string entityNameJustInCaseHahaNoReason) {
        if (!GravityHelperLoaded) {
            if (inverted) {
                CheckGravityHelper(entityNameJustInCaseHahaNoReason);
            }
        } else {
            GravityHelperContainmentChamber.setInverted(e, inverted);
        }
    }

    internal static Component? GravityComponentIfExists() => GravityHelperLoaded ? GravityHelperContainmentChamber.makeGravityComponent() : null;

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
        LevelEnter.ErrorMessage = "{big}Oops!{/big}{n}To use {# F94A4A}" + erroringEntity + "{#}, you need to have {# d678db}Maddie's Helping Hand{#} installed!";
        var session = Engine.Scene is Level level ? new Session(level.Session.Area) : new Session();
        LevelEnter.Go(session, fromSaveData: false);

        return false;
    }

    private static bool checkedEeveeHelper;
    internal static bool EeveeHelperLoaded {
        get {
            if (checkedEeveeHelper || Everest.Loader.DependencyLoaded(new EverestModuleMetadata { Name = "EeveeHelper", Version = new Version(1, 0, 0) })) {
                checkedEeveeHelper = true;
                return true;
            }
            return false;
        }
    }

    internal static IEnumerable<(Entity container, Holdable hold, Action<Vector2> speedSetter)> GetEeveeHelperHoldableContainers(this Tracker tracker) => EeveeHelperLoaded ? EeveeHelperContainmentChamber.getHoldableContainers(tracker) : [];

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

    public enum AngleFormat {
        ZeroToOne,
        Radians,
        Degrees
    }

    public static Color HSLToColor(float h, float s, float l, AngleFormat angleFormat = AngleFormat.ZeroToOne) {
        if (s == 0)
            return new Color(l, l, l);

        float hue = UnformatAngle(h, angleFormat);
        float v2 = (l < 0.5) ? (l * (1 + s)) : (l + s - (l * s));
        float v1 = 2 * l - v2;

        return new Color(
            HueToRGB(v1, v2, hue + (1.0f / 3)),
            HueToRGB(v1, v2, hue),
            HueToRGB(v1, v2, hue - (1.0f / 3))
        );
    }

    private static float HueToRGB(float v1, float v2, float vH) {
        if (vH < 0) vH += 1;
        if (vH > 1) vH -= 1;
        if ((6 * vH) < 1) return v1 + (v2 - v1) * 6 * vH;
        if ((2 * vH) < 1) return v2;
        if ((3 * vH) < 2) return v1 + (v2 - v1) * ((2.0f / 3) - vH) * 6;

        return v1;
    }

    public static (float h, float s, float l) ToHSL(this Color color, AngleFormat angleFormat = AngleFormat.ZeroToOne) {
        float r = color.R / 255.0f;
        float g = color.G / 255.0f;
        float b = color.B / 255.0f;

        float min = Math.Min(Math.Min(r, g), b);
        float max = Math.Max(Math.Max(r, g), b);
        float delta = max - min;

        float l = (max + min) / 2;

        if (delta == 0)
            return (0, 0, l);

        float s = (l <= 0.5) ? (delta / (max + min)) : (delta / (2 - max - min));
        float hue;

        if (r == max)
            hue = (g - b) / 6 / delta;
        else if (g == max)
            hue = (1.0f / 3) + (b - r) / 6 / delta;
        else
            hue = (2.0f / 3) + (r - g) / 6 / delta;

        if (hue < 0)
            hue += 1;
        if (hue > 1)
            hue -= 1;

        return (FormatAngle(hue, angleFormat), s, l);
    }

    public static float FormatAngle(float angle, AngleFormat angleFormat) {
        switch (angleFormat) {
            case AngleFormat.Radians: return angle * Calc.HalfCircle;
            case AngleFormat.Degrees: return angle * 360f;
            default: return angle;
        }
    }

    public static float UnformatAngle(float angle, AngleFormat angleFormat) {
        switch (angleFormat) {
            case AngleFormat.Radians: return angle / Calc.HalfCircle;
            case AngleFormat.Degrees: return angle / 360f;
            default: return angle;
        }
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