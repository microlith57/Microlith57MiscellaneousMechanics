using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using System;

using Celeste.Mod.Microlith57Misc.Components;
using MonoMod.Utils;

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
        LockPauseMenu = 1 << 2,
        LockRetrySaveQuitAndPauseMenu = LockRetryAndSaveQuit | LockPauseMenu
    }

    private readonly ConditionSource Condition;
    public bool LockActive => Condition.Value;

    public readonly LockMode Mode;
    public readonly bool UnlockWhenControllerRemoved;
    public readonly bool InhibitGBJPrevention;

    #endregion State
    #region --- Init ---

    public LockPauseController(
        Vector2 position,
        ConditionSource condition,
        LockMode mode,
        bool unlockWhenControllerRemoved,
        bool inhibitGBJPrevention
    ) : base(position) {
        Add(Condition = condition);
        Mode = mode;
        UnlockWhenControllerRemoved = unlockWhenControllerRemoved;
        InhibitGBJPrevention = inhibitGBJPrevention;
    }

    private static LockPauseController Create(EntityData data, Vector2 offset, ConditionSource condition)
        => new LockPauseController(
            data.Position + offset,
            condition,
            data.Enum<LockMode>("mode"),
            data.Bool("unlockWhenControllerRemoved", true),
            data.Bool("inhibitGBJPrevention")
        ).SetDepthAndTags(data);

    public static LockPauseController CreateFlag(Level _, LevelData __, Vector2 offset, EntityData data)
        => Create(
            data, offset,
            new ConditionSource.Flag(data, ifAbsent: "lockPause") { Default = true }
        );

    public static LockPauseController CreateExpr(Level _, LevelData __, Vector2 offset, EntityData data)
        => Create(
            data, offset,
            new ConditionSource.Expr(data, ifAbsent: "lockPause") { Default = true }
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

        if (InhibitGBJPrevention && locked && level.Wipe == null && level.Tracker.GetEntity<Player>() is Player player)
            DynamicData.For(player).Set("framesAlive", int.MaxValue);
    }

    #endregion Behaviour

}
