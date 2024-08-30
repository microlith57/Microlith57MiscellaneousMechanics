using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

using Celeste.Mod.GravityHelper.Components;

namespace Celeste.Mod.Microlith57Misc.Entities.Recordings;

[Tracked]
public class BoxRecording : Recording {

    public record struct State(
        Vector2 Position,
        Vector2 ShakeOffset,
        bool Inverted,
        bool Held,
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

    public State? CurrentState => Timeline[FrameIndex - FrameOffset];
    public Vector2 ShakeOffset => CurrentState.HasValue ? CurrentState.Value.ShakeOffset : Vector2.Zero;
    public bool IsHeld => CurrentState.HasValue ? CurrentState.Value.Held : false;

    public Sprite Sprite;
    public VertexLight Light;
    public ParticleType Dust;
    public BoxSurface Surface;

    public float LastInteraction;

    public BoxRecording(ParticleType dust) {
        Depth = Depths.Top;

        Collider = new Hitbox(20f, 20f, -10f, -20f);
        Add(Light = new(Collider.Center, Color.White, 1f, 24, 48));
        Add(new AreaSwitch.Activator());

        Add(Sprite = new(GFX.Game, "objects/microlith57/misc/box_playback/"));
        Sprite.Add("normal", "normal", 1f, [0]);
        Sprite.Add("inverted", "inverted", 1f, [0]);
        Sprite.Play("normal");
        Sprite.CenterOrigin();
        Sprite.Position = Collider.Center;
        Sprite.Visible = false;

        Dust = dust;

        Add(Surface = new BoxSurface(
            Collider,
            width: 20,
            depth: 1001,
            surfaceIndex: SurfaceIndex.Glitch
        ));
        Module.OverrideDust(Surface.SurfaceTop, Dust);
        Module.OverrideDust(Surface.SurfaceBot, Dust);
    }

    public override void Observe(int currentFrame, Color baseColor) {
        if (Timeline.Count == 0)
            FrameOffset = currentFrame;
        else if (currentFrame != LastFrame + 1)
            throw new Exception("tried to record a box with non-contiguous lifetime");

        if (RecordingOf is Box box) {
            var grav = box.Get<GravityComponent>();

            Timeline.Add(new(
                Position: box.Position,
                ShakeOffset: box.ShakeOffset,
                Held: box.Hold.IsHeld,
                Inverted: grav?.ShouldInvert ?? false,
                BonkH: box.BonkedH, BonkV: box.BonkedV,
                Color: baseColor
            ));
        } else if (RecordingOf is BoxRecording recording) {
            Timeline.Add(recording.CurrentState!.Value);
        }
    }

    public override void BeginPlayback() {
        base.BeginPlayback();
        Surface.Collidable = true;
        AppearEffect(Center, 12, Vector2.One * 6f, Sprite.Color);

        LastInteraction = Scene.TimeActive;
    }

    public override void EndPlayback(bool remove) {
        DisappearEffect(Center, 12, Vector2.One * 6f, Sprite.Color);
        Surface.Collidable = false;
        base.EndPlayback(remove);
    }

    public void SetFrame(int index) {
        bool wasHeld = IsHeld;

        var state = Timeline[index - FrameOffset];

        if (Scene != null && wasHeld && !IsHeld)
            LastInteraction = Scene.TimeActive;

        Position = state.Position;

        Collider.Position = state.Inverted ? new(-10f, 0f) : new(-10f, -20f);
        Surface.Move();

        Sprite.Play(state.Inverted ? "inverted" : "normal");
        Sprite.Position = Collider.Center;
        Sprite.Color = state.Color;

        Light.Color = Color.Lerp(state.Color, Color.White, 0.5f);

        if (state.BonkH)
            Audio.Play("event:/new_content/char/tutorial_ghost/grab", Position);

        if (state.BonkV)
            Audio.Play("event:/new_content/char/tutorial_ghost/land", Position);
    }

    public override void RenderSprite() {
        Sprite.Texture.Draw(Sprite.RenderPosition + ShakeOffset, Sprite.Origin, Sprite.Color * 0.8f, Sprite.Scale, Sprite.Rotation, Sprite.Effects);
    }

    public void RenderOutline() {
        Sprite.Texture.Draw(Sprite.RenderPosition + ShakeOffset + new Vector2(1f, 1f), Sprite.Origin, Color.Black, Sprite.Scale, Sprite.Rotation, Sprite.Effects);
        Sprite.Texture.Draw(Sprite.RenderPosition + ShakeOffset + new Vector2(1f, -1f), Sprite.Origin, Color.Black, Sprite.Scale, Sprite.Rotation, Sprite.Effects);
        Sprite.Texture.Draw(Sprite.RenderPosition + ShakeOffset + new Vector2(-1f, 1f), Sprite.Origin, Color.Black, Sprite.Scale, Sprite.Rotation, Sprite.Effects);
        Sprite.Texture.Draw(Sprite.RenderPosition + ShakeOffset + new Vector2(-1f, -1f), Sprite.Origin, Color.Black, Sprite.Scale, Sprite.Rotation, Sprite.Effects);
    }

}