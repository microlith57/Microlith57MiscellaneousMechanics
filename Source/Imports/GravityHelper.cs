using System;
using Monocle;
using MonoMod.ModInterop;

namespace Celeste.Mod.Microlith57Misc.Imports;

[ModImportName("GravityHelper")]
public static class GravityHelper {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    internal struct GravityType {

        public static GravityType None, Normal, Inverted, Toggle;

        private readonly int gravityType;

        public GravityType(int type) {
            gravityType = type;
        }

        public GravityType(string type) {
            gravityType = GravityTypeToInt(type);
        }

        public static implicit operator int(GravityType g) => g.gravityType;
        public static implicit operator GravityType(int i) => new(i);

        public override readonly string ToString() => GravityTypeFromInt(gravityType);

    }

    // public static void RegisterModSupportBlacklist(string modName) => ThirdPartyModSupport.BlacklistedMods.Add(modName);

    public delegate string _GravityTypeFromInt(int gravityType);
    public static _GravityTypeFromInt GravityTypeFromInt;

    public delegate int _GravityTypeToInt(string name);
    public static _GravityTypeToInt GravityTypeToInt;

    public delegate int _GetPlayerGravity();
    public static _GetPlayerGravity GetPlayerGravity;

    public delegate int _GetActorGravity(Actor actor);
    public static _GetActorGravity GetActorGravity;

    public delegate int _SetPlayerGravity(int gravityType, float momentumMultiplier);
    public static _SetPlayerGravity SetPlayerGravity;

    public delegate int _SetActorGravity(Actor actor, int gravityType, float momentumMultiplier);
    public static _SetActorGravity SetActorGravity;

    public delegate bool _IsPlayerInverted();
    public static _IsPlayerInverted IsPlayerInverted;

    public delegate bool _IsActorInverted(Actor actor);
    public static _IsActorInverted IsActorInverted;

    // public static Vector2 GetAboveVector(Actor actor) =>
    //     actor?.ShouldInvert() == true ? Vector2.UnitY : -Vector2.UnitY;

    // public static Vector2 GetBelowVector(Actor actor) =>
    //     actor?.ShouldInvert() == true ? -Vector2.UnitY : Vector2.UnitY;

    // public static Vector2 GetTopCenter(Actor actor) =>
    //     actor?.ShouldInvert() == true ? actor.BottomCenter : actor?.TopCenter ?? Vector2.Zero;

    // public static Vector2 GetBottomCenter(Actor actor) =>
    //     actor?.ShouldInvert() == true ? actor.TopCenter : actor?.BottomCenter ?? Vector2.Zero;

    // public static Vector2 GetTopLeft(Actor actor) =>
    //     actor?.ShouldInvert() == true ? actor.BottomLeft : actor?.TopLeft ?? Vector2.Zero;

    // public static Vector2 GetBottomLeft(Actor actor) =>
    //     actor?.ShouldInvert() == true ? actor.TopLeft : actor?.BottomLeft ?? Vector2.Zero;

    // public static Vector2 GetTopRight(Actor actor) =>
    //     actor?.ShouldInvert() == true ? actor.BottomRight : actor?.TopRight ?? Vector2.Zero;

    // public static Vector2 GetBottomRight(Actor actor) =>
    //     actor?.ShouldInvert() == true ? actor.TopRight : actor?.BottomRight ?? Vector2.Zero;

    public delegate TalkComponent.TalkComponentUI _CreateUpsideDownTalkComponentUI(TalkComponent talkComponent);
    public static _CreateUpsideDownTalkComponentUI CreateUpsideDownTalkComponentUI;

    public delegate Component _CreateGravityListener(Actor actor, Action<Entity, int, float> gravityChanged);
    public static _CreateGravityListener CreateGravityListener;

    public delegate Component _CreatePlayerGravityListener(Action<Player, int, float> gravityChanged);
    public static _CreatePlayerGravityListener CreatePlayerGravityListener;

    public static Action BeginOverride, EndOverride;
    public static Action<Action> ExecuteOverride;
    public static Func<IDisposable> WithOverride;

    internal static void OnImport() {
        if (GravityTypeToInt is null) return;

        GravityType.None = GravityTypeToInt("None");
        GravityType.Normal = GravityTypeToInt("Normal");
        GravityType.Inverted = GravityTypeToInt("Inverted");
        GravityType.Toggle = GravityTypeToInt("Toggle");
    }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}
