using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using Monocle;

using Celeste.Mod.Microlith57Misc.Components;
using Microsoft.Xna.Framework.Graphics;
using Celeste.Mod.UI;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity(
    "Microlith57Misc/BlackHoleRefractionController=Create"
)]
[Tracked]
public sealed class BlackHoleRefractionController : SliderController {
    #region --- Init ---

    private static VirtualRenderTarget? RefractionFill;

    public static readonly Vector2 FillSize = new(320, 180);

    public Color Tint;
    public Vector2 Spacing;

    public BlackHoleRefractionController(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        FloatSource valueSource
    ) : base(data, offset, enabledCondition, valueSource) {
        Tag |= TagsExt.SubHUD;

        Add(new BeforeRenderHook(BeforeRender));

        Tint = Color.White;
        Spacing = new(200, 200);

        Depth = Depths.Top;
    }

    public static BlackHoleRefractionController Create(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Flag(data, name: "enabled", invertName: "invertEnabled") { Default = true },
            new FloatSource.Slider(level.Session, data, name: "amount")
        );

    #endregion
    #region --- Behaviour ---

    public override void Added(Scene scene) {
        base.Added(scene);

        if (Scene is not Level level) return;

        level.Particles.Visible = false;
        level.ParticlesBG.Visible = false;
        level.ParticlesFG.Visible = false;
    }

    public override void Removed(Scene scene) {
        base.Removed(scene);

        if (Scene is not Level level) return;

        level.Particles.Visible = true;
        level.ParticlesBG.Visible = true;
        level.ParticlesFG.Visible = true;
    }

    public override void Update() {
        base.Update();

        if (Scene is not Level level) return;

        foreach (var ring in level.Tracker.GetEntities<SpeedRing>()) {
            ring.Visible = false;
            ring.RemoveSelf();
        }
        foreach (var snapshot in level.Tracker.GetEntities<TrailManager.Snapshot>()) {
            snapshot.Visible = false;
            snapshot.RemoveSelf();
        }
        level.Displacement.points.Clear();
    }

    #endregion
    #region --- Graphics ---

    private void BeforeRender() {
        if (Scene is not Level level || !Enabled || !Visible) return;
        if (level.Tracker.GetEntity<Player>() is not Player player) return;

        RefractionFill ??= VirtualContent.CreateRenderTarget("microlith57misc_black_hole_refraction", (int)FillSize.X, (int)FillSize.Y);

        Engine.Graphics.GraphicsDevice.SetRenderTarget(RefractionFill);
        Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);

        var pos = level.Camera.Position + new Vector2(320/2, 180/2);

        var mat = Matrix.Identity
            * Matrix.CreateTranslation(new((RefractionFill.Target.Width / 2) - pos.X,
                                           (RefractionFill.Target.Height / 2) - pos.Y,
                                           0f));

        Draw.SpriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointWrap,
            DepthStencilState.None,
            RasterizerState.CullNone,
            null,
            mat
        );

        foreach (var slash in level.Tracker.GetEntities<SlashFx>())
            slash.Render();

        player.Render();

        Draw.SpriteBatch.End();
    }

    public override void DebugRender(Camera camera)
    {
        base.DebugRender(camera);

        Draw.HollowRect(camera.Position.X, camera.Position.Y, 320, 180, Color.Red);
    }

    public override void Render() {
        if (Scene is not Level level || !Enabled || !Visible) return;

        // Vector2 extent = new Vector2(320f, 180f);
        // Vector2 vector2 = extent / level.ZoomTarget;
        // Vector2 origin = ((level.ZoomTarget != 1f) ? ((level.ZoomFocusPoint - vector2 / 2f) / (extent - vector2) * extent) : Vector2.Zero);

        // float scale = level.Zoom * (320f - level.ScreenPadding * 2f) / 320f;
        // Vector2 topleft = new(level.ScreenPadding, level.ScreenPadding * 9f / 16f);

        // Draw.SpriteBatch.Draw((RenderTarget2D)GameplayBuffers.Level, origin + topleft, GameplayBuffers.Level.Bounds, Color.White, 0f, origin, scale, SaveData.Instance.Assists.MirrorMode ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

        Draw.HollowRect(0, 0, 1920, 1080, Color.Orange);

        SubHudRenderer.EndRender();

        float scale = (320f - level.ScreenPadding * 2f) / 320f;

        #if false
        var mat = (SubHudRenderer.DrawToBuffer ? Matrix.Identity : Engine.ScreenMatrix)
//            * Matrix.CreateScale(scale)
            * Matrix.CreateTranslation(320f/2f, 180f/2f, 0f)
            * Matrix.CreateScale(6f)
            ;

        Draw.SpriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.Default,
            RasterizerState.CullNone,
            null,
            mat
        );

        Draw.HollowRect(-320/2, -180/2, 320, 180, Color.Green);
        #endif

        #if true
        var size = new Vector2(320f, 180f);
        var padding = new Vector2(level.ScreenPadding, level.ScreenPadding * (9f / 16f));

        var mat = (SubHudRenderer.DrawToBuffer ? Matrix.Identity : Engine.ScreenMatrix)
            * Matrix.CreateTranslation(-320/2, -180/2, 0)
            * Matrix.CreateScale((size.X - 2 * level.ScreenPadding) / size.X)
            * Matrix.CreateTranslation(320/2, 180/2, 0)
            * Matrix.CreateScale(6f);

        Draw.SpriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.Default,
            RasterizerState.CullNone,
            null,
            mat
        );

        var offset = size / 2;
        var position = offset + (size - FillSize) / 2;

        // Draw.Point(position, Color.Magenta);

        for (int i=-2; i<=2; i++)
            for (int j=-1; j<=1; j++) {
                if (i == 0 && j == 0) continue;

                var pos = position + new Vector2(i, j) * Spacing;
                // Draw.HollowRect(pos.X - offset.X, pos.Y - offset.Y, FillSize.X, FillSize.Y, SubHudRenderer.DrawToBuffer ? Color.Green : Color.Red);

                Draw.SpriteBatch.Draw(RefractionFill!, pos - offset, Tint);
                // Draw.Point(pos, Color.DarkMagenta);
            }

        // Draw.HollowRect(
        //     0, 0,
        //     320, 180,
        //     Color.Magenta
        // );
        #endif

        Draw.SpriteBatch.End();
        SubHudRenderer.BeginRender();
    }

    #endregion
}
