using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;

namespace HarvestMoon.Entities
{
    public class Door : Entity
    {
        private Action _triggerEnd;
        private Action _triggerStart;

        public RectangleF BoundingRectangle;

        private Vector2 _position;

        public bool Triggered { get; set; }

        public string Name { get; set; }

        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;
                BoundingRectangle.Position = new Vector2(value.X - BoundingRectangle.Size.Width * 0.5f,
                                                         value.Y - BoundingRectangle.Size.Height * 0.5f);
            }
        }

        public Door(Vector2 initialPosition, Size2 doorSize)
        {
            _position = initialPosition;

            BoundingRectangle = new RectangleF(new Vector2(_position.X - doorSize.Width * 0.5f,
                                                            _position.Y - doorSize.Height * 0.5f),
                                               new Size2(doorSize.Width, doorSize.Height));
        }

        public void OnTriggerStart(Action callback)
        {
            _triggerStart = callback;
        }

        public void OnTriggerEnd(Action callback)
        {
            _triggerEnd = callback;
        }

        public virtual void TriggerStart()
        {
            _triggerStart?.Invoke();
        }

        public virtual void TriggerEnd()
        {
            _triggerEnd?.Invoke();
        }

        public virtual void Trigger()
        {
            TriggerStart();
            TriggerEnd();
        }


        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(BoundingRectangle, Color.Fuchsia);
        }
    }

}
