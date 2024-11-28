using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Components;

public class FloatSource() : Component(active: false, visible: false) {

    public virtual float? RawValue => null;

    public float Default = 0f;
    public float Value => RawValue ?? Default;

    public class Function(Func<float> func) : FloatSource {

        private readonly Func<float> _Func = func;
        public override float? RawValue => _Func();

    }

    public class Slider : FloatSource {

        public readonly Session.Slider? _Slider;

        public Slider(Session.Slider? slider) {
            _Slider = slider;
        }

        public Slider(
            Session session,
            EntityData data,
            string name = "slider",
            string ifAbsent = ""
        ) : base() {
            string slider = data.Attr(name, ifAbsent);

            if (slider != "")
                _Slider = session.GetSliderObject(slider);
        }

        public override float? RawValue => _Slider?.Value;

    }

    public class Expr : FloatSource {

        private readonly object? _Expr;

        public Expr(string raw) {
            if (raw == "") return;

            if (Imports.FrostHelper.TryCreateSessionExpression == null)
                throw new Exception("tried to use a frosthelper session expression, but frosthelper is not loaded");

            Imports.FrostHelper.TryCreateSessionExpression(raw, out _Expr);
        }

        public Expr(
            EntityData data,
            string name = "expression",
            string ifAbsent = ""
        ) : this(
            data.Attr(name, ifAbsent)
        ) { }

        public override float? RawValue =>
            (_Expr == null || Scene is not Level level)
                ? null
                : Imports.FrostHelper.GetFloatSessionExpressionValue!(_Expr, level.Session);

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
            new FloatSource.Slider(session, data, name + "X", ifXAbsent),
            new FloatSource.Slider(session, data, name + "Y", ifYAbsent)
        );

    public static Vector2Source ExprSource(
        EntityData data,
        string name = "expression",
        string ifXAbsent = "",
        string ifYAbsent = ""
    ) => new(
            new FloatSource.Expr(data, name + "X", ifXAbsent),
            new FloatSource.Expr(data, name + "Y", ifYAbsent)
        );

}
