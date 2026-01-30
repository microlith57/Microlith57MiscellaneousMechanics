using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;

namespace Celeste.Mod.Microlith57Misc.Entities;

[Tracked]
[CustomEntity("Microlith57Misc/PressureSensor")]
public sealed class PressureSensor : Entity {

    public Color InactiveColor;
    public Color ActiveColor;
    public float EaseSpeed = 5f;
    public float Ease { get; internal set; } = 0f;

    public bool Pressed;

    public string Flag;

    public PressureSensor(EntityData data, Vector2 offset) : base(data.Position + offset) {
        Collider = new Hitbox(data.Width, 1f, 0f, -1f);
        Flag = data.Attr("flag");
    }

    public override void Update() {
        base.Update();

        if (Scene is not Level level) return;

        bool wasPressed = Pressed;
        Pressed = CollideCheck<Actor>();
        Ease = Calc.Approach(Ease, Pressed ? 0f : 1f, EaseSpeed * Engine.DeltaTime);

        level.Session.SetFlag(Flag, Pressed);

        if (wasPressed && !Pressed)
            Audio.Play("event:/game/04_cliffside/arrowblock_side_depress", Center);
        if (!wasPressed && Pressed)
            Audio.Play("event:/game/04_cliffside/arrowblock_side_release", Center);
    }

    public override void Render()
    {
        base.Render();

        Vector2 offset = Pressed ? Vector2.Zero : new(0, -1);
        Color col = Color.Lerp(InactiveColor, ActiveColor, Ease);

        Draw.Line(Position + offset, Position + offset + new Vector2(Width, 0), col);
    }

}
