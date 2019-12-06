using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace HarvestMoon.Entities.General
{
    public class SelectableMenu : NPC
    {
        private static readonly string SelectedItem = "> ";
        private static readonly string NonSelectedItemPadding = "  ";

        private readonly string _initialMessage;

        private int _messageIndex;
        private readonly List<Message> _menuItems;

        public Message CurrentMessage => _menuItems[_messageIndex];

        public SelectableMenu(Vector2 initialPosition, Size2 size, string initialMessage, List<Message> menuItems)
            : base(initialPosition, size)
        {
            _initialMessage = initialMessage;
            _menuItems = menuItems;
            _messageIndex = 0;
        }

        public string Text(bool downKey = false, bool upKey = false)
        {
            UpdateCurrentMessage(downKey, upKey);

            StringBuilder menuBuilder = new StringBuilder();

            for (int i = 0; i < _menuItems.Count; ++i)
            {
                if (_messageIndex == i)
                {
                    menuBuilder.Append(SelectedItem);
                }
                else
                {
                    menuBuilder.Append(NonSelectedItemPadding);
                }

                menuBuilder.Append(_menuItems[_messageIndex]);
            }
            menuBuilder.AppendLine();

            return menuBuilder.ToString();
        }

        public override void Interact(Item item, Action onInteractionStart, Action onInteractionEnd)
        {
            HarvestMoon.Instance.GUI.ShowMessage(_initialMessage, onInteractionStart, () =>
            {
                // TODO: Curry onInteractionEnd
                HarvestMoon.Instance.GUI.ShowSelectableMenu(this);
            });
        }

        private void UpdateCurrentMessage(bool downKey, bool upKey)
        {
            if (upKey)
            {
                _messageIndex++;
                _messageIndex = _messageIndex % _menuItems.Count;
                return;
            }

            if (downKey)
            {
                if (_messageIndex == 0)
                {
                    _messageIndex = _menuItems.Count - 1;
                }
                else
                {
                    _messageIndex--;
                }
            }
        }
    }
}