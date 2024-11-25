using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

using Celeste.Mod.Microlith57Misc.Components;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity(
    "Microlith57Misc/SetFacingTrigger=CreateFlag",
    "Microlith57Misc/SetFacingTrigger_Expression=CreateExpr"
)]
public sealed class SetFacingTrigger : Trigger {

    #region --- State ---

    private readonly ConditionSource Condition;
    public bool SetFacingActive => Condition.Value;

    public Facings Facing;
    public bool InvertIfUnset;
    public bool Continuous;

    #endregion State
    #region --- Init ---

    public SetFacingTrigger(
        EntityData data, Vector2 offset,
        ConditionSource condition,
        Facings facing,
        bool invertIfUnset,
        bool continuous
    ) : base(data, offset) {

        Add(Condition = condition);
        Facing = facing;
        InvertIfUnset = invertIfUnset;
        Continuous = continuous;
    }

    private static SetFacingTrigger Create(EntityData data, Vector2 offset, ConditionSource condition)
        => new(
            data, offset,
            condition,
            data.Enum<Facings>("direction"),
            data.Bool("invertIfUnset"),
            data.Bool("continuous")
        );

    public static SetFacingTrigger CreateFlag(Level _, LevelData __, Vector2 offset, EntityData data)
        => Create(data, offset, new ConditionSource.FlagSource(data) { Default = true });

    public static SetFacingTrigger CreateExpr(Level _, LevelData __, Vector2 offset, EntityData data)
        => Create(data, offset, new ConditionSource.ExpressionSource(data) { Default = true });

    public override void Added(Scene scene) {
        base.Added(scene);
        Add(Condition);
    }

    #endregion Init
    #region --- Behaviour ---

    public override void OnEnter(Player player) {
        base.OnEnter(player);
        SetFacing(player);
    }

    public override void Update() {
        base.Update();

        if (Continuous && PlayerIsInside &&
            (Scene as Level)?.Tracker?.GetEntity<Player>() is Player player)

            SetFacing(player);
    }

    private void SetFacing(Player player) {
        if (SetFacingActive)
            player.Facing = Facing;
        else if (InvertIfUnset)
            player.Facing = (Facing == Facings.Left) ? Facings.Right : Facings.Left;
    }

    #endregion Behaviour

}