using System;
using System.Diagnostics.CodeAnalysis;
using MonoMod.ModInterop;

namespace Celeste.Mod.Microlith57Misc.Imports;

[ModImportName("FrostHelper")]
public static class FrostHelper {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    /// <summary>
    /// Creates an object which can evaluate a Session Expression.
    /// The returned object can be passed to <see cref="GetIntSessionExpressionValue"/>
    /// </summary>
    public delegate bool _TryCreateSessionExpression(string str, [NotNullWhen(true)] out object? expression);
    public static _TryCreateSessionExpression TryCreateSessionExpression;

    /// <summary>
    /// Returns the current value of a Session Expression.
    /// The object passed as the 1st argument needs to be created via <see cref="TryCreateSessionExpression"/>
    /// </summary>
    public delegate object _GetSessionExpressionValue(object expression, Session session);
    public static _GetSessionExpressionValue GetSessionExpressionValue;

    /// <summary>
    /// Returns the current value of a Session Expression as a bool, coercing if needed.
    /// The object passed as the 1st argument needs to be created via <see cref="TryCreateSessionExpression"/>
    /// </summary>
    public delegate bool _GetBoolSessionExpressionValue(object expression, Session session);
    public static _GetBoolSessionExpressionValue GetBoolSessionExpressionValue;

    /// <summary>
    /// Returns the current value of a Session Expression as an integer, coercing if needed.
    /// The object passed as the 1st argument needs to be created via <see cref="TryCreateSessionExpression"/>
    /// </summary>
    public delegate int _GetIntSessionExpressionValue(object expression, Session session);
    public static _GetIntSessionExpressionValue GetIntSessionExpressionValue;

    /// <summary>
    /// Returns the current value of a Session Expression as a float, coercing if needed.
    /// The object passed as the 1st argument needs to be created via <see cref="TryCreateSessionExpression"/>
    /// </summary>
    public delegate float _GetFloatSessionExpressionValue(object expression, Session session);
    public static _GetFloatSessionExpressionValue GetFloatSessionExpressionValue;

    /// <summary>
    /// Returns the type that the given session expression will return, or typeof(object) if that's unknown.
    /// The object passed as the 1st argument needs to be created via <see cref="TryCreateSessionExpression"/>
    /// </summary>
    public delegate Type _GetSessionExpressionReturnedType(object expression);
    public static _GetSessionExpressionReturnedType GetSessionExpressionReturnedType;

    /// <summary>
    /// Registers a simple Session Expression command, which will be accessible via $modName.cmdName in Session Expressions.
    /// </summary>
    /// <param name="modName">Name of the mod which registers this command. Will be used to prefix the command name</param>
    /// <param name="cmdName">Name of the command</param>
    /// <param name="func">Function called each time the command needs to be evaluated</param>
    public delegate void _RegisterSimpleSessionExpressionCommand(string modName, string cmdName, Func<Session, object> func);
    public static _RegisterSimpleSessionExpressionCommand RegisterSimpleSessionExpressionCommand;

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}