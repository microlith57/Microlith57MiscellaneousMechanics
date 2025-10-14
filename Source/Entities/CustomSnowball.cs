
using System.Linq;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Entities;

[Tracked]
public class CustomSnowball : Snowball
{

    public string Name;
    public bool Once;

    public float? LockY;

    public float SafeArea = 64f;

    public float SineFrequency = 0.5f;
    public float SineOffset = 0f;

    private bool justCreated = true;

    public CustomSnowball(
        string name,
        bool once = false,
        float? lockY = null,
        float safeArea = 64f,
        float sineFrequency = 0.5f,
        float sineOffset = 0f
    ) : base() {
        Name = name;
        Once = once;
        LockY = lockY;
        SafeArea = safeArea;

        sine.Frequency = sineFrequency;
        sine.Counter = sineOffset;
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        justCreated = false;
    }

    private void override_ResetPosition() {
        if (Scene is not Level level || level.Tracker.GetEntity<Player>() is not Player player) return;

        if (justCreated && Once) {
            RemoveSelf();
            return;
        }

        if (player.Right >= level.Bounds.Right - SafeArea) {
            resetTimer = 0.05f;
            return;
        }

        spawnSfx.Play("event:/game/04_cliffside/snowball_spawn");
        Collidable = Visible = true;
        resetTimer = 0f;
        X = level.Camera.Right + 10f;

        if (LockY.HasValue) {
            atY = Y = LockY.Value;
        } else {
            atY = Y = player.CenterY;
        }

        sine.Reset();
        sprite.Play("spin");
    }

    private static void onResetPosition(On.Celeste.Snowball.orig_ResetPosition orig, Snowball self) {
        if (self is CustomSnowball snowball)
            snowball.override_ResetPosition();
        else
            orig(self);
    }

    internal static void Load()
        => On.Celeste.Snowball.ResetPosition += onResetPosition;

    internal static void Unload()
        => On.Celeste.Snowball.ResetPosition -= onResetPosition;

    public static CustomSnowball? FindInstanceByName(Scene scene, string name) {
        foreach (CustomSnowball snowball in scene.Tracker.GetEntities<CustomSnowball>())
            if (snowball.Name == name)
                return snowball;

        return null;
    }

}

[CustomEntity("Microlith57Misc/CustomSnowballTrigger")]
public class CustomSnowballTrigger : Trigger
{

    public string Name;
    public bool OncePerTrigger;
    public bool RemoveAfterTriggered;

    public float? LockY;
    public float SafeArea, SineFrequency, SineOffset;

    public CustomSnowballTrigger(EntityData data, Vector2 offset) : base(data, offset) {
        Name = data.Attr("name");
        OncePerTrigger = data.Bool("oncePerTrigger");
        RemoveAfterTriggered = data.Bool("removeAfterTriggered");

        LockY = null;
        if (data.Nodes.Length > 0)
            LockY = data.NodesOffset(offset).First().Y;

        SafeArea = data.Float("safeArea", 64f);
        SineFrequency = data.Float("sineFrequency", 0.5f);
        SineOffset = data.Float("sineOffset", 0f);
    }

    public override void OnEnter(Player player) {
        base.OnEnter(player);

        var instance = CustomSnowball.FindInstanceByName(Scene, Name);
        if (instance is not null)
            return;

        instance = new(Name, OncePerTrigger, LockY, SafeArea, SineFrequency, SineOffset);
        Scene.Add(instance);

        if (RemoveAfterTriggered) RemoveSelf();
    }
}
