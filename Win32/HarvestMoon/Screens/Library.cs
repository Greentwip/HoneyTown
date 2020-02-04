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
using HarvestMoon.Entities.Town;

namespace HarvestMoon.Screens
{
    public class Library : Map
    {
        public Library(Game game)
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
            _map = Content.Load<TiledMap>("maps/Library");
            // Create the map renderer
            _mapRenderer = new TiledMapRenderer(GraphicsDevice, _map);

            foreach (var layer in _map.ObjectLayers)
            {
                if (layer.Name == "Arrivals")
                {
                    foreach (var obj in layer.Objects)
                    {
                        if (obj.Name == "from-city")
                        {
                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                            objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;


                            _player = HarvestMoon.Instance.RanchState.Entities.FirstOrDefault(e => e is Jack) as Jack;

                            if (_player == null)
                            {
                                _player = _entityManager.AddEntity(new Jack(Content, _entityManager, this, objectPosition));
                            }
                            else
                            {
                                _entityManager.AddEntity(_player);
                                _player.EntityManager = _entityManager;

                            }

                            _player.Position = new Vector2(objectPosition.X, objectPosition.Y);

                            _player.PlayerFacing = Jack.Facing.UP;

                        }
                    }
                }
                else if (layer.Name == "Walls")
                {
                    foreach (var obj in layer.Objects)
                    {
                        var objectPosition = obj.Position;

                        objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                        objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                        var objectSize = obj.Size;

                        _entityManager.AddEntity(new Wall(objectPosition, objectSize));
                    }
                }
                else if (layer.Name == "Doors")
                {
                    foreach (var obj in layer.Objects)
                    {
                        var objectPosition = obj.Position;

                        objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                        objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                        var objectSize = obj.Size;

                        var door = new Door(objectPosition, objectSize);
                        _entityManager.AddEntity(door);

                        door.OnTriggerStart(() =>
                        {
                            _player.Freeze();
                        });

                        if (obj.Name == "city")
                        {
                            door.OnTriggerEnd(() =>
                            {
                                if (!door.Triggered)
                                {
                                    door.Triggered = true;
                                    var screen = new Town(Game, HarvestMoon.Arrival.Library);
                                    var transition = new FadeTransition(GraphicsDevice, Color.Black, 1.0f);
                                    ScreenManager.LoadScreen(screen, transition);
                                }
                            });
                        }
                    }
                }
                else if (layer.Name == "Interactables")
                {
                    foreach (var obj in layer.Objects)
                    {
                        var objectPosition = obj.Position;

                        objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                        objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                        var objectSize = obj.Size;

                        if(obj.Type == "npc")
                        {
                            string objectMessage = "";

                            foreach (var property in obj.Properties)
                            {
                                if (property.Key.Contains("message"))
                                {
                                    objectMessage = HarvestMoon.Instance.Strings.Get(property.Value);
                                }
                            }

                            if (obj.Name == "anna")
                            {
                                _entityManager.AddEntity(new Anna(Content, objectPosition, objectSize));
                            }
                            
                            if(obj.Name == "bee-anatomy")
                            {
                                _entityManager.AddEntity(new BeeAnatomyBookshelf(objectPosition,
                                                                                objectSize,
                                                                                "Useful info on bees' anatomy.",
                                                                                 "Bee anatomy"));
                            }

                            if(obj.Name == "bee-social")
                            {
                                _entityManager.AddEntity(new BeeSocialBookshelf(objectPosition,
                                                                                objectSize,
                                                                                "Useful info on bees' social structure.",
                                                                                 "Bee social"));
                            }

                        }

                    }

                }

            }

            LoadPlayer();
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
                _camera.LookAt(new Vector2(_map.Width * _map.TileWidth / 2, _map.Height * _map.TileHeight / 2));
            }

            CheckCollisions();


        }

        // Must be Draw(GameTime gametime)
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            var cameraMatrix = _camera.GetViewMatrix();
            cameraMatrix.Translation = new Vector3(cameraMatrix.Translation.X, cameraMatrix.Translation.Y, cameraMatrix.Translation.Z);

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

            _spriteBatch.Begin(transformMatrix: cameraMatrix, samplerState: SamplerState.PointClamp);

            foreach (var foregroundLayer in _map.Layers.Where(l => l is TiledMapGroupLayer))
            {
                var foregroundLayers = (foregroundLayer as TiledMapGroupLayer).Layers.Where(l => l.Name.Contains("-Foreground"));

                foreach (var layer in foregroundLayers)
                {
                    _mapRenderer.Draw(layer, cameraMatrix);

                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
