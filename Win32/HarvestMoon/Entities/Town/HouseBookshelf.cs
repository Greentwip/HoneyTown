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
            return new List<string> {
                "To use the hoe you should locate an unplotted piece of land, then press the action button to plot the land.",
                "After wheat has grown you can use the sickle to collect it, don't forget that grown crops can also be cut off, you don't get the harvest with this.",
                "This tool is used to break stones, required for mining. To break big stones you have to focus in-place without input and hit six times the rock.",
                "Used to collect wood pieces. You can cut small chunks with the action button, to cut bigger logs you have to focus without any input and slash 6 times with the axe.",
                "You have to fill the watering can at a river or a water pond, then water plants daily to make them grow.",
                "After you plot the land, press the action button to plant seeds on the soil. Some of the plants you can grow are turnips, potatoes, corn and tomatoes." }; 
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
