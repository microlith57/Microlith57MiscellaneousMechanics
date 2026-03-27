#if FEATURE_FLAG_RECORDINGS && FEATURE_FLAG_BOX

using System.Runtime.CompilerServices;

namespace Celeste.Mod.Microlith57Misc.Entities.Recordings;

[Tracked]
public class BoxRecording : Recording {

    public record struct State(
        Vector2 Center,
        Vector2 ShakeOffset,
        bool Inverted,
        bool Held,
        bool BonkH, bool BonkV,
        string Animation,
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

    public readonly bool GravityLocked;
    public readonly Sprite Sprite;
    public readonly VertexLight Light;
    public readonly ParticleType Dust;
    public readonly BoxSurface Surface;

    public float LastInteraction;

    public BoxRecording(ParticleType dust, bool gravityLocked) {
        GravityLocked = gravityLocked;
        Depth = Depths.Top;

        Collider = new Hitbox(20f, 20f, -10f, -10f);
        Add(Light = new(Vector2.Zero, Color.White, 1f, 24, 48));
        Add(new AreaSwitch.Activator());
        // Add(new PressureSensor.Activator());

        var spritePath = "objects/microlith57/misc/box/playback";
        if (gravityLocked)
            spritePath += "_locked";

        Add(Sprite = new(GFX.Game, spritePath));
        Sprite.Add("normal", "", 1f, [0]);
        Sprite.Add("inverted", "", 1f, [1]);
        Sprite.Add("shatter", "", 1f, [2]);
        Sprite.Play("normal");
        Sprite.CenterOrigin();
        Sprite.Visible = false;

        Dust = dust;

        Add(Surface = new BoxSurface(
            Collider,
            width: 20,
            depth: 1001,
            surfaceIndex: SurfaceIndex.Glitch
        ));
    }

    public override void Added(Scene scene) {
        base.Added(scene);

        OverrideDust(Surface.SurfaceTop!, Dust);
        OverrideDust(Surface.SurfaceBot!, Dust);
    }

    public override void Observe(int currentFrame, Color baseColor) {
        if (Timeline.Count == 0)
            FrameOffset = currentFrame;
        else if (currentFrame != LastFrame + 1)
            throw new Exception("tried to record a box with non-contiguous lifetime");

        if (RecordingOf is Box box) {
            Timeline.Add(new(
                Center: box.AbsCenter,
                ShakeOffset: box.ShakeOffset,
                Held: box.Hold.IsHeld,
                Inverted: box.ShouldInvert(),
                BonkH: box.BonkedH, BonkV: box.BonkedV,
                Animation: box.IndicatorSprite.CurrentAnimationID,
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

        Position = state.Center;
        Surface.Move();

        if (!string.IsNullOrEmpty(state.Animation))
            Sprite.Play(state.Animation);

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

    [OnLoad] internal static void Load() => On.Celeste.Player.DustParticleFromSurfaceIndex += hookDustParticle;
    [OnUnload] internal static void Unload() => On.Celeste.Player.DustParticleFromSurfaceIndex -= hookDustParticle;

    private static ConditionalWeakTable<Platform, ParticleType> platformDustOverrides = [];
    private static void OverrideDust(Platform platform, ParticleType particle) => platformDustOverrides.AddOrUpdate(platform, particle);

    private static ParticleType hookDustParticle(On.Celeste.Player.orig_DustParticleFromSurfaceIndex orig, Player self, int index) {
        if (index == SurfaceIndex.Glitch) {
            bool invert = self.ShouldInvert();
            var pos = self.Position + (invert ? -Vector2.UnitY : Vector2.UnitY);
            var platform = SurfaceIndex.GetPlatformByPriority(self.CollideAll<Platform>(pos));

            if (platform != null && platformDustOverrides.TryGetValue(platform, out var particle))
                return particle;
        }
        return orig(self, index);
    }

}

#endif
