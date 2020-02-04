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
    internal class BeeAnatomyBookshelf: Bookshelf
    {
        static List<string> GetItems()
        {
            return new List<string> { "Intro", "Head", "Abdomen", "Thorax"};
        }

        static List<string> GetText()
        {
            return new List<string> {
                "Despite their small size, honeybees are complex creatures with many fascinating characteristics, from long tongues that suck up nectar to stiff hairs that collect pollen. Their bodies are designed specifically for the role they play in nature.",
                "The head of a honeybee includes the antennae, eyes, and mouth. The antennae have thousands of smell, taste, and touch receptors that help the bee navigate the world around it. Because they spend much of their time in poorly lit hives, bees rely more on their antennae than on their eyes. This is why their eyes are designed to detect motion rather than to see high-resolution images. Bees have five eyes: two compound eyes that sense and focus on light, and three simple eyes that don't focus but instead provide information on the intensity of the light around them. Finally, a honeybee's mouth contains organs that allow them to suck and chew materials. The mandibles act as teeth and allow the bee to chew wood or wax. The proboscis is a tube-like organ that lets the bee suck in nectar, water, and honey.",
                "A honeybee's abdomen appears less complicated than other parts of the body, but it houses important organs such as the digestive tract and—in queen bees—the reproductive system. Worker bees have wax scales on their abdomens, produced by their own wax glands when they're a week or two old. The abdomen is also the location of the stinger and venom glands. Only female bees have stingers: sharp, barbed organs that bees use to attack predators and inject them with venom.",
                "The thorax is the location of a bee's legs and wings. Bees have two sets of wings: larger front wings and smaller hind wings, which work together in perfect synchronization during flight. The thorax also houses two sets of flight muscles—vertical and longitudinal—that contract in turn to raise and lower the wings. A honeybee's legs have stiff hairs that allow it to collect pollen and brush itself clean. Worker bees also have special pollen baskets on their hind legs. This allows them to brush pollen from their bodies and into the basket for safe transport back to the hive."
            }; 
        }

        public BeeAnatomyBookshelf(Vector2 initialPosition,
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
