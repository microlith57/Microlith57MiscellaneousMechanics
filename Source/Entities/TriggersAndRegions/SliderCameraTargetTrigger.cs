using Celeste.Mod.Entities;
using Celeste.Mod.Microlith57Misc.Components;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity(
    "Microlith57Misc/SliderCameraTargetTrigger=CreateFlag",
    "Microlith57Misc/SliderCameraTargetTrigger_Expression=CreateExpr"
)]
public sealed class SliderCameraTargetTrigger : CameraAdvanceTargetTrigger {

    #region --- State ---

    private readonly ConditionSource Condition;
    public bool Enabled => Condition.Value;

    private Vector2Source TargetSource, LerpStrengthSource;

    #endregion State
    #region --- Init ---

    public SliderCameraTargetTrigger(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        Vector2Source targetSource,
        Vector2Source lerpStrengthSource
    ) : base(AddDummyNode(data), offset) {

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

    #endregion Init
    #region --- Behaviour ---

    public override void Awake(Scene scene) {
        base.Awake(scene);

        if (Scene.Tracker.GetEntity<Player>() is Player player)
            player.PreUpdate += (_) => {
                Collidable = Enabled;
            };
    }

    public override void OnStay(Player player) {
        Target = TargetSource.Value - new Vector2(320 / 2, 180 / 2);
        LerpStrength = LerpStrengthSource.Value;
        base.OnStay(player);
    }

    private static EntityData AddDummyNode(EntityData data) {
        if (data.Nodes.Length == 0)
            data.Nodes = [data.Position + new Vector2(data.Width, data.Height) / 2f];
        return data;
    }

    #endregion Behaviour

}
