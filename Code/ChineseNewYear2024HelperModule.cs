using System;
using Celeste.Mod.ChineseNewYear2024Helper.BITS.Entities.Entities;
using Celeste.Mod.ChineseNewYear2024Helper.IHPHK;

namespace Celeste.Mod.ChineseNewYear2024Helper {
    public class ChineseNewYear2024HelperModule : EverestModule {
        public static readonly String NAME = "ChineseNewYear2024Helper";

        public static ChineseNewYear2024HelperModule Instance { get; private set; }

        public override Type SettingsType => typeof(ChineseNewYear2024HelperModuleSettings);
        public static ChineseNewYear2024HelperModuleSettings Settings => (ChineseNewYear2024HelperModuleSettings) Instance._Settings;

        public override Type SessionType => typeof(ChineseNewYear2024HelperModuleSession);
        public static ChineseNewYear2024HelperModuleSession Session => (ChineseNewYear2024HelperModuleSession) Instance._Session;

        private static IHPHKController IHPHK;

        public ChineseNewYear2024HelperModule() {
            Instance = this;
        }

        public override void Load() {
            Logger.SetLogLevel(NAME, LogLevel.Debug);
            IHPHK = new IHPHKController(Celeste.Instance);
            BITSMagicLanternFeatureController.Load();
            SpeedPowerup.Load();
            Celeste.Instance.Components.Add(IHPHK);
        }

        public override void Unload() {
            IHPHK.Unload();
            BITSMagicLanternFeatureController.Unload();
            SpeedPowerup.Unload();
            Celeste.Instance.Components.Remove(IHPHK);
        }

        public static void Info(string s) {
            Logger.Log(LogLevel.Info, NAME, s);
        }

        public static void Debug(string s) {
            Logger.Log(LogLevel.Debug, NAME, s);
        }
    }
}