using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;

using Celeste.Mod.Microlith57Misc.Components;
using static Celeste.Mod.Microlith57Misc.Components.ConditionSource;
using System.Linq;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity(
    "Microlith57Misc/FocusController=Create",
    "Microlith57Misc/FocusController_Button=CreateButton",
    "Microlith57Misc/FocusController_Expression=CreateExprs"
)]
public sealed class FocusController : Entity {

    // todo: sound, consumption

    #region --- State ---

    private readonly ConditionSource EnabledCondition;
    public bool CanFocus => EnabledCondition.Value;

    private readonly ConditionSource TargetCondition;
    public bool TryingToFocus => CanFocus && TargetCondition.Value;

    private string? consumptionUnbound;

    public ConsumableResource? Consumption { get; private set; }
    public readonly float ConsumptionMultiplier;
    private ConsumableResource.Drain Drain;

    public readonly float FadeDuration;
    public readonly Session.Slider Slider;

    public bool Slowing { get; private set; }

    #endregion State
    #region --- Init ---

    public FocusController(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        ConditionSource targetCondition,
        Session.Slider slider
    ) : base(data.Position + offset) {

        Add(EnabledCondition = enabledCondition);
        Add(TargetCondition = targetCondition);

        Slider = slider;

        consumptionUnbound = data.Attr("consumptionResourceName");
        ConsumptionMultiplier = data.Float("consumptionMultiplier", 1f);

        FadeDuration = data.Float("fadeDuration", 0f);
    }

    public static FocusController Create(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new Flag(data, name: "enabledFlag", invertName: "invertEnabledFlag") { Default = true },
            new Flag(data, name: "activeFlag", ifAbsent: "timeSlowActive", invertName: "invertActiveFlag"),
            level.Session.GetSliderObject(data.Attr("slider", "focus"))
        );

    public static FocusController CreateButton(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new Flag(data, name: "enabledFlag", invertName: "invertEnabledFlag") { Default = true },
            new Function(() => Input.Grab),
            level.Session.GetSliderObject(data.Attr("slider", "focus"))
        );

    public static FocusController CreateExprs(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new Expr(data, name: "enabledExpression") { Default = true },
            new Expr(data, name: "activeExpression", ifAbsent: "$input.talk"),
            level.Session.GetSliderObject(data.Attr("slider", "focus"))
        );

    #endregion Init
    #region --- Behaviour ---

    public override void Awake(Scene scene) {
        base.Awake(scene);

        if (consumptionUnbound != null) {
            Consumption = (ConsumableResource?)Scene.Tracker
                .GetEntities<ConsumableResource>()
                .FirstOrDefault(c => c is ConsumableResource r && r.Name == consumptionUnbound);
            consumptionUnbound = null;
        }
    }

    public override void Update() {
        base.Update();

        bool shouldSlow = TryingToFocus;

        if (Consumption != null) {
            if (!Consumption.CanConsume)
                shouldSlow = false;
        }

        float lerpTarget = shouldSlow ? 1f : 0f;
        if (FadeDuration == 0f)
            Slider.Value = lerpTarget;
        else
            Slider.Value = Calc.Approach(Slider.Value, lerpTarget, Engine.RawDeltaTime / FadeDuration);
    }

    #endregion Behaviour

}