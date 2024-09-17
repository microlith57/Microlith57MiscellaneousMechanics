using System;
using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.Microlith57Misc.Entities;

[Tracked]
[CustomEntity("Microlith57Misc/PressureSensor")]
public class PressureSensor : Solid {

    #region --- Util ---

    [Tracked]
    public sealed class Activator() : Component(false, false) {
    }

    [Tracked]
    private sealed class Group : Entity {

        public bool Activated = false;
        public readonly string Label;
        public List<PressureSensor> Sensors = [];

        public Group(string label) {
            Label = label;
            Depth = Depths.FakeWalls - 1;
        }
    }

    [Flags]
    public enum ButtonCombination : byte {
        None = 0,

        Top = 1 << 0,
        Left = 1 << 1,
        Right = 1 << 2,
        Bottom = 1 << 3,

        Horizontal = Left | Right,
        Vertical = Top | Bottom,

        All = Top | Left | Right | Bottom,
    }

    #endregion Util
    #region --- State ---

    public readonly ButtonCombination Combination;
    public readonly string Label;
    private readonly List<PressureSensor> Siblings = [];

    public Color InactiveColor;
    public Color ActiveColor;
    public float Ease { get; internal set; } = 0f;

    public ButtonCombination Pressed { get; private set; } = ButtonCombination.None;

    #endregion State
    #region --- Init ---

    public PressureSensor(EntityData data, Vector2 offset)
        : base(data.Position, data.Width, data.Height, true) {

        Depth = Depths.FakeWalls;
        SurfaceSoundIndex = 0;

        Combination = data.Enum("buttons", ButtonCombination.Top);
        Label = data.Attr("label");

        InactiveColor = Calc.HexToColor(data.Attr("inactiveColor", "5FCDE4"));
        ActiveColor = Calc.HexToColor(data.Attr("activeColor", "F141DF"));

    }

    public override void Awake(Scene scene) {
        base.Awake(scene);

        if (Scene.Tracker.GetEntity<Player>() is Player player &&
            !player.Components.Any(c => c is Activator))

            player.Add(new Activator());

        foreach (PressureSensor sensor in Scene.Tracker.GetEntities<PressureSensor>())
            if (Label == sensor.Label)
                Siblings.Add(sensor);
    }

    #endregion Init
    #region --- Behaviour ---

    public override void Update() {
        base.Update();

        var pressed = ButtonCombination.None;

        if ((Combination & ButtonCombination.Top) != ButtonCombination.None && CollideCheckByComponent<Activator>(Position - Vector2.UnitY))
            pressed |= ButtonCombination.Top;
        if ((Combination & ButtonCombination.Left) != ButtonCombination.None && CollideCheckByComponent<Activator>(Position - Vector2.UnitX))
            pressed |= ButtonCombination.Left;
        if ((Combination & ButtonCombination.Right) != ButtonCombination.None && CollideCheckByComponent<Activator>(Position + Vector2.UnitX))
            pressed |= ButtonCombination.Right;
        if ((Combination & ButtonCombination.Bottom) != ButtonCombination.None && CollideCheckByComponent<Activator>(Position + Vector2.UnitY))
            pressed |= ButtonCombination.Bottom;

        var click = (pressed & ~Pressed) != ButtonCombination.None;
        var clack = (Pressed & ~pressed) != ButtonCombination.None;

        Pressed = pressed;

        if (click)
            Audio.Play("event:/game/04_cliffside/arrowblock_side_depress", Center);
        if (clack)
            Audio.Play("event:/game/04_cliffside/arrowblock_side_release", Center);
    }

    #endregion Behaviour
    #region Rendering

    public override void Render() {
        base.Render();

        Draw.Rect(Collider, Color.Black);

        var col = Color.Lerp(InactiveColor, ActiveColor, Ease);

        if ((Combination & ButtonCombination.Top) != ButtonCombination.None)
            Draw.Line(Position + Collider.TopLeft, Position + Collider.TopRight, col);
        if ((Combination & ButtonCombination.Left) != ButtonCombination.None)
            Draw.Line(Position + Collider.TopLeft + Vector2.UnitX, Position + Collider.BottomLeft + Vector2.UnitX, col);
        if ((Combination & ButtonCombination.Right) != ButtonCombination.None)
            Draw.Line(Position + Collider.TopRight, Position + Collider.BottomRight, col);
        if ((Combination & ButtonCombination.Bottom) != ButtonCombination.None)
            Draw.Line(Position + Collider.BottomLeft - Vector2.UnitY, Position + Collider.BottomRight - Vector2.UnitY, col);
    }

    #endregion Rendering

}