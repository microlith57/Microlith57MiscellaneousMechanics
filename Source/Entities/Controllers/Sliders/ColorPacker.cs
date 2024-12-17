using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

using Celeste.Mod.Microlith57Misc.Components;
using Monocle;

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

        if (Scene is not Level level) return;
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

        if (Scene is not Level level) return;
        level.Session.SetCounter(Counter, unchecked((int)Color.PackedValue));
    }

}

#endregion Int