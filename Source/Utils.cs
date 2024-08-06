using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57.IntContest;

public static class Utils {

    public static float SoftCap(this float num, float magnitude, float softness) {
        var excess = Math.Max(Math.Abs(num) - magnitude, 0f);
        return num - Math.Sign(num) * excess * (1f - softness);
    }

    public static Vector2 SoftCap(this Vector2 vec, float magnitude, float softness)
        => vec.SafeNormalize() * SoftCap(vec.Length(), magnitude, softness);

}