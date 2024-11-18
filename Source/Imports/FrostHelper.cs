using System;
using System.Diagnostics.CodeAnalysis;
using MonoMod.ModInterop;

namespace Celeste.Mod.Microlith57Misc.Imports;

[ModImportName("FrostHelper")]
public static class FrostHelper {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public delegate bool _TryCreateSessionExpression(string str, [NotNullWhen(true)] out object? expression);
    public static _TryCreateSessionExpression TryCreateSessionExpression { get; set; }

    public delegate int _GetIntSessionExpressionValue(object expression, Session session);
    public static _GetIntSessionExpressionValue GetIntSessionExpressionValue { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}