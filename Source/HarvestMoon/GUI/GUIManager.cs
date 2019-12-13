using GeonBit.UI;
using GeonBit.UI.Animators;
using GeonBit.UI.Entities;
using HarvestMoon.Entities.General;
using HarvestMoon.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;


namespace HarvestMoon.GUI
{
    public class GUIManager
    {
        Panel _textPanel;
        Paragraph _textParagraph;
        TypeWriterAnimator _textAnimator;

        Action _onAfterConfirmCallback;

        private List<string> _bufferedStrings;

        private bool _busy;

        private bool _npcCoolDown;

        private float _coolingTimer = 0;

        private float _npcCoolingDelay = 0.3f;

        private bool _isActionButtonDown = false;
        private bool _isUpButtonDown = false;
        private bool _isDownButtonDown = false;


        public void Update(GameTime gameTime)
        {
            // GeonBit.UIL update UI manager
            UserInterface.Active.Update(gameTime);
            var deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_npcCoolDown)
            {
                _coolingTimer += deltaSeconds;

                if (_coolingTimer >= _npcCoolingDelay)
                {
                    _coolingTimer = 0.0f;
                    _npcCoolDown = false;
                    _busy = false;
                }
            }

            var keyboardState = HarvestMoon.Instance.Input;

            if (keyboardState.IsKeyUp(InputDevice.Keys.A))
            {
                _isActionButtonDown = false;
            }

            if (keyboardState.IsKeyUp(InputDevice.Keys.Up))
            {
                _isUpButtonDown = false;
            }

            if (keyboardState.IsKeyUp(InputDevice.Keys.Down))
            {
                _isDownButtonDown = false;
            }

            if ((keyboardState.IsKeyDown(InputDevice.Keys.Up) && !_isUpButtonDown) || (keyboardState.IsKeyDown(InputDevice.Keys.Down) && !_isDownButtonDown))
            {
                if (keyboardState.IsKeyDown(InputDevice.Keys.Up))
                {
                    _isUpButtonDown = true;
                }

                if (keyboardState.IsKeyDown(InputDevice.Keys.Down))
                {
                    _isDownButtonDown = true;
                }

                if (_isDisplayingMenu)
                {
                    _textParagraph.Text = _menu.Text(_isDownButtonDown, _isUpButtonDown);
                }
            }

            if (keyboardState.IsKeyDown(InputDevice.Keys.A) && !_isActionButtonDown)
            {
                _isActionButtonDown = true;
                if (_textPanel != null)
                {
                    if (_isDisplayingMenu)
                    {
                        UserInterface.Active.RemoveEntity(_textPanel);
                        _textPanel = null;
                        _npcCoolDown = true;
                        _busy = false;
                        _isDisplayingMenu = false;
                        _menu?.CurrentMessage?.Callback?.Invoke();
                        _menu = null;
                    }
                    else
                    {
                        if (_textAnimator != null)
                        {
                            if (_textAnimator.IsDone)
                            {
                                if (_bufferedStrings.Count > 0)
                                {
                                    _textAnimator.TextToType = _bufferedStrings.First();
                                    _bufferedStrings.Remove(_bufferedStrings.First());
                                }
                                else
                                {
                                    UserInterface.Active.RemoveEntity(_textPanel);
                                    _textPanel = null;
                                    _npcCoolDown = true;
                                    _busy = true;

                                    if (_onAfterConfirmCallback != null)
                                    {
                                        _onAfterConfirmCallback();
                                        _onAfterConfirmCallback = null;
                                    }

                                }
                            }
                        }
                    }
                }
            }

        }

        public List<string> SplitByLength(string sentence, int partLength)
        {
            string[] words = sentence.Split(' ');
            var parts = new List<string>();
            string part = string.Empty;
            int partCounter = 0;
            foreach (var word in words)
            {
                if (part.Length + word.Length < partLength)
                {
                    part += string.IsNullOrEmpty(part) ? word : " " + word;
                }
                else
                {
                    parts.Add(part);
                    part = word;
                    partCounter++;
                }
            }
            parts.Add(part);

            return parts;
        }

        private bool _isDisplayingMenu;

        private SelectableMenu _menu;

        public void ShowSelectableMenu(SelectableMenu menu)
        {
            _isDisplayingMenu = true;
            if (_textPanel != null)
            {
                UserInterface.Active.RemoveEntity(_textPanel);
                _textPanel = null;
            }

            _textPanel = NewTextPanel();
            UserInterface.Active.AddEntity(_textPanel);
            _textParagraph = new Paragraph("") {Text = menu.Text()};
            _textPanel.AddChild(_textParagraph);
            _textPanel.Opacity = 200;
            _menu = menu;
        }

        public void ShowMessage(string message, Action onStartCallback, Action onAfterConfirmCallback)
        {
            if (_textPanel != null)
            {
                UserInterface.Active.RemoveEntity(_textPanel);
                _textPanel = null;
            }

            _onAfterConfirmCallback = onAfterConfirmCallback;

            _bufferedStrings = SplitByLength(message, 130);

            _textPanel = NewTextPanel();

            UserInterface.Active.AddEntity(_textPanel);

            var paragraph = new Paragraph("");

            _textAnimator = new TypeWriterAnimator();

            _textAnimator.TextToType = _bufferedStrings.First();
            _bufferedStrings.Remove(_bufferedStrings.First());

            paragraph.AttachAnimator(_textAnimator);

            _textPanel.AddChild(paragraph);

            _textPanel.Opacity = 200;

            onStartCallback();
        }

        private static Panel NewTextPanel()
        {
            float scaleY = HarvestMoon.Instance.Graphics.GraphicsDevice.Viewport.Height / (1.0f * Screens.GUI.PixelHeight);

            return new Panel(new Vector2(Screens.GUI.PixelWidth * scaleY, Screens.GUI.PixelHeight / 4f * scaleY), PanelSkin.Default, Anchor.BottomCenter);
        }
    }
}
