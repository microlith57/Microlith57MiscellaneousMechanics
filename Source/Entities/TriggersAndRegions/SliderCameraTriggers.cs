using Celeste.Mod.Entities;
using Celeste.Mod.Microlith57Misc.Components;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Entities;

#region --- Target ---

[CustomEntity(
    "Microlith57Misc/SliderCameraTargetTrigger=CreateFlag",
    "Microlith57Misc/SliderCameraTargetTrigger_Expression=CreateExpr"
)]
public sealed class SliderCameraTargetTrigger : CameraAdvanceTargetTrigger {

    private readonly ConditionSource Condition;
    public bool Enabled => Condition.Value;

    private Vector2Source TargetSource, LerpStrengthSource;

    public SliderCameraTargetTrigger(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        Vector2Source targetSource,
        Vector2Source lerpStrengthSource
    ) : base(data, offset) {

        Add(Condition = enabledCondition);
        this.Add(TargetSource = targetSource);
        this.Add(LerpStrengthSource = lerpStrengthSource);

        TargetSource.Default = Target;
        LerpStrengthSource.Default = LerpStrength;
    }

    public static SliderCameraTargetTrigger CreateFlag(Level level, LevelData _, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Flag(data, "enableFlag", invertName: "invertFlag") { Default = true },
            Vector2Source.SliderSource(level.Session, data, "targetSlider"),
            Vector2Source.SliderSource(level.Session, data, "lerpStrengthSlider")
        );

    public static SliderCameraTargetTrigger CreateExpr(Level level, LevelData _, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Expr(data, "enableExpression") { Default = true },
            Vector2Source.ExprSource(data, "targetExpression"),
            Vector2Source.ExprSource(data, "lerpStrengthExpression")
        );

    public override void OnStay(Player player) {
        Target = TargetSource.Value;
        LerpStrength = LerpStrengthSource.Value;
        base.OnStay(player);
    }

}

#endregion Target
#region --- Offset ---

[CustomEntity(
    "Microlith57Misc/SliderCameraOffsetTrigger=CreateFlag",
    "Microlith57Misc/SliderCameraOffsetTrigger_Expression=CreateExpr"
)]
public sealed class SliderCameraOffsetTrigger : CameraOffsetTrigger {

    private bool Coarse;
    private Vector2 Coarseness => Coarse ? new(48f, 32f) : Vector2.One;

    private readonly ConditionSource Condition;
    public bool Enabled => Condition.Value;

    private Vector2Source OffsetSourceFrom, OffsetSourceTo;
    private PositionModes Mode;

    public SliderCameraOffsetTrigger(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        Vector2Source offsetSourceFrom,
        Vector2Source offsetSourceTo
    ) : base(data, offset) {

        Coarse = data.Bool("coarse");

        Add(Condition = enabledCondition);
        this.Add(OffsetSourceFrom = offsetSourceFrom);
        this.Add(OffsetSourceTo = offsetSourceTo);

        OffsetSourceFrom.Default = CameraOffset;
        OffsetSourceTo.Default = CameraOffset;
    }

    public static SliderCameraOffsetTrigger CreateFlag(Level level, LevelData _, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Flag(data, "enableFlag", invertName: "invertFlag") { Default = true },
            Vector2Source.SliderSource(level.Session, data, "offsetFromSlider"),
            Vector2Source.SliderSource(level.Session, data, "offsetToSlider")
        );

    public static SliderCameraOffsetTrigger CreateExpr(Level level, LevelData _, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Expr(data, "enableExpression") { Default = true },
            Vector2Source.ExprSource(data, "offsetFromExpression"),
            Vector2Source.ExprSource(data, "offsetToExpression")
        );

    private Vector2 GetOffset(Player player)
        => Calc.LerpSnap(OffsetSourceFrom.Value, OffsetSourceTo.Value, GetPositionLerp(player, Mode), snapThresholdSq: 0f) * Coarseness;

    public override void OnEnter(Player player) {
        CameraOffset = GetOffset(player);
        base.OnEnter(player);
    }

    public override void OnStay(Player player) {
        SceneAs<Level>().CameraOffset = CameraOffset = GetOffset(player);
        base.OnEnter(player);
    }

}

#endregion Offset