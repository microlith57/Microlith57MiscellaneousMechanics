using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity("Microlith57Misc/PlayerGroundedFlagController")]
public sealed class PlayerGroundedFlagController : Entity {

    public string Flag;
    public bool InvertFlag;

    public PlayerGroundedFlagController(
        EntityData data, Vector2 offset
    ) : base(data.Position + offset) {
        this.SetDepthAndTags(data);
        Flag = data.Attr("flag", "playerGrounded");
        InvertFlag = data.Bool("invertFlag");
    }

    private bool Grounded => Scene.Tracker.GetEntity<Player>()?.OnGround() ?? false;

    public override void Update() {
        base.Update();
        (Scene as Level)!.Session.SetFlag(Flag, Grounded ^ InvertFlag);
    }

}
