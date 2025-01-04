using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using Monocle;
using System.Linq;

using Celeste.Mod.Microlith57Misc.Components;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity(
    "Microlith57Misc/SliderAudioListenerPositionController=Create",
    "Microlith57Misc/SliderAudioListenerPositionController_Expression=CreateExpr"
)]
[Tracked]
public sealed class SliderAudioListenerPositionController : SliderController {

    #region --- Init ---

    public readonly bool Relative = false;

    private readonly Vector2Source PositionSource;
    public Vector2 ListenerPosition => PositionSource.Value;

    private Module.AudioListenerOverride? ListenerOverride;

    public SliderAudioListenerPositionController(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        FloatSource valueSource,
        Vector2Source positionSource
    ) : base(data, offset, enabledCondition, valueSource) {

        this.Add(PositionSource = positionSource);
    }

    public static SliderAudioListenerPositionController Create(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Flag(data) { Default = true },
            new FloatSource.Slider(level.Session, data, name: "lerp"),
            Vector2Source.SliderSource(level.Session, data, name: "listener")
        );

    public static SliderAudioListenerPositionController CreateExpr(Level _, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Expr(data) { Default = true },
            new FloatSource.Expr(data, name: "lerp"),
            Vector2Source.ExprSource(data, name: "listener")
        );

    #endregion Init
    #region --- Behaviour ---

    public override void Awake(Scene scene) {
        base.Awake(scene);
        if (scene is not Level level) return;

        var bind = scene.Tracker
            .GetEntities<SliderAudioListenerPositionController>()
            .Cast<SliderAudioListenerPositionController>()
            .Where(s => s.ListenerOverride != null)
            .FirstOrDefault();

        if (bind != null) {
            ListenerOverride = bind.ListenerOverride;
            bind.ListenerOverride = null;
            bind.RemoveSelf();
        } else {
            ListenerOverride = new();
        }

        Module.OverrideAudioListener(level.Camera, ListenerOverride);
    }

    public override void Removed(Scene scene) {
        base.Removed(scene);

        if (scene is Level level && ListenerOverride != null)
            Module.OverrideAudioListener(level.Camera, null);
    }

    public override void Update() {
        base.Update();

        if (ListenerOverride == null) { RemoveSelf(); return; }

        ListenerOverride.Position = ListenerPosition;
        ListenerOverride.Factor = Value;
        ListenerOverride.Relative = Relative;
    }

    #endregion Behaviour

}