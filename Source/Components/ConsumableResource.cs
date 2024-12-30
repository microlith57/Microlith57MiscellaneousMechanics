using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.Microlith57Misc.Components;

[Tracked(inherited: true)]
public abstract class ConsumableResource : Entity {

    public sealed class Drain : Component {

        #region --- Drain ---

        public readonly ConsumableResource Resource;
        public float ConsumptionRate;
        public bool Stacks;

        public float ConsumptionThisFrame => ConsumptionRate * Resource.DeltaTime;

        #region > Init

        public Drain(
            ConsumableResource resource,
            float consumptionRate,
            bool rawDeltaTime,
            bool stacks
        ) : base(active: true, visible: false) {

            // todo non-positive drain rates
            if (consumptionRate <= 0f)
                throw new Exception("non-positive drain rates are unimplemented");

            // todo non-stacking drains
            if (!stacks)
                throw new Exception("non-stacking drains are unimplemented");

            Resource = resource;
            ConsumptionRate = consumptionRate;
            Stacks = stacks;
        }

        public override void Added(Entity entity) {
            base.Added(entity);
            if (entity.Scene != null)
                Resource.Drains.Add(this);
        }

        public override void EntityAdded(Scene scene) {
            base.EntityAdded(scene);
            Resource.Drains.Add(this);
        }

        public override void Removed(Entity entity) {
            base.Removed(entity);
            Resource.Drains.Remove(this);
        }

        public override void EntityRemoved(Scene scene) {
            base.EntityRemoved(scene);
            Resource.Drains.Remove(this);
        }

        #endregion Drain > Init
        #endregion Drain

    }

    #region --- Abstract ---

    public readonly string Name;

    public readonly bool UseRawDeltaTime;
    public float DeltaTime => UseRawDeltaTime ? Engine.RawDeltaTime : Engine.DeltaTime;

    // public readonly bool FlashPlayer;
    public bool DieWhenConsumed { get; private set; }

    public readonly (string Any, string Full, string Low, string Flash)? FlagNames;
    public readonly Session.Slider Slider;
    private float prevSliderValue;

    public readonly float RestoreCooldown;
    public readonly float RestoreSpeed;
    private float cooldown;

    public virtual void BumpRestoreCooldown() => cooldown = RestoreCooldown;

    private HashSet<Drain> Drains = [];

    public abstract float Low { get; }
    public abstract float Maximum { get; }
    public abstract float Current { get; set; }

    public virtual bool CanConsume => Current > 0f;
    protected virtual bool ShouldFlash => Current <= Low;
    public virtual bool Flashing
        => ShouldFlash
        && Scene is Level level
        && level.BetweenInterval(0.05f);

    public virtual bool CanStartRestoring => true;
    public bool CanRestore => !Drains.Any(d => d.Active) && cooldown <= 0f;

    #region > Init

    protected ConsumableResource(
        EntityData data, Vector2 offset,
        Session.Slider slider,
        ConditionSource instantRefillCondition,
        ConditionSource instantDrainCondition
    ) : base(data.Position + offset) {

        Name = data.Attr("resource", "resourceName");

        var prefix = data.Attr("flagPrefix");
        if (prefix != "")
            FlagNames = (
                prefix + "Any",
                prefix + "Full",
                prefix + "Low",
                prefix + "Flash"
            );

        Slider = slider;

        UseRawDeltaTime = data.Bool("useRawDeltaTime");
        DieWhenConsumed = data.Bool("dieWhenConsumed");

        RestoreCooldown = data.Float("restoreCooldown", 0.1f);
        RestoreSpeed = data.Float("restoreSpeed", 120f);

        PreUpdate += BeforeUpdate;
        PostUpdate += AfterUpdate;
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        Slider.Value = prevSliderValue = Current;
    }

    #endregion Abstract > Init
    #region > Behaviour

    private void BeforeUpdate(Entity _) {
        if (prevSliderValue != Slider.Value) {
            if (Slider.Value < Current) BumpRestoreCooldown();
            Current = Slider.Value;
        }

        foreach (var drain in Drains) {
            if (!drain.Active) continue;

            BumpRestoreCooldown();
            if (CanConsume)
                Current = Calc.Approach(Current, 0f, drain.ConsumptionThisFrame);
            else break;
        }
    }

    private void AfterUpdate(Entity _) {
        if (Scene is not Level level) return;

        if (cooldown > 0) {
            if (CanStartRestoring)
                cooldown = Calc.Approach(cooldown, 0f, DeltaTime);
        } else if (Current < Maximum) {
            if (RestoreSpeed < 0)
                Current = Maximum;
            else
                Current = Calc.Approach(Current, Maximum, RestoreSpeed * DeltaTime);
        }

        var c = Current;

        if (DieWhenConsumed && c <= 0f) {
            level.Tracker.GetEntity<Player>()?.Die(Vector2.Zero);
            DieWhenConsumed = false;
        }

        if (FlagNames.HasValue) {
            level.Session.SetFlag(FlagNames.Value.Any, c > 0f);
            level.Session.SetFlag(FlagNames.Value.Full, c >= Maximum);
            level.Session.SetFlag(FlagNames.Value.Low, c <= Low);
            level.Session.SetFlag(FlagNames.Value.Flash, Flashing);
        }

        Slider.Value = prevSliderValue = c;
    }

    #endregion Abstract > Behaviour
    #endregion Abstract

    [CustomEntity(
        "Microlith57Misc/ConsumableResource_Custom=Create",
        "Microlith57Misc/ConsumableResource_Custom_Expresssion=CreateExpr"
    )]
    public class Custom : ConsumableResource {

        #region --- Custom ---

        private float _Low, _Maximum;

        public override float Low => _Low;
        public override float Maximum => _Maximum;
        public override float Current { get; set; }

        #region > Init

        public Custom(
            EntityData data, Vector2 offset,
            Session.Slider slider,
            ConditionSource instantRefillCondition,
            ConditionSource instantDrainCondition
        ) : base(
            data, offset,
            slider,
            instantRefillCondition,
            instantDrainCondition
        ) {
            _Low = data.Float("lowThreshold", 20f);
            _Maximum = data.Float("maximum", 110f);
        }

        public static Custom Create(Level level, LevelData __, Vector2 offset, EntityData data)
            => new(
                data, offset,
                level.Session.GetSliderObject(data.Attr("resource")),
                new ConditionSource.Flag(data, "instantRefillFlag", invertName: "invertInstantRefillFlag"),
                new ConditionSource.Flag(data, "instantDrainFlag", invertName: "invertInstantDrainFlag")
            );

        public static Custom CreateExpr(Level level, LevelData __, Vector2 offset, EntityData data)
            => new(
                data, offset,
                level.Session.GetSliderObject(data.Attr("resource")),
                new ConditionSource.Expr(data, "instantRefillExpression"),
                new ConditionSource.Expr(data, "instantDrainExpression")
            );

        #endregion MaxStamina > Init
        #endregion MaxStamina

    }

    [CustomEntity(
        "Microlith57Misc/ConsumableResource_Stamina=Create",
        "Microlith57Misc/ConsumableResource_Stamina_Expresssion=CreateExpr"
    )]
    public class Stamina : ConsumableResource {

        #region --- Stamina ---

        private Player? _Player;
        private Player? Player
            => _Player ??= (Scene as Level)?.Tracker?.GetEntity<Player>();

        private float _Maximum, _Current;

        public override float Low => 20f;
        public override float Maximum => _Maximum;

        public override float Current {
            get => _Current;
            set {
                if (Player is not Player player) return;
                if (value < _Current) BumpRestoreCooldown();
                player.Stamina = _Current = value;
            }
        }

        public override bool Flashing
            => ShouldFlash
            && Player is Player player
            && player.flash;

        #region > Init

        public Stamina(
            EntityData data, Vector2 offset,
            Session.Slider slider,
            ConditionSource instantRefillCondition,
            ConditionSource instantDrainCondition
        ) : base(
            data, offset,
            slider,
            instantRefillCondition,
            instantDrainCondition
        ) {
            // FlashPlayer = false;
        }

        public static Stamina Create(Level level, LevelData __, Vector2 offset, EntityData data)
            => new(
                data, offset,
                level.Session.GetSliderObject(data.Attr("resource")),
                new ConditionSource.Flag(data, "instantRefillFlag", invertName: "invertInstantRefillFlag"),
                new ConditionSource.Flag(data, "instantDrainFlag", invertName: "invertInstantDrainFlag")
            );

        public static Stamina CreateExpr(Level level, LevelData __, Vector2 offset, EntityData data)
            => new(
                data, offset,
                level.Session.GetSliderObject(data.Attr("resource")),
                new ConditionSource.Expr(data, "instantRefillExpression"),
                new ConditionSource.Expr(data, "instantDrainExpression")
            );

        #endregion Stamina > Init
        #region > Behaviour

        public override void Awake(Scene scene) {
            base.Awake(scene);

            if (Player is not Player player)
                throw new Exception("player somehow dead when creating stamina resource");

            _Current = player.Stamina;
            player.RefillStamina();
            _Maximum = player.Stamina;
            player.Stamina = _Current;
        }

        public override void Update() {
            base.Update();

            if (Player is Player player)
                _Current = player.Stamina;
        }

        #endregion Stamina > Behaviour
        #endregion Stamina

    }

    [CustomEntity(
        "Microlith57Misc/ConsumableResource_MaxStamina=Create",
        "Microlith57Misc/ConsumableResource_MaxStamina_Expresssion=CreateExpr"
    )]
    public class MaxStamina : ConsumableResource {

        #region --- MaxStamina ---

        private CappedStamina? Cap;

        private float _Low;

        public override float Low => _Low;
        public override float Maximum => Cap?.UpperCap ?? 110f;

        public override float Current {
            get => Cap?.CurrentCap ?? 110f;
            set {
                Cap!.CurrentCap = value;
                Cap.Enforce();
            }
        }

        public override bool Flashing
            => ShouldFlash
            && (Cap?.Player.flash ?? false);

        private bool _CanStartRestoring = true;
        public override bool CanStartRestoring => base.CanStartRestoring && _CanStartRestoring;

        public override void BumpRestoreCooldown() {
            base.BumpRestoreCooldown();
            _CanStartRestoring = false;
        }

        #region > Init

        public MaxStamina(
            EntityData data, Vector2 offset,
            Session.Slider slider,
            ConditionSource instantRefillCondition,
            ConditionSource instantDrainCondition
        ) : base(
            data, offset,
            slider,
            instantRefillCondition,
            instantDrainCondition
        ) {
            _Low = data.Float("lowThreshold", 20f);
            // FlashPlayer = false;
        }

        public static MaxStamina Create(Level level, LevelData __, Vector2 offset, EntityData data)
            => new(
                data, offset,
                level.Session.GetSliderObject(data.Attr("resource")),
                new ConditionSource.Flag(data, "instantRefillFlag", invertName: "invertInstantRefillFlag"),
                new ConditionSource.Flag(data, "instantDrainFlag", invertName: "invertInstantDrainFlag")
            );

        public static MaxStamina CreateExpr(Level level, LevelData __, Vector2 offset, EntityData data)
            => new(
                data, offset,
                level.Session.GetSliderObject(data.Attr("resource")),
                new ConditionSource.Expr(data, "instantRefillExpression"),
                new ConditionSource.Expr(data, "instantDrainExpression")
            );

        #endregion MaxStamina > Init
        #region > Behaviour

        public override void Awake(Scene scene) {
            base.Awake(scene);

            var player = Scene.Tracker.GetEntity<Player>() ?? throw new Exception("player somehow dead when creating stamina resource");

            if (player.Get<CappedStamina>() is CappedStamina cap)
                Cap = cap;
            else
                player.Add(Cap = new());

            Cap.OnRestore += () => {
                _CanStartRestoring = true;
            };
        }

        #endregion MaxStamina > Behaviour
        #endregion MaxStamina

    }

}
