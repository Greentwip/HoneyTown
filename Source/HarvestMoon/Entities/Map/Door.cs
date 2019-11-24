using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.ViewportAdapters;
using System.Collections.Generic;

using MonoGame.Extended.Content;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Collisions;
using System;

namespace HarvestMoon.Entities
{
    public class Door : Entity
    {
        private Action trigger;

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

        public void OnTrigger(Action callback)
        {
            trigger = callback;
        }

        public void Trigger()
        {
            trigger?.Invoke();
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
