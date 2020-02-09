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
    public class George : NPC
    {
        private readonly AnimatedSprite _sprite;

        public George(ContentManager content, Vector2 initialPosition, Size2 size)
            : base(initialPosition, size)
        {
            float frameDuration = 1.0f / 4.0f;

            var animatedSprite = AnimationLoader.LoadAnimatedSprite(content,
                                                                 "animations/NPC_13",
                                                                 "animations/georgeMap",
                                                                 "george",
                                                                 frameDuration,
                                                                 true);


            _sprite = animatedSprite;
            _sprite.Play("walk_right");

            X = initialPosition.X;
            Y = initialPosition.Y;

            BoundingBoxEnabled = true;

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
