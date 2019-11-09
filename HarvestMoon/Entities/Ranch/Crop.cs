using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace HarvestMoon.Entities.Ranch
{
    public class Crop
    {
        private readonly AnimatedSprite _sprite;

        private bool IsWatered { get; set; }

        private string CropType { get; set; }

        private string Maturity { get; set; }

        private float X;
        private float Y;

        public Crop(ContentManager content, Vector2 initialPosition, string cropType, int maturity)
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
                        "turnip_a", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(4)
                            }
                        }
                    },
                    {
                        "turnip_a_watered", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(5)
                            }
                        }
                    },
                    {
                        "turnip_b", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(6)
                            }
                        }
                    },
                    {
                        "turnip_b_watered", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(7)
                            }
                        }
                    },
                    {
                        "potato_a", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(8)
                            }
                        }
                    },
                    {
                        "potato_a_watered", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(9)
                            }
                        }
                    },
                    {
                        "potato_b", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(10)
                            }
                        }
                    },
                    {
                        "potato_b_watered", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(11)
                            }
                        }
                    }


                }
            };

            _sprite = new AnimatedSprite(cropAnimationFactory, "soil_normal");

            X = initialPosition.X;
            Y = initialPosition.Y;

            CropType = cropType;
            HaveMaturity(maturity);

            _sprite.Play(CropType + "_" + Maturity);
        }

        public void HaveMaturity(int maturity)
        {
            if (CropType == "turnip")
            {
                if (maturity >= 2)
                {
                    Maturity = "a";
                }
                else if (maturity >= 4)
                {
                    Maturity = "b";
                }
            }
            else if (CropType == "potato")
            {
                if (maturity >= 2)
                {
                    Maturity = "a";
                }
                else if (maturity >= 7)
                {
                    Maturity = "b";
                }
            }
            else if (CropType == "tomato")
            {
                if (maturity >= 2)
                {
                    Maturity = "a";
                }
                else if (maturity >= 4)
                {
                    Maturity = "b";
                }
                else if(maturity >= 6)
                {
                    Maturity = "c";
                }
                else if(maturity >= 9)
                {
                    Maturity = "d";
                }
            }
            else if (CropType == "corn")
            {
                if (maturity >= 2)
                {
                    Maturity = "a";
                }
                else if (maturity >= 6)
                {
                    Maturity = "b";
                }
                else if (maturity >= 9)
                {
                    Maturity = "c";
                }
                else if (maturity >= 12)
                {
                    Maturity = "d";
                }
            }
            else if(CropType == "grass")
            {
                if (maturity >= 2)
                {
                    Maturity = "a";
                }
                else if (maturity >= 4)
                {
                    Maturity = "b";
                }
                else if (maturity >= 6)
                {
                    Maturity = "c";
                }
                else if (maturity >= 9)
                {
                    Maturity = "d";
                }
            }
        }

        public void Water()
        {
            _sprite.Play(CropType + "_" + Maturity + "_" + "watered");

            IsWatered = true;
        }

        public void Dry()
        {
            _sprite.Play(CropType + "_" + Maturity);
            IsWatered = false;
        }
        
        public void Update(GameTime gameTime)
        {
            _sprite.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite, new Vector2(X, Y), 0.0f, new Vector2(2, 2));
        }
    }
}
