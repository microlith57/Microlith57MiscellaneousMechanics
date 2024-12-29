using Celeste.Mod.Entities;
using Celeste.Mod.Microlith57Misc.Components;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity(
    "Microlith57Misc/SliderCameraTargetTrigger=CreateTarget",
    // "Microlith57Misc/SliderCameraTargetTrigger_Expression=CreateTargetExpr",
    "Microlith57Misc/SliderCameraOffsetTrigger=CreateOffset" /* , */
    // "Microlith57Misc/SliderCameraOffsetTrigger_Expression=CreateOffsetExpr",
    // "Microlith57Misc/SliderCameraZoomTrigger=CreateZoom",
    // "Microlith57Misc/SliderCameraZoomTrigger_Expression=CreateZoomExpr"
)]
public sealed class SliderCameraTrigger : Entity {

    // todo: plugins, lang

    // dummy
    private SliderCameraTrigger() { }

    #region --- Target ---

    public static CameraTargetTrigger CreateTarget(Level level, LevelData _, Vector2 offset, EntityData data) {
        var default_pos = data.Position + new Vector2(data.Width, data.Height) / 2f;

        if (data.Nodes.Length > 0)
            default_pos = data.Nodes[0];
        else
            data.Nodes = [default_pos];

        var res = new CameraTargetTrigger(data, offset) { Depth = Depths.Below };

        var targetSource = new Vector2Source(
            new FloatSource.Slider(level.Session, data, "targetSliderX") { Default = res.Target.X },
            new FloatSource.Slider(level.Session, data, "targetSliderY") { Default = res.Target.Y }
        );
        res.Add(targetSource);

        var lerpStrengthSource = new FloatSource.Slider(level.Session, data, "lerpStrengthSlider") { Default = res.LerpStrength };
        res.Add(lerpStrengthSource);

        res.PreUpdate += (_) => {
            // todo: ext zoom compat
            res.Target = targetSource.Value - new Vector2(320 / 2, 180 / 2);
            res.LerpStrength = lerpStrengthSource.Value;
        };

        return res;
    }

    #endregion Target
    #region --- Offset ---

    public static CameraOffsetTrigger CreateOffset(Level level, LevelData _, Vector2 offset, EntityData data) {
        // todo
        bool coarse = data.Bool("coarse", true);

        var res = new CameraOffsetTrigger(data, offset) { Depth = Depths.Below };

        var offsetSource = new Vector2Source(
            new FloatSource.Slider(level.Session, data, "offsetSliderX") { Default = res.CameraOffset.X },
            new FloatSource.Slider(level.Session, data, "offsetSliderY") { Default = res.CameraOffset.Y }
        );
        res.Add(offsetSource);

        res.PreUpdate += (_) => {
            res.CameraOffset = offsetSource.Value;
        };

        return res;
    }

    #endregion Offset

}
