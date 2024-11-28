using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.Entities;
using Celeste.Mod.Microlith57Misc.Components;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Entities.Interpolator;

internal abstract class InterpolatorStop<T>() {

    #region --- Abstract ---

    public abstract T Value { get; }
    public abstract T Lerp(T next, float t);

    internal virtual void Added(Entity entity) { }
    internal virtual void Removed(Entity entity) { }

    #endregion Abstract

}
#region --- Impls ---

internal class FlagStop(bool value) : InterpolatorStop<bool>() {
    public override bool Value => value;
    public override bool Lerp(bool _, float __) => Value;
}

internal class DynamicFlagStop(ConditionSource source) : FlagStop(default) {
    public override bool Value => source.Value;
    internal override void Added(Entity e) => e.Add(source);
    internal override void Removed(Entity e) => e.Remove(source);
}

internal class SliderStop(float value) : InterpolatorStop<float>() {
    public override float Value => value;
    public override float Lerp(float next, float t)
        => Calc.LerpClamp(Value, next, t);
}

internal class DynamicSliderStop(FloatSource source) : SliderStop(default) {
    public override float Value => source.Value;
    internal override void Added(Entity e) => e.Add(source);
    internal override void Removed(Entity e) => e.Remove(source);
}

internal class ColorStop(Color value) : InterpolatorStop<Color>() {
    public override Color Value => value;
    public override Color Lerp(Color next, float t)
        => Color.Lerp(Value, next, t);
}

internal class DynamicColorStop(IntSource source) : ColorStop(default) {
    public override Color Value => source.ColorValue;
    internal override void Added(Entity e) => e.Add(source);
    internal override void Removed(Entity e) => e.Remove(source);
}

#endregion Impls
#region --- Holder ---

[CustomEntity(
    "Microlith57Misc/InterpolatorStop_Flag=CreateFlag",
    "Microlith57Misc/InterpolatorStop_Flag_Dynamic=CreateFlag_Dynamic",
    "Microlith57Misc/InterpolatorStop_Flag_Expression=CreateFlag_Expression",
    "Microlith57Misc/InterpolatorStop_Slider=CreateSlider",
    "Microlith57Misc/InterpolatorStop_Slider_Dynamic=CreateSlider_Dynamic",
    "Microlith57Misc/InterpolatorStop_Slider_Expression=CreateSlider_Expression",
    "Microlith57Misc/InterpolatorStop_Color=CreateColor",
    "Microlith57Misc/InterpolatorStop_Color_Dynamic=CreateColor_Dynamic",
    "Microlith57Misc/InterpolatorStop_Color_Expression=CreateColor_Expression"
)]
[Tracked]
internal class InterpolatorStopHolder : Entity {

    public static InterpolatorStopHolder Get(Level level) {
        if (level.Tracker.GetEntity<InterpolatorStopHolder>() is InterpolatorStopHolder holder)
            return holder;

        level.Add(holder = new());
        return holder;
    }

    #region > Flag

    internal Dictionary<string, HashSet<(Vector2 position, FlagStop stop)>> FlagStops = [];

    public static Entity Add(Level level, Vector2 position, string subtrack, FlagStop stop) {
        var dict = Get(level).FlagStops;
        if (dict.TryGetValue(subtrack, out var list))
            dict[subtrack] = list = [];

        list!.Add((position, stop));
        return null!;
    }

    public static Entity CreateFlag(Level level, LevelData __, Vector2 offset, EntityData data)
        => Add(
            level, data.Position + offset,
            data.Attr("flag"),
            new FlagStop(data.Bool("value"))
        );

    public static Entity CreateFlag_Dynamic(Level level, LevelData __, Vector2 offset, EntityData data)
        => Add(
            level, data.Position + offset,
            data.Attr("flag"),
            new DynamicFlagStop(new ConditionSource.Flag(
                data, "value", invertName: "invertValue"
            ))
        );

    public static Entity CreateFlag_Expression(Level level, LevelData __, Vector2 offset, EntityData data)
        => Add(
            level, data.Position + offset,
            data.Attr("flag"),
            new DynamicFlagStop(new ConditionSource.Expr(
                data, name: "value"
            ))
        );

    #endregion Flag
    #region > Slider

    internal Dictionary<string, HashSet<(Vector2 position, SliderStop stop)>> SliderStops = [];

    public static Entity Add(Level level, Vector2 position, string subtrack, SliderStop stop) {
        var dict = Get(level).SliderStops;
        if (dict.TryGetValue(subtrack, out var list))
            dict[subtrack] = list = [];

        list!.Add((position, stop));
        return null!;
    }

    public static Entity CreateSlider(Level level, LevelData __, Vector2 offset, EntityData data)
        => Add(
            level, data.Position + offset,
            data.Attr("slider"),
            new SliderStop(data.Float("value"))
        );

    public static Entity CreateSlider_Dynamic(Level level, LevelData __, Vector2 offset, EntityData data)
        => Add(
            level, data.Position + offset,
            data.Attr("slider"),
            new DynamicSliderStop(new FloatSource.Slider(
                level.Session, data, name: "value"
            ))
        );

    public static Entity CreateSlider_Expression(Level level, LevelData __, Vector2 offset, EntityData data)
        => Add(
            level, data.Position + offset,
            data.Attr("slider"),
            new DynamicSliderStop(new FloatSource.Expr(
                data, name: "value"
            ))
        );

    #endregion Slider
    #region > Color

    internal Dictionary<string, HashSet<(Vector2 position, ColorStop stop)>> ColorStops = [];

    public static Entity Add(Level level, Vector2 position, string subtrack, ColorStop stop) {
        var dict = Get(level).ColorStops;
        if (dict.TryGetValue(subtrack, out var list))
            dict[subtrack] = list = [];

        list!.Add((position, stop));
        return null!;
    }

    public static Entity CreateColor(Level level, LevelData __, Vector2 offset, EntityData data)
        => Add(
            level, data.Position + offset,
            data.Attr("counter"),
            new ColorStop(
                Calc.HexToColorWithAlpha(data.Attr("value")) * data.Float("alpha")
            )
        );

    public static Entity CreateColor_Dynamic(Level level, LevelData __, Vector2 offset, EntityData data)
        => Add(
            level, data.Position + offset,
            data.Attr("counter"),
            new DynamicColorStop(
                new IntSource.CounterSource(level.Session, data, name: "value")
            )
        );

    public static Entity CreateColor_Expression(Level level, LevelData __, Vector2 offset, EntityData data)
        => Add(
            level, data.Position + offset,
            data.Attr("counter"),
            new DynamicColorStop(
                new IntSource.Expr(data, name: "value")
            )
        );

    #endregion Color
    #region --- Behaviour ---

    public override void Awake(Scene scene) {
        base.Awake(scene);

        var tracks = scene.Tracker.GetEntities<InterpolatorTrack>();
        if (tracks.Count == 0) goto end;

        foreach ((var subtrack_name, var stops) in FlagStops) {
            foreach (InterpolatorTrack track in tracks) {
                var this_track = stops
                    .Where(s => track.CollidePoint(s.position))
                    .OrderBy(s => s.position.X)
                    .ToList();
                if (this_track.Count == 0) continue;

                var subtrack = new InterpolatorTrack.FlagSubtrack(subtrack_name);
                track.Subtracks.Add(subtrack);
                track.Add(subtrack);

                foreach (var stop in this_track) {
                    subtrack.Add(track.PosToFac(stop.position), stop.stop);
                    stops.Remove(stop);
                }
            }
        }

        foreach ((var subtrack_name, var stops) in SliderStops)  {
            foreach (InterpolatorTrack track in tracks) {
                var this_track = stops.Where(s => track.CollidePoint(s.position)).ToList();
                if (this_track.Count == 0) continue;

                var subtrack = new InterpolatorTrack.SliderSubtrack(subtrack_name);
                track.Subtracks.Add(subtrack);
                track.Add(subtrack);

                foreach (var stop in this_track) {
                    subtrack.Add(track.PosToFac(stop.position), stop.stop);
                    stops.Remove(stop);
                }
            }
        }

        foreach ((var subtrack_name, var stops) in ColorStops)  {
            foreach (InterpolatorTrack track in tracks) {
                var this_track = stops.Where(s => track.CollidePoint(s.position)).ToList();
                if (this_track.Count == 0) continue;

                var subtrack = new InterpolatorTrack.ColorSubtrack(subtrack_name);
                track.Subtracks.Add(subtrack);
                track.Add(subtrack);

                foreach (var stop in this_track) {
                    subtrack.Add(track.PosToFac(stop.position), stop.stop);
                    stops.Remove(stop);
                }
            }
        }

    end:
        RemoveSelf();
    }

    #endregion Behaviour

}

#endregion --- Holder ---
