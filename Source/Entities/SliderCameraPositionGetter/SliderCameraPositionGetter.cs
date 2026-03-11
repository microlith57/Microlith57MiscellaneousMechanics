using System.Diagnostics;
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

    public enum TrackingType {
        Position,
        Origin,
        BottomLeft,
        BottomRight,
        TopLeft,
        TopRight,
        Center,
    }

    #region --- State ---

    private TrackingType Tracking;

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
        this.ProcessCommonFields(data);

        SliderX = sliderX;
        SliderY = sliderY;
        EnabledSource = enabledSource;
    }

    public static SliderCameraPositionGetter Create(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            level.Session.GetSliderObject(data.Attr("sliderPrefix", "cameraPosition") + 'X'),
            level.Session.GetSliderObject(data.Attr("sliderPrefix", "cameraPosition") + 'Y'),
            new ConditionSource.Flag(data) { Default = true }
        );

    #endregion
    #region --- Behaviour ---

    public override void Update() {
        base.Update();

        if (Scene is not Level level || !Enabled) return;

        var pos = GetPosition(level.Camera);
        SliderX.Value = pos.X;
        SliderY.Value = pos.Y;
    }

    private Vector2 GetPosition(Camera camera) => Tracking switch {
        TrackingType.Position => camera.Position,
        TrackingType.Origin => camera.Origin,
        TrackingType.BottomLeft => new(camera.Left, camera.Bottom),
        TrackingType.BottomRight => new(camera.Right, camera.Bottom),
        TrackingType.TopLeft => new(camera.Left, camera.Top),
        TrackingType.TopRight => new(camera.Right, camera.Top),
        TrackingType.Center => new((camera.Left + camera.Right) / 2, (camera.Top + camera.Bottom) / 2),
        _ => throw new UnreachableException()
    };

    #endregion

}
