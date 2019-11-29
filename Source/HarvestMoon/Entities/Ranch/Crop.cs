using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using HarvestMoon.Entities.Items;

namespace HarvestMoon.Entities.Ranch
{
    public class Crop
    {
        private readonly AnimatedSprite _sprite;

        private bool IsWatered { get; set; }

        private string CropType { get; set; }

        public string Maturity { get; set; }

        private float X;
        private float Y;

        ContentManager Content;

        public Crop(ContentManager content, Vector2 initialPosition, string cropType, int maturity)
        {
            Content = content;

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
                    },
                    {
                        "tomato_a", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(12)
                            }
                        }
                    },
                    {
                        "tomato_a_watered", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(13)
                            }
                        }
                    },
                    {
                        "tomato_b", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(14)
                            }
                        }
                    },
                    {
                        "tomato_b_watered", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(15)
                            }
                        }
                    },
                    {
                        "tomato_c", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(16)
                            }
                        }
                    },
                    {
                        "tomato_c_watered", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(17)
                            }
                        }
                    },
                    {
                        "tomato_d", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(18)
                            }
                        }
                    },
                    {
                        "tomato_d_watered", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(19)
                            }
                        }
                    },
                    {
                        "corn_a", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(20)
                            }
                        }
                    },
                    {
                        "corn_a_watered", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(21)
                            }
                        }
                    },
                    {
                        "corn_b", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(22)
                            }
                        }
                    },
                    {
                        "corn_b_watered", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(23)
                            }
                        }
                    },
                    {
                        "corn_c", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(24)
                            }
                        }
                    },
                    {
                        "corn_c_watered", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(25)
                            }
                        }
                    },
                    {
                        "corn_d", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(26)
                            }
                        }
                    },
                    {
                        "corn_d_watered", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(27)
                            }
                        }
                    },
                    {
                        "grass_a", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(29)
                            }
                        }
                    },
                    {
                        "grass_b", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(30)
                            }
                        }
                    },
                    {
                        "grass_c", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = 1.0f,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(31)
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
                if (maturity >= 2 && maturity < 4)
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
                if (maturity >= 2 && maturity < 7)
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
                if (maturity >= 2 && maturity < 4)
                {
                    Maturity = "a";
                }
                else if (maturity >= 4 && maturity < 6)
                {
                    Maturity = "b";
                }
                else if(maturity >= 6 && maturity < 9)
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
                if (maturity >= 2 && maturity < 6)
                {
                    Maturity = "a";
                }
                else if (maturity >= 6 && maturity < 9)
                {
                    Maturity = "b";
                }
                else if (maturity >= 9 && maturity < 12)
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
                if (maturity >= 3 && maturity < 6)
                {
                    Maturity = "a";
                }
                else if (maturity >= 6 && maturity < 9)
                {
                    Maturity = "b";
                }
                else if (maturity >= 9)
                {
                    Maturity = "c";
                }
            }
        }

        public Item Harvest()
        {
            Item harvest = null;

            if (CropType == "turnip")
            {
                if (Maturity == "b")
                {
                    harvest = new Turnip(Content, new Vector2(X, Y));
                }
            }
            else if (CropType == "potato")
            {
                if (Maturity == "b")
                {
                    harvest = new Potato(Content, new Vector2(X, Y));
                }
            }
            else if (CropType == "tomato")
            {
                if (Maturity == "d")
                {
                    harvest = new Tomato(Content, new Vector2(X, Y));
                }
            }
            else if (CropType == "corn")
            {
                if (Maturity == "d")
                {
                    harvest = new Corn(Content, new Vector2(X, Y));
                }
            }
            else if (CropType == "grass")
            {
                harvest = null;
            }

            return harvest;
        }

        public void Water()
        {
            if(CropType != "grass")
            {
                _sprite.Play(CropType + "_" + Maturity + "_" + "watered");
            }
            else
            {
                _sprite.Play(CropType + "_" + Maturity);
            }
            

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
