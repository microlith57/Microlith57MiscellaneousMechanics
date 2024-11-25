using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;

using Celeste.Mod.Microlith57Misc.Components;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity(
    "Microlith57Misc/SliderTimeRateController=CreateFlag",
    "Microlith57Misc/SliderTimeRateController_Expression=CreateExpr"
)]
[Tracked]
public sealed class SliderTimeRateController : Entity {

    #region --- State ---

    private readonly ConditionSource EnabledCondition;
    public bool Enabled => EnabledCondition.Value;

    private readonly FloatSource MultiplierSource;
    public float Multiplier => MultiplierSource.Value;

    private TimeRateModifier Modifier;

    #endregion State
    #region --- Init ---

    public SliderTimeRateController(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        FloatSource multiplierSource
    ) : base(data.Position + offset) {

        Add(EnabledCondition = enabledCondition);
        Add(MultiplierSource = multiplierSource);
        Add(Modifier = new(multiplier: 1f, enabled: false));
    }

    public static SliderTimeRateController CreateFlag(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.FlagSource(data) { Default = true },
            new FloatSource.SliderSource(level.Session, data) { Default = 1f }
        );

    public static SliderTimeRateController CreateExpr(Level _, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.ExpressionSource(data) { Default = true },
            new FloatSource.ExpressionSource(data) { Default = 1f }
        );

    #endregion Init
    #region --- Behaviour ---

    public override void Update() {
        base.Update();

        Modifier.Enabled = Enabled;
        Modifier.Multiplier = Multiplier;
    }

    #endregion Behaviour

}