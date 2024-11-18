using System;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Components;

public class ConditionSource() : Component(active: false, visible: false) {

    public bool Default = false;
    public virtual bool Value => Default;

    public static ConditionSource Create(EntityData data, bool @default = false) {
        if (data.Attr("sessionExpression") is { } sessionExpression && sessionExpression != "")
            return new ExpressionSource(sessionExpression) { Default = @default };

        if (data.Attr("flag") is { } flag && flag != "")
            return new FlagSource(flag) { Default = @default };

        return new ConditionSource() { Default = @default };
    }

    public class FlagSource(string flag, bool invert = false) : ConditionSource {

        public readonly string Flag = flag;
        public bool Invert = invert;

        public override bool Value =>
            (Flag != "" || Scene is not Level level)
                ? Default
                : level.Session.GetFlag(Flag) ^ Invert;

    }

    public class ExpressionSource : ConditionSource {

        public readonly string RawExpression;
        private readonly object? Expression;

        public ExpressionSource(string raw) {
            RawExpression = raw;

            if (RawExpression == "") return;

            if (Imports.FrostHelper.TryCreateSessionExpression == null)
                throw new Exception("tried to use a frosthelper session expression, but frosthelper is not loaded");

            Imports.FrostHelper.TryCreateSessionExpression(RawExpression, out Expression);
        }

        public override bool Value =>
            (Expression == null || Scene is not Level level)
                ? Default
                : Imports.FrostHelper.GetIntSessionExpressionValue!(Expression, level.Session) != 0;

    }

}
