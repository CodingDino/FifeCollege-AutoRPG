using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace AutoRPG
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        SpriteFont gameFont;
        int level = 1;
        int HP = 150;
        int XP = 0;
        int XPGain = 10; // from killing 1 slime
        int XPGainFrequency = 200; // in miliseconds
        TimeSpan lastXPGain; // last time we killed a slime
        int XPToNextLevel = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            gameFont = Content.Load<SpriteFont>("arial");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here


            // Increase XP (kill slimes)
            // Is it time to kill a slime?
            TimeSpan timeSinceLastSlimeKilled = gameTime.TotalGameTime - lastXPGain;
            if (timeSinceLastSlimeKilled.TotalMilliseconds >= XPGainFrequency)
            {
                // It is time to kill a slime!
                // Gain some XP
                XP += XPGain;
                // Record the time we gained it
                lastXPGain = gameTime.TotalGameTime;
            }

            // Calculate XP needed for next level
            // using a quadratic function for levels 1-10
            // y = a * x ^2 + b * x + c
            //     where a = 40     b = 360    c = 0
            // and a cubic function for levels 11+
            // y = a * x^3 + b * x^2 + c * x + d
            //     where a = -0.4    b = 40.4   c = 396.0    d = 0
            if (level <= 10)
                XPToNextLevel = 40 * (int)Math.Pow(level, 2) + 360 * level + 0;
            else
                XPToNextLevel = (int)(-0.4 * Math.Pow(level, 3) + 40.4 * Math.Pow(level, 2) + 396.0 * (double)level + 0);

            if (XP >= XPToNextLevel)
            {
                // Level up!
                ++level;

                // Calculate HP based on level
                // using a linear function
                // y = m * x + c
                HP = 50 * level + 100;
            }
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin();

            // Draw the level
            spriteBatch.DrawString(
                gameFont, 
                "Level: " + level.ToString(), 
                new Vector2(10f, 10f),
                Color.White
            );

            // Draw HP
            spriteBatch.DrawString(
                gameFont,
                "HP: " + HP.ToString(),
                new Vector2(10f, 40f),
                Color.White
            );

            // Draw XP
            spriteBatch.DrawString(
                gameFont,
                "XP: " + XP.ToString(),
                new Vector2(10f, 70f),
                Color.White
            );

            // Draw XP to next level
            spriteBatch.DrawString(
                gameFont,
                "XP To Next Level: " + XPToNextLevel.ToString(),
                new Vector2(10f, 100f),
                Color.White
            );


            // Draw rectangle for HP
            Texture2D rect = new Texture2D(graphics.GraphicsDevice, HP, 30);
            Color[] data = new Color[HP * 30];
            for (int i = 0; i < data.Length; ++i)
                data[i] = Color.Green;
            rect.SetData(data);
            spriteBatch.Draw(rect, new Vector2(10f, 130f), Color.White);




            spriteBatch.End();


            base.Draw(gameTime);
        }

        private double LinearInterpolate(
            double x0, 
            double x1, 
            double y0, 
            double y1, 
            double x)
        {
            // Given a linear x and y equation such as:
            //    y = m*x + c

            // This function will find the y for a given x
            // needing only two points on the line

            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }


    }


}
