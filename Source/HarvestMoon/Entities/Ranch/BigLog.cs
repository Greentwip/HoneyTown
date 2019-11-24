using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.ViewportAdapters;
using System.Collections.Generic;

using MonoGame.Extended.Content;
using Microsoft.Xna.Framework.Content;

namespace HarvestMoon.Entities
{
    public class BigLog : Interactable
    {
        private readonly Sprite _sprite;

        public BigLog()
        {

        }

        public BigLog(ContentManager content, Vector2 initialPosition)
        {
            var logBigTexture = content.Load<Texture2D>("maps/ranch/items/log-big");

            _sprite = new Sprite(logBigTexture);

            BoundingRectangle = new RectangleF(new Vector2(initialPosition.X - 32,
                                                            initialPosition.Y - 32),
                                               new Size2(64, 64));

            X = initialPosition.X;
            Y = initialPosition.Y;

            Planked = true;

            Carryable = false;
            Splittable = true;

            Interacts = true;

            TypeName = "big-log";

        }

        public override void OnAxe(int power)
        {
            if(power >= 6)
            {
                Destroy();
                HarvestMoon.Instance.Planks += 6;
            }
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite, new Vector2(X, Y));
        }
    }

}
