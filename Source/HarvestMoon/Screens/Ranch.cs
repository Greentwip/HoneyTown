using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;
using System;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using System.Collections.Generic;

using HarvestMoon.Entities;
using System.Linq;
using MonoGame.Extended.Screens.Transitions;
using HarvestMoon.Entities.General;
using HarvestMoon.Entities.Ranch;

namespace HarvestMoon.Screens
{
    public class Slide<T>
    {
        #region Fields

        private double _currentMS;
        private T _end;
        private Func<T, T, float, T> _function;
        private T _start;
        private double _totalMS;

        #endregion Fields

        #region Constructors

        public Slide(T start, T end, double totalMS, Func<T, T, float, T> function)
        {
            _start = start;
            _end = end;
            _totalMS = totalMS;
            _function = function;
        }

        #endregion Constructors

        #region Properties

        public double CurrentMS => _currentMS;
        public bool Done => _currentMS >= _totalMS;

        public T End { get => _end; set => _end = value; }
        public Func<T, T, float, T> Function { get => _function; set => _function = value; }
        public T Start { get => _start; set => _start = value; }
        public double TotalMS { get => _totalMS; set => _totalMS = value; }

        #endregion Properties

        #region Methods

        public void Restart() => _currentMS = 0d;

        public void Reverse()
        {
            T tmp = _start; _start = _end; _end = tmp;
        }

        public float UpdatePercent(GameTime gameTime)
        {
            if (!Done)
            {
                _currentMS += gameTime.ElapsedGameTime.TotalMilliseconds;
                return (float)(Done ? 1f : _currentMS / _totalMS);
            }
            else
                return 1f;
        }

        public T Update(GameTime gameTime)
        {
            float percent;
            if (_function != null && (percent = UpdatePercent(gameTime)) != 1f)
                return _function(_start, _end, percent);
            return _end;
        }

        #endregion Methods
    }

    public class Ranch : Map
    {

        private List<WoodPiece> _woodPieces = new List<WoodPiece>();
        private List<SmallRock> _smallRocks = new List<SmallRock>();
        private List<BigRock> _bigRocks = new List<BigRock>();
        private List<BigLog> _bigLogs = new List<BigLog>();
        private List<Bush> _bushes = new List<Bush>();
        private List<Grid> _grids = new List<Grid>();
        
        private HarvestMoon.Arrival _arrival;

        public Ranch(Game game, HarvestMoon.Arrival arrival)
            : base(game)
        {
            _arrival = arrival;
            _entityManager = HarvestMoon.Instance.RanchState;
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
            _map = Content.Load<TiledMap>("maps/Ranch");
            // Create the map renderer
            _mapRenderer = new TiledMapRenderer(GraphicsDevice, _map);

            _dayTime = HarvestMoon.Instance.RanchDayTime;

            var ranchState = _entityManager as RanchState;
            if(!ranchState.IsLoaded || HarvestMoon.Instance.HasNotSeenTheRanch)
            {
                HarvestMoon.Instance.HasNotSeenTheRanch = false;

                foreach (var layer in _map.ObjectLayers)
                {
                    if (layer.Name == "Arrivals")
                    {
                        foreach (var obj in layer.Objects)
                        {
                            if (obj.Name == "house" && _arrival == HarvestMoon.Arrival.House)
                            {
                                var objectPosition = obj.Position;

                                objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                                objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                                _player = _entityManager.AddEntity(new Jack(Content, _entityManager, this, objectPosition));
                                _player.UnFreeze();
                                _player.PlayerFacing = Jack.Facing.DOWN;
                            }
                            else if (obj.Name == "barn" && _arrival == HarvestMoon.Arrival.Barn)
                            {
                                var objectPosition = obj.Position;

                                objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                                objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                                _player = _entityManager.AddEntity(new Jack(Content, _entityManager, this, objectPosition));
                                _player.UnFreeze();

                                _player.PlayerFacing = Jack.Facing.DOWN;

                            }
                            else if(obj.Name == "from-city" && _arrival == HarvestMoon.Arrival.Town)
                            {
                                var objectPosition = obj.Position;

                                objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                                objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                                _player = _entityManager.AddEntity(new Jack(Content, _entityManager, this, objectPosition));
                                _player.UnFreeze();

                                _player.PlayerFacing = Jack.Facing.DOWN;
                            }
                        }
                    }
                }
                
                foreach (var layer in _map.ObjectLayers)
                {
                    if (layer.Name == "objects")
                    {
                        foreach (var obj in layer.Objects)
                        {
                           
                            if (obj.Type == "wood")
                            {

                                var objectPosition = obj.Position;

                                objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                                objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                                _woodPieces.Add(_entityManager.AddEntity(new WoodPiece(Content, objectPosition)));
                            }

                            if (obj.Type == "rock-small")
                            {

                                var objectPosition = obj.Position;

                                objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                                objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                                _smallRocks.Add(_entityManager.AddEntity(new SmallRock(Content, objectPosition)));
                            }

                            if (obj.Type == "rock-big")
                            {

                                var objectPosition = obj.Position;

                                objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                                objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                                _bigRocks.Add(_entityManager.AddEntity(new BigRock(Content, objectPosition)));
                            }

                            if (obj.Type == "log-big")
                            {

                                var objectPosition = obj.Position;

                                objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                                objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                                _bigLogs.Add(_entityManager.AddEntity(new BigLog(Content, objectPosition)));
                            }

                            if (obj.Type == "bush")
                            {

                                var objectPosition = obj.Position;

                                objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                                objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                                _bushes.Add(_entityManager.AddEntity(new Bush(Content, objectPosition)));
                            }

                        }
                    }
                }

                ranchState.IsLoaded = true;
            }
            else
            {
                _player = _entityManager.Entities.FirstOrDefault(e => e is Jack) as Jack;

                if(_player != null)
                {
                    _player.MapScreen = this;
                    _player.UnFreeze();


                    foreach (var layer in _map.ObjectLayers)
                    {
                        if (layer.Name == "Arrivals")
                        {
                            foreach (var obj in layer.Objects)
                            {
                                if (obj.Name == "house" && _arrival == HarvestMoon.Arrival.House)
                                {
                                    var objectPosition = obj.Position;

                                    objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                                    objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                                    _player.Position = new Vector2(objectPosition.X, objectPosition.Y);

                                    _player.UnFreeze();
                                    _player.PlayerFacing = Jack.Facing.DOWN;
                                }
                                else if (obj.Name == "barn" && _arrival == HarvestMoon.Arrival.Barn)
                                {
                                    var objectPosition = obj.Position;

                                    objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                                    objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                                    _player.Position = new Vector2(objectPosition.X, objectPosition.Y);

                                    _player.UnFreeze();
                                    _player.PlayerFacing = Jack.Facing.DOWN;

                                }
                                else if (obj.Name == "from-city" && _arrival == HarvestMoon.Arrival.Town)
                                {
                                    var objectPosition = obj.Position;

                                    objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                                    objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                                    _player.Position = new Vector2(objectPosition.X, objectPosition.Y);

                                    _player.UnFreeze();
                                    _player.PlayerFacing = Jack.Facing.DOWN;
                                }
                            }

                        }
                    }

                }
                else
                {
                    foreach (var layer in _map.ObjectLayers)
                    {
                        if (layer.Name == "Arrivals")
                        {
                            foreach (var obj in layer.Objects)
                            {
                                if (obj.Name == "house" && _arrival == HarvestMoon.Arrival.House)
                                {
                                    var objectPosition = obj.Position;

                                    objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                                    objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                                    _player = _entityManager.AddEntity(new Jack(Content, _entityManager, this, objectPosition));
                                    _player.UnFreeze();
                                    _player.PlayerFacing = Jack.Facing.DOWN;
                                }
                                else if (obj.Name == "barn" && _arrival == HarvestMoon.Arrival.Barn)
                                {
                                    var objectPosition = obj.Position;

                                    objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                                    objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                                    _player = _entityManager.AddEntity(new Jack(Content, _entityManager, this, objectPosition));
                                    _player.UnFreeze();
                                    _player.PlayerFacing = Jack.Facing.DOWN;

                                }
                                else if (obj.Name == "from-city" && _arrival == HarvestMoon.Arrival.Town)
                                {
                                    var objectPosition = obj.Position;

                                    objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                                    objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                                    _player = _entityManager.AddEntity(new Jack(Content, _entityManager, this, objectPosition));

                                    _player.UnFreeze();
                                    _player.PlayerFacing = Jack.Facing.DOWN;
                                }
                            }
                        }
                    }

                }

            }

            foreach (var layer in _map.ObjectLayers)
            {
                if (layer.Name == "Doors")
                {
                    foreach (var obj in layer.Objects)
                    {
                        var objectPosition = obj.Position;

                        objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                        objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                        var objectSize = obj.Size;


                        if (obj.Name == "barn")
                        {
                            var door = new BarnDoor(Content, objectPosition, objectSize);
                            _entityManager.AddEntity(door);

                            door.Name = obj.Name;

                            door.OnTriggerStart(() =>
                            {
                                _player.Freeze();
                            });

                            door.OnTriggerEnd(() =>
                            {
                                if (!door.Triggered)
                                {
                                    door.Triggered = true;
                                    var screen = new Barn(Game, HarvestMoon.Arrival.Ranch);
                                    var transition = new FadeTransition(GraphicsDevice, Color.Black, 1.0f);
                                    ScreenManager.LoadScreen(screen, transition);
                                }
                            });
                        }
                        else if (obj.Name == "house")
                        {
                            var door = new RanchDoor(Content, objectPosition, objectSize);
                            _entityManager.AddEntity(door);

                            door.Name = obj.Name;

                            door.OnTriggerStart(() =>
                            {
                                _player.Freeze();
                            });

                            door.OnTriggerEnd(() =>
                            {
                                if (!door.Triggered)
                                {
                                    door.Triggered = true;
                                    var screen = new House(Game, HarvestMoon.Arrival.Ranch);
                                    var transition = new FadeTransition(GraphicsDevice, Color.Black, 1.0f);
                                    ScreenManager.LoadScreen(screen, transition);
                                }
                            });
                        }
                        else if(obj.Name == "city")
                        {
                            var door = new Door(objectPosition, objectSize);
                            _entityManager.AddEntity(door);

                            door.Name = obj.Name;

                            door.OnTriggerStart(() =>
                            {
                                _player.Freeze();
                            });

                            door.OnTriggerEnd(() =>
                            {
                                if (!door.Triggered)
                                {
                                    door.Triggered = true;
                                    var screen = new Town(Game, HarvestMoon.Arrival.Ranch);
                                    var transition = new FadeTransition(GraphicsDevice, Color.Black, 1.0f);
                                    ScreenManager.LoadScreen(screen, transition);
                                }
                            });
                        }
                    }
                }
                if (layer.Name == "Plot")
                {
                    foreach (var obj in layer.Objects)
                    {
                        var objectPosition = obj.Position;

                        objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                        objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                        var objectSize = obj.Size;

                        _grids.Add(_entityManager.AddEntity(new Grid(objectPosition, objectSize)));
                    }
                }
                if (layer.Name == "Walls")
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

            foreach (var layer in _map.ObjectLayers)
            {
                if (layer.Name == "Interactables")
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

                            string objectMessage = HarvestMoon.Instance.Strings.Get(objectMessageKP.Value);

                            if (obj.Name == "materials-signpost")
                            {
                                _entityManager.AddEntity(new WoodSignPost(objectPosition, objectSize, objectMessage));
                            }
                            else if (obj.Name == "feed-signpost")
                            {
                                _entityManager.AddEntity(new FooderSignPost(objectPosition, objectSize, objectMessage));
                            }
                            else
                            {
                                _entityManager.AddEntity(new BasicMessage(objectPosition, objectSize, objectMessage));
                            }

                        }

                        if(obj.Type == "special")
                        {
                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                            objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                            if (obj.Name == "shipping-box")
                            {
                                _entityManager.AddEntity(new ShippingBox(Content, objectPosition));
                            }
                        }
                    }

                }
            }

            LoadPlayer();
        }

        private float _dayTime;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // TODO: Add your update logic here
            // Update the map
            // map Should be the `TiledMap`

            var dayTimeEffect = HarvestMoon.Instance.DayTimeEffect;

            var currentDayTimeColor = dayTimeEffect.DiffuseColor;


            if (currentDayTimeColor == Color.DarkGray)
            {
                if (HarvestMoon.Instance.GetDayTime() == HarvestMoon.DayTime.Afternoon)
                {
                    if (!HarvestMoon.Instance.HasNightTriggered())
                    {
                        HarvestMoon.Instance.SetNightTriggered(true);

                        string harvest = "STR_NOSHIPPING";

                        if (HarvestMoon.Instance.TodayGold != 0)
                        {
                            harvest = "STR_SHIPPINGTODAY";

                            harvest = HarvestMoon.Instance.Strings.Get(harvest);

                            harvest = harvest.Replace("gold", HarvestMoon.Instance.TodayGold.ToString());
                        }
                        else
                        {
                            harvest = HarvestMoon.Instance.Strings.Get(harvest);
                        }
                        HarvestMoon.Instance.GUI.ShowMessage(harvest,
                            () => { _player.Freeze(); _player.Busy(); },
                            () => { _player.UnFreeze(); _player.Cooldown(); });

                    }

                }

            }

            CheckCollisions();


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

                if(constraints.X != 0)
                {
                    _camera.LookAt(new Vector2(constraints.X, _player.Position.Y));
                }

                if (constraints.Y != 0)
                {
                    _camera.LookAt(new Vector2(_player.Position.X, constraints.Y));
                }

                if(constraints.X != 0 && constraints.Y != 0)
                {
                    _camera.LookAt(new Vector2(constraints.X, constraints.Y));
                }

            }

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

            foreach(var foregroundLayer in _map.Layers.Where(l => l is TiledMapGroupLayer))
            {
                var foregroundLayers = (foregroundLayer as TiledMapGroupLayer).Layers.Where(l => l.Name.Contains("-Foreground"));

                foreach(var layer in foregroundLayers)
                {
                    _mapRenderer.Draw(layer, cameraMatrix, effect: HarvestMoon.Instance.DayTimeEffect);

                }
            }


            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
