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
using HarvestMoon.Entities.Town;
using Microsoft.Xna.Framework.Media;

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

        private bool _mediaShouldFadeToTown;

        public static Random Random = new Random(Cow.Random.Next(256 + new Random().Next(128)));

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


        private enum Randomization
        {
            SmallRock,
            Bush,
            BigRock,
            WoodLog
        }

        private void Randomize(Grid container, int amount, Size2 objectSize, Randomization randomization)
        {
            for (int i = 0; i < amount; ++i) 
            {
                int chancesX = (int)(container.BoundingRectangle.Width / objectSize.Width);
                int chancesY = (int)(container.BoundingRectangle.Height / objectSize.Height);

                var objectPosition = new Vector2(container.BoundingRectangle.TopLeft.X + Random.Next(0, chancesX) * objectSize.Width,
                                                 container.BoundingRectangle.TopLeft.Y + Random.Next(0, chancesY) * objectSize.Height);


                objectPosition.X = objectPosition.X + objectSize.Width * 0.5f;
                objectPosition.Y = objectPosition.Y + objectSize.Height * 0.5f;

                Interactable newEntity = null;

                if(randomization == Randomization.SmallRock)
                {
                    newEntity = new SmallRock(Content, objectPosition);
                }
                else if(randomization == Randomization.Bush)
                {
                    newEntity = new Bush(Content, objectPosition);
                }
                else if(randomization == Randomization.BigRock)
                {
                    newEntity = new BigRock(Content, objectPosition);
                }
                else if(randomization == Randomization.WoodLog)
                {
                    newEntity = new BigLog(Content, objectPosition);
                }

                bool intersects = false;

                var entities = _entityManager.Entities.Where(e => e is Interactable).Cast<Interactable>().ToArray();

                foreach (var entity in entities)
                {
                    if (entity.BoundingRectangle.Intersects(newEntity.BoundingRectangle))
                    {
                        intersects = true;
                        break;
                    }
                }

                if (intersects)
                {
                    i--;
                }
                else
                {
                    _entityManager.AddEntity(newEntity);
                }
            }
        }

        public override void LoadContent()
        {
            base.LoadContent();

            if (MediaPlayer.State == MediaState.Stopped)
            {
                var song = Content.Load<Song>("audio/music/spring");

                MediaPlayer.Play(song);
                MediaPlayer.IsRepeating = true;
            }


            // Load the compiled map
            _map = Content.Load<TiledMap>("maps/Ranch");
            // Create the map renderer
            _mapRenderer = new TiledMapRenderer(GraphicsDevice, _map);

            _dayTime = HarvestMoon.Instance.RanchDayTime;

            var ranchState = _entityManager as RanchState;
            if(!ranchState.IsLoaded || HarvestMoon.Instance.HasNotSeenTheRanch)
            {

                if (HarvestMoon.Instance.HasNotSeenTheRanch)
                {
                    Grid grid = null;
                    foreach (var layer in _map.ObjectLayers)
                    {
                        if (layer.Name == "Plot")
                        {
                            foreach (var obj in layer.Objects)
                            {
                                var objectPosition = obj.Position;

                                objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                                objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                                grid = new Grid(objectPosition, obj.Size);
                            }
                        }
                    }

                    Randomize(grid, 30, new Size2(32, 32), Randomization.SmallRock);
                    Randomize(grid, 30, new Size2(32, 32), Randomization.Bush);
                    Randomize(grid, 15, new Size2(64, 64), Randomization.BigRock);
                    Randomize(grid, 15, new Size2(64, 64), Randomization.WoodLog);

                }


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
                
                HarvestMoon.Instance.HasNotSeenTheRanch = false;

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

            var doors = _entityManager.Entities.Where(e => e is Door).Cast<Door>().ToArray();
            var walls = _entityManager.Entities.Where(e => e is Wall).Cast<Wall>().ToArray();
            var grids = _entityManager.Entities.Where(e => e is Grid).Cast<Grid>().ToArray();
            var npcs = _entityManager.Entities.Where(e => e is NPC).Cast<NPC>().ToArray();

            foreach (var door in doors)
            {
                _entityManager.Entities.Remove(door);
            }

            foreach (var wall in walls)
            {
                _entityManager.Entities.Remove(wall);
            }

            foreach (var grid in grids)
            {
                _entityManager.Entities.Remove(grid);
            }

            foreach (var npc in npcs)
            {
                _entityManager.Entities.Remove(npc);
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
                                    this._mediaShouldFadeToTown = true;
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

                        if(obj.Type == "enemy")
                        {
                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                            objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                            var objectSize = obj.Size;

                            _entityManager.AddEntity(new Mouse(Content, objectPosition, objectSize));

                        }

                        if (obj.Type == "special")
                        {
                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X + obj.Size.Width * 0.5f;
                            objectPosition.Y = obj.Position.Y + obj.Size.Height * 0.5f;

                            if (obj.Name == "shipping-box")
                            {
                                _entityManager.AddEntity(new ShippingBox(Content, objectPosition));
                            }
                            else if(obj.Name == "leader-a")
                            {
                                _leaderAPosition = objectPosition;
                            }
                            else if(obj.Name == "leader-b")
                            {
                                _leaderBPosition = objectPosition;
                            }
                            else if(obj.Name == "leader-c")
                            {
                                _leaderCPosition = objectPosition;
                            }
                            else if(obj.Name == "clark-a")
                            {
                                _clarkAPosition = objectPosition;
                            }
                            else if(obj.Name == "clark-b")
                            {
                                _clarkBPosition = objectPosition;
                            }
                            else if(obj.Name == "clark-c")
                            {
                                _clarkCPosition = objectPosition;
                            }
                        }
                    }

                }
            }

            LoadPlayer();
            _player.EntityManager = _entityManager;
        }

        private Vector2 _leaderAPosition;
        private Vector2 _leaderBPosition;
        private Vector2 _leaderCPosition;

        private Vector2 _clarkAPosition;
        private Vector2 _clarkBPosition;
        private Vector2 _clarkCPosition;

        private Leader _leader;
        private Clark _clark;

        private float _dayTime;

        private enum ClarkStatus
        {
            Entry,
            Exit
        }

        private ClarkStatus _clarkStatus;

        private int _clarkWaitTicks;

        private void ClarkRoutine()
        {
            if (_clark != null)
            {
                if (_clarkStatus == ClarkStatus.Entry)
                {
                    if (_clark.X == _clarkAPosition.X)
                    {
                        _clark.Y++;
                    }

                    if (_clark.Y == _clarkBPosition.Y)
                    {
                        _clark.PlayAnimation("walk_left");
                        _clark.X--;
                    }

                    if (_clark.X == _clarkCPosition.X)
                    {
                        _clark.PlayAnimation("walk_down");

                        _clarkStatus = ClarkStatus.Exit;
                    }

                }
                else
                {
                    if (_clarkWaitTicks >= 180)
                    {

                        if (_clark.X < _clarkBPosition.X)
                        {
                            _clark.PlayAnimation("walk_right");
                            _clark.X++;
                        }

                        if (_clark.X == _clarkBPosition.X)
                        {
                            _clark.PlayAnimation("walk_up");
                            _clark.Y--;
                        }

                        if (_clark.Y == _clarkAPosition.Y)
                        {
                            _clark.Destroy();
                            _clark = null;
                            _clarkWaitTicks = 0;
                        }

                    }
                    else if (_clarkWaitTicks == 0)
                    {

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

                    _clarkWaitTicks++;

                }
            }

        }

        private enum LeaderStatus
        {
            Onboarding,
            Idle,
            Exit
        }

        private LeaderStatus _leaderStatus;

        private void LeaderRoutine()
        {
            if (_leader != null)
            {

                if (_leaderStatus == LeaderStatus.Onboarding)
                {
                    Action onboardingStart = () => { _player.Freeze(); _player.Busy(); };
                    Action onboardingEnd = () => { _leaderStatus = LeaderStatus.Exit; _player.UnFreeze(); _player.Cooldown(); };

                    Action onboardingF = () =>
                    {
                        HarvestMoon.Instance.GUI.ShowMessage("I think we're done, please have a nice stay and enjoy everything Honey Town has for you!",
                                                             onboardingStart,
                                                             onboardingEnd);
                    };

                    Action onboardingE = () =>
                    {
                        HarvestMoon.Instance.GUI.ShowMessage("If you need to buy seeds you can do so at the town shop, if you want to buy cattle you must talk to the livestock dealer, also in the town.",
                                                             onboardingStart,
                                                             onboardingF);
                    };

                    Action onboardingD = () =>
                    {
                        HarvestMoon.Instance.GUI.ShowMessage("Talking about the harvest goods, there is a shipping box, you should ship all your harvest before night as Clark will come up and pick them, paying you the next day.",
                                                             onboardingStart,
                                                             onboardingE);
                    };


                    Action onboardingC = () =>
                    {
                        HarvestMoon.Instance.GUI.ShowMessage("There are also a few ladies, maybe you fall in love, who knows.",
                                                             onboardingStart,
                                                             onboardingD);
                    };


                    Action onboardingB = () =>
                    {
                        HarvestMoon.Instance.GUI.ShowMessage("While we're at this, it is worth mentioning that you can go to the town, located north from here and meet the villagers, befriend them.",
                                                             onboardingStart,
                                                             onboardingC);
                    };


                    Action onboardingA = () =>
                    {
                        HarvestMoon.Instance.GUI.ShowMessage("I'm the beekeeping guild leader and I'm pleased to welcome you, this is something we celebrate as previous owners of the farm decided to move out.",
                                                             onboardingStart,
                                                             onboardingB);
                    };


                    HarvestMoon.Instance.GUI.ShowMessage("Welcome to your new farm.",
                                                         onboardingStart,
                                                         onboardingA);

                    _leaderStatus = LeaderStatus.Idle;

                }
                else if (_leaderStatus == LeaderStatus.Exit)
                {
                    if (_leader.X < _leaderBPosition.X)
                    {
                        _leader.X++;
                        _leader.PlayAnimation("walk_right");
                    }

                    if (_leader.X == _leaderBPosition.X)
                    {
                        _leader.Y--;
                        _leader.PlayAnimation("walk_up");
                    }

                    if (_leader.Y == _leaderCPosition.Y)
                    {
                        _leader.Destroy();
                        _leader = null;
                        _leaderStatus = LeaderStatus.Onboarding;
                    }
                }
            }

        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_mediaShouldFadeToTown)
            {
                if(MediaPlayer.Volume > 0.0f && MediaPlayer.State == MediaState.Playing)
                {
                    MediaPlayer.Volume -= 0.1f;
                }

                if(MediaPlayer.Volume <= 0.0f)
                {
                    MediaPlayer.Stop();
                    MediaPlayer.Volume = 1.0f;

                    var screen = new Town(Game, HarvestMoon.Arrival.Ranch);
                    var transition = new FadeTransition(GraphicsDevice, Color.Black, 1.0f);
                    ScreenManager.LoadScreen(screen, transition);
                }

            }

            ClarkRoutine();

            LeaderRoutine();

            if (!HarvestMoon.Instance.HasTriggeredCutscene("onboarding"))
            {
                _player.Freeze();

                _leader = _entityManager.AddEntity(new Leader(Content, _leaderAPosition, new Size2(32, 45)));

                HarvestMoon.Instance.SetCutsceneTriggered("onboarding", true);

                _leaderStatus = LeaderStatus.Onboarding;
            }

            var dayTimeEffect = HarvestMoon.Instance.DayTimeEffect;

            var currentDayTimeColor = dayTimeEffect.DiffuseColor;

            if (currentDayTimeColor == Color.DarkGray)
            {
                if (HarvestMoon.Instance.GetDayTime() == HarvestMoon.DayTime.Afternoon)
                {
                    if (!HarvestMoon.Instance.HasNightTriggered())
                    {
                        HarvestMoon.Instance.SetNightTriggered(true);

                        _clark = _entityManager.AddEntity(new Clark(Content, _clarkAPosition, new Size2(32, 45)));
                        _clarkStatus = ClarkStatus.Entry;
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
            var soilPieces = _entityManager.Entities.Where(e => e is Soil).Cast<Soil>().ToList();

            soilPieces = soilPieces.OrderBy(s => s.Y).ToList();

            for (var i = 0; i < soilPieces.Count; ++i)
            {
                var soilPiece = soilPieces[i];
                soilPiece.Draw(_spriteBatch);
            }

            var otherObjects = _entityManager.Entities.Where(e => { return !(e is Soil); }).Cast<Entity>().ToList();

            otherObjects = otherObjects.OrderBy(o => o.Priority).ToList();

            for (var i = 0; i < otherObjects.Count; ++i)
            {
                var otherObject = otherObjects[i];
                otherObject.Draw(_spriteBatch);
            }

            //_player.Draw(_spriteBatch);

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
