using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarvestMoon.Entities
{
    [Serializable]
    public abstract class Interactable : Entity
    {
        public RectangleF BoundingRectangle;

        
        public bool Planked { get; set; }

        public bool Shippable { get; set; }

        public bool Carryable { get; set; }

        public bool Packable { get; set; }

        public bool Breakable { get; set; }

        public bool Interacts { get; set; }

        public bool IsNPC { get; set; }

        public string TypeName { get; set; }

        private float _x;
        private float _y;

        public float X
        {
            get => _x;
            set
            {
                _x = value;
                BoundingRectangle.Position = new Vector2(X - BoundingRectangle.Size.Width * 0.5f,
                                                         Y - BoundingRectangle.Size.Height * 0.5f);
            }
        }

        public float Y
        {
            get => _y;
            set
            {
                _y = value;
                BoundingRectangle.Position = new Vector2(X - BoundingRectangle.Size.Width * 0.5f,
                                                         Y - BoundingRectangle.Size.Height * 0.5f);
            }
        }

        public virtual void OnInteractableDrop()
        {

        }

    }
}
