using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc;

[CustomEntity("Microlith57Misc/RainbowLight")]
public sealed class BreakableLamp : Entity {

    #region --- State, Init, Behaviour ---

    public readonly VertexLight Light;

    public BreakableLamp(Vector2 position, float alpha, int startFade, int endFade)
        : base(position)
        => Add(Light = new(Color.White, alpha, startFade, endFade));

    public BreakableLamp(EntityData data, Vector2 offset)
        : this(data.Position + offset,
               data.Float("alpha", 1f),
               data.Int("startFade", 16),
               data.Int("endFade", 32)) { }

    public override void Awake(Scene scene) {
        base.Awake(scene);
    }

    public override void Update() {
        base.Update();
    }

    #endregion State, Init, Behaviour
    
}
