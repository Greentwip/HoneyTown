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

namespace HarvestMoon.Entities
{
    public class Grid : Entity
    {
        public RectangleF BoundingRectangle;

        private readonly Transform2 _transform;

        public Vector2 Position
        {
            get => _transform.Position;
            set
            {
                _transform.Position = value;
                BoundingRectangle.Position = new Vector2(value.X - BoundingRectangle.Size.Width * 0.5f,
                                                         value.Y - BoundingRectangle.Size.Height * 0.5f);
            }
        }

        public CollisionGrid CollisionGridSet{ get; set; }

        public Grid(Vector2 initialPosition, Size2 gridSize)
        {
            _transform = new Transform2
            {
                Scale = Vector2.One,
                Position = initialPosition
            };

            BoundingRectangle = new RectangleF(new Vector2(_transform.Position.X - gridSize.Width * 0.5f,
                                                            _transform.Position.Y - gridSize.Height * 0.5f),
                                               new Size2(gridSize.Width, gridSize.Height));

            int columns = (int)(gridSize.Width / 32.0f);
            int rows = (int)(gridSize.Height / 32.0f);

            List<int> dataArray = new List<int>();

            for(var i = 0; i<columns; ++i)
            {
                for(var j = 0; j<rows; ++j)
                {
                    dataArray.Add(1);
                }
            }

            CollisionGridSet = new CollisionGrid(dataArray.ToArray(), columns, rows, 32, 32, BoundingRectangle.TopLeft);
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
