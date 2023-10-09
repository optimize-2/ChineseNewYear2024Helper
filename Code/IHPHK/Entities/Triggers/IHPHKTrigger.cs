using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.ChineseNewYear2024Helper.IHPHK.Entities.Triggers {
    [Tracked]
    [CustomEntity("ChineseNewYear2024Helper/IHPHKTrigger")]
    class IHPHKTrigger : Trigger {
        private static EntityData entityData;

        public static bool MovingSpinner, StaticSpinner;
        public static bool Sawblade, Wingmould;
        public static bool Seeker;
        public static bool ZipMover;

        public IHPHKTrigger(EntityData data, Vector2 offset) : base(data, offset) {
            this.Depth = 10000;
            entityData = data;
            MovingSpinner = data.Bool("movingSpinner", true);
            StaticSpinner = data.Bool("staticSpinner", true);
            Sawblade = data.Bool("sawblade", true);
            Wingmould = data.Bool("wingmould", true);
            Seeker = data.Bool("seeker", true);
            ZipMover = data.Bool("zipMover", true);
        }

        public override void Added(Scene scene) {
            base.Added(scene);
        }

        public override void OnEnter(Player player) {
            base.OnEnter(player); 
        }

        public override void OnStay(Player player) {
            base.OnStay(player);
        }

        public override void OnLeave(Player player) {
            base.OnLeave(player);
        }
    }
}