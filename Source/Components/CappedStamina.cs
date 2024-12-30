using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;

namespace Celeste.Mod.Microlith57Misc.Components;

public class CappedStamina() : Component(active: false, visible: false) {

    #region --- State ---

    public Player Player => (Player)Entity;

    public float UpperCap, CurrentCap;
    public bool Recovering = false;

    public event Action OnRestore = () => {};

    #endregion State
    #region --- Behaviour ---

    public override void Added(Entity entity) {
        base.Added(entity);

        if (entity is not Player player)
            throw new Exception("cannot add stamina cap to a non-player");

        float toRestore = player.Stamina;
        player.RefillStamina();
        UpperCap = CurrentCap = player.Stamina;
        player.Stamina = toRestore;
        Active = true;
    }

    public void Enforce() {
        var player = (Player)Entity;
        if (player.Stamina > CurrentCap)
            player.Stamina = CurrentCap;
    }

    #endregion Behaviour
    #region --- Hook ---

    private static ILHook? hookPlayerUpdate;

    internal static void Load() {
        hookPlayerUpdate = new(
            typeof(Player).GetMethod("orig_Update")!,
            HookPlayerMethods
        );

        IL.Celeste.Player.RefillStamina += HookPlayerMethods;
        IL.Celeste.Player.ClimbUpdate += HookPlayerMethods;
        IL.Celeste.Player.SwimBegin += HookPlayerMethods;
        IL.Celeste.Player.DreamDashBegin += HookPlayerMethods;
        // IL.Celeste.SummitGem.SmashRoutine += HookPlayerMethods;
    }

    internal static void Unload() {
        hookPlayerUpdate?.Dispose();
        hookPlayerUpdate = null;

        IL.Celeste.Player.RefillStamina -= HookPlayerMethods;
        IL.Celeste.Player.ClimbUpdate -= HookPlayerMethods;
        IL.Celeste.Player.SwimBegin -= HookPlayerMethods;
        IL.Celeste.Player.DreamDashBegin -= HookPlayerMethods;
    }

    private static void HookPlayerMethods(ILContext il) {
        ILCursor cursor = new(il);

        bool matched = false;
        while (cursor.TryGotoNext(
            MoveType.After,
            i => i.MatchLdarg0(),
            i => i.MatchLdcR4(110f)
        )) {
            matched = true;

            cursor.EmitLdarg0();
            cursor.EmitDelegate(ModMaxStamina);
        }

        if (!matched)
            throw new Exception($"failed to match stamina cap instruction in {il.Method.Name}");
    }

    private static float ModMaxStamina(float orig, Player self) {
        if (self.Get<CappedStamina>() is CappedStamina cap && cap.Active) {
            cap.OnRestore.Invoke();
            return Math.Min(cap.CurrentCap, orig);
        }

        return orig;
    }

    #endregion

}
