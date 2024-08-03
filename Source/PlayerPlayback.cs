using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57.IntContest;

[Tracked]
public class PlayerPlayback : Entity {

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public static ParticleType P_Appear;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public record struct State(
        Player.ChaserState Underlying,
        Vector2 LightOffset,
        float TimeStamp
    ) { }

    public List<State> Timeline;

    public PlayerSprite Sprite;
    public PlayerHair Hair;
    public VertexLight Light;

    public int FrameIndex = 0;
    public int FrameCount => Timeline.Count;

    public float Time = 0f;
    public float Duration => FrameCount == 0 ? 0 : Timeline[^1].TimeStamp;

    public bool Playing => Visible;

    public PlayerPlayback(Vector2 start, List<State>? timeline = null) {
        Collider = new Hitbox(8f, 11f, -4f, -11f);
        Timeline = timeline ?? [];
        Position = start;

        Sprite = new PlayerSprite(PlayerSpriteMode.Playback);
        Add(Hair = new PlayerHair(Sprite));
        Add(Sprite);

        Collider = new Hitbox(8f, 4f, -4f, -4f);

        Add(Light = new(Color.White, 1f, 32, 64));

        Add(new AreaSwitch.Activator());

        Depth = 1000;
        Visible = false;
    }

    public void Observe(Player player) {
        var chaserState = player.ChaserStates[^1];
        float time = FrameCount == 0 ? 0f : chaserState.TimeStamp - Timeline[0].Underlying.TimeStamp;

        Timeline.Add(new(
            Underlying: chaserState,
            LightOffset: player.Light.Position,
            TimeStamp: time
        ));
    }

    public void BeginPlayback() {
        Audio.Play("event:/new_content/char/tutorial_ghost/appear", Position);
        Visible = true;

        FrameIndex = 0;
        Time = 0f;
        SetFrame(0);
        for (int i = 0; i < 10; i++) {
            Hair.AfterUpdate();
        }

        if (Scene is Level level)
            level.Particles.Emit(P_Appear, 12, Center, Vector2.One * 6f, Sprite.Color);
    }

    public void EndPlayback() {
        if (Visible)
            Audio.Play("event:/new_content/char/tutorial_ghost/disappear", Position);

        if (Scene is Level level)
            level.Particles.Emit(P_Appear, 12, Center, Vector2.One * 6f, Sprite.Color);

        Visible = false;
        RemoveSelf();
    }

    public void SetFrame(int index) {
        State state = Timeline[index];

        string currentAnimationID = Sprite.CurrentAnimationID;
        bool onGround = Scene != null && CollideCheck<Solid>(Position + new Vector2(0f, 1f));

        Position = state.Underlying.Position;

        var anim = state.Underlying.Animation;
        if (anim != null && anim != Sprite.CurrentAnimationID && Sprite.Has(anim))
            Sprite.Play(anim, restart: true);

        Sprite.Scale = state.Underlying.Scale;
        if (Sprite.Scale.X != 0f)
            Hair.Facing = (Facings)Math.Sign(Sprite.Scale.X);

        Sprite.Color = Hair.Color = state.Underlying.HairColor;

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

        if (!Visible)
            return;

        if (FrameIndex >= Timeline.Count - 1 || Time >= Duration) {
            EndPlayback();
            return;
        }

        SetFrame(FrameIndex);
        Time += Engine.DeltaTime;
        while (FrameIndex < Timeline.Count - 1 && Time >= Timeline[FrameIndex + 1].TimeStamp)
            FrameIndex++;

        if (Scene != null && Scene.OnInterval(0.1f))
            TrailManager.Add(Position, Sprite, Hair, Sprite.Scale, Hair.Color, Depth + 1);
    }
}