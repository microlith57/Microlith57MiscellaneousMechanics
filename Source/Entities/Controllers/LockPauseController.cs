using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using System;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity("Microlith57Misc/LockPauseController")]
public sealed class LockPauseController(EntityData data, Vector2 offset) : Entity(data.Position + offset) {

    [Flags]
    public enum LockMode : byte {
        Nothing = 0,
        LockRetry = 1 << 0,
        LockSaveQuit = 1 << 1,
        LockRetryAndSaveQuit = LockRetry | LockSaveQuit,
        LockPauseMenu = 1 << 2
    }

    public string Flag = data.Attr("flag", "lockPause");
    public bool InvertFlag = data.Bool("invertFlag");

    public LockMode Mode = data.Enum<LockMode>("mode");
    public bool UnlockOnControllerRemoved = data.Bool("unlockWhenControllerRemoved", true);

    public override void Update() {
        base.Update();

        if (Scene is not Level level) return;

        Set(level, level.Session.GetFlag(Flag) ^ InvertFlag);
    }

    public override void Removed(Scene scene) {
        base.Removed(scene);

        if (scene is Level level && UnlockOnControllerRemoved) Set(level, false);
    }

    private void Set(Level level, bool locked) {
        if ((Mode & LockMode.LockRetry) != LockMode.Nothing)
            level.CanRetry = !locked;

        if ((Mode & LockMode.LockSaveQuit) != LockMode.Nothing)
            level.SaveQuitDisabled = locked;

        if ((Mode & LockMode.LockPauseMenu) != LockMode.Nothing)
            level.PauseLock = locked;
    }

}
