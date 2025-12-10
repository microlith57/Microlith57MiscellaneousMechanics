using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;

using Celeste.Mod.Microlith57Misc.Components;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity(
    "Microlith57Misc/SliderCameraPositionGetter=Create"
)]
[Tracked]
public sealed class SliderCameraPositionGetter : Entity {

    #region --- State ---

    private ConditionSource EnabledSource;
    private bool Enabled => EnabledSource.Value;

    private Session.Slider SliderX, SliderY;

    #endregion
    #region --- Init ---

    public SliderCameraPositionGetter(
        EntityData data, Vector2 offset,
        Session.Slider sliderX, Session.Slider sliderY,
        ConditionSource enabledSource
    ) : base(Vector2.Zero) {
        this.SetDepthAndTags(data);

        SliderX = sliderX;
        SliderY = sliderY;
        EnabledSource = enabledSource;
    }

    #endregion
    #region --- Behaviour ---

    public override void Update() {
        base.Update();

        if (Scene is not Level level || !Enabled) return;

        var pos = level.Camera.Position;
        SliderX.Value = pos.X;
        SliderY.Value = pos.Y;
    }

    #endregion

}
