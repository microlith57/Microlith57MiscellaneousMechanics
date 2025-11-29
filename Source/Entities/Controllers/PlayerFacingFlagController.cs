using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity("Microlith57Misc/PlayerFacingFlagController")]
public sealed class PlayerFacingFlagController : Entity {

    public string FlagLeft;
    public string FlagRight;
    public bool PersistOnDeath;

    public PlayerFacingFlagController(
        EntityData data, Vector2 offset
    ) : base(data.Position + offset) {
        this.SetDepthAndTags(data);
        FlagLeft = data.Attr("flagLeft", "playerFacingLeft");
        FlagRight = data.Attr("flagRight", "playerFacingRight");
        PersistOnDeath = data.Bool("persistOnDeath");
    }

    private int Facing => (int)(Scene.Tracker.GetEntity<Player>()?.Facing ?? 0);

    public override void Update() {
        base.Update();

        if (Scene is not Level level) return;

        bool left = false, right = false;

        switch (Facing) {
            case (int)Facings.Left: left = true; break;
            case (int)Facings.Right: right = true; break;
            default: if (PersistOnDeath) return; break;
        }

        if (!string.IsNullOrEmpty(FlagLeft)) level.Session.SetFlag(FlagLeft, left);
        if (!string.IsNullOrEmpty(FlagRight)) level.Session.SetFlag(FlagRight, right);
    }

}
