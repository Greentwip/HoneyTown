using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;


namespace HarvestMoon.Entities.General
{
    public class UpfrontStore: NPC
    {
        private string _title;

        public static bool ConfirmPurchase;


        protected List<string> _items = new List<string>();
        protected List<string> _classes = new List<string>();
        protected List<int> _prices = new List<int>();

        private Func<List<string>, List<int>, int, string> _onPurchaseCallback;

        protected string _message;

        public string Message => _message;

        public UpfrontStore(Vector2 initialPosition, 
                            Size2 size, 
                            string message,
                            string title,
                            List<string> items, 
                            List<string> classes, 
                            List<int> prices,
                            Func<List<string>, List<int>, int, string> onPurchaseCallback)
            : base(initialPosition, size)
        {
            _message = message;

            _title = title;
            _items = items;
            _classes = classes;
            _prices = prices;
            _onPurchaseCallback = onPurchaseCallback;

        }

        public virtual string GetMessage()
        {
            return Message;
        }

        public override void Interact(Item item, Action onInteractionStart, Action onInteractionEnd)
        {
            HarvestMoon.Instance.GUI.ShowMessage(GetMessage(), onInteractionStart, () =>
            {

                HarvestMoon.Instance.GUI.ShowUpfrontStore(_title, _items, _classes, _prices, onInteractionEnd, (List<string> purchases, List<int> amounts, int price)=> {

                    return _onPurchaseCallback?.Invoke(purchases, amounts, price);

                });
            });

        }
    }
}
