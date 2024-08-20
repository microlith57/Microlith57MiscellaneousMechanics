using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57.IntContest.Entities.Recordings;

[Tracked(inherited: true)]
public abstract class Recording : Entity {

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public static ParticleType P_Appear;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public Entity? RecordingOf;

    public abstract int? FirstFrame { get; }
    public abstract int? LastFrame { get; }

    public abstract int FrameIndex { get; set; }

    public bool IsRecording => RecordingOf != null;
    public bool IsPlaying => Visible;

    public Recording() {
        Visible = false;
        Collidable = false;
    }

    public abstract void Observe(int currentFrame, Color baseColor);

    public override void Update() {
        base.Update();

        if (RecordingOf != null && RecordingOf.Scene == null)
            EndRecording();
    }

    public void BeginRecording(Entity toRecord) {
        RecordingOf = toRecord;
    }

    public void EndRecording() {
        RecordingOf = null;
    }

    public virtual void BeginPlayback() {
        if (IsRecording) EndRecording();

        FrameIndex = FirstFrame!.Value;
        Visible = true;
        Collidable = true;
    }

    public virtual void EndPlayback(bool remove) {
        Visible = false;
        Collidable = false;

        if (remove) RemoveSelf();
    }

    public void AppearEffect(Vector2 position, int count, Vector2 range, Color color) {
        if (Scene is not Level level) return;

        Audio.Play("event:/new_content/char/tutorial_ghost/appear", position);
        level.Particles.Emit(P_Appear, count, Center, range, color);
    }

    public void DisappearEffect(Vector2 position, int count, Vector2 range, Color color) {
        if (Scene is not Level level) return;

        Audio.Play("event:/new_content/char/tutorial_ghost/disappear", Position);
        level.Particles.Emit(P_Appear, count, Center, range, color);
    }

}