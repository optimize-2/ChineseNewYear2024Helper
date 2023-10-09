using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.ChineseNewYear2024Helper.BITS.Entities.Entities {
    [Tracked(true)]
    [CustomEntity("ChineseNewYear2024Helper/BITSMagicLanternFeatureController")]
    class BITSMagicLanternFeatureController : Entity {
        public float DarkDuration;
        public float MaxDarkDuration;
        public string Mode;
        public bool Flashing;

        public BITSMagicLanternFeatureController(EntityData data, Vector2 offset) : base(data.Position + offset) {
            DarkDuration = 0;
            MaxDarkDuration = data.Float("maxDarkDuration", 10f);
        }

        public override void Update() {
            Player player = base.Scene.Tracker.GetEntity<Player>();

            if (player != null) {
                bool inDark = true;
                base.Scene.Tracker.GetEntities<BITSMagicLantern>().ForEach(e => {
                    if ((e.Position + new Vector2(0, 5) - player.Position).Length() <= ((BITSMagicLantern) e).Radius) {
                        inDark = false;
                    }
                });

                if (inDark) {
                    DarkDuration += Engine.DeltaTime;
                } else {
                    DarkDuration = 0f;
                }

                if (inDark && DarkDuration >= MaxDarkDuration && !player.Dead) {
                    player.Die(Vector2.Zero);
                }
            }

            float interval = 0f;

            if (DarkDuration > MaxDarkDuration * 0.7) {
                interval = 0.6f;
            } else if (DarkDuration > MaxDarkDuration * 0.5) {
                interval = 1.0f;
            } else if (DarkDuration > MaxDarkDuration * 0.3) {
                interval = 1.4f;
            }

            if (interval > 0 && base.Scene.OnInterval(interval) && player != null && !player.Dead) {
                Input.Rumble(RumbleStrength.Climb, RumbleLength.Short);
                Flashing = !Flashing;
            }

            base.Update();
        }

        public static void Load() {
            On.Celeste.Player.Render += Player_OnRender;
        }

        public static void Unload() {
            On.Celeste.Player.Render -= Player_OnRender;
        }

        private static void Player_OnRender(On.Celeste.Player.orig_Render orig, Player self) {
            // Entity is not tracked during code hotswaps, prevents crashes
            BITSMagicLanternFeatureController controller = self.Scene.Tracker.IsEntityTracked<BITSMagicLanternFeatureController>() ? self.Scene.Tracker.GetEntity<BITSMagicLanternFeatureController>() : null;

            if (controller != null && controller.DarkDuration > 0) {
                float stamina = self.Stamina;
                self.Stamina = controller.DarkDuration + 2 > controller.MaxDarkDuration ? 0 : Player.ClimbMaxStamina;

                orig(self);

                self.Stamina = stamina;
            } else {
                orig(self);
            }
        }
    }
}