using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using Monocle;

using Celeste.Mod.Microlith57Misc.Components;
using static Celeste.Mod.Microlith57Misc.Utils;

namespace Celeste.Mod.Microlith57Misc.Entities;

#region --- Float ---

[CustomEntity(
    "Microlith57Misc/ColorUnpacker_Float=Create",
    "Microlith57Misc/ColorUnpacker_Float_Expression=CreateExpr"
)]
public sealed class ColorUnpackerFloat : Entity {

    #region --- State ---

    private readonly ConditionSource EnabledCondition;
    public bool Enabled => EnabledCondition.Value;

    private readonly IntSource ColorSource;
    public Color Color => new() { PackedValue = unchecked((uint)ColorSource.Value) };

    private Session.Slider SliderR, SliderG, SliderB, SliderA;

    #endregion State
    #region --- Init ---

    public ColorUnpackerFloat(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        IntSource colorSource,
        Session.Slider sliderR,
        Session.Slider sliderG,
        Session.Slider sliderB,
        Session.Slider sliderA
    ) : base(data.Position + offset) {

        Add(EnabledCondition = enabledCondition);
        Add(ColorSource = colorSource);
        SliderR = sliderR;
        SliderG = sliderG;
        SliderB = sliderB;
        SliderA = sliderA;
    }

    private ColorUnpackerFloat(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        IntSource colorSource,
        Session session, string prefix
    ) : this(
        data, offset,
        enabledCondition,
        colorSource,
        session.GetSliderObject(prefix + "R"),
        session.GetSliderObject(prefix + "G"),
        session.GetSliderObject(prefix + "B"),
        session.GetSliderObject(prefix + "A")
    ) {}

    public static ColorUnpackerFloat Create(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Flag(data) { Default = true },
            new IntSource.Counter(level.Session, data, "packedColor", ifAbsent: "color"),
            level.Session, data.Attr("unpackedColorPrefix", defaultValue: "unpackedColor")
        );

    public static ColorUnpackerFloat CreateExpr(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Expr(data) { Default = true },
            new IntSource.Expr(data, "packedColor", ifAbsent: "#color"),
            level.Session, data.Attr("unpackedColorPrefix", defaultValue: "unpackedColor")
        );

    #endregion Init

    public override void Update() {
        base.Update();

        if (!Enabled) return;

        var col = Color;
        SliderR.Value = col.R / 255f;
        SliderG.Value = col.G / 255f;
        SliderB.Value = col.B / 255f;
        SliderA.Value = col.A / 255f;
    }

}

#endregion Float
#region --- Int ---

[CustomEntity(
    "Microlith57Misc/ColorUnpacker_Int=Create",
    "Microlith57Misc/ColorUnpacker_Int_Expression=CreateExpr"
)]
public sealed class ColorUnpackerInt : Entity {

    #region --- State ---

    private readonly ConditionSource EnabledCondition;
    public bool Enabled => EnabledCondition.Value;

    private readonly IntSource ColorSource;
    public Color Color => new() { PackedValue = unchecked((uint)ColorSource.Value) };

    private string CounterR, CounterG, CounterB, CounterA;

    #endregion State
    #region --- Init ---

    public ColorUnpackerInt(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        IntSource colorSource,
        string counterR,
        string counterG,
        string counterB,
        string counterA
    ) : base(data.Position + offset) {

        Add(EnabledCondition = enabledCondition);
        Add(ColorSource = colorSource);
        CounterR = counterR;
        CounterG = counterG;
        CounterB = counterB;
        CounterA = counterA;
    }

    private ColorUnpackerInt(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        IntSource colorSource, string prefix
    ) : this(
        data, offset,
        enabledCondition,
        colorSource,
        prefix + "R", prefix + "G", prefix + "B", prefix + "A"
    ) {}

    public static ColorUnpackerInt Create(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Flag(data) { Default = true },
            new IntSource.Counter(level.Session, data, "packedColor", ifAbsent: "color"),
            data.Attr("unpackedColorPrefix", defaultValue: "unpackedColor")
        );

    public static ColorUnpackerInt CreateExpr(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Expr(data) { Default = true },
            new IntSource.Expr(data, "packedColor", ifAbsent: "#color"),
            data.Attr("unpackedColorPrefix", defaultValue: "unpackedColor")
        );

    #endregion Init

    public override void Update() {
        base.Update();

        if (!Enabled || Scene is not Level level) return;

        var col = Color;
        level.Session.SetCounter(CounterR, col.R);
        level.Session.SetCounter(CounterG, col.G);
        level.Session.SetCounter(CounterB, col.B);
        level.Session.SetCounter(CounterA, col.A);
    }

}

#endregion Int
#region --- HSL ---

[CustomEntity(
    "Microlith57Misc/ColorUnpacker_HSL=Create",
    "Microlith57Misc/ColorUnpacker_HSL_Expression=CreateExpr"
)]
public sealed class ColorUnpackerHSL : Entity {

    #region --- State ---

    private readonly ConditionSource EnabledCondition;
    public bool Enabled => EnabledCondition.Value;

    private readonly IntSource ColorSource;
    public Color Color => new() { PackedValue = unchecked((uint)ColorSource.Value) };

    public readonly AngleFormat Format;

    private Session.Slider SliderH, SliderS, SliderL;

    #endregion State
    #region --- Init ---

    public ColorUnpackerHSL(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        IntSource colorSource,
        Session.Slider sliderH,
        Session.Slider sliderS,
        Session.Slider sliderL
    ) : base(data.Position + offset) {

        Format = data.Enum("format", AngleFormat.ZeroToOne);

        Add(EnabledCondition = enabledCondition);
        Add(ColorSource = colorSource);
        SliderH = sliderH;
        SliderS = sliderS;
        SliderL = sliderL;
    }

    private ColorUnpackerHSL(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        IntSource colorSource,
        Session session, string prefix
    ) : this(
        data, offset,
        enabledCondition,
        colorSource,
        session.GetSliderObject(prefix + "H"),
        session.GetSliderObject(prefix + "S"),
        session.GetSliderObject(prefix + "L")
    ) {}

    public static ColorUnpackerHSL Create(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Flag(data) { Default = true },
            new IntSource.Counter(level.Session, data, "packedColor", ifAbsent: "color"),
            level.Session, data.Attr("unpackedColorPrefix", defaultValue: "unpackedColor")
        );

    public static ColorUnpackerHSL CreateExpr(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Expr(data) { Default = true },
            new IntSource.Expr(data, "packedColor", ifAbsent: "#color"),
            level.Session, data.Attr("unpackedColorPrefix", defaultValue: "unpackedColor")
        );

    #endregion Init

    public override void Update() {
        base.Update();

        if (!Enabled) return;

        (SliderH.Value, SliderS.Value, SliderL.Value) = Color.ToHSL(Format);
    }

}

#endregion Int