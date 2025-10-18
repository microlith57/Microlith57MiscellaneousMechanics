using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using Monocle;

using Celeste.Mod.Microlith57Misc.Components;
using static Celeste.Mod.Microlith57Misc.Utils;

namespace Celeste.Mod.Microlith57Misc.Entities;

#region --- Float ---

[CustomEntity(
    "Microlith57Misc/ColorPacker_Float=Create",
    "Microlith57Misc/ColorPacker_Float_Expression=CreateExpr"
)]
public sealed class ColorPackerFloat : Entity {

    #region --- State ---

    private readonly ConditionSource EnabledCondition;
    public bool Enabled => EnabledCondition.Value;

    private readonly FloatSource RSource, GSource, BSource, ASource, AlphaSource;
    public Color Color => new Color(RSource.Value, GSource.Value, BSource.Value, ASource.Value) * AlphaSource.Value;

    private string Counter;

    #endregion State
    #region --- Init ---

    public ColorPackerFloat(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        FloatSource rSource,
        FloatSource gSource,
        FloatSource bSource,
        FloatSource aSource,
        FloatSource alphaSource
    ) : base(data.Position + offset) {

        Add(EnabledCondition = enabledCondition);
        Add(RSource = rSource);
        Add(GSource = gSource);
        Add(BSource = bSource);
        Add(ASource = aSource);
        Add(AlphaSource = alphaSource);

        Counter = data.Attr("packedColor", "color");
    }

    public static ColorPackerFloat Create(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Flag(data) { Default = true },
            new FloatSource.Slider(level.Session, data, "r") { Default = 1f },
            new FloatSource.Slider(level.Session, data, "g") { Default = 1f },
            new FloatSource.Slider(level.Session, data, "b") { Default = 1f },
            new FloatSource.Slider(level.Session, data, "a") { Default = 1f },
            new FloatSource.Slider(level.Session, data, "alpha") { Default = 1f }
        );

    public static ColorPackerFloat CreateExpr(Level _, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Expr(data) { Default = true },
            new FloatSource.Expr(data, "r") { Default = 1f },
            new FloatSource.Expr(data, "g") { Default = 1f },
            new FloatSource.Expr(data, "b") { Default = 1f },
            new FloatSource.Expr(data, "a") { Default = 1f },
            new FloatSource.Expr(data, "alpha") { Default = 1f }
        );

    #endregion Init

    public override void Update() {
        base.Update();
        if (Scene is not Level level || !Enabled) return;

        level.Session.SetCounter(Counter, unchecked((int)Color.PackedValue));
    }

}

#endregion Float
#region --- Int ---

[CustomEntity(
    "Microlith57Misc/ColorPacker_Int=Create",
    "Microlith57Misc/ColorPacker_Int_Expression=CreateExpr"
)]
public sealed class ColorPackerInt : Entity {

    #region --- State ---

    private readonly ConditionSource EnabledCondition;
    public bool Enabled => EnabledCondition.Value;

    private readonly IntSource RSource, GSource, BSource, ASource;
    private readonly FloatSource AlphaSource;
    public Color Color => new Color(RSource.Value, GSource.Value, BSource.Value, ASource.Value) * AlphaSource.Value;

    private string Counter;

    #endregion State
    #region --- Init ---

    public ColorPackerInt(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        IntSource rSource,
        IntSource gSource,
        IntSource bSource,
        IntSource aSource,
        FloatSource alphaSource
    ) : base(data.Position + offset) {

        Add(EnabledCondition = enabledCondition);
        Add(RSource = rSource);
        Add(GSource = gSource);
        Add(BSource = bSource);
        Add(ASource = aSource);
        Add(AlphaSource = alphaSource);

        Counter = data.Attr("packedColor", "color");
    }

    public static ColorPackerInt Create(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Flag(data) { Default = true },
            new IntSource.Counter(level.Session, data, "r") { Default = 255 },
            new IntSource.Counter(level.Session, data, "g") { Default = 255 },
            new IntSource.Counter(level.Session, data, "b") { Default = 255 },
            new IntSource.Counter(level.Session, data, "a") { Default = 255 },
            new FloatSource.Slider(level.Session, data, "alpha") { Default = 1f }
        );

    public static ColorPackerInt CreateExpr(Level _, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Expr(data) { Default = true },
            new IntSource.Expr(data, "r") { Default = 255 },
            new IntSource.Expr(data, "g") { Default = 255 },
            new IntSource.Expr(data, "b") { Default = 255 },
            new IntSource.Expr(data, "a") { Default = 255 },
            new FloatSource.Expr(data, "alpha") { Default = 1f }
        );

    #endregion Init

    public override void Update() {
        base.Update();
        if (Scene is not Level level || !Enabled) return;

        level.Session.SetCounter(Counter, unchecked((int)Color.PackedValue));
    }

}

#endregion Int
#region --- HSL / HSV ---

[CustomEntity(
    "Microlith57Misc/ColorPacker_HSL=CreateHSL",
    "Microlith57Misc/ColorPacker_HSL_Expression=CreateHSLExpr",
    "Microlith57Misc/ColorPacker_HSV=CreateHSV",
    "Microlith57Misc/ColorPacker_HSV_Expression=CreateHSVExpr"
)]
public sealed class ColorPackerHSLV : Entity {

    #region --- State ---

    private readonly ConditionSource EnabledCondition;
    public bool Enabled => EnabledCondition.Value;

    public readonly AngleFormat Format;
    public readonly bool IsHSV;

    private readonly FloatSource HSource, SSource, LVSource, AlphaSource;
    public Color ColorHSL => HSLToColor(HSource.Value, SSource.Value, LVSource.Value, Format) * AlphaSource.Value;
    public Color ColorHSV => HSVToColor(HSource.Value, SSource.Value, LVSource.Value, Format) * AlphaSource.Value;

    private string Counter;

    #endregion State
    #region --- Init ---

    public ColorPackerHSLV(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        FloatSource hSource,
        FloatSource sSource,
        FloatSource lvSource,
        FloatSource alphaSource,
        bool isHSV
    ) : base(data.Position + offset) {

        Format = data.Enum("format", AngleFormat.ZeroToOne);

        Add(EnabledCondition = enabledCondition);
        Add(HSource = hSource);
        Add(SSource = sSource);
        Add(LVSource = lvSource);
        Add(AlphaSource = alphaSource);

        Counter = data.Attr("packedColor", "color");

        IsHSV = isHSV;
    }

    public static ColorPackerHSLV CreateHSL(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Flag(data) { Default = true },
            new FloatSource.Slider(level.Session, data, "h") { Default = 0f },
            new FloatSource.Slider(level.Session, data, "s") { Default = 1f },
            new FloatSource.Slider(level.Session, data, "l") { Default = 1f },
            new FloatSource.Slider(level.Session, data, "alpha") { Default = 1f },
            isHSV: false
        );

    public static ColorPackerHSLV CreateHSLExpr(Level _, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Expr(data) { Default = true },
            new FloatSource.Expr(data, "h") { Default = 0f },
            new FloatSource.Expr(data, "s") { Default = 1f },
            new FloatSource.Expr(data, "l") { Default = 1f },
            new FloatSource.Expr(data, "alpha") { Default = 1f },
            isHSV: false
        );

    public static ColorPackerHSLV CreateHSV(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Flag(data) { Default = true },
            new FloatSource.Slider(level.Session, data, "h") { Default = 0f },
            new FloatSource.Slider(level.Session, data, "s") { Default = 1f },
            new FloatSource.Slider(level.Session, data, "v") { Default = 1f },
            new FloatSource.Slider(level.Session, data, "alpha") { Default = 1f },
            isHSV: true
        );

    public static ColorPackerHSLV CreateHSVExpr(Level _, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Expr(data) { Default = true },
            new FloatSource.Expr(data, "h") { Default = 0f },
            new FloatSource.Expr(data, "s") { Default = 1f },
            new FloatSource.Expr(data, "v") { Default = 1f },
            new FloatSource.Expr(data, "alpha") { Default = 1f },
            isHSV: true
        );

    #endregion Init

    public override void Update() {
        base.Update();
        if (Scene is not Level level || !Enabled) return;

        level.Session.SetCounter(Counter, unchecked((int)(IsHSV ? ColorHSV : ColorHSL).PackedValue));
    }

}

#endregion HSL / HSV
