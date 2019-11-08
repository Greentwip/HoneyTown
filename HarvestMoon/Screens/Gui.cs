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
    public abstract class GUI : GameScreen
    {

        // The tile map
        protected TiledMap _map;
        // The renderer for the map
        protected TiledMapRenderer _mapRenderer;

        protected SpriteBatch _spriteBatch;

        protected OrthographicCamera _camera;

        protected bool _isActionButtonDown = false;
        protected bool _isUpButtonDown = false;
        protected bool _isDownButtonDown = false;

        private bool _busy;

        private bool _selectionCoolDown;

        private float _coolingTimer = 0;

        private float _selectionCoolingDelay = 0.3f;

        public GUI(Game game)
        : base(game)
        {
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

            if (_selectionCoolDown)
            {
                _coolingTimer += deltaSeconds;

                if (_coolingTimer >= _selectionCoolingDelay)
                {
                    _coolingTimer = 0.0f;
                    _selectionCoolDown = false;
                    _busy = false;
                }
            }
            var keyboardState = Keyboard.GetState();

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
        }

        public override void Draw(GameTime gameTime)
        {
            UserInterface.Active.Draw(_spriteBatch);
        }
    }
}
