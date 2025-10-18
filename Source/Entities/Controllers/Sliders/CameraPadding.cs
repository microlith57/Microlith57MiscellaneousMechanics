using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

using Celeste.Mod.Microlith57Misc.Components;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity(
    "Microlith57Misc/SliderCameraPaddingController=Create",
    "Microlith57Misc/SliderCameraPaddingController_Expression=CreateExpr"
)]
public sealed class SliderCameraPaddingController : SliderController {

    #region --- State ---

    private bool wasEnabled = false;

    #endregion State
    #region --- Init ---

    public SliderCameraPaddingController(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        FloatSource valueSource
    ) : base(data, offset, enabledCondition, valueSource) {}

    public static SliderCameraPaddingController Create(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Flag(data) { Default = true },
            new FloatSource.Slider(level.Session, data, "amount") { Default = 0f }
        );

    public static SliderCameraPaddingController CreateExpr(Level _, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Expr(data) { Default = true },
            new FloatSource.Expr(data, "amount") { Default = 0f }
        );

    #endregion Init
    #region --- Behaviour ---

    public override void Update() {
        base.Update();

        if (Scene is not Level level) return;

        if (Enabled)
            level.ScreenPadding = Value;
        else if (wasEnabled)
            level.ScreenPadding = 0f;

        wasEnabled = Enabled;
    }

    #endregion Behaviour

}
