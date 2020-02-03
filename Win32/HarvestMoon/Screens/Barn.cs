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
using HarvestMoon.Entities.Ranch;
using System.Collections.Generic;

namespace HarvestMoon.Screens
{
    public class Barn : Map
    {
        private HarvestMoon.Arrival _arrival;


        private List<CowPlacement> _cowPlacements;

        struct CowPlacement
        {
            public Vector2 Position;
            public Size2 Size;
        }

        public Barn(Game game, HarvestMoon.Arrival arrival)
            : base(game)
        {
            _arrival = arrival;

            _cowPlacements = new List<CowPlacement>();

            for (int i = 0; i<8; ++i)
            {
                _cowPlacements.Add(new CowPlacement());
            }
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
            _map = Content.Load<TiledMap>("maps/Barn");
            // Create the map renderer
            _mapRenderer = new TiledMapRenderer(GraphicsDevice, _map);

            foreach (var layer in _map.ObjectLayers)
            {
                if (layer.Name == "Arrivals")
                {
                    foreach (var obj in layer.Objects)
                    {
                        if (obj.Name == "barn" && _arrival == HarvestMoon.Arrival.Ranch)
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
                            }

                            _player.Position = new Vector2(objectPosition.X, objectPosition.Y);

                            _player.PlayerFacing = Jack.Facing.UP;
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

                        var door = new Door(objectPosition, objectSize);
                        _entityManager.AddEntity(door);

                        if (obj.Name == "ranch")
                        {

                            door.OnTriggerStart(() =>
                            {
                                _player.Freeze();
                            });

                            door.OnTriggerEnd(() =>
                            {
                                if (!door.Triggered)
                                {
                                    door.Triggered = true;
                                    var screen = new Ranch(Game, HarvestMoon.Arrival.Barn);
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
                else if (layer.Name == "Special")
                {
                    foreach (var obj in layer.Objects)
                    {
                        var objectPosition = obj.Position;

                        objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                        objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                        var objectSize = obj.Size;

                        var placementName = obj.Name;
                        int lastIndexOf = placementName.LastIndexOf("-");
                        string specialName = placementName.Substring(0, lastIndexOf);
                        string specialPosition = placementName.Substring(lastIndexOf + 1, placementName.Length - specialName.Length - 1);

                        var cowPlacement = new CowPlacement();
                        cowPlacement.Position = objectPosition;
                        cowPlacement.Size = objectSize;

                        _cowPlacements[int.Parse(specialPosition)] = cowPlacement;
                    }

                }
            }

            for(int i = 0; i<HarvestMoon.Instance.Cows; ++i)
            {
                _entityManager.AddEntity(new Cow(Content, _cowPlacements[i].Position, _cowPlacements[i].Size));
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

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            float scaleY = HarvestMoon.Instance.Graphics.GraphicsDevice.Viewport.Height / 480.0f;

            var cameraMatrix = _camera.GetViewMatrix();
            cameraMatrix.Translation = new Vector3(cameraMatrix.Translation.X, cameraMatrix.Translation.Y, cameraMatrix.Translation.Z);

            _spriteBatch.Begin(transformMatrix: cameraMatrix, samplerState: SamplerState.PointClamp);


            for (int i = 0; i < _map.Layers.Count; ++i)
            {
                _mapRenderer.Draw(_map.Layers[i], cameraMatrix);

            }
            // End the sprite batch
            _spriteBatch.End();

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix());
            _entityManager.Draw(_spriteBatch);
            _spriteBatch.End();

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix());

            foreach (var foregroundLayer in _map.Layers.Where(l => l is TiledMapGroupLayer))
            {
                var foregroundLayers = (foregroundLayer as TiledMapGroupLayer).Layers.Where(l => l.Name.Contains("-Foreground"));

                foreach (var layer in foregroundLayers)
                {
                    _mapRenderer.Draw(layer, cameraMatrix);

                }
            }

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
