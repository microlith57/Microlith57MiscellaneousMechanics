using System;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Components;

public class ConditionSource(bool invert = false) : Component(active: false, visible: false) {

    public virtual bool? RawValue => null;

    public bool Default = false;
    public bool Invert = invert;
    public bool Value => RawValue.HasValue ? RawValue.Value ^ Invert : Default;

    public class FuncSource(Func<bool> func, bool invert = false) : ConditionSource(invert) {

        private readonly Func<bool> Func = func;

        public override bool? RawValue => Func() ^ Invert;

    }

    public class FlagSource(string flag, bool invert = false) : ConditionSource(invert) {

        public readonly string Flag = flag;

        public FlagSource(
            EntityData data,
            string name = "flag",
            string ifAbsent = "",
            string invertName = "invertFlag"
        ) : this(
            data.Attr(name, ifAbsent),
            data.Bool(invertName)
        ) {}

        public override bool? RawValue =>
            (Flag == "" || Scene is not Level level)
                ? null
                : level.Session.GetFlag(Flag);

    }

    public class ExpressionSource : ConditionSource {

        public readonly string RawExpression;
        private readonly object? Expression;

        public ExpressionSource(
            string raw,
            bool invert = false
        ) : base(invert) {

            RawExpression = raw;

            if (RawExpression == "") return;

            if (Imports.FrostHelper.TryCreateSessionExpression == null)
                throw new Exception("tried to use a frosthelper session expression, but frosthelper is not loaded");

            Imports.FrostHelper.TryCreateSessionExpression(RawExpression, out Expression);
        }

        public ExpressionSource(
            EntityData data,
            string name = "expression",
            string ifAbsent = "",
            bool invert = false
        ) : this(
            data.Attr(name, ifAbsent),
            invert
        ) {}

        public override bool? RawValue =>
            (Expression == null || Scene is not Level level)
                ? null
                : Imports.FrostHelper.GetBoolSessionExpressionValue!(Expression, level.Session);

    }

}
