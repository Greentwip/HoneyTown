using HarvestMoon.Entities.General;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarvestMoon.Entities.Ranch
{
    public class FooderSignPost : NPC
    {
        public FooderSignPost(Vector2 initialPosition, Size2 size, bool displaysMessage, string message)
            : base(initialPosition, size, displaysMessage, message)
        {

        }

        public override string GetMessage(Item item)
        {
            var replacedPieces = Message.Replace("number", HarvestMoon.Instance.FeedPieces.ToString());
            return replacedPieces;
        }
    }
}
