using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;

using Celeste.Mod.Microlith57Misc.Components;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity(
    "Microlith57Misc/SliderColorgradeController=CreateFlag",
    "Microlith57Misc/SliderColorgradeController_Expression=CreateExpr"
)]
[Tracked]
public sealed class SliderColorgradeController : Entity {

    #region --- State ---

    private readonly ConditionSource EnabledCondition;
    public bool Enabled => EnabledCondition.Value;

    private readonly FloatSource LerpSource;
    public float Lerp => LerpSource.Value;

    public readonly string ColorgradeA, ColorgradeB;

    #endregion State
    #region --- Init ---

    public SliderColorgradeController(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        FloatSource lerpSource
    ) : base(data.Position + offset) {

        Add(EnabledCondition = enabledCondition);
        Add(LerpSource = lerpSource);

        ColorgradeA = data.Attr("colorgradeA", "none");
        ColorgradeB = data.Attr("colorgradeB", "none");
    }

    public static SliderColorgradeController CreateFlag(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.FlagSource(data) { Default = true },
            new FloatSource.SliderSource(level.Session, data, ifAbsent: "colorgradeLerp")
        );

    public static SliderColorgradeController CreateExpr(Level _, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.ExpressionSource(data) { Default = true },
            new FloatSource.ExpressionSource(data, ifAbsent: "@colorgradeLerp")
        );

    #endregion Init
    #region --- Behaviour ---

    public override void Update() {
        base.Update();

        if (!Enabled || Scene is not Level level) return;

        if (ColorgradeA == ColorgradeB) {
            level.SnapColorGrade(ColorgradeA);
        } else {
            string from = ColorgradeA, to = ColorgradeB;
            float lerp = Lerp;

            if (lerp > 0.5f) {
                lerp = 0.5f - lerp;
                (from, to) = (to, from);
            }

            level.lastColorGrade = from;
            level.Session.ColorGrade = to;
            level.colorGradeEase = lerp;
            level.colorGradeEaseSpeed = 0f;
        }
    }

    #endregion Behaviour

}