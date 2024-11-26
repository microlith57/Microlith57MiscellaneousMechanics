using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;

using Celeste.Mod.Microlith57Misc.Components;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity(
    "Microlith57Misc/SliderAmbienceVolumeController=CreateFlag",
    "Microlith57Misc/SliderAmbienceVolumeController_Expression=CreateExpr"
)]
[Tracked]
public sealed class SliderAmbienceVolumeController : Entity {

    #region --- State ---

    private readonly ConditionSource EnabledCondition;
    public bool Enabled => EnabledCondition.Value;

    private readonly FloatSource ValueSource;
    public float Value => ValueSource.Value;

    #endregion State
    #region --- Init ---

    public SliderAmbienceVolumeController(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        FloatSource valueSource
    ) : base(data.Position + offset) {

        Add(EnabledCondition = enabledCondition);
        Add(ValueSource = valueSource);
    }

    public static SliderAmbienceVolumeController CreateFlag(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.FlagSource(data) { Default = true },
            new FloatSource.SliderSource(level.Session, data)
        );

    public static SliderAmbienceVolumeController CreateExpr(Level _, LevelData __, Vector2 offset, EntityData data)
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

        float val = Value;
        level.Session.Audio.AmbienceVolume = val;
        Audio.CurrentAmbienceEventInstance?.setVolume(val);
    }

    #endregion Behaviour

}