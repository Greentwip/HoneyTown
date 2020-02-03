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
    public class Mouse : NPC
    {
        private readonly AnimatedSprite _sprite;

        private enum Movement
        {
            Up = 0,
            Left = 1,
            Down = 2,
            Right = 3,
            None
        };

        private int _ticks;

        private bool _moving = false;

        private int _leftToMove = 32;

        private Movement _currentMovement = Movement.None;

        private static Random Random = new Random();

        public Mouse(ContentManager content, Vector2 initialPosition, Size2 size)
            : base(initialPosition, size)
        {
            float frameDuration = 1.0f / 7.5f;

            _sprite = AnimationLoader.LoadAnimatedSprite(content,
                                                         "animations/mouse",
                                                         "animations/mouseMap",
                                                         "mouse",
                                                         frameDuration,
                                                         true);

            _sprite.Play("walk_left");

            X = initialPosition.X;
            Y = initialPosition.Y;

            BoundingBoxEnabled = true;
            Interacts = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _sprite.Update(gameTime);

            _ticks = Random.Next(0, 25);

            if (!_recoiling)
            {
                if (!_moving)
                {
                    if (_ticks == 0 || _ticks == 1 || _ticks == 2 || _ticks == 3)
                    {
                        _currentMovement = Movement.Up;
                    }
                    else if (_ticks == 4 || _ticks == 5 || _ticks == 6 || _ticks == 7)
                    {
                        _currentMovement = Movement.Left;
                    }
                    else if (_ticks == 8 || _ticks == 9 || _ticks == 10 || _ticks == 11)
                    {
                        _currentMovement = Movement.Down;
                    }
                    else if (_ticks == 12 || _ticks == 13 || _ticks == 14 || _ticks == 15)
                    {
                        _currentMovement = Movement.Right;
                    }
                    else
                    {
                        _currentMovement = Movement.None;
                    }

                }

                if (!_moving)
                {
                    if (_currentMovement != Movement.None)
                    {
                        _leftToMove = 64;
                    }
                    else
                    {
                        _leftToMove = 64;
                    }
                    _moving = true;
                }

                switch (_currentMovement)
                {
                    case Movement.None:
                        _sprite.Play("walk_down");
                        break;

                    case Movement.Up:
                        _sprite.Play("walk_up");
                        break;

                    case Movement.Left:
                        _sprite.Play("walk_left");
                        break;

                    case Movement.Down:
                        _sprite.Play("walk_down");
                        break;

                    case Movement.Right:
                        _sprite.Play("walk_right");
                        break;
                }


                switch (_currentMovement)
                {
                    case Movement.None:
                        break;

                    case Movement.Up:
                        Y -= 1;
                        break;

                    case Movement.Left:
                        X -= 1;
                        break;

                    case Movement.Down:
                        Y += 1;
                        break;

                    case Movement.Right:
                        X += 1;
                        break;
                }

                _leftToMove -= 1;

                if (_leftToMove <= 0)
                {
                    _moving = false;
                }
            }


            if (_recoiling)
            {
                _recoilTimeCounter++;

                if(_recoilTimeCounter >= 60)
                {
                    _recoiling = false;
                    _recoilTimeCounter = 0;
                    _sprite.Play("walk_left");
                }
                else
                {
                    X -= _recoilNormal.X;
                    Y -= _recoilNormal.Y;
                }
            }

        }

        public void TakeDamage(int amount, Vector2 normal)
        {
            _recoiling = true;
            _recoilNormal = normal;
            _sprite.Play("hit");
        }

        private bool _recoiling;
        private Vector2 _recoilNormal;
        private int _recoilTimeCounter;

        public override void Interact(Item item, Action onInteractionStart, Action onInteractionEnd)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(_sprite.CurrentAnimation != null)
            {
                if(_sprite.CurrentAnimation.Name == "walk_right" || _sprite.CurrentAnimation.Name == "walk_down")
                {
                    _sprite.Effect = SpriteEffects.FlipHorizontally;
                    spriteBatch.Draw(_sprite, new Vector2(X, Y), 0.0f, new Vector2(0.5f, 0.5f));
                }
                else
                {
                    _sprite.Effect = SpriteEffects.None;
                    spriteBatch.Draw(_sprite, new Vector2(X, Y), 0.0f, new Vector2(0.5f, 0.5f));
                }
            }
            else
            {
                _sprite.Effect = SpriteEffects.None;
                spriteBatch.Draw(_sprite, new Vector2(X, Y), 0.0f, new Vector2(0.5f, 0.5f));
            }
        }


    }
}
