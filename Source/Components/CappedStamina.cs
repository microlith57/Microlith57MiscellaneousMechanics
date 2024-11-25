using Monocle;
using System;

namespace Celeste.Mod.Microlith57Misc.Components;

public class CappedStamina() : Component(active: false, visible: false) {

    #region --- State ---

    public float UpperCap, CurrentCap;
    public bool Recovering = false;

    #endregion State
    #region --- Init ---

    public override void Added(Entity entity) {
        base.Added(entity);

        if (entity is not Player player)
            throw new Exception("cannot add stamina cap to a non-player");

        float toRestore = player.Stamina;
        player.RefillStamina();
        UpperCap = CurrentCap = player.Stamina;
        player.Stamina = toRestore;
    }

    #endregion Init

}
