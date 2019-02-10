using GameTimer;
using MenuBuddy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleBuddy;
using System;

namespace InputParticlesComponent
{
	/// <summary>
	/// THis is a game component that adds particle effects whenever the user clicks a UI element
	/// </summary>
	public class InputParticlesComponent : DrawableGameComponent
	{
		#region Properties

		/// <summary>
		/// The input component that listens for click events
		/// </summary>
		private IInputHandler InputHandler { get; set; }

		private IScreenManager ScreenManager { get; set; }

		/// <summary>
		/// The clock used to time this game
		/// </summary>
		private GameClock Clock { get; set; }

		/// <summary>
		/// The particle engine for this game
		/// </summary>
		private ParticleEngine ParticleEngine { get; set; }

		/// <summary>
		/// The particle effect to play whenever the user clicks on the screen
		/// </summary>
		private EmitterTemplate ClickEmitter { get; set; }

		private string ParticleTexture { get; set; }

		#endregion Properties

		#region Methods

		public InputParticlesComponent(Game game, string particleTexture) : base(game)
		{
			game.Components.Add(this);
			this.DrawOrder = 1000;
			ParticleTexture = particleTexture;
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			//get the other components we need
			InputHandler = Game.Services.GetService<IInputHandler>();
			ScreenManager = Game.Services.GetService<IScreenManager>();

			if (null == InputHandler || null == ScreenManager)
			{
				throw new Exception("InputParticlesComponent initialized in the wrong order");
			}

			//Create the particle engine
			Clock = new GameClock();
			ParticleEngine = new ParticleEngine();

			//create the click emitter particle effect
			ClickEmitter = new EmitterTemplate()
			{
				ParticleSize = 32,
				MinSpin = -5f,
				MaxSpin = 5f,
				MaxScale = 100f,
				MinScale = -100f,
				ParticleGravity = 500f,
				MaxParticleVelocity = new Vector2(200f, -300f),
				MinParticleVelocity = new Vector2(-200f, 100f),
				FadeSpeed = 2f,
				Texture = Game.Content.Load<Texture2D>(ParticleTexture),
				EmitterLife = 0f,
			};

			InputHandler.OnClickHandled += ((onj, e) =>
			{
				//Create a particle effect
				ParticleEngine.PlayParticleEffect(ClickEmitter, Vector2.Zero, e.Position, Vector2.Zero, Color.White, false);
			});
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			Clock.Update(gameTime);
			ParticleEngine.Update(Clock);
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			//draw all the current particle effects
			ScreenManager.SpriteBatchBegin();
			ParticleEngine.Render(ScreenManager.SpriteBatch);
			ScreenManager.SpriteBatchEnd();
		}

		#endregion Methods
	}
}
