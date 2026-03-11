using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Entities;

[Tracked]
public class BoxSurface : Component {

    [Tracked]
    public class BelongsToBox(BoxSurface surface, bool isTop) : Component(false, false) {
        public BoxSurface Surface => surface;
        public bool IsTop => isTop;
        public bool IsBot => !isTop;
    }

    public Collider Collider;
    public readonly int Width, Depth;
    public readonly int SurfaceIndex;

    public JumpThru? SurfaceTop, SurfaceBot;

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

    public BoxSurface(Collider collider, int width, int depth, int surfaceIndex)
        : base(true, false) {

        Collider = collider;
        Width = width;
        Depth = depth;
        SurfaceIndex = surfaceIndex;
    }

    public override void EntityAdded(Scene scene) {
        base.EntityAdded(scene);

        if (scene is not Level level) {
            RemoveSelf();
            return;
        }

        SurfaceTop = makeTopSurface(Collider.AbsolutePosition, Width, Depth, SurfaceIndex);
        SurfaceTop.Add(new BelongsToBox(this, true));
        SurfaceTop.AddTag(Tags.Persistent);

        SurfaceBot = makeBottomSurface(level, Collider.AbsolutePosition, Width, Depth, SurfaceIndex);
        SurfaceBot.Add(new BelongsToBox(this, false));
        SurfaceBot.AddTag(Tags.Persistent);

        SurfaceTop.Position = Collider.AbsolutePosition;
        SurfaceTop.Collider.Width = Collider.Width;

        SurfaceBot.Position = Collider.AbsolutePosition
                               + new Vector2(0f, Collider.Height - SurfaceBot.Collider.Height - 3f);
        SurfaceBot.Collider.Width = Collider.Width;

        scene.Add(SurfaceTop);
        scene.Add(SurfaceBot);
        updateCollision();
    }

    public override void EntityAwake() {
        base.EntityAwake();

        if (Scene is not Level level || level.Tracker.GetEntity<Player>() is not Player player || player.Get<PlayerUpdateHook>() is not null) return;
        player.Add(new PlayerUpdateHook());
    }

    public override void EntityRemoved(Scene scene) {
        base.EntityRemoved(scene);

        scene.Remove(SurfaceTop);
        scene.Remove(SurfaceBot);
    }

    private void updateCollision() {
        if (SurfaceTop is not null)
            SurfaceTop.Collidable = collidable && collidableTop;

        if (SurfaceBot is not null)
            SurfaceBot.Collidable = collidable && collidableBot;
    }

    public void Move() {
        if (SurfaceTop is not null)
            SurfaceTop.MoveTo(Collider.AbsolutePosition);

        if (SurfaceBot is not null)
            SurfaceBot.MoveTo(Collider.AbsolutePosition
                            + new Vector2(0f, Collider.Height - SurfaceBot.Collider.Height - 3f));
    }

    private static JumpThru makeTopSurface(Vector2 position, int width, int depth, int surfaceIndex) {
        return new(position, width, safe: false) {
            Depth = depth,
            SurfaceSoundIndex = surfaceIndex,
            Visible = false,
            Collidable = false
        };
    }

    private static JumpThru makeBottomSurface(Level level, Vector2 position, int width, int depth, int surfaceIndex) {
        var name = "GravityHelper/UpsideDownJumpThru";

        var bottomSurfaceData = new EntityData() {
            Name = name,
            Position = position,
            Width = width,
            Values = []
        };
        bottomSurfaceData.Values.Add("modVersion", "1.2.18");
        bottomSurfaceData.Values.Add("pluginVersion", "1");
        bottomSurfaceData.Values.Add("surfaceIndex", surfaceIndex);

        var upsideDownJumpthruLoader = Level.EntityLoaders[name];
        var bottomSurface = upsideDownJumpthruLoader(level, level.Session.LevelData, Vector2.Zero, bottomSurfaceData) as JumpThru
            ?? throw new Exception("failed to load GravityHelper/UpsideDownJumpThru entity; this is probably my fault for getting it with this cursed approach");

        bottomSurface.Depth = depth;
        bottomSurface.Visible = false;
        bottomSurface.Collidable = false;

        return bottomSurface;
    }

    public class PlayerUpdateHook() : Component(false, false) {

        public Player Player => Entity as Player ?? throw new Exception("the PlayerUpdateHook component should only be added to the Player!");

        private Dictionary<BoxSurface, (bool, bool, bool)> BoxesWithOrigCollidableStates = [];

        public override void Added(Entity entity) {
            base.Added(entity);
            Player.PreUpdate += (_) => BeforePlayerUpdate();
            Player.PostUpdate += (_) => AfterPlayerUpdate();
        }

        private void BeforePlayerUpdate() {
            BoxesWithOrigCollidableStates.Clear();

            bool invert = Player.ShouldInvert();

            foreach (BoxSurface boxSurface in Scene.Tracker.GetComponents<BoxSurface>()) {
                BoxesWithOrigCollidableStates.Add(boxSurface, (boxSurface.Collidable,
                                                               boxSurface.CollidableTop,
                                                               boxSurface.CollidableBot));

                if (Player.Holding?.Entity == boxSurface.Entity)
                    boxSurface.Collidable = false;
                else if (!invert)
                    boxSurface.CollidableBot = false;
                else
                    boxSurface.CollidableTop = false;
            }
        }

        private void AfterPlayerUpdate() {
            if (BoxesWithOrigCollidableStates is null) return;

            foreach ((var surface, (var wasCollidable, var wasCollidableTop, var wasCollidableBot)) in BoxesWithOrigCollidableStates) {
                surface.Collidable = wasCollidable;
                surface.CollidableTop = wasCollidableTop;
                surface.CollidableBot = wasCollidableBot;
            }

            BoxesWithOrigCollidableStates.Clear();
        }

    }

}
