using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Entities;

public partial class Box {
    [Pooled]
    public class ShardDebris : Actor {
        public static ParticleType P_Dust => CrystalDebris.P_Dust;

        private Image Sprite;
        private float Percent;
        private float Duration;
        private Vector2 Speed;

        private Collision collideH;
        private Collision collideV;

        public ShardDebris()
            : base(Vector2.Zero) {
            Depth = -9990;
            Collider = new Hitbox(2f, 2f, -1f, -1f);
            Collidable = false;
            collideH = OnCollideH;
            collideV = OnCollideV;
            Sprite = new Image(GFX.Game["particles/shard"]);
            Sprite.CenterOrigin();
            Add(Sprite);
        }

        public void Init(Vector2 position, Color color) {
            Position = position;
            Sprite.Color = color;
            Sprite.Scale = Vector2.One;
            Percent = 0f;
            Duration = Calc.Random.Range(0.25f, 1f);
            Speed = Calc.AngleToVector(Calc.Random.NextAngle(), Calc.Random.Range(200, 240));
        }

        public override void Update() {
            base.Update();

            if (Percent > 1f) {
                RemoveSelf();
                return;
            }
            Percent += Engine.DeltaTime / Duration;

            Speed.X = Calc.Approach(Speed.X, 0f, Engine.DeltaTime * 20f);
            Speed.Y += 200f * Engine.DeltaTime;
            if (Speed.Length() > 0f)
                Sprite.Rotation = Speed.Angle();

            Sprite.Scale = Vector2.One * Calc.ClampedMap(Percent, 0.8f, 1f, 1f, 0f);
            Sprite.Scale.X *= Calc.ClampedMap(Speed.Length(), 0f, 400f, 1f, 2f);
            Sprite.Scale.Y *= Calc.ClampedMap(Speed.Length(), 0f, 400f, 1f, 0.2f);

            MoveH(Speed.X * Engine.DeltaTime, collideH);
            MoveV(Speed.Y * Engine.DeltaTime, collideV);

            if (Scene.OnInterval(0.05f))
                (Scene as Level)!.ParticlesFG.Emit(P_Dust, Position);
        }

        public override void Render() {
            Color color = Sprite.Color;
            Sprite.Color = Color.Black;
            Sprite.Position = new Vector2(-1f, 0f);
            Sprite.Render();
            Sprite.Position = new Vector2(0f, -1f);
            Sprite.Render();
            Sprite.Position = new Vector2(1f, 0f);
            Sprite.Render();
            Sprite.Position = new Vector2(0f, 1f);
            Sprite.Render();
            Sprite.Position = Vector2.Zero;
            Sprite.Color = color;
            base.Render();
        }

        private void OnCollideH(CollisionData hit) => Speed.X *= -0.8f;
        private void OnCollideV(CollisionData hit) {
            if (Math.Sign(Speed.X) != 0)
                Speed.X += Math.Sign(Speed.X) * 5;
            else
                Speed.X += Calc.Random.Choose(-1, 1) * 5;

            Speed.Y *= -1.2f;
        }

        public static void Burst(Vector2 position, Color color, int count = 1) {
            for (int i = 0; i < count; i++) {
                var debris = Engine.Pooler.Create<ShardDebris>();
                debris.Init(position + new Vector2(Calc.Random.Range(-4, 4), Calc.Random.Range(-4, 4)), color);
                Engine.Scene.Add(debris);
            }
        }
    }
}
