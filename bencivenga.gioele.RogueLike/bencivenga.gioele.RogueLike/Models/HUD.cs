using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bencivenga.gioele.RogueLike
{
    public class HUD
    {
        public SpriteBatch spriteBatch;
        public SpriteFont spriteFont;
        public GraphicsDevice graphicsDevice;

        public Vector2 position;

        public String textValue;
        public Color textColor;

        public bool enabled { get; set; }

        public HUD(Vector2 position, SpriteBatch spriteBatch, SpriteFont spriteFont, GraphicsDevice graphicsDevice)
        {
            this.position = position;

            this.spriteBatch = spriteBatch;
            this.spriteFont = spriteFont;
            this.graphicsDevice = graphicsDevice;
        }

        public void Enable(bool enabled)
        {
            this.enabled = enabled;
        }

        public void Update(String textValue, Color textColor)
        {
            this.textValue = textValue.ToUpper();
            this.textColor = textColor;
        }

        public void Draw()
        {
            if (enabled)
            {
                Color myTransparentColor = new Color(0, 0, 0, 127);

                Vector2 stringDimensions = spriteFont.MeasureString(textValue);
                float width = stringDimensions.X;
                float height = stringDimensions.Y;

                Rectangle backgroundRectangle = new Rectangle();
                backgroundRectangle.Width = (int)width + 10;
                backgroundRectangle.Height = (int)height + 10;
                backgroundRectangle.X = (int)position.X - 5;
                backgroundRectangle.Y = (int)position.Y - 5;

                Texture2D dummyTexture = new Texture2D(graphicsDevice, 1, 1);
                dummyTexture.SetData(new Color[] { myTransparentColor });

                spriteBatch.Draw(dummyTexture, backgroundRectangle, myTransparentColor);
                spriteBatch.DrawString(spriteFont, textValue, position, textColor);
            }
        }
    }
}
