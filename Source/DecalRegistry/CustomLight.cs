using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

using Celeste.Mod.Microlith57Misc.Components;
using System.Xml;
using System.Linq;
using Celeste.Mod.Registry.DecalRegistryHandlers;

namespace Celeste.Mod.Microlith57Misc.DecalRegistryExt;

public class CustomLight : LightRenderHook {

    public Vector2 Offset;
    public Color Color;
    private List<MTexture> Textures;
    private float frame = 0f;

    private Decal Decal => (Decal) Entity;

    public CustomLight(
        Vector2 offset,
        Color color,
        List<MTexture> textures
    ) {
        Offset = offset;
        Color = color;
        Textures = textures;

        Active = true;
        OnRenderLight = RenderLight;
    }

    public override void Update() {
        if (Textures.Count > 0)
            frame = (frame + Decal.AnimationSpeed * Engine.DeltaTime) % Textures.Count;
    }

    public void RenderLight() {
        if (Textures.Count > 0)
            Textures[(int) frame].DrawCentered(Decal.Position + Offset, Color, Decal.scale, Decal.Rotation);
    }

    internal sealed class Handler : DecalRegistryHandler {

        private float _offX, _offY;
        private Color? _color;
        private float _alpha;
        private string? _path;
        private int[]? _frames;
        private bool _replace;

        public override string Name => "microlith57misc_light";

        public override void Parse(XmlAttributeCollection xml) {
            _offX = Get(xml, "offsetX", 0f);
            _offY = Get(xml, "offsetY", 0f);

            if (xml["color"] is not null)
                _color = GetHexColor(xml, "color", Color.White);

            _alpha =  Get(xml, "alpha", 1f);

            _path = Get(xml, "path", "");

            if (xml["frames"] is not null)
                _frames = GetCSVIntWithTricks(xml, "frames", "0");

            _replace = Get(xml, "replace", false);
        }

        public override void ApplyTo(Decal decal) {
            var offset = decal.GetScaledOffset(_offX, _offY);

            var color = (_color ?? decal.Color) * _alpha;

            var textures = decal.textures;

            if (!string.IsNullOrEmpty(_path))
                textures = GFX.Game.GetAtlasSubtextures(_path);

            if (_frames is not null)
                textures = _frames.Select(i => textures[i]).ToList();

            CustomLight light = new(offset, color, textures);
            decal.Add(light);

            if (_replace) decal.Color = Color.Transparent;
        }

        internal static void Load()
            => DecalRegistry.AddPropertyHandler<Handler>();

    }
}
