using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

using Celeste.Mod.GravityHelper;
using Celeste.Mod.GravityHelper.Components;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity("Microlith57Misc/Box")]
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

    #endregion Util
    #region --- State ---

    internal static bool updatedThisFrame = false;

    public readonly string RemoveIfFlag = "";
    public readonly bool GravityLocked = true;
    public readonly bool IsTutorial = false;

    public readonly Hitbox MainCollider, FullSizeCollider, PickupCollider, ActivatorCollider;
    public readonly Holdable Hold;
    public readonly BoxSurface Surface;
    public GravityComponent? Gravity { get; private set; }
    public readonly Image BaseSprite;
    public readonly Sprite IndicatorSprite;
    public readonly VertexLight Light;

    private readonly Collision onCollideH, onCollideV;

    public Vector2 Speed;
    public bool PositionInverted;
    public bool Inverted;

    public float NoGravityTimer;
    public float LastInteraction;

    public bool ShouldShatter, Shattering;

    public bool Shaking;
    public Vector2 ShakeOffset;

    public bool BonkedH, BonkedV;

    public Vector2 AbsCenter => Position + (PositionInverted ? new Vector2(0f, 10f) : new Vector2(0f, -10f));

    #endregion State
    #region --- Init ---

    public Box(Vector2 position,
               string removeIfFlag = "",
               bool gravityLocked = false,
               bool isTutorial = false)
            : base(position) {

        Depth = -2;
        RemoveIfFlag = removeIfFlag;
        GravityLocked = gravityLocked;
        IsTutorial = isTutorial;

        Collider = MainCollider = new Hitbox(8f, 10f, -4f, -10f);
        FullSizeCollider = new Hitbox(20f, 20f, -10f, -20f);
        FullSizeCollider.Added(this);

        Add(BaseSprite = new(GFX.Game["objects/microlith57/misc/box/base"]));
        BaseSprite.CenterOrigin();
        BaseSprite.Position = FullSizeCollider.Center;

        var spritePath = "objects/microlith57/misc/box/indicator";
        if (gravityLocked)
            spritePath += "_locked";

        Add(IndicatorSprite = new(GFX.Game, spritePath));
        IndicatorSprite.Add("normal", "", 1f, [0]);
        IndicatorSprite.Add("inverted", "", 1f, [1]);
        IndicatorSprite.Add("shatter", "", 1f, [2]);
        IndicatorSprite.Play("normal");
        IndicatorSprite.CenterOrigin();
        IndicatorSprite.Position = FullSizeCollider.Center;

        PickupCollider = new Hitbox(28f, 28f, -14f, -24f);
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

        Add(Light = new(FullSizeCollider.Center, Color.White * 0.7f, 1f, 24, 48));

        ActivatorCollider = new Hitbox(20f, 20f, -10f, -20f);
        ActivatorCollider.Added(this);
        Add(new AreaSwitch.Activator() { Collider = ActivatorCollider });

        Add(new PressureSensor.Activator());

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

        Add(new PlayerGravityListener() { GravityChanged = OnPlayerChangeGravity });
    }

    public Box(EntityData data, Vector2 offset)
        : this(data.Position + offset,
              data.Attr("removeIfFlag"),
              data.Bool("gravityLocked"),
              data.Bool("tutorial")) {
        Speed = new(data.Float("speedX"), data.Float("speedY"));
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

        Gravity = Get<GravityComponent>() ?? throw new Exception("expected gravityhelper to do its thing, but it didn't! missing a force load gravity controller?");
        Gravity.UpdatePosition = OnGravityChange_Position;
        Gravity.UpdateColliders = OnGravityChange_Colliders;
        Gravity.UpdateVisuals = OnGravityChange_Visuals;

        if (GravityLocked)
            Gravity.Lock();
    }

    #endregion Init
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

    public void UpdateShake() {
        if (Shaking) {
            if (!Scene.OnInterval(0.04f))
                return;

            ShakeOffset = Calc.Random.ShakeVector();
        } else
            ShakeOffset = Vector2.Zero;
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
            if (Top < level.Bounds.Top - 24f) {
                Top = level.Bounds.Top + 24f;
                Speed.Y = 0f;
            } else if (Top > level.Bounds.Bottom + 48f)
                RemoveSelf();
        } else {
            if (Bottom > level.Bounds.Bottom + 24f) {
                Bottom = level.Bounds.Bottom - 24f;
                Speed.Y = 0f;
            } else if (Bottom < level.Bounds.Top - 48f)
                RemoveSelf();
        }
    }

    #endregion Physics
    #region > Actor

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
            Shatter();
    }

    #endregion Actor/Holdable
    #region > Holdable

    private void OnPickup() {
        var holder = Hold.Holder;
        holder.Speed = holder.Speed.SoftCap(80f, 0.4f);

        Speed = Vector2.Zero;
        AddTag(Tags.Persistent);

        if (GravityLocked &&
            holder.Get<GravityComponent>() is GravityComponent playerGrav &&
            playerGrav.ShouldInvert is bool playerInvert &&
            playerInvert != Inverted)

            GravUpdate(playerInvert);
    }

    private void OnRelease(Vector2 dir) {
        RemoveTag(Tags.Persistent);

        if (dir.X != 0f && dir.Y == 0f)
            dir.Y = -0.4f;
        Speed = dir * 100f;
        if (Speed != Vector2.Zero)
            NoGravityTimer = 0.05f;

        LastInteraction = Scene.TimeActive;

        if (GravityLocked)
            GravUpdate(Inverted);
    }

    #endregion Pickup
    #region > Gravity

    private void OnPlayerChangeGravity(Entity player, GravityChangeArgs args) {
        if (!args.Changed || Hold.Holder != player)
            return;

        bool playerInvert = args.NewValue == GravityType.Inverted;
        if (GravityLocked && playerInvert != PositionInverted)
            GravUpdate(playerInvert);
    }

    private void OnGravityChange_Position(GravityChangeArgs args) {
        if (args.Changed) {
            Inverted = args.NewValue == GravityType.Inverted;
            GravUpdatePosition(args.NewValue == GravityType.Inverted);
        }
    }

    private void OnGravityChange_Colliders(GravityChangeArgs args) {
        if (args.Changed)
            GravUpdateColliders(args.NewValue == GravityType.Inverted);
    }

    private void OnGravityChange_Visuals(GravityChangeArgs args) {
        if (args.Changed)
            GravUpdateVisuals(args.NewValue == GravityType.Inverted);
    }

    private void GravUpdate(bool inverted) {
        GravUpdatePosition(inverted);
        GravUpdateColliders(inverted);
        GravUpdateVisuals(inverted);
    }

    private void GravUpdatePosition(bool inverted) {
        PositionInverted = inverted;
        Position.Y = !inverted
                   ? FullSizeCollider.AbsoluteBottom
                   : FullSizeCollider.AbsoluteTop;
    }

    private void GravUpdateColliders(bool inverted) {
        if (!inverted) {
            MainCollider.Position = new(-4f, -10f);
            FullSizeCollider.Position = new(-10f, -20f);
            PickupCollider.Position = new(-14f, -24f);
            ActivatorCollider.Position = new(-10f, -20f);
        } else {
            MainCollider.Position = new(-4f, 0f);
            FullSizeCollider.Position = new(-10f, 0f);
            PickupCollider.Position = new(-14f, -4f);
            ActivatorCollider.Position = new(-10f, 0f);
        }
    }

    private void GravUpdateVisuals(bool inverted) {
        if (!GravityLocked)
            IndicatorSprite.Play(!inverted ? "normal" : "inverted");

        BaseSprite.Position = IndicatorSprite.Position = Light.Position = FullSizeCollider.Center;
    }

    #endregion Gravity
    #region > Shattering

    public void BeginShatter() {
        Shattering = true;

        Get<GravityComponent>()?.Lock();
        IndicatorSprite.Play("shatter");

        Remove(Get<AreaSwitch.Activator>());

        Surface.Collidable = false;
    }


    public void Shatter() {
        Shattering = true;
        Audio.Play("event:/game/general/wall_break_stone", AbsCenter);
        Audio.Play("event:/game/06_reflection/fall_spike_smash", AbsCenter);
        ShardDebris.Burst(AbsCenter, Inverted ? Calc.HexToColor("e36363") : Calc.HexToColor("6c63e3"), 10);

        RemoveSelf();
    }

    #endregion Shattering
    #endregion Behaviour
    #region --- Rendering ---

    public override void Render() {
        if (!Hold.IsHeld) return;

        RenderOutline();
        RenderSprite();
    }

    private void RenderSprite() {
        RenderBase(new Vector2(0f, 0f), Color.White);
        RenderIndicator(new Vector2(0f, 0f), Color.White);
    }

    private void RenderOutline() {
        RenderBase(new Vector2(1f, 1f), Color.Black);
        RenderBase(new Vector2(1f, -1f), Color.Black);
        RenderBase(new Vector2(-1f, 1f), Color.Black);
        RenderBase(new Vector2(-1f, -1f), Color.Black);
    }

    private void RenderBase(Vector2 offset, Color col) {
        BaseSprite.Texture.Draw(BaseSprite.RenderPosition + ShakeOffset + offset, BaseSprite.Origin, col, BaseSprite.Scale, BaseSprite.Rotation, BaseSprite.Effects);
    }

    private void RenderIndicator(Vector2 offset, Color col) {
        IndicatorSprite.Texture.Draw(IndicatorSprite.RenderPosition + ShakeOffset + offset, IndicatorSprite.Origin, col, IndicatorSprite.Scale, IndicatorSprite.Rotation, IndicatorSprite.Effects);
    }

    #endregion Rendering

}
