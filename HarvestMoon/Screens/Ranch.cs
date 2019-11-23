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
        
        TiledMapEffect _dayTimeEffect;

        Slide<Color> _sunriseToMorningColor;
        Slide<Color> _morningToEveningColor;
        Slide<Color> _eveningToAfternoonColor;
        Slide<Color> _afternoonToSunriseColor;

        Slide<Color> _currentDayTimeColorSlider;

        Color _currentDayTimeColor;

        private float _dayTime;

        private float _day;

        private float _morning;
        private float _evening;
        private float _afternoon;

        private bool _isFromHouse;

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


        private void ResetDayTime()
        {
            _dayTime = 0.0f;

            _day = 2.5f;

            _morning = (_day * 60.0f / 3) * 1;
            _evening = (_day * 60.0f / 3) * 2;
            _afternoon = (_day * 60.0f / 3) * 3;


            _sunriseToMorningColor = new Slide<Color>(Color.White, Color.LightYellow, 2000d, Color.Lerp);
            _morningToEveningColor = new Slide<Color>(Color.LightYellow, new Color(220, 220, 180), 2000d, Color.Lerp);
            _eveningToAfternoonColor = new Slide<Color>(new Color(220, 220, 180), Color.DarkGray, 2000d, Color.Lerp);
            _afternoonToSunriseColor = new Slide<Color>(Color.DarkGray, Color.White, 2000d, Color.Lerp);

            _currentDayTimeColorSlider = _afternoonToSunriseColor;
        }

        private void AdvanceDayTime()
        {
            if(_currentDayTimeColorSlider == _afternoonToSunriseColor)
            {
                _currentDayTimeColorSlider = _sunriseToMorningColor;
            }
            else if(_currentDayTimeColorSlider == _sunriseToMorningColor)
            {
                _currentDayTimeColorSlider = _morningToEveningColor;
            }
            else if(_currentDayTimeColorSlider == _morningToEveningColor)
            {
                _currentDayTimeColorSlider = _eveningToAfternoonColor;
            }
            /*else if(_currentDayTimeColor == _eveningToAfternoonColor)
            {
                _currentDayTimeColor = _afternoonToSunriseColor;
            }
            else
            {
                ResetDayTime();
            }*/
        }


        public override void LoadContent()
        {
            base.LoadContent();

            // Load the compiled map
            _map = Content.Load<TiledMap>("maps/ranch/level");
            // Create the map renderer
            _mapRenderer = new TiledMapRenderer(GraphicsDevice, _map);

            _dayTimeEffect = new TiledMapEffect(GraphicsDevice);

            _dayTimeEffect.TextureEnabled = true;
            _dayTimeEffect.DiffuseColor = Color.White;

            ResetDayTime();

            _dayTime = HarvestMoon.Instance.RanchDayTime;

            switch (HarvestMoon.Instance.GetDayTime())
            {
                case HarvestMoon.DayTime.Sunrise:
                    _currentDayTimeColorSlider = _afternoonToSunriseColor;
                    break;

                case HarvestMoon.DayTime.Morning:
                    _currentDayTimeColorSlider = _sunriseToMorningColor;
                    break;

                case HarvestMoon.DayTime.Evening:
                    _currentDayTimeColorSlider = _morningToEveningColor;
                    break;

                case HarvestMoon.DayTime.Afternoon:
                    _currentDayTimeColorSlider = _eveningToAfternoonColor;
                    break;
            }


            var ranchState = _entityManager as RanchState;
            if(!ranchState.IsLoaded || HarvestMoon.Instance.HasNotSeenTheRanch)
            {
                HarvestMoon.Instance.HasNotSeenTheRanch = false;

                foreach (var layer in _map.ObjectLayers)
                {
                    if (layer.Name == "objects")
                    {
                        foreach (var obj in layer.Objects)
                        {
                            if (obj.Type == "player_start" && obj.Name == "house" && _arrival == HarvestMoon.Arrival.House)
                            {
                                var objectPosition = obj.Position;

                                objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                                objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                                _player = _entityManager.AddEntity(new Jack(Content, _entityManager, this, objectPosition));
                                _player.UnFreeze();
                                _player.PlayerFacing = Jack.Facing.DOWN;
                            }
                            else if(obj.Type == "player_start" && obj.Name == "passage" && _arrival == HarvestMoon.Arrival.Passage)
                            {
                                var objectPosition = obj.Position;

                                objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                                objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                                _player = _entityManager.AddEntity(new Jack(Content, _entityManager, this, objectPosition));
                                _player.UnFreeze();
                                _player.PlayerFacing = Jack.Facing.RIGHT;

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

                    var lastDoorPosition = _player.LastVisitedDoor.BoundingRectangle.Center;
                    var lastDoorSize = _player.LastVisitedDoor.BoundingRectangle.Size;

                    _player.Position = new Vector2(lastDoorPosition.X,
                                                    lastDoorPosition.Y + lastDoorSize.Height * 0.5f +
                                                        _player.BoundingRectangle.Size.Height * 0.5f + 2);

                    _player.PlayerFacing = Jack.Facing.DOWN;
                }
                else
                {
                    foreach (var layer in _map.ObjectLayers)
                    {
                        if (layer.Name == "objects")
                        {
                            foreach (var obj in layer.Objects)
                            {
                                if (obj.Type == "player_start" && obj.Name == "house" && _arrival == HarvestMoon.Arrival.House)
                                {
                                    var objectPosition = obj.Position;

                                    objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                                    objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                                    _player = _entityManager.AddEntity(new Jack(Content, _entityManager, this, objectPosition));
                                    _player.UnFreeze();
                                    _player.PlayerFacing = Jack.Facing.DOWN;
                                }
                                else if (obj.Type == "player_start" && obj.Name == "passage" && _arrival == HarvestMoon.Arrival.Passage)
                                {
                                    var objectPosition = obj.Position;

                                    objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                                    objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                                    _player = _entityManager.AddEntity(new Jack(Content, _entityManager, this, objectPosition));
                                    _player.UnFreeze();
                                    _player.PlayerFacing = Jack.Facing.RIGHT;

                                }
                            }
                        }
                    }

                }

            }

            foreach (var layer in _map.ObjectLayers)
            {
                if (layer.Name == "doors")
                {
                    foreach (var obj in layer.Objects)
                    {
                        if (obj.Type == "door")
                        {
                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                            objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                            var objectSize = obj.Size;

                            var door = new Door(objectPosition, objectSize);
                            _entityManager.AddEntity(door);

                            door.Name = obj.Name;

                            if (obj.Name == "tools-room")
                            {
                                door.OnTrigger(() =>
                                {
                                    if (!door.Triggered)
                                    {
                                        _player.Freeze();
                                        _player.LastVisitedDoor = door;

                                        door.Triggered = true;
                                        var screen = new ToolsRoom(Game);
                                        var transition = new FadeTransition(GraphicsDevice, Color.Black, 1.0f);
                                        ScreenManager.LoadScreen(screen, transition);
                                    }
                                });
                            }
                            else if (obj.Name == "house")
                            {
                                door.OnTrigger(() =>
                                {
                                    if (!door.Triggered)
                                    {
                                        _player.Freeze();
                                        _player.LastVisitedDoor = door;

                                        door.Triggered = true;
                                        var screen = new House(Game, HarvestMoon.Arrival.Ranch);
                                        var transition = new FadeTransition(GraphicsDevice, Color.Black, 1.0f);
                                        ScreenManager.LoadScreen(screen, transition);
                                    }
                                });
                            }
                            else if (obj.Name == "passage")
                            {
                                door.OnTrigger(() =>
                                {
                                    if (!door.Triggered)
                                    {
                                        _player.Freeze();
                                        _player.LastVisitedDoor = door;

                                        door.Triggered = true;
                                        var screen = new Passage(Game, HarvestMoon.Arrival.Ranch);
                                        var transition = new FadeTransition(GraphicsDevice, Color.Black, 1.0f);
                                        ScreenManager.LoadScreen(screen, transition);
                                    }
                                });
                            }
                        }

                    }
                }
                if (layer.Name == "grids")
                {
                    foreach (var obj in layer.Objects)
                    {
                        if (obj.Type == "grid")
                        {

                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                            objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                            var objectSize = obj.Size;

                            _grids.Add(_entityManager.AddEntity(new Grid(objectPosition, objectSize)));
                        }
                    }
                }
                if (layer.Name == "walls")
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

            foreach (var layer in _map.ObjectLayers)
            {
                if (layer.Name == "objects")
                {
                    foreach (var obj in layer.Objects)
                    {
                        if (obj.Type == "npc")
                        {
                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                            objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                            var objectSize = obj.Size;

                            var objectMessage = obj.Properties.First(p => p.Key.Contains("message"));

                            if (obj.Name == "materials-signpost")
                            {
                                _entityManager.AddEntity(new WoodSignPost(objectPosition, objectSize, true, objectMessage.Value));
                            }
                            else if (obj.Name == "feed-signpost")
                            {
                                _entityManager.AddEntity(new FooderSignPost(objectPosition, objectSize, true, objectMessage.Value));
                            }
                            else
                            {
                                _entityManager.AddEntity(new NPC(objectPosition, objectSize, true, objectMessage.Value));
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


        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // TODO: Add your update logic here
            // Update the map
            // map Should be the `TiledMap`
            var deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _dayTime += deltaSeconds;

            if(_dayTime >= _morning && _dayTime <= _evening)
            {
                if(_currentDayTimeColorSlider == _afternoonToSunriseColor)
                {
                    AdvanceDayTime();
                }
                
            }
            else if(_dayTime >= _evening && _dayTime <= _afternoon)
            {
                if (_currentDayTimeColorSlider == _sunriseToMorningColor)
                {
                    AdvanceDayTime();
                }

            }
            else if(_dayTime >= _afternoon)
            {
                if (_currentDayTimeColorSlider == _morningToEveningColor)
                {
                    AdvanceDayTime();
                }
            }

            HarvestMoon.Instance.RanchDayTime = _dayTime;

            if (HarvestMoon.Instance.GetDayTimeTriggered(HarvestMoon.Instance.GetDayTime()))
            {
                switch (HarvestMoon.Instance.GetDayTime())
                {
                    case HarvestMoon.DayTime.Sunrise:
                        _currentDayTimeColor = Color.White;
                        break;

                    case HarvestMoon.DayTime.Morning:
                        _currentDayTimeColor = Color.LightYellow;
                        break;

                    case HarvestMoon.DayTime.Evening:
                        _currentDayTimeColor = new Color(220, 220, 180);
                        break;

                    case HarvestMoon.DayTime.Afternoon:
                        _currentDayTimeColor = Color.DarkGray;
                        break;
                }
            }
            else
            {
                _currentDayTimeColor = _currentDayTimeColorSlider.Update(gameTime);
                
                if(_currentDayTimeColor == Color.White)
                {
                    if (HarvestMoon.Instance.GetDayTime() == HarvestMoon.DayTime.Sunrise)
                    {
                        HarvestMoon.Instance.SetDayTimeTriggered(HarvestMoon.DayTime.Sunrise, true);
                    }
                }
                else if(_currentDayTimeColor == Color.LightYellow)
                {
                    if (HarvestMoon.Instance.GetDayTime() == HarvestMoon.DayTime.Morning)
                    {
                        HarvestMoon.Instance.SetDayTimeTriggered(HarvestMoon.DayTime.Morning, true);
                    }
                }
                else if(_currentDayTimeColor == new Color(220, 220, 180))
                {
                    if(HarvestMoon.Instance.GetDayTime() == HarvestMoon.DayTime.Evening)
                    {
                        HarvestMoon.Instance.SetDayTimeTriggered(HarvestMoon.DayTime.Evening, true);
                    }
                    
                }
                else if(_currentDayTimeColor == Color.DarkGray)
                {
                    if(HarvestMoon.Instance.GetDayTime() == HarvestMoon.DayTime.Afternoon)
                    {
                        HarvestMoon.Instance.SetDayTimeTriggered(HarvestMoon.DayTime.Afternoon, true);

                        if(HarvestMoon.Instance.TodayGold != 0)
                        {
                            string harvest = "Is that all you are shipping today? It costs " +
                                            HarvestMoon.Instance.TodayGold.ToString() + "G" + " in total. " +
                                            "I'll put money in the box tomorrow";

                            ShowMessage(harvest, null);
                        }
                        else
                        {
                            string harvest = "Is there no shipping today? Ok, bye.";
                            ShowMessage(harvest, null);
                        }
                        
                    }
                    
                }

            }

            _dayTimeEffect.DiffuseColor = _currentDayTimeColor;

            CheckCollisions();


            _mapRenderer.Update(gameTime);
            _entityManager.Update(gameTime);

            if (_player != null && !_player.IsDestroyed)
            {
                _camera.LookAt(_player.Position);
                var constraints = new Vector2();

                if(_camera.BoundingRectangle.Center.X < 320)
                {
                    constraints.X = 320;
                }

                if (_camera.BoundingRectangle.Center.X > 1760)
                {
                    constraints.X = 1760;
                }

                if (_camera.BoundingRectangle.Center.Y < 240)
                {
                    constraints.Y = 240;
                }

                if (_camera.BoundingRectangle.Center.Y > 1712)
                {
                    constraints.Y = 1712;
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
               
        public override void OnPreGuiDraw(GameTime gameTime)
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
                _mapRenderer.Draw(_map.Layers[i], cameraMatrix, effect: _dayTimeEffect);

            }
            // End the sprite batch
            _spriteBatch.End();

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix());
            _entityManager.Draw(_spriteBatch);
            _spriteBatch.End();

            _spriteBatch.Begin(transformMatrix: cameraMatrix, samplerState: SamplerState.PointClamp);

            _mapRenderer.Draw(_map.Layers.First(l => l.Name == "Foreground"), cameraMatrix, effect: _dayTimeEffect);

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
        }
    }
}
