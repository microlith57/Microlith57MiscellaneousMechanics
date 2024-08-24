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

        foreach (MagnetComponent pole in scene.Tracker.GetComponents<MagnetComponent>())
            field += pole.FieldAt(position);

        return field.SoftCap(1800f, 0f);
    }

}

[Tracked(inherited: true)]
public abstract class MagnetComponent(Vector2 offset, float strength) : Component(true, true) {

    public Vector2 Position = offset;
    public Vector2 AbsPosition => (Entity?.Position ?? Vector2.Zero) + Position;

    public float Strength = strength;

    public abstract Vector2 FieldAt(Vector2 position);

}
