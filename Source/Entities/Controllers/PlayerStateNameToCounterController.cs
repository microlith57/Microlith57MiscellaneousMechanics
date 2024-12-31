using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using System;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity("Microlith57Misc/PlayerStateNameToCounterController")]
public sealed class PlayerStateNameToCounterController(EntityData data, Vector2 offset) : Entity(data.Position + offset) {

    public readonly string StateName = Format(data.Attr("stateName", "StNormal").Trim());
    public readonly string Counter = data.Attr("counter", "stNormal");

    public readonly string Flag = data.Attr("inStateFlag", "stNormal");
    public readonly bool InvertFlag = data.Bool("invertFlag");

    private Player? Player => Scene.Tracker.GetEntity<Player>();

    private int StateIndex;
    private bool InState => Player?.StateMachine.State == StateIndex;

    public override void Awake(Scene scene) {
        base.Awake(scene);

        if (Player is not Player player) goto notFound;

        try {
            for (int i = 0; ; i++)
                if (Format(player.StateMachine.GetStateName(i)).IsIgnoreCase(StateName)) {
                    (Scene as Level)!.Session.SetCounter(Counter, StateIndex = i);

                    if (Flag == "") RemoveSelf(); // our work here is done
                    return;
                }
        } catch (IndexOutOfRangeException) { }

    notFound:
        (Scene as Level)!.Session.SetCounter(Counter, -1);
        RemoveSelf();
    }

    public override void Update() {
        base.Update();

        (Scene as Level)!.Session.SetFlag(Flag, InState ^ InvertFlag);
    }

    private static string Format(string name) {
        if (name.StartsWith("St", StringComparison.InvariantCultureIgnoreCase))
            name = name[2..];

        return name;
    }

}
