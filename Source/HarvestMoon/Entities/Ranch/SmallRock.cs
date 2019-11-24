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
    public class SmallRock : Interactable
    {
        private readonly Sprite _sprite;

        public SmallRock()
        {

        }

        public SmallRock(ContentManager content, Vector2 initialPosition)
        {
            var rockSmallTexture = content.Load<Texture2D>("maps/ranch/items/rock-small");
            
            _sprite = new Sprite(rockSmallTexture);

            BoundingRectangle = new RectangleF(new Vector2(initialPosition.X - 16,
                                                            initialPosition.Y - 16), 
                                               new Size2(32, 32));

            X = initialPosition.X;
            Y = initialPosition.Y;

            Planked = true;

            Carryable = true;
            Hammerable = true;

            Interacts = true;

            TypeName = "small-rock";

        }

        public override void OnHammer(int power)
        {
            if(power >= 1)
            {
                Destroy();
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
