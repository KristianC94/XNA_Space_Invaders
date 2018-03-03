using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SpaceInvaders
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class SpaceInvaders : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D Background, Invader, Gunship, Missile;

        SpriteFont Font1, Font2, Font3;

        Rectangle[,] InvaderArray;
        Rectangle MissileHitbox, ShipHitbox;

        String[,] InvaderLiving;

        Missile ShotsFired;

        int InvaderSpd, ShipXPos, Score, GameState;

        double Ticks;

        string InvaderDir = "Right";

        public SpaceInvaders()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 650;
            graphics.PreferredBackBufferWidth = 900;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        
        // INITIALISATION CODE
        protected override void Initialize()
        {
            InitialiseGameVariables();

            GameState = 1;

            base.Initialize();
        }
        
        public void InitialiseGameVariables()
        {
            Ticks = 0;
            InvaderSpd = 20;
            ShipXPos = 400;
            ShotsFired = null;
            Score = 0;

            InvaderArray = new Rectangle[5, 11];
            InvaderLiving = new String[5, 11];
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // CONTENT LOAD
            Background = Content.Load<Texture2D>("Background");
            Invader = Content.Load<Texture2D>("Invader");
            Gunship = Content.Load<Texture2D>("Gunship");
            Missile = Content.Load<Texture2D>("Missile");
            Font1 = Content.Load<SpriteFont>("Font1");
            Font2 = Content.Load<SpriteFont>("Font2");
            Font3 = Content.Load<SpriteFont>("Font3");

            for (int rows = 0; rows < 5; rows++)
                for (int cols = 0; cols < 11; cols++)
                {
                    InvaderArray[rows, cols].Width = Invader.Width;
                    InvaderArray[rows, cols].Height = Invader.Height;
                    InvaderArray[rows, cols].X = 60 * cols + 120;
                    InvaderArray[rows, cols].Y = 60 * rows + 50;
                    InvaderLiving[rows, cols] = "Yes";
                }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
        
        // UPDATE CODE
        public void UpdateStarted(GameTime currentTime) // Start screen
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                GameState = 2;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.C))
            {
                GameState = 4;
            }
        }

        public void UpdatePlaying(GameTime currentTime) // Main gameplay
        {
            Ticks = Ticks + currentTime.ElapsedGameTime.TotalMilliseconds;

            // Invader movement
            if (Ticks > 500)
            {

                // Invader sideways movement
                for (int rows = 0; rows < 5; rows++)
                    for (int cols = 0; cols < 11; cols++)
                    {
                        if (InvaderDir.Equals("Right"))
                            InvaderArray[rows, cols].X = InvaderArray[rows, cols].X + InvaderSpd;
                        if (InvaderDir.Equals("Left"))
                            InvaderArray[rows, cols].X = InvaderArray[rows, cols].X - InvaderSpd;
                    }

                // Invader Limits
                string ChangeDir = "No";
                for (int rows = 0; rows < 5; rows++)
                    for (int cols = 0; cols < 11; cols++)
                    {
                        if (InvaderLiving[rows, cols].Equals("Yes"))
                        {
                            if (InvaderArray[rows, cols].X + InvaderArray[rows, cols].Width > 870)
                            {
                                InvaderDir = "Left";
                                ChangeDir = "Yes";
                            }

                            if (InvaderArray[rows, cols].X < 30)
                            {
                                InvaderDir = "Right";
                                ChangeDir = "Yes";
                            }
                        }
                    }

                // Invader downward movement
                if (ChangeDir.Equals("Yes"))
                {
                    for (int rows = 0; rows < 5; rows++)
                        for (int cols = 0; cols < 11; cols++)
                        {
                            InvaderArray[rows, cols].Y = InvaderArray[rows, cols].Y + 8;
                        }
                }


                Ticks = 0;
            }

            // Gunship control
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                ShipXPos = ShipXPos + 3;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                ShipXPos = ShipXPos - 3;
            }

            //Gunship limits
            if (ShipXPos < 15)
            {
                ShipXPos = 15;
            }

            if (ShipXPos > 780)
            {
                ShipXPos = 780;
            }

            // Firing missile
            if (ShotsFired != null) // Moves missile when it is visible
            {
                ShotsFired.Move();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space) & ShotsFired == null) // Fires missile when space is pressed
            {
                ShotsFired = new Missile(ShipXPos + 45, 565);
            }

            if (ShotsFired != null) // Deletes the missile when it passes the top of the screen
            {
                if (ShotsFired.GetMissilePos().Y < 0)
                {
                    ShotsFired = null;
                }
            }

            // Missile/invader collision detection
            if (ShotsFired != null)
            {
                MissileHitbox = new Rectangle((int)ShotsFired.GetMissilePos().X, (int)ShotsFired.GetMissilePos().Y,
                    Missile.Width, Missile.Height);

                for (int rows = 0; rows < 5; rows++)
                    for (int cols = 0; cols < 11; cols++)
                    {
                        if (InvaderLiving[rows, cols].Equals("Yes"))
                        {
                            if (MissileHitbox.Intersects(InvaderArray[rows, cols]))
                            {
                                ShotsFired = null;
                                InvaderLiving[rows, cols] = "No";
                                Score = Score + 50;
                            }
                        }
                    }
            }

            // Gunship/Invader collision detection
            ShipHitbox = new Rectangle(ShipXPos, 565, Gunship.Width, Gunship.Height);

            for (int rows = 0; rows < 5; rows++)
                for (int cols = 0; cols < 11; cols++)
                {
                    if (InvaderLiving[rows, cols].Equals("Yes"))
                    {
                        if (InvaderArray[rows, cols].Y + InvaderArray[rows, cols].Height > ShipHitbox.Y)
                        {
                            GameState = 3;
                        }
                    }
                }

            // Speeds up invaders when they reach a certain point
            for (int rows = 0; rows < 5; rows++)
                for (int cols = 0; cols < 11; cols++)
                {
                    if (InvaderLiving[rows, cols].Equals("Yes"))
                    {
                        if (InvaderArray[rows, cols].Y > 350)
                        {
                            InvaderSpd = 30;
                        }
                    }
                }

            int Count = 0;
            for (int rows = 0; rows < 5; rows++)
                for (int cols = 0; cols < 11; cols++)
                {
                    if (InvaderLiving[rows, cols].Equals("Yes"))
                    {
                        Count = Count + 1;
                    }
                }
                     
            // Finishes when all invaders killed
            if (Count == 0)
            {
                this.Exit();
            }
             
        }
                
        public void UpdateEndedBad(GameTime currentTime) // If player is defeated
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }
        }

        public void UpdateControls(GameTime currentTime) // Controls screen
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Back))
            {
                GameState = 1;
            }
        }

        public void UpdateEndedGood(GameTime currentTime) // If player kills all invaders
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }
        }

        protected override void Update(GameTime gameTime)
        {
            switch (GameState)
            {
                case 1: UpdateStarted(gameTime);
                    break;

                case 2: UpdatePlaying(gameTime);
                    break;

                case 3: UpdateEndedBad(gameTime);
                    break;

                case 4: UpdateControls(gameTime);
                    break;

                case 5: UpdateEndedGood(gameTime);
                    break;
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
       
        // DRAWING CODE
        public void DrawStarted(GameTime gameTime) // Start screen
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            spriteBatch.Draw(Background, Vector2.Zero, Color.White);

            Vector2 StringDimensions = Font2.MeasureString("SPACE");

            int XPos = (900 - (int)StringDimensions.X) / 2;
            spriteBatch.DrawString(Font2, "SPACE", new Vector2(XPos, 100), Color.LimeGreen);

            StringDimensions = Font2.MeasureString("INVADERS");

            XPos = (900 - (int)StringDimensions.X) / 2;
            spriteBatch.DrawString(Font2, "INVADERS", new Vector2(XPos, 230), Color.LimeGreen);

            StringDimensions = Font1.MeasureString("PRESS ENTER TO START GAME");

            XPos = (900 - (int)StringDimensions.X) / 2;
            spriteBatch.DrawString(Font1, "PRESS ENTER TO START GAME", new Vector2(XPos, 350), Color.LimeGreen);

            StringDimensions = Font1.MeasureString("PRESS 'C' TO VIEW CONTROLS");

            XPos = (900 - (int)StringDimensions.X) / 2;
            spriteBatch.DrawString(Font1, "PRESS 'C' TO VIEW CONTROLS", new Vector2(XPos, 385), Color.LimeGreen);

            StringDimensions = Font3.MeasureString("Free for public use Earth photo, credit: NASA");

            XPos = (900 - (int)StringDimensions.X) / 2;
            spriteBatch.DrawString(Font3, "Free for public use Earth photo, credit: NASA", new Vector2(XPos, 630), Color.White);

            spriteBatch.End();
        }

        public void DrawPlaying(GameTime currentTime) // Main game screen
        {
            spriteBatch.Begin();

            // Background
            spriteBatch.Draw(Background, Vector2.Zero, Color.White);

            // Invaders
            for (int rows = 0; rows < 5; rows++)
                for (int cols = 0; cols < 11; cols++)
                {
                    if (InvaderLiving[rows, cols].Equals("Yes"))
                    {
                        spriteBatch.Draw(Invader, InvaderArray[rows, cols], Color.White);
                    }
                }

            // Gunship
            spriteBatch.Draw(Gunship, new Vector2(ShipXPos, 565), Color.White);

            // Missile
            if (ShotsFired != null)
            {
                Vector2 MissileStartPos = new Vector2(ShotsFired.GetMissilePos().X, ShotsFired.GetMissilePos().Y - Missile.Height);
                spriteBatch.Draw(Missile, MissileStartPos, Color.White);
            }

            // Score
            string ScoreText = String.Format("Score: {0}", Score);
            spriteBatch.DrawString(Font1, ScoreText, new Vector2(772, 3), Color.LimeGreen);

            spriteBatch.End();
        }

        public void DrawEndedBad(GameTime currentTime) // Loser screen
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            spriteBatch.Draw(Background, Vector2.Zero, Color.White);

            string FinalScore = String.Format("Final score = {0}", Score);

            Vector2 StringDimensions = Font1.MeasureString(FinalScore);

            int XPos = (900 - (int)StringDimensions.X) / 2;
            spriteBatch.DrawString(Font1, FinalScore, new Vector2(XPos, 250), Color.LimeGreen);

            StringDimensions = Font1.MeasureString("PRESS 'ESC' TO EXIT GAME");

            XPos = (900 - (int)StringDimensions.X) / 2;
            spriteBatch.DrawString(Font1, "PRESS 'ESC' TO EXIT GAME", new Vector2(XPos, 350), Color.LimeGreen);

            StringDimensions = Font2.MeasureString("YOU DIED");

            XPos = (900 - (int)StringDimensions.X) / 2;
            spriteBatch.DrawString(Font2, "YOU DIED", new Vector2(XPos, 140), Color.LimeGreen);

            StringDimensions = Font3.MeasureString("Free for public use Earth photo, credit: NASA");

            XPos = (900 - (int)StringDimensions.X) / 2;
            spriteBatch.DrawString(Font3, "Free for public use Earth photo, credit: NASA", new Vector2(XPos, 630), Color.White);

            spriteBatch.End();
        }

        public void DrawControls(GameTime currentTime) // Controls screen
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            spriteBatch.Draw(Background, Vector2.Zero, Color.White);

            Vector2 StringDimensions = Font1.MeasureString("LEFT ARROW KEY = MOVE LEFT");

            int XPos = (900 - (int)StringDimensions.X) / 2;
            spriteBatch.DrawString(Font1, "LEFT ARROW KEY = MOVE LEFT", new Vector2(XPos, 200), Color.LimeGreen);

            StringDimensions = Font1.MeasureString("RIGHT ARROW KEY = MOVE RIGHT");

            XPos = (900 - (int)StringDimensions.X) / 2;
            spriteBatch.DrawString(Font1, "RIGHT ARROW KEY = MOVE RIGHT", new Vector2(XPos, 250), Color.LimeGreen);

            StringDimensions = Font1.MeasureString("SPACEBAR = SHOOT");

            XPos = (900 - (int)StringDimensions.X) / 2;
            spriteBatch.DrawString(Font1, "SPACEBAR = SHOOT", new Vector2(XPos, 300), Color.LimeGreen);

            StringDimensions = Font2.MeasureString("CONTROLS");

            XPos = (900 - (int)StringDimensions.X) / 2;
            spriteBatch.DrawString(Font2, "CONTROLS", new Vector2(XPos, 90), Color.LimeGreen);

            StringDimensions = Font1.MeasureString("PRESS BACKSPACE TO RETURN TO START SCREEN");

            XPos = (900 - (int)StringDimensions.X) / 2;
            spriteBatch.DrawString(Font1, "PRESS BACKSPACE TO RETURN TO START SCREEN", new Vector2(XPos, 400), Color.LimeGreen);

            StringDimensions = Font3.MeasureString("Free for public use Earth photo, credit: NASA");

            XPos = (900 - (int)StringDimensions.X) / 2;
            spriteBatch.DrawString(Font3, "Free for public use Earth photo, credit: NASA", new Vector2(XPos, 630), Color.White);

            spriteBatch.End();
        }

        public void DrawEndedGood(GameTime currentTime) // Winner screen
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            spriteBatch.Draw(Background, Vector2.Zero, Color.White);

            string FinalScore = String.Format("Final score = {0}", Score);

            Vector2 StringDimensions = Font1.MeasureString(FinalScore);

            int XPos = (900 - (int)StringDimensions.X) / 2;
            spriteBatch.DrawString(Font1, FinalScore, new Vector2(XPos, 250), Color.LimeGreen);

            StringDimensions = Font1.MeasureString("PRESS 'ESC' TO EXIT GAME");

            XPos = (900 - (int)StringDimensions.X) / 2;
            spriteBatch.DrawString(Font1, "PRESS 'ESC' TO EXIT GAME", new Vector2(XPos, 350), Color.LimeGreen);

            StringDimensions = Font2.MeasureString("YOU WIN");

            XPos = (900 - (int)StringDimensions.X) / 2;
            spriteBatch.DrawString(Font2, "YOU WIN", new Vector2(XPos, 140), Color.LimeGreen);

            StringDimensions = Font3.MeasureString("Free for public use Earth photo, credit: NASA");

            XPos = (900 - (int)StringDimensions.X) / 2;
            spriteBatch.DrawString(Font3, "Free for public use Earth photo, credit: NASA", new Vector2(XPos, 630), Color.White);

            spriteBatch.End();
        }

        protected override void Draw(GameTime gameTime)
        {
            switch (GameState)
            {
                case 1: DrawStarted(gameTime);
                    break;

                case 2: DrawPlaying(gameTime);
                    break;

                case 3: DrawEndedBad(gameTime);
                    break;

                case 4: DrawControls(gameTime);
                    break;

                case 5: DrawEndedGood(gameTime);
                    break;
            }

            base.Draw(gameTime);
        }
    }
}