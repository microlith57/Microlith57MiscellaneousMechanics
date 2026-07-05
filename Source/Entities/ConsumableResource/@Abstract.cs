namespace Celeste.Mod.Microlith57Misc.Components;

[Tracked(inherited: true)]
public abstract class ConsumableResource : Entity {
    public readonly string Name;

    public readonly bool UseRawDeltaTime;
    public float DeltaTime => UseRawDeltaTime ? Engine.RawDeltaTime : Engine.DeltaTime;

    // public readonly bool FlashPlayer;
    public bool DieWhenConsumed { get; private set; }

    public readonly Session.Slider Slider;
    private float prevSliderValue;

    public readonly (
        string Any,
        string Full,
        string Low,
        string Flash
    )? FlagNames;

    public readonly float RestoreCooldown;
    public readonly float RestoreSpeed;
    private float cooldown;

    public virtual void BumpRestoreCooldown() => cooldown = RestoreCooldown;

    public readonly float FlashRate;

    private HashSet<Drain> Drains = [];

    public abstract float Low { get; }
    public abstract float Maximum { get; }
    public abstract float Current { get; set; }

    public virtual bool CanConsume => Current > 0f;
    protected virtual bool ShouldFlash => Current <= Low;
    public virtual bool Flashing
        => ShouldFlash
        && Scene is Level level
        && level.BetweenInterval(FlashRate);

    public virtual bool CanStartRestoring => true;
    public bool CanRestore => !Drains.Any(d => d.Active) && cooldown <= 0f;

    protected ConsumableResource(
        EntityData data, Vector2 offset,
        Session.Slider slider,
        ConditionSource instantRefillCondition,
        ConditionSource instantDrainCondition
    ) : base(data.Position + offset) {

        Name = data.Attr("resource", "resourceName");

        var prefix = data.Attr("flagPrefix");
        if (prefix != "")
            FlagNames = (
                prefix + "Any",
                prefix + "Full",
                prefix + "Low",
                prefix + "Flash"
            );

        Slider = slider;

        UseRawDeltaTime = data.Bool("useRawDeltaTime");
        DieWhenConsumed = data.Bool("dieWhenConsumed");

        RestoreCooldown = data.Float("restoreCooldown", 0.1f);
        RestoreSpeed = data.Float("restoreSpeed", 120f);

        FlashRate = data.Float("flashRate", 0.05f);

        PreUpdate += BeforeUpdate;
        PostUpdate += AfterUpdate;
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        Slider.Value = prevSliderValue = Current;
    }

    private void BeforeUpdate(Entity _) {
        if (prevSliderValue != Slider.Value) {
            if (Slider.Value < Current) BumpRestoreCooldown();
            Current = Slider.Value;
        }

        foreach (var drain in Drains) {
            if (!drain.Active) continue;

            BumpRestoreCooldown();
            if (CanConsume)
                Current = Calc.Approach(Current, 0f, drain.ConsumptionThisFrame);
            else break;
        }
    }

    private void AfterUpdate(Entity _) {
        if (Scene is not Level level) return;

        if (cooldown > 0) {
            if (CanStartRestoring)
                cooldown = Calc.Approach(cooldown, 0f, DeltaTime);
        } else if (Current < Maximum) {
            if (RestoreSpeed < 0)
                Current = Maximum;
            else
                Current = Calc.Approach(Current, Maximum, RestoreSpeed * DeltaTime);
        }

        var c = Current;

        if (DieWhenConsumed && c <= 0f) {
            level.Tracker.GetEntity<Player>()?.Die(Vector2.Zero);
            DieWhenConsumed = false;
        }

        if (FlagNames.HasValue) {
            level.Session.SetFlag(FlagNames.Value.Any, c > 0f);
            level.Session.SetFlag(FlagNames.Value.Full, c >= Maximum);
            level.Session.SetFlag(FlagNames.Value.Low, c <= Low);
            level.Session.SetFlag(FlagNames.Value.Flash, Flashing);
        }

        Slider.Value = prevSliderValue = c;
    }
}
