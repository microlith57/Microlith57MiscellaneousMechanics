using System;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Magnetism;

[CustomEntity("Microlith57Misc/DipoleMagnet")]
[Tracked]
public class DipoleMagnet : Entity {

    #region --- Init ---

    public DipoleMagnet(Vector2 position,
                  float width, float height,
                  Polarity polarity,
                  float radius, float strength,
                  int particles) : base(position) {

        Depth = Depths.Below;
        Collider = new Hitbox(width, height);

        switch (polarity) {
            case Polarity.MonopolePlus:
            case Polarity.MonopoleMinus:
                throw new Exception("dipole magnets cannot be monopoles!");
            case Polarity.Up:
                Add(new MagnetPole(Collider.TopCenter, radius, strength));
                Add(new MagnetPole(Collider.BottomCenter, radius, -strength));
                break;
            case Polarity.Down:
                Add(new MagnetPole(Collider.TopCenter, radius, -strength));
                Add(new MagnetPole(Collider.BottomCenter, radius, strength));
                break;
            case Polarity.Left:
                Add(new MagnetPole(Collider.CenterLeft, radius, strength));
                Add(new MagnetPole(Collider.CenterRight, radius, -strength));
                break;
            case Polarity.Right:
                Add(new MagnetPole(Collider.CenterLeft, radius, -strength));
                Add(new MagnetPole(Collider.CenterRight, radius, strength));
                break;
        }

    }

    public DipoleMagnet(EntityData data, Vector2 offset)
    : this(
        data.Position + offset,
        data.Width, data.Height,
        data.Enum<Polarity>("polarity"),
        data.Float("radius", 64f),
        data.Float("strength", 100f),
        data.Int("particles", 32)
    ) { }

    #endregion Init
    #region --- Rendering ---

    public override void Render() {
        base.Render();
        // todo
        Draw.HollowRect(Collider, Color.White);
    }

    #endregion Rendering

}