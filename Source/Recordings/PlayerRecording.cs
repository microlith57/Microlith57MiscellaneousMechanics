using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57.IntContest.Recordings;

[Tracked]
public class PlayerRecording : Recording {
    public record struct State(
        Player.ChaserState Underlying,
        Vector2 LightOffset,
        Color Color,
        Rectangle Collider
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

    public PlayerSprite Sprite;
    public PlayerHair Hair;
    public VertexLight Light;

    public PlayerRecording() {
        Sprite = new PlayerSprite(PlayerSpriteMode.Playback);
        Add(Hair = new PlayerHair(Sprite));
        Add(Sprite);

        Collider = new Hitbox(8f, 11f, -4f, -11f);

        Add(Light = new(Color.White, 1f, 32, 64));

        Add(new AreaSwitch.Activator());

        Depth = 1000;
    }

    public override void Observe(int currentFrame, Color baseColor) {
        if (Timeline.Count == 0)
            FrameOffset = currentFrame;
        else if (currentFrame != LastFrame + 1) {
#if DEBUG
            throw new Exception("tried to record a player with non-contiguous lifetime");
#else
            return null
#endif
        }

        if (RecordingOf is Player player) {
            Timeline.Add(new(
                Underlying: player.ChaserStates[^1],
                LightOffset: player.Light.Position,
                Color: baseColor,
                Collider: new(
                    (int)player.Collider.Position.X,
                    (int)player.Collider.Position.Y,
                    (int)player.Collider.Width,
                    (int)player.Collider.Height
                )
            ));
        } else if (RecordingOf is PlayerRecording recording) {
            Timeline.Add(recording.CurrentState);
        }
    }

    public override void BeginPlayback() {
        base.BeginPlayback();

        for (int i = 0; i < 10; i++)
            Hair.AfterUpdate();

        if (Scene is Level level) {
            Audio.Play("event:/new_content/char/tutorial_ghost/appear", Position);
            level.Particles.Emit(P_Appear, 12, Center, Vector2.One * 6f, Sprite.Color);
        }
    }

    public override void EndPlayback(bool remove) {
        if (Visible)
            Audio.Play("event:/new_content/char/tutorial_ghost/disappear", Position);

        if (Scene is Level level)
            level.Particles.Emit(P_Appear, 12, Center, Vector2.One * 6f, Sprite.Color);

        base.EndPlayback(remove);
    }

    public void SetFrame(int index) {
        State state = Timeline[index - FrameOffset];

        string currentAnimationID = Sprite.CurrentAnimationID;
        bool onGround = Scene != null && CollideCheck<Solid>(Position + new Vector2(0f, 1f));

        Position = state.Underlying.Position;
        Collider.Position.X = state.Collider.X;
        Collider.Position.Y = state.Collider.Y;
        Collider.Width = state.Collider.Width;
        Collider.Height = state.Collider.Height;

        var anim = state.Underlying.Animation;
        if (anim != null && anim != Sprite.CurrentAnimationID && Sprite.Has(anim))
            Sprite.Play(anim, restart: true);

        Sprite.Scale = state.Underlying.Scale;
        if (Sprite.Scale.X != 0f)
            Hair.Facing = (Facings)Math.Sign(Sprite.Scale.X);

        Sprite.Color = Hair.Color = state.Color;

        Light.Position = state.LightOffset;

        if (Scene == null)
            return;

        if (!onGround && CollideCheck<Solid>(Position + new Vector2(0f, 1f)))
            Audio.Play("event:/new_content/char/tutorial_ghost/land", Position);

        if (currentAnimationID == Sprite.CurrentAnimationID)
            return;

        string animID = Sprite.CurrentAnimationID;
        int animFrame = Sprite.CurrentAnimationFrame;
        switch (animID) {
            case "jumpFast":
            case "jumpSlow":
                Audio.Play("event:/new_content/char/tutorial_ghost/jump", Position);
                break;
            case "dreamDashIn":
                Audio.Play("event:/new_content/char/tutorial_ghost/dreamblock_sequence", Position);
                break;
            case "dash":
                if (state.Underlying.DashDirection.Y != 0f) {
                    Audio.Play("event:/new_content/char/tutorial_ghost/jump_super", Position);
                } else if (state.Underlying.Scale.X > 0f) {
                    Audio.Play("event:/new_content/char/tutorial_ghost/dash_red_right", Position);
                } else {
                    Audio.Play("event:/new_content/char/tutorial_ghost/dash_red_left", Position);
                }

                break;
            case "climbUp":
            case "climbDown":
            case "wallslide":
                Audio.Play("event:/new_content/char/tutorial_ghost/grab", Position);
                break;
            case "carryTheoWalk":
            case "runSlow_carry":
            case "runFast":
            case "runSlow":
            case "runWind":
            case "walk":
                if (animFrame == 0 || animFrame == 6)
                    Audio.Play("event:/new_content/char/tutorial_ghost/footstep", Position);
                break;
            case "runStumble":
                if (animFrame == 6)
                    Audio.Play("event:/new_content/char/tutorial_ghost/footstep", Position);
                break;
            case "flip":
                if (animFrame == 4)
                    Audio.Play("event:/new_content/char/tutorial_ghost/footstep", Position);
                break;
            case "idleC":
                if (Sprite.Mode == PlayerSpriteMode.MadelineNoBackpack &&
                    (animFrame == 3 || animFrame == 6 || animFrame == 8 || animFrame == 11))

                    Audio.Play("event:/new_content/char/tutorial_ghost/footstep", Position);
                break;
            case "push":
                if (animFrame == 8 || animFrame == 15)
                    Audio.Play("event:/new_content/char/tutorial_ghost/footstep", Position);
                break;
        }
    }

    public override void Update() {
        base.Update();

        if (Visible && Scene != null && Scene.OnInterval(0.1f))
            TrailManager.Add(Position, Sprite, Hair, Sprite.Scale, Hair.Color, Depth + 1);
    }
}