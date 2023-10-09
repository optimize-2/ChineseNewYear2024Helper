using System;
using System.Collections.Generic;
using Celeste.Mod.ChineseNewYear2024Helper.IHPHK.Entities.Entities;
using Celeste.Mod.ChineseNewYear2024Helper.IHPHK.Entities.Triggers;
using FrostHelper;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.ChineseNewYear2024Helper.IHPHK {
    public class IHPHKController : GameComponent {
        public static Game CelesteGame;
        public static Level Level;
        public static Player Player;

        private bool failed;

        private static bool leftButtonReleased = true;

        private Vector2 previousPos;
        private bool bouncing;
        private static Entity slashedEntity;
        private static IHPHKSlash slash;
        private int cooldown;
        private int buffer;

        public IHPHKController(Game game) : base(game) {
            IHPHKController.CelesteGame = game;
            bouncing = false;
            Load();
        }
        
        public static void OnLoadLevel(Level level, Player.IntroTypes intro, bool fromLoader) {
            Level = level;
            Player = level.Tracker.GetEntity<Player>();
        }

        public static bool OnIsRiding(On.Celeste.Player.orig_IsRiding_Solid orig, Player self, Solid solid) {
            if (solid == slashedEntity) return true;
            else return orig(self, solid);
        }

        public void Load() {
            Everest.Events.Level.OnLoadLevel += OnLoadLevel;
            On.Celeste.Player.IsRiding_Solid += OnIsRiding;
        }
        
        public void Unload() {
            Everest.Events.Level.OnLoadLevel -= OnLoadLevel;
            On.Celeste.Player.IsRiding_Solid -= OnIsRiding;
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            if (Level == null || Level.Paused) return;
            var playerList = Level.Tracker.GetEntities<Player>();
            if (playerList.Count > 0) Player = (Player) playerList[0];
            else return;
            if (Player == null) return;
            if (Level.Tracker.GetEntity<IHPHKRenderer>() == null) {
                Level.Add(new IHPHKRenderer());
            }
            if (Level.Tracker.GetEntity<IHPHKTrigger>() == null) return;
            if (cooldown >= 0) cooldown -= 1;
            if (buffer >= 0) buffer -= 1;
            if (bouncing) {
                if (slashedEntity != null) {
                    Vector2 curpos = slashedEntity.Position;
                    if (slashedEntity is Actor) {
                        curpos = ((Actor) slashedEntity).ExactPosition;
                    }
                    Vector2 speed = (curpos - previousPos) / Engine.DeltaTime;
                    if (slashedEntity is Seeker) {
                        speed = ((Seeker) slashedEntity).Speed;
                        ((Seeker) slashedEntity).Speed.Y = 150f;
                    }
                    bouncePlayer(speed);
                }
                bouncing = false;
                slashedEntity = null;
            }
            if (buffer >= 0) {
                if (Level.Tracker.GetEntities<Player>().Count == 0) return;
                if (Player != null && !Player.Dead && slash != null && slash.failed) {
                    if (!checkSlash(Player.Position + new Vector2(0, -8f))) {
                        slash.BufferedActive();
                    }
                }
            }
            if (ChineseNewYear2024HelperModule.Settings.DownSlash.Pressed && cooldown < 0) {
                if (Level.Tracker.GetEntities<Player>().Count == 0) return;
                if (Player != null && !Player.Dead) {
                    Level.Add(slash = new IHPHKSlash(Player, checkSlash(Player.Position + new Vector2(0, -8f)), (int) Player.Facing));
                    cooldown = 12;
                    buffer = 3;
                }
            }
        }

        private void bouncePlayer(Vector2 speed) {
			Celeste.Freeze(0.05f);
            
            if (speed.Y > 0) speed.Y = 0;
            Player.Speed.Y = 0;
            Player.StateMachine.ForceState(Player.StNormal);
            Player.Speed += speed + new Vector2(0, -180);
        }

        private bool checkSlash(Vector2 pos) {
            bool solidFound = false;
            Level.Tracker.GetEntities<Solid>().ForEach(e => {
                if (e.CollideRect(new Rectangle((int) Player.Position.X - 8, (int) Player.Position.Y, 16, 25))) {
                    if (solidFound) return;
                    if (
                        (e is (ZipMover or CustomZipMover) && IHPHKTrigger.ZipMover)
                    ) {
                        bouncing = true;
                        slashedEntity = e;
                        previousPos = e.Position;
                        if (e is Actor) {
                            previousPos = ((Actor) e).ExactPosition;
                        }
                        solidFound = true;
                        return;
                    }
                }
            });
            if (solidFound) return false;
            
            using (List<Component>.Enumerator enumerator = Level.Tracker.GetComponents<PlayerCollider>().GetEnumerator()) {
                while (enumerator.MoveNext()) {
                    if (enumerator.Current.Entity.CollideRect(new Rectangle((int) Player.Position.X - 8, (int) Player.Position.Y, 16, 25))) {
                        Entity e = enumerator.Current.Entity;
                        if (
                            (e is (TrackSpinner or RotateSpinner or CustomSpeedRotateSpinner) && IHPHKTrigger.MovingSpinner) ||
                            (e is CrystalStaticSpinner && IHPHKTrigger.StaticSpinner) ||
                            (e is Seeker && IHPHKTrigger.Seeker) ||
                            (e.GetType().Name == "Sawblade" && IHPHKTrigger.Sawblade) ||
                            (e.GetType().Name == "Wingmould" && IHPHKTrigger.Wingmould)
                        ) {
                            bouncing = true;
                            slashedEntity = e;
                            previousPos = e.Position;
                            if (e is Actor) {
                                previousPos = ((Actor) e).ExactPosition;
                            }
                            // if (e is ZipMover) {
                            //     ((ZipMover) e).
                            // }
                            
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}