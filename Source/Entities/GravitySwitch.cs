using Celeste.Mod.Entities;
using Celeste.Mod.GravityHelper.Components;
using Microsoft.Xna.Framework;
using Monocle;

using Celeste.Mod.GravityHelper;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity("Microlith57Misc/GravitySwitch")]
[Tracked]
public class GravitySwitch : Entity {

    public float Cooldown { get; private set; }
    public GravityType GravityType { get; }

    private Sprite Sprite;

    private float cooldownRemaining;
    private bool playSounds;

    private bool usable => true;

    public GravitySwitch(EntityData data, Vector2 offset)
        : base(data.Position + offset) {

        Depth = Depths.Below;

        GravityType = data.Enum("gravityType", GravityType.Toggle);
        Cooldown = data.Float("cooldown", 1f);

        Collider = new Hitbox(16f, 24f, -8f, -12f);

        Add(new HoldableCollider(OnHoldable));
        Add(new PlayerCollider(OnPlayer));
        Add(new PlayerGravityListener(OnGravityChanged));
        Add(Sprite = GFX.SpriteBank.Create(GravityType == GravityType.Toggle ? "gravitySwitchToggle" : "gravitySwitch"));
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        UpdateSprite(animate: false);
    }

    private void UpdateSprite(bool animate) {
        var inverted = Scene.Tracker.GetEntity<Player>()?.Get<GravityComponent>()?.ShouldInvert ?? false;
        var key = inverted ? "up" : "down";

        if (animate) {
            if (playSounds)
                Audio.Play(inverted ? "event:/game/09_core/switch_to_cold" : "event:/game/09_core/switch_to_hot", Position);
            if (usable)
                Sprite.Play(key);
            else {
                if (playSounds)
                    Audio.Play("event:/game/09_core/switch_dies", Position);
                Sprite.Play($"{key}Off");
            }
        } else if (usable)
            Sprite.Play($"{key}Loop");
        else
            Sprite.Play($"{key}OffLoop");

        playSounds = false;
    }

    private void OnHoldable(Holdable holdable) {
        if (!holdable.IsHeld)
            trigger(holdable.Entity);
    }

    private void OnPlayer(Player player) => trigger(player);

    private void trigger(Entity entity) {
        if (!usable || cooldownRemaining > 0)
            return;

        playSounds = true;
        cooldownRemaining = Cooldown;

        var inverted = Scene.Tracker.GetEntity<Player>()?.Get<GravityComponent>()?.ShouldInvert ?? false;

        foreach (GravityComponent component in Scene.Tracker.GetComponents<GravityComponent>())
            component.SetGravity(inverted ? GravityType.Normal : GravityType.Inverted);

        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
        SceneAs<Level>().Flash(Color.White * 0.15f, true);
        Celeste.Freeze(0.05f);
    }

    private void OnGravityChanged(Entity entity, GravityChangeArgs args) => UpdateSprite(args.Changed);

    public override void Update() {
        base.Update();
        if (cooldownRemaining > 0)
            cooldownRemaining -= Engine.DeltaTime;
    }

}
