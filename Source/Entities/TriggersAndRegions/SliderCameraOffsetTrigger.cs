using Celeste.Mod.Entities;
using Celeste.Mod.Microlith57Misc.Components;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity(
    "Microlith57Misc/SliderCameraOffsetTrigger=CreateFlag",
    "Microlith57Misc/SliderCameraOffsetTrigger_Expression=CreateExpr"
)]
public sealed class SliderCameraOffsetTrigger : CameraOffsetTrigger {

    #region --- State ---

    private bool Coarse;
    private Vector2 Coarseness => Coarse ? new(48f, 32f) : Vector2.One;

    private readonly ConditionSource Condition;
    public bool Enabled => Condition.Value;

    private Vector2Source OffsetSourceFrom, OffsetSourceTo;
    private PositionModes Mode;

    #endregion State
    #region --- Init ---

    public SliderCameraOffsetTrigger(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        Vector2Source offsetSourceFrom,
        Vector2Source offsetSourceTo
    ) : base(data, offset) {
        this.SetDepthAndTags(data);

        Mode = data.Enum("direction", PositionModes.NoEffect);
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
            Vector2Source.SliderSource(level.Session, data, "offsetFrom"),
            Vector2Source.SliderSource(level.Session, data, "offsetTo")
        );

    public static SliderCameraOffsetTrigger CreateExpr(Level level, LevelData _, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Expr(data, "enableExpression") { Default = true },
            Vector2Source.ExprSource(data, "offsetFrom"),
            Vector2Source.ExprSource(data, "offsetTo")
        );

    #endregion Init
    #region --- Behaviour ---

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

    #endregion Behaviour

}
