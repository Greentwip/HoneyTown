using HarvestMoon.Animation;
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
    public class Ted : NPC
    {
        private readonly AnimatedSprite _sprite;

        public Ted(ContentManager content, Vector2 initialPosition, Size2 size)
            : base(initialPosition, size)
        {
            float frameDuration = 1.0f / 4.0f;

            var animatedSprite = AnimationLoader.LoadAnimatedSprite(content,
                                                                 "animations/NPC_02",
                                                                 "animations/tedMap",
                                                                 "ted",
                                                                 frameDuration,
                                                                 true);


            _sprite = animatedSprite;
            _sprite.Play("walk_left");

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
            string Message = "Do you even know how much I lift? It's crazy.";


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

                if (Affection < 0)
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
