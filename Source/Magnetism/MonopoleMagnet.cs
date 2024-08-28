using System;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Magnetism;

[CustomEntity("Microlith57Misc/MonopoleMagnet")]
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
                Add(new PointMonopole(Position, strength));
                break;
            case Polarity.MonopoleMinus:
                Add(new PointMonopole(Position, -strength));
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