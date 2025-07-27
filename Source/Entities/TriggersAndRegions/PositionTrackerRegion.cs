using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.Entities;
using Celeste.Mod.Microlith57Misc.Components;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity(
    "Microlith57Misc/PositionTrackerRegion=CreateFlag",
    "Microlith57Misc/PositionTrackerRegion_Expression=CreateExpr"
)]
public sealed class PositionTrackerRegion : Entity {

    #region --- State ---

    private enum TargetType {
        Player,
        Actor,
        NonPlayerActor,
        Solid,
    }

    private enum DetectionType {
        Within,
        Intersecting,
        Nearest,
    }

    private enum StickinessType {
        Free,
        Transient,
        UntilNewMatch,
        UntilDeath,
        Lifelink,
        Soulbond,
    }

    private enum TrackingType {
        Position,
        Center,
        TopCenter,
        BottomCenter,
        CenterLeft,
        CenterRight,
        Size,
        Speed,
    }

    private enum DirectionalityType {
        EntityToSliders,
        SlidersToEntity,
        AddDeltas,
    }

    private readonly ConditionSource Condition;
    public bool Enabled => Condition.Value;

    private TargetType WillTarget;
    private DetectionType Detection;
    private StickinessType Stickiness;
    private TrackingType Tracking;
    private DirectionalityType Directionality;

    private Entity? _Target;
    private bool EverTargetted = false;

    private Entity? Target {
        get => _Target;
        set {
            if (_Target == value) return;
            if (value != null) EverTargetted = true;

            _Target = value;
            LastValue = null;
        }
    }

    private Session.Slider SliderX, SliderY;
    private string TargettingFlag;
    private Vector2? LastValue;

    #endregion State
    #region --- Init ---

    public PositionTrackerRegion(
        EntityData data, Vector2 offset,
        Session.Slider sliderX, Session.Slider sliderY,
        ConditionSource condition
    ) : base(data.Position + offset) {

        Collider = new Hitbox(data.Width, data.Height);
        Collidable = false;

        Add(Condition = condition);

        WillTarget = data.Enum("target", TargetType.Player);
        Detection = data.Enum("detection", DetectionType.Intersecting);
        Stickiness = data.Enum("stickiness", StickinessType.Lifelink);
        Tracking = data.Enum("tracking", TrackingType.Position);
        Directionality = data.Enum("directionality", DirectionalityType.EntityToSliders);

        SliderX = sliderX;
        SliderY = sliderY;
        TargettingFlag = data.Attr("targettingFlag", "");
    }


    public static PositionTrackerRegion CreateFlag(Level level, LevelData _, Vector2 offset, EntityData data)
        => new(
            data, offset,
            level.Session.GetSliderObject(data.Attr("sliderPrefix", "trackedPosition") + 'X'),
            level.Session.GetSliderObject(data.Attr("sliderPrefix", "trackedPosition") + 'Y'),
            new ConditionSource.Flag(data, "retargetIfFlag", invertName: "invertRetargetIfFlag") { Default = true }
        );

    public static PositionTrackerRegion CreateExpr(Level level, LevelData _, Vector2 offset, EntityData data)
        => new(
            data, offset,
            level.Session.GetSliderObject(data.Attr("sliderPrefix", "trackedPosition") + 'X'),
            level.Session.GetSliderObject(data.Attr("sliderPrefix", "trackedPosition") + 'Y'),
            new ConditionSource.Expr(data, "retargetIfExpression") { Default = true }
        );

    #endregion Init
    #region --- Behaviour ---

    public override void Awake(Scene scene) {
        base.Awake(scene);
        UpdateTarget();
        UpdateLink(DirectionalityType.EntityToSliders);
    }

    public override void Update() {
        base.Update();
        UpdateTarget();
        UpdateLink(Directionality);
    }

    private void UpdateLink(DirectionalityType bidi) {
        Vector2? position;

        switch (bidi) {
            case DirectionalityType.SlidersToEntity:
                position = new(SliderX.Value, SliderY.Value);
                break;

            case DirectionalityType.AddDeltas:
                if (LastValue != null) {
                    position = LastValue.Value
                             + LastValue.Value - new Vector2(SliderX.Value, SliderY.Value)
                             + LastValue.Value - Value;
                } else
                    position = Value;
                break;

            default:
                position = Value;
                break;
        }

        if (position is Vector2 vec) {
            if (bidi != DirectionalityType.EntityToSliders)
                Value = position;

            if (bidi != DirectionalityType.SlidersToEntity) {
                SliderX.Value = vec.X;
                SliderY.Value = vec.Y;
            }
        }

        if (Scene is Level level && TargettingFlag != "")
            level.Session.SetFlag(TargettingFlag, Target != null);
    }

    private Vector2? Value {
        get {
            if (Target == null) return null;
            switch (Tracking) {
                default:
                    return GetPositionWithSubpixels(Target, Tracking);
                case TrackingType.Size:
                    return new(Target.Collider?.Width ?? 0f, Target.Collider?.Height ?? 0f);
                case TrackingType.Speed:
                    if (Target is Player player)
                        return player.Speed;
                    else if (Target.Get<Holdable>() is Holdable hold)
                        return hold.GetSpeed();

                    return null;
            }
        }
        set {
            if (Target == null || value == null) return;
            switch (Tracking) {
                default:
                    SetPositionWithSubpixels(Target, Tracking, value.Value);
                    return;
                case TrackingType.Size:
                    if (Target.Collider != null) {
                        Target.Collider.Width = value.Value.X;
                        Target.Collider.Height = value.Value.Y;
                    }
                    return;
                case TrackingType.Speed:
                    if (Target is Player player)
                        player.Speed = value.Value;
                    else if (Target.Get<Holdable>() is Holdable hold)
                        hold.SetSpeed(value.Value);

                    return;
            }
        }
    }

    private static Vector2 GetPositionWithoutSubpixels(Entity e, TrackingType t) {
        switch (t) {
            case TrackingType.Position: return e.Position;
            case TrackingType.Center: return e.Center;
            case TrackingType.TopCenter: return e.TopCenter;
            case TrackingType.BottomCenter: return e.BottomCenter;
            case TrackingType.CenterLeft: return e.CenterLeft;
            case TrackingType.CenterRight: return e.CenterRight;
            default: throw new Exception("unreachable");
        }
    }

    private static Vector2 GetPositionWithSubpixels(Entity e, TrackingType t)
        => GetPositionWithoutSubpixels(e, t) + ((e is Actor a) ? a.movementCounter : Vector2.Zero);

    private static void SetPositionWithoutSubpixels(Entity e, TrackingType t, Vector2 pos) {
        switch (t) {
            case TrackingType.Position: e.Position = pos; return;
            case TrackingType.Center: e.Center = pos; return;
            case TrackingType.TopCenter: e.TopCenter = pos; return;
            case TrackingType.BottomCenter: e.BottomCenter = pos; return;
            case TrackingType.CenterLeft: e.CenterLeft = pos; return;
            case TrackingType.CenterRight: e.CenterRight = pos; return;
            default: throw new Exception("unreachable");
        }
    }

    private static void SetPositionWithSubpixels(Entity e, TrackingType t, Vector2 pos) {
        if (e is Actor a) {
            var floor = Calc.Floor(pos);
            SetPositionWithoutSubpixels(e, t, floor);
            a.movementCounter = pos - floor;
        } else SetPositionWithoutSubpixels(e, t, pos);
    }

    #endregion Behaviour
    #region --- Targetting ---

    private void UpdateTarget() {
        bool canSwitch = Enabled;

        if (Target?.Scene == null)
            Target = null;

        Entity? target = null;

        Collidable = true;
        switch (Stickiness) {
            case StickinessType.Free:
                if (TryGetTarget(ref target) && canSwitch)
                    Target = target;
                goto case StickinessType.Transient;

            case StickinessType.Transient:
                if (Target != null && !Matches(Target) && canSwitch)
                    Target = null;
                goto case StickinessType.UntilDeath;

            case StickinessType.UntilNewMatch:
                if ((Target == null || !Matches(Target)) && TryGetTarget(ref target) && canSwitch)
                    Target = target;
                break;

            case StickinessType.UntilDeath:
                if (Target == null && TryGetTarget(ref target) && canSwitch)
                    Target = target;
                break;

            case StickinessType.Lifelink:
            case StickinessType.Soulbond:
                if (!EverTargetted && TryGetTarget(ref target) && canSwitch)
                    Target = target;
                else if (Target == null && (EverTargetted || Stickiness == StickinessType.Soulbond))
                    RemoveSelf();
                break;

            default: throw new Exception("unreachable");
        }
        Collidable = false;
    }

    private bool TryGetTarget(ref Entity? target)
        => (target ??= Filter(Candidates)) != null;

    private IEnumerable<Entity> Candidates {
        get {
            switch (WillTarget) {
                case TargetType.Player: return Scene.Tracker.GetEntities<Player>();
                case TargetType.Actor: return Scene.Tracker.GetEntities<Actor>();
                case TargetType.NonPlayerActor: return Scene.Tracker.GetEntities<Actor>().Where(c => c is not Player);
                case TargetType.Solid: return Scene.Tracker.GetEntities<Solid>();
                default: throw new Exception("unreachable");
            }
        }
    }

    private Entity? Filter(IEnumerable<Entity> candidates) {
        switch (Detection) {
            case DetectionType.Within:
            case DetectionType.Intersecting:
                candidates = candidates.Where(Matches);
                goto case DetectionType.Nearest;

            case DetectionType.Nearest:
                return candidates.MinBy(c => (Center - c.Center).LengthSquared());

            default: throw new Exception("unreachable");
        }
    }

    private bool Matches(Entity e) {
        if (e is SolidTiles) return false;

        switch (Detection) {
            case DetectionType.Within: return IsWithin(e);
            case DetectionType.Intersecting: return IsIntersecting(e);
            default: return true;
        }
    }

    private bool IsWithin(Entity e) {
        if (e.Collider == null)
            return Collide.CheckPoint(this, e.Position);

        return (
            e.Collider.AbsoluteLeft >= Left
            && e.Collider.AbsoluteRight <= Right
            && e.Collider.AbsoluteTop >= Top
            && e.Collider.AbsoluteBottom <= Bottom
        );
    }

    private bool IsIntersecting(Entity e) {
        if (e.Collider == null)
            return Collide.CheckPoint(this, e.Position);

        return CollideCheck(e);
    }

    #endregion Targetting
    #region --- Rendering ---

    public override void DebugRender(Camera camera) {
        base.DebugRender(camera);

        var canSwitch = Enabled;
        switch (Stickiness) {
            case StickinessType.Free:
                break;
            case StickinessType.Transient:
            case StickinessType.UntilNewMatch:
                canSwitch &= Target == null || !Matches(Target); break;
            default:
                canSwitch &= Target == null; break;
        }

        Draw.HollowRect(X, Y, Width, Height, canSwitch ? Color.Red : Color.DarkRed);

        if (EverTargetted)
            Draw.Line(Center, new(SliderX.Value, SliderY.Value), Target == null ? Color.DarkRed : Color.Red);
    }

    #endregion Rendering


}
