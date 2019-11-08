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
using System.Collections.Generic;
using System;

namespace HarvestMoon.Screens
{
    public class House : Map
    {
        private bool _isWakeUp;
        private bool _isFromRanch;
        private bool _isFromDiary;

        public House(Game game, bool isWakeUp = false, bool isFromRanch = false, bool isFromDiary = false)
            : base(game)
        {
            _isWakeUp = isWakeUp;
            _isFromRanch = isFromRanch;
            _isFromDiary = isFromDiary;
        }

        public override void Initialize()
        {
            // call base initialize func
            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            if (!_isFromRanch && _isFromDiary)
            {
                HarvestMoon.Instance.LoadGameState(HarvestMoon.Instance.Diary);
            }

            // Load the compiled map
            _map = Content.Load<TiledMap>("maps/house/house");
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
                            if(obj.Name == "start" && !_isWakeUp)
                            {
                                var objectPosition = obj.Position;

                                objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                                objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                                _player = _entityManager.AddEntity(new Jack(Content, _entityManager, this, objectPosition));
                                _player.PlayerFacing = Jack.Facing.UP;
                            }
                            else if(obj.Name == "wake" && _isWakeUp)
                            {
                                var objectPosition = obj.Position;

                                objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                                objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                                _player = _entityManager.AddEntity(new Jack(Content, _entityManager, this, objectPosition));
                                _player.PlayerFacing = Jack.Facing.DOWN;
                            }

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
                                        var screen = new Ranch(Game);
                                        var transition = new FadeTransition(GraphicsDevice, Color.Black, 1.0f);
                                        ScreenManager.LoadScreen(screen, transition);
                                    }
                                });
                            }
                        }

                        if(obj.Type == "npc")
                        {
                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                            objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                            var objectSize = obj.Size;

                            var objectMessage = obj.Properties.First(p => p.Key.Contains("message"));

                            if(obj.Name == "diary")
                            {
                                _entityManager.AddEntity(new NPC(objectPosition,
                                                                 objectSize,
                                                                 objectMessage.Value,
                                                                 true,
                                                                 NPC.NPCMenu.YesNo,
                                                                 new List<string>() { "Yes", "No" },
                                                                 new List<Action>() {
                                                                     () => {

                                                                        _player.Freeze();
                                                                        _player.Busy();
                                                                        _player.Cooldown();
                                                                        HarvestMoon.Instance.ResetDay();
                                                                        HarvestMoon.Instance.IncrementDay();
                                                                        HarvestMoon.Instance.SaveGameState(HarvestMoon.Instance.Diary);

                                                                        var screen = new House(Game, true);
                                                                        var transition = new FadeTransition(GraphicsDevice, Color.Black, 2.0f);
                                                                        ScreenManager.LoadScreen(screen, transition);

                                                                     },
                                                                     () => {
                                                                        ShowYesNoMessage("I'm going to bed.",
                                                                                         "Oh, I've got something to do.",
                                                                                        () =>
                                                                                        {

                                                                                            _player.Freeze();
                                                                                            _player.Busy();
                                                                                            _player.Cooldown();
                                                                                            HarvestMoon.Instance.ResetDay();
                                                                                            HarvestMoon.Instance.IncrementDay();
                                                                                            var screen = new House(Game, true);
                                                                                            var transition = new FadeTransition(GraphicsDevice, Color.Black, 2.0f);
                                                                                            ScreenManager.LoadScreen(screen, transition);

                                                                                        },
                                                                                        () =>
                                                                                        {
                                                                                            _player.UnFreeze();
                                                                                            _player.Cooldown();
                                                                                        });
                                                                 } }));
                            }
                            else if(obj.Name == "calendar")
                            {
                                var replacedDayName = objectMessage.Value.Replace("day", HarvestMoon.Instance.DayName);
                                var replacedDayNumber = replacedDayName.Replace("number", HarvestMoon.Instance.DayNumber.ToString());
                                var replacedSeason = replacedDayNumber.Replace("season", HarvestMoon.Instance.Season);

                                _entityManager.AddEntity(new NPC(objectPosition, objectSize, replacedSeason));
                            }
                            else
                            {
                                _entityManager.AddEntity(new NPC(objectPosition, objectSize, objectMessage.Value));
                            }
                            
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

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Transform matrix is only needed if you have a Camera2D
            // Setting the sampler state to `SamplerState.PointClamp` is reccomended to remove gaps between the tiles when rendering
            //spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);
            //spriteBatch.Begin();
            var cameraMatrix = _camera.GetViewMatrix();
            cameraMatrix.Translation = new Vector3(cameraMatrix.Translation.X, cameraMatrix.Translation.Y - 32, cameraMatrix.Translation.Z);

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
