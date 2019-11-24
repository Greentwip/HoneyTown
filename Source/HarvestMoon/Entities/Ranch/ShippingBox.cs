using HarvestMoon.Entities.General;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarvestMoon.Entities.Ranch
{
    public class ShippingBox : NPC
    {
        private readonly Sprite _sprite;

        public ShippingBox(ContentManager content, Vector2 initialPosition)
            : base(initialPosition, new Size2(64, 64))
        {
            var cropTexture = content.Load<Texture2D>("maps/ranch/items/shipping-box");

            _sprite = new Sprite(cropTexture);

            X = initialPosition.X;
            Y = initialPosition.Y;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite, new Vector2(X, Y), 0.0f);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Interact(Item item)
        {
            if(item != null)
            {
                if (item.Shippable)
                {
                    HarvestMoon.Instance.TodayGold += item.SellPrice;
                    item.Destroy();
                }
            }            
        }

    }
}
