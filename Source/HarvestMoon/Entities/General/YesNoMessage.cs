using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace HarvestMoon.Entities.General
{
    public class YesNoMessage : NPC
    {
        protected List<Action> _callbacks = new List<Action>();

        protected List<string> _strings = new List<string>();

        public List<string> Strings => _strings;
        public List<Action> Callbacks => _callbacks;

        protected string _message;

        public string Message => _message;

        public YesNoMessage(Vector2 initialPosition, Size2 size, string message, List<string> strings, List<Action> callbacks)
            : base(initialPosition, size)
        {
            _message = message;

            _strings = strings;

            _callbacks = callbacks;
        }

        public virtual string GetMessage()
        {
            return Message;
        }

        public override void Interact(Item item, Action onInteractionStart, Action onInteractionEnd)
        {
            HarvestMoon.Instance.GUI.ShowMessage(GetMessage(), onInteractionStart, () =>
            {

                HarvestMoon.Instance.GUI.ShowYesNoMessage(Strings[0],
                                            Strings[1],
                                            () =>
                                            {
                                                Callbacks[0]();
                                                onInteractionEnd();
                                            },
                                            () =>
                                            {
                                                Callbacks[1]();
                                                onInteractionEnd();
                                            });
            });

        }

    }
}
