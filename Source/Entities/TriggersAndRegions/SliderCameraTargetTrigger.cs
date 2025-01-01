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

    public enum SnapMode {
        NeverSnap,
        SnapWhenInitiallyEnabled,
        AlwaysSnap,
        AlwaysSnapIgnoringRoomBounds
    }

    private readonly ConditionSource Condition;
    public bool Enabled => Condition.Value;

    private Vector2Source TargetSource, LerpStrengthSource;

    public readonly SnapMode Snap;
    private bool JustEnabled = false;

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

        Snap = data.Enum("snapMode", SnapMode.NeverSnap);
    }


    public static SliderCameraTargetTrigger CreateFlag(Level level, LevelData _, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Flag(data, "enableFlag", invertName: "invertFlag") { Default = true },
            Vector2Source.SliderSource(level.Session, data, "target"),
            Vector2Source.SliderSource(level.Session, data, "lerpStrength")
        );

    public static SliderCameraTargetTrigger CreateExpr(Level level, LevelData _, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Expr(data, "enableExpression") { Default = true },
            Vector2Source.ExprSource(data, "target"),
            Vector2Source.ExprSource(data, "lerpStrength")
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

    public override void Update() {
        base.Update();
        Collidable = Enabled;
    }

    public override void OnEnter(Player player) {
        base.OnEnter(player);
        JustEnabled = true;
    }

    public override void OnStay(Player player) {
        Target = TargetSource.Value - new Vector2(320 / 2, 180 / 2);
        LerpStrength = LerpStrengthSource.Value;

        base.OnStay(player);

        if (Snap != SnapMode.NeverSnap &&
            (Snap != SnapMode.SnapWhenInitiallyEnabled || JustEnabled) &&
            Scene is Level level
        ) {
            var origEnforceLevelBounds = player.EnforceLevelBounds;
            player.EnforceLevelBounds = Snap != SnapMode.AlwaysSnapIgnoringRoomBounds;
            level.Camera.Position = player.CameraTarget;
            player.EnforceLevelBounds = origEnforceLevelBounds;
        }

        JustEnabled = false;
    }

    private static EntityData AddDummyNode(EntityData data) {
        if (data.Nodes.Length == 0)
            data.Nodes = [data.Position + new Vector2(data.Width, data.Height) / 2f];
        return data;
    }

    #endregion Behaviour

}
