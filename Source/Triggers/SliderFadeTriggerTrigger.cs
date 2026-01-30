
using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;

using Celeste.Mod.Microlith57Misc.Components;

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
    public float MapFrom, MapTo;
    public OutOfBoundsBehaviour OOBBehaviour;

    public SliderFadeTriggerTrigger(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        FloatSource lerpSource
    ) : base(data, offset) {
        EnabledCondition = enabledCondition;
        LerpSource = lerpSource;

        MapFrom = data.Float("mapFrom", 0f);
        MapTo = data.Float("mapTo", 1f);

        OOBBehaviour = data.Enum("oobBehaviour", OutOfBoundsBehaviour.Clamp);
    }

    public override void OnEnter(Player player) {
        base.OnEnter(player);
    }

    public override void OnStay(Player player) {
        base.OnStay(player);
    }

    public override void OnLeave(Player player) {
        base.OnLeave(player);
    }

}
