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

        public int Index { get; set; }

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

        public Cow(ContentManager content, Vector2 initialPosition, Size2 size)
            : base(initialPosition, size)
        {
            Index = -1;

            float frameDuration = 1.0f / 7.5f;

            _sprite = AnimationLoader.LoadAnimatedSprite(content,
                                                         "animations/animals",
                                                         "animations/cowMap",
                                                         "cow",
                                                         frameDuration,
                                                         true);

            _sprite.Play("walk_down_idle");

            X = initialPosition.X;
            Y = initialPosition.Y;

            BoundingBoxEnabled = true;
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

            _ticks = Random.Next(0, 60);

            if (!_moving)
            {
                if (_ticks == 60 || _ticks == 30 || _ticks == 15 || _ticks == 7)
                {
                    _currentMovement = Movement.Up;
                }
                else if (_ticks == 10 || _ticks == 20 || _ticks == 40 || _ticks == 50)
                {
                    _currentMovement = Movement.Left;
                }
                else if (_ticks == 1 || _ticks == 2 || _ticks == 3 || _ticks == 4)
                {
                    _currentMovement = Movement.Down;
                }
                else if (_ticks == 11 || _ticks == 12 || _ticks == 13 || _ticks == 14)
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
                if(_currentMovement != Movement.None)
                {
                    _leftToMove = 32;
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
                    _sprite.Play("walk_down_idle");
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
