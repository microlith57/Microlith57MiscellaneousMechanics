using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System.Linq;

using Celeste.Mod.Microlith57Misc.Components;
using static Celeste.Mod.Microlith57Misc.Components.ConditionSource;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity(
    "Microlith57Misc/FreezeTimeActiveController=CreateFlag",
    "Microlith57Misc/FreezeTimeActiveController_Expression=CreateExpr"
)]
[Tracked]
public sealed class FreezeTimeActiveController : Entity {

    #region --- State ---

    private readonly ConditionSource Condition;
    public bool FreezeActive => Condition.Value;

    #endregion State
    #region --- Init ---

    public FreezeTimeActiveController(
        Vector2 position,
        ConditionSource condition
    ) : base(position) {

        Add(Condition = condition);
    }

    public static FreezeTimeActiveController CreateFlag(Level _, LevelData __, Vector2 offset, EntityData data)
        => new(
                data.Position + offset,
                new FlagSource(data, ifAbsent: "freezeTimeActive") { Default = true }
            );

    public static FreezeTimeActiveController CreateExpr(Level _, LevelData __, Vector2 offset, EntityData data)
        => new(
                data.Position + offset,
                new ExpressionSource(data, "expression", "freezeTimeActive") { Default = true }
            );

    #endregion Init
    #region --- Behaviour ---

    private static bool AppliesTo(Scene scene)
        => scene is Level level
        && level.Tracker.GetEntities<FreezeTimeActiveController>()
            .Any(c => c is FreezeTimeActiveController ctrl && ctrl.FreezeActive);

    #endregion Behaviour
    #region --- Hook ---

    internal static void manipSceneBeforeUpdate(ILContext il) {
        ILCursor cursor = new(il);

        cursor.GotoNext(MoveType.After, instr => instr.MatchLdfld<Scene>("Paused"));

        cursor.Emit(OpCodes.Ldarg_0);
        cursor.EmitDelegate(AppliesTo);
        cursor.Emit(OpCodes.Or);
    }

    #endregion Hook

}
