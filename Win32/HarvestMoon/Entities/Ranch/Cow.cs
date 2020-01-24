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

namespace HarvestMoon.Entities.Ranch
{
    public class Cow : NPC
    {
        private readonly AnimatedSprite _sprite;

        public Cow(ContentManager content, Vector2 initialPosition, Size2 size)
            : base(initialPosition, size)
        {
            float frameDuration = 1.0f / 7.5f;

            _sprite = AnimationLoader.LoadAnimatedSprite(content,
                                                         "animations/animals",
                                                         "animations/cowMap",
                                                         "cow",
                                                         frameDuration,
                                                         true);

            _sprite.Play("walking_down_idle");

            X = initialPosition.X;
            Y = initialPosition.Y;
        }

        public string GetInteractionMessage()
        {
            string Message = "";

            Message = "There, there, you're doing fine";

            return Message;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _sprite.Update(gameTime);
        }

        public override void Interact(Item item, Action onInteractionStart, Action onInteractionEnd)
        {
            HarvestMoon.Instance.GUI.ShowMessage(GetInteractionMessage(), onInteractionStart, onInteractionEnd);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite, new Vector2(X, Y), 0.0f);
        }


    }
}
