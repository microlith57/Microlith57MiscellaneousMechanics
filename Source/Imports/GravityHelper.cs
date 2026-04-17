using ModInteropImportGenerator;

namespace Celeste.Mod.Microlith57Misc.Imports {
    [GenerateImports("GravityHelper")]
    public static partial class GravityHelper {
        public static partial void RegisterModSupportBlacklist(string modName);

        public static partial string GravityTypeFromInt(int gravityType);
        public static partial int GravityTypeToInt(string name);
        public static partial int GetPlayerGravity();
        public static partial int GetActorGravity(Actor actor);
        public static partial int SetPlayerGravity(int gravityType, float momentumMultiplier);
        public static partial int SetActorGravity(Actor actor, int gravityType, float momentumMultiplier);
        public static partial bool IsPlayerInverted();
        public static partial bool IsActorInverted(Actor actor);

        public static partial Vector2 GetAboveVector(Actor actor);
        public static partial Vector2 GetBelowVector(Actor actor);
        public static partial Vector2 GetTopCenter(Actor actor);
        public static partial Vector2 GetBottomCenter(Actor actor);
        public static partial Vector2 GetTopLeft(Actor actor);
        public static partial Vector2 GetBottomLeft(Actor actor);
        public static partial Vector2 GetTopRight(Actor actor);
        public static partial Vector2 GetBottomRight(Actor actor);

        // public static partial TalkComponent.TalkComponentUI CreateUpsideDownTalkComponentUI(TalkComponent talkComponent);

        public static partial Component CreateGravityListener(Actor actor, Action<Entity, int, float> gravityChanged);
        public static partial Component CreatePlayerGravityListener(Action<Player, int, float> gravityChanged);

        public static partial void BeginOverride();
        public static partial void EndOverride();
        public static partial void ExecuteOverride(Action action);
        public static partial IDisposable WithOverride();

        public static partial void SetHoldableResetTime(Holdable holdable, float resetTime);
        public static partial void SetHoldableResetType(Holdable holdable, int gravityType);

        public static partial Component CreateAccessibilityListener(Action onAccessibilityChange);
        public static partial Color GetColor(int gravityType);
        public static partial Color GetNormalColor();
        public static partial Color GetInvertedColor();
        public static partial Color GetToggleColor();

        public static partial bool BeginCustomTintShader(bool onlyForAccessibility = true);
        public static partial void EndCustomTintShader();
        public static partial IDisposable WithCustomTintShader(bool onlyForAccessibility = true);

        [OnLoad]
        internal static void OnLoad()
        {
            Load();
            if (!GravityHelper.IsImported) return;

            GravityType.None = GravityTypeToInt("None");
            GravityType.Normal = GravityTypeToInt("Normal");
            GravityType.Inverted = GravityTypeToInt("Inverted");
            GravityType.Toggle = GravityTypeToInt("Toggle");
        }
    }
}

namespace Celeste.Mod.Microlith57Misc {
    public static partial class Utils {
        public struct GravityType {
            public static GravityType None = -1, Normal = 0, Inverted = 1, Toggle = 2;

            private readonly int gravityType;
            public GravityType(int type) => gravityType = type;
            public GravityType(string type) => gravityType = Imports.GravityHelper.GravityTypeToInt(type);

            public static implicit operator int(GravityType g) => g.gravityType;
            public static implicit operator GravityType(int i) => new(i);

            public readonly bool IsNormal => this == GravityType.None || this == GravityType.Normal;
            public readonly bool IsInverted => this == GravityType.Inverted;

            public override readonly string ToString() {
                if (Imports.GravityHelper.IsImported)
                    return Imports.GravityHelper.GravityTypeFromInt(gravityType);

                switch (gravityType) {
                    case -1: return "None";
                    case 0: return "Normal";
                    case 1: return "Inverted";
                    case 2: return "Toggle";
                    default: throw new UnreachableException();
                }
            }
        }

        public static GravityType Gravity(this Actor self) {
            if (!Imports.GravityHelper.IsImported) return GravityType.None;
            return Imports.GravityHelper.GetActorGravity(self);
        }

        public static void Gravity(this Actor self, GravityType grav, float momentumMultiplier = 1f) {
            if (!Imports.GravityHelper.IsImported && grav.IsNormal) return;
            Imports.GravityHelper.SetActorGravity(self, grav, momentumMultiplier);
        }

        public static bool IsInverted(this Actor self) {
            if (!Imports.GravityHelper.IsImported)
                return false;
            return Imports.GravityHelper.IsActorInverted(self);
        }

        public static void SetInverted(this Actor self, bool inverted) => self.Gravity(inverted ? GravityType.Inverted : GravityType.Normal);
    }
}