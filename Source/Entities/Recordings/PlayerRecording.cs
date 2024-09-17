using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Entities.Recordings;

[Tracked]
public class PlayerRecording : Recording {

    public record struct State(
        Player.ChaserState Underlying,
        bool Inverted,
        Vector2 LightOffset,
        Color Color,
        Rectangle Collider,
        Vector2[] HairNodes
    ) { }

    public List<State> Timeline = [];
    public int FrameOffset = 0;
    public override int? FirstFrame => Timeline.Count > 0 ? FrameOffset : null;
    public override int? LastFrame => Timeline.Count > 0 ? FirstFrame!.Value + Timeline.Count - 1 : null;

    private int currentFrame;
    public override int FrameIndex {
        get => currentFrame;
        set => SetFrame(currentFrame = value, false);
    }

    public State CurrentState => Timeline[FrameIndex - FrameOffset];

    public PlayerSprite Sprite;
    public PlayerHair Hair;
    public VertexLight Light;

    public PlayerRecording(int hairCount) {
        Depth = Depths.Top;

        if (Utils.GravityComponentIfExists() is Component c) Add(c);

        Sprite = new PlayerSprite(PlayerSpriteMode.Playback) { HairCount = hairCount, Visible = false };
        Add(Hair = new PlayerHair(Sprite) { Active = false, Visible = false });
        Add(Sprite);

        Collider = new Hitbox(8f, 11f, -4f, -11f);

        Add(Light = new(Color.White, 1f, 32, 64));

        Add(new AreaSwitch.Activator());
        Add(new PressureSensor.Activator());
    }

    public override void Observe(int currentFrame, Color baseColor) {
        if (Timeline.Count == 0)
            FrameOffset = currentFrame;
        else if (currentFrame != LastFrame + 1)
            throw new Exception("tried to record a player with non-contiguous lifetime");

        if (RecordingOf is Player player) {
            Timeline.Add(new(
                Underlying: player.ChaserStates[^1],
                Inverted: player.ShouldInvert(),
                LightOffset: player.Light.Position,
                Color: baseColor,
                Collider: new(
                    (int)player.Collider.Position.X,
                    (int)player.Collider.Position.Y,
                    (int)player.Collider.Width,
                    (int)player.Collider.Height
                ),
                HairNodes: [.. player.Hair.Nodes]
            ));
        } else if (RecordingOf is PlayerRecording recording) {
            Timeline.Add(recording.CurrentState);
        }
    }

    public override void BeginPlayback() {
        base.BeginPlayback();
        AppearEffect(Center, 12, Vector2.One * 6f, Sprite.Color);
    }

    public override void EndPlayback(bool remove) {
        DisappearEffect(Center, 12, Vector2.One * 6f, Sprite.Color);
        base.EndPlayback(remove);
    }

    public void SetFrame(int index, bool silent) {
        var state = Timeline[index - FrameOffset];

        var currentAnimationID = Sprite.CurrentAnimationID;
        var onGround = Scene != null && CollideCheck<Solid>(Position + new Vector2(0f, 1f));

        Position = state.Underlying.Position;
        Collider.Position.X = state.Collider.X;
        Collider.Position.Y = state.Collider.Y;
        Collider.Width = state.Collider.Width;
        Collider.Height = state.Collider.Height;

        var anim = state.Underlying.Animation;
        if (anim != null && anim != Sprite.CurrentAnimationID && Sprite.Has(anim))
            Sprite.Play(anim, restart: true);

        this.SetInverted(state.Inverted, "Player Recording Inversion");

        Sprite.Scale = state.Underlying.Scale;
        if (state.Inverted)
            Sprite.Scale.Y = -Sprite.Scale.Y;

        if (Sprite.Scale.X != 0f)
            Hair.Facing = (Facings)Math.Sign(Sprite.Scale.X);

        Sprite.Color = Hair.Color = state.Color;

        for (var i = 0; i < state.HairNodes.Length; i++)
            Hair.Nodes[i] = state.HairNodes[i];

        Light.Position = state.LightOffset;
        Light.Color = Color.Lerp(state.Color, Color.White, 0.5f);

        if (silent || Scene == null)
            return;

        if (!onGround && CollideCheck<Solid>(Position + new Vector2(0f, 1f)))
            Audio.Play("event:/new_content/char/tutorial_ghost/land", Position);

        if (currentAnimationID == Sprite.CurrentAnimationID)
            return;

        var animID = Sprite.CurrentAnimationID;
        var animFrame = Sprite.CurrentAnimationFrame;
        switch (animID) {
            case "jumpFast":
            case "jumpSlow":
                Audio.Play("event:/new_content/char/tutorial_ghost/jump", Position);
                break;
            case "dreamDashIn":
                Audio.Play("event:/new_content/char/tutorial_ghost/dreamblock_sequence", Position);
                break;
            case "dash":
                if (state.Underlying.DashDirection.Y != 0f)
                    Audio.Play("event:/new_content/char/tutorial_ghost/jump_super", Position);
                else if (state.Underlying.Scale.X > 0f)
                    Audio.Play("event:/new_content/char/tutorial_ghost/dash_red_right", Position);
                else
                    Audio.Play("event:/new_content/char/tutorial_ghost/dash_red_left", Position);

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

    public override void RenderSprite() {
        base.RenderSprite();

        Hair.Render();
        Sprite.Render();
    }

    public override void Update() {
        base.Update();

        if (IsPlaying && Scene != null && Scene.OnInterval(0.1f))
            TrailManager.Add(Position, Sprite, Hair, Sprite.Scale, Hair.Color, 1001);
    }

}