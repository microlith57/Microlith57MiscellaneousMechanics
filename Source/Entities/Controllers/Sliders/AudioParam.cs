using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

using Celeste.Mod.Microlith57Misc.Components;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity(
    "Microlith57Misc/SliderAudioParamController=Create",
    "Microlith57Misc/SliderAudioParamController_Expression=CreateExpr"
)]
public sealed class SliderAudioParamController : SliderController {

    #region --- State ---

    public readonly bool IsAmbience;
    public readonly string Param;

    #endregion State
    #region --- Init ---

    public SliderAudioParamController(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        FloatSource valueSource
    ) : base(data, offset, enabledCondition, valueSource) {

        Param = data.Attr("param");
        IsAmbience = data.Bool("isAmbience");
    }

    public static SliderAudioParamController Create(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Flag(data) { Default = true },
            new FloatSource.Slider(level.Session, data, name: "value")
        );

    public static SliderAudioParamController CreateExpr(Level _, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Expr(data) { Default = true },
            new FloatSource.Expr(data, name: "value")
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