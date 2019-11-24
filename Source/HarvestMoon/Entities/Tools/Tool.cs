using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using Microsoft.Xna.Framework.Content;

namespace HarvestMoon.Entities
{
    public abstract class Tool : Interactable
    {
        private readonly Sprite _sprite;

        public string Name { get; set; }

        public bool Visible { get; set; }

        private Vector2 StartupPosition = Vector2.Zero;

        public void Hide()
        {
            Visible = false;
            Interacts = false;
        }

        public void Reset()
        {
            BoundingRectangle = new RectangleF(new Vector2(StartupPosition.X - 16,
                                                           StartupPosition.Y - 16),
                                   new Size2(32, 32));

            X = StartupPosition.X;
            Y = StartupPosition.Y;

            Visible = true;

            Planked = true;
            Carryable = true;
            Packable = true;
            Interacts = true;

        }

        public Tool(string name, ContentManager content, Vector2 initialPosition, string texturePath)
        {
            Name = name;

            var texture = content.Load<Texture2D>(texturePath);
            
            _sprite = new Sprite(texture);

            StartupPosition = initialPosition;

            BoundingRectangle = new RectangleF(new Vector2(initialPosition.X - 16,
                                                            initialPosition.Y - 16), 
                                               new Size2(32, 32));

            X = initialPosition.X;
            Y = initialPosition.Y;

            Planked = true;
            Carryable = true;
            Packable = true;
            Visible = true;
            Interacts = true;

        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                spriteBatch.Draw(_sprite, new Vector2(X, Y));
            }
            
        }
    }

}
