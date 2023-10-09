using Microsoft.Xna.Framework;

namespace Celeste.Mod.ChineseNewYear2024Helper {
    public class ChineseNewYear2024HelperModuleSession : EverestModuleSession {
        public bool HasSpeedPower { get; set; } = false;
        public bool CanAddSpeed { get; set; } = false;
        public Vector2 StoredSpeed { get; set; } = Vector2.Zero;
        public Facings Facing { get; set; } = Facings.Right;

        public void ResetSpeedPowerup() {
            HasSpeedPower = false;
            CanAddSpeed = false;
            StoredSpeed = Vector2.Zero;
            Facing = Facings.Right;
        }

        public int AlwaysBreakDashBlockDash = 0;
    }
}
