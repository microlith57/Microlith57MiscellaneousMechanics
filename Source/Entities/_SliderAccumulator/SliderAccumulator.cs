using System.Runtime.CompilerServices;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity("Microlith57Misc/SliderAccumulator=Create")]
[Tracked]
public sealed class SliderAccumulator : Entity {

    public enum Operation {
        Sum,
        AbsSum,
        Product,
        AbsProduct,
        Average,
    }

    public enum Aggregation {
        PerSet,
        PerFrame,
    }

    private static ConditionalWeakTable<Session.Slider, List<SliderAccumulator>> Accumulators = [];
    private static bool Recursing = false;

    private readonly Operation Op;
    private readonly Aggregation Ag;
    private readonly bool Relative;

    private readonly Session.Slider Input, Output;

    private readonly ConditionSource ResetSource;
    private bool ShouldReset => ResetSource.Value;
    private readonly float? ResetInput, ResetOutput;

    private float? InitialThisFrame;
    private float Delta = 0f;
    private float AvgSum = 0f;
    private int AvgCount = 0;

    public SliderAccumulator(
        EntityData data, Vector2 offset,
        Session.Slider input, Session.Slider output
    ) : base(data.Position + offset) {
        Input = input;
        Output = output;

        Op = data.Enum("operation", Operation.Sum);
        Ag = data.Enum("aggregation", Aggregation.PerFrame);

        if (!string.IsNullOrWhiteSpace(data.Attr("resetInput", "")))
            ResetInput = data.Float("resetInput");
        if (!string.IsNullOrWhiteSpace(data.Attr("resetOutput", "")))
            ResetOutput = data.Float("resetOutput");

        Relative = data.Bool("relative", true);
    }

    public static SliderAccumulator Create(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            level.Session.GetSliderObject(data.Attr("input", "input")),
            level.Session.GetSliderObject(data.Attr("output", "output"))
        );

    public override void Added(Scene scene) {
        base.Added(scene);
        Accumulators.GetOrCreateValue(Input).Add(this);
    }

    public override void Removed(Scene scene) {
        base.Removed(scene);
        if (!Accumulators.TryGetValue(Input, out var accs)) return;

        accs.Remove(this);
        if (accs.Count == 0)
            Accumulators.Remove(Input);
    }

    private void Reset() {
        if (ResetInput.HasValue)
            Input.Value = ResetInput.Value;
        if (ResetOutput.HasValue)
            Output.Value = ResetOutput.Value;
        InitialThisFrame = Input.Value;
        AvgSum = AvgCount = 0;
    }

    private void Add(float delta) {
        switch (Op) {
            case Operation.Sum:
                Output.Value += delta;
                break;
            case Operation.AbsSum:
                Output.Value += Math.Abs(delta);
                break;
            case Operation.Product:
                Output.Value *= delta;
                break;
            case Operation.AbsProduct:
                Output.Value *= Math.Abs(delta);
                break;
            case Operation.Average:
                AvgSum += delta;
                AvgCount += 1;
                Output.Value = AvgSum / AvgCount;
                break;
        }
    }

    private static void OnBeforeUpdate(Level level) {
        Recursing = true;
        try {
            foreach (SliderAccumulator acc in level.Tracker.GetEntities<SliderAccumulator>())
                acc.Reset();
        } finally {
            Recursing = false;
        }
    }

    private static void OnSliderChanged(Session session, Session.Slider slider, float? prev) {
        if (Recursing || !Accumulators.TryGetValue(slider, out var accs)) return;

        var abs = slider.Value;
        var rel = abs - (prev ?? 0f);
        Recursing = true;
        try {
            foreach (var acc in accs)
                if (acc.Active)
                    acc.Add(acc.Relative ? rel : abs);
        } finally {
            Recursing = false;
        }
    }

    [OnLoad]
    internal static void Load() {
        Everest.Events.Level.OnBeforeUpdate += OnBeforeUpdate;
        Everest.Events.Session.OnSliderChanged += OnSliderChanged;
    }

    [OnUnload]
    internal static void Unload() {
        Everest.Events.Level.OnBeforeUpdate -= OnBeforeUpdate;
        Everest.Events.Session.OnSliderChanged -= OnSliderChanged;
    }

}
