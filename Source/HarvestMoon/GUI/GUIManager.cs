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
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using HarvestMoon.Entities.General;

namespace HarvestMoon.GUI
{
    public class GUIManager
    {
        public enum NPCMenu
        {
            YesNo,
            UpfrontStore
        }

        Panel _textPanel;
        Paragraph _textParagraph;
        TypeWriterAnimator _textAnimator;

        Action _onAfterConfirmCallback;
        Action _storeOnAfterConfirmCallback;

        private List<string> _bufferedStrings;

        private bool _busy;

        private bool _npcCoolDown;

        private float _coolingTimer = 0;

        private float _npcCoolingDelay = 0.3f;

        private bool _isActionButtonDown = false;
        private bool _isCancelButtonDown = false;
        private bool _isUpButtonDown = false;
        private bool _isDownButtonDown = false;
        private bool _isLeftButtonDown = false;
        private bool _isRightButtonDown = false;

        private bool _isDisplayingMenu;

        private List<string> _menuStrings;
        private List<Action> _menuCallbacks;


        // upfront store
        private int _selectedIndex;
        private int _selectedAmount;
        private int _selectedTotal;
        private List<string> _items;
        private List<string> _classes;
        private List<int> _prices;
        private List<int> _amounts;

        private Paragraph _gParagraph;
        private Paragraph _totalParagraph;

        SelectList _upfrontStoreList;

        private Func<List<string>, List<int>, int, string> _onPurchaseCallback;


        NPCMenu _npcMenu;

        private AnimatedSprite _dayToolSprite;
        private AnimatedSprite _goldSprite;
        private List<AnimatedSprite> _staminaSprites = new List<AnimatedSprite>();

        private Dictionary<string, Sprite> _holdingItemSprites = new Dictionary<string, Sprite>();

        private Paragraph _numberDaySeasonParagraph;
        private Paragraph _goldParagraph;

        private Texture2D _window_11Texture;


        public GUIManager(ContentManager content)
        {
            float frameDuration = 1.0f / 7.5f;

            var characterTexture = content.Load<Texture2D>("ui/objects");
            var characterMap = content.Load<Dictionary<string, Rectangle>>("ui/uiMap");
            var characterAtlas = new TextureAtlas("ui", characterTexture, characterMap);
            var characterAnimationFactory = new SpriteSheet
            {
                TextureAtlas = characterAtlas,
                Cycles =
                {
                    {
                        "day_tool", new SpriteSheetAnimationCycle
                        {
                            IsLooping = true,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                // TODO: Fix per frame duration
                                new SpriteSheetAnimationFrame(0)
                            }
                        }
                    },
                    {
                        "gold", new SpriteSheetAnimationCycle
                        {
                            IsLooping = true,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                // TODO: Fix per frame duration
                                new SpriteSheetAnimationFrame(1)
                            }
                        }
                    },
                    {
                        "heart_full", new SpriteSheetAnimationCycle
                        {
                            IsLooping = true,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                // TODO: Fix per frame duration
                                new SpriteSheetAnimationFrame(2)
                            }
                        }
                    },
                    {
                        "heart_quarter", new SpriteSheetAnimationCycle
                        {
                            IsLooping = true,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                // TODO: Fix per frame duration
                                new SpriteSheetAnimationFrame(3)
                            }
                        }
                    },
                    {
                        "heart_half", new SpriteSheetAnimationCycle
                        {
                            IsLooping = true,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                // TODO: Fix per frame duration
                                new SpriteSheetAnimationFrame(4)
                            }
                        }
                    },
                    {
                        "heart_three_quarters", new SpriteSheetAnimationCycle
                        {
                            IsLooping = true,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                // TODO: Fix per frame duration
                                new SpriteSheetAnimationFrame(5)
                            }
                        }
                    },
                    {
                        "heart_empty", new SpriteSheetAnimationCycle
                        {
                            IsLooping = true,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                // TODO: Fix per frame duration
                                new SpriteSheetAnimationFrame(6)
                            }
                        }
                    }
                }
            };

            _dayToolSprite = new AnimatedSprite(characterAnimationFactory, "day_tool");
            _goldSprite = new AnimatedSprite(characterAnimationFactory, "gold");
            _staminaSprites.Add(new AnimatedSprite(characterAnimationFactory, "heart_full"));
            _staminaSprites.Add(new AnimatedSprite(characterAnimationFactory, "heart_full"));
            _staminaSprites.Add(new AnimatedSprite(characterAnimationFactory, "heart_full"));
            _staminaSprites.Add(new AnimatedSprite(characterAnimationFactory, "heart_full"));

            _holdingItemSprites.Add("axe", new Sprite(content.Load<Texture2D>("maps/tools-room/items/axe")));
            _holdingItemSprites.Add("grass-seeds", new Sprite(content.Load<Texture2D>("maps/tools-room/items/grass-seeds")));
            _holdingItemSprites.Add("turnip-seeds", new Sprite(content.Load<Texture2D>("maps/tools-room/items/turnip-seeds")));
            _holdingItemSprites.Add("potato-seeds", new Sprite(content.Load<Texture2D>("maps/tools-room/items/potato-seeds")));
            _holdingItemSprites.Add("tomato-seeds", new Sprite(content.Load<Texture2D>("maps/tools-room/items/tomato-seeds")));
            _holdingItemSprites.Add("corn-seeds", new Sprite(content.Load<Texture2D>("maps/tools-room/items/corn-seeds")));
            _holdingItemSprites.Add("hammer", new Sprite(content.Load<Texture2D>("maps/tools-room/items/hammer")));
            _holdingItemSprites.Add("hoe", new Sprite(content.Load<Texture2D>("maps/tools-room/items/hoe")));
            _holdingItemSprites.Add("sickle", new Sprite(content.Load<Texture2D>("maps/tools-room/items/sickle")));
            _holdingItemSprites.Add("watering-can", new Sprite(content.Load<Texture2D>("maps/tools-room/items/watering-can")));

            _window_11Texture = content.Load<Texture2D>("ui/window_10");
        }

        public void Update(GameTime gameTime)
        {
            _dayToolSprite.Update(gameTime);
            _goldSprite.Update(gameTime);

            foreach(var staminaSprite in _staminaSprites)
            {
                staminaSprite.Update(gameTime);
            }

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

            if (keyboardState.IsKeyUp(InputDevice.Keys.B))
            {
                _isCancelButtonDown = false;
            }

            if (keyboardState.IsKeyUp(InputDevice.Keys.Up))
            {
                _isUpButtonDown = false;
            }

            if (keyboardState.IsKeyUp(InputDevice.Keys.Down))
            {
                _isDownButtonDown = false;
            }

            if (keyboardState.IsKeyUp(InputDevice.Keys.Left))
            {
                _isLeftButtonDown = false;
            }

            if (keyboardState.IsKeyUp(InputDevice.Keys.Right))
            {
                _isRightButtonDown = false;
            }

            if ((keyboardState.IsKeyDown(InputDevice.Keys.Left) && !_isLeftButtonDown))
            {
                _isLeftButtonDown = true;

                if (_isDisplayingMenu)
                {
                    switch (_npcMenu)
                    {
                        case NPCMenu.UpfrontStore:
                            if (!UpfrontStore.ConfirmPurchase)
                            {
                                _upfrontStoreList.ClearItems();

                                _upfrontStoreList.LockedItems[0] = true;
                                _upfrontStoreList.AddItem(System.String.Format("{0}{1,-8} {2,-8} {3, -10} {4, -10}", "{{RED}}", "Name", "Class", "Price", "Amount"));

                                _amounts[_selectedIndex - 1] = _amounts[_selectedIndex - 1] - 1;

                                if (_amounts[_selectedIndex - 1] < 0)
                                {
                                    _amounts[_selectedIndex - 1] = 0;
                                }

                                for (int i = 0; i < _items.Count; ++i)
                                {
                                    // add items as formatted table
                                    _upfrontStoreList.AddItem(System.String.Format("{0,-8} {1,-8} {2,-10} {3, -10}", _items[i], _classes[i], _prices[i].ToString(), _amounts[i].ToString()));
                                }

                                UpdateTotal();


                            }
                            break;
                    }

                }

            }

            if ((keyboardState.IsKeyDown(InputDevice.Keys.Right) && !_isRightButtonDown))
            {
                _isRightButtonDown = true;


                if (_isDisplayingMenu)
                {
                    switch (_npcMenu)
                    {
                        case NPCMenu.UpfrontStore:
                            if (!UpfrontStore.ConfirmPurchase)
                            {
                                _upfrontStoreList.ClearItems();

                                _upfrontStoreList.LockedItems[0] = true;
                                _upfrontStoreList.AddItem(System.String.Format("{0}{1,-8} {2,-8} {3, -10} {4, -10}", "{{RED}}", "Name", "Class", "Price", "Amount"));

                                _amounts[_selectedIndex - 1] = _amounts[_selectedIndex - 1] + 1;

                                if (_amounts[_selectedIndex - 1] > 10)
                                {
                                    _amounts[_selectedIndex - 1] = 10;
                                }

                                for (int i = 0; i < _items.Count; ++i)
                                {
                                    // add items as formatted table
                                    _upfrontStoreList.AddItem(System.String.Format("{0,-8} {1,-8} {2,-10} {3, -10}", _items[i], _classes[i], _prices[i].ToString(), _amounts[i].ToString()));
                                }

                                UpdateTotal();


                            }
                            break;
                    }
                }
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

                        case NPCMenu.UpfrontStore:
                            if (!UpfrontStore.ConfirmPurchase)
                            {
                                if (_isDownButtonDown)
                                {
                                    _selectedIndex++;

                                    if (_selectedIndex == _upfrontStoreList.Items.Length)
                                    {
                                        _selectedIndex = 1;
                                    }
                                }

                                if (_isUpButtonDown)
                                {
                                    _selectedIndex--;

                                    if (_selectedIndex == 0)
                                    {
                                        _selectedIndex = _upfrontStoreList.Items.Length - 1;
                                    }
                                }

                            }
                            break;
                    }
                }
            }

            if (_isDisplayingMenu)
            {
                if(_npcMenu == NPCMenu.UpfrontStore)
                {
                    _gParagraph.Text = "Current G: " + HarvestMoon.Instance.Gold.ToString() + "G";

                    if (_upfrontStoreList.SelectedIndex != _selectedIndex)
                    {
                        _upfrontStoreList.SelectedIndex = _selectedIndex;
                        _selectedAmount = 0;
                        _selectedTotal = 0;
                    }

                }
            }

            if(keyboardState.IsKeyDown(InputDevice.Keys.B) && !_isCancelButtonDown)
            {
                _isCancelButtonDown = true;

                if (_textPanel != null)
                {
                    if (_isDisplayingMenu)
                    {
                        if(_npcMenu == NPCMenu.UpfrontStore)
                        {
                            if (UpfrontStore.ConfirmPurchase)
                            {
                                UpdateTotal();
                                UpfrontStore.ConfirmPurchase = false;
                            }
                            else
                            {
                                UserInterface.Active.RemoveEntity(_textPanel);
                                _textPanel = null;
                                _npcCoolDown = true;
                                _busy = true;
                                _isDisplayingMenu = false;

                                _storeOnAfterConfirmCallback?.Invoke();
                                _storeOnAfterConfirmCallback = null;

                            }
                        }
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

                            case NPCMenu.UpfrontStore:
                                _npcCoolDown = true;
                                _busy = true;

                                List<string> items = new List<string>();
                                List<int> amounts = new List<int>();

                                for (int i = 0; i < _items.Count; ++i)
                                {
                                    if (_amounts[i] > 0)
                                    {
                                        items.Add(_items[i]);
                                        amounts.Add(_amounts[i]);
                                    }
                                }

                                UpdateTotal();

                                var result = _onPurchaseCallback?.Invoke(items, amounts, _selectedTotal);

                                if(!UpfrontStore.ConfirmPurchase)
                                {
                                    for (int i = 0; i < _amounts.Count; ++i)
                                    {
                                        _amounts[i] = 0;
                                    }

                                    UpdateTotal(result);

                                    _upfrontStoreList.ClearItems();

                                    _upfrontStoreList.LockedItems[0] = true;
                                    _upfrontStoreList.AddItem(System.String.Format("{0}{1,-8} {2,-8} {3, -10} {4, -10}", "{{RED}}", "Name", "Class", "Price", "Amount"));

                                    for (int i = 0; i < _items.Count; ++i)
                                    {
                                        // add items as formatted table
                                        _upfrontStoreList.AddItem(System.String.Format("{0,-8} {1,-8} {2,-10} {3, -10}", _items[i], _classes[i], _prices[i].ToString(), _amounts[i].ToString()));
                                    }

                                }
                                else
                                {
                                    UpdateTotal(result);
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

                                    _onAfterConfirmCallback?.Invoke();
                                    _onAfterConfirmCallback = null;
                                }
                            }
                        }
                    }
                }
            }

        }

        private void UpdateTotal(string withMessage = "")
        {
            _selectedTotal = 0;

            for (int i = 0; i < _amounts.Count; ++i)
            {
                _selectedTotal += _amounts[i] * _prices[i];
            }

            _totalParagraph.Text = "Total: " + _selectedTotal.ToString() + "G" + " " + withMessage;
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


        // onPurchaseCallback name, amount, total
        public void ShowUpfrontStore(string title, 
                                     List<string> items, 
                                     List<string> classes, 
                                     List<int> prices, 
                                     Action onInteractionEnd,
                                     Func<List<string>, List<int>, int, string> onPurchaseCallback)
        {
            _isDisplayingMenu = true;

            if (_textPanel != null)
            {
                UserInterface.Active.RemoveEntity(_textPanel);
                _textPanel = null;
            }

            _selectedIndex = 1;
            _selectedAmount = 0;
            _selectedTotal = 0;

            _items = items;
            _classes = classes;
            _prices = prices;
            _amounts = new List<int>();

            _storeOnAfterConfirmCallback = onInteractionEnd;

            _onPurchaseCallback = null;
            _onPurchaseCallback = onPurchaseCallback;


            float scaleY = HarvestMoon.Instance.Graphics.GraphicsDevice.Viewport.Height / 480.0f;

            // create panel and add to list of panels and manager
            Panel panel = new Panel(new Vector2(620 * scaleY, -1));

            _textPanel = panel;
            _textPanel.SetCustomSkin(_window_11Texture);

            UserInterface.Active.AddEntity(panel);

            // list title
            panel.AddChild(new Header(title));
            panel.AddChild(new HorizontalLine());

            _gParagraph = new Paragraph("Current G: " + HarvestMoon.Instance.Gold.ToString());
            _totalParagraph = new Paragraph("Total: " + "0G");

            panel.AddChild(_gParagraph);
            panel.AddChild(_totalParagraph);

            // create the list
            _upfrontStoreList = new SelectList(new Vector2(0, 280));

            // lock and create title
            _upfrontStoreList.LockedItems[0] = true;
            _upfrontStoreList.AddItem(System.String.Format("{0}{1,-8} {2,-8} {3, -10} {4, -10}", "{{RED}}", "Name", "Class", "Price", "Amount"));


            for(int i = 0; i< items.Count; ++i)
            {
                // add items as formatted table
                _amounts.Add(0);
                _upfrontStoreList.AddItem(System.String.Format("{0,-8} {1,-8} {2,-10} {3, -10}", items[i], classes[i], prices[i].ToString(), _amounts[i].ToString()));   
            }

            if(_upfrontStoreList.Items.Length >= 2)
            {
                _upfrontStoreList.SelectedIndex = _selectedIndex;
            }
            else
            {
                _selectedIndex = 0;
            }

            panel.AddChild(_upfrontStoreList);

            _npcMenu = NPCMenu.UpfrontStore;
        }

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
            _textPanel = new Panel(new Vector2(620 * scaleY, 120 * scaleY), PanelSkin.Default, Anchor.BottomCenter);
            _textPanel.SetCustomSkin(_window_11Texture);


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
            _textPanel = new Panel(new Vector2(620 * scaleY, 120 * scaleY), PanelSkin.Default, Anchor.BottomCenter);

            _textPanel.SetCustomSkin(_window_11Texture);

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

        public void ShowGUI()
        {

            float scaleX = HarvestMoon.Instance.Graphics.GraphicsDevice.Viewport.Width / 640.0f;
            float scaleY = HarvestMoon.Instance.Graphics.GraphicsDevice.Viewport.Height / 480.0f;

            var numberDaySeasonText = HarvestMoon.Instance.DayNumber.ToString() + ", " + HarvestMoon.Instance.DayName;
            var goldText = HarvestMoon.Instance.Gold.ToString() + "G";

            // create a panel and position in bottom center of screen
            _numberDaySeasonParagraph = new Paragraph(numberDaySeasonText, Anchor.TopLeft, null, new Vector2(12 * scaleX, (80 - 16) * scaleY));

            _goldParagraph = new Paragraph(numberDaySeasonText, Anchor.BottomLeft, null, new Vector2(12 * scaleX, (80 - 16) * scaleY));

            UserInterface.Active.AddEntity(_numberDaySeasonParagraph);
            UserInterface.Active.AddEntity(_goldParagraph);

        }

        private void DrawHeart(SpriteBatch spriteBatch, AnimatedSprite staminaSprite, int offset, Vector2 position)
        {

            if (HarvestMoon.Instance.MaxStamina >= 60 + offset)
            {
                if (HarvestMoon.Instance.Stamina == 0 + offset)
                {
                    staminaSprite.Play("heart_empty");
                }
                else if (HarvestMoon.Instance.Stamina > 0 + offset && HarvestMoon.Instance.Stamina <= 15 + offset)
                {
                    staminaSprite.Play("heart_three_quarters");
                }
                else if (HarvestMoon.Instance.Stamina > 15 + offset && HarvestMoon.Instance.Stamina <= 30 + offset)
                {
                    staminaSprite.Play("heart_half");
                }
                else if (HarvestMoon.Instance.Stamina > 30 + offset && HarvestMoon.Instance.Stamina <= 45 + offset)
                {
                    staminaSprite.Play("heart_quarter");
                }
                else if (HarvestMoon.Instance.Stamina > 45 + offset && HarvestMoon.Instance.Stamina <= 60 + offset)
                {
                    staminaSprite.Play("heart_full");
                }
                else if (HarvestMoon.Instance.Stamina > 60 + offset)
                {
                    staminaSprite.Play("heart_full");
                }

                spriteBatch.Draw(staminaSprite, position, 0, new Vector2(2, 2));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend);
            var vHeight = HarvestMoon.Instance.Graphics.GraphicsDevice.Viewport.Height;

            var gRect = _goldSprite.GetBoundingRectangle(new Transform2(new Vector2(0, 0)));
            gRect = _goldSprite.GetBoundingRectangle(
                new Transform2(
                    new Vector2(0 + gRect.Width + gRect.Width * 0.5f, 
                                vHeight- gRect.Height)));
            var gSize = new Vector2(gRect.Width, gRect.Height);
            spriteBatch.Draw(_goldSprite, gRect.Position, 0, new Vector2(2, 2));

            spriteBatch.End();

            spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend);
            var rectangle = _dayToolSprite.GetBoundingRectangle(new Transform2(new Vector2(0, 0)));
            spriteBatch.Draw(_dayToolSprite, new Vector2(rectangle.Width, rectangle.Height), 0, new Vector2(2, 2));
            var currentTool = HarvestMoon.Instance.GetCurrentTool();
            var otherTool = HarvestMoon.Instance.GetOtherTool();

            if (currentTool != default(string))
            {
                var currentToolSprite = _holdingItemSprites[currentTool];

                spriteBatch.Draw(currentToolSprite, new Vector2(28, 32));
            }

            if (otherTool != default(string) && otherTool != currentTool)
            {
                var currentToolSprite = _holdingItemSprites[otherTool];

                spriteBatch.Draw(currentToolSprite, new Vector2(82, 32));
            }

            DrawHeart(spriteBatch, _staminaSprites[0], 0, new Vector2(128, 16));
            DrawHeart(spriteBatch, _staminaSprites[1], 60, new Vector2(128 + 28, 16));
            DrawHeart(spriteBatch, _staminaSprites[2], 120, new Vector2(128, 16 + 26));
            DrawHeart(spriteBatch, _staminaSprites[3], 180, new Vector2(128 + 28, 16 + 26));

            spriteBatch.End();

            var numberDaySeasonText = HarvestMoon.Instance.DayNumber.ToString() + ", " + HarvestMoon.Instance.DayName;

            _numberDaySeasonParagraph.Text = numberDaySeasonText;

            var goldText = HarvestMoon.Instance.Gold.ToString() + "G";
            _goldParagraph.Text = goldText;

            float scaleX = HarvestMoon.Instance.Graphics.GraphicsDevice.Viewport.Width / 640.0f;
            float scaleY = HarvestMoon.Instance.Graphics.GraphicsDevice.Viewport.Height / 480.0f;

            _goldParagraph.Offset = new Vector2((gRect.Width / 2 + 32)*scaleX, (gRect.Height / 2 + 12)*scaleY);

            UserInterface.Active.Draw(spriteBatch);
        }
    }
}
