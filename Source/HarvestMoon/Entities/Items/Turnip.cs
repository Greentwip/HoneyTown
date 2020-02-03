using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarvestMoon.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace HarvestMoon.Entities.Items
{
    public class Turnip : Item
    {

        private readonly AnimatedSprite _sprite;

        public Turnip(ContentManager content, Vector2 initialPosition)
            : base(initialPosition)
        {
            var cropItems = AnimationLoader.LoadAnimatedSprite(content,
                                                                "animations/iconSet",
                                                                "animations/cropItemsMap",
                                                                "turnipItem",
                                                                1.0f / 7.5f,
                                                                false);



            AffectionPoints = 4;

            _sprite = cropItems;


            _sprite.Play("turnip_normal");

            X = initialPosition.X;
            Y = initialPosition.Y;

            SellPrice = 60;

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite, new Vector2(X, Y), 0.0f, new Vector2(1, 1));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _sprite.Update(gameTime);

            if (_dropped)
            {
                _sprite.Play("turnip_broken");
            }
        }

    }
}
