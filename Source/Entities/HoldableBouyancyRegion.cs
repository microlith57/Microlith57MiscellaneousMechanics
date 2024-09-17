using System;
using Celeste.Mod.Entities;
using Celeste.Mod.GravityHelper.Components;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity("Microlith57Misc/HoldableBouyancyRegion")]
[Tracked]
public sealed class HoldableBouyancyRegion : Entity {

    public bool AlsoAffectPlayer;
    public float MinForce, MaxForce, Damping;

    public HoldableBouyancyRegion(EntityData data, Vector2 offset) : base(data.Position + offset) {
        Collider = new Hitbox(data.Width, data.Height);

        AlsoAffectPlayer = data.Bool("alsoAffectPlayer", false);
        MinForce = data.Float("minForce", 0f);
        MaxForce = data.Float("maxForce", 300f);
        Damping = data.Float("damping", 1f);
    }

    public override void Update() {
        base.Update();

        foreach (Holdable hold in Scene.Tracker.GetComponents<Holdable>())
            if (!hold.IsHeld && hold.Entity is Entity e && e.CollideCheck(this))
                hold.SetSpeed(Affect(e, hold.GetSpeed()));

        if (AlsoAffectPlayer && Scene.Tracker.GetEntity<Player>() is Player p && p.CollideCheck(this))
            p.Speed = Affect(p, p.Speed);
    }

    private Vector2 Affect(Entity e, Vector2 speed) {
        float force;

        if (e.Get<GravityComponent>()?.ShouldInvert != true) {
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

}