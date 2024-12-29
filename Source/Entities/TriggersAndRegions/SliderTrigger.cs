using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

using Celeste.Mod.Microlith57Misc.Components;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity(
    "Microlith57Misc/SliderTrigger=Create",
    "Microlith57Misc/SliderTrigger_Expression=CreateExpr"
)]
public sealed class SliderTrigger : Trigger {

    #region --- State ---

    private readonly ConditionSource EnabledCondition;
    public bool Enabled => EnabledCondition.Value;

    private Session.Slider Slider;

    private float From, To;
    private PositionModes Mode;

    #endregion State
    #region --- Init ---

    public SliderTrigger(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        Session.Slider slider,
        float from, float to,
        PositionModes mode
    ) : base(data, offset) {

        Add(EnabledCondition = enabledCondition);
        Slider = slider;
        From = from;
        To = to;
        Mode = mode;
    }


    public static SliderTrigger Create(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Flag(data) { Default = true },
            level.Session.GetSliderObject(data.Attr("slider", defaultValue: "slider")),
            data.Float("from", 0f),
            data.Float("to", 1f),
            data.Enum("direction", PositionModes.LeftToRight)
        );

    public static SliderTrigger CreateExpr(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Expr(data) { Default = true },
            level.Session.GetSliderObject(data.Attr("slider", defaultValue: "slider")),
            data.Float("from", 0f),
            data.Float("to", 1f),
            data.Enum("direction", PositionModes.LeftToRight)
        );

    #endregion Init
    #region --- Behaviour ---

    public override void OnStay(Player player) {
        base.OnStay(player);

        if (Enabled)
            Slider.Value = Calc.ClampedMap(GetPositionLerp(player, Mode), 0f, 1f, From, To);
    }

    #endregion Behaviour

}