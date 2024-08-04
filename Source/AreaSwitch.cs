using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;

using FlagSwitchGate = Celeste.Mod.MaxHelpingHand.Entities.FlagSwitchGate;

namespace Celeste.Mod.Microlith57.IntContest;

[CustomEntity("Microlith57_IntContest24/AreaSwitch")]
[Tracked]
public class AreaSwitch : Entity {

    [Tracked]
    public class Activator() : Component(true, false) {

        public Collider? Collider;

        public Vector2 Position => Entity.Center;
        public List<AreaSwitch> Activating = [];

        public override void Update() {
            base.Update();

            foreach (var areaSwitch in Scene.Tracker.GetEntities<AreaSwitch>().Cast<AreaSwitch>())
                if ((Collider ?? Entity.Collider).Collide(areaSwitch))
                    areaSwitch.Activate(this);
                else
                    areaSwitch.Deactivate(this);
        }

        public override void Removed(Entity entity) {
            base.Removed(entity);
            onRemove();
        }

        public override void EntityRemoved(Scene scene) {
            base.EntityRemoved(scene);
            onRemove();
        }

        private void onRemove() {
            foreach (var areaSwitch in Activating)
                areaSwitch.Activators.Remove(this);
            Activating = [];
        }

    }

    // todo this depend on circumference
    public static readonly int NUM_LINES = 56;
    public static readonly float AWARENESS_SPIKE_SCALE = 4f;

    // todo box-only switch
    // todo box-destroying switch

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public static ParticleType P_FireInactive;
    public static ParticleType P_FireActive => TouchSwitch.P_FireWhite;
    public static ParticleType P_FireFinished => TouchSwitch.P_FireWhite;
    public static ParticleType P_Spark;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public string Flag;
    public List<AreaSwitch> Siblings = [];

    public float Radius;
    public float AwarenessRange;

    private SoundSource TouchSfx;

    private static MTexture Border = GFX.Game["objects/touchswitch/container"];
    private Sprite Icon;
    private int[] Frames;

    private Color InactiveColor;
    private Color ActiveColor;
    private Color FinishColor;

    private Color InactiveLineColor;
    private Color ActiveLineColor;
    private Color FinishLineColor;

    private float Ease = 0f;
    private float FinishedEase = 0f;
    private float Spin = 0f;
    private Wiggler Wiggler;
    private Vector2 Pulse = Vector2.One;

    private float Timer = 0f;

    private BloomPoint Bloom;

    public StateMachine StateMachine;
    public static int StInactive;
    public static int StActive;
    public static int StFinished;

    public List<Activator> Activators = [];
    public bool Activated => Activators.Count > 0;

    private bool Finished {
        get => StateMachine.State == StFinished;
        set {
            if (value)
                StateMachine.State = StFinished;
        }
    }

    public AreaSwitch(EntityData data, Vector2 offset) : base(data.Position + offset) {

        Depth = 2000;
        Flag = data.Attr("flag");

        Radius = data.Float("radius", 32f);
        AwarenessRange = data.Float("radius", 32f);

        InactiveColor = Calc.HexToColor(data.Attr("inactiveColor", "5FCDE4"));
        ActiveColor = Calc.HexToColor(data.Attr("activeColor", "FFFFFF"));
        FinishColor = Calc.HexToColor(data.Attr("finishColor", "F141DF"));

        InactiveLineColor = Calc.HexToColor(data.Attr("inactiveLineColor", "5FCDE4"));
        ActiveLineColor = Calc.HexToColor(data.Attr("activeLineColor", "FFFFFF"));
        FinishLineColor = Calc.HexToColor(data.Attr("finishLineColor", "F141DF"));

        if (int.TryParse(data.Attr("animationLength", "6"), out int frameVal))
            Frames = Enumerable.Range(0, frameVal).ToArray();
        else
            Frames = [0, 1, 2, 3, 4, 5];

        Add(Icon = new Sprite(GFX.Game, "objects/touchswitch/icon"));
        Icon.Add("idle", "", 0f, 0);
        Icon.Add("spin", "", 0.1f, new Chooser<string>("spin", 1f), Frames);
        Icon.Play("spin");
        Icon.Color = InactiveColor;
        Icon.CenterOrigin();

        Add(Bloom = new BloomPoint(0f, 16f) { Alpha = 0f });

        Add(Wiggler = Wiggler.Create(0.5f, 4f, v => {
            Pulse = Vector2.One * (1f + v * 0.25f);
        }));

        Add(new VertexLight(Color.White, 0.8f, 16, 32));
        Add(TouchSfx = new SoundSource());

        Collider = new Circle(Radius);

        Add(StateMachine = new());
        StInactive = StateMachine.AddState<AreaSwitch>(
            "Inactive",
            onUpdate: s => s.InactiveUpdate(),
            begin: s => s.OnDeactivated()
        );
        StActive = StateMachine.AddState<AreaSwitch>(
            "Active",
            onUpdate: s => s.ActiveUpdate(),
            begin: s => s.OnActivated()
        );
        StFinished = StateMachine.AddState<AreaSwitch>(
            "Finished",
            onUpdate: s => s.FinishedUpdate(),
            begin: s => s.StateMachine.Locked = true
        );
        StateMachine.State = StInactive;

    }


    public override void Added(Scene scene) {
        if (scene is not Level level) return;
        if (level.Session.GetFlag(Flag)) {
            StateMachine.State = StFinished;
            Icon.Rate = 0.1f;
            Icon.Play("idle");
            Icon.Color = FinishColor;
            Ease = 1f;
            Bloom.Alpha = 1f;
        }

        base.Added(scene);
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);

        if (Scene.Tracker.GetEntity<Player>() is Player player &&
            !player.Components.Any(c => c is Activator))

            player.Add(new Activator());

        foreach (var @switch in Scene.Tracker.GetEntities<AreaSwitch>().Cast<AreaSwitch>())
            if (Flag == @switch.Flag)
                Siblings.Add(@switch);
    }

    public override void Update() {
        base.Update();

        Timer += Engine.DeltaTime * 8f;
        Ease = Calc.Approach(Ease, (Finished || Activated) ? 1f : 0f, Engine.DeltaTime * 2f);
        FinishedEase = Calc.Approach(FinishedEase, Finished ? 1f : 0f, Engine.DeltaTime * 2f);

        Spin += Engine.DeltaTime * 0.05f * Ease;

        Icon.Color = Color.Lerp(InactiveColor, Finished ? FinishColor : ActiveColor, Ease);
        Icon.Color *= 0.5f + ((float)Math.Sin(Timer) + 1f) / 2f * (1f - Ease) * 0.5f + 0.5f * Ease;

        Bloom.Alpha = Ease;
    }

    private int InactiveUpdate() => StInactive;
    private int ActiveUpdate() => StActive;

    private int FinishedUpdate() {
        if (Scene is not Level level) return StFinished;

        if (Icon.Rate > 0.1f) {
            Icon.Rate -= 2f * Engine.DeltaTime;
            if (Icon.Rate <= 0.1f) {
                Icon.Rate = 0.1f;
                Wiggler.Start();
                Icon.Play("idle");
                level.Displacement.AddBurst(Position, 0.6f, 4f, 28f, 0.2f);


                for (int i = 0; i < NUM_LINES; i++) {
                    float jiggle = (float)Math.Sin(Scene.TimeActive * 0.5f) * 0.02f;
                    float angle = (i / (float)NUM_LINES + jiggle + Spin) * Calc.Circle;

                    Vector2 relStart = Calc.AngleToVector(angle, 1f);
                    Vector2 absStart = Position + relStart * Radius;

                    Vector2 offset = relStart * (float)Math.Sin(Scene.TimeActive * 2f + i * 0.6f);
                    if (i % 2 == 0)
                        offset *= -1f;
                    absStart += offset;

                    level.ParticlesBG.Emit(P_Spark, absStart, FinishLineColor);
                }
            }
        } else if (Scene.OnInterval(0.03f)) {
            Vector2 position = Position + new Vector2(0f, 1f) + Calc.AngleToVector(Calc.Random.NextAngle(), 5f);
            level.ParticlesBG.Emit(P_FireFinished, position, FinishColor);
        }

        return StFinished;
    }

    private void OnDeactivated() {
        Icon.Rate = 1f;

        if (Scene is not Level)
            return;

        for (int i = 0; i < 24; i++) {
            float dir = Calc.Random.NextFloat((float)Math.PI * 2f);
            (Scene as Level)!.Particles.Emit(P_FireInactive, Position + Calc.AngleToVector(dir, 6f), InactiveColor, dir);
        }

        TouchSfx.Play("event:/game/04_cliffside/arrowblock_side_release");
    }

    private void OnActivated() {
        Icon.Rate = 4f;

        Wiggler.Start();

        if (Scene is not Level)
            return;

        for (int i = 0; i < 32; i++) {
            float dir = Calc.Random.NextFloat((float)Math.PI * 2f);
            (Scene as Level)!.Particles.Emit(P_FireActive, Position + Calc.AngleToVector(dir, 6f), ActiveColor, dir);
        }

        TouchSfx.Play("event:/game/general/touchswitch_any");
    }

    public void Activate(Activator activator) {
        if (Finished || Activators.Contains(activator)) return;

        Activators.Add(activator);
        activator.Activating.Add(this);
        StateMachine.State = StActive;

        if (Siblings.All(s => s.Activated))
            Finish();
    }

    public void Deactivate(Activator activator) {
        if (Finished) return;

        Activators.Remove(activator);
        activator.Activating.Remove(this);

        if (Activators.Count == 0)
            StateMachine.State = StInactive;
    }

    public void Finish() {
        if (Finished) return;

        if (Scene is not Level level) return;

        level.Session.SetFlag(Flag);

        foreach (var s in Siblings)
            s.Finished = true;

        foreach (var gate in level.Tracker.GetEntities<FlagSwitchGate>().Cast<FlagSwitchGate>())
            if (gate.Flag == Flag)
                gate.Trigger();

        Add(new SoundSource("event:/game/general/touchswitch_last_cutoff"));
        SoundEmitter.Play("event:/game/general/touchswitch_last_oneshot");
    }

    public override void Render() {
        Border.DrawCentered(Position + new Vector2(0f, -1f), Color.Black);
        Border.DrawCentered(Position, Icon.Color, Pulse);
        base.Render();

        if (Scene is not Level level || Icon.CurrentAnimationID != "spin") return;

        var col = Color.Lerp(InactiveLineColor, Finished ? FinishLineColor : ActiveLineColor, Ease);

        var nearby = level.Tracker.GetComponents<Activator>().Cast<Activator>()
            .SelectMany<Activator, Vector2>(act => {
                var vec = act.Position - Position;
                return (vec.Length() > (Radius + AwarenessRange)) ? [] : [vec];
            })
            .ToList();

        for (int i = 0; i < NUM_LINES; i++) {
            float jiggle = (float)Math.Sin(Scene.TimeActive * 0.5f) * 0.02f;
            float angle = (i / (float)NUM_LINES + jiggle + Spin) * Calc.Circle;

            Vector2 relStart = Calc.AngleToVector(angle, 1f);
            Vector2 absStart = Position + relStart * Radius;

            Vector2 offset = relStart * (float)Math.Sin(Scene.TimeActive * 2f + i * 0.6f);
            if (i % 2 == 0)
                offset *= -1f;

            absStart = Calc.Round(absStart + offset);

            float t = Ease;
            if (t < 1f)
                t += (float)Math.Tanh(nearby.Sum((vec) => {
                    var distFactor = Calc.ClampedMap(vec.Length(), Radius, Radius + AwarenessRange, 1f, 0f);
                    var angleDiff = Calc.AbsAngleDiff(vec.Angle(), angle);
                    var angleFactor = Calc.ClampedMap(angleDiff, 0f, Calc.Circle / 6f, 1f, 0f);

                    return distFactor * angleFactor;
                }) * AWARENESS_SPIKE_SCALE);
            t *= 1 - FinishedEase;

            Vector2 absEnd = Calc.Round(absStart - relStart * Calc.ClampedMap(t, 0f, 1f, 1f, 3f));
            Color color = col * Calc.ClampedMap(t, 0f, 1f, 0.6f, 1f);

            if (t < 0.4f)
                Draw.Point(absStart, color);
            else
                Draw.Line(absStart, absEnd, color);
        }
    }

}