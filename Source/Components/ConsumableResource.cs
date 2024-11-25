using Monocle;
using System;

namespace Celeste.Mod.Microlith57Misc.Components;

public abstract class ConsumableResource() : Component(active: true, visible: false) {

    public abstract float Low { get; }
    public abstract float Current { get; }
    public abstract float Maximum { get; }

    public class StaminaResource() : ConsumableResource() {

        private Player? Player => (Scene as Level)?.Tracker?.GetEntity<Player>();
        private float _Current, _Maximum;

        public override float Low => 20f;
        public override float Current => _Current;
        public override float Maximum => _Maximum;

        public override void EntityAwake() {
            base.EntityAwake();

            if (Player is not Player player)
                throw new Exception("player somehow dead when creating stamina resource???");

            _Current = player.Stamina;
            player.RefillStamina();
            _Maximum = player.Stamina;
            player.Stamina = _Current;
        }

        public override void Update() {
            base.Update();

            if (Player is Player player)
                _Current = player.Stamina;
        }

    }

}
