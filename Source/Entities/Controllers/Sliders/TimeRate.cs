using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;

using Celeste.Mod.Microlith57Misc.Components;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity(
    "Microlith57Misc/SliderTimeRateController=Create",
    "Microlith57Misc/SliderTimeRateController_Expression=CreateExpr"
)]
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
        this.SetDepthAndTags(data);

        Add(EnabledCondition = enabledCondition);
        Add(MultiplierSource = multiplierSource);
        Add(Modifier = new(multiplier: 1f, enabled: false));
    }

    public static SliderTimeRateController Create(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Flag(data) { Default = true },
            new FloatSource.Slider(level.Session, data, name: "multiplier") { Default = 1f }
        );

    public static SliderTimeRateController CreateExpr(Level _, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Expr(data) { Default = true },
            new FloatSource.Expr(data, name: "multiplier") { Default = 1f }
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
