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
using HarvestMoon.Input;

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
        protected ViewportAdapter _viewportAdapter;

        protected bool _isActionButtonDown = false;
        protected bool _isRunButtonDown = false;
        protected bool _isInteractionButtonDown = false;
        protected bool _isUpButtonDown = false;
        protected bool _isDownButtonDown = false;

        public GUI(Game game)
        : base(game)
        {
        }

        public override void Initialize()
        {
            // GeonBit.UI: Init the UI manager using the "hd" built-in theme
            if(UserInterface.Active == null)
            {
                UserInterface.Initialize(Content, BuiltinThemes.hd);
            }
            

            // GeonBit.UI: tbd create your GUI layouts here..

            // call base initialize func
            base.Initialize();

            UserInterface.Active.UseRenderTarget = false;
            UserInterface.Active.ShowCursor = false;


            /*var HM = Game as HarvestMoon;

            
            HM.Graphics.PreferredBackBufferWidth = 640;
            HM.Graphics.PreferredBackBufferHeight = 480;
            HM.Graphics.IsFullScreen = false; 
            HM.Graphics.ApplyChanges();*/

        }

        public override void LoadContent()
        {

            _viewportAdapter = new BoxingViewportAdapter(Game.Window, GraphicsDevice, 640, 480);
            //var viewportAdapter = new DefaultViewportAdapter(GraphicsDevice);
            _camera = new OrthographicCamera(_viewportAdapter);
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            // GeonBit.UIL update UI manager
            UserInterface.Active.Update(gameTime);

            var keyboardState = HarvestMoon.Instance.Input;

            if (keyboardState.IsKeyUp(InputDevice.Keys.A))
            {
                _isActionButtonDown = false;
            }

            if (keyboardState.IsKeyUp(InputDevice.Keys.X))
            {
                _isRunButtonDown = false;
            }

            if (keyboardState.IsKeyUp(InputDevice.Keys.B))
            {
                _isInteractionButtonDown = false;
            }


            if (keyboardState.IsKeyUp(InputDevice.Keys.Up))
            {
                _isUpButtonDown = false;
            }

            if (keyboardState.IsKeyUp(InputDevice.Keys.Down))
            {
                _isDownButtonDown = false;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _viewportAdapter.Reset();
            /*
            UserInterface.Active.Draw(_spriteBatch);

            _viewportAdapter.Reset();

            OnPreGuiDraw(gameTime);

            var cameraMatrix = _camera.GetViewMatrix();

            UserInterface.Active.RenderTargetTransformMatrix = cameraMatrix;
            UserInterface.Active.DrawMainRenderTarget(_spriteBatch);

            */
            //float scaleY = HarvestMoon.Instance.Graphics.GraphicsDevice.Viewport.Height / 480.0f;
            //UserInterface.Active.GlobalScale = scaleY;

            UserInterface.Active.Draw(_spriteBatch);

        }
    }
}
