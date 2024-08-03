using System;
using System.Collections;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57.IntContest;

[CustomEntity("Microlith57_IntContest24/RecorderTerminal")]
public class RecorderTerminal : Entity {

    public class ProgressBar(Vector2 Offset, float Width) : Component(active: false, visible: true) {
        public float Progress = 0f;
        public Color Color = Color.White;

        public Vector2 Position => Entity.Position + Offset;
        public float EffectiveWidth => (float)Math.Floor(Width * Progress);
        public float Remainder => Math.Clamp((Width * Progress) - EffectiveWidth, 0f, 1f);

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

    public Sprite Sprite;
    public Sprite Screen;
    public ProgressBar Progress;
    public TalkComponent Talker;
    public VertexLight Light;
    public StateMachine StateMachine;

    public PlayerPlayback? PlayerPlayback;

    private bool gracePeriod = false;

    public static int StIdle;
    public static int StRecording;
    public static int StPlayback;

    public RecorderTerminal(EntityData data, Vector2 offset) : base(data.Position + offset) {
        Depth = 2000;

        Add(Sprite = new(GFX.Game, "objects/INTcontest24/microlith57/terminal") {
            Justify = new(0.5f, 1f),
            OnChange = (from, to) => {
                if (from == "idle" && to == "interact")
                    Audio.Play("event:/game/04_cliffside/arrowblock_side_depress", Position);
                else if (from == "interact" && to == "idle")
                    Audio.Play("event:/game/04_cliffside/arrowblock_side_release", Position);
            }
        });
        Sprite.Add("idle", "", 1f, [0]);
        Sprite.Add("interact", "", 0.1f, "idle", [1]);
        Sprite.Play("idle");

        Add(Screen = new(GFX.Game, "objects/INTcontest24/microlith57/screen") {
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
        StateMachine.State = StIdle;
    }

    public override void Update() {
        base.Update();

        Talker.Enabled = (
            Scene.Tracker.GetEntity<Player>() is Player player
            && (player.Position - Position).Length() <= PROMPT_RANGE
        );
    }

    private void IdleBegin() {
        PlayerPlayback?.EndPlayback();
        PlayerPlayback = null;

        Screen.Play("idle");
        Light.Color = COLOR_IDLE;
        Progress.Progress = 0f;
    }
    private int IdleUpdate() => StIdle;


    private void RecordingBegin() {
        PlayerPlayback?.EndPlayback();
        PlayerPlayback = new(Position, []);

        Screen.Play("recording");
        Progress.Color = Light.Color = COLOR_RECORDING;
    }
    private int RecordingUpdate() {
        var player = Scene.Tracker.GetEntity<Player>();

        if (PlayerPlayback == null ||
            (PlayerPlayback.Duration >= MAX_DURATION && !gracePeriod) ||
            player == null)
            return StIdle;

        PlayerPlayback.Observe(player);

        Progress.Progress = PlayerPlayback.Duration / MAX_DURATION;

        return StRecording;
    }


    private void PlaybackBegin() {
        PlayerPlayback ??= new(Position, []);
        Scene.Add(PlayerPlayback);
        PlayerPlayback.BeginPlayback();

        Screen.Play("playback");
        Progress.Color = Light.Color = COLOR_PLAYBACK;
    }
    private int PlaybackUpdate() {
        if (PlayerPlayback == null || !PlayerPlayback.Playing)
            return StIdle;

        Progress.Progress = PlayerPlayback.Time / PlayerPlayback.Duration;

        return StPlayback;
    }


    public IEnumerator OnInteract(Player player) {
        player.StateMachine.State = Player.StDummy;
        player.StateMachine.Locked = true;
        gracePeriod = true;

        yield return player.DummyRunTo(Position.X - 16f);
        yield return player.DummyWalkToExact((int)(Position.X - 16f));
        player.Facing = Facings.Right;

        yield return 0.1f;
        Sprite.Play("interact");
        yield return 0.1f;

        if (StateMachine.State == StIdle)
            StateMachine.State = StRecording;
        else if (StateMachine.State == StRecording)
            StateMachine.State = StPlayback;
        else
            StateMachine.State = StIdle;

        player.StateMachine.Locked = false;
        player.StateMachine.State = Player.StNormal;
        gracePeriod = false;
    }

}