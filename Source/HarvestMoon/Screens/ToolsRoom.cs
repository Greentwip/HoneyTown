using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

using HarvestMoon.Entities;
using System.Linq;
using MonoGame.Extended.Screens.Transitions;
using HarvestMoon.Entities.Tools;

namespace HarvestMoon.Screens
{
    public class ToolsRoom : Map
    {
        public ToolsRoom(Game game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            // call base initialize func
            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            // Load the compiled map
            _map = Content.Load<TiledMap>("maps/tools-room/tools-room");
            // Create the map renderer
            _mapRenderer = new TiledMapRenderer(GraphicsDevice, _map);

            foreach (var layer in _map.ObjectLayers)
            {
                if (layer.Name == "objects")
                {
                    foreach (var obj in layer.Objects)
                    {
                        if (obj.Type == "player_start")
                        {
                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                            objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                            _player = _entityManager.AddEntity(new Jack(Content, _entityManager, this, objectPosition));
                            _player.OnPack((toolName) =>
                            {
                                PackTool(toolName);
                            });

                            _player.PlayerFacing = Jack.Facing.UP;

                        }

                        if (obj.Type == "door")
                        {
                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                            objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                            var objectSize = obj.Size;

                            var door = new Door(objectPosition, objectSize);
                            _entityManager.AddEntity(door);

                            if (obj.Name == "ranch")
                            {
                                door.OnTrigger(() =>
                                {
                                    if (!door.Triggered)
                                    {
                                        door.Triggered = true;
                                        var screen = new Ranch(Game, HarvestMoon.Arrival.Tools);
                                        var transition = new FadeTransition(GraphicsDevice, Color.Black, 1.0f);
                                        ScreenManager.LoadScreen(screen, transition);
                                    }
                                });
                            }
                        }

                        if (obj.Type == "sickle")
                        {

                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                            objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                            _entityManager.AddEntity(new Sickle(Content, objectPosition));
                        }

                        if (obj.Type == "hoe")
                        {

                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                            objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                            _entityManager.AddEntity(new Hoe(Content, objectPosition));
                        }

                        if (obj.Type == "hammer")
                        {

                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                            objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                            _entityManager.AddEntity(new Hammer(Content, objectPosition));
                        }

                        if (obj.Type == "axe")
                        {

                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                            objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                            _entityManager.AddEntity(new Axe(Content, objectPosition));
                        }

                        if (obj.Type == "watering-can")
                        {

                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                            objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                            _entityManager.AddEntity(new WateringCan(Content, objectPosition));
                        }

                        if (obj.Type == "grass-seeds")
                        {

                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                            objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                            _entityManager.AddEntity(new GrassSeeds(Content, objectPosition)); //Not working yet
                        }

                        if (obj.Type == "turnip-seeds")
                        {

                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                            objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                            _entityManager.AddEntity(new TurnipSeeds(Content, objectPosition));
                        }


                        if (obj.Type == "potato-seeds")
                        {

                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                            objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                            _entityManager.AddEntity(new PotatoSeeds(Content, objectPosition));
                        }

                        if (obj.Type == "corn-seeds")
                        {

                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                            objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                            _entityManager.AddEntity(new CornSeeds(Content, objectPosition));
                        }

                        if (obj.Type == "tomato-seeds")
                        {

                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                            objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                            _entityManager.AddEntity(new TomatoSeeds(Content, objectPosition));
                        }



                    }
                }
                else if (layer.Name == "walls")
                {
                    foreach (var obj in layer.Objects)
                    {
                        if (obj.Type == "wall")
                        {

                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                            objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                            var objectSize = obj.Size;

                            _entityManager.AddEntity(new Wall(objectPosition, objectSize));
                        }
                    }
                }
            }

            var tools = _entityManager.Entities.Where(e => e is Tool).Cast<Tool>().ToArray();

            var hm = Game as HarvestMoon;

            foreach (var tool in tools)
            {
                if (hm.IsToolPacked(tool.Name))
                {
                    tool.Hide();
                }
            }

        }

        public void PackTool(string toolName)
        {
            var hm = Game as HarvestMoon;

            var lastToolName = hm.PackTool(toolName);
            var currentToolName = hm.GetCurrentTool();

            if(lastToolName != "none")
            {
                var tools = _entityManager.Entities.Where(e => e is Tool).Cast<Tool>().ToArray();

                foreach (var tool in tools)
                {
                    if (tool.Name == lastToolName)
                    {
                        tool.Reset();
                    }

                    if(tool.Name == currentToolName)
                    {
                        tool.Reset();
                        tool.Hide();
                    }
                }

            }
            else
            {
                var tools = _entityManager.Entities.Where(e => e is Tool).Cast<Tool>().ToArray();

                foreach (var tool in tools)
                {
                    if(tool.Name == toolName)
                    {
                        tool.Reset();
                        tool.Hide();
                    }
                }
            }

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // TODO: Add your update logic here
            // Update the map
            // map Should be the `TiledMap`
            _mapRenderer.Update(gameTime);
            _entityManager.Update(gameTime);


            if (_player != null && !_player.IsDestroyed)
            {
                _camera.LookAt(new Vector2(_map.Width * 4 / 2, _map.Height * 4 / 2));
            }

            CheckCollisions();


        }

        // Must be Draw(GameTime gametime)
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            float scaleY = HarvestMoon.Instance.Graphics.GraphicsDevice.Viewport.Height / 480.0f;

            var cameraMatrix = _camera.GetViewMatrix();
            cameraMatrix.Translation = new Vector3(cameraMatrix.Translation.X, cameraMatrix.Translation.Y - 32 * scaleY, cameraMatrix.Translation.Z);

            _spriteBatch.Begin(transformMatrix: cameraMatrix, samplerState: SamplerState.PointClamp);


            for (int i = 0; i < _map.Layers.Count; ++i)
            {
                // map Should be the `TiledMap`
                // Once again, the transform matrix is only needed if you have a Camera2D
                _mapRenderer.Draw(_map.Layers[i], cameraMatrix);

            }
            // End the sprite batch
            _spriteBatch.End();

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix());
            _entityManager.Draw(_spriteBatch);
            _spriteBatch.End();

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix());

            var interactables = _entityManager.Entities.Where(e => e is Interactable).Cast<Interactable>().ToArray();

            foreach (var interactable in interactables)
            {
                _spriteBatch.DrawRectangle(interactable.BoundingRectangle, Color.Fuchsia);
            }
            _spriteBatch.End();


            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix());
            _spriteBatch.DrawRectangle(_player.BoundingRectangle, Color.Fuchsia);
            _spriteBatch.DrawRectangle(_player.ActionBoundingRectangle, Color.Fuchsia);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
