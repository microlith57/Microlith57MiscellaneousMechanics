using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Entities;

internal class ElevatorFall : Entity {

    public static readonly float Thickness = 3f;

    public TalkComponent Talker;
    public Solid TopSurface, BottomSurface;

    public ElevatorFall(EntityData data, Vector2 offset) : base(data.Position + offset) {
		Vector2 drawAt = new Vector2(data.Width / 2, 0f);
		if (data.Nodes.Length != 0)
			drawAt = data.Nodes[0] - data.Position;

		Add(Talker = new(new(0, 0, data.Width, data.Height), drawAt, OnTalk));

        Collider = new Hitbox(data.Width, data.Height);

        TopSurface = new(Position - Vector2.UnitY * Thickness, Width, Thickness, safe: false);
        BottomSurface = new(Position + Vector2.UnitY * Height, Width, Thickness, safe: false);
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        scene.Add(TopSurface);
        scene.Add(BottomSurface);
    }

    public override void Update() {
        base.Update();
        TopSurface.MoveTo(Position - Vector2.UnitY * Thickness);
        BottomSurface.MoveTo(Position + Vector2.UnitY * Height);
    }

    public void OnTalk(Player player) {
		Scene.Add(new Cutscene(this, player));
    }

    public class Cutscene(ElevatorFall elevator, Player player) : CutsceneEntity() {

        public override void OnBegin(Level level) {
            Add(new Coroutine(Routine(level)));
        }

        public override void OnEnd(Level level) {
        }

        public IEnumerator Routine(Level level) {
            player.StateMachine.State = 11;
            player.StateMachine.Locked = true;

            // sit down

            // stand up
            // dialogue
            // jump, dash

            // shake
            // breaks, falls

            var pos = elevator.Position;

            for (float t = 0; t < 0.4f; t += Engine.DeltaTime) {
                elevator.Position = pos + Calc.Random.ShakeVector();
                yield return null;
            }

            EndCutscene(level);
        }

    }

}
