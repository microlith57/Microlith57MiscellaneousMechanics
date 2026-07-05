namespace Celeste.Mod.Microlith57Misc.Entities;

public abstract class Controller: Entity {
    public Controller(EntityData data, Vector2 offset) : base(data.Position + offset) {
        this.ProcessCommonFields(data);
    }

    public override void Update() {
        base.Update();
        OnUpdate();
    }

    public virtual void OnUpdate() {}
}
