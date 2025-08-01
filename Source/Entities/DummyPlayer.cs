
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Entities;

[Tracked]
public class DummyPlayer : Player
{

    public DummyPlayer(Vector2 position, PlayerSpriteMode spriteMode) : base(position, spriteMode)
    {
        StateMachine.State = StDummy;
        StateMachine.Locked = true;
    }

}
