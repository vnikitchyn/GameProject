using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameProject
{
    /// <summary>
    /// A burger
    /// </summary>
    public class Burger
    {
        #region Fields

        // graphic and drawing info
        Texture2D sprite;
        Rectangle drawRectangle;

        // burger stats
        int health = 100;

        // shooting support
        bool canShoot = true;
        int elapsedCooldownMilliseconds = 0;

        // sound effect
        SoundEffect shootSound;

        #endregion

        #region Constructors

        /// <summary>
        ///  Constructs a burger
        /// </summary>
        /// <param name="contentManager">the content manager for loading content</param>
        /// <param name="spriteName">the sprite name</param>
        /// <param name="x">the x location of the center of the burger</param>
        /// <param name="y">the y location of the center of the burger</param>
        /// <param name="shootSound">the sound the burger plays when shooting</param>
        public Burger(ContentManager contentManager, string spriteName, int x, int y,
            SoundEffect shootSound)
        {
            LoadContent(contentManager, spriteName, x, y);
            this.shootSound = shootSound;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collision rectangle for the burger
        /// </summary>
        public Rectangle CollisionRectangle
        {
            get { return drawRectangle; }
        }
        public int Health {
            get { return health; }
            set { if (value > 0)
                    health = value;
                else
                    health = 0;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Updates the burger's location based on mouse. Also fires 
        /// french fries as appropriate
        /// </summary>
        /// <param name="gameTime">game time</param>
        /// <param name="mouse">the current state of the mouse</param>
        public void Update(GameTime gameTime, MouseState mouse, KeyboardState keyboard)
        {
            // burger should only respond to input if it still has health
            if (health > 0)

            // move burger using mouse
            {
                //drawRectangle.X = mouse.X;
                //drawRectangle.Y = mouse.Y;

                //similar, but based on keyboard (3 could be changed with any of figures, better - const fields) 
                //obviosly, mouse approach should be commented to enable below.
                //Also, this is implementation with well-known Doom straferunning bug (moving diagonally is faster than up/d/r/l directons.
                //Solving by  x = ax/|a| |a|= sqrt((ax * ax) + (ay * ay) or Vector Normalize


                if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D))
                {
                    drawRectangle.X += 5;
                }
                if (keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.A))
                {     
                    drawRectangle.X -= 5;
                }
                if (keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.W))
                {
                    drawRectangle.Y -= 5;
                }
                if (keyboard.IsKeyDown(Keys.Down) || keyboard.IsKeyDown(Keys.S))
                {
                    drawRectangle.Y += 5;
                }

                //drawRectangle.Y += (int)normalizedDirection.Y *2;


                // create a new projectile if the left mouse button is pressed and health>0

                if (mouse.LeftButton == ButtonState.Pressed && canShoot || keyboard.IsKeyDown(Keys.Space)&& canShoot)
                    {
                        canShoot = false;
                        Projectile projectile =
                            new Projectile(ProjectileType.FrenchFries, Game1.GetProjectileSprite(ProjectileType.FrenchFries),
                            drawRectangle.Center.X, drawRectangle.Top - GameConstants.FrenchFriesProjectileOffset, GameConstants.FrenchFriesProjectileSpeed);
                        Game1.AddProjectile(projectile);
                        shootSound.Play();
                    }               
            }
            // clamp burger in window
            if (drawRectangle.Y < 0)
                drawRectangle.Y = 0;
            else if ((drawRectangle.Y + drawRectangle.Height) > GameConstants.WindowHeight)
                drawRectangle.Y = GameConstants.WindowHeight - drawRectangle.Height;
            if (drawRectangle.X < 0)
                drawRectangle.X = 0;
            else if ((drawRectangle.X + drawRectangle.Width) > GameConstants.WindowWidth)
                drawRectangle.X = GameConstants.WindowWidth - drawRectangle.Width;

            // update shooting allowed
            // timer concept (for animations) 
            // shoot if appropriate

            if (!canShoot)
            {
                elapsedCooldownMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
                bool keyBool = mouse.LeftButton == ButtonState.Released && keyboard.IsKeyUp(Keys.Space);
                if (elapsedCooldownMilliseconds >= GameConstants.BurgerTotalCooldownMilliseconds || keyBool)
                {
                    canShoot = true;
                    elapsedCooldownMilliseconds = 0;
                } 
            }
        }

        /// <summary>
        /// Draws the burger
        /// </summary>
        /// <param name="spriteBatch">the sprite batch to use</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, drawRectangle, Color.White);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Loads the content for the burger
        /// </summary>
        /// <param name="contentManager">the content manager to use</param>
        /// <param name="spriteName">the name of the sprite for the burger</param>
        /// <param name="x">the x location of the center of the burger</param>
        /// <param name="y">the y location of the center of the burger</param>
        private void LoadContent(ContentManager contentManager, string spriteName,
            int x, int y)

        {
            // load content and set remainder of draw rectangle
            sprite = contentManager.Load<Texture2D>(spriteName);
            drawRectangle = new Rectangle(x - sprite.Width / 2,
                y - sprite.Height / 2, sprite.Width,
                sprite.Height);
        }

        #endregion
    }
}
