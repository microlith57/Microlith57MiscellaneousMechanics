using System;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc;

[CustomEntity("Microlith57Misc/RainbowLight")]
public sealed class RainbowLight : Entity {

    #region --- State, Init, Behaviour ---

    public readonly VertexLight Light;
    public readonly float offset = Calc.Random.NextFloat();

    public RainbowLight(Vector2 position, float alpha, int startFade, int endFade)
        : base(position)
        => Add(Light = new(Color.Transparent, alpha, startFade, endFade));

    public RainbowLight(EntityData data, Vector2 offset)
        : this(data.Position + offset,
               data.Float("alpha", 1f),
               data.Int("startFade", 16),
               data.Int("endFade", 32)) { }

    public override void Awake(Scene scene) {
        base.Awake(scene);

        Light.Color = GetHue(Center);
    }

    public override void Update() {
        base.Update();

        if (Scene.OnInterval(0.08f, offset))
            Light.Color = GetHue(Center);
    }

    #endregion State, Init, Behaviour
    #region --- Utils ---

    private static WeakReference<CrystalStaticSpinner>? weakDummy;
    private CrystalStaticSpinner? dummy;

    private CrystalStaticSpinner Dummy {
        get {
            if (dummy == null &&
                (weakDummy == null || !weakDummy.TryGetTarget(out dummy))) {

                dummy = new(Vector2.Zero, false, CrystalColor.Rainbow);
                weakDummy = new(dummy, false);

            }

            dummy.Scene = Scene;
            return dummy;
        }
    }

    public Color GetHue(Vector2 pos) => Dummy.GetHue(pos);

    #endregion Utils

}
