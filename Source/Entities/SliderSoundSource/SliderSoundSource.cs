using FMOD.Studio;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity(
    "Microlith57Misc/SliderSoundSource=Create",
    "Microlith57Misc/SliderSoundSource_Expression=CreateExpr"
)]
[Tracked]
public sealed class SliderSoundSource : Entity {

    public enum ListenerMode {
        Origin,
        VanillaCamera,
        TrueCamera,
        Player,
    }

    #region --- State ---

    private string Event;
    private SoundSource? Source;

    private ConditionSource EnabledSource;
    private bool Enable => EnabledSource.Value;
    private bool isEnabled => Source?.instance != null;

    private ConditionSource PlayingSource;
    private bool Play => PlayingSource.Value;
    private bool isPlaying => Source != null && Source.Playing;

    private Vector2Source PositionSource;
    private Vector2 SoundPosition => PositionSource.Value;

    private bool RelativeToSource;
    private ListenerMode Listener;
    private Vector2 LastKnownListenerPos;

    private List<(string param, FloatSource valueSource)> ParamSources;
    private IEnumerable<(string param, float value)> Params
        => ParamSources.Select(p => (p.param, p.valueSource.Value));

    private FloatSource VolumeSource;
    private float Volume => VolumeSource.Value;

    private bool GlobalRoomCompat;

    #endregion State
    #region --- Init ---

    public SliderSoundSource(
        EntityData data, Vector2 offset,
        ConditionSource enabledSource,
        ConditionSource playingSource,
        Vector2Source positionSource,
        IEnumerable<(string param, FloatSource valueSource)> paramSources,
        FloatSource volumeSource
    ) : base(data.Position + offset) {

        Tag |= Tags.TransitionUpdate;
        Depth = -8500;
        this.ProcessCommonFields(data);

        RelativeToSource = data.Bool("positionRelative", true);

        Add(EnabledSource = enabledSource);
        Add(PlayingSource = playingSource);
        this.Add(PositionSource = positionSource);

        foreach (var (_, valueSource) in ParamSources = paramSources.ToList())
            Add(valueSource);

        Add(VolumeSource = volumeSource);

        Event = SFX.EventnameByHandle(data.Attr("sound"));
        GlobalRoomCompat = data.Bool("globalRoomCompat");
    }


    public static SliderSoundSource Create(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Flag(data, "enableFlag", invertName: "invertEnable") { Default = true },
            new ConditionSource.Flag(data, "playingFlag", invertName: "invertPlaying") { Default = true },
            Vector2Source.SliderSource(level.Session, data, "position"),
            UnpackParamAttr(data.Attr("params"), s => new FloatSource.Slider(level.Session.GetSliderObject(s))),
            new FloatSource.Slider(level.Session, data, "volume") { Default = 1f }
        );

    public static SliderSoundSource CreateExpr(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Expr(data, "enableExpression") { Default = true },
            new ConditionSource.Expr(data, "playingExpression") { Default = true },
            Vector2Source.ExprSource(data, "position"),
            UnpackParamAttr(data.Attr("params"), s => new FloatSource.Expr(s)),
            new FloatSource.Expr(data, "volume") { Default = 1f }
        );


    private static IEnumerable<(string, FloatSource)> UnpackParamAttr(string attr, Func<string, FloatSource> unpacker)
        => attr
            .Split(',', StringSplitOptions.TrimEntries)
            .Select(s => s.Split(':', count: 2, StringSplitOptions.TrimEntries))
            .Where(a => a.Length >= 2)
            .Select(a => (a[0], unpacker(a[1])));

    #endregion Init
    #region --- Behaviour ---

    public override void Awake(Scene scene) {
        if (GlobalRoomCompat) {
            var bind = scene.Tracker
                .GetEntities<SliderSoundSource>()
                .Cast<SliderSoundSource>()
                .Where(s => s.GlobalRoomCompat
                         && s.Source != null
                         && s.Event == Event)
                .FirstOrDefault();

            if (bind != null) {
                Source = bind.Source;
                bind.Source = null;
                bind.RemoveSelf();
                goto added_source;
            }
        }

        Add(Source = new SoundSource() { Position = PositionSource.Default });

    added_source:
        Apply();

        base.Awake(scene);
    }

    public override void Update() {
        if (Scene is not Level level) return;

        switch (Listener) {
            case ListenerMode.VanillaCamera:
                LastKnownListenerPos = level.Camera.Position + new Vector2(320f, 180f) / 2f;
                break;
            case ListenerMode.TrueCamera:
                if (level.Camera is Camera camera)
                    LastKnownListenerPos = new Vector2((camera.Left + camera.Right) / 2f, (camera.Top + camera.Bottom) / 2f);
                break;
            case ListenerMode.Player:
                if (level.Tracker.GetEntity<Player>() is Player player)
                    LastKnownListenerPos = player.Position;
                break;
        }

        Apply();
        base.Update();
        SetPosition();
    }

    private void Apply() {
        var shouldEnable = Enable;
        if (isEnabled && !shouldEnable) {
            Source!.Stop(); return;
        } else if (!isEnabled && shouldEnable)
            Source!.Play(Event);

        var shouldPlay = Play;
        if (isPlaying && !shouldPlay)
            Source!.Pause();
        else if (!isPlaying && shouldPlay)
            Source!.Resume();

        Source!.Position = SoundPosition;

        foreach (var (param, value) in Params)
            Source.Param(param, value);

        Source.instance?.setVolume(Volume);
    }

    private void SetPosition() {
        if (Scene is not Level level || level.Camera is not Camera camera || Source?.instance is not EventInstance instance) return;

        if (Source?.instance is not null && Source.Is3D) {
            var pos = SoundPosition - LastKnownListenerPos;
            if (RelativeToSource)
                pos += Position;

            var cam = Audio.currentCamera;
            Audio.currentCamera = null;
            try {
                Audio.Position(instance, pos);
            } finally {
                Audio.currentCamera = cam;
            }
        }
    }

    #endregion Behaviour

}
