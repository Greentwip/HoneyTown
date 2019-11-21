using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using Microsoft.Xna.Framework.Content;

namespace HarvestMoon.Entities
{
    public class BigRock : Interactable
    {
        private readonly Sprite _sprite;

        public BigRock()
        {

        }


        public BigRock(ContentManager content, Vector2 initialPosition)
        {
            var rockBigTexture = content.Load<Texture2D>("maps/ranch/items/rock-big");

            _sprite = new Sprite(rockBigTexture);

            BoundingRectangle = new RectangleF(new Vector2(initialPosition.X - 32,
                                                            initialPosition.Y - 32),
                                               new Size2(64, 64));

            X = initialPosition.X;
            Y = initialPosition.Y;

            Planked = true;

            Carryable = false;

            Hammerable = true;

            Interacts = true;

            TypeName = "big-rock";

        }

        public override void OnHammer(int power)
        {
            if(power >= 4)
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
