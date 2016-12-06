using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.Xml.Serialization;
using System.IO;

namespace GameProject
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        Burger burger;
        List<TeddyBear> bears = new List<TeddyBear>();
        static List<Projectile> projectiles = new List<Projectile>();
        List<Explosion> explosions = new List<Explosion>();

        // projectile and explosion sprites. Saved so they don't have to
        // be loaded every time projectiles or explosions are created
        static Texture2D frenchFriesSprite;
        static Texture2D teddyBearProjectileSprite;
        static Texture2D explosionSpriteStrip;

        //possible click and keyboard support

        string text;
        Keys[] keysToCheck 
            = new Keys[] {
                          Keys.A, Keys.B, Keys.C, Keys.D, Keys.E,
                          Keys.F, Keys.G, Keys.H, Keys.I, Keys.J,
                          Keys.K, Keys.L, Keys.M, Keys.N, Keys.O,
                          Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T,
                          Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y,
                          Keys.Z, Keys.Back, Keys.Space };

        KeyboardState currentKeyboardState;
        KeyboardState lastKeyboardState;

        //random c support
        //Random rand = new Random();
        List<Texture2D> charectersList = new List<Texture2D>();

        // scoring support
        static int score = 0;
        string scoreString;
        ScoreTable scoreTable;

        // health support
        string healthString = GameConstants.HealthPrefix +
            GameConstants.BurgerInitialHealth;
        bool burgerDead = false;
        string gameOver;

        // text display support
        SpriteFont font;
        SpriteFont fontGameOver;
        List<Message> messages;
        Message healthMessage;
        Message scoreMessage;
        Message eventMessage;

        // sound effects
        SoundEffect fon;
        SoundEffect burgerDamage;
        SoundEffect burgerDeath;
        SoundEffect burgerShot;
        SoundEffect explosion;
        SoundEffect teddyBounce;
        SoundEffect teddyShot;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // set resolution
            graphics.PreferredBackBufferWidth = GameConstants.WindowWidth;
            graphics.PreferredBackBufferHeight = GameConstants.WindowHeight;
           // IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            RandomNumberGenerator.Initialize();
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

            // load audio content            
            burgerDamage = Content.Load<SoundEffect>(@"audio\BurgerDamage");
            
            // make below soubd one-play in the loop
            burgerDeath = Content.Load<SoundEffect>(@"audio\BurgerDeath");

            fon = Content.Load<SoundEffect>(@"audio\Fon4");
            SoundEffectInstance fonMusic = fon.CreateInstance();
            fonMusic.IsLooped = true;
            fonMusic.Volume = 1;
            fonMusic.Play();

            burgerShot = Content.Load<SoundEffect>(@"audio\BurgerShot");
            explosion = Content.Load<SoundEffect>(@"audio\Explosion");
            teddyBounce = Content.Load<SoundEffect>(@"audio\TeddyBounce");
            teddyShot = Content.Load<SoundEffect>(@"audio\bang3");

            // load sprite font
            font = Content.Load<SpriteFont>(@"fonts\Arial20");
            fontGameOver = Content.Load<SpriteFont>(@"fonts\Arial30");

            // score string and m
            messages = new List<Message>();
            scoreString = GameConstants.ScorePrefix + score;
            gameOver = "GAME OVER";

            // load projectile and explosion sprites
            frenchFriesSprite = Content.Load<Texture2D>(@"graphics\frenchfries");
            teddyBearProjectileSprite = Content.Load<Texture2D>(@"graphics\teddybearprojectile");
            explosionSpriteStrip = Content.Load<Texture2D>(@"graphics\explosion");

            //start in the ceenter as exmple
            //Rectangle drawRectangle = new Rectangle(GameConstants.WindowWidth / 2 - frenchFriesSprite.Width / 2, GameConstants.WindowHeight / 2 - frenchFriesSprite.Height / 2, frenchFriesSprite.Width, frenchFriesSprite.Height);

            // add initial game objects
            burger = new Burger(Content, @"graphics\burger", GameConstants.WindowWidth / 2, GameConstants.WindowHeight - GameConstants.WindowHeight / 8, burgerShot);
            for (int i = 0; i < GameConstants.MaxBears; i++)
            {
                SpawnBear();
            }
            // set initial health and score strings
            //adding health string
            healthMessage = new Message(healthString,font, GameConstants.HealthLocation);
            scoreMessage = new Message(scoreString, font, GameConstants.ScoreLocation);
            messages.AddRange(new Message []{ healthMessage, scoreMessage});

            // score xml ser init
            scoreTable = new ScoreTable();
            scoreTable.ScoreList = new List<ScoreValues>();
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
            // get current mouse\keyboard state and update burger
            KeyboardState currentKeyboardState = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();
            if (burger!=null)
            burger.Update(gameTime, mouse, currentKeyboardState);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.Escape))
                Exit();

            //


            // update other game objects
            if (bears.Any())
            {
                foreach (TeddyBear bear in bears)
                {
                    bear.Update(gameTime);
                }
            }
            foreach (Projectile projectile in projectiles)
            {
                projectile.Update(gameTime);
            }
            foreach (Explosion explosion in explosions)
            {
                explosion.Update(gameTime);
            }

            // check and resolve collisions between teddy bears
            if (bears.Any() && burger != null)
            {
                for (int i = 0; i < bears.Count; i++)
                {
                    for (int j = i + 1; j < bears.Count; j++)
                    {
                        if (bears[i].Active && bears[j].Active)
                        {
                            CollisionResolutionInfo collisitionResInfo = CollisionUtils.CheckCollision(gameTime.ElapsedGameTime.Milliseconds,
                                GameConstants.WindowWidth, GameConstants.WindowHeight,
                                bears[i].Velocity, bears[i].DrawRectangle, bears[j].Velocity, bears[j].DrawRectangle);
                            if (collisitionResInfo != null)
                            {
                                // 1 bear check and bounce if needed
                                if (collisitionResInfo.FirstOutOfBounds)
                                    bears[i].Active = false;
                                else
                                {
                                    bears[i].Velocity = collisitionResInfo.FirstVelocity;
                                    bears[i].DrawRectangle = collisitionResInfo.FirstDrawRectangle;
                                    teddyBounce.Play();
                                }

                                // 2 bear check and bounce if needed
                                if (collisitionResInfo.SecondOutOfBounds)
                                    bears[j].Active = false;
                                else
                                {
                                    bears[j].Velocity = collisitionResInfo.SecondVelocity;
                                    bears[j].DrawRectangle = collisitionResInfo.SecondDrawRectangle;
                                    teddyBounce.Play();
                                }
                            }
                        }
                    }
                }
            }


            //a collision between the burger and each projectile 
            if (bears.Any() && burger != null)
            {
                foreach (Projectile projectile in projectiles)
                {
                    if (projectile.Type == ProjectileType.TeddyBear
                        && burger.CollisionRectangle.Intersects(projectile.CollisionRectangle)
                         && projectile.Active && !burgerDead)
                    {
                        projectile.Active = false;
                        explosions.Add(new Explosion(explosionSpriteStrip, burger.CollisionRectangle.Center.X, burger.CollisionRectangle.Center.Y));
                        explosion.Play();
                        burger.Health -= GameConstants.TeddyBearProjectileDamage;
                        messages.Remove(healthMessage);
                        healthMessage = new Message(GameConstants.HealthPrefix + burger.Health, font, GameConstants.HealthLocation);
                        messages.Add(healthMessage);
                        CheckBurgerKill();
                    }
                }
            }
            // bear collisisons check
            if (bears.Any()&& burger != null)
            {
                for (int i = 0; i < bears.Count; i++)
                {
                    if (bears[i].Active)
                    {
                        // check and resolve collisions between burger and teddy bears
                        if (burger.CollisionRectangle.Intersects(bears[i].CollisionRectangle) && !burgerDead)
                        {
                            score++;
                            burger.Health -= GameConstants.BearDamage;
                            messages.Clear();
                            healthMessage = new Message(GameConstants.HealthPrefix + burger.Health, font, GameConstants.HealthLocation);
                            scoreMessage = new Message(GetScoreString(score), font, GameConstants.ScoreLocation);
                            messages.AddRange(new Message[] { healthMessage, scoreMessage });
                            bears[i].Active = false;
                            explosions.Add(new Explosion(explosionSpriteStrip, bears[i].Location.X, bears[i].Location.Y));
                            explosion.Play();
                            CheckBurgerKill();
                        }

                        // check and resolve collisions between teddy bears and projectiles
                        else
                        {
                            foreach (Projectile projectile in projectiles)
                            {
                                if (projectile.Type == ProjectileType.FrenchFries
                                    && bears[i].DrawRectangle.Intersects(projectile.CollisionRectangle)
                                    && bears[i].Active && projectile.Active)
                                {
                                    score += GameConstants.BearPoints;
                                    messages.Remove(scoreMessage);
                                    GetScoreString(score);
                                    scoreMessage = new Message(GetScoreString(score), font, GameConstants.ScoreLocation);
                                    messages.Add(scoreMessage);
                                    bears[i].Active = false;
                                    projectile.Active = false;
                                    explosions.Add(new Explosion(explosionSpriteStrip, bears[i].Location.X, bears[i].Location.Y));
                                    explosion.Play();
                                }
                            }
                        }
                    }
                }
            }

            // clean out inactive teddy bears and add new ones as necessary
            for (int i = bears.Count - 1; i >= 0; i--)
            {
                if (!bears[i].Active)
                {
                    bears.RemoveAt(i);
                }
            }

            // adding bear to max value
            while (bears.Count<GameConstants.MaxBears&&burger!=null)
            {
                SpawnBear();
            }

            // clean out inactive projectiles
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                if (!projectiles[i].Active)
                {
                    projectiles.RemoveAt(i);
                }
            }

            // clean out finished explosions
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                if (explosions[i].Finished)
                {
                    explosions.Remove(explosions[i]);
                }
            }

            lastKeyboardState = currentKeyboardState;
            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightSlateGray);

            spriteBatch.Begin();

            // draw game objects
            if (burger!=null)
            burger.Draw(spriteBatch);

            if (bears.Any())
            {
                foreach (TeddyBear bear in bears)
                {
                    bear.Draw(spriteBatch);
                }
            }

            foreach (Projectile projectile in projectiles)
            {
                projectile.Draw(spriteBatch);
            }
            foreach (Explosion explosion in explosions)
            {
                explosion.Draw(spriteBatch);
            }

            //   draw score, health, g/o
            foreach (Message message in messages)
            {
                if (!burgerDead)
                    message.Draw(spriteBatch, Color.Wheat);                
            }
            if (burgerDead)
            {
                eventMessage.Draw(spriteBatch, Color.Red);
                foreach (Message message in messages)
                    message.Draw(spriteBatch, Color.Beige);

            }


            //alt way
            //spriteBatch.DrawString(font,scoreString, GameConstants.ScoreLocation, Color.Wheat);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #region Public methods

        /// <summary>
        /// Gets the projectile sprite for the given projectile type
        /// </summary>
        /// <param name="type">the projectile type</param>
        /// <returns>the projectile sprite for the type</returns>
        public static Texture2D GetProjectileSprite(ProjectileType type)
        {
            // replace with code to return correct projectile sprite based on projectile type
            if (type == ProjectileType.FrenchFries)
                return frenchFriesSprite;
            else                                    //(type == ProjectileType.TeddyBear)
                return teddyBearProjectileSprite;
        }

        /// <summary>
        /// Adds the given projectile to the game
        /// </summary>
        /// <param name="projectile">the projectile to add</param>
        public static void AddProjectile(Projectile projectile)
        {
            projectiles.Add(projectile);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Spawns a new teddy bear at a random location
        /// </summary>
        private void SpawnBear()
        {
            // generate random location
            int xBearRandom = GetRandomLocation(GameConstants.SpawnBorderSize, GameConstants.WindowWidth - GameConstants.SpawnBorderSize * 2 - Content.Load<Texture2D>(@"graphics\teddybear").Width);
            int yBearRandom = GetRandomLocation(GameConstants.SpawnBorderSize, GameConstants.WindowHeight - GameConstants.SpawnBorderSize * 2 - Content.Load<Texture2D>(@"graphics\teddybear").Height);


            //GameConstants.SpawnBorderSize + RandomNumberGenerator.Next(GameConstants.WindowWidth - GameConstants.SpawnBorderSize * 2 + 1);

            // generate random velocity            
            float speedBear = RandomNumberGenerator.NextFloat(GameConstants.BearSpeedRange);
            if (speedBear < GameConstants.MinBearSpeed)
                speedBear = GameConstants.MinBearSpeed;

            // a random angle using Math.PI and the RandomNumberGenerator NextFloat method
            float angleInRadians = RandomNumberGenerator.NextFloat(2 * (float)Math.PI); //360 = 2*Pi
            float angleInDegree = angleInRadians * (180 / (float)Math.PI);

            //random speed and angles you generated and the appropriate trigonometry: speed as the length of hypotenuse of a right triangle
            float xVelocity = speedBear * (float)Math.Cos(angleInRadians);
            float yVelocity = speedBear * (float)Math.Sin(angleInRadians);
            Vector2 velocityBear = new Vector2(xVelocity, yVelocity);

            // create new bear  be speed * cos(angle) and the Y component would be speed * sin(angle)
            TeddyBear newBear = new TeddyBear(Content, @"graphics\teddybear", xBearRandom, yBearRandom, velocityBear, teddyBounce, teddyShot);

            // make sure we don't spawn into a collision
            List <Rectangle> collisions = GetCollisionRectangles();

                while (!CollisionUtils.IsCollisionFree(newBear.DrawRectangle, collisions))
                {
                    int newXBearRandom = GetRandomLocation(GameConstants.SpawnBorderSize, GameConstants.WindowWidth - GameConstants.SpawnBorderSize * 2 - Content.Load<Texture2D>(@"graphics\teddybear").Width);
                    int newYBearRandom = GetRandomLocation(GameConstants.SpawnBorderSize, GameConstants.WindowHeight - GameConstants.SpawnBorderSize * 2 - Content.Load<Texture2D>(@"graphics\teddybear").Height);
                    newBear.X = newXBearRandom;
                    newBear.Y = newYBearRandom;

                    //alternate way ()instead of 2 above statements:
                    //newBear = new TeddyBear(Content, @"graphics\teddybear", newXBearRandom, newYBearRandom, velocityBear, null, null);
                }
            

            // add new bear to list
            bears.Add(newBear);

        }

        /// <summary>
        /// Gets a random location using the given min and range
        /// </summary>
        /// <param name="min">the minimum</param>
        /// <param name="range">the range</param>
        /// <returns>the random location</returns>
        private int GetRandomLocation(int min, int range)
        {
            return min + RandomNumberGenerator.Next(range);
        }

        /// <summary>
        /// Gets a list of collision rectangles for all the objects in the game world
        /// </summary>
        /// <returns>the list of collision rectangles</returns>
        private List<Rectangle> GetCollisionRectangles()
        {
        
            
                List<Rectangle> collisionRectangles = new List<Rectangle>();
                if (burger != null)
                    collisionRectangles.Add(burger.CollisionRectangle);

                if (bears.Any())
                {
                    foreach (TeddyBear bear in bears)
                    {
                        collisionRectangles.Add(bear.CollisionRectangle);
                    }
                }
                foreach (Projectile projectile in projectiles)
                {
                    collisionRectangles.Add(projectile.CollisionRectangle);
                }
                foreach (Explosion explosion in explosions)
                {
                    collisionRectangles.Add(explosion.CollisionRectangle);
                }
                return collisionRectangles;
            
      
        }

        /// <summary>
        /// Checks to see if the burger has just been killed
        /// </summary>
        private void CheckBurgerKill()
        {
            if (burger.Health<=0&&!burgerDead)
            {
                burgerDeath.Play();
                fontGameOver.Spacing = 3;
                ClearTable();
                eventMessage =new Message(gameOver,fontGameOver,new Vector2(GameConstants.WindowWidth / 2 - font.Texture.Width/2+font.Texture.Width / 8, GameConstants.WindowHeight/2 - font.Texture.Height / 8));
                burgerDead = true;
                SaveScore(Environment.UserName, @".\results.xml" ); // UserName should be replaced with input
              
                messages.Add(new Message(string.Format("Your finally score result is: {0}", score), font, GameConstants.ScoreLocation));

                scoreTable = LoadScore(@".\results.xml");
                int scoreValuesCount = 3;

                Vector2 scoreLocation = new Vector2(GameConstants.DisplayOffset, scoreValuesCount * GameConstants.DisplayOffset);
                messages.Add(new Message(string.Format("Score table results: "), font, scoreLocation));
                foreach (ScoreValues score in scoreTable.ScoreList)
                {
                    scoreValuesCount++;
                    scoreLocation = new Vector2(GameConstants.DisplayOffset, scoreValuesCount * GameConstants.DisplayOffset);
                    messages.Add(new Message(string.Format("{0} worths {1} points", score.Name, score.Score), font, scoreLocation));
                }
            }
        }


        private void ClearTable()
        {
            messages.Clear();
            bears.Clear();
            burger = null;
        }

        private void SaveScore(string nickName, string pathFile)
        {


            if (File.Exists(pathFile))
            {
                scoreTable = LoadScore(pathFile);
            }

            using (Stream scoreFile = File.Open(pathFile, FileMode.Create))
            {
                ScoreValues svCurrent = new ScoreValues();
                svCurrent.Name = nickName;
                svCurrent.Score = score;
                scoreTable.ScoreList.Add(svCurrent);
                XmlSerializer xmlSer = new XmlSerializer(typeof(ScoreTable));
                xmlSer.Serialize(scoreFile,scoreTable);
            }
        }

        private ScoreTable LoadScore(string pathFile)
        {
            if (File.Exists(pathFile))
            {
                using (Stream scoreFile = File.Open(pathFile, FileMode.Open))
                {
                    XmlSerializer xmlSer = new XmlSerializer(typeof(ScoreTable));
                    ScoreTable scoreTable = xmlSer.Deserialize(scoreFile) as ScoreTable;
                    return scoreTable;
                }
            }
            else return null;      
        }


        private string GetScoreString(int scoreValue)
        {
            return GameConstants.ScorePrefix + scoreValue;
        }


        private void TextInput (KeyboardState keyboardState){

            while (!keyboardState.IsKeyDown(Keys.Enter))
            foreach (Keys key in keysToCheck)
            {
                if (CheckKey(key))
                {
                    AddKeyToText(key);
                    break;
                }
            }

        }

        private void AddKeyToText(Keys key)
        {
            string newChar = "";

            if (text.Length >= 20 && key != Keys.Back)
                return;

            switch (key)
            {
                case Keys.A:
                    newChar += "a";
                    break;
                case Keys.B:
                    newChar += "b";
                    break;
                case Keys.C:
                    newChar += "c";
                    break;
                case Keys.D:
                    newChar += "d";
                    break;
                case Keys.E:
                    newChar += "e";
                    break;
                case Keys.F:
                    newChar += "f";
                    break;
                case Keys.G:
                    newChar += "g";
                    break;
                case Keys.H:
                    newChar += "h";
                    break;
                case Keys.I:
                    newChar += "i";
                    break;
                case Keys.J:
                    newChar += "j";
                    break;
                case Keys.K:
                    newChar += "k";
                    break;
                case Keys.L:
                    newChar += "l";
                    break;
                case Keys.M:
                    newChar += "m";
                    break;
                case Keys.N:
                    newChar += "n";
                    break;
                case Keys.O:
                    newChar += "o";
                    break;
                case Keys.P:
                    newChar += "p";
                    break;
                case Keys.Q:
                    newChar += "q";
                    break;
                case Keys.R:
                    newChar += "r";
                    break;
                case Keys.S:
                    newChar += "s";
                    break;
                case Keys.T:
                    newChar += "t";
                    break;
                case Keys.U:
                    newChar += "u";
                    break;
                case Keys.V:
                    newChar += "v";
                    break;
                case Keys.W:
                    newChar += "w";
                    break;
                case Keys.X:
                    newChar += "x";
                    break;
                case Keys.Y:
                    newChar += "y";
                    break;
                case Keys.Z:
                    newChar += "z";
                    break;
                case Keys.Space:
                    newChar += " ";
                    break;
                case Keys.Back:
                    if (text.Length != 0)
                        text = text.Remove(text.Length - 1);
                    return;
            }
            if (currentKeyboardState.IsKeyDown(Keys.RightShift) ||
                currentKeyboardState.IsKeyDown(Keys.LeftShift))
            {
                newChar = newChar.ToUpper();
            }
            text += newChar;
        }

        private bool CheckKey(Keys theKey)
        {
            return lastKeyboardState.IsKeyDown(theKey) && currentKeyboardState.IsKeyUp(theKey);
        }



        #endregion
    }
}