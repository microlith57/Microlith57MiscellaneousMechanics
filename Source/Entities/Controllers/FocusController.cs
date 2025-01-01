using System.Linq;
using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;

using Celeste.Mod.Microlith57Misc.Components;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity(
    "Microlith57Misc/FocusController=Create",
    "Microlith57Misc/FocusController_Button=CreateButton",
    "Microlith57Misc/FocusController_Expression=CreateExpr"
)]
public sealed class FocusController : Entity {

    #region --- State ---

    private readonly ConditionSource EnabledCondition;
    public bool CanFocus => EnabledCondition.Value;

    private readonly ConditionSource TargetCondition;
    public bool TryingToFocus => CanFocus && TargetCondition.Value;

    public readonly bool UseRawDeltaTime;
    public float DeltaTime => UseRawDeltaTime ? Engine.RawDeltaTime : Engine.DeltaTime;

    private string? consumptionUnbound;

    public ConsumableResource? Consumption { get; private set; }
    public readonly float ConsumptionRate;
    public readonly bool UnfocusWhenResourceLow = true;
    private ConsumableResource.Drain? Drain;

    public readonly float FadeDuration;
    public readonly Session.Slider Slider;

    public readonly (
        string Trying,
        string Focusing,
        string AnyFocus,
        string FullFocus
    )? FlagNames;

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

        UseRawDeltaTime = data.Bool("useRawDeltaTime");

        Slider = slider;

        var prefix = data.Attr("flagPrefix");
        if (prefix != "")
            FlagNames = (
                prefix + "Trying",
                prefix + "Focusing",
                prefix + "AnyFocus",
                prefix + "FullFocus"
            );

        consumptionUnbound = data.Attr("consumptionResourceName");
        ConsumptionRate = data.Float("consumptionRate", 12f);
        UnfocusWhenResourceLow = data.Bool("unfocusWhenResourceLow", true);

        FadeDuration = data.Float("fadeDuration", 1f);
    }

    public static FocusController Create(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Flag(data, name: "enabledFlag", invertName: "invertEnabledFlag") { Default = true },
            new ConditionSource.Flag(data, name: "activeFlag", ifAbsent: "tryingToFocus", invertName: "invertActiveFlag"),
            level.Session.GetSliderObject(data.Attr("slider", "focus"))
        );

    public static FocusController CreateButton(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Flag(data, name: "enabledFlag", invertName: "invertEnabledFlag") { Default = true },
            new ConditionSource.Function(() => Module.Settings.Focus),
            level.Session.GetSliderObject(data.Attr("slider", "focus"))
        );

    public static FocusController CreateExpr(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Expr(data, name: "enabledExpression") { Default = true },
            new ConditionSource.Expr(data, name: "activeExpression", ifAbsent: "$input.grab"),
            level.Session.GetSliderObject(data.Attr("slider", "focus"))
        );

    #endregion Init
    #region --- Behaviour ---

    public override void Awake(Scene scene) {
        base.Awake(scene);

        if (consumptionUnbound != "") {
            Consumption = (ConsumableResource?)Scene.Tracker
                .GetEntities<ConsumableResource>()
                .FirstOrDefault(c => c is ConsumableResource r && r.Name == consumptionUnbound);

            if (Consumption != null)
                Add(Drain = new(
                        Consumption,
                        ConsumptionRate,
                        UseRawDeltaTime,
                        stacks: true
                    ) { Active = false });

            consumptionUnbound = null;
        }
    }

    public override void Update() {
        base.Update();

        if (Scene is not Level level) return;

        bool shouldSlow = TryingToFocus;

        if (FlagNames != null)
            level.Session.SetFlag(FlagNames.Value.Trying, shouldSlow);

        if (Consumption != null) {
            Drain!.Active = shouldSlow;

            if (!Consumption.CanConsume)
                shouldSlow = false;
        }

        if (FlagNames != null)
            level.Session.SetFlag(FlagNames.Value.Focusing, shouldSlow);

        float lerpTarget = shouldSlow ? 1f : 0f;

        if (Consumption != null && UnfocusWhenResourceLow)
            lerpTarget *= Calc.ClampedMap(Consumption.Current, 0f, Consumption.Low);

        if (FadeDuration == 0f)
            Slider.Value = lerpTarget;
        else
            Slider.Value = Calc.Approach(Slider.Value, lerpTarget, DeltaTime / FadeDuration);

        if (FlagNames != null) {
            level.Session.SetFlag(FlagNames.Value.AnyFocus, Slider.Value > 0f);
            level.Session.SetFlag(FlagNames.Value.FullFocus, Slider.Value == 1f);
        }
    }

    #endregion Behaviour

}