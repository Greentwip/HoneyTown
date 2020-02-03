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

using HarvestMoon.Screens;

namespace HarvestMoon.Entities.Town
{
    public class Leader : NPC
    {
        private readonly AnimatedSprite _sprite;

        public Leader(ContentManager content, Vector2 initialPosition, Size2 size)
            : base(initialPosition, size)
        {
            float frameDuration = 1.0f / 4.0f;

            var animatedSprite = AnimationLoader.LoadAnimatedSprite(content,
                                                                 "animations/NPC_14",
                                                                 "animations/leaderMap",
                                                                 "leader",
                                                                 frameDuration,
                                                                 true);


            _sprite = animatedSprite;

            if (HarvestMoon.Instance.ScreenManager.ActiveScreen is Screens.Ranch)
            {
                _sprite.Play("walk_up");
            }
            else
            {
                _sprite.Play("walk_down");
            }
                

            X = initialPosition.X;
            Y = initialPosition.Y;

            if (HarvestMoon.Instance.ScreenManager.ActiveScreen is Screens.Ranch)
            {
                BoundingBoxEnabled = false;
            }
            else
            {
                BoundingBoxEnabled = true;
            }

        }

        public void PlayAnimation(string name)
        {
            _sprite.Play(name);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _sprite.Update(gameTime);
        }

        public override void Interact(Item item, Action onInteractionStart, Action onInteractionEnd)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite, new Vector2(X, Y), 0.0f);
        }


    }
}
