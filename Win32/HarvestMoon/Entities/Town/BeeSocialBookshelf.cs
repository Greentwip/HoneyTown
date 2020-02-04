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
    internal class BeeSocialBookshelf: Bookshelf
    {
        static List<string> GetItems()
        {
            return new List<string> { "Intro", "Queen", "Workers", "Laying workers", "Drones"};
        }

        static List<string> GetText()
        {
            return new List<string> {
                "Unlike most insects, honey bees are social creatures. They live in well-maintained and organized colonies, relying on the hard work of themselves and their brethren to keep the hive safe and healthy. The complexity of the hive—from the communication to the division of labor to behaviors like environmental regulation and defense—makes these highly evolved creatures downright fascinating. So, what is the social structure of honey bees?",
                "Everyone's heard of the queen bee. Colonies can have tens of thousands of bees, but there's only one queen. She also lives far longer than the others—four years to a worker bee's six or so weeks. Obviously, she's the most important member of the hive. The queen's main responsibility is reproduction. She lays all the eggs in the colony, and she's the only one who can produce fertilized eggs. This is because, shortly after a new queen emerges from her cell, she goes on her mating flight. By herself, she leaves the hive and flies a distance away to mate with drones from other colonies. A couple of days after the mating flight, the queen begins to lay eggs. She will continue reproducing fertilized and unfertilized eggs (creating worker bees and drones, respectively) for the next couple of years. That is until she dies or becomes too old to lay.",
                "If you see a honey bee flying around, chances are it’s a worker bee. These bees make up the vast majority of the colony. Worker bees are all females. However, unlike the queen, they are not as sexually developed. Instead of reproducing, they do all the work in the hive. Worker bees are responsible for forming the hive cells, making honey and beeswax, taking care of the queen, and defending the hive. A subset of the worker bees is nurse bees, who feed and take care of the brood. Other workers are field bees. They're the ones who forage for nectar, pollen, and other resources for the hive. The worker bees share these jobs throughout their life. They start by working in the hive before eventually becoming field bees.",
                "If a colony loses its queen and can't replace her right away, some of the worker bees will begin to lay eggs. However, because these workers never went on a mating flight, they'll only be able to lay unfertilized eggs. This results in new drones, but no new workers, throwing off the balance of the colony. With no new populations to take care of the brood, make honey, and forage for resources, the hive quickly fails.",
                "The final type of adult bee is the drone. These are the male bees in the hive. They're larger than their female counterparts—though not as big as the queen—and they don't have a stinger, pollen baskets, or wax glands. Their main responsibility is to fertilize another colony's queen on her mating flight, and they die after mating. While they don’t make any contributions to their own hive, they play an important role in spreading genetics throughout bee populations. The more drones a queen mates with, the greater the genetic diversity of her colony is, and the more successful the hive will be. However, because they can drain resources, the worker bees will often push out any drones in the hive in preparation for winter."
            }; 
        }

        public BeeSocialBookshelf(Vector2 initialPosition,
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
