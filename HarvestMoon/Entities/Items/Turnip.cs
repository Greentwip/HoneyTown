using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var cropTexture = content.Load<Texture2D>("maps/ranch/items/crops");
            var cropMap = content.Load<Dictionary<string, Rectangle>>("maps/ranch/items/cropItemsMap");
            var cropAtlas = new TextureAtlas("crop", cropTexture, cropMap);
            var cropAnimationFactory = new SpriteSheet
            {
                TextureAtlas = cropAtlas,
                Cycles =
                {
                    {
                        "turnip_normal", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(0)
                            }
                        }
                    },
                    {
                        "turnip_broken", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(1)
                            }
                        }
                    }

                }
            };

            _sprite = new AnimatedSprite(cropAnimationFactory, "turnip_normal");

            X = initialPosition.X;
            Y = initialPosition.Y;

            SellPrice = 60;

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite, new Vector2(X, Y), 0.0f, new Vector2(2, 2));
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
