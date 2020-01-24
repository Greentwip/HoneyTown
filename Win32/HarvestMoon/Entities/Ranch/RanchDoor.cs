using HarvestMoon.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarvestMoon.Entities.Ranch
{
    public class RanchDoor: Door
    {
        private AnimatedSprite _sprite;
        private bool _triggered;
        private bool _transitioning;

        private bool _triggerStart;

        public RanchDoor(ContentManager content, Vector2 initialPosition, Size2 doorSize): base(initialPosition, doorSize)
        {
            _triggered = false;
            _transitioning = false;
            _triggerStart = false;

            float frameDuration = 1.0f / 7.5f;

            _sprite = AnimationLoader.LoadAnimatedSprite(content,
                                                 "animations/doorsA",
                                                 "animations/door1Map",
                                                 "ranch-door",
                                                 frameDuration,
                                                 false);

            _sprite.Play("closed");
        }

        public override void Trigger()
        {
            _triggered = true;
        }

        public override void Update(GameTime gameTime)
        {
            _sprite.Update(gameTime);

            if (_triggered)
            {
                _sprite.Play("open");

                if (!_transitioning)
                {
                    if (!_triggerStart)
                    {
                        base.TriggerStart();

                        _triggerStart = true;
                    }
                    _transitioning = true;
                }

            }

            if (_transitioning)
            {
                if (_sprite.CurrentAnimation.IsComplete)
                {
                    _transitioning = false;
                    base.TriggerEnd();
                }
            }

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite, Position, 0, new Vector2(0.75f, 0.75f));
            base.Draw(spriteBatch);
        }
    }
}
