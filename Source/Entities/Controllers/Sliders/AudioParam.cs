using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;

using Celeste.Mod.Microlith57Misc.Components;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity(
    "Microlith57Misc/SliderAudioParamController=CreateFlag",
    "Microlith57Misc/SliderAudioParamController_Expression=CreateExpr"
)]
[Tracked]
public sealed class SliderAudioParamController : Entity {

    #region --- State ---

    public readonly bool IsAmbience;
    public readonly string Param;

    private readonly ConditionSource EnabledCondition;
    public bool Enabled => EnabledCondition.Value;

    private readonly FloatSource ValueSource;
    public float Value => ValueSource.Value;

    #endregion State
    #region --- Init ---

    public SliderAudioParamController(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        FloatSource valueSource
    ) : base(data.Position + offset) {

        Param = data.Attr("param");
        IsAmbience = data.Bool("isAmbience");

        Add(EnabledCondition = enabledCondition);
        Add(ValueSource = valueSource);
    }

    public static SliderAudioParamController CreateFlag(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.FlagSource(data) { Default = true },
            new FloatSource.SliderSource(level.Session, data)
        );

    public static SliderAudioParamController CreateExpr(Level _, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.ExpressionSource(data) { Default = true },
            new FloatSource.ExpressionSource(data)
        );

    #endregion Init
    #region --- Behaviour ---

    public override void Update() {
        base.Update();

        if (Scene is not Level level) return;

        if (IsAmbience) {
            level.Session.Audio.Ambience.Param(Param, Value);
            level.Session.Audio.Apply();
        } else {
            Audio.SetMusicParam(Param, Value);
        }
    }

    #endregion Behaviour

}