#if FEATURE_FLAG_RECORDINGS

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Entities.Recordings;

[Tracked]
public class RecordingRenderer : Entity {

    private static BlendState scanlineBlend = new() {
        // res.RGB = (dest.RGB) - (source.RGB * dest.A)
        ColorBlendFunction = BlendFunction.ReverseSubtract,
        ColorSourceBlend = Blend.DestinationAlpha,
        ColorDestinationBlend = Blend.One,

        // res.A = (dest.A) - (source.A * dest.A)
        AlphaBlendFunction = BlendFunction.ReverseSubtract,
        AlphaSourceBlend = Blend.DestinationAlpha,
        AlphaDestinationBlend = Blend.One,
    };

    private Level Level => (Level)Scene;

    private VirtualRenderTarget? buffer;
    private bool bufferFilled = false;

    public float ScanlineOffset;
    public MTexture scanlines;

    public RecordingRenderer() {
        Depth = 1000;

        Add(new BeforeRenderHook(BeforeRender));

        scanlines = GFX.Game["objects/microlith57/misc/scanlines"];
    }

    public override void Update() {
        base.Update();

        ScanlineOffset += 10f * Engine.DeltaTime;
    }

    public void BeforeRender() {
        bufferFilled = false;

        var recordings = Scene.Tracker.GetEntities<Recording>().Where(e => e is Recording r && r.Visible).ToList();
        if (recordings.Count == 0)
            return;

        if (buffer == null || buffer.IsDisposed)
            buffer = new("Microlith57Misc/RecordingRenderer", 320, 180, 0, false, true);


#if FEATURE_FLAG_BOX
        List<BoxRecording> unheld_boxes = [];
        List<BoxRecording> held_boxes = [];

        foreach (BoxRecording box in recordings.Where(e => e is BoxRecording).OrderBy(b => ((BoxRecording)b).LastInteraction).ToList())
            (box.IsHeld ? held_boxes : unheld_boxes).Add(box);
#endif

        // draw unheld boxes to tempA
        Engine.Graphics.GraphicsDevice.SetRenderTarget(GameplayBuffers.TempA);
        Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);

#if FEATURE_FLAG_BOX
        beginBatch();

        foreach (var box in unheld_boxes)
            box.RenderOutline();
        foreach (var box in unheld_boxes)
            box.RenderSprite();

        endBatch();
#endif

        // draw opaque players to tempB
        Engine.Graphics.GraphicsDevice.SetRenderTarget(GameplayBuffers.TempB);
        Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
        beginBatch();

        foreach (PlayerRecording rec in recordings.Where(e => e is PlayerRecording))
            rec.RenderSprite();

        endBatch();

        // back to tempA to draw the players transparently, then the held boxes
        Engine.Graphics.GraphicsDevice.SetRenderTarget(GameplayBuffers.TempA);
        beginBatch();

        Draw.SpriteBatch.Draw((RenderTarget2D)GameplayBuffers.TempB, Level.Camera.Position, Color.White * 0.8f);

#if FEATURE_FLAG_BOX
        foreach (var box in held_boxes)
            box.RenderOutline();
        foreach (var box in held_boxes)
            box.RenderSprite();
#endif

        endBatch();

        // subtract the scanlines
        beginScanlineBatch();

        var offset = (int)Math.Round(ScanlineOffset) % scanlines.Height;
        Rectangle rect = new Rectangle(0, -offset, 320, 180);
        Draw.SpriteBatch.Draw(scanlines.Texture.Texture, Vector2.Zero, rect, Color.White);

        endBatch();

        // finally draw tempA onto our buffer for later
        Engine.Graphics.GraphicsDevice.SetRenderTarget(buffer);
        Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
        beginBatch();
        Draw.SpriteBatch.Draw((RenderTarget2D)GameplayBuffers.TempA, Level.Camera.Position, new Color(1f, 1f, 1f, 0.8f));
        endBatch();

        bufferFilled = true;
    }

    public override void Render() {
        base.Render();

        if (bufferFilled)
            Draw.SpriteBatch.Draw((RenderTarget2D)buffer, Level.Camera.Position, Color.White);
    }

    private void beginBatch() {
        Draw.SpriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.None,
            RasterizerState.CullNone,
            null,
            Matrix.CreateTranslation(new(-Level.Camera.Position, 0f))
        );
    }

    private void beginScanlineBatch() {
        Draw.SpriteBatch.Begin(
            SpriteSortMode.Deferred,
            scanlineBlend,
            SamplerState.PointWrap,
            DepthStencilState.None,
            RasterizerState.CullNone,
            null,
            Matrix.Identity
        );
    }

    private void endBatch() {
        Draw.SpriteBatch.End();
    }

}

#endif