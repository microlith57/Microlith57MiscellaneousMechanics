using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc;

public static partial class Utils {

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

    public static Color HSVToColor(float h, float s, float v, AngleFormat angleFormat = AngleFormat.ZeroToOne) {
        if (s == 0)
            return new Color(v, v, v);

        float hue = UnformatAngle(h, angleFormat);

        if (hue == 1f) hue = 0f;
        else hue *= 6f;

        int i = (int)Math.Truncate(hue);
        float f = hue - i;

        float p = v * (1f - s);
        float q = v * (1f - (s * f));
        float t = v * (1f - (s * (1f - f)));

        switch (i) {
            case 0: return new(v, t, p);
            case 1: return new(q, v, p);
            case 2: return new(p, v, t);
            case 3: return new(p, q, v);
            case 4: return new(t, p, v);
            default: return new(v, p, q);
        }
    }

    public static (float h, float s, float v) ToHSV(this Color color, AngleFormat angleFormat = AngleFormat.ZeroToOne) {
        (float r, float g, float b) = (color.R / 255f, color.G / 255f, color.B / 255f);

        float h = 0f, s, v;

        float min = Math.Min(Math.Min(r, g), b);
        v = Math.Max(Math.Max(r, g), b);
        float delta = v - min;

        if (v == 0.0) s = 0;
        else s = delta / v;

        if (s != 0) {
            if (r == v)
                h = (g - b) / delta;
            else if (g == v)
                h = 2 + (b - r) / delta;
            else if (b == v)
                h = 4 + (r - g) / delta;

            h *= 60;

            if (h < 0.0)
                h += 360;
        }

        return new (FormatAngle(h, angleFormat), s, v);
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

    public static T SetDepthAndTags<T>(this T self, EntityData data) where T : Entity {
        var depth = data.Int("depth");

        if (data.Attr("tags", null) is string tags && !string.IsNullOrWhiteSpace(tags)) {
            foreach (string tag in tags.Split(',')) {
                string trimmed = tag.Trim();
                try {
                    self.AddTag(BitTag.Get(trimmed));
                } catch (KeyNotFoundException e) {
                    throw new KeyNotFoundException($"Invalid tag '{trimmed}'", e);
                }
            }
        }

        return self;
    }

}
