using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity("Microlith57Misc/PlayerGroundedFlagController")]
public sealed class PlayerGroundedFlagController(EntityData data, Vector2 offset) : Entity(data.Position + offset) {

    public string Flag = data.Attr("flag", "playerGrounded");
    public bool InvertFlag = data.Bool("invertFlag");

    private bool Grounded => Scene.Tracker.GetEntity<Player>()?.OnGround() ?? false;

    public override void Update() {
        base.Update();
        (Scene as Level)!.Session.SetFlag(Flag, Grounded ^ InvertFlag);
    }

}
