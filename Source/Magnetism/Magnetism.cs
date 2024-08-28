using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Magnetism;

public enum Polarity {
    MonopolePlus,
    MonopoleMinus,
    Up,
    Down,
    Left,
    Right
}

public static partial class Utils {

    public static Vector2 FieldAt(this Scene scene, Vector2 position) {
        var field = Vector2.Zero;

        foreach (Monopole pole in scene.Tracker.GetComponents<Monopole>())
            field += pole.FieldAt(position);

        return field.SoftCap(1800f, 0f);
    }

}

[Tracked(inherited: true)]
public abstract class Monopole(float strength) : Component(true, true) {

    public float Strength = strength;

    public abstract Vector2 FieldAt(Vector2 position);

}

public class PointMonopole(Vector2 offset, float strength) : Monopole(strength) {

    public Vector2 Position = offset;
    public Vector2 AbsPosition => (Entity?.Position ?? Vector2.Zero) + Position;

    public override Vector2 FieldAt(Vector2 position) {
        var delta = position - AbsPosition;

        if (delta == Vector2.Zero)
            return Vector2.Zero;

        return delta.SafeNormalize() * Strength / delta.LengthSquared();
    }

    public override void DebugRender(Camera camera) {
        Draw.Circle(AbsPosition, 16f, Strength > 0 ? Color.Red : Color.Blue, 8);
    }

}

public class LinearMonopole : Monopole {

    public readonly Vector2 Start;
    public readonly Vector2 End;

    public Vector2 AbsStart => (Entity?.Position ?? Vector2.Zero) + Start;
    public Vector2 AbsEnd => (Entity?.Position ?? Vector2.Zero) + End;
    public Vector2 Delta => End - Start;

    private readonly float Δx, Δy;

    public LinearMonopole(Vector2 offset_start, Vector2 offset_end, float strength) : base(strength) {
        Start = offset_start;
        End = offset_end;

        // Δx = Delta.X;
        // Δy = Delta.Y;
    }

    public override Vector2 FieldAt(Vector2 position) {
        // var A = AbsStart - position;
        // var B = AbsEnd - position;

        // var ax2 = (float)Math.Pow(A.X, 2);
        // var ay2 = (float)Math.Pow(A.Y, 2);

        // var ax3 = (float)Math.Pow(A.X, 3);
        // var ay3 = (float)Math.Pow(A.Y, 3);

        // var a_len2 = ax2 + ay2;
        // var a_len = (float)Math.Sqrt(a_len2);

        // var b_len = B.Length();

        // var denom = b_len * a_len2 * (A.X * Δy - A.Y * Δx);

        // if (denom == 0f)
        //     return Vector2.Zero;

        // return new(
        //     Strength * (float)(-A.Y * b_len * a_len - a_len2 * Δy - ay3 - ax2 * A.Y) / denom,
        //     Strength * (float)(A.X * b_len * a_len - a_len2 * Δx - ax3 - ay2 * A.X) / denom
        // );

        var length = Delta.Length();
        var steps = (int)Math.Ceiling(length / 4f);
        var strength = Strength / steps;

        var res = Vector2.Zero;

        for (int i = 0; i < steps; i++) {
            var delta = position - (AbsStart + Delta / steps);

            if (delta == Vector2.Zero)
                return Vector2.Zero;

            res += delta.SafeNormalize() * strength / delta.LengthSquared();
        }

        return res;
    }

    public override void DebugRender(Camera camera) {
        Draw.Line(AbsStart, AbsEnd, Strength > 0 ? Color.Red : Color.Blue, 2f);
    }

}
