#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

using Microsoft.Xna.Framework.Input;

namespace Celeste.Mod.Microlith57Misc;

public class Microlith57MiscSettings : EverestModuleSettings {

    [DefaultButtonBinding(
        buttons: [Buttons.LeftTrigger, Buttons.RightTrigger, Buttons.LeftShoulder, Buttons.RightShoulder],
        keys: [Keys.Z, Keys.V, Keys.LeftShift]
    )]
    public ButtonBinding Focus { get; set; }

}