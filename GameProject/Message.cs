
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject
{
 
        /// <summary>
        /// A message
        /// </summary>
        public class Message
        {
            #region Fields

            string text;
            SpriteFont font;
            Vector2 fontLocation;
            Vector2 position;

            #endregion

            #region Constructors

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="text">the text for the message</param>
            /// <param name="font">the sprite font for the message</param>
            /// <param name="center">the center of the message</param>
            public Message(string text, SpriteFont font, Vector2 fontLocation)
            {
                this.text = text;
                this.font = font;
                this.fontLocation = fontLocation;

                // calculate position from text and w/o center
                float textWidth = font.MeasureString(text).X;
                float textHeight = font.MeasureString(text).Y;
                position = new Vector2(fontLocation.X , fontLocation.Y );
                //position = new Vector2(center.X - textWidth / 2,
                //    center.Y - textHeight / 2);
            }

            #endregion

            #region Properties

            /// <summary>
            /// Sets the text for the message
            /// </summary>
            public string Text
            {
                set
                {
                    text = value;

                    // changing text could change text location
                    float textWidth = font.MeasureString(text).X;
                    float textHeight = font.MeasureString(text).Y;
                    position.X = fontLocation.X ;
                    position.Y = fontLocation.Y ;
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Draws the message
            /// </summary>
            /// <param name="spriteBatch"></param>
            public void Draw(SpriteBatch spriteBatch, Color color)
            {
                spriteBatch.DrawString(font, text, position, color);
            }

            #endregion
        }
    
}
