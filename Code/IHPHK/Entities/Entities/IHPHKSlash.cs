using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.ChineseNewYear2024Helper.IHPHK.Entities.Entities {
    [Tracked]
    [CustomEntity("ChineseNewYear2024Helper/IHPHKSlash")]
    class IHPHKSlash : Entity {
        private Sprite sprite;
        public bool failed = true;
        Entity entity;

        private static Vector2 off = new Vector2(0, 8f); 

        public IHPHKSlash(Entity entity, bool failed, int facing) : base(entity.Position + off) {
            this.Depth = -10000;
            this.failed = failed;
            this.entity = entity;

            Add(sprite = GFX.SpriteBank.Create("ChineseNewYear2024HelperIHPHKSlash"));
            sprite.RenderPosition += off;
            sprite.Scale = new Vector2(1.2f, 1.2f);
            // Add(sprite = GFX.SpriteBank.Create("seeker"));
            // sprite.Scale = new Vector2(10f, 10f);

            if (facing == 1) sprite.FlipX = true;
            sprite.OnFinish = (name) => {
                sprite.Entity.RemoveSelf();
            };
        }

        public override void Added(Scene scene) {
            base.Added(scene);
            Audio.Play("event:/OP/slash");
            if (!failed) Audio.Play("event:/OP/succeed");
            sprite.Play(failed ? "failed" : "succeed", false);
            // sprite.Play("idle", false);
        }

        public void BufferedActive() {
            int frame = sprite.CurrentAnimationFrame;
            sprite.Play("succeed", false);
            sprite.SetAnimationFrame(frame);
            Audio.Play("event:/OP/succeed");
        }

        public override void Update() {
            // if (this.entity != null) {
            //     base.Position = this.entity.Position + off;
            // }
            base.Update();
        }
    }
}