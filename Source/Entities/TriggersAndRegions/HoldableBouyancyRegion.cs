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

    private readonly FloatSource MinForceSource, MaxForceSource, DampingSource;
    public float MinForce => MinForceSource.Value;
    public float MaxForce => MaxForceSource.Value;
    public float Damping => DampingSource.Value;

    public bool AlsoAffectPlayer;

    #endregion State
    #region --- Init ---

    public HoldableBouyancyRegion(
        EntityData data, Vector2 offset,
        ConditionSource condition,
        FloatSource minForce, FloatSource maxForce, FloatSource damping
    ) : base(data.Position + offset) {

        Collider = new Hitbox(data.Width, data.Height);
        Depth = Depths.Top;

        Add(Condition = condition);
        Add(MinForceSource = minForce);
        Add(MaxForceSource = maxForce);
        Add(DampingSource = damping);

        AlsoAffectPlayer = data.Bool("alsoAffectPlayer");
    }

    public static HoldableBouyancyRegion CreateFlag(Level level, LevelData _, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Flag(data) { Default = true },
            new FloatSource.Slider(level.Session, data, "minForce", "0"),
            new FloatSource.Slider(level.Session, data, "maxForce", "300"),
            new FloatSource.Slider(level.Session, data, "damping", "1")
        );

    public static HoldableBouyancyRegion CreateExpr(Level level, LevelData _, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Expr(data) { Default = true },
            new FloatSource.Expr(data, "minForce", "0"),
            new FloatSource.Expr(data, "maxForce", "300"),
            new FloatSource.Expr(data, "damping", "1")
        );

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