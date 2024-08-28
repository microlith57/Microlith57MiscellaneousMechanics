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
            case Polarity.Up:
                Add(new LinearMonopole(Collider.TopLeft, Collider.TopRight, strength));
                Add(new LinearMonopole(Collider.BottomLeft, Collider.BottomRight, -strength));
                break;
            case Polarity.Down:
                Add(new LinearMonopole(Collider.TopLeft, Collider.TopRight, -strength));
                Add(new LinearMonopole(Collider.BottomLeft, Collider.BottomRight, strength));
                break;
            case Polarity.Left:
                Add(new LinearMonopole(Collider.TopLeft, Collider.BottomLeft, strength));
                Add(new LinearMonopole(Collider.TopRight, Collider.BottomRight, -strength));
                break;
            case Polarity.Right:
                Add(new LinearMonopole(Collider.TopLeft, Collider.BottomLeft, -strength));
                Add(new LinearMonopole(Collider.TopRight, Collider.BottomRight, strength));
                break;
            default:
                throw new Exception("dipole magnets cannot be monopoles!");
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

    private static float biggest_so_far = 0.001f;

    public override void Render() {
        var scaling = 8f / biggest_so_far;

        for (float x = Left - 32f; x <= Right + 32f; x += 8f)
            for (float y = Top - 32f; y <= Bottom + 32f; y += 8f) {
                var pos = new Vector2(x, y);
                var f = Scene.FieldAt(pos);
                var l = f.Length();

                if (biggest_so_far < l)
                    biggest_so_far = l;

                Draw.Line(pos, pos + f * scaling, Color.HotPink);
            }
    }


    #endregion Rendering

}