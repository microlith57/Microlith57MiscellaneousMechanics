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

    public class FuncSource(Func<int> func) : IntSource {

        private readonly Func<int> Func = func;

        public override int? RawValue => Func();

    }

    public class CounterSource : IntSource {

        public readonly string Counter;

        public CounterSource(
            Session session,
            EntityData data,
            string name = "slider",
            string ifAbsent = ""
        ) : base() {
            Counter = data.Attr(name, ifAbsent);
        }

        public override int? RawValue =>
            (Counter == "" || Scene is not Level level)
                ? null
                : level.Session.GetCounter(Counter);

    }

    public class ExpressionSource : IntSource {

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

        public override int? RawValue =>
            (Expression == null || Scene is not Level level)
                ? null
                : Imports.FrostHelper.GetIntSessionExpressionValue!(Expression, level.Session);

    }

}
