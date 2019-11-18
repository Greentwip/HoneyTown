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

        private List<string> _seasons;

        public bool IsPlanted { get; set; }
        public bool IsWatered { get; set; }
        public bool HasGrown { get; set; }

        public int DaysToGrow { get; set; }

        public int DaysWatered { get; set; }

        public string SeasonPlanted { get; set; }

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
                    bool isPlanted = false, 
                    string cropType = default(string), 
                    int daysWatered = -1,
                    string seasonPlanted = default(string))
        {
            _seasons = new List<string>();

            DaysWatered = 0;


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

            if (isPlanted)
            {
                Plant(cropType, daysWatered, seasonPlanted);
                GrowAccordingly();
            }

        }

        public void GrowAccordingly()
        {
            if (IsPlanted)
            {
                if (IsWatered)
                {
                    DaysWatered++;
                }
                
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
                    Seasons.Add("Autumn");
                }

                // Determine if it has grown from soil to plant
                var cropMaturity = DaysWatered;

                if (cropMaturity >= 2 && Seasons.Contains(SeasonPlanted)) // This should be Crop.CalculateMaturity(cropMaturity) == "a"
                {
                    HasGrown = true;
                    HarvestCrop = new Crop(HarvestMoon.Instance.Content, new Vector2(X, Y), CropType, cropMaturity);
                }

            }
        }

        public Item Harvest()
        {
            return null;
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
                    _sprite.Play("soil_planted_watered");
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

        public void Plant(string cropType, int daysWatered, string seasonPlanted)
        {
            IsPlanted = true;
            CropType = cropType;
            DaysWatered = daysWatered;
            SeasonPlanted = seasonPlanted;

            _sprite.Play("soil_planted_normal");
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
                spriteBatch.Draw(_sprite, new Vector2(X, Y), 0.0f, new Vector2(2, 2));

                if(_sprite.CurrentAnimation.Name == "soil_normal")
                {

                }
            }            
        }
    }
}
