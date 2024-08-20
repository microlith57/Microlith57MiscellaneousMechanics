using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57.IntContest;

[CustomEntity("Microlith57_IntContest24/RainbowLight")]
public class RainbowLight : Entity {

    public VertexLight Light;

    public RainbowLight(EntityData data, Vector2 offset)
        : this(data.Position + offset,
               data.Float("alpha", 1f),
               data.Int("startFade", 16),
               data.Int("endFade", 32)) { }

    public RainbowLight(Vector2 position, float alpha, int startFade, int endFade) : base(position) {
        Add(Light = new(Color.Transparent, alpha, startFade, endFade));
    }

    public override void Update() {
        base.Update();

        Light.Color = GetHue(Center);
    }

    private static CrystalStaticSpinner dummy = new(Vector2.Zero, false, CrystalColor.Rainbow);
    public Color GetHue(Vector2 pos) {
        dummy.Scene = Scene;
        return dummy.GetHue(pos);
    }

}
