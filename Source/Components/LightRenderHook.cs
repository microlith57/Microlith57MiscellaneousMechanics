using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;

namespace Celeste.Mod.Microlith57Misc.Components;

[Tracked]
public class LightRenderHook() : Component(false, false) {

    public Action? OnRenderLight;

    private static void hook_LightingRenderer_BeforeRender(On.Celeste.LightingRenderer.orig_BeforeRender orig, LightingRenderer self, Scene scene) {
        orig(self, scene);

        if (scene is not Level level) return;

        var components = level.Tracker.GetComponents<LightRenderHook>();
        if (components.Count == 0) return;

        Engine.Graphics.GraphicsDevice.SetRenderTarget(GameplayBuffers.Light);
        bool usingSpritebatch = false;

        StartSpritebatch(ref usingSpritebatch, Matrix.CreateTranslation(new(-level.Camera.Position, 0)));
        foreach (var component in components)
            if (component is LightRenderHook light && light.OnRenderLight is not null)
                light.OnRenderLight();
        EndSpritebatch(ref usingSpritebatch);
    }

    private static void StartSpritebatch(ref bool usingSpritebatch)
        => StartSpritebatch(ref usingSpritebatch, Matrix.Identity);

    private static void StartSpritebatch(ref bool usingSpritebatch, Matrix matrix) {
        if (usingSpritebatch)
            return;

        Draw.SpriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.Additive,
            SamplerState.LinearClamp,
            DepthStencilState.None,
            RasterizerState.CullCounterClockwise,
            null, matrix
        );
        usingSpritebatch = true;
    }

    private static void EndSpritebatch(ref bool usingSpritebatch) {
        if (!usingSpritebatch)
            return;

        Draw.SpriteBatch.End();
        usingSpritebatch = false;
    }

    internal static void Load()
        => On.Celeste.LightingRenderer.BeforeRender += hook_LightingRenderer_BeforeRender;
    internal static void Unload()
        => On.Celeste.LightingRenderer.BeforeRender -= hook_LightingRenderer_BeforeRender;

}
