using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using Microsoft.Xna.Framework.Content;

namespace HarvestMoon.Entities
{
    public class WoodPiece : Interactable
    {
        private readonly Sprite _sprite;

        public WoodPiece()
        {

        }

        public WoodPiece(ContentManager content, Vector2 initialPosition)
        {
            var woodTexture = content.Load<Texture2D>("maps/ranch/items/wood");
            
            _sprite = new Sprite(woodTexture);

            BoundingRectangle = new RectangleF(new Vector2(initialPosition.X - 16,
                                                            initialPosition.Y - 16), 
                                               new Size2(32, 32));

            X = initialPosition.X;
            Y = initialPosition.Y;

            Planked = true;

            Carryable = true;

            Hammerable = true;

            Interacts = true;
            TypeName = "wood-piece";
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
