using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57.IntContest.Recordings;

[Tracked]
public class BoxRecording : Recording {
    public record struct State(
        Vector2 Position,
        bool BonkH, bool BonkV,
        Color Color
    ) { }

    public List<State> Timeline = [];
    public int FrameOffset = 0;
    public override int? FirstFrame => Timeline.Count > 0 ? FrameOffset : null;
    public override int? LastFrame => Timeline.Count > 0 ? FirstFrame!.Value + Timeline.Count - 1 : null;

    private int currentFrame;
    public override int FrameIndex {
        get => currentFrame;
        set => SetFrame(currentFrame = value);
    }

    public State CurrentState => Timeline[FrameIndex - FrameOffset];

    public Image Sprite;
    public VertexLight Light;
    public JumpThru Surface;

    public BoxRecording() {
        Depth = 1010;

        Add(Sprite = new(GFX.Game["objects/INTcontest24/microlith57/box_playback"]) {
            Origin = new(11f, 21f)
        });

        Collider = new Hitbox(20f, 20f, -10f, -20f);
        Add(Light = new(Collider.Center, Color.White, 1f, 24, 48));
        Add(new AreaSwitch.Activator());

        Surface = new(Position + new Vector2(-10f, -20f), 20, safe: false) {
            Depth = 1009,
            SurfaceSoundIndex = SurfaceIndex.Glitch,
            Collidable = false
        };
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        Scene.Add(Surface);
    }

    public override void Observe(int currentFrame, Color baseColor) {
        if (Timeline.Count == 0)
            FrameOffset = currentFrame;
        else if (currentFrame != LastFrame + 1) {
#if DEBUG
            throw new Exception("tried to record a box with non-contiguous lifetime");
#else
            return null
#endif
        }

        if (RecordingOf is Box box) {
            Timeline.Add(new(
                box.Position,
                box.BonkedH,
                box.BonkedV,
                baseColor
            ));
        } else if (RecordingOf is BoxRecording recording) {
            Timeline.Add(recording.CurrentState);
        }
    }

    public override void BeginPlayback() {
        base.BeginPlayback();

        Surface.Collidable = true;

        if (Scene is Level level) {
            Audio.Play("event:/new_content/char/tutorial_ghost/appear", Position);
            level.Particles.Emit(P_Appear, 12, Center, Vector2.One * 6f, Sprite.Color);
        }
    }

    public override void EndPlayback(bool remove) {
        Surface.Collidable = false;

        if (Visible)
            Audio.Play("event:/new_content/char/tutorial_ghost/disappear", Position);

        if (Scene is Level level)
            level.Particles.Emit(P_Appear, 12, Center, Vector2.One * 6f, Sprite.Color);

        base.EndPlayback(remove);
    }

    public void SetFrame(int index) {
        State state = Timeline[index - FrameOffset];

        Position = state.Position;
        Surface.MoveTo(Position + new Vector2(-10f, -20f));

        Sprite.Color = state.Color;

        if (state.BonkH)
            Audio.Play("event:/new_content/char/tutorial_ghost/grab", Position);

        if (state.BonkV)
            Audio.Play("event:/new_content/char/tutorial_ghost/land", Position);
    }

    public override void Removed(Scene scene) {
        base.Removed(scene);
        Surface.RemoveSelf();
    }
}