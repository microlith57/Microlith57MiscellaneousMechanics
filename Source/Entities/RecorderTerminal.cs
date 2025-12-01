using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

using Celeste.Mod.Microlith57Misc.Entities.Recordings;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity("Microlith57Misc/RecorderTerminal")]
[Tracked]
public sealed class RecorderTerminal : Entity {

    #region --- Util ---

    public sealed class ProgressBar(Vector2 offset, float width) : Component(active: false, visible: true) {

        public float Progress = 0f;
        public Color Color = Color.White;

        public Vector2 Position => Entity.Position + offset;

        public float Width = width;
        public float EffectiveWidth => (float)Math.Floor(Width * Progress);
        public float Remainder => Math.Clamp(Width * Progress - EffectiveWidth, 0f, 1f);

        public override void Render() {
            if (EffectiveWidth > 0f)
                Draw.Line(Position, Position + Vector2.UnitX * (float)Math.Floor(EffectiveWidth), Color);

            if (Remainder > 0f)
                Draw.Point(Position + Vector2.UnitX * EffectiveWidth, Color * Remainder);
        }

    }

    public static readonly float MAX_DURATION = 60f;
    public static readonly float PROMPT_RANGE = 72f;

    public static readonly Color COLOR_IDLE = Color.White * 0.7f;
    public static readonly Color COLOR_RECORDING = Calc.HexToColor("ff8888");
    public static readonly Color COLOR_PLAYBACK = Calc.HexToColor("88ff88");
    public static readonly Color COLOR_COOLDOWN = Calc.HexToColor("ff8800");

    #endregion Util
    #region --- State ---

    public Color BaseColor;
    public ParticleType BaseDust;

    public readonly Sprite ButtonAndStripe;
    public readonly Sprite Screen;
    public readonly ProgressBar Progress;
    public readonly TalkComponent Talker;
    public readonly VertexLight Light;
    public readonly StateMachine StateMachine;

    public readonly List<Recording> Recordings = [];

    public readonly List<float> TimeStamps = [];
    public float Duration => TimeStamps.Count > 0 ? TimeStamps[^1] : 0;
    public int FrameCount => TimeStamps.Count;

    public float Time = 0f;
    public int FrameIndex = 0;

    private bool gracePeriod = false;

    public readonly int StIdle;
    public readonly int StRecording;
    public readonly int StPlayback;
    public readonly int StCooldown;

    #endregion State
    #region --- Init ---

    public RecorderTerminal(EntityData data, Vector2 offset)
        : base(data.Position + offset) {

        Depth = 2000;

        BaseColor = data.HexColor("color", Calc.HexToColor("ac3232"));
        BaseDust = new ParticleType(ParticleTypes.SparkyDust) {
            Color = BaseColor,
            Color2 = Color.White
        };

        Image image = new(GFX.Game["objects/microlith57/misc/terminal"]);
        image.JustifyOrigin(new(0.5f, 1f));
        Add(image);

        Add(ButtonAndStripe = new(GFX.Game, "objects/microlith57/misc/terminalcolor") {
            Color = BaseColor,
            Justify = new(0.5f, 1f),
            OnChange = (from, to) => {
                if (from == "idle" && to == "interact")
                    Audio.Play("event:/game/04_cliffside/arrowblock_side_depress", Position);
                else if (from == "interact" && to == "idle")
                    Audio.Play("event:/game/04_cliffside/arrowblock_side_release", Position);
            }
        });
        ButtonAndStripe.Add("idle", "", 1f, [0]);
        ButtonAndStripe.Add("interact", "", 0.1f, "idle", [1]);
        ButtonAndStripe.Play("idle");

        Add(Screen = new(GFX.Game, "objects/microlith57/misc/screen") {
            Justify = new(0.5f, 1f)
        });
        Screen.AddLoop("idle", "", 0.05f, [0, 1, 2]);
        Screen.Add("recording", "", 1f, [3]);
        Screen.Add("playback", "", 1f, [4]);

        Add(Progress = new(new Vector2(4f, -9f), 10f));

        Add(Talker = new(
            new Rectangle(-20, -8, 40, 16),
            new Vector2(-3.5f, -24f),
            player => Add(new Coroutine(OnInteract(player)))
        ));
        Talker.PlayerMustBeFacing = false;

        Add(Light = new(
            new Vector2(9f, -15f),
            COLOR_IDLE, 1f,
            16, 32
        ));

        Add(StateMachine = new());
        StIdle = StateMachine.AddState<RecorderTerminal>(
            "Idle",
            onUpdate: e => e.IdleUpdate(),
            begin: e => e.IdleBegin()
        );
        StRecording = StateMachine.AddState<RecorderTerminal>(
            "Recording",
            onUpdate: e => e.RecordingUpdate(),
            begin: e => e.RecordingBegin()
        );
        StPlayback = StateMachine.AddState<RecorderTerminal>(
            "Playback",
            onUpdate: e => e.PlaybackUpdate(),
            begin: e => e.PlaybackBegin()
        );
        StCooldown = StateMachine.AddState<RecorderTerminal>(
            "Cooldown",
            onUpdate: e => e.CooldownUpdate(),
            begin: e => e.CooldownBegin(),
            end: e => e.CooldownEnd()
        );
        StateMachine.State = StIdle;
    }

    #endregion
    #region --- Behaviour ---

    public override void Update() {
        base.Update();

        Talker.Enabled = StateMachine.State != StCooldown &&
                         Scene.Tracker.GetEntity<Player>() is Player player &&
                         (player.Position - Position).Length() <= PROMPT_RANGE;

        BaseDust.Color = BaseColor;
    }

    public IEnumerator OnInteract(Player player) {
        player.StateMachine.State = Player.StDummy;
        player.ForceCameraUpdate = true;
        player.StateMachine.Locked = true;
        gracePeriod = true;

        yield return player.DummyRunTo(Position.X - 16f);
        yield return player.DummyWalkToExact((int)(Position.X - 16f));
        player.Facing = Facings.Right;

        yield return 0.1f;
        ButtonAndStripe.Play("interact");
        yield return 0.1f;

        if (StateMachine.State == StIdle)
            StateMachine.State = StRecording;
        else if (StateMachine.State == StRecording)
            StateMachine.State = StPlayback;
        else if (StateMachine.State == StPlayback)
            StateMachine.State = StCooldown;

        player.StateMachine.Locked = false;
        player.ForceCameraUpdate = false;
        player.StateMachine.State = Player.StNormal;
        gracePeriod = false;
    }

    #region Rendering

    public override void DebugRender(Camera camera) {
        base.DebugRender(camera);

        if (StateMachine.State == StRecording)
            foreach (var rec in Recordings)
                if (rec.RecordingOf != null)
                    Draw.Line(Position, rec.RecordingOf.Center, BaseColor);

                else if (StateMachine.State == StPlayback)
                    foreach (var rec2 in Recordings)
                        if (rec2.Visible)
                            Draw.Line(Position, rec2.Center, BaseColor);
    }

    #endregion
    #region > Idle

    private void IdleBegin() {
        Recordings.ForEach(r => r.EndPlayback(remove: true));
        Recordings.Clear();

        Screen.Play("idle");
        Light.Color = COLOR_IDLE;
        Progress.Progress = 0f;
    }
    private int IdleUpdate() => StIdle;

    #endregion
    #region > Recording

    private void RecordingBegin() {
        Recordings.ForEach(r => r.EndPlayback(remove: true));
        Recordings.Clear();

        TimeStamps.Clear();
        FrameIndex = 0;
        Time = 0f;
        TimeStamps.Add(Time);

        Screen.Play("recording");
        Progress.Color = Light.Color = COLOR_RECORDING;
    }
    private int RecordingUpdate() {
        if (Duration >= MAX_DURATION && !gracePeriod)
            return StIdle;

        var entities = new List<Entity>();

        if (Scene.Tracker.GetEntity<Player>() is Player player)
            entities.Add(player);

        foreach (var box in Scene.Tracker.GetEntities<Box>())
            entities.Add(box);

        foreach (var playback in Scene.Tracker.GetEntities<Recording>())
            if (((Recording)playback).IsPlaying)
                entities.Add(playback);

        entities = [.. entities.OrderBy(e => e.actualDepth)];

        var recordings = Recordings.Where(r => r.IsRecording).ToList();

        foreach (var entity in entities) {
            var rec = recordings.Find(rec => rec.RecordingOf == entity);

            if (rec == null) {
                if (entity is Player p)
                    rec = new PlayerRecording(p.Hair.Nodes.Count);
                else if (entity is PlayerRecording pr)
                    rec = new PlayerRecording(pr.Hair.Nodes.Count);
                else if (entity is Box b)
                    rec = new BoxRecording(BaseDust, b.GravityLocked);
                else if (entity is BoxRecording br)
                    rec = new BoxRecording(br.Dust, br.GravityLocked);
                else
                    throw new Exception("should be unreachable");

                Scene.Add(rec);
                Recordings.Add(rec);
                rec.BeginRecording(entity);
            }

            rec.Observe(FrameIndex, BaseColor);

            recordings.Remove(rec); // remove from the temp list since we've dealt with it
        }

        foreach (var remaining in recordings)
            remaining.EndRecording();

        FrameIndex += 1;
        Time += Engine.DeltaTime;
        TimeStamps.Add(Time);

        Progress.Progress = Duration / MAX_DURATION;

        return StRecording;
    }

    #endregion
    #region > Playback

    private void PlaybackBegin() {
        Time = 0f;
        FrameIndex = 0;

        Screen.Play("playback");
        Progress.Color = Light.Color = COLOR_PLAYBACK;
    }
    private int PlaybackUpdate() {
        if (FrameIndex >= FrameCount || Time >= Duration)
            return StCooldown;

        foreach (var recording in Recordings) {
            var inBounds = FrameIndex >= recording.FirstFrame && FrameIndex <= recording.LastFrame;

            if (!recording.IsPlaying && inBounds)
                recording.BeginPlayback();
            else if (recording.IsPlaying && !inBounds)
                recording.EndPlayback(remove: false);

            if (recording.IsPlaying)
                recording.FrameIndex = FrameIndex;
        }

        Time += Engine.DeltaTime;
        while (FrameIndex + 1 < FrameCount && Time >= TimeStamps[FrameIndex + 1])
            FrameIndex++;

        Progress.Progress = Time / Duration;

        return StPlayback;
    }

    #endregion
    #region > Cooldown

    private void CooldownBegin() {
        Recordings.ForEach(r => r.EndPlayback(remove: true));
        Recordings.Clear();

        Screen.Visible = Light.Visible = false;
        Progress.Progress = 1f;
    }
    private int CooldownUpdate() {
        if (Scene.Tracker.GetEntities<RecorderTerminal>()
                         .All(t => ((RecorderTerminal)t).StateMachine.State == StIdle ||
                                   ((RecorderTerminal)t).StateMachine.State == StCooldown))
            return StIdle;

        Progress.Color = Scene.TimeActive % 1f < 0.5f ? COLOR_COOLDOWN : Color.Transparent;

        return StCooldown;
    }
    private void CooldownEnd() {
        Screen.Visible = Light.Visible = true;
    }

    #endregion
    #endregion

}
