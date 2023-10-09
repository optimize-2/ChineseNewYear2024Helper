using System;
using System.Collections;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.ChineseNewYear2024Helper.SNY.Entities.Entities {
    [Tracked]
    [CustomEntity("ChineseNewYear2024Helper/SNYFireworkHoldable")]
	public class SNYFireworkHoldable : Actor {
		public float BeforeLaunchTime;
		public float LaunchTime;
		public float LaunchSpeed, LaunchBeginSpeed, LaunchMaxSpeed, LaunchSpeedModifier;
		public bool Holdable;

		public SNYFireworkHoldable(EntityData e, Vector2 offset) : base(e.Position + offset) {
			this.BeforeLaunchTime = e.Float("beforeLaunchTime", 3f);
			this.LaunchTime = e.Float("launchTime", 5f);
			this.LaunchBeginSpeed = e.Float("launchBeginMaxSpeed", 2.0f);
			this.LaunchMaxSpeed = e.Float("launchMaxSpeed", 3.0f);
			this.LaunchSpeedModifier = e.Float("launchSpeedModifier", 0.1f);
			this.launching = false;

			this.previousPosition = e.Position + offset;
			base.Collider = new Hitbox(8f, 10f, -4f, -10f);
			base.Add(this.sprite = GFX.SpriteBank.Create("ChineseNewYear2024BitsMagicLanternLight"));
			sprite.Y += -4;
			this.sprite.Play("holdable");
			this.sprite.Scale.X = -1f;
			base.Add(this.Hold = new Holdable(0.1f));
			this.Hold.PickupCollider = new Hitbox(16f, 22f, -8f, -16f);
			
			this.Hold.SlowFall = false;
			this.Hold.SlowRun = false;
			this.Hold.OnPickup = new Action(this.OnPickup);
			this.Hold.OnRelease = new Action<Vector2>(this.OnRelease);
			this.Hold.DangerousCheck = new Func<HoldableCollider, bool>(this.Dangerous);
			this.Hold.OnHitSeeker = new Action<Seeker>(this.HitSeeker);
			this.Hold.OnSwat = new Action<HoldableCollider, int>(this.Swat);
			this.Hold.OnHitSpring = new Func<Spring, bool>(this.HitSpring);
			this.Hold.OnHitSpinner = new Action<Entity>(this.HitSpinner);
			this.Hold.SpeedGetter = (() => this.Speed);
			this.onCollideH = new Collision(this.OnCollideH);
			this.onCollideV = new Collision(this.OnCollideV);
			this.LiftSpeedGraceTime = 0.1f;
			base.Add(new VertexLight(base.Collider.Center, Color.White, 1f, 32, 64));
			base.Add(new MirrorReflection());
		}

		public override void Added(Scene scene) {
			base.Added(scene);
			// base.SceneAs<Level>() = base.SceneAs<Level>();
			// foreach (Entity entity in base.SceneAs<Level>().Tracker.GetEntities<SNYFireworkHoldable>()) {
			// 	SNYFireworkHoldable SNYFireworkHoldable = (SNYFireworkHoldable)entity;
			// 	if (SNYFireworkHoldable != this && SNYFireworkHoldable.Hold.IsHeld) {
			// 		base.RemoveSelf();
			// 	}
			// }
		}

		public override void Update() {
			base.Depth = -100000000;
			base.Update();
			Player player = base.Scene.Tracker.GetEntity<Player>();
			if (this.shattering || this.dead) {
				return;
			}
			if (this.Hold.IsHeld) this.launching = true;
			if (this.launching) {
				if (this.BeforeLaunchTime > 0) {
					base.SceneAs<Level>().Particles.Emit(SNYFireworkHoldable.P_LaunchingA, 1, this.Position + new Vector2(0, 3f), new Vector2(1f, 1f), (float) Math.PI * 0.5f);
					this.BeforeLaunchTime -= Engine.DeltaTime;
					this.LaunchSpeed = this.LaunchBeginSpeed;
				} else if (this.LaunchTime > 0) {
					base.SceneAs<Level>().Particles.Emit(SNYFireworkHoldable.P_LaunchingA, 1 + (int) Math.Min(4, (int) (LaunchTime * 2f)), this.Position + new Vector2(0, 3f), new Vector2(1f, 1f), (float) Math.PI * 0.5f);
					base.SceneAs<Level>().Particles.Emit(SNYFireworkHoldable.P_LaunchingB, (int) Math.Min(4, (int) (LaunchTime * 2f)), this.Position + new Vector2(0, 3f), new Vector2(1f, 1f), (float) Math.PI * 0.5f);
					ChineseNewYear2024HelperModule.Debug(this.BeforeLaunchTime.ToString() + " " + this.LaunchTime.ToString() + " " + this.LaunchSpeedModifier.ToString() + " " + this.LaunchMaxSpeed.ToString() + " " + this.LaunchSpeed.ToString());
					this.LaunchTime -= Engine.DeltaTime;
					this.LaunchSpeed = Calc.Approach(this.LaunchSpeed, this.LaunchMaxSpeed, this.LaunchSpeedModifier);
					if (!this.Hold.IsHeld) {
						// this.Speed.Y += this.LaunchSpeedModifier * 0.7f;
						MoveV(-this.LaunchSpeed, (CollisionData e) => {
							if (e.Hit?.OnDashCollide == null) return;
							e.Hit.OnDashCollide(player, Vector2.UnitY * -1);
						});
					} else if (this.Hold.IsHeld && player != null && !player.Dead) {
						player.MoveV(-this.LaunchSpeed, (CollisionData e) => {
							if (e.Hit?.OnDashCollide == null) return;
							e.Hit.OnDashCollide(player, Vector2.UnitY * -1);
						});
						// player.Speed.Y += this.LaunchSpeedModifier;
					}
				} else {
					// explode
					// this.CollideCheck<>
				}
			}
			if (this.swatTimer > 0f) {
				this.swatTimer -= Engine.DeltaTime;
			}
			this.hardVerticalHitSoundCooldown -= Engine.DeltaTime;
			if (this.OnPedestal) {
				base.Depth = 8999;
				return;
			}
			base.Depth = 100;
			if (this.Hold.IsHeld) {
				this.prevLiftSpeed = Vector2.Zero;
			} else {
				if (base.OnGround(1)) {
					Vector2 liftSpeed = base.LiftSpeed;
					if (liftSpeed == Vector2.Zero && this.prevLiftSpeed != Vector2.Zero) {
						this.Speed = this.prevLiftSpeed;
						this.prevLiftSpeed = Vector2.Zero;
						this.Speed.Y = Math.Min(this.Speed.Y * 0.6f, 0f);
						if (this.Speed.X != 0f && this.Speed.Y == 0f) {
							this.Speed.Y = -60f;
						}
						if (this.Speed.Y < 0f) {
							this.noGravityTimer = 0.15f;
						}
					} else {
						this.prevLiftSpeed = liftSpeed;
						if (liftSpeed.Y < 0f && this.Speed.Y < 0f) {
							this.Speed.Y = 0f;
						}
					}
				} else if (this.Hold.ShouldHaveGravity) {
					float num = 800f;
					if (Math.Abs(this.Speed.Y) <= 30f) {
						num *= 0.5f;
					}
					float num2 = 350f;
					if (this.Speed.Y < 0f) {
						num2 *= 0.5f;
					}
					this.Speed.X = Calc.Approach(this.Speed.X, 0f, num2 * Engine.DeltaTime);
					if (this.noGravityTimer > 0f) {
						this.noGravityTimer -= Engine.DeltaTime;
					} else {
						this.Speed.Y = Calc.Approach(this.Speed.Y, 100f, num * Engine.DeltaTime);
					}
				}
				this.previousPosition = base.ExactPosition;
				base.MoveH(this.Speed.X * Engine.DeltaTime, this.onCollideH, null);
				base.MoveV(this.Speed.Y * Engine.DeltaTime, this.onCollideV, null);
				if (base.Center.X > (float)base.SceneAs<Level>().Bounds.Right) {
					base.MoveH(32f * Engine.DeltaTime, null, null);
					if (base.Left - 8f > (float)base.SceneAs<Level>().Bounds.Right) {
						base.RemoveSelf();
					}
				} else if (base.Left < (float)base.SceneAs<Level>().Bounds.Left) {
					base.Left = (float)base.SceneAs<Level>().Bounds.Left;
					this.Speed.X = this.Speed.X * -0.4f;
				} else if (base.Top < (float)(base.SceneAs<Level>().Bounds.Top - 4)) {
					base.Top = (float)(base.SceneAs<Level>().Bounds.Top + 4);
					this.Speed.Y = 0f;
				} else if (base.Bottom > (float)base.SceneAs<Level>().Bounds.Bottom && SaveData.Instance.Assists.Invincible) {
					base.Bottom = (float)base.SceneAs<Level>().Bounds.Bottom;
					this.Speed.Y = -300f;
					Audio.Play("event:/game/general/assist_screenbottom", this.Position);
				} else if (base.Top > (float)base.SceneAs<Level>().Bounds.Bottom) {
					this.RemoveSelf();
				} if (base.X < (float)(base.SceneAs<Level>().Bounds.Left + 10)) {
					base.MoveH(32f * Engine.DeltaTime, null, null);
				}
			}
			if (!this.dead) {
				this.Hold.CheckAgainstColliders();
			}
			if (this.hitSeeker != null && this.swatTimer <= 0f && !this.hitSeeker.Check(this.Hold)) {
				this.hitSeeker = null;
			}
		}

		public void ExplodeLaunch(Vector2 from) {
			if (!this.Hold.IsHeld) {
				this.Speed = (base.Center - from).SafeNormalize(120f);
				SlashFx.Burst(base.Center, this.Speed.Angle());
			}
		}

		public void Swat(HoldableCollider hc, int dir) {
			if (this.Hold.IsHeld && this.hitSeeker == null) {
				this.swatTimer = 0.1f;
				this.hitSeeker = hc;
				this.Hold.Holder.Swat(dir);
			}
		}

		public bool Dangerous(HoldableCollider holdableCollider) {
			return !this.Hold.IsHeld && this.Speed != Vector2.Zero && this.hitSeeker != holdableCollider;
		}

		public void HitSeeker(Seeker seeker) {
			if (!this.Hold.IsHeld) {
				this.Speed = (base.Center - seeker.Center).SafeNormalize(120f);
			}
			Audio.Play("event:/game/05_mirror_temple/crystaltheo_hit_side", this.Position);
		}

		public void HitSpinner(Entity spinner) {
			if (!this.Hold.IsHeld && this.Speed.Length() < 0.01f && base.LiftSpeed.Length() < 0.01f && (this.previousPosition - base.ExactPosition).Length() < 0.01f && base.OnGround(1)) {
				int num = Math.Sign(base.X - spinner.X);
				if (num == 0) {
					num = 1;
				}
				this.Speed.X = (float)num * 120f;
				this.Speed.Y = -30f;
			}
		}

		public bool HitSpring(Spring spring) {
			if (!this.Hold.IsHeld) {
				if (spring.Orientation == Spring.Orientations.Floor && this.Speed.Y >= 0f) {
					this.Speed.X = this.Speed.X * 0.5f;
					this.Speed.Y = -160f;
					this.noGravityTimer = 0.15f;
					return true;
				}
				if (spring.Orientation == Spring.Orientations.WallLeft && this.Speed.X <= 0f) {
					base.MoveTowardsY(spring.CenterY + 5f, 4f, null);
					this.Speed.X = 220f;
					this.Speed.Y = -80f;
					this.noGravityTimer = 0.1f;
					return true;
				}
				if (spring.Orientation == Spring.Orientations.WallRight && this.Speed.X >= 0f) {
					base.MoveTowardsY(spring.CenterY + 5f, 4f, null);
					this.Speed.X = -220f;
					this.Speed.Y = -80f;
					this.noGravityTimer = 0.1f;
					return true;
				}
			}
			return false;
		}

		private void OnCollideH(CollisionData data) {
			if (data.Hit is DashSwitch) {
				(data.Hit as DashSwitch).OnDashCollide(null, Vector2.UnitX * (float)Math.Sign(this.Speed.X));
			}
			Audio.Play("event:/game/05_mirror_temple/crystaltheo_hit_side", this.Position);
			if (Math.Abs(this.Speed.X) > 100f) {
				this.ImpactParticles(data.Direction);
			}
			this.Speed.X = this.Speed.X * -0.4f;
		}

		private void OnCollideV(CollisionData data) {
			if (data.Hit is DashSwitch) {
				(data.Hit as DashSwitch).OnDashCollide(null, Vector2.UnitY * (float)Math.Sign(this.Speed.Y));
			}
			if (this.Speed.Y > 0f) {
				if (this.hardVerticalHitSoundCooldown <= 0f) {
					Audio.Play("event:/game/05_mirror_temple/crystaltheo_hit_ground", this.Position, "crystal_velocity", Calc.ClampedMap(this.Speed.Y, 0f, 200f, 0f, 1f));
					this.hardVerticalHitSoundCooldown = 0.5f;
				} else {
					Audio.Play("event:/game/05_mirror_temple/crystaltheo_hit_ground", this.Position, "crystal_velocity", 0f);
				}
			}
			if (this.Speed.Y > 160f) {
				this.ImpactParticles(data.Direction);
			}
			if (this.Speed.Y > 140f && !(data.Hit is SwapBlock) && !(data.Hit is DashSwitch)) {
				this.Speed.Y = this.Speed.Y * -0.6f;
				return;
			}
			this.Speed.Y = 0f;
		}

		private void ImpactParticles(Vector2 dir) {
			float direction;
			Vector2 position;
			Vector2 positionRange;
			if (dir.X > 0f) {
				direction = 3.1415927f;
				position = new Vector2(base.Right, base.Y - 4f);
				positionRange = Vector2.UnitY * 6f;
			} else if (dir.X < 0f) {
				direction = 0f;
				position = new Vector2(base.Left, base.Y - 4f);
				positionRange = Vector2.UnitY * 6f;
			} else if (dir.Y > 0f) {
				direction = -1.5707964f;
				position = new Vector2(base.X, base.Bottom);
				positionRange = Vector2.UnitX * 6f;
			} else {
				direction = 1.5707964f;
				position = new Vector2(base.X, base.Top);
				positionRange = Vector2.UnitX * 6f;
			}
			base.SceneAs<Level>().Particles.Emit(SNYFireworkHoldable.P_Impact, 12, position, positionRange, direction);
		}

		public override bool IsRiding(Solid solid) {
			return this.Speed.Y == 0f && base.IsRiding(solid);
		}

		private void OnPickup() {
			this.Speed = Vector2.Zero;
			base.AddTag(Tags.Persistent);
		}

		private bool isLaunching() {
			return this.LaunchTime >= 0 && this.BeforeLaunchTime < 0;
		}

		private void OnRelease(Vector2 force) {
			base.RemoveTag(Tags.Persistent);
			if (force.X != 0f && force.Y == 0f) {
				force.Y = isLaunching() ? -0.05f : -0.4f;
			}
			this.Speed = force * (isLaunching() ? 100f : 200f);
			if (this.Speed != Vector2.Zero) {
				this.noGravityTimer = 0.1f;
			}
		}
		public static ParticleType P_Impact = new ParticleType {
			Color = Calc.HexToColor("eccc15"),
			Size = 1f,
			FadeMode = ParticleType.FadeModes.Late,
			DirectionRange = 1.7453293f,
			SpeedMin = 10f,
			SpeedMax = 20f,
			SpeedMultiplier = 0.1f,
			LifeMin = 0.3f,
			LifeMax = 0.8f
		};
		public static ParticleType P_LaunchingA = new ParticleType {
			Color = Color.Red,
			Size = 1f,
			FadeMode = ParticleType.FadeModes.Late,
			DirectionRange = 1.7453293f / 2f,
			SpeedMin = 90f,
			SpeedMax = 130f,
			SpeedMultiplier = 0.5f,
			LifeMin = 0.4f,
			LifeMax = 0.7f
		};
		public static ParticleType P_LaunchingB = new ParticleType {
			Color = Color.Gold,
			Size = 1f,
			FadeMode = ParticleType.FadeModes.Late,
			DirectionRange = 1.7453293f / 2f,
			SpeedMin = 90f,
			SpeedMax = 130f,
			SpeedMultiplier = 0.5f,
			LifeMin = 0.4f,
			LifeMax = 0.7f
		};

		public Vector2 Speed;

		public bool OnPedestal;

		public Holdable Hold;

		private Collider lightCollider;

		private Sprite sprite, lightSprite;

		private bool dead;

		private bool launching = false;

		private Level Level;

		private Collision onCollideH;

		private Collision onCollideV;
		private float noGravityTimer;

		private Vector2 prevLiftSpeed;

		private Vector2 previousPosition;

		private HoldableCollider hitSeeker;

		private float swatTimer;

		private bool shattering;

		private float hardVerticalHitSoundCooldown;

		private BirdTutorialGui tutorialGui;

		private float tutorialTimer;
	}
}
