using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.EeveeHelper.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc;

public static partial class Utils {

    private static bool checkedEeveeHelper;
    internal static bool EeveeHelperLoaded {
        get {
            if (checkedEeveeHelper || Everest.Loader.DependencyLoaded(new EverestModuleMetadata { Name = "EeveeHelper", Version = new Version(1, 0, 0) })) {
                checkedEeveeHelper = true;
                return true;
            }
            return false;
        }
    }
    internal static IEnumerable<(Entity container, Holdable hold, Action<Vector2> speedSetter)> GetEeveeHelperHoldableContainers(this Tracker tracker) => EeveeHelperLoaded ? EeveeHelperContainmentChamber.getHoldableContainers(tracker) : [];
    // public static bool IsHoldableContainer(this Holdable hold) => EeveeHelperLoaded && EeveeHelperContainmentChamber.isHoldableContainer(hold);

}

internal static class EeveeHelperContainmentChamber {

    internal static IEnumerable<(Entity container, Holdable hold, Action<Vector2> speedSetter)> getHoldableContainers(Tracker tracker)
        => (
            from e in tracker.GetEntities<HoldableContainer>()
            let c = e as HoldableContainer
            where c != null
            select ((Entity)c, c.Hold, (Action<Vector2>)((speed) => c.Speed = speed))
        );

}
