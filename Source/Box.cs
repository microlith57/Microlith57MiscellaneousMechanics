using System;
using System.Collections;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57.IntContest;

[CustomEntity("Microlith57_IntContest24/Box")]
[Tracked]
public class Box : Actor {

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    // todo
    public static ParticleType P_Impact;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public Vector2 Speed;
    public Holdable Hold;
    public Image Sprite;
    public JumpThru Surface;

    private Collision onCollideH;
    private Collision onCollideV;

    public float NoGravityTimer;

    private Vector2 prevLiftSpeed;
    private Vector2 previousPosition;

    public bool Shattering;
    public bool Dead;

    public bool BonkedH, BonkedV;

    private bool IsTutorial;
    private BirdTutorialGui? tutorialGui;
    private float tutorialTimer;

    public Box(Vector2 position, Vector2 speed = default, bool tutorial = false) : base(position) {
        previousPosition = Position;
        IsTutorial = tutorial;

        Depth = 100;
        Collider = new Hitbox(8f, 10f, -4f, -10f);

        Add(Sprite = new(GFX.Game["objects/INTcontest24/microlith57/box"]) {
            Origin = new(11f, 21f)
        });

        Add(Hold = new Holdable {
            PickupCollider = new Hitbox(28f, 28f, -14f, -24f),
            SlowFall = false,
            SlowRun = true,
            OnPickup = OnPickup,
            OnRelease = OnRelease,
            DangerousCheck = Dangerous,
            OnHitSpring = HitSpring,
            OnHitSpinner = HitSpinner,
            SpeedGetter = () => Speed
        });

        onCollideH = OnCollideH;
        onCollideV = OnCollideV;
        LiftSpeedGraceTime = 0.1f;

        var activatorCollider = new Hitbox(20f, 20f, -10f, -20f);
        activatorCollider.Added(this);

        Add(new VertexLight(activatorCollider.Center, Color.White, 1f, 24, 48));
        Add(new AreaSwitch.Activator() { Collider = activatorCollider });

        Surface = new(Position + new Vector2(-10f, -20f), 20, safe: false) {
            Depth = 99,
            SurfaceSoundIndex = SurfaceIndex.Girder
        };

        // todo: do something about transitions
        // Tag = Tags.TransitionUpdate;
    }

    public Box(EntityData data, Vector2 offset)
        : this(data.Position + offset,
               new(data.Float("speedX"), data.Float("speedY")),
               data.Bool("tutorial")) { }

    public override void Added(Scene scene) {
        base.Added(scene);

        Scene.Add(Surface);

        if (IsTutorial)
            Scene.Add(tutorialGui = new(
                this, new Vector2(0f, -24f),
                Dialog.Clean("tutorial_carry"),
                Dialog.Clean("tutorial_hold"),
                BirdTutorialGui.ButtonPrompt.Grab) {
                Open = false
            });
    }

    public override void Update() {
        base.Update();

        BonkedH = BonkedV = false;

        if (Scene is not Level level)
            return;

        if (Shattering || Dead) {
            Surface.Collidable = false;
            Hold.cannotHoldTimer = 0.1f;
            return;
        }

        if (Hold.IsHeld)
            prevLiftSpeed = Vector2.Zero;
        else {
            if (OnGround()) {
                float target = (!OnGround(Position + Vector2.UnitX * 3f)) ? 20f : (OnGround(Position - Vector2.UnitX * 3f) ? 0f : (-20f));
                Speed.X = Calc.Approach(Speed.X, target, 800f * Engine.DeltaTime);
                Vector2 liftSpeed = LiftSpeed;
                if (liftSpeed == Vector2.Zero && prevLiftSpeed != Vector2.Zero) {
                    Speed = prevLiftSpeed;
                    prevLiftSpeed = Vector2.Zero;
                    Speed.Y = Math.Min(Speed.Y * 0.6f, 0f);
                    if (Speed.X != 0f && Speed.Y == 0f) {
                        Speed.Y = -60f;
                    }
                    if (Speed.Y < 0f) {
                        NoGravityTimer = 0.15f;
                    }
                } else {
                    prevLiftSpeed = liftSpeed;
                    if (liftSpeed.Y < 0f && Speed.Y < 0f) {
                        Speed.Y = 0f;
                    }
                }
            } else if (Hold.ShouldHaveGravity) {
                float num = 800f;
                if (Math.Abs(Speed.Y) <= 30f) {
                    num *= 0.5f;
                }
                float num2 = 350f;
                if (Speed.Y < 0f) {
                    num2 *= 0.5f;
                }
                Speed.X = Calc.Approach(Speed.X, 0f, num2 * Engine.DeltaTime);
                if (NoGravityTimer > 0f) {
                    NoGravityTimer -= Engine.DeltaTime;
                } else {
                    Speed.Y = Calc.Approach(Speed.Y, 200f, num * Engine.DeltaTime);
                }
            }
            previousPosition = ExactPosition;
            MoveH(Speed.X * Engine.DeltaTime, onCollideH);
            MoveV(Speed.Y * Engine.DeltaTime, onCollideV);
            if (Center.X > level.Bounds.Right) {
                MoveH(32f * Engine.DeltaTime);
                if (Left - 8f > level.Bounds.Right) {
                    RemoveSelf();
                }
            } else if (Left < level.Bounds.Left) {
                Left = level.Bounds.Left;
                Speed.X *= -0.4f;
            } else if (Top < level.Bounds.Top - 4) {
                Top = level.Bounds.Top + 4;
                Speed.Y = 0f;
            } else if (Bottom > level.Bounds.Bottom && SaveData.Instance.Assists.Invincible) {
                Bottom = level.Bounds.Bottom;
                Speed.Y = -300f;
                Audio.Play("event:/game/general/assist_screenbottom", Position);
            } else if (Top > level.Bounds.Bottom) {
                Die();
            }
            if (X < level.Bounds.Left + 10) {
                MoveH(32f * Engine.DeltaTime);
            }
            Player entity = Scene.Tracker.GetEntity<Player>();
            TempleGate templeGate = CollideFirst<TempleGate>();
            if (templeGate != null && entity != null) {
                templeGate.Collidable = false;
                MoveH(Math.Sign(entity.X - X) * 32 * Engine.DeltaTime);
                templeGate.Collidable = true;
            }
        }

        if (!Dead)
            Hold.CheckAgainstColliders();

        if (Hold.IsHeld)
            Surface.Position = Position + new Vector2(-10f, -20f);
        else
            Surface.MoveTo(Position + new Vector2(-10f, -20f));

        if (tutorialGui != null) {
            if (Hold.IsHeld) {
                tutorialTimer = 0f;
                tutorialGui.Open = false;
                tutorialGui.RemoveSelf();
                tutorialGui = null;
            } else if (!OnGround())
                tutorialTimer = 0f;
            else
                tutorialTimer += Engine.DeltaTime;

            if (tutorialGui != null)
                tutorialGui.Open = tutorialTimer > 0.25f;
        }
    }

    public void Shatter() {
        var center = Position + new Vector2(0f, -10f);
        Audio.Play("event:/game/general/wall_break_stone", center);

        for (int i = 0; i < 7; i++) {
            var pos = Calc.Round(new(Calc.Random.NextFloat(12f) + 4f, Calc.Random.NextFloat(12f) + 4f));
            var debris = new Debris().orig_Init(TopLeft + pos, '1').BlastFrom(center);
            debris.image.Texture = GFX.Game["debris/7"];
            Scene.Add(debris);
        }

        RemoveSelf();
    }

    public void ExplodeLaunch(Vector2 from) {
        if (Hold.IsHeld)
            return;

        Speed = (Center - from).SafeNormalize(120f);
        SlashFx.Burst(Center, Speed.Angle());
    }

    public bool Dangerous(HoldableCollider holdableCollider) => !Hold.IsHeld && Speed != Vector2.Zero;

    public void HitSpinner(Entity spinner) {
        if (Hold.IsHeld ||
            Speed.Length() >= 0.01f ||
            LiftSpeed.Length() >= 0.01f ||
            (previousPosition - ExactPosition).Length() >= 0.01f ||
            !OnGround())

            return;

        int dir = Math.Sign(X - spinner.X);
        if (dir == 0)
            dir = 1;

        Speed.X = dir * 120f;
        Speed.Y = -30f;
    }

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
        if (Speed.Y == 0f) {
            return base.IsRiding(solid);
        }
        return false;
    }

    public override void OnSquish(CollisionData data) {
        if (!TrySquishWiggle(data) && !SaveData.Instance.Assists.Invincible) {
            Die();
        }
    }

    private void OnPickup() {
        Speed = Vector2.Zero;
        AddTag(Tags.Persistent);
        Surface.AddTag(Tags.Persistent);
        Surface.Collidable = false;
    }

    private void OnRelease(Vector2 force) {
        RemoveTag(Tags.Persistent);
        Surface.AddTag(Tags.Persistent);
        Surface.Collidable = true;
        Surface.Position = Position + new Vector2(-10f, -20f);

        if (force.X != 0f && force.Y == 0f)
            force.Y = -0.4f;
        Speed = force * 200f;
        if (Speed != Vector2.Zero)
            NoGravityTimer = 0.1f;
    }

    public void Die() {
        if (Dead || Shattering)
            return;

        Dead = true;
        Sprite.Visible = false;
        Depth = -1000000;
        AllowPushing = false;
    }

    public override void Removed(Scene scene) {
        base.Removed(scene);
        Surface.RemoveSelf();
    }

}