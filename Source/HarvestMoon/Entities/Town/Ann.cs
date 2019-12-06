using HarvestMoon.Entities.General;
using HarvestMoon.Entities.Items;
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

namespace HarvestMoon.Entities.Town
{
    public class Ann : NPC
    {
        private readonly AnimatedSprite _sprite;

        public Ann(ContentManager content, Vector2 initialPosition, Size2 size)
            : base(initialPosition, size)
        {
            float frameDuration = 1.0f / 7.5f;

            var characterTexture = content.Load<Texture2D>("animations/ann");
            var characterMap = content.Load<Dictionary<string, Rectangle>>("animations/annMap");
            var characterAtlas = new TextureAtlas("ann", characterTexture, characterMap);
            var characterAnimationFactory = new SpriteSheet
            {
                TextureAtlas = characterAtlas,
                Cycles =
                {
                    {
                        "idle_down", new SpriteSheetAnimationCycle
                        {
                            IsLooping = true,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                // TODO: Fix per frame duration
                                new SpriteSheetAnimationFrame(0)
                            }
                        }
                    },
                    {
                        "idle_up", new SpriteSheetAnimationCycle
                        {
                            IsLooping = true,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                // TODO: Fix per frame duration
                                new SpriteSheetAnimationFrame(1)
                            }
                        }
                    },
                    {
                        "idle_left", new SpriteSheetAnimationCycle
                        {
                            IsLooping = true,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                // TODO: Fix per frame duration
                                new SpriteSheetAnimationFrame(2)
                            }
                        }
                    },
                    {
                        "idle_right", new SpriteSheetAnimationCycle
                        {
                            IsLooping = true,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                // TODO: Fix per frame duration
                                new SpriteSheetAnimationFrame(3)
                            }
                        }
                    }
                }
            };

            _sprite = new AnimatedSprite(characterAnimationFactory, "idle_down");

            X = initialPosition.X;
            Y = initialPosition.Y;
        }

        public string GetThankYouMessage()
        {
            string Message = "Thank you!";

            return Message;
        }

        public string GetNoThanksMessage()
        {
            string Message = "No thanks.";

            return Message;
        }

        public string GetInteractionMessage()
        {
            string Message = "";

            if(Affection >= 0 && Affection <= 10)
            {
                Message = HarvestMoon.Instance.Strings.Get("STR_ANN_AFFECTION_1");
            }
            else if (Affection >= 10 && Affection <= 20) {
                Message = HarvestMoon.Instance.Strings.Get("STR_ANN_AFFECTION_2");
            }
            else if (Affection >= 20 && Affection <= 30)
            {
                Message = HarvestMoon.Instance.Strings.Get("STR_ANN_AFFECTION_3");
            }
            else if (Affection >= 30 && Affection <= 40)
            {
                Message = HarvestMoon.Instance.Strings.Get("STR_ANN_AFFECTION_4");
            }
            else if (Affection >= 40 && Affection <= 50)
            {
                Message = HarvestMoon.Instance.Strings.Get("STR_ANN_AFFECTION_5");
            }
            else if (Affection > 50)
            {
                Message = HarvestMoon.Instance.Strings.Get("STR_ANN_AFFECTION_6");
            }

            return Message;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _sprite.Update(gameTime);
        }

        public override void Interact(Item item, Action onInteractionStart, Action onInteractionEnd)
        {
            if (item != null)
            {
                bool isPositiveReaction = true;

                if (item is Turnip)
                {
                    Affection += item.AffectionPoints;
                }
                else
                {
                    Affection -= item.AffectionPoints;
                    isPositiveReaction = false;
                }

                if(Affection < 0)
                {
                    Affection = 0;
                }

                item.Destroy();

                if (isPositiveReaction)
                {
                    HarvestMoon.Instance.GUI.ShowMessage(GetThankYouMessage(), onInteractionStart, onInteractionEnd);
                }
                else
                {
                    HarvestMoon.Instance.GUI.ShowMessage(GetNoThanksMessage(), onInteractionStart, onInteractionEnd);
                }
                
            }
            else
            {
                HarvestMoon.Instance.GUI.ShowMessage(GetInteractionMessage(), onInteractionStart, onInteractionEnd);
            }

            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite, new Vector2(X, Y), 0.0f);
        }


    }
}
