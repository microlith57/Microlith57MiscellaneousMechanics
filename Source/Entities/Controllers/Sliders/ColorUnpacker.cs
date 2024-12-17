using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

using Celeste.Mod.Microlith57Misc.Components;
using Monocle;

namespace Celeste.Mod.Microlith57Misc.Entities;

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
