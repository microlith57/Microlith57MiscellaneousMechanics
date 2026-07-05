namespace Celeste.Mod.Microlith57Misc.Components;

public abstract partial class ConsumableResource : Entity {
    public sealed class Drain : Component {
        public readonly ConsumableResource Resource;
        public float ConsumptionRate;
        public bool Stacks;

        public float ConsumptionThisFrame => ConsumptionRate * Resource.DeltaTime;

        public Drain(
            ConsumableResource resource,
            float consumptionRate,
            bool rawDeltaTime,
            bool stacks
        ) : base(active: true, visible: false) {
            // todo non-positive drain rates
            if (consumptionRate <= 0f)
                throw new Exception("non-positive drain rates are unimplemented");

            // todo non-stacking drains
            if (!stacks)
                throw new Exception("non-stacking drains are unimplemented");

            Resource = resource;
            ConsumptionRate = consumptionRate;
            Stacks = stacks;
        }

        public override void Added(Entity entity) {
            base.Added(entity);
            if (entity.Scene != null)
                Resource.Drains.Add(this);
        }

        public override void EntityAdded(Scene scene) {
            base.EntityAdded(scene);
            Resource.Drains.Add(this);
        }

        public override void Removed(Entity entity) {
            base.Removed(entity);
            Resource.Drains.Remove(this);
        }

        public override void EntityRemoved(Scene scene) {
            base.EntityRemoved(scene);
            Resource.Drains.Remove(this);
        }
    }
}
