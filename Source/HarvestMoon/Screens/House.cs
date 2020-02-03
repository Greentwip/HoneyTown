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
using HarvestMoon.Entities.Ranch;
using Microsoft.Xna.Framework.Media;

namespace HarvestMoon.Screens
{
    public class House : Map
    {
        private HarvestMoon.Arrival _arrival;

        public House(Game game, HarvestMoon.Arrival arrival)
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

            var song = Content.Load<Song>("audio/music/spring");

            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;

            if (_arrival == HarvestMoon.Arrival.Diary)
            {
                HarvestMoon.Instance.LoadGameState(HarvestMoon.Instance.Diary);
            }

            if (_arrival != HarvestMoon.Arrival.Ranch)
            {
                HarvestMoon.Instance.Stamina = 60;

                HarvestMoon.Instance.Gold += HarvestMoon.Instance.TodayGold;
                HarvestMoon.Instance.TodayGold = 0;

                var soilSegments = HarvestMoon.Instance.RanchState.Entities.Where(e => e is Soil).Cast<Soil>().ToArray();

                foreach (var soilSegment in soilSegments)
                {
                    soilSegment.GrowAccordingly();
                    soilSegment.Dry();
                }
            }
            

            // Load the compiled map
            _map = Content.Load<TiledMap>("maps/House");
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

                            if(_player == null)
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
                        else if (obj.Name == "from-diary" && (_arrival == HarvestMoon.Arrival.Wake || _arrival == HarvestMoon.Arrival.Diary))
                        {
                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                            objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                            _player = _entityManager.AddEntity(new Jack(Content, _entityManager, this, objectPosition));
                            _player.PlayerFacing = Jack.Facing.DOWN;

                            HarvestMoon.Instance.ResetDayTime();

                        }
                        else if (obj.Name == "from-tools" && _arrival == HarvestMoon.Arrival.Tools)
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

                            _player.PlayerFacing = Jack.Facing.LEFT;
                        }
                    }
                }
                else if(layer.Name == "Interactables")
                {
                    foreach (var obj in layer.Objects)
                    {
                        if (obj.Type == "npc")
                        {
                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                            objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                            var objectSize = obj.Size;

                            var objectMessageKP = obj.Properties.First(p => p.Key.Contains("message"));

                            var objectMessage = HarvestMoon.Instance.Strings.Get(objectMessageKP.Value);

                            var yes = HarvestMoon.Instance.Strings.Get("STR_YES");
                            var no = HarvestMoon.Instance.Strings.Get("STR_NO");

                            if (obj.Name == "diary")
                            {
                                _entityManager.AddEntity(new YesNoMessage(objectPosition,
                                                                 objectSize,
                                                                 objectMessage,
                                                                 new List<string>() { yes, no },
                                                                 new List<Action>() {
                                                                     () => {

                                                                        HarvestMoon.Instance.ResetDay();
                                                                        HarvestMoon.Instance.IncrementDay();
                                                                        HarvestMoon.Instance.SaveGameState(HarvestMoon.Instance.Diary);

                                                                        var screen = new House(Game, HarvestMoon.Arrival.Wake);
                                                                        var transition = new FadeTransition(GraphicsDevice, Color.Black, 2.0f);
                                                                        ScreenManager.LoadScreen(screen, transition);

                                                                     },
                                                                     () => {
                                                                        /*ShowYesNoMessage("I'm going to bed.",
                                                                                         "Oh, I've got something to do.",
                                                                                        () =>
                                                                                        {
                                                                                            HarvestMoon.Instance.ResetDay();
                                                                                            HarvestMoon.Instance.IncrementDay();
                                                                                            var screen = new House(Game, HarvestMoon.Arrival.Wake);
                                                                                            var transition = new FadeTransition(GraphicsDevice, Color.Black, 2.0f);
                                                                                            ScreenManager.LoadScreen(screen, transition);

                                                                                        },
                                                                                        () =>
                                                                                        {
                                                                                        });*/
                                                                 } }));
                            }
                            else if (obj.Name == "calendar")
                            {
                                var replacedDayName = objectMessage.Replace("day", HarvestMoon.Instance.DayName);
                                var replacedDayNumber = replacedDayName.Replace("number", HarvestMoon.Instance.DayNumber.ToString());
                                var replacedSeason = replacedDayNumber.Replace("season", HarvestMoon.Instance.Season);

                                _entityManager.AddEntity(new BasicMessage(objectPosition, objectSize, replacedSeason));
                            }
                            else
                            {
                                _entityManager.AddEntity(new BasicMessage(objectPosition, objectSize, objectMessage));
                            }

                        }

                    }

                }
                else if(layer.Name == "Doors")
                {
                    foreach(var obj in layer.Objects)
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

                        if (obj.Name == "ranch")
                        {
                            door.OnTriggerEnd(() =>
                            {
                                if (!door.Triggered)
                                {
                                    door.Triggered = true;
                                    var screen = new Ranch(Game, HarvestMoon.Arrival.House);
                                    var transition = new FadeTransition(GraphicsDevice, Color.Black, 1.0f);
                                    ScreenManager.LoadScreen(screen, transition);
                                }
                            });
                        }
                        else if(obj.Name == "tools")
                        {
                            door.OnTriggerEnd(() =>
                            {
                                if (!door.Triggered)
                                {
                                    door.Triggered = true;
                                    var screen = new ToolsRoom(Game);
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
