using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;


namespace HarvestMoon.Entities.General
{
    class UpfrontStore: NPC
    {
        private string _title;

        protected List<string> _items = new List<string>();
        protected List<string> _classes = new List<string>();
        protected List<int> _prices = new List<int>();

        private System.Action<string, int, int> _onPurchaseCallback;

        protected string _message;

        public string Message => _message;

        public UpfrontStore(Vector2 initialPosition, 
                            Size2 size, 
                            string message,
                            string title,
                            List<string> items, 
                            List<string> classes, 
                            List<int> prices,
                            System.Action<string, int, int> onPurchaseCallback)
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

                HarvestMoon.Instance.GUI.ShowUpfrontStore(_title, _items, _classes, _prices, (string purchase, int amount, int price)=> {

                    _onPurchaseCallback?.Invoke(purchase, amount, price);
                    onInteractionEnd();

                });
            });

        }
    }
}
