using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using System;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity("Microlith57Misc/PlayerStateNameToCounterController")]
public sealed class PlayerStateNameToCounterController : Entity {

    public readonly string StateName;
    public readonly string Counter;

    public readonly string Flag;
    public readonly bool InvertFlag;

    private Player? Player => Scene.Tracker.GetEntity<Player>();

    private int StateIndex;

    public PlayerStateNameToCounterController(
        EntityData data, Vector2 offset
    ) : base(data.Position + offset) {
        this.SetDepthAndTags(data);
        StateName = Format(data.Attr("stateName", "StNormal").Trim());
        Counter = data.Attr("counter", "stNormal");
        Flag = data.Attr("inStateFlag", "stNormal");
        InvertFlag = data.Bool("invertFlag");
    }

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
