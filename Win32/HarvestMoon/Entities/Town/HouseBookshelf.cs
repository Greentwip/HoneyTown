using HarvestMoon.Entities.General;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarvestMoon.Entities.Town
{
    internal class HouseBookshelf: Bookshelf
    {
        static List<string> GetItems()
        {
            return new List<string> { "Hoe", "Sickle", "Hammer", "Axe", "Watering can", "Seeds" };
        }

        static List<string> GetText()
        {
            return new List<string> { "Hoe", "Sickle", "Hammer", "Axe", "Watering can", "Seeds" }; 
        }

        public HouseBookshelf(Vector2 initialPosition,
                            Size2 size,
                            string message,
                            string title):
           base(initialPosition,
                size,
                message,
                title,
                GetItems(),
                GetText())
        {
            Reading = false;
        }
    }
}
