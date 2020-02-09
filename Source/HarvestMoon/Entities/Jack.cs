using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.ViewportAdapters;
using System.Collections.Generic;
using System.Linq;

using MonoGame.Extended.Content;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Collisions;
using System;
using HarvestMoon.Entities.Ranch;
using HarvestMoon.Screens;
using HarvestMoon.Entities.General;
using HarvestMoon.Input;
using HarvestMoon.Animation;
using HarvestMoon.Entities.Items;
using HarvestMoon.Tweening;

namespace HarvestMoon.Entities
{
    public class Jack : Entity
    {
        public enum Facing
        {
            UP,
            DOWN,
            LEFT,
            RIGHT
        }

        private AnimatedSprite _sprite;
        private readonly Transform2 _transform;

        private readonly AnimatedSprite _heroSprite;
        private readonly AnimatedSprite _heroPackSprite;
        private readonly AnimatedSprite _heroHoldSprite;

        private readonly AnimatedSprite _heroCarrySprite;
        private readonly AnimatedSprite _heroRunSpriteA;

        private readonly Dictionary<string, AnimatedSprite> _toolingSprites = new Dictionary<string, AnimatedSprite>();

        private const float _defaultWalkSpeed = 100.0f;
        private const float _defaultRunSpeed = 180.0f;

        private int _hammerPower;
        private int _axePower;

        private Facing _playerFacing = Facing.RIGHT;

        public Facing PlayerFacing { get => _playerFacing; set => _playerFacing = value; }

        public RectangleF BoundingRectangle;

        private Vector2 _carryingPosition = Vector2.Zero;

        private Vector2 _packingPosition = Vector2.Zero;

        public Vector2 Position
        {
            get => _transform.Position;
            set
            {
                _transform.Position = value;
                BoundingRectangle.Position = new Vector2(value.X - BoundingRectangle.Size.Width * 0.5f,
                                                         value.Y);

                _carryingPosition = new Vector2(value.X, BoundingRectangle.Top - 32);
                _packingPosition = new Vector2(value.X, BoundingRectangle.Top - 8);
                SetupActionBoundingRectangle(BoundingRectangle.TopLeft);
            }
        }

        public Vector2 Velocity { get; set; }

        public RectangleF ActionBoundingRectangle = RectangleF.Empty;

        private Interactable _carryingObject = null;
        private Interactable _currentInteractable = null;
        private readonly Tweener _tweener = new Tweener();
        private bool _isCarrying = false;
        private bool _isPacking = false;
        private bool _isFrozen = false;

        private bool _isTooling = false;
        private bool _isHolding = false;
        private bool _isRunning = false;

        public bool IsCarrying => _isCarrying;
        public Interactable CarryingObject => _carryingObject;

        private readonly ContentManager _contentManager;
        private EntityManager _entityManager;

        public EntityManager EntityManager { get => _entityManager; set => _entityManager = value; }

        private Vector2 _actionBoundingRectangleOffset = new Vector2(16, 20);

        private Action<string> pack;

        private Dictionary<string, Sprite> _holdingItemSprites = new Dictionary<string, Sprite>();


        private bool _busy;
        private bool _npcCoolDown;

        private float _packTimer = 0;
        private float _toolTimer = 0;
        private float _holdTimer = 0;
        private float _coolingTimer = 0;

        private float _itemTimerDelay = 1.0f;
        private float _packingTimerDelay = 0.375f;
        private float _toolingTimerDelay = 0.5f;
        private float _npcCoolingDelay = 1.0f;


        private Map _mapScreen;

        public Map MapScreen { get => _mapScreen; set => _mapScreen = value; }

        public void OnPack(Action<string> callback)
        {
            pack = callback;
        }

        public Jack(ContentManager content, EntityManager entityManager, Map mapScreen, Vector2 initialPosition)
        {
            _contentManager = content;
            _entityManager = entityManager;
            _mapScreen = mapScreen;

            Priority = 256;

            float frameDuration = 1.0f / 7.5f;

            _heroSprite = AnimationLoader.LoadAnimatedSprite(content, 
                                                             "animations/hero", 
                                                             "animations/heroMap", 
                                                             "hero", 
                                                             frameDuration,
                                                             true);

            _sprite = _heroSprite;

            _heroPackSprite = AnimationLoader.LoadAnimatedSprite(content,
                                                                 "animations/heroPack",
                                                                 "animations/heroPackMap",
                                                                 "heroPack",
                                                                 frameDuration,
                                                                 false);

            _heroHoldSprite = AnimationLoader.LoadAnimatedSprite(content,
                                                                 "animations/heroHold",
                                                                 "animations/heroHoldMap",
                                                                 "heroHold",
                                                                 frameDuration,
                                                                 false);

            _heroCarrySprite = AnimationLoader.LoadAnimatedSprite(content,
                                                                 "animations/heroCarry",
                                                                 "animations/heroCarryMap",
                                                                 "heroCarry",
                                                                 frameDuration,
                                                                 true);


            _heroRunSpriteA = AnimationLoader.LoadAnimatedSprite(content,
                                                                 "animations/heroRunA",
                                                                 "animations/heroRunMapA",
                                                                 "heroRunA",
                                                                 frameDuration,
                                                                 true);

            var heroAxeAnimation = AnimationLoader.LoadAnimatedSprite(content,
                                                                      "animations/heroAxe",
                                                                      "animations/heroAxeMap",
                                                                      "heroAxe",
                                                                      frameDuration,
                                                                      false);


            var heroHammerAnimation = AnimationLoader.LoadAnimatedSprite(content,
                                                                      "animations/heroHammer",
                                                                      "animations/heroHammerMap",
                                                                      "heroHammer",
                                                                      frameDuration,
                                                                      false);


            var heroHoeAnimation = AnimationLoader.LoadAnimatedSprite(content,
                                                                      "animations/heroHoe",
                                                                      "animations/heroHoeMap",
                                                                      "heroHoe",
                                                                      frameDuration,
                                                                      false);

            var heroSickleAnimation = AnimationLoader.LoadAnimatedSprite(content,
                                                                      "animations/heroSickle",
                                                                      "animations/heroSickleMap",
                                                                      "heroSickle",
                                                                      frameDuration,
                                                                      false);


            var heroSeedsAnimation = AnimationLoader.LoadAnimatedSprite(content,
                                                                      "animations/heroSeeds",
                                                                      "animations/heroSeedsMap",
                                                                      "heroSeeds",
                                                                      frameDuration,
                                                                      false);



            var heroWateringCanAnimation = AnimationLoader.LoadAnimatedSprite(content,
                                                                      "animations/heroMisc",
                                                                      "animations/heroWateringCanMap",
                                                                      "heroSeeds",
                                                                      frameDuration,
                                                                      false);

            var heroMilkerAnimation = AnimationLoader.LoadAnimatedSprite(content,
                                                                      "animations/hero",
                                                                      "animations/heroMilkerMap",
                                                                      "heroMilker",
                                                                      frameDuration,
                                                                      false);

            var heroSwordAnimation = AnimationLoader.LoadAnimatedSprite(content,
                                                                      "animations/hero",
                                                                      "animations/heroSwordMap",
                                                                      "heroSword",
                                                                      frameDuration,
                                                                      false);
            _toolingSprites.Add("axe", heroAxeAnimation);
            _toolingSprites.Add("hammer", heroHammerAnimation);
            _toolingSprites.Add("hoe", heroHoeAnimation);
            _toolingSprites.Add("sickle", heroSickleAnimation);
            _toolingSprites.Add("seeds", heroSeedsAnimation);
            _toolingSprites.Add("watering-can", heroWateringCanAnimation);
            _toolingSprites.Add("milker", heroMilkerAnimation);
            _toolingSprites.Add("sword", heroSwordAnimation);

            _transform = new Transform2
            {
                Scale = Vector2.One,
                Position = initialPosition
            };

            BoundingRectangle = new RectangleF(new Vector2(_transform.Position.X - 8, 
                                                            _transform.Position.Y - 12), 
                                               new Size2(16, 24));

            _carryingPosition = new Vector2(initialPosition.X, BoundingRectangle.Top - 16);

            SetupActionBoundingRectangle(BoundingRectangle.TopLeft);

            _holdingItemSprites.Add("axe", new Sprite(content.Load<Texture2D>("maps/tools-room/items/axe")));
            _holdingItemSprites.Add("grass-seeds", new Sprite(content.Load<Texture2D>("maps/tools-room/items/grass-seeds")));
            _holdingItemSprites.Add("turnip-seeds", new Sprite(content.Load<Texture2D>("maps/tools-room/items/turnip-seeds")));
            _holdingItemSprites.Add("potato-seeds", new Sprite(content.Load<Texture2D>("maps/tools-room/items/potato-seeds")));
            _holdingItemSprites.Add("tomato-seeds", new Sprite(content.Load<Texture2D>("maps/tools-room/items/tomato-seeds")));
            _holdingItemSprites.Add("corn-seeds", new Sprite(content.Load<Texture2D>("maps/tools-room/items/corn-seeds")));
            _holdingItemSprites.Add("hammer", new Sprite(content.Load<Texture2D>("maps/tools-room/items/hammer")));
            _holdingItemSprites.Add("hoe", new Sprite(content.Load<Texture2D>("maps/tools-room/items/hoe")));
            _holdingItemSprites.Add("sickle", new Sprite(content.Load<Texture2D>("maps/tools-room/items/sickle")));
            _holdingItemSprites.Add("watering-can", new Sprite(content.Load<Texture2D>("maps/tools-room/items/watering-can")));
            _holdingItemSprites.Add("milker", new Sprite(content.Load<Texture2D>("maps/tools-room/items/milker")));
            _holdingItemSprites.Add("sword", new Sprite(content.Load<Texture2D>("maps/tools-room/items/sword")));
        }

        public void SetupActionBoundingRectangle(Vector2 withPosition)
        {
            _actionBoundingRectangleOffset = new Vector2(14, 20);

            ActionBoundingRectangle = BoundingRectangle;

            switch (_playerFacing)
            {
                case Facing.UP:
                    ActionBoundingRectangle.X = withPosition.X;
                    ActionBoundingRectangle.Y = withPosition.Y - ActionBoundingRectangle.Height - _actionBoundingRectangleOffset.Y;
                    break;

                case Facing.DOWN:
                    ActionBoundingRectangle.X = withPosition.X;
                    ActionBoundingRectangle.Y = withPosition.Y + ActionBoundingRectangle.Height + _actionBoundingRectangleOffset.Y;
                    break;

                case Facing.LEFT:
                    ActionBoundingRectangle.X = withPosition.X - ActionBoundingRectangle.Width - _actionBoundingRectangleOffset.X;
                    ActionBoundingRectangle.Y = withPosition.Y;
                    break;

                case Facing.RIGHT:
                    ActionBoundingRectangle.X = withPosition.X + ActionBoundingRectangle.Width + _actionBoundingRectangleOffset.X;
                    ActionBoundingRectangle.Y = withPosition.Y;
                    break;
            }
        }

        private bool _isInteractButtonDown = false;
        private bool _isToolButtonDown = false;
        private bool _isHoldButtonDown = false;

        public void Interact(Interactable interactable)
        {
            if(interactable == null)
            {
                _currentInteractable = null;
                return;
            }

            if (interactable.Breakable)
            {
               // _breakableObject = interactable;
            }

            _currentInteractable = interactable;

        }

        public void CheckInteractions()
        {
            var keyboardState = HarvestMoon.Instance.Input;

            if (_carryingObject == null &&
                keyboardState.IsKeyDown(InputDevice.Keys.A) &&
                !_isInteractButtonDown &&
                (_currentInteractable.Carryable || _currentInteractable.Packable) &&
                _currentInteractable.Interacts)
            {
                _isInteractButtonDown = true;


                _carryingObject = _currentInteractable;

                var carryingParameters = new Vector2{ X = _carryingPosition.X, Y = _carryingPosition.Y };

                if (_currentInteractable.Packable)
                {
                    carryingParameters = new Vector2{ X = _packingPosition.X, Y = _packingPosition.Y };
                }

                _tweener.Tween(_currentInteractable, carryingParameters, 0.125f)
                        .OnBegin(() =>
                        {
                            Freeze();
                            _carryingObject.Planked = false;
                            _carryingObject.Priority = Priority;
                            _isCarrying = true;
                            _isPacking = true;
                        });


            }
            else if (keyboardState.IsKeyDown(InputDevice.Keys.A) && !_isInteractButtonDown && _currentInteractable.IsNPC && _currentInteractable.Interacts && !_busy)
            {
                _busy = true;
                _isInteractButtonDown = true;

                var npc = _currentInteractable as NPC;

                if (_carryingObject is Item)
                {
                    npc.Interact(_carryingObject as Item, () => { Freeze(); Busy(); }, () => { UnFreeze(); Cooldown(); });
                }
                else
                {
                    npc.Interact(null, () => { Freeze(); Busy(); }, () => { UnFreeze(); Cooldown(); });
                }

                if (_carryingObject != null)
                {
                    if (_carryingObject.IsDestroyed)
                    {
                        _carryingObject.Priority = 0;
                        _isCarrying = false;
                        _carryingObject = null;
                    }
                }
            }
            else if (keyboardState.IsKeyDown(InputDevice.Keys.A) && !_isInteractButtonDown && _currentInteractable is Soil && _carryingObject == null && !_busy)
            {
                var harvest = (_currentInteractable as Soil).Harvest();

                if (harvest != null)
                {
                    _entityManager.SubmitEntity(harvest);
                    _carryingObject = harvest;

                    var carryingParameters = new Vector2{ X = _carryingPosition.X, Y = _carryingPosition.Y };

                    _tweener.Tween(harvest, carryingParameters, 0.125f)
                            .OnBegin(() =>
                            {
                                Freeze();
                                _carryingObject.Planked = false;
                                _carryingObject.Priority = Priority;
                                _isCarrying = true;
                                _isPacking = true;
                            });
                }
            }
            
        }

        public void Busy()
        {
            _busy = true;
        }

        public void Cooldown()
        {
            _npcCoolDown = true;
        }

        public void Drop()
        {
            if(_carryingObject != null)
            {
                if (_carryingObject.Shippable || _carryingObject is Bush)
                {
                    Vector3 boundPosition = new Vector3(0, 0, 0);

                    switch (_playerFacing)
                    {
                        case Facing.UP:
                            boundPosition = new Vector3(ActionBoundingRectangle.Center.X,
                                                        ActionBoundingRectangle.Center.Y,
                                                        0);
                            break;

                        case Facing.DOWN:
                            boundPosition = new Vector3(ActionBoundingRectangle.Center.X,
                                                        ActionBoundingRectangle.Top,
                                                        0);
                            break;
                        case Facing.RIGHT:
                            boundPosition = new Vector3(ActionBoundingRectangle.Right,
                                                        ActionBoundingRectangle.Center.Y,
                                                        0);
                            break;
                        case Facing.LEFT:
                            boundPosition = new Vector3(ActionBoundingRectangle.Left,
                                                        ActionBoundingRectangle.Center.Y,
                                                        0);
                            break;
                    }

                    Freeze();
                    _tweener.Tween(_carryingObject,
                           new Vector2{ X = boundPosition.X, Y = boundPosition.Y },
                           0.125f)
                               .OnComplete(() =>
                               {
                                   UnFreeze();
                                   Cooldown();
                                   _carryingObject.Planked = true;
                                   _carryingObject.Priority = 0;
                                   _isCarrying = false;

                                   _carryingObject.OnInteractableDrop();

                                   _carryingObject = null;


                               });

                }
                else
                {
                    var grids = _entityManager.Entities.Where(e => e is Grid).Cast<Grid>().ToArray();
                    foreach (var grid in grids)
                    {
                        Vector3 boundPosition = new Vector3(0, 0, 0);

                        switch (_playerFacing)
                        {
                            case Facing.UP:
                                boundPosition = new Vector3(ActionBoundingRectangle.Center.X,
                                                            ActionBoundingRectangle.Center.Y,
                                                            0);
                                break;

                            case Facing.DOWN:
                                boundPosition = new Vector3(ActionBoundingRectangle.Center.X,
                                                            ActionBoundingRectangle.Top,
                                                            0);
                                break;
                            case Facing.RIGHT:
                                boundPosition = new Vector3(ActionBoundingRectangle.Right,
                                                            ActionBoundingRectangle.Center.Y,
                                                            0);
                                break;
                            case Facing.LEFT:
                                boundPosition = new Vector3(ActionBoundingRectangle.Left,
                                                            ActionBoundingRectangle.Center.Y,
                                                            0);
                                break;
                        }

                        var cellAtPosition = grid.CollisionGridSet.GetCellAtPosition(boundPosition);
                        if (cellAtPosition.Flag == CollisionGridCellFlag.Solid)
                        {
                            var targetPosition = cellAtPosition.BoundingBox.Center + grid.CollisionGridSet.GridPosition;

                            /*System.Diagnostics.Debug.WriteLine(ActionBoundingRectangle.Center.ToString());
                            System.Diagnostics.Debug.WriteLine(cellAtPosition.BoundingBox.Center.ToString());
                            System.Diagnostics.Debug.WriteLine(grid.CollisionGridSet.GridPosition.ToString());
                            System.Diagnostics.Debug.WriteLine(targetPosition.ToString());*/

                            var interactablesButSoil = _entityManager
                                                        .Entities
                                                        .Where(e => { return e is Interactable && !(e is Soil); })
                                                        .Cast<Interactable>().ToArray();
                            bool collides = false;
                            foreach (var interactable in interactablesButSoil)
                            {
                                if (interactable.BoundingRectangle.Contains(new Point2(targetPosition.X, targetPosition.Y)))
                                {
                                    collides = true;
                                    break;
                                }
                            }

                            if (!collides)
                            {
                                if (_carryingObject is SmallRock || _carryingObject is WoodPiece)
                                {
                                    var soilSegments = _entityManager.Entities
                                                                 .Where(e => { return e is Soil; })
                                                                 .Cast<Soil>().ToArray();

                                    foreach (var soilSegment in soilSegments)
                                    {
                                        if (soilSegment.BoundingRectangle.Contains(new Point2(targetPosition.X, targetPosition.Y)))
                                        {
                                            if (soilSegment.HasGrown)
                                            {
                                                collides = true;
                                            }
                                            else
                                            {
                                                soilSegment.Destroy();
                                            }
                                        }

                                    }
                                }


                            }

                            if (!collides)
                            {
                                Freeze();
                                _tweener.Tween(_carryingObject,
                                       new Vector2 { X = targetPosition.X, Y = targetPosition.Y },
                                       0.125f)
                                           .OnComplete(() =>
                                           {
                                               UnFreeze();
                                               _carryingObject.Planked = true;
                                               _carryingObject.Priority = 0;
                                               _isCarrying = false;

                                               _carryingObject.OnInteractableDrop();

                                               _carryingObject = null;
                                           });

                            }
                            break;
                        }
                    }


                }

            }

        }

        public void Hoe()
        {
            var grids = _entityManager.Entities.Where(e => e is Grid).Cast<Grid>().ToArray();
            foreach (var grid in grids)
            {
                Vector3 boundPosition = new Vector3(0, 0, 0);

                switch (_playerFacing)
                {
                    case Facing.UP:
                        boundPosition = new Vector3(ActionBoundingRectangle.Center.X,
                                                    ActionBoundingRectangle.Bottom,
                                                    0);
                        break;

                    case Facing.DOWN:
                        boundPosition = new Vector3(ActionBoundingRectangle.Center.X,
                                                    ActionBoundingRectangle.Top,
                                                    0);
                        break;
                    case Facing.RIGHT:
                        boundPosition = new Vector3(ActionBoundingRectangle.Right,
                                                    ActionBoundingRectangle.Center.Y,
                                                    0);
                        break;
                    case Facing.LEFT:
                        boundPosition = new Vector3(ActionBoundingRectangle.Left,
                                                    ActionBoundingRectangle.Center.Y,
                                                    0);
                        break;
                }

                var cellAtPosition = grid.CollisionGridSet.GetCellAtPosition(boundPosition);
                if (cellAtPosition.Flag == CollisionGridCellFlag.Solid)
                {
                    var targetPosition = cellAtPosition.BoundingBox.Center + grid.CollisionGridSet.GridPosition;

                    var interactablesButSoil = _entityManager
                                                            .Entities
                                                            .Where(e => { return e is Interactable && !(e is Soil); })
                                                            .Cast<Interactable>().ToArray();

                    bool collides = false;
                    foreach (var interactable in interactablesButSoil)
                    {
                        if (interactable.BoundingRectangle.Contains(new Point2(targetPosition.X, targetPosition.Y)))
                        {
                            collides = true;
                            break;
                        }
                    }


                    var soilPieces = _entityManager.Entities.Where(e => e is Soil).Cast<Soil>().ToArray();

                    foreach (var soilPiece in soilPieces)
                    {
                        Vector2 soilPosition = new Vector2(soilPiece.X, soilPiece.Y);
                        Vector2 targetPositionVector = new Vector2(targetPosition.X, targetPosition.Y);

                        if (soilPosition == targetPositionVector && (!soilPiece.HasGrown || soilPiece.CropType == "grass"))
                        {
                            soilPiece.Destroy();
                            break;
                        }
                    }

                    foreach (var soilPiece in soilPieces)
                    {
                        Vector2 soilPosition = new Vector2(soilPiece.X, soilPiece.Y);
                        Vector2 targetPositionVector = new Vector2(targetPosition.X, targetPosition.Y);

                        if (soilPosition == targetPositionVector && soilPiece.HasGrown)
                        {
                            collides = true;
                            break;
                        }
                    }

                    if (!collides)
                    {
                        _entityManager.SubmitEntity(new Soil(_contentManager,
                                                      new Vector2(targetPosition.X, targetPosition.Y)));
                    }
                    

                    break;
                }
            }
        }

        public void Water()
        {
            var grids = _entityManager.Entities.Where(e => e is Grid).Cast<Grid>().ToArray();
            foreach (var grid in grids)
            {
                Vector3 boundPosition = new Vector3(0, 0, 0);

                switch (_playerFacing)
                {
                    case Facing.UP:
                        boundPosition = new Vector3(ActionBoundingRectangle.Center.X,
                                                    ActionBoundingRectangle.Bottom,
                                                    0);
                        break;

                    case Facing.DOWN:
                        boundPosition = new Vector3(ActionBoundingRectangle.Center.X,
                                                    ActionBoundingRectangle.Top,
                                                    0);
                        break;
                    case Facing.RIGHT:
                        boundPosition = new Vector3(ActionBoundingRectangle.Right,
                                                    ActionBoundingRectangle.Center.Y,
                                                    0);
                        break;
                    case Facing.LEFT:
                        boundPosition = new Vector3(ActionBoundingRectangle.Left,
                                                    ActionBoundingRectangle.Center.Y,
                                                    0);
                        break;
                }

                var cellAtPosition = grid.CollisionGridSet.GetCellAtPosition(boundPosition);
                if (cellAtPosition.Flag == CollisionGridCellFlag.Solid)
                {
                    var targetPosition = cellAtPosition.BoundingBox.Center + grid.CollisionGridSet.GridPosition;
                    var soilPieces = _entityManager.Entities.Where(e => e is Soil).Cast<Soil>().ToArray();

                    foreach (var soilPiece in soilPieces)
                    {
                        Vector2 soilPosition = new Vector2(soilPiece.X, soilPiece.Y);
                        Vector2 targetPositionVector = new Vector2(targetPosition.X, targetPosition.Y);

                        if (soilPosition == targetPositionVector)
                        {
                            soilPiece.Water();
                            break;
                        }
                    }
                }
            }
        }

        public void Sickle()
        {
            if(_currentInteractable != null)
            {
                if (_currentInteractable.Cuttable)
                {
                    _currentInteractable.OnCut();
                }
            }
        }

        public void Hammer()
        {
            if (_currentInteractable != null)
            {
                if (_currentInteractable.Hammerable)
                {
                    _hammerPower++;
                    _currentInteractable.OnHammer(_hammerPower);
                }
            }
        }

        public void Axe()
        {
            if (_currentInteractable != null)
            {
                if (_currentInteractable.Splittable)
                {
                    _axePower++;
                    _currentInteractable.OnAxe(_axePower);
                }
            }
        }

        public void Sword()
        {
            if (_currentInteractable != null)
            {
                if (_currentInteractable is Ranch.Mouse)
                {
                    var mouse = _currentInteractable as Ranch.Mouse;
                    var intersection = mouse.BoundingRectangle.Intersection(ActionBoundingRectangle);

                    var normal = new Vector2(Position.X - intersection.Center.X, Position.Y - intersection.Center.Y);
                    normal.Normalize();
                    mouse.TakeDamage(1, normal);


                }
            }
        }


        public void Milk()
        {
            if(_currentInteractable != null)
            {
                if(_currentInteractable is Cow)
                {
                    var cow = _currentInteractable as Cow;

                    if (!HarvestMoon.Instance.MilkedList[cow.Index])
                    {
                        HarvestMoon.Instance.MilkedList[cow.Index] = true;
                        var harvest = new Milk(_contentManager, (_currentInteractable as Cow).BoundingRectangle.Center);

                        if (harvest != null)
                        {
                            _entityManager.SubmitEntity(harvest);
                            _carryingObject = harvest;

                            var carryingParameters = new Vector2{ X = _carryingPosition.X, Y = _carryingPosition.Y };

                            _tweener.Tween(harvest, carryingParameters, 0.125f)
                                    .OnBegin(() =>
                                    {
                                        Freeze();
                                        _carryingObject.Planked = false;
                                        _carryingObject.Priority = Priority;
                                        _isCarrying = true;
                                        _isPacking = true;
                                    });
                        }
                    }

                }
            }
        }

        public void Plant()
        {
            var grids = _entityManager.Entities.Where(e => e is Grid).Cast<Grid>().ToArray();
            foreach (var grid in grids)
            {
                List<Vector3> boundPositions = new List<Vector3>();

                boundPositions.Add(new Vector3(Position.X, Position.Y, 0));
                boundPositions.Add(new Vector3(Position.X + 32, Position.Y, 0));
                boundPositions.Add(new Vector3(Position.X - 32, Position.Y, 0));
                boundPositions.Add(new Vector3(Position.X, Position.Y - 32, 0));
                boundPositions.Add(new Vector3(Position.X + 32, Position.Y -32, 0));
                boundPositions.Add(new Vector3(Position.X - 32, Position.Y -32, 0));
                boundPositions.Add(new Vector3(Position.X, Position.Y + 32, 0));
                boundPositions.Add(new Vector3(Position.X - 32, Position.Y + 32, 0));
                boundPositions.Add(new Vector3(Position.X + 32, Position.Y + 32, 0));
                foreach(var boundPosition in boundPositions)
                {
                    var cellAtPosition = grid.CollisionGridSet.GetCellAtPosition(boundPosition);
                    if (cellAtPosition.Flag == CollisionGridCellFlag.Solid)
                    {
                        var targetPosition = cellAtPosition.BoundingBox.Center + grid.CollisionGridSet.GridPosition;
                        var soilPieces = _entityManager.Entities.Where(e => e is Soil).Cast<Soil>().ToArray();

                        foreach (var soilPiece in soilPieces)
                        {
                            Vector2 soilPosition = new Vector2(soilPiece.X, soilPiece.Y);
                            Vector2 targetPositionVector = new Vector2(targetPosition.X, targetPosition.Y);

                            if (soilPosition == targetPositionVector && !soilPiece.IsPlanted)
                            {
                                var currentTool = HarvestMoon.Instance.GetCurrentTool();
                                if (currentTool.Contains("grass"))
                                {
                                    soilPiece.Plant("grass", 0, 0);
                                }

                                if (currentTool.Contains("turnip"))
                                {
                                    soilPiece.Plant("turnip", 0, 0);
                                }

                                if (currentTool.Contains("potato"))
                                {
                                    soilPiece.Plant("potato", 0, 0);
                                }

                                if (currentTool.Contains("tomato"))
                                {
                                    soilPiece.Plant("tomato", 0, 0);
                                }

                                if (currentTool.Contains("corn"))
                                {
                                    soilPiece.Plant("corn", 0, 0);
                                }

                                if (soilPiece.IsWatered)
                                {
                                    soilPiece.Water();
                                }
                                break;
                            }
                        }
                    }
                }
                
            }
        }

        public void Freeze()
        {
            _isFrozen = true;
        }

        public void UnFreeze()
        {
            _isFrozen = false;
        }

        public override void Update(GameTime gameTime)
        {
            var deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _sprite.Update(deltaSeconds);

            _tweener.Update(deltaSeconds);

            if (_npcCoolDown)
            {
                _coolingTimer += deltaSeconds;

                if(_coolingTimer >= _npcCoolingDelay)
                {
                    _coolingTimer = 0.0f;
                    _npcCoolDown = false;
                    _busy = false;
                }
            }

            string currentTool = HarvestMoon.Instance.GetCurrentTool();

            if (_isPacking && _carryingObject != null)
            {
                _packTimer += deltaSeconds;

                if(_packTimer >= _itemTimerDelay && _carryingObject is Tool)
                {
                    _packTimer = 0.0f;
                    _isPacking = false;
                    _isCarrying = false;

                    pack((_carryingObject as Tool).Name);

                    _carryingObject = null;

                    UnFreeze();

                } else if(_packTimer >= _packingTimerDelay * 0.5 && !(_carryingObject is Tool))
                {
                    _packTimer = 0.0f;
                    _isPacking = false;
                    UnFreeze();
                }
            }

            if (_isTooling)
            {
                _toolTimer += deltaSeconds;

                float totalWaitTime = _toolingTimerDelay;

                if(currentTool != default(string))
                {
                    if (currentTool.Contains("seeds"))
                    {
                        switch (_playerFacing)
                        {
                            case Facing.DOWN:
                                totalWaitTime *= 3;
                                break;

                            default:
                                totalWaitTime *= 3;
                                break;
                        }
                        
                    }
                }

                if (_toolTimer >= totalWaitTime)
                {
                    _toolTimer = 0.0f;
                    _isTooling = false;

                    if(_sprite.CurrentAnimation != null)
                    {
                        _sprite.CurrentAnimation.Rewind();
                        _sprite.Update(gameTime);
                    }

                    if (currentTool == "hoe")
                    {
                        Hoe();
                    }
                    else if(currentTool == "watering-can")
                    {
                        Water();
                    }
                    else if(currentTool == "sickle")
                    {
                        Sickle();
                    }
                    else if(currentTool == "hammer")
                    {
                        Hammer();
                    }
                    else if (currentTool == "axe")
                    {
                        Axe();
                    }
                    else if(currentTool == "milker")
                    {
                        Milk();
                    }

                    if (currentTool.Contains("seeds"))
                    {
                        Plant();
                    }
                    
                    //if(_breakableObject != null)
                    {
                        //try_break(_breakableObject, _breakingPower);
                    }

                    UnFreeze();
                    Cooldown();
                }
            }

            if (_isHolding)
            {
                _holdTimer += deltaSeconds;

                float totalWaitTime = _itemTimerDelay;

                if (_holdTimer >= totalWaitTime)
                {
                    _holdTimer = 0.0f;
                    _isHolding = false;
                    if(_sprite.CurrentAnimation != null)
                    {
                        _sprite.CurrentAnimation.Rewind();
                        _sprite.Update(gameTime);
                    }

                    UnFreeze();
                }
            }

            var keyboardState = HarvestMoon.Instance.Input;

            float movementSpeed = _defaultWalkSpeed;
            bool movementHit = false;
            bool isRunning = false;

            if (keyboardState.IsKeyUp(InputDevice.Keys.A))
            {
                _isInteractButtonDown = false;
            }

            if (keyboardState.IsKeyUp(InputDevice.Keys.B))
            {
                _isToolButtonDown = false;
            }

            if (keyboardState.IsKeyUp(InputDevice.Keys.Y))
            {
                _isHoldButtonDown = false;
            }


            if (keyboardState.IsKeyDown(InputDevice.Keys.B) && !_isToolButtonDown && !_isCarrying && !_isFrozen && !_busy)
            {
                _isToolButtonDown = true;

                if(currentTool != default(string))
                {
                    int lessStamina = 0;

                    if (!currentTool.Contains("seeds"))
                    {
                        lessStamina = 2;
                    }
                    else
                    {
                        lessStamina = 1;
                    }

                    if (HarvestMoon.Instance.Stamina - lessStamina >= 0)
                    {
                        HarvestMoon.Instance.Stamina -= lessStamina;
                        _isTooling = true;
                        Freeze();

                        if(currentTool == "sword")
                        {
                            Sword(); // sword is immediate.
                        }
                    }
                    else
                    {
                        HarvestMoon.Instance.Stamina = 0;
                    }

                }

                //_breakPower++;
            }

            if (keyboardState.IsKeyDown(InputDevice.Keys.Y) && !_isHoldButtonDown && !_isCarrying && !_isFrozen)
            {
                _isHoldButtonDown = true;

                var swappedTool = HarvestMoon.Instance.SwapTools();

                if (swappedTool != default(string))
                {
                    currentTool = swappedTool;
                    _isHolding = true;
                    Freeze();
                }
                else if(currentTool != default(string))
                {
                    _isHolding = true;
                    Freeze();
                }

                //_breakPower++;
            }


            if (!_isFrozen)
            {
                if (keyboardState.IsKeyDown(InputDevice.Keys.X))
                {
                    isRunning = true;
                    movementHit = true;
                }

                if (keyboardState.IsKeyDown(InputDevice.Keys.Up))
                {
                    _playerFacing = Facing.UP;
                    movementHit = true;
                }

                if (keyboardState.IsKeyDown(InputDevice.Keys.Down))
                {
                    _playerFacing = Facing.DOWN;
                    movementHit = true;
                }

                if (keyboardState.IsKeyDown(InputDevice.Keys.Left))
                {
                    _playerFacing = Facing.LEFT;
                    movementHit = true;
                }

                if (keyboardState.IsKeyDown(InputDevice.Keys.Right))
                {
                    _playerFacing = Facing.RIGHT;
                    movementHit = true;
                }

                if (movementHit)
                {
                    _hammerPower = 0;
                    _axePower = 0;

                    switch (_playerFacing)
                    {
                        case Facing.UP:
                        case Facing.LEFT:
                            if (isRunning)
                            {
                                movementSpeed = -_defaultRunSpeed;
                            }
                            else
                            {
                                movementSpeed = -_defaultWalkSpeed;
                            }
                            break;

                        case Facing.DOWN:
                        case Facing.RIGHT:
                            if (isRunning)
                            {
                                movementSpeed = _defaultRunSpeed;
                            }
                            else
                            {
                                movementSpeed = _defaultWalkSpeed;
                            }
                            break;
                    }

                }

                if (movementHit)
                {
                    switch (_playerFacing)
                    {
                        case Facing.UP:
                            Velocity = new Vector2(0, movementSpeed);
                            break;

                        case Facing.DOWN:
                            Velocity = new Vector2(0, movementSpeed);
                            break;

                        case Facing.LEFT:
                            Velocity = new Vector2(movementSpeed, 0);
                            break;

                        case Facing.RIGHT:
                            Velocity = new Vector2(movementSpeed, 0);
                            break;
                    }
                }
                else
                {
                    Velocity = new Vector2(0, 0);
                }

            }
            else
            {
                Velocity = new Vector2(0, 0);
            }

            _isRunning = isRunning;

            if (currentTool != default(string))
            {
                if (currentTool.Contains("seeds"))
                {
                    currentTool = "seeds";
                }
            }

#if DEBUG
            HarvestMoon.Instance.Season = "Summer";
            HarvestMoon.Instance.Stamina = 50;
            HarvestMoon.Instance.Gold = 10000;
#endif
            if (movementHit)
            {
                if (_isCarrying)
                {
                    _sprite = _heroCarrySprite;
                }
                else if (isRunning)
                {
                    _sprite = _heroRunSpriteA;
                }
                else
                {
                    _sprite = _heroSprite;
                }

            }
            else
            {

                if (_isCarrying && _isPacking)
                {
                    _sprite = _heroPackSprite;
                }
                else if (_isCarrying)
                {
                    _sprite = _heroCarrySprite;
                }
                else if (_isTooling)
                {
                    _sprite = _toolingSprites[currentTool];
                }
                else if (_isHolding)
                {
                    _sprite = _heroHoldSprite;
                }
                else
                {
                    _sprite = _heroSprite;
                }


            }

            string newAnimation = "";

            switch (_playerFacing)
            {
                case Facing.UP:
                    if (movementHit)
                    {
                        if (_isCarrying)
                        {
                            newAnimation = ("carry_up");
                        }
                        else if (isRunning)
                        {
                            newAnimation = ("run_up");
                        }
                        else
                        {
                            newAnimation = ("walk_up");
                        }

                    }
                    else
                    {
                        if (_isCarrying && _isPacking)
                        {
                            newAnimation = ("pack_up");
                        }
                        else if (_isCarrying)
                        {
                            newAnimation = ("carry_up_idle");
                        }
                        else if (_isTooling)
                        {
                            newAnimation = (currentTool + "_" + "up");
                        }
                        else if (_isHolding)
                        {
                            newAnimation = ("hold");
                        }
                        else
                        {
                            newAnimation = ("walk_up_idle");
                        }
                    }
                    break;

                case Facing.DOWN:
                    if (movementHit)
                    {
                        if (_isCarrying)
                        {
                            newAnimation = ("carry_down");
                        }
                        else if (isRunning)
                        {
                            newAnimation = ("run_down");
                        }
                        else
                        {
                            newAnimation = ("walk_down");
                        }
                    }
                    else
                    {
                        if (_isCarrying && _isPacking)
                        {
                            newAnimation = ("pack_down");
                        }
                        else if (_isCarrying)
                        {
                            newAnimation = ("carry_down_idle");
                        }
                        else if (_isTooling)
                        {
                            newAnimation = (currentTool + "_" + "down");
                        }
                        else if (_isHolding)
                        {
                            newAnimation = ("hold");
                        }
                        else
                        {
                            newAnimation = ("walk_down_idle");
                        }
                    }
                    break;

                case Facing.LEFT:
                    if (movementHit)
                    {
                        if (_isCarrying)
                        {
                            newAnimation = ("carry_left");
                        }
                        else if (isRunning)
                        {
                            newAnimation = ("run_left");
                        }
                        else
                        {
                            newAnimation = ("walk_left");
                        }
                    }
                    else
                    {
                        if (_isCarrying && _isPacking)
                        {
                            newAnimation = ("pack_left");
                        }
                        else if (_isCarrying)
                        {
                            newAnimation = ("carry_left_idle");
                        }
                        else if (_isTooling)
                        {
                            newAnimation = (currentTool + "_" + "left");
                        }
                        else if (_isHolding)
                        {
                            newAnimation = ("hold");
                        }
                        else
                        {
                            newAnimation = ("walk_left_idle");
                        }
                    }
                    break;

                case Facing.RIGHT:
                    if (movementHit)
                    {
                        if (_isCarrying)
                        {
                            newAnimation = ("carry_right");
                        }
                        else if (isRunning)
                        {
                            newAnimation = ("run_right");
                        }
                        else
                        {
                            newAnimation = ("walk_right");
                        }
                    }
                    else
                    {
                        if (_isCarrying && _isPacking)
                        {
                            newAnimation = ("pack_right");
                        }
                        else if (_isCarrying)
                        {
                            newAnimation = ("carry_right_idle");
                        }
                        else if (_isTooling)
                        {
                            newAnimation = (currentTool + "_" + "right");
                        }
                        else if (_isHolding)
                        {
                            newAnimation = ("hold");
                        }
                        else
                        {
                            newAnimation = ("walk_right_idle");
                        }

                    }

                    break;
            }

            if(newAnimation != "" && newAnimation != default(string))
            {
                if(_sprite.CurrentAnimation != null)
                {
                    if (_sprite.CurrentAnimation.Name != newAnimation)
                    {
                        _sprite.Play(newAnimation);
                        _sprite.CurrentAnimation.Rewind();
                        _sprite.Update(gameTime);
                    }
                }
                else
                {
                    _sprite.Play(newAnimation);
                }
            }

            if(_currentInteractable != null)
            {
                CheckInteractions();
            }

            if (keyboardState.IsKeyDown(InputDevice.Keys.A) && !_isInteractButtonDown && _isCarrying && _carryingObject != null && !_isFrozen && !_busy)
            {
                _isInteractButtonDown = true;

                Drop();
            }

            if (_carryingObject != null && _isCarrying)
            {
                if (!_tweener.Tweening)
                {
                    _carryingObject.X = _carryingPosition.X;
                    _carryingObject.Y = _carryingPosition.Y;
                }
            }

            if(currentTool != default(string))
            {
                if (currentTool.Contains("seeds"))
                {
                    if(currentTool == "grass-seeds")
                    {
                        if(HarvestMoon.Instance.GrassSeeds == 0)
                        {
                            HarvestMoon.Instance.PackTool("none");
                        }
                    }
                    else if (currentTool == "turnip-seeds")
                    {
                        if (HarvestMoon.Instance.TurnipSeeds == 0)
                        {
                            HarvestMoon.Instance.PackTool("none");
                        }
                    }
                    else if (currentTool == "potato-seeds")
                    {
                        if (HarvestMoon.Instance.PotatoSeeds == 0)
                        {
                            HarvestMoon.Instance.PackTool("none");
                        }
                    }
                    else if (currentTool == "corn-seeds")
                    {
                        if (HarvestMoon.Instance.CornSeeds == 0)
                        {
                            HarvestMoon.Instance.PackTool("none");
                        }
                    }
                    else if (currentTool == "tomato-seeds")
                    {
                        if (HarvestMoon.Instance.TomatoSeeds == 0)
                        {
                            HarvestMoon.Instance.PackTool("none");
                        }
                    }
                }
            }

            var otherTool = HarvestMoon.Instance.GetOtherTool();

            if(otherTool != default(string))
            {
                if (otherTool.Contains("seeds"))
                {
                    if (otherTool == "grass-seeds")
                    {
                        if (HarvestMoon.Instance.GrassSeeds == 0)
                        {
                            HarvestMoon.Instance.PackOtherTool("none");
                        }
                    }
                    else if (otherTool == "turnip-seeds")
                    {
                        if (HarvestMoon.Instance.TurnipSeeds == 0)
                        {
                            HarvestMoon.Instance.PackOtherTool("none");
                        }
                    }
                    else if (otherTool == "potato-seeds")
                    {
                        if (HarvestMoon.Instance.PotatoSeeds == 0)
                        {
                            HarvestMoon.Instance.PackOtherTool("none");
                        }
                    }
                    else if (otherTool == "corn-seeds")
                    {
                        if (HarvestMoon.Instance.CornSeeds == 0)
                        {
                            HarvestMoon.Instance.PackOtherTool("none");
                        }
                    }
                    else if (otherTool == "tomato-seeds")
                    {
                        if (HarvestMoon.Instance.TomatoSeeds == 0)
                        {
                            HarvestMoon.Instance.PackOtherTool("none");
                        }
                    }
                }
            }

            Position += Velocity * deltaSeconds;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float scale = 1.0f;

            if (_isTooling)
            {

                var currentTool = HarvestMoon.Instance.GetCurrentTool();


                if(currentTool == "watering-can")
                {

                    if (_playerFacing == Facing.UP || _playerFacing == Facing.DOWN)
                    {
                        spriteBatch.Draw(_sprite, new Vector2(Position.X, Position.Y - 8), 0.0f, new Vector2(scale, scale));
                    }
                    else
                    {
                        if(_sprite.CurrentAnimation.CurrentFrameIndex == 1)
                        {
                            spriteBatch.Draw(_sprite, new Vector2(Position.X, Position.Y), 0.0f, new Vector2(scale, scale));
                        }
                        else
                        {
                            spriteBatch.Draw(_sprite, new Vector2(Position.X, Position.Y-4), 0.0f, new Vector2(scale, scale));
                        }

                    }

                }
                else
                {

                    if (_playerFacing == Facing.UP || _playerFacing == Facing.DOWN)
                    {
                        spriteBatch.Draw(_sprite, new Vector2(Position.X, Position.Y - 8), 0.0f, new Vector2(scale, scale));
                    }
                    else
                    {
                        spriteBatch.Draw(_sprite, new Vector2(Position.X, Position.Y - 4), 0.0f, new Vector2(scale, scale));

                    }

                }
            }
            else
            {
                if (_isRunning)
                {
                    spriteBatch.Draw(_sprite, new Vector2(Position.X + 1, Position.Y + 1), 0.0f, new Vector2(scale, scale));
                }
                else
                {
                    spriteBatch.Draw(_sprite, Position, 0.0f, new Vector2(scale, scale));
                }

            }



            if (_isHolding)
            {
                var currentTool = HarvestMoon.Instance.GetCurrentTool();

                var currentToolSprite = _holdingItemSprites[currentTool];

                spriteBatch.Draw(currentToolSprite, _carryingPosition);
            }

        }
    }

}
