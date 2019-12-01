using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace HarvestMoon.Entities.General
{
    public class BasicMessage : NPC
    {

        protected string _message;

        public string Message => _message;

        public BasicMessage(Vector2 initialPosition, Size2 size, string message)
            : base(initialPosition, size)
        {
            _message = message;
        }

        public virtual string GetMessage()
        {
            return Message;
        }

        public override void Interact(Item item, Action onInteractionStart, Action onInteractionEnd)
        {
            HarvestMoon.Instance.GUI.ShowMessage(GetMessage(), onInteractionStart, onInteractionEnd);
        }

    }
}
