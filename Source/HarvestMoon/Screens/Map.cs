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
        protected ViewportAdapter _viewportAdapter;

        protected Jack _player;

        private bool _isActionButtonDown = false;
        private bool _isUpButtonDown = false;
        private bool _isDownButtonDown = false;

        public Map(Game game)
        : base(game)
        {
            _entityManager = new EntityManager();
        }

        public void LoadPlayer()
        {
            _player.UnFreeze();

            if (_player.IsCarrying)
            {
                _entityManager.AddEntity(_player.CarryingObject);
            }
        }

        public override void Initialize()
        {
            // GeonBit.UI: Init the UI manager using the "hd" built-in theme

            if(UserInterface.Active == null)
            {
                UserInterface.Initialize(Content, BuiltinThemes.hd);
            }
            else
            {
                UserInterface.Active.Root.ClearChildren();
            }

            HarvestMoon.Instance.GUI.ShowGUI();

            // GeonBit.UI: tbd create your GUI layouts here..

            // call base initialize func
            base.Initialize();
        }

        public override void LoadContent()
        {

            _viewportAdapter = new BoxingViewportAdapter(Game.Window, GraphicsDevice, 640, 480);
            //_viewportAdapter = new DefaultViewportAdapter(GraphicsDevice);
            _camera = new OrthographicCamera(_viewportAdapter);
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        public override void Update(GameTime gameTime)
        {
            // GeonBit.UIL update UI manager
            UserInterface.Active.Update(gameTime);
            var deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

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
            }
        }

        protected void CheckCollisions()
        {
            var interactables = _entityManager.Entities.Where(e => { return e is Interactable && !(e is Soil); }).Cast<Interactable>().ToArray();
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
            }

            interactables = _entityManager.Entities.Where(e => { return e is Interactable; }).Cast<Interactable>().ToArray();

            bool foundInteractable = false;
            Interactable closestInteractable = null;

            foreach (var interactable in interactables)
            {
                

                if (interactable.BoundingRectangle.Intersects(_player.ActionBoundingRectangle))
                {
                    foundInteractable = true;

                    if(closestInteractable == null)
                    {
                        if (interactable.Planked)
                        {
                            closestInteractable = interactable;
                        }
                            
                    }
                    else
                    {
                        Vector2 closestInteractablePosition = new Vector2(closestInteractable.X, closestInteractable.Y);
                        Vector2 interactablePosition = new Vector2(interactable.X, interactable.Y);
                        if (Vector2.Distance(_player.Position, interactablePosition) < Vector2.Distance(_player.Position, closestInteractablePosition))
                        {
                            if (interactable.Planked)
                            {
                                closestInteractable = interactable;
                            }
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

            var soilSegments = _entityManager.Entities.Where(e => e is Soil).Cast<Soil>().ToArray();

            foreach (var soilSegment in soilSegments)
            {
                if (soilSegment.BoundingRectangle.Intersects(_player.BoundingRectangle) && soilSegment.HasGrown)
                {
                    var intersection = soilSegment.BoundingRectangle.Intersection(_player.BoundingRectangle);
                    if (intersection.Height > intersection.Width)
                    {
                        if (_player.Position.X > soilSegment.X)
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
                        if (_player.Position.Y > soilSegment.Y)
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
        }

        public override void Draw(GameTime gameTime)
        {
            _viewportAdapter.Reset();

            /*UserInterface.Active.Draw(_spriteBatch);

            OnPreGuiDraw(gameTime);

            var cameraMatrix = _camera.GetViewMatrix();

            UserInterface.Active.RenderTargetTransformMatrix = cameraMatrix;
            UserInterface.Active.DrawMainRenderTarget(_spriteBatch);*/

            //float scaleY = HarvestMoon.Instance.Graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height / 480.0f;
            //UserInterface.Active.GlobalScale = scaleY;

            HarvestMoon.Instance.GUI.Draw(_spriteBatch);
        }
    }
}
