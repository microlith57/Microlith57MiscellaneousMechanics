using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

using Celeste.Mod.Microlith57Misc.Components;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity(
    "Microlith57Misc/SliderAmbienceVolumeController=Create",
    "Microlith57Misc/SliderAmbienceVolumeController_Expression=CreateExpr"
)]
public sealed class SliderAmbienceVolumeController : SliderController {

    #region --- Init ---

    public SliderAmbienceVolumeController(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        FloatSource valueSource
    ) : base(data, offset, enabledCondition, valueSource) {
        this.SetDepthAndTags(data);
    }

    public static SliderAmbienceVolumeController Create(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Flag(data) { Default = true },
            new FloatSource.Slider(level.Session, data, name: "volume")
        );

    public static SliderAmbienceVolumeController CreateExpr(Level _, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Expr(data) { Default = true },
            new FloatSource.Expr(data, name: "volume")
        );

    #endregion Init
    #region --- Behaviour ---

    public override void Update() {
        base.Update();

        if (Scene is not Level level || !Enabled) return;

        float val = Value;
        level.Session.Audio.AmbienceVolume = val;
        Audio.CurrentAmbienceEventInstance?.setVolume(val);
    }

    #endregion Behaviour

}
