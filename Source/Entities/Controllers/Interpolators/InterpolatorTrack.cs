using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;

using Celeste.Mod.Microlith57Misc.Components;
using System;

namespace Celeste.Mod.Microlith57Misc.Entities.Interpolator;

[CustomEntity(
    "Microlith57Misc/InterpolatorTrack=Create",
    "Microlith57Misc/InterpolatorTrack_Expression=CreateExpr"
)]
[Tracked]
internal class InterpolatorTrack : Entity {

    internal class SortedListFloatTo<T> where T : class {
        private SortedList<float, T> list = [];
        public void Add(float t, T val) => list.Add(t, val);

        public (T left, T right, float lerp) Get(float pos, bool wrap) {
            if (list.Count == 0) throw new Exception("interpolator track empty somehow");
            if (list.Count == 1) return (list[1], list[1], 0f);

            int lo = 0;
            int hi = list.Count - 1;
            while (lo <= hi) {
                int i = lo + ((hi - lo) / 2);

                float key = list.GetKeyAtIndex(i);
                if (key == pos) {
                    var val = list.GetValueAtIndex(i);
                    return (val, val, 0f);
                }
                if (key < pos) {
                    lo = i + 1;
                } else {
                    hi = i - 1;
                }
            }

            T before = lo == 0 ? list.GetValueAtIndex(list.Count - 1) : list.GetValueAtIndex(lo - 1);
            T after = lo >= list.Count ? list.GetValueAtIndex(0) : list.GetValueAtIndex(lo);

            if (lo == 0)
                return wrap ? (
                    before, after, 1f - list.GetKeyAtIndex(0)
                ) : (
                    after, after, 0f
                );
            else if (lo == list.Count) {
                return wrap ? (
                    before, after, 1f - pos
                ) : (
                    before, before, 0f
                );
            } else {
                return (
                    before, after,
                    Calc.ClampedMap(pos, list.GetKeyAtIndex(lo - 1), list.GetKeyAtIndex(lo))
                );
            }

        }
    }

    internal class Subtrack() : Component(active: true, visible: false) {}

    internal class FlagSubtrack(string flag) : Subtrack {
        private string Flag = flag;
        private SortedListFloatTo<FlagStop> Stops = new();

        internal void Add(float fac, FlagStop stop) {
            Stops.Add(fac, stop);
            stop.Added(Entity);
        }

        public override void Update() {
            base.Update();
            if (Entity?.Scene is not Level level || Entity is not InterpolatorTrack track) return;
            (var a, var b, var t) = Stops.Get(track.Pos, track.Wrap);
            level.Session.SetFlag(Flag, a.Lerp(b.Value, t));
        }
    }

    internal class SliderSubtrack(string slider) : Subtrack {
        private string Slider = slider;
        private SortedListFloatTo<SliderStop> Stops = new();

        internal void Add(float fac, SliderStop stop) {
            Stops.Add(fac, stop);
            stop.Added(Entity);
        }

        public override void Update() {
            base.Update();
            if (Entity?.Scene is not Level level || Entity is not InterpolatorTrack track) return;
            (var a, var b, var t) = Stops.Get(track.Pos, track.Wrap);
            level.Session.SetSlider(Slider, a.Lerp(b.Value, t));
        }
    }

    internal class ColorSubtrack(string counter) : Subtrack {
        private string Counter = counter;
        private SortedListFloatTo<ColorStop> Stops = new();

        internal void Add(float fac, ColorStop stop) {
            Stops.Add(fac, stop);
            stop.Added(Entity);
        }

        public override void Update() {
            base.Update();
            if (Entity?.Scene is not Level level || Entity is not InterpolatorTrack track) return;
            (var a, var b, var t) = Stops.Get(track.Pos, track.Wrap);
            level.Session.SetCounter(Counter, unchecked((int)a.Lerp(b.Value, t).PackedValue));
        }
    }

    internal bool Contains(Vector2 pos)
        => Left <= pos.X && pos.X <= Right;

    internal float PosToFac(Vector2 pos)
        => Calc.ClampedMap(pos.X, Left, Right);


    #region --- State ---

    private readonly ConditionSource EnabledCondition;
    public bool Enabled => EnabledCondition.Value;

    private FloatSource ValueSource;
    public float Value => ValueSource.Value;

    public readonly float Minimum, Maximum;
    public readonly bool Wrap;
    private float Pos;

    internal readonly List<Subtrack> Subtracks = new();

    #endregion State
    #region --- Init ---

    public InterpolatorTrack(
        EntityData data, Vector2 offset,
        ConditionSource enabledCondition,
        FloatSource valueSource
    ) : base(data.Position + offset) {

        AddTag(Tags.PauseUpdate | Tags.FrozenUpdate);

        Collider = new Hitbox(data.Width, data.Height);

        Add(EnabledCondition = enabledCondition);
        Add(ValueSource = valueSource);

        Minimum = data.Float("minimum", 0f);
        Maximum = data.Float("maximum", 1f);
        Wrap = data.Bool("wrap");
    }

    public static InterpolatorTrack Create(Level level, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Flag(data) { Default = true },
            new FloatSource.Slider(level.Session, data)
        );

    public static InterpolatorTrack CreateExpr(Level _, LevelData __, Vector2 offset, EntityData data)
        => new(
            data, offset,
            new ConditionSource.Expr(data) { Default = true },
            new FloatSource.Expr(data)
        );

    #endregion Init
    #region --- Behaviour ---

    public override void Update() {
        if (Enabled) {
            Pos = Calc.ClampedMap(Value, Minimum, Maximum);
            base.Update();
        }
    }

    #endregion Behaviour

}