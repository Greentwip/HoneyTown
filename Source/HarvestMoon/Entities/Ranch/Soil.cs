using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using HarvestMoon.Animation;

namespace HarvestMoon.Entities.Ranch
{
    public class Soil : Interactable
    {
        private readonly AnimatedSprite _sprite;

        private List<string> _seasons;

        public bool IsPlanted { get; set; }
        public bool IsWatered { get; set; }
        public bool HasGrown { get; set; }

        public int DaysToGrow { get; set; }

        public int DaysWatered { get; set; }

        public int DaysPlanted { get; set; }

        private List<string> Seasons { get => _seasons; set => _seasons = value; }

        public string CropType { get; set; }

        private Crop HarvestCrop { get; set; }

        public Soil()
        {
            _seasons = new List<string>();
            DaysWatered = 0;
        }

        public Soil(ContentManager content, 
                    Vector2 initialPosition,
                    bool isWatered = false,
                    bool isPlanted = false, 
                    string cropType = default(string), 
                    int daysWatered = -1,
                    int daysPlanted = -1)
        {
            _seasons = new List<string>();

            DaysWatered = 0;

            var cropsB = AnimationLoader.LoadAnimatedSprite(content,
                                                            "animations/cropsB",
                                                            "animations/cropsBMap",
                                                            "cropsB",
                                                            1.0f / 7.5f,
                                                            false);



            _sprite = cropsB;

            _sprite.Play("soil_normal");

            BoundingRectangle = new RectangleF(new Vector2(initialPosition.X - 16,
                                                            initialPosition.Y - 16),
                                               new Size2(32, 32));

            X = initialPosition.X;
            Y = initialPosition.Y;

            Planked = true;
            Carryable = false;
            Interacts = true;
            Cuttable = true;

            Priority = -32;

            TypeName = "soil";

            if (isPlanted)
            {
                IsWatered = isWatered;

                Plant(cropType, daysWatered, daysPlanted);
                GrowAccordingly();
            }

        }

        public void GrowAccordingly()
        {
            if (IsPlanted)
            {
                
                Seasons.Clear();

                // validate each crop type and their corresponding seasons
                if (CropType == "turnip")
                {
                    DaysToGrow = 4;
                    Seasons.Add("Spring");
                }
                else if (CropType == "potato")
                {
                    DaysToGrow = 7;
                    Seasons.Add("Spring");
                }
                else if (CropType == "tomato")
                {
                    DaysToGrow = 9;
                    Seasons.Add("Summer");
                }
                else if(CropType == "corn")
                {
                    DaysToGrow = 12;
                    Seasons.Add("Summer");
                }
                else if(CropType == "grass")
                {
                    Seasons.Add("Spring");
                    Seasons.Add("Summer");
                    Seasons.Add("Fall");

                    DaysToGrow = 9;
                }

                // Determine if it has grown from soil to plant
                int cropMaturity = 0;

                if(CropType != "grass")
                {
                    cropMaturity = DaysWatered;
                }
                else
                {
                    cropMaturity = DaysPlanted;
                }

                if (Seasons.Contains(HarvestMoon.Instance.Season))
                {
                    if (IsWatered)
                    {
                        DaysWatered++;
                    }

                    DaysPlanted++;
                }

                if (cropMaturity >= 2 && Seasons.Contains(HarvestMoon.Instance.Season)) // This should be Crop.CalculateMaturity(cropMaturity) == "a"
                {
                    HasGrown = true;
                    HarvestCrop = new Crop(HarvestMoon.Instance.Content, new Vector2(X, Y), CropType, cropMaturity);
                } 
               

            }
        }

        public Item Harvest()
        {
            Item harvest = null;

            if (HasGrown)
            {
                harvest = HarvestCrop.Harvest();

                if(HarvestMoon.Instance.Season == "Fall")
                {
                    harvest = null;
                }

                if(harvest != null)
                {
                    if (CropType == "turnip" || CropType == "potato")
                    {
                        Reset();
                    }
                    else
                    {
                        HasGrown = true;
                        if (CropType == "tomato")
                        {
                            HarvestCrop = new Crop(HarvestMoon.Instance.Content, new Vector2(X, Y), CropType, 6);
                            DaysWatered = 6;
                        }
                        else if (CropType == "corn")
                        {
                            HarvestCrop = new Crop(HarvestMoon.Instance.Content, new Vector2(X, Y), CropType, 9);
                            DaysWatered = 9;
                        }

                        if (IsWatered)
                        {
                            HarvestCrop.Water();
                        }
                        else
                        {
                            HarvestCrop.Dry();
                        }
                    }
                }
                
            }



            return harvest;
        }

        public override void OnCut()
        {
            if (HasGrown)
            {
                if(CropType != "grass")
                {
                    Reset();
                }
                else
                {
                    if(HarvestCrop.Maturity == "d")
                    {
                        DaysWatered = 0;
                        DaysPlanted = 1;
                        HasGrown = false;
                        HarvestCrop = new Crop(HarvestMoon.Instance.Content, new Vector2(X, Y), CropType, 1);
                        HarvestMoon.Instance.FeedPieces++;
                    }
                }
                
            }

        }

        public void Water()
        {
            if (HasGrown)
            {
                HarvestCrop.Water();
            }
            else
            {
                if (IsPlanted)
                {
                    if(CropType != "grass")
                    {
                        _sprite.Play("soil_planted_watered");
                    }
                    else
                    {
                        _sprite.Play("soil_planted_grass");
                    }
                }
                else
                {
                    _sprite.Play("soil_watered");
                }
            }
            

            IsWatered = true;
        }

        public void Dry()
        {
            if (HasGrown)
            {
                HarvestCrop.Dry();
            }
            else
            {
                if (IsPlanted)
                {
                    _sprite.Play("soil_planted_normal");
                }
                else
                {
                    _sprite.Play("soil_normal");
                }
            }

            IsWatered = false;
        }

        private void Reset()
        {
            IsPlanted = false;
            CropType = "none";
            DaysToGrow = -1;
            DaysWatered = 0;
            DaysPlanted = 0;
            HasGrown = false;
            _sprite.Play("soil_normal");

        }

        public void Plant(string cropType, int daysWatered, int daysPlanted)
        {
            IsPlanted = true;
            CropType = cropType;
            DaysWatered = daysWatered;
            DaysPlanted = daysPlanted;

            if(CropType != "grass")
            {
                _sprite.Play("soil_planted_normal");
            }
            else
            {
                _sprite.Play("soil_planted_grass");
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (HasGrown)
            {
                HarvestCrop.Update(gameTime);
            }
            else
            {
                _sprite.Update(gameTime);
            }
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (HasGrown)
            {
                HarvestCrop.Draw(spriteBatch);
            }
            else
            {
                spriteBatch.Draw(_sprite, new Vector2(X, Y - BoundingRectangle.Height / 2), 0.0f, new Vector2(1, 1));
            }            
        }
    }
}
