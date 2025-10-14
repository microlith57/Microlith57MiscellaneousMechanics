#if false

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Components;

public class CircleIndicatorLines : Component {

    public struct Line {
        public float Angle, Radius, Length, Justification;
        public Color Color;
    }

    public delegate Line _AdjustLine(Line line);
    public _AdjustLine? AdjustLine;

    public Vector2 Position;
    public float Radius, TargetDensity, Angle;
    public Color Color;

    public int NumLines { get; private set; }

    public CircleIndicatorLines(Vector2 position, float radius, float density = 3.6f, float angle = 0f) : base(false, true) {
        Position = position;
        Radius = radius;
        TargetDensity = density;
        Angle = angle;

        ResetNumLines();
    }

    public void ResetNumLines()
        => NumLines = (int)(Calc.Circle * Radius / TargetDensity);

    public List<Line> GetLines() {

    }

    public override void Render() {

    }
    
}

#endif
