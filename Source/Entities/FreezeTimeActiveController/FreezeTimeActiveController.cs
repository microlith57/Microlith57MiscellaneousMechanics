using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace Celeste.Mod.Microlith57Misc.Entities;

[GeneratedEntity] [Tracked]
public sealed partial class FreezeTimeActiveController(
    EntityData data, Vector2 offset,
    FlagOrExpr conditionType
): Controller(data.Position + offset) {

    [AddComponent]
    private readonly ConditionSource EnabledCondition = ConditionSource.From(conditionType, data, ifAbsent: "freezeTimeActive", @default: true);
    public bool FreezeActive => Condition.Value;

    private static bool AppliesTo(Scene scene)
        => scene is Level level
        && level.Tracker.GetEntities<FreezeTimeActiveController>()
            .Any(c => c is FreezeTimeActiveController ctrl && ctrl.FreezeActive);

    [OnLoad] internal static void Load() => IL.Monocle.Scene.BeforeUpdate += manipSceneBeforeUpdate;
    [OnUnload] internal static void Unload() => IL.Monocle.Scene.BeforeUpdate -= manipSceneBeforeUpdate;

    private static void manipSceneBeforeUpdate(ILContext il) {
        ILCursor cursor = new(il);

        cursor.GotoNext(MoveType.After, instr => instr.MatchLdfld<Scene>("Paused"));
        cursor.Emit(OpCodes.Ldarg_0);
        cursor.EmitDelegate(AppliesTo);
        cursor.Emit(OpCodes.Or);
    }
}
