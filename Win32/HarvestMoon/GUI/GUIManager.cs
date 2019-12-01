using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.ViewportAdapters;
using System.Collections.Generic;
using HarvestMoon.Entities;
using System.Linq;

using GeonBit.UI;
using GeonBit.UI.Entities;

using GeonBit.UI.Animators;
using System;
using static HarvestMoon.Entities.General.NPC;
using HarvestMoon.Entities.Ranch;
using HarvestMoon.Input;


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
                    switch (_npcMenu)
                    {
                        case NPCMenu.YesNo:
                            if (_textParagraph.Text == "> " + _menuStrings.First() + "\n" + "  " + _menuStrings.Last())
                            {
                                _textParagraph.Text = "  " + _menuStrings.First() + "\n" + "> " + _menuStrings.Last();
                            }
                            else
                            {
                                _textParagraph.Text = "> " + _menuStrings.First() + "\n" + "  " + _menuStrings.Last();
                            }

                            break;
                    }
                }
            }


            if (keyboardState.IsKeyDown(InputDevice.Keys.A) && !_isActionButtonDown)
            {
                _isActionButtonDown = true;
                if (_textPanel != null)
                {
                    if (_isDisplayingMenu)
                    {
                        switch (_npcMenu)
                        {
                            case NPCMenu.YesNo:
                                if (_textParagraph.Text == "> " + _menuStrings.First() + "\n" + "  " + _menuStrings.Last())
                                {
                                    UserInterface.Active.RemoveEntity(_textPanel);
                                    _textPanel = null;
                                    _npcCoolDown = true;
                                    _busy = true;
                                    _isDisplayingMenu = false;
                                    _menuCallbacks[0]();
                                }
                                else
                                {
                                    UserInterface.Active.RemoveEntity(_textPanel);
                                    _textPanel = null;
                                    _npcCoolDown = true;
                                    _busy = true;
                                    _isDisplayingMenu = false;
                                    _menuCallbacks[1]();
                                }

                                break;
                        }
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

        private List<string> _menuStrings;
        private List<Action> _menuCallbacks;

        NPCMenu _npcMenu;

        // This must be on a separated class
        public void ShowYesNoMessage(string yesString, string noString, Action yesCallback, Action noCallback)
        {
            _isDisplayingMenu = true;

            _menuStrings = new List<string>();
            _menuStrings.Add(yesString);
            _menuStrings.Add(noString);

            _menuCallbacks = new List<Action>();

            _menuCallbacks.Add(yesCallback);
            _menuCallbacks.Add(noCallback);

            if (_textPanel != null)
            {
                UserInterface.Active.RemoveEntity(_textPanel);
                _textPanel = null;
            }

            float scaleY = HarvestMoon.Instance.Graphics.GraphicsDevice.Viewport.Height / 480.0f;

            // create a panel and position in bottom center of screen
            _textPanel = new Panel(new Vector2(320 * scaleY, 120 * scaleY), PanelSkin.Default, Anchor.BottomCenter);

            UserInterface.Active.AddEntity(_textPanel);

            _textParagraph = new Paragraph("");

            _textParagraph.Text = "> " + _menuStrings.First() + "\n" + "  " + _menuStrings.Last();

            _textPanel.AddChild(_textParagraph);

            _textPanel.Opacity = 200;

            _npcMenu = NPCMenu.YesNo;
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

            float scaleY = HarvestMoon.Instance.Graphics.GraphicsDevice.Viewport.Height / 480.0f;

            // create a panel and position in bottom center of screen
            _textPanel = new Panel(new Vector2(320 * scaleY, 120 * scaleY), PanelSkin.Default, Anchor.BottomCenter);

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
    }
}
