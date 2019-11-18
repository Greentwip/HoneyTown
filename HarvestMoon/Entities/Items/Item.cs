using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using Microsoft.Xna.Framework.Content;

namespace HarvestMoon.Entities
{
    public abstract class Item : Interactable
    {
        public int AffectionPoints { get; set; }
        public int SellPrice { get; set; }

        public Item(Vector2 initialPosition)
        {
            BoundingRectangle = new RectangleF(new Vector2(initialPosition.X - 16,
                                                            initialPosition.Y - 16), 
                                               new Size2(32, 32));

            X = initialPosition.X;
            Y = initialPosition.Y;

            Planked = false;

            Carryable = true;

            Interacts = true;

            TypeName = "item";

        }
    }

}
