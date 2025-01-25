using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Entities;

public partial class Box {
    private class TutorialPrompt() : Component(true, false) {

        private Box Box => (Box)Entity;
        private BirdTutorialGui? Gui;
        private float Timer;

        private void AddGui() {
            if (Gui != null || Entity?.Scene == null) return;

            Gui = new(
                Entity, new Vector2(0f, -24f),
                Dialog.Clean("tutorial_carry"),
                Dialog.Clean("tutorial_hold"),
                BirdTutorialGui.ButtonPrompt.Grab) {
                Open = false
            };
            Gui.AddTag(Tags.Persistent);
        }

        public override void Update() {
            base.Update();

            if (Gui?.Scene == null) AddGui();

            if (Box.Hold.IsHeld)
                RemoveSelf();

            if (!Box.OnGround())
                Timer = 0.25f;
            else if (Timer > 0)
                Timer -= Engine.DeltaTime;

            Gui!.Open = Timer <= 0;
        }

        public override void Removed(Entity entity) {
            base.Removed(entity);
            Gui?.RemoveSelf();
        }

        public override void EntityRemoved(Scene scene) {
            base.EntityRemoved(scene);
            Gui?.RemoveSelf();
        }

    }
}