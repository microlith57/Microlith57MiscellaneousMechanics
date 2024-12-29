using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

using Celeste.Mod.Microlith57Misc.Components;

namespace Celeste.Mod.Microlith57Misc.Entities;

[CustomEntity(
    "Microlith57Misc/SliderColorgradeController=Create",
    "Microlith57Misc/SliderColorgradeController_Expression=CreateExpr"
)]
public sealed class SliderColorgradeController : SliderController {

    #region --- State ---

    public readonly string ColorgradeA, ColorgradeB;

    #endregion State
    #region --- Init ---

    public SliderColorgradeController(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        FloatSource valueSource
    ) : base(data, offset, enabledCondition, valueSource) {

        ColorgradeA = data.Attr("colorgradeA", "none");
        ColorgradeB = data.Attr("colorgradeB", "none");
    }

    public static SliderColorgradeController Create(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Flag(data) { Default = true },
            new FloatSource.Slider(level.Session, data, name: "lerp", ifAbsent: "colorgradeLerp")
        );

    public static SliderColorgradeController CreateExpr(Level _, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Expr(data) { Default = true },
            new FloatSource.Expr(data, name: "lerp", ifAbsent: "@colorgradeLerp")
        );

    #endregion Init
    #region --- Behaviour ---

    public override void Update() {
        base.Update();

        if (Scene is not Level level || !Enabled) return;

        LerpColorgrade(level, ColorgradeA, ColorgradeB, Value);
    }

    private static void LerpColorgrade(Level level, string from, string to, float lerp) {
        if (from == to || lerp == 0f) {
            level.SnapColorGrade(from);
            return;
        } else if (lerp == 1f) {
            level.SnapColorGrade(to);
            return;
        }

        if (lerp > 0.5f) {
            lerp = 1f - lerp;
            (from, to) = (to, from);
        }

        level.lastColorGrade = from;
        level.Session.ColorGrade = to;
        level.colorGradeEase = lerp;
        level.colorGradeEaseSpeed = 0f;
    }

    #endregion Behaviour

}