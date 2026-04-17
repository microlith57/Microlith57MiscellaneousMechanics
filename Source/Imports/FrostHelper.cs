using System.Diagnostics.CodeAnalysis;
using ModInteropImportGenerator;

namespace Celeste.Mod.Microlith57Misc.Imports;

[GenerateImports("FrostHelper")]
public static partial class FrostHelper {
    /// <summary>
    /// Creates an object which can evaluate a Session Expression.
    /// The returned object can be passed to <see cref="GetIntSessionExpressionValue"/>
    /// </summary>
    public static partial bool TryCreateSessionExpression(string str, [NotNullWhen(true)] out object? expression);

    /// <summary>
    /// Returns the current value of a Session Expression.
    /// The object passed as the 1st argument needs to be created via <see cref="TryCreateSessionExpression"/>
    /// </summary>
    public static partial object GetSessionExpressionValue(object expression, Session session);

    /// <summary>
    /// Returns the current value of a Session Expression as a bool, coercing if needed.
    /// The object passed as the 1st argument needs to be created via <see cref="TryCreateSessionExpression"/>
    /// </summary>
    public static partial bool GetBoolSessionExpressionValue(object expression, Session session);

    /// <summary>
    /// Returns the current value of a Session Expression as an integer, coercing if needed.
    /// The object passed as the 1st argument needs to be created via <see cref="TryCreateSessionExpression"/>
    /// </summary>
    public static partial int GetIntSessionExpressionValue(object expression, Session session);

    /// <summary>
    /// Returns the current value of a Session Expression as a float, coercing if needed.
    /// The object passed as the 1st argument needs to be created via <see cref="TryCreateSessionExpression"/>
    /// </summary>
    public static partial float GetFloatSessionExpressionValue(object expression, Session session);

    /// <summary>
    /// Returns the type that the given session expression will return, or typeof(object) if that's unknown.
    /// The object passed as the 1st argument needs to be created via <see cref="TryCreateSessionExpression"/>
    /// </summary>
    public static partial Type GetSessionExpressionReturnedType(object expression);

    /// <summary>
    /// Registers a simple Session Expression command, which will be accessible via $modName.cmdName in Session Expressions.
    /// </summary>
    /// <param name="modName">Name of the mod which registers this command. Will be used to prefix the command name</param>
    /// <param name="cmdName">Name of the command</param>
    /// <param name="func">Function called each time the command needs to be evaluated</param>
    public static partial void RegisterSimpleSessionExpressionCommand(string modName, string cmdName, Func<Session, object> func);

    [OnLoad] internal static void OnLoad() => Load();
}
