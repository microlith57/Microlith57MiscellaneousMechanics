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

    private Level Level => (Scene as Level)!;

    public override void Update() {
        base.Update();

        Set(Level.Session.GetFlag(Flag) ^ InvertFlag);
    }

    public override void Removed(Scene scene) {
        base.Removed(scene);

        if (UnlockOnControllerRemoved) Set(false);
    }

    private void Set(bool locked) {
        if ((Mode & LockMode.LockRetry) != LockMode.Nothing)
            Level.CanRetry = !locked;

        if ((Mode & LockMode.LockSaveQuit) != LockMode.Nothing)
            Level.SaveQuitDisabled = locked;

        if ((Mode & LockMode.LockPauseMenu) != LockMode.Nothing)
            Level.PauseLock = locked;
    }

}
