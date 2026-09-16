namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity("Microlith57Misc/VeryInvisibleBarrier")]
public sealed class VeryInvisibleBarrier : InvisibleBarrier {
    public VeryInvisibleBarrier(EntityData data, Vector2 offset) : base(data, offset) {}
    public override void DebugRender(Camera camera) {}
}
