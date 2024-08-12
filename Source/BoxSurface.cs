using System;
using System.Collections.Generic;
using Celeste.Mod.GravityHelper.Components;
using Celeste.Mod.GravityHelper.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57.IntContest;

[Tracked]
public class BoxSurface : Component {

    public class BelongsToBox(BoxSurface surface, bool isTop) : Component(false, false) {
        public BoxSurface Surface => surface;
        public bool IsTop => isTop;
        public bool IsBot => !isTop;
    }

    public Collider Collider;

    public JumpThru SurfaceTop;
    public UpsideDownJumpThru SurfaceBot;

    public bool CollidableTop {
        get => SurfaceTop.Collidable;
        set => SurfaceTop.Collidable = value;
    }

    public bool CollidableBot {
        get => SurfaceBot.Collidable;
        set => SurfaceBot.Collidable = value;
    }

    public bool Collidable {
        get => SurfaceTop.Collidable || SurfaceBot.Collidable;
        set => SurfaceTop.Collidable = SurfaceBot.Collidable = value;
    }

    public bool Persistent {
        set {
            if (value) {
                SurfaceTop.AddTag(Tags.Persistent);
                SurfaceBot.AddTag(Tags.Persistent);
            } else {
                SurfaceTop.RemoveTag(Tags.Persistent);
                SurfaceBot.RemoveTag(Tags.Persistent);
            }
        }
    }

    public BoxSurface(Collider collider, int width, int depth, int surfaceIndex) : base(true, false) {
        Collider = collider;

        SurfaceTop = makeTopSurface(collider.AbsolutePosition, width, depth, surfaceIndex);
        SurfaceTop.Add(new BelongsToBox(this, true));

        SurfaceBot = makeBottomSurface(collider.AbsolutePosition, width, depth, surfaceIndex);
        SurfaceBot.Add(new BelongsToBox(this, false));
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

        Collidable = true;
    }

    public override void EntityRemoved(Scene scene) {
        base.EntityRemoved(scene);

        scene.Remove(SurfaceTop);
        scene.Remove(SurfaceBot);
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