using System;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Components;

public class ConditionSource(bool invert = false) : Component(active: false, visible: false) {

    public virtual bool? RawValue => null;

    public bool Default = false;
    public bool Invert = invert;
    public bool Value => RawValue.HasValue ? RawValue.Value ^ Invert : Default;

    public class Function(Func<bool> func, bool invert = false) : ConditionSource(invert) {

        private readonly Func<bool> _Func = func;
        public override bool? RawValue => _Func() ^ Invert;

    }

    public class Flag(string flag, bool invert = false) : ConditionSource(invert) {

        public readonly string _Flag = flag;

        public Flag(
            EntityData data,
            string name = "flag",
            string ifAbsent = "",
            string invertName = "invertFlag"
        ) : this(
            data.Attr(name, ifAbsent),
            data.Bool(invertName)
        ) {}

        public override bool? RawValue =>
            (_Flag == "" || Scene is not Level level)
                ? null
                : level.Session.GetFlag(_Flag);

    }

    public class Expr : ConditionSource {

        private readonly object? _Expr;

        public Expr(
            string raw,
            bool invert = false
        ) : base(invert) {

            if (raw == "") return;

            if (Imports.FrostHelper.TryCreateSessionExpression == null)
                throw new Exception("tried to use a frosthelper session expression, but frosthelper is not loaded");

            Imports.FrostHelper.TryCreateSessionExpression(raw, out _Expr);
        }

        public Expr(
            EntityData data,
            string name = "expression",
            string ifAbsent = "",
            bool invert = false
        ) : this(
            data.Attr(name, ifAbsent),
            invert
        ) {}

        public override bool? RawValue =>
            (_Expr == null || Scene is not Level level)
                ? null
                : Imports.FrostHelper.GetBoolSessionExpressionValue!(_Expr, level.Session);

    }

}
