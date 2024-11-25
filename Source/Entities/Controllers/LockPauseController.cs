using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using System;

using Celeste.Mod.Microlith57Misc.Components;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity(
    "Microlith57Misc/LockPauseController=CreateFlag",
    "Microlith57Misc/LockPauseController_Expression=CreateExpr"
)]
public sealed class LockPauseController : Entity {

    #region --- State ---

    [Flags]
    public enum LockMode : byte {
        Nothing = 0,
        LockRetry = 1 << 0,
        LockSaveQuit = 1 << 1,
        LockRetryAndSaveQuit = LockRetry | LockSaveQuit,
        LockPauseMenu = 1 << 2
    }

    private readonly ConditionSource Condition;
    public bool LockActive => Condition.Value;

    public readonly LockMode Mode;
    public readonly bool UnlockWhenControllerRemoved;

    #endregion State
    #region --- Init ---

    public LockPauseController(
        Vector2 position,
        ConditionSource condition,
        LockMode mode,
        bool unlockWhenControllerRemoved
    ) : base(position) {

        Add(Condition = condition);
        Mode = mode;
        UnlockWhenControllerRemoved = unlockWhenControllerRemoved;
    }

    private static LockPauseController Create(EntityData data, Vector2 offset, ConditionSource condition)
        => new(
            data.Position + offset,
            condition,
            data.Enum<LockMode>("mode"),
            data.Bool("unlockWhenControllerRemoved", true)
        );

    public static LockPauseController CreateFlag(Level _, LevelData __, Vector2 offset, EntityData data)
        => Create(
                data, offset,
                new ConditionSource.FlagSource(data, ifAbsent: "lockPause") { Default = true }
            );

    public static LockPauseController CreateExpr(Level _, LevelData __, Vector2 offset, EntityData data)
        => Create(
                data, offset,
                new ConditionSource.ExpressionSource(data, ifAbsent: "lockPause") { Default = true }
            );

    #endregion Init
    #region --- Behaviour ---

    public override void Update() {
        base.Update();

        if (Scene is Level level) Set(level, LockActive);
    }

    public override void Removed(Scene scene) {
        base.Removed(scene);

        if (scene is Level level && UnlockWhenControllerRemoved) Set(level, false);
    }

    private void Set(Level level, bool locked) {
        if ((Mode & LockMode.LockRetry) != LockMode.Nothing)
            level.CanRetry = !locked;

        if ((Mode & LockMode.LockSaveQuit) != LockMode.Nothing)
            level.SaveQuitDisabled = locked;

        if ((Mode & LockMode.LockPauseMenu) != LockMode.Nothing)
            level.PauseLock = locked;
    }

    #endregion Behaviour

}
