using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace HarvestMoon.Entities.Ranch
{
    public class Soil : Interactable
    {
        private readonly AnimatedSprite _sprite;

        public bool IsPlanted { get; set; }
        public bool IsWatered { get; set; }

        public Soil()
        {

        }

        public Soil(ContentManager content, Vector2 initialPosition)
        {
            var cropTexture = content.Load<Texture2D>("maps/ranch/items/crops");
            var cropMap = content.Load<Dictionary<string, Rectangle>>("maps/ranch/items/cropsMap");
            var cropAtlas = new TextureAtlas("crop", cropTexture, cropMap);
            var cropAnimationFactory = new SpriteSheet
            {
                TextureAtlas = cropAtlas,
                Cycles =
                {
                    {
                        "soil_normal", new SpriteSheetAnimationCycle
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
                        "soil_watered", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(1)
                            }
                        }
                    },
                    {
                        "soil_planted_normal", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(2)
                            }
                        }
                    },
                    {
                        "soil_planted_watered", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(3)
                            }
                        }
                    }
                }
            };

            _sprite = new AnimatedSprite(cropAnimationFactory, "soil_normal");

            BoundingRectangle = new RectangleF(new Vector2(initialPosition.X - 16,
                                                            initialPosition.Y - 16),
                                               new Size2(32, 32));

            X = initialPosition.X;
            Y = initialPosition.Y;

            Planked = false;
            Carryable = false;
            Interacts = true;
            Priority = -32;

            TypeName = "soil";
        }

        public void Water()
        {
            if (IsPlanted)
            {
                _sprite.Play("soil_planted_watered");
            }
            else
            {
                _sprite.Play("soil_watered");
            }

            IsWatered = true;
        }

        public void Plant(string content)
        {
            IsPlanted = true;
            _sprite.Play("soil_planted_normal");
        }

        public override void Update(GameTime gameTime)
        {
            _sprite.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite, new Vector2(X, Y), 0.0f, new Vector2(2, 2));
        }
    }
}
