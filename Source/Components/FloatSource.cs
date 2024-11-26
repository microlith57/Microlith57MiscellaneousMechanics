using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Components;

public class FloatSource() : Component(active: false, visible: false) {

    public virtual float? RawValue => null;

    public float Default = 0f;
    public float Value => RawValue ?? Default;

    public class FuncSource(Func<float> func) : FloatSource {

        private readonly Func<float> Func = func;

        public override float? RawValue => Func();

    }

    public class SliderSource : FloatSource {

        public readonly Session.Slider? Slider;

        public SliderSource(Session.Slider? slider) {
            Slider = slider;
        }

        public SliderSource(
            Session session,
            EntityData data,
            string name = "slider",
            string ifAbsent = ""
        ) : base() {
            string slider = data.Attr(name, ifAbsent);

            if (slider != "")
                Slider = session.GetSliderObject(slider);
        }

        public override float? RawValue => Slider?.Value;

    }

    public class ExpressionSource : FloatSource {

        public readonly string RawExpression;
        private readonly object? Expression;

        public ExpressionSource(string raw) {
            RawExpression = raw;

            if (RawExpression == "") return;

            if (Imports.FrostHelper.TryCreateSessionExpression == null)
                throw new Exception("tried to use a frosthelper session expression, but frosthelper is not loaded");

            Imports.FrostHelper.TryCreateSessionExpression(RawExpression, out Expression);
        }

        public ExpressionSource(
            EntityData data,
            string name = "expression",
            string ifAbsent = ""
        ) : this(
            data.Attr(name, ifAbsent)
        ) { }

        public override float? RawValue =>
            (Expression == null || Scene is not Level level)
                ? null
                : Imports.FrostHelper.GetFloatSessionExpressionValue!(Expression, level.Session);

    }

}

public struct Vector2Source(FloatSource x, FloatSource y) {

    public readonly FloatSource X = x;
    public readonly FloatSource Y = y;

    public Vector2? RawValue {
        get {
            (float? x, float? y) = (X.RawValue, Y.RawValue);

            if (x == null || y == null) return null;

            return new(x.Value, y.Value);
        }
    }

    public readonly Vector2 Value => new(X.Value, Y.Value);

    public static Vector2Source SliderSource(
        Session session,
        EntityData data,
        string name = "slider",
        string ifXAbsent = "",
        string ifYAbsent = ""
    ) => new(
            new FloatSource.SliderSource(session, data, name + "X", ifXAbsent),
            new FloatSource.SliderSource(session, data, name + "Y", ifYAbsent)
        );

    public static Vector2Source ExpressionSource(
        EntityData data,
        string name = "expression",
        string ifXAbsent = "",
        string ifYAbsent = ""
    ) => new(
            new FloatSource.ExpressionSource(data, name + "X", ifXAbsent),
            new FloatSource.ExpressionSource(data, name + "Y", ifYAbsent)
        );

}
