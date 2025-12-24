using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Entities;

public sealed class ForcefieldDoor : Solid {
    public sealed class BounceInProgress : Component {

        public static void Add(Actor actor) {
            if (actor.Get<BounceInProgress>() is null)
                actor.Add(new BounceInProgress());
        }

        private BounceInProgress() : base(false, false) {}

        public override void Added(Entity entity) {
            base.Added(entity);
            entity.PreUpdate += PreUpdate;
            entity.PostUpdate += PostUpdate;
        }

        public override void Removed(Entity entity) {
            base.Removed(entity);
            entity.PreUpdate -= PreUpdate;
            entity.PostUpdate -= PostUpdate;
        }

        private static void PreUpdate(Entity entity) {
            if (entity.Scene is not Level level) return;
        }

        private static void PostUpdate(Entity entity) {
            if (entity.Scene is not Level level) return;
        }

    }

	// private Sprite sprite;
	// private Wiggler wiggler;

	public ForcefieldDoor(
        EntityData data, Vector2 offset
    ) : base(data.Position + offset, 32f, 32f, safe: false) {
		// Add(sprite = GFX.SpriteBank.Create("ghost_door"));
		// sprite.Position = new Vector2(base.Width, base.Height) / 2f;
		// sprite.Play("idle");
		OnDashCollide = OnDashed;
		// Add(wiggler = Wiggler.Create(0.6f, 3f, (float f) =>
		// {
		// 	sprite.Scale = Vector2.One * (1f - f * 0.2f);
		// }));
		SurfaceSoundIndex = 20;
	}

	public override void Added(Scene scene) {
		base.Added(scene);
	}

	public void Open() {
		if (Collidable) {
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			Audio.Play("event:/game/03_resort/forcefield_vanish", Position);
			// sprite.Play("open");
			Collidable = false;
		}
	}

	public void InstantOpen() {
		Collidable = (Visible = false);
	}

	private DashCollisionResults OnDashed(Player player, Vector2 direction) {
		Audio.Play("event:/game/03_resort/forcefield_bump", Position);
		// wiggler.Start();
		return DashCollisionResults.Bounce;
	}
}
