using Celeste.Mod.Helpers;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity("Microlith57Misc/CameraDetector")]
public sealed class CameraDetector : Entity {
    private readonly string Flag;
    private readonly float Leniency;

    public CameraDetector(EntityData data, Vector2 offset) : base(data.Position + offset) {
        this.ProcessCommonFields(data);

        Collidable = false;
        Collider = new Hitbox(data.Width, data.Height);
        Flag = data.Attr("flag");
        Leniency = data.Float("leniency", 4f);
    }

    public override void Update() {
        base.Update();
        if (Scene is not Level level || level.Camera is null) return;

        level.Session.SetFlag(Flag, CullHelper.IsRectangleVisible(X, Y, Width, Height, Leniency));
    }
}
