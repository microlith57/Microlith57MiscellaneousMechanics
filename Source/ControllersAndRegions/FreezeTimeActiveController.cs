using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System.Linq;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity("Microlith57Misc/FreezeTimeActiveController")]
[Tracked]
public sealed class FreezeTimeActiveController(EntityData data, Vector2 offset) : Entity(data.Position + offset) {

    #region --- Behaviour ---

    public string Flag = data.Attr("flag", "freezeTimeActive");
    public bool InvertFlag = data.Bool("invertFlag");

    private static bool AppliesTo(Scene scene)
        => scene is Level level
        && level.Tracker.GetEntities<FreezeTimeActiveController>()
            .Any(c => c is FreezeTimeActiveController ctrl && (
                    string.IsNullOrEmpty(ctrl.Flag) ||
                    (level.Session.GetFlag(ctrl.Flag) ^ ctrl.InvertFlag)
                ));

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
