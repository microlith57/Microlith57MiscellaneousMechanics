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

    public class Flag : ConditionSource {
        public readonly string _Flag;

        public Flag(string flag, bool invert = false) : base(invert) {
            flag = flag.Trim();
            while (flag.StartsWith('!')) {
                flag = flag.Remove(0, 1).TrimStart();
                Invert = !Invert;
            }
            _Flag = flag;
        }

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
            (string.IsNullOrEmpty(_Flag) || Scene is not Level level)
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
                : Imports.FrostHelper.GetBoolSessionExpressionValue(_Expr, level.Session);
    }

    public static ConditionSource From(
        FlagOrExpr conditionType,
        EntityData data,
        string namePrefix = "",
        string? name = null,
        string? legacyInvertName = "invertFlag",
        string ifAbsent = "",
        bool invert = false,
        bool @default = false
    ) {
        string FormatName(string suffix) {
            if (name is not null) return name;
            if (string.IsNullOrEmpty(namePrefix)) return suffix;
            return name + suffix.Substring(0, 1).ToUpperInvariant() + suffix.Substring(1);
        }

        switch (conditionType) {
            case FlagOrExpr.Flag:
                return new Flag(data, FormatName("flag"), ifAbsent, legacyInvertName) {Invert = invert, Default = @default};
            case FlagOrExpr.Expr:
                return new Expr(data, FormatName("expression"), ifAbsent) {Invert = invert, Default = @default};
        }
    }
}
