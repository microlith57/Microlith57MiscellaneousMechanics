using Microsoft.Xna.Framework;
using Monocle;

using Celeste.Mod.Microlith57Misc.Components;

namespace Celeste.Mod.Microlith57Misc.Entities;

public abstract class SliderController : Entity {

    #region --- State ---

    private readonly ConditionSource EnabledCondition;
    public bool Enabled => EnabledCondition.Value;

    private readonly FloatSource ValueSource;
    public float Value => ValueSource.Value;

    #endregion State
    #region --- Init ---

    public SliderController(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        FloatSource valueSource
    ) : base(data.Position + offset) {

        Add(EnabledCondition = enabledCondition);
        Add(ValueSource = valueSource);
    }

    #endregion Init

}