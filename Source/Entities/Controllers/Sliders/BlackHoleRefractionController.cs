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
public sealed class BlackHoleRefractionController : SliderController {
    #region --- Init ---

    private static VirtualRenderTarget? RefractionFill;

    public BlackHoleRefractionController(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        FloatSource valueSource
    ) : base(data, offset, enabledCondition, valueSource) {
        Tag |= TagsExt.SubHUD;

        Add(new BeforeRenderHook(BeforeRender));
    }

    public static BlackHoleRefractionController Create(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Flag(data, name: "enabled", invertName: "invertEnabled") { Default = true },
            new FloatSource.Slider(level.Session, data, name: "amount")
        );

    #endregion
    #region --- Graphics ---

    private void BeforeRender() {
        if (Scene is not Level level || !Enabled || !Visible) return;

        RefractionFill ??= VirtualContent.CreateRenderTarget("microlith57misc_black_hole_refraction", 180, 180);

        Engine.Graphics.GraphicsDevice.SetRenderTarget(RefractionFill);
        Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);

        var pos = Position;

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

        Draw.Circle(pos, 8f, Color.Aqua, 32);

        Draw.SpriteBatch.End();
    }

    public override void Render() {
        if (Scene is not Level level || !Enabled || !Visible) return;

        // Vector2 extent = new Vector2(320f, 180f);
        // Vector2 vector2 = extent / level.ZoomTarget;
        // Vector2 origin = ((level.ZoomTarget != 1f) ? ((level.ZoomFocusPoint - vector2 / 2f) / (extent - vector2) * extent) : Vector2.Zero);

        // float scale = level.Zoom * (320f - level.ScreenPadding * 2f) / 320f;
        // Vector2 topleft = new(level.ScreenPadding, level.ScreenPadding * 9f / 16f);

        // Draw.SpriteBatch.Draw((RenderTarget2D)GameplayBuffers.Level, origin + topleft, GameplayBuffers.Level.Bounds, Color.White, 0f, origin, scale, SaveData.Instance.Assists.MirrorMode ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

        SubHudRenderer.EndRender();

        var size = new Vector2(320f, 180f);
        var padding = new Vector2(level.ScreenPadding, level.ScreenPadding * (9f / 16f));

        // var topleft = padding;
        //var size = fullsize - 2 * padding;

        var mat = Matrix.Identity
//            * Engine.ScreenMatrix
            * Matrix.CreateTranslation(-320/2, -180/2, 0)
            * Matrix.CreateScale((size.X - 2 * level.ScreenPadding) / size.X)
            * Matrix.CreateTranslation(320/2, 180/2, 0)
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

        for (int i=-2; i<=2; i++)
            for (int j=-1; j<=1; j++) {
                if (i == 0 && j == 0) continue;

                var s = new Vector2(180, 180);
                var o = size / 2;
                var pos = o + s * new Vector2(i, j) + (size-s) / 2;
                // Draw.HollowRect(pos.X - o.X, pos.Y - o.Y, s.X, s.Y, Color.Green);

                Draw.SpriteBatch.Draw(RefractionFill!, pos - o, Color.White);
            }

        // Draw.HollowRect(
        //     0, 0,
        //     320, 180,
        //     Color.Magenta
        // );

        Draw.SpriteBatch.End();
        SubHudRenderer.BeginRender();
    }

    #endregion
}
