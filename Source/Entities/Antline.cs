using Microsoft.Xna.Framework;
using Monocle;

using Celeste.Mod.Microlith57Misc.Components;

namespace Celeste.Mod.Microlith57Misc.Entities;

public class Antline : Entity {

    public Vector2[] Points;
    public readonly ConditionSource VisibleSource, ActiveSource;

    public Color InactiveColor, ActiveColor;
    private bool Activated;

    public Antline(
        EntityData data, Vector2 offset,
        ConditionSource visibleSource, ConditionSource activeSource
    ) : base(data.Position + offset) {
        Points = data.NodesWithPosition(offset);

        VisibleSource = visibleSource;
        ActiveSource = activeSource;

        InactiveColor = Calc.HexToColorWithAlpha(data.Attr("inactiveColor"));
        ActiveColor = Calc.HexToColorWithAlpha(data.Attr("activeColor"));
    }

    public override void Update() {
        base.Update();
        if (Scene is not Level level) return;

        Visible = VisibleSource.Value;
        Activated = ActiveSource.Value;
    }

    public override void Render() {
        base.Render();
        for (int i = 1; i < Points.Length; i++)
            Draw.Line(Points[i-1], Points[i], Activated ? ActiveColor : InactiveColor);
    }

}
