using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Components;

public class IntSource() : Component(active: false, visible: false) {

    public virtual int? RawValue => null;
    public Color? RawColorValue {
        get {
            int? val = RawValue;
            if (val == null) return null;

            return new() { PackedValue = unchecked((uint)Value) };
        }
    }

    public int Default = 0;
    public int Value => RawValue ?? Default;
    public Color ColorValue => new() { PackedValue = unchecked((uint)Value) };

    public class Function(Func<int> func) : IntSource {

        private readonly Func<int> _Func = func;
        public override int? RawValue => _Func();

    }

    public class CounterSource : IntSource {

        private readonly string _Counter;

        public CounterSource(
            Session session,
            EntityData data,
            string name = "slider",
            string ifAbsent = ""
        ) : base() {
            _Counter = data.Attr(name, ifAbsent);
        }

        public override int? RawValue =>
            (_Counter == "" || Scene is not Level level)
                ? null
                : level.Session.GetCounter(_Counter);

    }

    public class Expr : IntSource {

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

        public override int? RawValue =>
            (_Expr == null || Scene is not Level level)
                ? null
                : Imports.FrostHelper.GetIntSessionExpressionValue!(_Expr, level.Session);

    }

}
