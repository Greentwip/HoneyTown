using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using HarvestMoon.Entities;
using System.Linq;
using MonoGame.Extended.Screens.Transitions;
using HarvestMoon.Entities.General;
using HarvestMoon.Entities.Town;
using System.Collections.Generic;
using HarvestMoon.Entities.Ranch;
using Microsoft.Xna.Framework.Media;

namespace HarvestMoon.Screens
{
    public class Town : Map
    {
        private HarvestMoon.Arrival _arrival;

        private bool _mediaShouldFadeToRanch;

        public Town(Game game, HarvestMoon.Arrival arrival)
            : base(game)
        {
            _arrival = arrival;
        }

        public override void Initialize()
        {
            // call base initialize func
            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            if (MediaPlayer.State == MediaState.Stopped)
            {
                var song = Content.Load<Song>("audio/music/town");

                MediaPlayer.Play(song);
                MediaPlayer.IsRepeating = true;
            }


            // Load the compiled map
            _map = Content.Load<TiledMap>("maps/City");
            // Create the map renderer
            _mapRenderer = new TiledMapRenderer(GraphicsDevice, _map);

            foreach (var layer in _map.ObjectLayers)
            {
                if (layer.Name == "Arrivals")
                {
                    foreach (var obj in layer.Objects)
                    {
                        if (obj.Name == "from-ranch" && _arrival == HarvestMoon.Arrival.Ranch)
                        {
                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                            objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                            _player = HarvestMoon.Instance.RanchState.Entities.FirstOrDefault(e => e is Jack) as Jack;

                            _entityManager.AddEntity(_player);
                            _player.EntityManager = _entityManager;

                            _player.Position = new Vector2(objectPosition.X, objectPosition.Y);

                            _player.PlayerFacing = Jack.Facing.UP;
                        }
                        else if (obj.Name == "from-library" && _arrival == HarvestMoon.Arrival.Library)
                        {
                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                            objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                            _player = HarvestMoon.Instance.RanchState.Entities.FirstOrDefault(e => e is Jack) as Jack;
                            _entityManager.AddEntity(_player);
                            _player.EntityManager = _entityManager;

                            _player.Position = new Vector2(objectPosition.X, objectPosition.Y);

                            _player.PlayerFacing = Jack.Facing.DOWN;
                        }
                        else if (obj.Name == "from-librarian" && _arrival == HarvestMoon.Arrival.Librarian)
                        {
                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                            objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                            _player = HarvestMoon.Instance.RanchState.Entities.FirstOrDefault(e => e is Jack) as Jack;

                            _entityManager.AddEntity(_player);
                            _player.EntityManager = _entityManager;

                            _player.Position = new Vector2(objectPosition.X, objectPosition.Y);

                            _player.PlayerFacing = Jack.Facing.DOWN;
                        }
                        else if (obj.Name == "from-dealer" && _arrival == HarvestMoon.Arrival.Dealer)
                        {
                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                            objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                            _player = HarvestMoon.Instance.RanchState.Entities.FirstOrDefault(e => e is Jack) as Jack;
                            _entityManager.AddEntity(_player);
                            _player.EntityManager = _entityManager;

                            _player.Position = new Vector2(objectPosition.X, objectPosition.Y);

                            _player.PlayerFacing = Jack.Facing.DOWN;
                        }
                        else if (obj.Name == "from-florist" && _arrival == HarvestMoon.Arrival.Florist)
                        {
                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                            objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                            _player = HarvestMoon.Instance.RanchState.Entities.FirstOrDefault(e => e is Jack) as Jack;

                            _entityManager.AddEntity(_player);
                            _player.EntityManager = _entityManager;

                            _player.Position = new Vector2(objectPosition.X, objectPosition.Y);

                            _player.PlayerFacing = Jack.Facing.DOWN;
                        }
                    }
                }
                else if (layer.Name == "Interactables")
                {
                    foreach (var obj in layer.Objects)
                    {
                        if (obj.Type == "npc")
                        {
                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                            objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                            var objectSize = obj.Size;

                            string objectMessage = "";

                            foreach (var property in obj.Properties)
                            {
                                if (property.Key.Contains("message"))
                                {
                                    objectMessage = HarvestMoon.Instance.Strings.Get(property.Value);
                                }
                            }

                            if(obj.Name == "edd")
                            {
                                _entityManager.AddEntity(new Edd(Content, objectPosition, objectSize));
                            }
                            else if (obj.Name == "ted")
                            {
                                _entityManager.AddEntity(new Ted(Content, objectPosition, objectSize));
                            }
                            else if (obj.Name == "monica")
                            {
                                _entityManager.AddEntity(new Monica(Content, objectPosition, objectSize));
                            }
                            else
                            {
                                _entityManager.AddEntity(new BasicMessage(objectPosition, objectSize, objectMessage));
                            }
                        }
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

                        if (obj.Name == "ranch")
                        {
                            var door = new Door(objectPosition, objectSize);
                            _entityManager.AddEntity(door);

                            door.OnTriggerStart(() =>
                            {
                                _player.Freeze();
                            });


                            door.OnTriggerEnd(() =>
                            {
                                if (!door.Triggered)
                                {
                                    door.Triggered = true;
                                    this._mediaShouldFadeToRanch = true;
                                }
                            });
                        }
                        else if (obj.Name == "library")
                        {
                            var door = new RanchDoor(Content, objectPosition, objectSize);
                            _entityManager.AddEntity(door);

                            door.OnTriggerStart(() =>
                            {
                                _player.Freeze();
                            });

                            door.OnTriggerEnd(() =>
                            {
                                if (!door.Triggered)
                                {
                                    door.Triggered = true;
                                    var screen = new Library(Game);
                                    var transition = new FadeTransition(GraphicsDevice, Color.Black, 1.0f);
                                    ScreenManager.LoadScreen(screen, transition);
                                }
                            });
                        }
                        else if (obj.Name == "librarian")
                        {
                            var door = new RanchDoor(Content, objectPosition, objectSize);
                            _entityManager.AddEntity(door);

                            door.OnTriggerStart(() =>
                            {
                                _player.Freeze();
                            });

                            door.OnTriggerEnd(() =>
                            {
                                if (!door.Triggered)
                                {
                                    door.Triggered = true;
                                    var screen = new Librarian(Game);
                                    var transition = new FadeTransition(GraphicsDevice, Color.Black, 1.0f);
                                    ScreenManager.LoadScreen(screen, transition);
                                }
                            });
                        }
                        else if (obj.Name == "dealer")
                        {
                            var door = new RanchDoor(Content, objectPosition, objectSize);
                            _entityManager.AddEntity(door);

                            door.OnTriggerStart(() =>
                            {
                                _player.Freeze();
                            });

                            door.OnTriggerEnd(() =>
                            {
                                if (!door.Triggered)
                                {
                                    door.Triggered = true;
                                    var screen = new Dealer(Game);
                                    var transition = new FadeTransition(GraphicsDevice, Color.Black, 1.0f);
                                    ScreenManager.LoadScreen(screen, transition);
                                }
                            });
                        }
                        else if (obj.Name == "florist")
                        {
                            var door = new RanchDoor(Content, objectPosition, objectSize);
                            _entityManager.AddEntity(door);

                            door.OnTriggerStart(() =>
                            {
                                _player.Freeze();
                            });

                            door.OnTriggerEnd(() =>
                            {
                                if (!door.Triggered)
                                {
                                    door.Triggered = true;
                                    var screen = new Florist(Game);
                                    var transition = new FadeTransition(GraphicsDevice, Color.Black, 1.0f);
                                    ScreenManager.LoadScreen(screen, transition);
                                }
                            });
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
            }

            LoadPlayer();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_mediaShouldFadeToRanch)
            {
                if (MediaPlayer.Volume > 0.0f && MediaPlayer.State == MediaState.Playing)
                {
                    MediaPlayer.Volume -= 0.1f;
                }

                if (MediaPlayer.Volume <= 0.0f)
                {
                    MediaPlayer.Stop();
                    MediaPlayer.Volume = 1.0f;

                    var screen = new Ranch(Game, HarvestMoon.Arrival.Town);
                    var transition = new FadeTransition(GraphicsDevice, Color.Black, 1.0f);
                    ScreenManager.LoadScreen(screen, transition);
                }

            }


            // Update the map
            // map Should be the `TiledMap`
            _mapRenderer.Update(gameTime);
            _entityManager.Update(gameTime);


            if (_player != null && !_player.IsDestroyed)
            {
                _camera.LookAt(_player.Position);
                var constraints = new Vector2();

                if (_camera.BoundingRectangle.Center.X < 320)
                {
                    constraints.X = 320;
                }

                if (_camera.BoundingRectangle.Center.X > _map.Width * _map.TileWidth - 320)
                {
                    constraints.X = _map.Width * _map.TileWidth - 320;
                }

                if (_camera.BoundingRectangle.Center.Y < 240)
                {
                    constraints.Y = 240;
                }

                if (_camera.BoundingRectangle.Center.Y > _map.Height * _map.TileHeight - 240)
                {
                    constraints.Y = _map.Height * _map.TileHeight - 240;
                }

                if (constraints.X != 0)
                {
                    _camera.LookAt(new Vector2(constraints.X, _player.Position.Y));
                }

                if (constraints.Y != 0)
                {
                    _camera.LookAt(new Vector2(_player.Position.X, constraints.Y));
                }

                if (constraints.X != 0 && constraints.Y != 0)
                {
                    _camera.LookAt(new Vector2(constraints.X, constraints.Y));
                }

            }


            CheckCollisions();
        }

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
                _mapRenderer.Draw(_map.Layers[i], cameraMatrix, effect: HarvestMoon.Instance.DayTimeEffect);

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
                    _mapRenderer.Draw(layer, cameraMatrix, effect: HarvestMoon.Instance.DayTimeEffect);

                }
            }


            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }

}
