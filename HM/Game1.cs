using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.ViewportAdapters;

namespace HM
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        // The tile map
        private TiledMap map;
        // The renderer for the map
        private TiledMapRenderer mapRenderer;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        private OrthographicCamera _camera;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            // Load the compiled map
            map = Content.Load<TiledMap>("maps/ranch/level");
            // Create the map renderer
            mapRenderer = new TiledMapRenderer(GraphicsDevice);


            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new OrthographicCamera(viewportAdapter);

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            // Update the map
            // map Should be the `TiledMap`
            mapRenderer.Update(gameTime);

            var keyboardState = Keyboard.GetState();
            const float movementSpeed = 200;

            if (keyboardState.IsKeyDown(Keys.Up))
                _camera.Move(new Vector2(0, -movementSpeed));


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Transform matrix is only needed if you have a Camera2D
            // Setting the sampler state to `SamplerState.PointClamp` is reccomended to remove gaps between the tiles when rendering
            spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);


            for(int i = 0; i<map.Layers.Count; ++i)
            {
                // map Should be the `TiledMap`
                // Once again, the transform matrix is only needed if you have a Camera2D
                mapRenderer.Draw(map.Layers[i], _camera.GetViewMatrix());

            }

            // End the sprite batch
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
