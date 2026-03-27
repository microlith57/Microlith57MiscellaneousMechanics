namespace Celeste.Mod.Microlith57Misc.Entities;

public sealed class ForcefieldDoor : Solid {
    private sealed class BounceHook : Component {

        private List<ForcefieldDoor> preUpdatedThisFrame = [];

        public static void Add(Actor actor) {
            if (actor.Get<BounceHook>() is null)
                actor.Add(new BounceHook());
        }

        private BounceHook() : base(false, false) {}

        public override void Added(Entity entity) {
            if (entity is not Actor) throw new Exception("ForcefieldDoor.BounceHook can only be added to Actors");
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
            if (entity.Scene is not Level level || entity.Get<BounceHook>() is not {} hook) return;
            hook.preUpdatedThisFrame.Clear();
            foreach (ForcefieldDoor door in entity.CollideAll<ForcefieldDoor>()) {
                door.PreUpdateFor((Actor)entity);
                hook.preUpdatedThisFrame.Add(door);
            }
        }

        private static void PostUpdate(Entity entity) {
            if (entity.Scene is not Level level || entity.Get<BounceHook>() is not {} hook) return;
            foreach (ForcefieldDoor door in hook.preUpdatedThisFrame)
                door.PostUpdateFor((Actor)entity);
            hook.preUpdatedThisFrame.Clear();
        }

    }

	// private Sprite sprite;
	// private Wiggler wiggler;

    private bool wasCollidable;

    public float BounceSpeed;

	public ForcefieldDoor(
        EntityData data, Vector2 offset
    ) : base(data.Position + offset, data.Width, data.Height, safe: false) {
        this.ProcessCommonFields(data);

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

    public override void Update() {
        base.Update();
        Collidable = wasCollidable;
    }

    public void Open() {
		if (Collidable) {
			Audio.Play("event:/game/03_resort/forcefield_vanish", Position);
			// sprite.Play("open");
            InstantOpen();
		}
	}

	public void InstantOpen() {
		Collidable = false;
	}

    public void Close() {
		if (!Collidable) {
			Audio.Play("event:/game/03_resort/forcefield_bump", Position);
			// sprite.Play("close");
            InstantClose();
		}
    }

    public void InstantClose() {
        Collidable = true;
        foreach (Actor actor in CollideAll<Actor>())
            if (actor is Player || actor.Get<Holdable>() is not null)
                BounceHook.Add(actor);
    }

	private DashCollisionResults OnDashed(Player player, Vector2 direction) {
		Audio.Play("event:/game/03_resort/forcefield_bump", Position);
		// wiggler.Start();
		return DashCollisionResults.Bounce;
	}

    private void PreUpdateFor(Actor actor) {
        wasCollidable = Collidable;

        var delta = actor.ExactPosition - Center;

        // TODO wide as well as tall

        var sign = Calc.Sign(delta);
        var speed = GetSpeed(actor);
        if ((sign.X > 0) ^ (speed.X > 0f))
            speed.X = 0f;

        var absSpeed = (sign.X > 0) ? speed.X : -speed.X;

        SetSpeed(actor, speed);

        Collidable = false;
    }

    private void PostUpdateFor(Actor actor) => Collidable = wasCollidable;

    private Vector2 GetSpeed(Actor actor) {
        if (actor is Player player)
            return player.Speed;
        else if (actor.Get<Holdable>() is Holdable hold)
            return hold.GetSpeed();
        throw new UnreachableException();
    }

    private void SetSpeed(Actor actor, Vector2 speed) {
        if (actor is Player player)
            player.Speed = speed;
        else if (actor.Get<Holdable>() is Holdable hold)
            hold.SetSpeed(speed);
        throw new UnreachableException();
    }
}
