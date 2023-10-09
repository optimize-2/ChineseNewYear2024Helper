using Microsoft.Xna.Framework.Input;

namespace Celeste.Mod.ChineseNewYear2024Helper {
    public class ChineseNewYear2024HelperModuleSettings : EverestModuleSettings {
        [DefaultButtonBinding(Buttons.LeftShoulder, Keys.Space)]
        public ButtonBinding DownSlash { get; set; }
    }
}