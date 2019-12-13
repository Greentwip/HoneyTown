using System;

namespace HarvestMoon.Entities.General
{
    public sealed class Message
    {
        public string Text { get; }

        public Action Callback { get; }

        public Message(string text, Action callback)
        {
            Text = text;
            Callback = callback;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}