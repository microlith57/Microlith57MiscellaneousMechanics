using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;

using Celeste.Mod.Microlith57Misc.Components;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity(
    "Microlith57Misc/SliderCameraZoomController=CreateFlag",
    "Microlith57Misc/SliderCameraZoomController_Expression=CreateExpr"
)]
[Tracked]
public sealed class SliderCameraZoomController : Entity {

    #region --- State ---

    private readonly ConditionSource EnabledCondition;
    public bool Enabled => EnabledCondition.Value;

    private readonly Vector2Source FocusSource;
    public Vector2 Focus => FocusSource.Value;

    private readonly FloatSource ZoomSource;
    public float Zoom => ZoomSource.Value;

    #endregion State
    #region --- Init ---

    public SliderCameraZoomController(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        Vector2Source focusSource,
        FloatSource zoomSource
    ) : base(data.Position + offset) {

        Add(EnabledCondition = enabledCondition);
        this.Add(FocusSource = focusSource);
        Add(ZoomSource = zoomSource);
    }

    public static SliderCameraZoomController CreateFlag(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.FlagSource(data) { Default = true },
            new(
                new FloatSource.SliderSource(level.Session, data, "focusXSlider") { Default = 320 / 2 },
                new FloatSource.SliderSource(level.Session, data, "focusYSlider") { Default = 180 / 2 }
            ),
            new FloatSource.SliderSource(level.Session, data) { Default = 1f }
        );

    public static SliderCameraZoomController CreateExpr(Level _, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.ExpressionSource(data) { Default = true },
            new(
                new FloatSource.ExpressionSource(data, "focusXExpression") { Default = 320 / 2 },
                new FloatSource.ExpressionSource(data, "focusYExpression") { Default = 180 / 2 }
            ),
            new FloatSource.ExpressionSource(data) { Default = 1f }
        );

    #endregion Init
    #region --- Behaviour ---

    public override void Update() {
        base.Update();

        if (Scene is not Level level) return;

        // todo: adv camera utils interop?
        level.ZoomSnap(Focus, Zoom);
    }

    #endregion Behaviour

}