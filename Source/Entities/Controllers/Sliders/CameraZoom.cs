using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

using Celeste.Mod.Microlith57Misc.Components;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity(
    "Microlith57Misc/SliderCameraZoomController=Create",
    "Microlith57Misc/SliderCameraZoomController_Expression=CreateExpr"
)]
public sealed class SliderCameraZoomController : SliderController {

    #region --- State ---

    private readonly Vector2Source FocusSource;
    public Vector2 Focus => FocusSource.Value;

    private bool wasEnabled = false;

    #endregion State
    #region --- Init ---

    public SliderCameraZoomController(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        Vector2Source focusSource,
        FloatSource valueSource
    ) : base(data, offset, enabledCondition, valueSource) {

        this.Add(FocusSource = focusSource);
    }

    public static SliderCameraZoomController Create(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Flag(data) { Default = true },
            new(
                new FloatSource.Slider(level.Session, data, "focusXSlider", ifAbsent: "160"),
                new FloatSource.Slider(level.Session, data, "focusYSlider", ifAbsent: "90")
            ),
            new FloatSource.Slider(level.Session, data, name: "amount", ifAbsent: "1.0")
        );

    public static SliderCameraZoomController CreateExpr(Level _, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Expr(data) { Default = true },
            new(
                new FloatSource.Expr(data, "focusXExpression", ifAbsent: "160"),
                new FloatSource.Expr(data, "focusYExpression", ifAbsent: "90")
            ),
            new FloatSource.Expr(data, name: "amount", ifAbsent: "1.0")
        );

    #endregion Init
    #region --- Behaviour ---

    public override void Update() {
        base.Update();

        if (Scene is not Level level) return;

        if (Enabled)
            level.ZoomSnap(Focus, Value);
        else if (wasEnabled)
            level.ZoomSnap(new(320 / 2, 180 / 2), 1f);

        wasEnabled = Enabled;
    }

    #endregion Behaviour

}