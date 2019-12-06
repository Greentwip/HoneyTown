using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace HarvestMoon.Entities.General
{
    public class YesNoMessage : SelectableMenu
    {
        public YesNoMessage(Vector2 initialPosition, Size2 size, string message, string yesText, string noText,
            Action yesCallback, Action noCallback)
            : base(initialPosition, size, "",
            new List<Message>(2) {new Message(yesText, yesCallback), new Message(noText, noCallback)})
        {

        }
    }
}