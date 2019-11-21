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
    public class Bush : Item
    {
        private readonly Sprite _sprite;

        public Bush()
            : base(new Vector2(0, 0))
        {

        }

        public Bush(ContentManager content, Vector2 initialPosition)
            : base(initialPosition)
        {
            var bushTexture = content.Load<Texture2D>("maps/ranch/items/bush");
            
            _sprite = new Sprite(bushTexture);

            BoundingRectangle = new RectangleF(new Vector2(initialPosition.X - 16,
                                                            initialPosition.Y - 16), 
                                               new Size2(32, 32));

            X = initialPosition.X;
            Y = initialPosition.Y;

            Planked = true;

            Carryable = true;

            Interacts = true;

            Cuttable = true;

            TypeName = "bush";

        }

        public override void OnCut()
        {
            OnInteractableDrop();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite, new Vector2(X, Y));
        }
    }

}
