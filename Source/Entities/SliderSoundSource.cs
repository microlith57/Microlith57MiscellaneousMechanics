using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;

using Celeste.Mod.Microlith57Misc.Components;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity(
    "Microlith57Misc/SliderSoundSource=Create",
    "Microlith57Misc/SliderSoundSource_Expression=CreateExpr"
)]
public sealed class SliderSoundSource : Entity {

    // todo: global room support

    #region --- State ---

    private string Event;
    private SoundSource Source;

    private ConditionSource EnabledSource;
    private bool Enable => EnabledSource.Value;
    private bool isEnabled => Source.instance != null;

    private ConditionSource PlayingSource;
    private bool Play => PlayingSource.Value;
    private bool isPlaying => Source.Playing;

    private Vector2Source PositionSource;
    private Vector2 SoundPosition => PositionSource.Value;

    private List<(string param, FloatSource valueSource)> ParamSources;
    private IEnumerable<(string param, float value)> Params
        => ParamSources.Select(p => (p.param, p.valueSource.Value));

    private FloatSource VolumeSource;
    private float Volume => VolumeSource.Value;

    #endregion State
    #region --- Init ---

    public SliderSoundSource(
        EntityData data, Vector2 offset,
        ConditionSource enabledSource,
        ConditionSource playingSource,
        Vector2Source positionSource,
        IEnumerable<(string param, FloatSource valueSource)> paramSources,
        FloatSource volumeSource
    ) : base(Vector2.Zero) {

        Tag = Tags.TransitionUpdate;
        Depth = -8500;

        bool positionRelative = data.Bool("positionRelative", true);
        if (positionRelative)
            Position = data.Position + offset;
        else
            positionSource.Default = data.Position + offset;

        Add(EnabledSource = enabledSource);
        Add(PlayingSource = playingSource);
        this.Add(PositionSource = positionSource);

        foreach (var (_, valueSource) in ParamSources = paramSources.ToList())
            Add(valueSource);

        Add(VolumeSource = volumeSource);

        Add(Source = new SoundSource() { Position = PositionSource.Default });
        Event = SFX.EventnameByHandle(data.Attr("sound"));
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
            .Select(s => s.Split(':', 2, StringSplitOptions.TrimEntries))
            .Select(a => (a[0], unpacker(a[1])));

    #endregion Init
    #region --- Behaviour ---

    public override void Awake(Scene scene) {
        base.Awake(scene);
        Apply();
    }

    public override void Update() {
        Apply();
        base.Update();
    }

    private void Apply() {
        var shouldEnable = Enable;
        if (isEnabled && !shouldEnable) {
            Source.Stop(); return;
        } else if (!isEnabled && shouldEnable)
            Source.Play(Event);

        var shouldPlay = Play;
        if (isPlaying && !shouldPlay)
            Source.Pause();
        else if (!isPlaying && shouldPlay)
            Source.Resume();

        Source.Position = SoundPosition;

        foreach (var (param, value) in Params)
            Source.Param(param, value);

        Source.instance.setVolume(Volume);
    }

    #endregion Behaviour

}