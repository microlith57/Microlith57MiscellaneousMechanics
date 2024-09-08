using System.Reflection;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using Monocle;
using MonoMod.Cil;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity("Microlith57Misc/HoldablePriorityController")]
[Tracked]

public sealed class HoldablePriorityController(EntityData data, Vector2 offset) : Entity(data.Position + offset) {

    #region --- State, Behaviour ---

    public Vector2 CheckOffset = new(data.Float("checkOffsetX", 6f), data.Float("checkOffsetY", 0f));

    private static bool AppliesTo(Player player) => player.Scene.Tracker.GetEntity<HoldablePriorityController>() != null;

    private static bool TryPickupAny(Player player) {
        var controller = player.Scene.Tracker.GetEntity<HoldablePriorityController>();
        var checkPos = player.Center + ((int)player.Facing * controller.CheckOffset);

        var holdables = player.Scene.Tracker.GetComponents<Holdable>();

        if (holdables.Count == 0)
            return false;

        Holdable? closest = null;
        float closest_dist = 0f;

        foreach (Holdable holdable in holdables) {
            if (!holdable.Check(player))
                continue;

            var dist = (checkPos - (holdable.Entity.Position + holdable.PickupCollider.Center)).LengthSquared();
            if (closest == null || dist < closest_dist) {
                closest = holdable;
                closest_dist = dist;
            }
        }

        if (closest != null)
            return player.Pickup(closest);

        return false;
    }

    #endregion Init, State, Behaviour
    #region --- Hook ---

    internal static void manipPlayerNormalUpdate(ILContext il) {
        ILCursor cursor = new(il);

        cursor.GotoNext(instr => instr.MatchCallOrCallvirt<Player>("get_Holding"));
        cursor.GotoNext(instr => instr.MatchCallOrCallvirt<Tracker>("GetComponents"));

        // | foreach (var h in Scene.Tracker.GetComponents<Holdable>())
        cursor.GotoPrev(MoveType.AfterLabel,
                        instr => instr.MatchLdarg(0),
                        instr => instr.MatchCallOrCallvirt<Entity>("get_Scene"),
                        instr => instr.MatchCallOrCallvirt<Scene>("get_Tracker"));

        ILLabel label_nocontroller = cursor.DefineLabel();
        ILLabel label_didntpickup = cursor.DefineLabel();

        // + if (HoldablePriorityController.AppliesTo(this))
        cursor.EmitLdarg(0);
        cursor.Emit(OpCodes.Call, typeof(HoldablePriorityController).GetMethod(nameof(AppliesTo), BindingFlags.NonPublic | BindingFlags.Static)!);
        cursor.Emit(OpCodes.Brfalse, label_nocontroller);
        // + {

        // +   if (HoldablePriorityController.TryPickupAny(this))
        cursor.EmitLdarg(0);
        cursor.Emit(OpCodes.Call, typeof(HoldablePriorityController).GetMethod(nameof(TryPickupAny), BindingFlags.NonPublic | BindingFlags.Static)!);
        cursor.Emit(OpCodes.Brfalse, label_didntpickup);
        // +   {

        // +     return 8;
        cursor.EmitLdcI4(8);
        cursor.Emit(OpCodes.Ret);

        // +   }
        // + } else {
        cursor.MarkLabel(label_nocontroller);

        // foreach (var h in Scene.Tracker.GetComponents<Holdable>()) { ... } |
        cursor.GotoNext(MoveType.After, instr => instr.MatchEndfinally());
        cursor.GotoNext(MoveType.Before, instr => instr.MatchLdarg0());

        // + }
        cursor.MarkLabel(label_didntpickup);

        return;
    }

    #endregion Hook

}