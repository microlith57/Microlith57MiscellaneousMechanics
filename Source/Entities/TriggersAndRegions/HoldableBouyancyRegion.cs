using System;
using Celeste.Mod.Entities;
using Celeste.Mod.Microlith57Misc.Components;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity(
    "Microlith57Misc/HoldableBouyancyRegion=CreateFlag",
    "Microlith57Misc/HoldableBouyancyRegion_Expression=CreateExpr"
)]
public sealed class HoldableBouyancyRegion : Entity {

    #region --- State ---

    private readonly ConditionSource Condition;
    public bool BouyancyActive => Condition.Value;

    public bool AlsoAffectPlayer;
    public float MinForce, MaxForce, Damping;

    #endregion State
    #region --- Init ---

    public HoldableBouyancyRegion(
        Vector2 position,
        ConditionSource condition,
        bool alsoAffectPlayer,
        float minForce, float maxForce, float damping
    ) : base(position) {

        Add(Condition = condition);
        AlsoAffectPlayer = alsoAffectPlayer;
        MinForce = minForce;
        MaxForce = maxForce;
        Damping = damping;
    }

    private static HoldableBouyancyRegion Create(EntityData data, Vector2 offset, ConditionSource condition)
        => new(
            data.Position + offset,
            condition,
            data.Bool("alsoAffectPlayer"),
            data.Float("minForce", 0f),
            data.Float("maxForce", 300f),
            data.Float("damping", 1f)
        ) {
            Collider = new Hitbox(data.Width, data.Height),
            Depth = Depths.Top
        };

    public static HoldableBouyancyRegion CreateFlag(Level _1, LevelData _2, Vector2 offset, EntityData data)
        => Create(data, offset, new ConditionSource.Flag(data) { Default = true });

    public static HoldableBouyancyRegion CreateExpr(Level _1, LevelData _2, Vector2 offset, EntityData data)
        => Create(data, offset, new ConditionSource.Expr(data) { Default = true });

    #endregion Init
    #region --- Behaviour ---

    public override void Update() {
        base.Update();

        if (!BouyancyActive)
            return;

        foreach (Holdable hold in Scene.Tracker.GetComponents<Holdable>())
            if (!hold.IsHeld && hold.Entity is Entity e && e.CollideCheck(this))
                hold.SetSpeed(Affect(e, hold.GetSpeed()));

        // TODO: remove once eeveehelper has proper speedsetters (if possible)
        foreach ((var container, var hold, var speedSetter) in Scene.Tracker.GetEeveeHelperHoldableContainers())
            if (!hold.IsHeld && hold.SpeedSetter == null && container.CollideCheck(this))
                speedSetter(Affect(container, hold.GetSpeed()));

        if (AlsoAffectPlayer && Scene.Tracker.GetEntity<Player>() is Player p && p.CollideCheck(this))
            p.Speed = Affect(p, p.Speed);
    }

    private Vector2 Affect(Entity e, Vector2 speed) {
        float force;

        if (!e.ShouldInvert()) {
            var bottom = e.Bottom + (e is Actor a ? a.ExactPosition.Y - a.Position.Y : 0f);
            force = Calc.ClampedMap(bottom, Top, Top + e.Height, MinForce, MaxForce);
        } else {
            var top = e.Top + (e is Actor a ? a.ExactPosition.Y - a.Position.Y : 0f);
            force = Calc.ClampedMap(top, Bottom, Bottom - e.Height, MinForce, MaxForce);
        }

        var rawSpeed = speed - force * Engine.DeltaTime * Vector2.UnitY;
        var damping = (float)Math.Exp(-Damping * Engine.DeltaTime);

        return rawSpeed * damping;
    }

    #endregion Behaviour

}