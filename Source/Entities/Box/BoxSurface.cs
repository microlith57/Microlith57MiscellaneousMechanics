using Celeste.Mod.GravityHelper.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57.IntContest.Entities;

[Tracked]
public class BoxSurface : Component {

    [Tracked]
    public class BelongsToBox(BoxSurface surface, bool isTop) : Component(false, false) {
        public BoxSurface Surface => surface;
        public bool IsTop => isTop;
        public bool IsBot => !isTop;
    }

    public Collider Collider;

    public JumpThru SurfaceTop;
    public UpsideDownJumpThru SurfaceBot;

    private bool collidableTop = true;
    public bool CollidableTop {
        get => collidableTop;
        set { collidableTop = value; updateCollision(); }
    }

    private bool collidableBot = true;
    public bool CollidableBot {
        get => collidableBot;
        set { collidableBot = value; updateCollision(); }
    }

    private bool collidable = true;
    public bool Collidable {
        get => collidable;
        set { collidable = value; updateCollision(); }
    }

    public BoxSurface(Collider collider, int width, int depth, int surfaceIndex) : base(true, false) {
        Collider = collider;

        SurfaceTop = makeTopSurface(collider.AbsolutePosition, width, depth, surfaceIndex);
        SurfaceTop.Add(new BelongsToBox(this, true));
        SurfaceTop.AddTag(Tags.Persistent);

        SurfaceBot = makeBottomSurface(collider.AbsolutePosition, width, depth, surfaceIndex);
        SurfaceBot.Add(new BelongsToBox(this, false));
        SurfaceBot.AddTag(Tags.Persistent);
    }

    public override void EntityAdded(Scene scene) {
        base.EntityAdded(scene);

        SurfaceTop.Position = Collider.AbsolutePosition;
        SurfaceTop.Collider.Width = Collider.Width;

        SurfaceBot.Position = Collider.AbsolutePosition
                               + new Vector2(0f, Collider.Height - SurfaceBot.Collider.Height - 3f);
        SurfaceBot.Collider.Width = Collider.Width;

        scene.Add(SurfaceTop);
        scene.Add(SurfaceBot);
    }

    public override void EntityRemoved(Scene scene) {
        base.EntityRemoved(scene);

        scene.Remove(SurfaceTop);
        scene.Remove(SurfaceBot);
    }

    private void updateCollision() {
        SurfaceTop.Collidable = collidable && collidableTop;
        SurfaceBot.Collidable = collidable && collidableBot;
    }

    public void Move() {
        SurfaceTop.MoveTo(Collider.AbsolutePosition);
        SurfaceBot.MoveTo(Collider.AbsolutePosition
                          + new Vector2(0f, Collider.Height - SurfaceBot.Collider.Height - 3f));
    }

    private static JumpThru makeTopSurface(Vector2 position, int width, int depth, int surfaceIndex) {
        return new(position, width, safe: false) {
            Depth = depth,
            SurfaceSoundIndex = surfaceIndex,
            Collidable = false
        };
    }

    private static UpsideDownJumpThru makeBottomSurface(Vector2 position, int width, int depth, int surfaceIndex) {
        var bottomSurfaceData = new EntityData() {
            Name = "GravityHelper/UpsideDownJumpThru",
            Position = position,
            Width = width,
            Values = []
        };
        bottomSurfaceData.Values.Add("modVersion", "1.2.18");
        bottomSurfaceData.Values.Add("pluginVersion", "1");
        bottomSurfaceData.Values.Add("surfaceIndex", surfaceIndex);

        return new(bottomSurfaceData, Vector2.Zero) {
            Depth = depth,
            Visible = false,
            Collidable = false
        };
    }

}