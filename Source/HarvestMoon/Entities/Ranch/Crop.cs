using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using HarvestMoon.Entities.Items;
using HarvestMoon.Animation;

namespace HarvestMoon.Entities.Ranch
{
    public class Crop
    {
        private AnimatedSprite _sprite;

        private readonly AnimatedSprite _cropsA;
        private readonly AnimatedSprite _cropsB;

        private bool IsWatered { get; set; }

        private string CropType { get; set; }

        public string Maturity { get; set; }

        private float X;
        private float Y;

        ContentManager Content;

        public Crop(ContentManager content, Vector2 initialPosition, string cropType, int maturity)
        {
            Content = content;

            var cropsA = AnimationLoader.LoadAnimatedSprite(content,
                                                            "animations/cropsA",
                                                            "animations/cropsAMap",
                                                            "cropsA",
                                                            1.0f / 7.5f,
                                                            false);


            var cropsB = AnimationLoader.LoadAnimatedSprite(content,
                                                            "animations/cropsB",
                                                            "animations/cropsBMap",
                                                            "cropsB",
                                                            1.0f / 7.5f,
                                                            false);


            _sprite = cropsB;

            _cropsA = cropsA;
            _cropsB = cropsB;

            _sprite.Play("soil_normal");

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
                _sprite = _cropsA;

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
                _sprite = _cropsB;

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
                _sprite = _cropsA;

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
                _sprite = _cropsA;

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
                _sprite = _cropsB;

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
            spriteBatch.Draw(_sprite, new Vector2(X, Y - 16), 0.0f, new Vector2(1, 1));
        }
    }
}
