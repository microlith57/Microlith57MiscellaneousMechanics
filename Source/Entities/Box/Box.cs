using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.Entities;
using Celeste.Mod.GravityHelper;
using Celeste.Mod.GravityHelper.Components;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57.IntContest.Entities;

[CustomEntity("Microlith57_IntContest24/Box")]
[Tracked]
public partial class Box : Actor {

    #region --- Util ---

    [Tracked]
    private class Renderer : Entity {

        public Renderer() : base() {
            Depth = 100;
            AddTag(Tags.Persistent);
        }

        public override void Render() {
            base.Render();

            var boxes = (
                from e in Scene.Tracker.GetEntities<Box>()
                let box = (Box)e
                where !box.Hold.IsHeld
                orderby box.LastInteraction
                select box
            ).ToList();

            foreach (var box in boxes)
                box.RenderOutline();

            foreach (var box in boxes)
                box.RenderSprite();
        }

    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public static ParticleType P_Impact;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    #endregion
    #region --- State ---

    internal static bool updatedThisFrame = false;

    public string RemoveIfFlag = "";

    public Vector2 Speed;
    public Holdable Hold;
    public Sprite Sprite;
    public VertexLight Light;
    public BoxSurface Surface;

    public bool Inverted;
    public Hitbox MainCollider, FullSizeCollider, PickupCollider, PickupColliderStacked, ActivatorCollider;

    private Collision onCollideH, onCollideV;

    public float NoGravityTimer;
    public float LastInteraction;

    public bool ShouldShatter, Shattering;

    public bool Shaking;
    public Vector2 ShakeOffset;

    public bool BonkedH, BonkedV;

    private bool IsTutorial;

    #endregion
    #region --- Init ---

    public Box(Vector2 position, Vector2 speed = default) : base(position) {
        Depth = -2;
        Collider = MainCollider = new Hitbox(8f, 10f, -4f, -10f);
        FullSizeCollider = new Hitbox(20f, 20f, -10f, -20f);
        FullSizeCollider.Added(this);

        Add(Sprite = new(GFX.Game, "objects/INTcontest24/microlith57/box/"));
        Sprite.Add("normal", "normal", 1f, [0]);
        Sprite.Add("inverted", "inverted", 1f, [0]);
        Sprite.Play("normal");
        Sprite.CenterOrigin();
        Sprite.Position = FullSizeCollider.Center;

        PickupCollider = new Hitbox(28f, 28f, -14f, -24f);
        PickupColliderStacked = new Hitbox(28f, 20f, -14f, -16f);
        Add(Hold = new Holdable {
            PickupCollider = PickupCollider,
            SlowFall = false,
            SlowRun = true,
            OnPickup = OnPickup,
            OnRelease = OnRelease,
            DangerousCheck = Dangerous,
            OnHitSpring = HitSpring,
            SpeedGetter = () => Speed,
            SpeedSetter = (value) => Speed = value
        });

        onCollideH = OnCollideH;
        onCollideV = OnCollideV;
        LiftSpeedGraceTime = 0.1f;

        Add(Light = new(FullSizeCollider.Center, Color.White, 1f, 24, 48));

        ActivatorCollider = new Hitbox(20f, 20f, -10f, -20f);
        ActivatorCollider.Added(this);
        Add(new AreaSwitch.Activator() { Collider = ActivatorCollider });

        Add(Surface = new BoxSurface(
            FullSizeCollider,
            width: 20,
            depth: 101,
            surfaceIndex: SurfaceIndex.Girder
        ));

        Add(new TransitionListener() {
            OnOutBegin = () => {
                ShouldShatter = true;
                Shaking = true;
            },
            OnOut = _ => UpdateShake()
        });
    }

    public Box(EntityData data, Vector2 offset)
        : this(data.Position + offset,
               new(data.Float("speedX"), data.Float("speedY"))) {
        RemoveIfFlag = data.Attr("removeIfFlag");
        IsTutorial = data.Bool("tutorial");
    }

    public override void Added(Scene scene) {
        base.Added(scene);

        LastInteraction = Scene.TimeActive;

        if (!string.IsNullOrEmpty(RemoveIfFlag) && (Scene as Level)!.Session.GetFlag(RemoveIfFlag)) {
            RemoveSelf();
            return;
        }

        if (IsTutorial && !(Scene as Level)!.Session.GetFlag("pickup_box_tutorial_done"))
            Add(new TutorialPrompt());
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);

        if (Scene.Tracker.GetEntity<Renderer>() == null)
            Scene.Add(new Renderer());

        var grav = Get<GravityComponent>();
        grav.UpdatePosition = OnGravityChange_Position;
        grav.UpdateColliders = OnGravityChange_Colliders;
        grav.UpdateVisuals = OnGravityChange_Visuals;
    }

    #endregion
    #region --- Behaviour ---

    public override void Update() {
        base.Update();

        if (ShouldShatter) {
            Shatter();
            return;
        }

        if (!updatedThisFrame)
            UpdateAll((Level)Scene);

        Hold.CheckAgainstColliders();

        UpdateShake();
    }

    private static void UpdateAll(Level level) {
        updatedThisFrame = true;

        List<Entity> boxes = [.. level.Tracker.GetEntities<Box>()];

        List<Entity> normal = [.. boxes.Where(box => !((Box)box).Inverted).OrderByDescending(box => box.Y)];
        List<Entity> inverted = [.. boxes.Where(box => ((Box)box).Inverted).OrderBy(box => box.Y)];

        foreach (Box box in boxes) {
            box.BonkedH = box.BonkedV = false;
            box.Hold.PickupCollider = box.PickupCollider;

            box.Collidable = !box.Inverted;
            box.Surface.CollidableTop = !box.Inverted;
            box.Surface.CollidableBot = false;
        }

        foreach (Box box in normal) {
            box.UpdatePhysics();
            box.Surface.Move();
        }

        foreach (Box box in boxes) {
            box.Collidable = box.Inverted;
            box.Surface.CollidableTop = false;
            box.Surface.CollidableBot = box.Inverted;
        }

        foreach (Box box in inverted) {
            box.UpdatePhysics();
            box.Surface.Move();
        }

        foreach (Box box in boxes) {
            box.Collidable = true;
            box.Surface.CollidableTop = true;
            box.Surface.CollidableBot = true;
        }
    }

    #region > Physics

    public void UpdatePhysics() {
        var level = (Level)Scene;

        if (Shattering) {
            Surface.Collidable = false;
            Hold.cannotHoldTimer = 0.1f;
            return;
        }

        if (Hold.IsHeld)
            return;

        if (OnGround())
            UpdatePhysics_OnGround();
        else if (Hold.ShouldHaveGravity)
            UpdatePhysics_InAir();

        MoveH(Speed.X * Engine.DeltaTime, onCollideH);
        MoveV(Speed.Y * Engine.DeltaTime, onCollideV);

        // aaaaaaaaaa
        Position = Position.Round();

        UpdatePhysics_ClampBounds();
        UpdatePhysics_DetectStack();
    }

    private void UpdatePhysics_OnGround() {
        var pitLeft = !OnGround(Position - Vector2.UnitX * 3f);
        var pitRight = !OnGround(Position + Vector2.UnitX * 3f);

        var wallLeft = CollideCheck<Solid>(Position - 4f * Vector2.UnitX);
        var wallRight = CollideCheck<Solid>(Position + 4f * Vector2.UnitX);

        var target = 0f;
        if ((pitRight && !pitLeft) || (wallLeft && !wallRight))
            target = 20f;
        else if ((pitLeft && !pitRight) || (wallRight && !wallLeft))
            target = -20f;

        Speed.X = Calc.Approach(Speed.X, target, 800f * Engine.DeltaTime);
    }

    private void UpdatePhysics_InAir() {
        var gravityRate = 800f;
        if (Math.Abs(Speed.Y) <= 30f)
            gravityRate *= 0.5f;

        var airResistance = 350f;
        if (Speed.Y < 0f)
            airResistance *= 0.5f;

        Speed.X = Calc.Approach(Speed.X, 0f, airResistance * Engine.DeltaTime);

        if (NoGravityTimer > 0f)
            NoGravityTimer -= Engine.DeltaTime;
        else
            Speed.Y = Calc.Approach(Speed.Y, 200f, gravityRate * Engine.DeltaTime);
    }

    private void UpdatePhysics_ClampBounds() {
        var level = (Level)Scene;

        if (Right > level.Bounds.Right) {
            Right = level.Bounds.Right;
            Speed.X *= -0.4f;
        } else if (Left < level.Bounds.Left) {
            Left = level.Bounds.Left;
            Speed.X *= -0.4f;
        }

        if (!Inverted) {
            if (Bottom > level.Bounds.Bottom + 4) {
                Bottom = level.Bounds.Bottom - 4;
                Speed.Y = 0f;
            } else if (Bottom < level.Bounds.Top - 48f)
                Die();
        } else {
            if (Top < level.Bounds.Top - 4) {
                Top = level.Bounds.Top + 4;
                Speed.Y = 0f;
            } else if (Top > level.Bounds.Bottom + 48f)
                Die();
        }
    }

    private void UpdatePhysics_DetectStack() {
        foreach (var surf in Scene.Tracker.GetComponents<BoxSurface.BelongsToBox>()) {
            if (surf is BoxSurface.BelongsToBox bb &&
                bb.Entity.Collidable &&
                bb.Surface is BoxSurface bs &&
                bs.Collidable &&
                bs.Entity is Box box &&
                box != this &&
                (!Inverted ? bb.IsTop : bb.IsBot) &&
                box.FullSizeCollider.Intersects(
                    Position.X - 8f, Position.Y - 4f,
                    16f, 8f
                )
            )
                box.Hold.PickupCollider = box.PickupColliderStacked;
        }
    }

    #endregion
    #region > Actor/Holdable

    public void ExplodeLaunch(Vector2 from) {
        if (Hold.IsHeld)
            return;

        Speed = (Center - from).SafeNormalize(120f);
        SlashFx.Burst(Center, Speed.Angle());
    }

    public bool Dangerous(HoldableCollider holdableCollider) => !Hold.IsHeld && Speed != Vector2.Zero;

    public bool HitSpring(Spring spring) {
        if (Hold.IsHeld)
            return false;

        if (spring.Orientation == Spring.Orientations.Floor && Speed.Y >= 0f) {
            Speed.X *= 0.5f;
            Speed.Y = -160f;
            NoGravityTimer = 0.15f;
            return true;
        }
        if (spring.Orientation == Spring.Orientations.WallLeft && Speed.X <= 0f) {
            MoveTowardsY(spring.CenterY + 5f, 4f);
            Speed.X = 220f;
            Speed.Y = -80f;
            NoGravityTimer = 0.1f;
            return true;
        }
        if (spring.Orientation == Spring.Orientations.WallRight && Speed.X >= 0f) {
            MoveTowardsY(spring.CenterY + 5f, 4f);
            Speed.X = -220f;
            Speed.Y = -80f;
            NoGravityTimer = 0.1f;
            return true;
        }

        return false;
    }

    private void OnCollideH(CollisionData data) {
        if (data.Hit is DashSwitch @switch)
            @switch.OnDashCollide(null, Vector2.UnitX * Math.Sign(Speed.X));

        BonkedH = true;
        Audio.Play("event:/char/madeline/grab", Position, "surface_index", SurfaceIndex.Girder);

        if (Math.Abs(Speed.X) > 100f)
            ImpactParticles(data.Direction);

        Speed.X *= -0.4f;
    }

    private void OnCollideV(CollisionData data) {
        if (data.Hit is DashSwitch @switch)
            @switch.OnDashCollide(null, Vector2.UnitY * Math.Sign(Speed.Y));

        if (Speed.Y > 0f) {
            BonkedV = true;
            Audio.Play("event:/char/madeline/landing", Position, "surface_index", SurfaceIndex.Girder);
        }

        if (Speed.Y > 160f)
            ImpactParticles(data.Direction);

        if (Speed.Y > 140f && !(data.Hit is SwapBlock or DashSwitch))
            Speed.Y *= -0.6f;
        else
            Speed.Y = 0f;
    }

    private void ImpactParticles(Vector2 dir) {
        if (Scene is not Level level)
            return;

        float direction;
        Vector2 position;
        Vector2 positionRange;
        if (dir.X > 0f) {
            direction = (float)Math.PI;
            position = new Vector2(Right, Y - 4f);
            positionRange = Vector2.UnitY * 6f;
        } else if (dir.X < 0f) {
            direction = 0f;
            position = new Vector2(Left, Y - 4f);
            positionRange = Vector2.UnitY * 6f;
        } else if (dir.Y > 0f) {
            direction = -(float)Math.PI / 2f;
            position = new Vector2(X, Bottom);
            positionRange = Vector2.UnitX * 6f;
        } else {
            direction = (float)Math.PI / 2f;
            position = new Vector2(X, Top);
            positionRange = Vector2.UnitX * 6f;
        }

        level.Particles.Emit(P_Impact, 12, position, positionRange, direction);
    }

    public override bool IsRiding(Solid solid) {
        if (Speed.Y != 0f)
            return false;

        return base.IsRiding(solid);
    }

    public override void OnSquish(CollisionData data) {
        if (!TrySquishWiggle(data))
            Die();
    }

    private void OnPickup() {
        var holder = Hold.Holder;
        holder.Speed = holder.Speed.SoftCap(80f, 0.4f);

        Speed = Vector2.Zero;
        AddTag(Tags.Persistent);
    }

    private void OnRelease(Vector2 dir) {
        RemoveTag(Tags.Persistent);

        if (dir.X != 0f && dir.Y == 0f)
            dir.Y = -0.4f;
        Speed = dir * 100f;
        if (Speed != Vector2.Zero)
            NoGravityTimer = 0.05f;

        LastInteraction = Scene.TimeActive;
    }

    #endregion
    #region > Gravity

    private void OnGravityChange_Position(GravityChangeArgs args) {
        if (!args.Changed) return;

        Position.Y = args.NewValue == GravityType.Inverted
                ? FullSizeCollider.AbsoluteTop
                : FullSizeCollider.AbsoluteBottom;
    }

    private void OnGravityChange_Colliders(GravityChangeArgs args) {
        if (!args.Changed) return;

        if (args.NewValue != GravityType.Inverted) {
            Inverted = false;

            MainCollider.Position = new(-4f, -10f);
            FullSizeCollider.Position = new(-10f, -20f);
            PickupCollider.Position = new(-14f, -24f);
            PickupColliderStacked.Position = new(-14f, -16f);
            ActivatorCollider.Position = new(-10f, -20f);
        } else {
            Inverted = true;

            MainCollider.Position = new(-4f, 0f);
            FullSizeCollider.Position = new(-10f, 0f);
            PickupCollider.Position = new(-14f, -4f);
            PickupColliderStacked.Position = new(-14f, -4f);
            ActivatorCollider.Position = new(-10f, 0f);
        }
    }

    private void OnGravityChange_Visuals(GravityChangeArgs args) {
        Sprite.Play(args.NewValue == GravityType.Inverted ? "inverted" : "normal");
        Sprite.Position = Light.Position = FullSizeCollider.Center;
    }

    #endregion
    #region > Destruction

    public void Shatter() {
        Shattering = true;
        var center = Position + new Vector2(0f, -10f);
        Audio.Play("event:/game/general/wall_break_stone", center);

        for (var i = 0; i < 7; i++) {
            var pos = Calc.Round(new(Calc.Random.NextFloat(12f) + 4f, Calc.Random.NextFloat(12f) + 4f));
            var debris = new Debris().orig_Init(TopLeft + pos, '1').BlastFrom(center);
            debris.image.Texture = GFX.Game["debris/7"];
            Scene.Add(debris);
        }

        RemoveSelf();
    }

    public void Die() {
        RemoveSelf();
    }

    #endregion

    public void UpdateShake() {
        if (Shaking) {
            if (!Scene.OnInterval(0.04f))
                return;

            ShakeOffset = Calc.Random.ShakeVector();
        } else {
            ShakeOffset = Vector2.Zero;
        }
    }

    #endregion
    #region --- Rendering ---

    public override void Render() {
        if (!Hold.IsHeld) return;

        RenderOutline();
        RenderSprite();
    }

    public void RenderSprite() {
        Sprite.Texture.Draw(Sprite.RenderPosition + ShakeOffset, Sprite.Origin, Sprite.Color, Sprite.Scale, Sprite.Rotation, Sprite.Effects);
    }

    public void RenderOutline() {
        Sprite.Texture.Draw(Sprite.RenderPosition + ShakeOffset + new Vector2( 1f,  1f), Sprite.Origin, Color.Black, Sprite.Scale, Sprite.Rotation, Sprite.Effects);
        Sprite.Texture.Draw(Sprite.RenderPosition + ShakeOffset + new Vector2( 1f, -1f), Sprite.Origin, Color.Black, Sprite.Scale, Sprite.Rotation, Sprite.Effects);
        Sprite.Texture.Draw(Sprite.RenderPosition + ShakeOffset + new Vector2(-1f,  1f), Sprite.Origin, Color.Black, Sprite.Scale, Sprite.Rotation, Sprite.Effects);
        Sprite.Texture.Draw(Sprite.RenderPosition + ShakeOffset + new Vector2(-1f, -1f), Sprite.Origin, Color.Black, Sprite.Scale, Sprite.Rotation, Sprite.Effects);
    }

    #endregion

}
