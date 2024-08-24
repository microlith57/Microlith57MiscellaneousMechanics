using System;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Magnetism;

public class Monopole(Vector2 offset, float strength) : MagnetComponent(offset, strength) {

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

[CustomEntity("Microlith57Misc/DipoleMagnet")]
[Tracked]
public class MonopoleMagnet : Entity {

    #region --- Init ---

    public MonopoleMagnet(Vector2 position,
                  Polarity polarity,
                  float radius, float strength,
                  int particles) : base(position) {

        Depth = Depths.Below;

        switch (polarity) {
            case Polarity.MonopolePlus:
                Add(new Monopole(Collider.TopCenter, strength));
                break;
            case Polarity.MonopoleMinus:
                Add(new Monopole(Collider.TopCenter, -strength));
                break;
            default:
                throw new Exception("monopole magnets cannot be dipoles!");
        }

    }

    public MonopoleMagnet(EntityData data, Vector2 offset)
    : this(
        data.Position + offset,
        data.Enum<Polarity>("polarity"),
        data.Float("radius", 64f),
        data.Float("strength", 100f),
        data.Int("particles", 32)
    ) { }

    #endregion Init

}