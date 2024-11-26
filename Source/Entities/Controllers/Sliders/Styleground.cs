using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;

using Celeste.Mod.Microlith57Misc.Components;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity(
    "Microlith57Misc/SliderStylegroundController=CreateFlag",
    "Microlith57Misc/SliderStylegroundController_Expression=CreateExpr"
)]
[Tracked]
public sealed class SliderStylegroundController : Entity {

    #region --- State ---

    public readonly string StyleTag;

    private readonly ConditionSource EnabledCondition;
    public bool Enabled => EnabledCondition.Value;

    private readonly Vector2Source PositionSource, ScrollSource, SpeedSource;

    private readonly IntSource ColorSource;
    public Color? Color => ColorSource.RawColorValue;

    private readonly FloatSource AlphaSource;
    public float? Alpha => AlphaSource.RawValue;

    #endregion State
    #region --- Init ---

    public SliderStylegroundController(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        Vector2Source positionSource,
        Vector2Source scrollSource,
        Vector2Source speedSource,
        IntSource colorSource,
        FloatSource alphaSource
    ) : base(data.Position + offset) {

        StyleTag = data.Attr("tag");

        Add(EnabledCondition = enabledCondition);
        this.Add(PositionSource = positionSource);
        this.Add(ScrollSource = scrollSource);
        this.Add(SpeedSource = speedSource);
        Add(ColorSource = colorSource);
        Add(AlphaSource = alphaSource);
    }

    public static SliderStylegroundController CreateFlag(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.FlagSource(data) { Default = true },
            Vector2Source.SliderSource(level.Session, data, "position"),
            Vector2Source.SliderSource(level.Session, data, "scroll"),
            Vector2Source.SliderSource(level.Session, data, "speed"),
            new IntSource.CounterSource(level.Session, data, "packedColor"),
            new FloatSource.SliderSource(level.Session, data, "alphaMultiplier")
        );

    public static SliderStylegroundController CreateExpr(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.ExpressionSource(data) { Default = true },
            Vector2Source.ExpressionSource(data, "position"),
            Vector2Source.ExpressionSource(data, "scroll"),
            Vector2Source.ExpressionSource(data, "speed"),
            new IntSource.ExpressionSource(data, "packedColor"),
            new FloatSource.ExpressionSource(data, "alphaMultiplier")
        );

    #endregion Init
    #region --- Behaviour ---

    public override void Update() {
        base.Update();

        if (Scene is not Level level) return;

        float? position_x = PositionSource.X.RawValue;
        float? position_y = PositionSource.X.RawValue;

        float? scroll_x = ScrollSource.X.RawValue;
        float? scroll_y = ScrollSource.X.RawValue;

        float? speed_x = SpeedSource.X.RawValue;
        float? speed_y = SpeedSource.X.RawValue;

        float? alpha = Alpha;
        Color? color = Color;

        foreach (Backdrop backdrop in level.Background.GetEach<Backdrop>(StyleTag)) {
            if (position_x != null) backdrop.Position.X = position_x.Value;
            if (position_y != null) backdrop.Position.X = position_y.Value;

            if (scroll_x != null) backdrop.Scroll.X = scroll_x.Value;
            if (scroll_y != null) backdrop.Scroll.X = scroll_y.Value;

            if (speed_x != null) backdrop.Speed.X = speed_x.Value;
            if (speed_y != null) backdrop.Speed.X = speed_y.Value;

            if (color != null) backdrop.Color = color.Value;
            if (alpha != null) backdrop.FadeAlphaMultiplier = alpha.Value;
        }

        foreach (Backdrop backdrop in level.Foreground.GetEach<Backdrop>(StyleTag)) {
            if (position_x != null) backdrop.Position.X = position_x.Value;
            if (position_y != null) backdrop.Position.X = position_y.Value;

            if (scroll_x != null) backdrop.Scroll.X = scroll_x.Value;
            if (scroll_y != null) backdrop.Scroll.X = scroll_y.Value;

            if (speed_x != null) backdrop.Speed.X = speed_x.Value;
            if (speed_y != null) backdrop.Speed.X = speed_y.Value;

            if (color != null) backdrop.Color = color.Value;
            if (alpha != null) backdrop.FadeAlphaMultiplier = alpha.Value;
        }
    }

    #endregion Behaviour

}