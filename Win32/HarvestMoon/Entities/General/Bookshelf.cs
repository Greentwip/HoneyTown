using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;


namespace HarvestMoon.Entities.General
{
    public class Bookshelf: NPC
    {
        private string _title;

        public static bool Reading;

        protected List<string> _items = new List<string>();
        protected List<string> _text = new List<string>();

        protected string _message;

        public string Message => _message;

        public Bookshelf(Vector2 initialPosition, 
                            Size2 size, 
                            string message,
                            string title,
                            List<string> items, 
                            List<string> text)
            : base(initialPosition, size)
        {
            _message = message;

            _title = title;
            _items = items;
            _text = text;
        }

        public virtual string GetMessage()
        {
            return Message;
        }

        public override void Interact(Item item, Action onInteractionStart, Action onInteractionEnd)
        {
            HarvestMoon.Instance.GUI.ShowMessage(GetMessage(), onInteractionStart, () =>
            {
                HarvestMoon.Instance.GUI.ShowBookshelf(_title, _items, _text, onInteractionEnd);
            });

        }
    }
}
