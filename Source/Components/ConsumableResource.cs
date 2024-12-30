using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.Microlith57Misc.Components;

[Tracked(inherited: true)]
public abstract class ConsumableResource : Entity {

    public sealed class Drain : Component {

        public readonly ConsumableResource Resource;
        public float ConsumptionRate;
        public bool UseRawDeltaTime;
        public bool Stacks;

        public float ConsumptionThisFrame => ConsumptionRate * (UseRawDeltaTime ? Engine.RawDeltaTime : Engine.DeltaTime);

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
            UseRawDeltaTime = rawDeltaTime;
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

    }

    public readonly string Name;

    // public readonly bool FlashPlayer;
    public bool DieWhenConsumed { get; private set; }

    public readonly (string Any, string Full, string Low, string Flash)? FlagNames;
    public readonly Session.Slider Slider;
    private float prevSliderValue;

    private HashSet<Drain> Drains = [];

    protected ConsumableResource(
        EntityData data, Vector2 offset,
        Session.Slider slider,
        ConditionSource instantRefillCondition,
        ConditionSource instantDrainCondition
    ) : base(data.Position + offset) {

        Name = data.Attr("name", "resource");

        var prefix = data.Attr("flagPrefix");
        if (prefix != "")
            FlagNames = (
                prefix + "Any",
                prefix + "Full",
                prefix + "Low",
                prefix + "Flash"
            );

        Slider = slider;

        DieWhenConsumed = data.Bool("dieWhenConsumed");

        PreUpdate += BeforeUpdate;
        PostUpdate += AfterUpdate;
    }

    public abstract float Low { get; }
    public abstract float Current { get; set; }
    public abstract float Maximum { get; }

    public virtual bool CanConsume => Current > 0f;
    protected virtual bool ShouldFlash => Current <= Low;
    public virtual bool Flashing
        => ShouldFlash
        && Scene is Level level
        && level.BetweenInterval(0.05f);

    public override void Awake(Scene scene) {
        base.Awake(scene);

        Slider.Value = prevSliderValue = Current;
    }

    private void BeforeUpdate(Entity _) {
        if (prevSliderValue != Slider.Value)
            Current = Slider.Value;

        foreach (var drain in Drains)
            Current -= drain.ConsumptionThisFrame;
    }

    private void AfterUpdate(Entity _) {
        if (Scene is not Level level) return;

        var c = Current;

        if (DieWhenConsumed && c <= 0f) {
            level.Tracker.GetEntity<Player>()?.Die(Vector2.Zero);
            DieWhenConsumed = false;
        }

        if (FlagNames.HasValue) {
            level.Session.SetFlag(FlagNames.Value.Any, c > 0f);
            level.Session.SetFlag(FlagNames.Value.Full, c > Maximum);
            level.Session.SetFlag(FlagNames.Value.Low, c <= Low);
            level.Session.SetFlag(FlagNames.Value.Flash, Flashing);
        }

        Slider.Value = prevSliderValue = c;
    }

    [CustomEntity(
        "Microlith57Misc/ConsumableResource_Stamina=Create",
        "Microlith57Misc/ConsumableResource_Stamina_Expresssion=CreateExpr"
    )]
    public class Stamina : ConsumableResource {

        private Player? Player => (Scene as Level)?.Tracker?.GetEntity<Player>();
        private float _Current, _Maximum;

        public override bool Flashing
            => Player is Player player
            && player.flash;

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

        public override float Low => 20f;

        public override float Current {
            get => _Current;
            set {
                if (Player is not Player player) return;
                player.Stamina = _Current = value;
            }
        }

        public override float Maximum => _Maximum;

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

    }

}
