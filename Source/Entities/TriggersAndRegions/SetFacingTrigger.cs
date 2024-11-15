using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity("Microlith57Misc/SetFacingTrigger")]
public sealed class SetFacingTrigger(EntityData data, Vector2 offset) : Trigger(data, offset) {

    public Facings Facing = data.Enum<Facings>("direction");
    public string Flag = data.Attr("flag");
    public bool InvertIfUnset = data.Bool("invertIfUnset");

    public bool Continuous = data.Bool("continuous");

    public override void OnEnter(Player player) {
        base.OnEnter(player);
        SetFacing(player);
    }

    public override void Update() {
        base.Update();

        if (Continuous && PlayerIsInside &&
            (Scene as Level)?.Tracker?.GetEntity<Player>() is Player player)

            SetFacing(player);
    }

    private void SetFacing(Player player) {
        if (player.Scene is not Level level) return;

        if (string.IsNullOrEmpty(Flag) || level.Session.GetFlag(Flag))
            player.Facing = Facing;
        else if (InvertIfUnset)
            player.Facing = (Facing == Facings.Left) ? Facings.Right : Facings.Left;
    }

}