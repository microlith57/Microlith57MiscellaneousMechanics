using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using System;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity("Microlith57Misc/LevelTimerToCounterController")]
public sealed class LevelTimerToCounterController(EntityData data, Vector2 offset) : Entity(data.Position + offset) {

    public enum WrapMode {
        NoWrapping,
        Positive,
        FullRange,
    }

    public enum PrecisionMode {
        Seconds,
        FramesProbably,
        Milliseconds,
    }

    public (WrapMode Wrap, PrecisionMode Precision) Mode
        = (data.Enum<WrapMode>("wrapMode"), data.Enum<PrecisionMode>("precisionMode"));


    public string Flag = data.Attr("flag", "");
    public bool InvertFlag = data.Bool("invertFlag");
    public string Counter = data.Attr("counter", "levelTimer");

    public override void Update() {
        base.Update();

        var session = (Scene as Level)!.Session;

        if (string.IsNullOrEmpty(Flag) || (session.GetFlag(Flag) ^ InvertFlag))
            session.SetCounter(Counter, PerformWrapping(Math.Floor(GetValue(session.Time))));
    }

    private double GetValue(long time) {
        var span = TimeSpan.FromTicks(time);

        switch (Mode.Precision) {
            case PrecisionMode.Seconds:
                return span.TotalSeconds;
            case PrecisionMode.FramesProbably:
                return span.TotalSeconds * 60f;
            case PrecisionMode.Milliseconds:
                return span.TotalMilliseconds;
            default:
                throw new Exception("should be unreachable!");
        }
    }

    private int PerformWrapping(double val) {
        if (val <= int.MaxValue)
            return (int)val;
        else switch (Mode.Wrap) {
                case WrapMode.NoWrapping:
                    return int.MaxValue;
                case WrapMode.Positive:
                    return (int)(val % ((double)int.MaxValue + 1));
                case WrapMode.FullRange:
                    return (int)val;
                default:
                    throw new Exception("should be unreachable!");
            }
    }

}
