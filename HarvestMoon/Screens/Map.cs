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

namespace HarvestMoon.Screens
{
    public abstract class Map : GameScreen
    {

        protected EntityManager _entityManager;

        // The tile map
        protected TiledMap _map;
        // The renderer for the map
        protected TiledMapRenderer _mapRenderer;

        protected SpriteBatch _spriteBatch;

        protected OrthographicCamera _camera;

        protected Jack _player;

        Panel _textPanel;
        Paragraph _textParagraph;
        TypeWriterAnimator _textAnimator;

        Action _onAfterConfirmCallback;

        private bool _isGUIButtonDown = false;
        private bool _isActionButtonDown = false;
        private bool _isUpButtonDown = false;
        private bool _isDownButtonDown = false;
        private List<string> _bufferedStrings;

        private bool _busy;

        private bool _npcCoolDown;

        private float _coolingTimer = 0;

        private float _npcCoolingDelay = 0.3f;



        public Map(Game game)
        : base(game)
        {
            _entityManager = new EntityManager();
        }

        public override void Initialize()
        {
            // GeonBit.UI: Init the UI manager using the "hd" built-in theme
            UserInterface.Initialize(Content, BuiltinThemes.hd);

            // GeonBit.UI: tbd create your GUI layouts here..

            // call base initialize func
            base.Initialize();
        }

        public override void LoadContent()
        {

            var viewportAdapter = new BoxingViewportAdapter(Game.Window, GraphicsDevice, 640, 480);
            //var viewportAdapter = new DefaultViewportAdapter(GraphicsDevice);
            _camera = new OrthographicCamera(viewportAdapter);
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        public override void Update(GameTime gameTime)
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

            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyUp(Keys.Q))
            {
                _isGUIButtonDown = false;
            }

            if (keyboardState.IsKeyUp(Keys.V))
            {
                _isActionButtonDown = false;
            }

            if (keyboardState.IsKeyUp(Keys.Up))
            {
                _isUpButtonDown = false;
            }

            if (keyboardState.IsKeyUp(Keys.Down))
            {
                _isDownButtonDown = false;
            }

            if (keyboardState.IsKeyDown(Keys.Q) && !_isGUIButtonDown)
            {
                _isGUIButtonDown = true;
                ShowMessage("");
            }

            if ((keyboardState.IsKeyDown(Keys.Up) && !_isUpButtonDown) || (keyboardState.IsKeyDown(Keys.Down) && !_isDownButtonDown))
            {
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    _isUpButtonDown = true;
                }

                if (keyboardState.IsKeyDown(Keys.Down))
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
                                _textParagraph.Text = "  " + _menuStrings.First() + "\n" + "> " + _menuStrings.Last();                           }
                            else
                            {
                                _textParagraph.Text = "> " + _menuStrings.First() + "\n" + "  " + _menuStrings.Last();
                            }

                            break;
                    }
                }
            }


            if (keyboardState.IsKeyDown(Keys.V) && !_isActionButtonDown)
            {
                _isActionButtonDown = true;
                if (_textPanel != null)
                {
                    if (_isDisplayingMenu )
                    {
                        switch (_npcMenu)
                        {
                            case NPCMenu.YesNo:
                                if (_textParagraph.Text == "> " + _menuStrings.First() + "\n" + "  " + _menuStrings.Last())
                                {
                                    UserInterface.Active.RemoveEntity(_textPanel);
                                    _textPanel = null;
                                    _player.UnFreeze();
                                    _player.Busy();
                                    _npcCoolDown = true;
                                    _busy = true;
                                    _isDisplayingMenu = false;
                                    _menuCallbacks[0]();
                                }
                                else
                                {
                                    UserInterface.Active.RemoveEntity(_textPanel);
                                    _textPanel = null;
                                    _player.UnFreeze();
                                    _player.Busy();
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
                                    _player.UnFreeze();

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

        public void ShowYesNoMessage(string yesString, string noString, Action yesCallback, Action noCallback)
        {
            _isDisplayingMenu = true;

            _player.Freeze();

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

            // create a panel and position in center of screen
            _textPanel = new Panel(new Vector2(620, 150), PanelSkin.Default, Anchor.BottomCenter);

            UserInterface.Active.AddEntity(_textPanel);

            _textParagraph = new Paragraph("");

            _textParagraph.Text = "> " + _menuStrings.First() + "\n" +  "  " + _menuStrings.Last();

            _textPanel.AddChild(_textParagraph);
            
            _textPanel.Opacity = 200;

            _npcMenu = NPCMenu.YesNo;
        }

        public void ShowMessage(string message, Action onAfterConfirmCallback = null)
        {
            if (_textPanel != null)
            {
                UserInterface.Active.RemoveEntity(_textPanel);
                _textPanel = null;
            }

            if(onAfterConfirmCallback != null)
            {
                _onAfterConfirmCallback = onAfterConfirmCallback;
            }

            _player.Freeze();

            _bufferedStrings = SplitByLength(message, 130);

            // create a panel and position in center of screen
            _textPanel = new Panel(new Vector2(620, 150), PanelSkin.Default, Anchor.BottomCenter);

            UserInterface.Active.AddEntity(_textPanel);

            var paragraph = new Paragraph("");

            _textAnimator = new TypeWriterAnimator();

            _textAnimator.TextToType = _bufferedStrings.First();
            _bufferedStrings.Remove(_bufferedStrings.First());

            paragraph.AttachAnimator(_textAnimator);

            _textPanel.AddChild(paragraph);

            _textPanel.Opacity = 200;

        }

        protected void CheckCollisions()
        {
            var interactables = _entityManager.Entities.Where(e => e is Interactable).Cast<Interactable>().ToArray();

            bool foundInteractable = false;
            Interactable closestInteractable = null;

            foreach (var interactable in interactables)
            {
                if (interactable.BoundingRectangle.Intersects(_player.BoundingRectangle) && interactable.Planked)
                {
                    var intersection = interactable.BoundingRectangle.Intersection(_player.BoundingRectangle);
                    if (intersection.Height > intersection.Width)
                    {
                        if (_player.Position.X > interactable.X)
                        {
                            _player.Position = new Vector2(_player.Position.X + intersection.Width, _player.Position.Y);
                        }
                        else
                        {
                            _player.Position = new Vector2(_player.Position.X - intersection.Width, _player.Position.Y);
                        }

                    }
                    else
                    {
                        if (_player.Position.Y > interactable.Y)
                        {
                            _player.Position = new Vector2(_player.Position.X, _player.Position.Y + intersection.Height);
                        }
                        else
                        {
                            _player.Position = new Vector2(_player.Position.X, _player.Position.Y - intersection.Height);
                        }

                    }
                }

                if (interactable.BoundingRectangle.Intersects(_player.ActionBoundingRectangle))
                {
                    foundInteractable = true;

                    if(closestInteractable == null)
                    {
                        closestInteractable = interactable;
                    }
                    else
                    {
                        Vector2 closestInteractablePosition = new Vector2(closestInteractable.X, closestInteractable.Y);
                        Vector2 interactablePosition = new Vector2(interactable.X, interactable.Y);
                        if (Vector2.Distance(_player.Position, interactablePosition) < Vector2.Distance(_player.Position, closestInteractablePosition))
                        {
                            closestInteractable = interactable;
                        }
                    }

                    
                }

            }

            if (!foundInteractable)
            {
                _player.Interact(null);
            }
            else
            {
                _player.Interact(closestInteractable);
            }

            var walls = _entityManager.Entities.Where(e => e is Wall).Cast<Wall>().ToArray();

            foreach (var wall in walls)
            {
                if (wall.BoundingRectangle.Intersects(_player.BoundingRectangle))
                {
                    var intersection = wall.BoundingRectangle.Intersection(_player.BoundingRectangle);
                    if (intersection.Height > intersection.Width)
                    {
                        if (_player.Position.X > wall.Position.X)
                        {
                            _player.Position = new Vector2(_player.Position.X + intersection.Width, _player.Position.Y);
                        }
                        else
                        {
                            _player.Position = new Vector2(_player.Position.X - intersection.Width, _player.Position.Y);
                        }

                    }
                    else
                    {
                        if (_player.Position.Y > wall.Position.Y)
                        {
                            _player.Position = new Vector2(_player.Position.X, _player.Position.Y + intersection.Height);
                        }
                        else
                        {
                            _player.Position = new Vector2(_player.Position.X, _player.Position.Y - intersection.Height);
                        }

                    }
                }
            }

            var doors = _entityManager.Entities.Where(e => e is Door).Cast<Door>().ToArray();

            foreach (var door in doors)
            {
                if (door.BoundingRectangle.Intersects(_player.BoundingRectangle))
                {
                    door.Trigger();
                }

            }

        }


        public override void Draw(GameTime gameTime)
        {
            UserInterface.Active.Draw(_spriteBatch);
        }
    }
}
