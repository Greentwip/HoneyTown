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
    internal class LivestockDealerStore: UpfrontStore
    {
        static List<string> GetItems()
        {
            return new List<string> { "Cow", "Sheep", "Chicken" };
        }

        static List<string> GetClasses()
        {
            return new List<string> { "Cattle", "Cattle", "Poultry" }; 
        }

        static List<int> GetPrices()
        {
            return new List<int> { 5000, 3000, 1000 };
        }

        static Func<List<string>, List<int>, int, string> GetPurchaseCallback()
        {
            return (List<string> purchases, List<int> amounts, int total) =>
            {
                if (purchases.Count == 0)
                {
                    ConfirmPurchase = false;
                    return "There is nothing to buy";
                }

                if (total > HarvestMoon.Instance.Gold)
                {
                    ConfirmPurchase = false;
                    return "Not enough Gold";
                }

                if (!ConfirmPurchase)
                {
                    ConfirmPurchase = true;
                    return "Please confirm purchase";
                }
                else
                {
                    ConfirmPurchase = false;
                }

                for (int i = 0; i < purchases.Count; ++i)
                {
                    string purchase = purchases[i].ToLower();

                    if (purchase == "cow")
                    {
                        if (amounts[i] + HarvestMoon.Instance.Cows > 8)
                        {
                            return "You can't have more than 8 Cows";
                        }
                    }


                    if (purchase == "sheep")
                    {
                        if (amounts[i] + HarvestMoon.Instance.Sheeps > 8)
                        {
                            return "You can't have more than 8 Sheeps";
                        }
                    }


                    if (purchase == "chicken")
                    {
                        if (amounts[i] + HarvestMoon.Instance.Cows > 10)
                        {
                            return "You can't have more than 10 Chickens";
                        }
                    }

                }

                for (int i = 0; i < purchases.Count; ++i)
                {
                    string purchase = purchases[i].ToLower();

                    if (purchase == "cow")
                    {
                        int openIndex = -1;
                        for (int j = 0; j < HarvestMoon.Instance.CowAliveList.Count; ++j)
                        {
                            if (!HarvestMoon.Instance.CowAliveList[j])
                            {
                                openIndex = i;
                                break;
                            }
                        }

                        HarvestMoon.Instance.CowAliveList[openIndex] = true;
                        HarvestMoon.Instance.CowNameIndexDictionary.Add("cow-" + openIndex.ToString(), openIndex);
                    }

                    if (purchase == "sheep")
                    {
                        HarvestMoon.Instance.Sheeps += amounts[i];
                    }


                    if (purchase == "chicken")
                    {
                        HarvestMoon.Instance.Chickens += amounts[i];
                    }

                }

                HarvestMoon.Instance.Gold -= total;

                return "Thanks for your purchase";
            };
        }

        public LivestockDealerStore(Vector2 initialPosition,
                            Size2 size,
                            string message,
                            string title):
           base(initialPosition,
                size,
                message,
                title,
                GetItems(),
                GetClasses(),
                GetPrices(),
                GetPurchaseCallback())
        {
            ConfirmPurchase = false;
        }
    }
}
