
using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;

using Celeste.Mod.Microlith57Misc.Components;
using System.Collections.Generic;
using System;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity(
    "Microlith57Misc/SliderFadeTriggerTrigger=Create",
    "Microlith57Misc/SliderFadeTriggerTrigger_Expression=CreateExpr"
)]
public sealed class SliderFadeTriggerTrigger : Trigger {

    public enum OutOfBoundsBehaviour {
        Clamp,
        Disable
    }

    private readonly ConditionSource EnabledCondition;
    public bool Enabled => EnabledCondition.Value;

    private FloatSource LerpSource;
    public float Lerp => LerpSource.Value;
    public readonly float MapFrom, MapTo;
    public readonly Vector2 PosFrom, PosTo;
    public readonly OutOfBoundsBehaviour OOBBehaviour;

    private List<(Trigger trigger, bool pointWasInside)> Triggers = [];

    public SliderFadeTriggerTrigger(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        FloatSource lerpSource
    ) : base(data, offset) {
        EnabledCondition = enabledCondition;
        LerpSource = lerpSource;

        MapFrom = data.Float("mapFrom", 0f);
        MapTo = data.Float("mapTo", 1f);

        var nodes = data.NodesOffset(offset);
        if (nodes.Length < 2)
            throw new Exception("not enough nodes!");
        PosFrom = nodes[0];
        PosTo = nodes[1];
        if (PosFrom.X != PosTo.X && PosFrom.Y != PosTo.Y)
            throw new Exception("nodes must be aligned on the x or y axis!");

        OOBBehaviour = data.Enum("oobBehaviour", OutOfBoundsBehaviour.Clamp);
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);

        foreach (Trigger trigger in Scene.Tracker.GetEntities<Trigger>()) {
            if (trigger.Collider is Hitbox hitbox && hitbox.Collide(PosFrom, PosTo))
                Triggers.Add((trigger, false));
        }
    }

    public override void Update() {
        base.Update();
        Triggers.RemoveAll(t => t.trigger.Scene is null);
    }

    public override void OnEnter(Player player) {
        base.OnEnter(player);
        var prevPosition = player.Position;

        var unclamped = Vector2.Lerp(PosFrom, PosTo, Calc.ClampedMap(Lerp, MapFrom, MapTo));
        foreach ((Trigger trigger, bool wasInside) in Triggers)
            if (SetPositionForTrigger(player, trigger, unclamped))
                trigger.OnEnter(player);

        player.Position = prevPosition;
    }

    public override void OnStay(Player player) {
        base.OnStay(player);
        var prevPosition = player.Position;

        var unclamped = Vector2.Lerp(PosFrom, PosTo, Calc.ClampedMap(Lerp, MapFrom, MapTo));
        foreach ((Trigger trigger, bool wasInside) in Triggers)
            if (SetPositionForTrigger(player, trigger, unclamped))
                trigger.OnStay(player);

        player.Position = prevPosition;
    }

    public override void OnLeave(Player player) {
        base.OnLeave(player);
        var prevPosition = player.Position;

        var unclamped = Vector2.Lerp(PosFrom, PosTo, Calc.ClampedMap(Lerp, MapFrom, MapTo));
        foreach ((Trigger trigger, bool wasInside) in Triggers) {
            SetPositionForTrigger(player, trigger, unclamped);
            trigger.OnLeave(player);
        }

        player.Position = prevPosition;
    }

    private bool SetPositionForTrigger(Player player, Trigger trigger, Vector2 unclamped) {
        if (trigger.Collider is not Hitbox hitbox) return false;

        var left = hitbox.AbsoluteLeft;
        var right = hitbox.AbsoluteRight;
        var top = hitbox.AbsoluteTop;
        var bot = hitbox.AbsoluteBottom;

        Monocle.Collide.RectToPoint(left, top, hitbox.Width, hitbox.Height, unclamped);
    }
    
}
