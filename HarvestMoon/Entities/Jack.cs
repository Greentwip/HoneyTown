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
using Glide;
using MonoGame.Extended.Collisions;
using System;
using HarvestMoon.Entities.Ranch;
using HarvestMoon.Screens;
using HarvestMoon.Entities.General;

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

        public Door LastVisitedDoor { get; set; }

        private readonly AnimatedSprite _sprite;
        private readonly Transform2 _transform;

        private const float _defaultWalkSpeed = 100.0f;
        private const float _defaultRunSpeed = 180.0f;

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
                                                         value.Y - BoundingRectangle.Size.Height * 0.5f);

                _carryingPosition = new Vector2(value.X, BoundingRectangle.Top - 16);
                _packingPosition = new Vector2(value.X, BoundingRectangle.Top - 8);
                SetupActionBoundingRectangle(BoundingRectangle.TopLeft);
            }
        }

        public Vector2 Velocity { get; set; }

        public RectangleF ActionBoundingRectangle = RectangleF.Empty;

        private Interactable _carryingObject = null;
        private Interactable _breakableObject = null;
        private readonly Tweener _tweener = new Tweener();
        private bool _isCarrying = false;
        private bool _isPacking = false;
        private bool _isFrozen = false;

        private bool _isTooling = false;
        private bool _isHolding = false;

        private readonly ContentManager _contentManager;
        private readonly EntityManager _entityManager;

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
            
            var characterTexture = content.Load<Texture2D>("animations/jack");
            var characterMap = content.Load<Dictionary<string, Rectangle>>("animations/jackMap");
            var characterAtlas = new TextureAtlas("jack", characterTexture, characterMap);
            var characterAnimationFactory = new SpriteSheet
            {
                TextureAtlas = characterAtlas,
                Cycles =
                {
                    {
                        "walk_down", new SpriteSheetAnimationCycle
                        {
                            IsLooping = true,
                            IsPingPong = true,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                // TODO: Fix per frame duration
                                new SpriteSheetAnimationFrame(0),
                                new SpriteSheetAnimationFrame(1),
                                new SpriteSheetAnimationFrame(2),
                                new SpriteSheetAnimationFrame(1)
                            }
                        }
                    },
                    {
                        "walk_left", new SpriteSheetAnimationCycle
                        {
                            IsLooping = true,
                            IsPingPong = true,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                // TODO: Fix per frame duration
                                new SpriteSheetAnimationFrame(3),
                                new SpriteSheetAnimationFrame(4),
                                new SpriteSheetAnimationFrame(5),
                                new SpriteSheetAnimationFrame(4)
                            }
                        }
                    },
                    {
                        "walk_right", new SpriteSheetAnimationCycle
                        {
                            IsLooping = true,
                            IsPingPong = true,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                // TODO: Fix per frame duration
                                new SpriteSheetAnimationFrame(3),
                                new SpriteSheetAnimationFrame(4),
                                new SpriteSheetAnimationFrame(5),
                                new SpriteSheetAnimationFrame(4)
                            }
                        }
                    },
                    {
                        "walk_up", new SpriteSheetAnimationCycle
                        {
                            IsLooping = true,
                            IsPingPong = true,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                // TODO: Fix per frame duration
                                new SpriteSheetAnimationFrame(6),
                                new SpriteSheetAnimationFrame(7),
                                new SpriteSheetAnimationFrame(8),
                                new SpriteSheetAnimationFrame(7)
                            }
                        }
                    },
                    {
                        "walk_down_idle", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                // TODO: Fix per frame duration
                                new SpriteSheetAnimationFrame(9)
                                
                            }
                        }
                    },
                    {
                        "walk_left_idle", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(10)                             
                            }
                        }
                    },
                    {
                        "walk_right_idle", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(10)
                            }
                        }
                    },
                    {
                        "walk_up_idle", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(11)
                            }
                        }
                    },
                    {
                        "run_down", new SpriteSheetAnimationCycle
                        {
                            IsLooping = true,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(12),
                                new SpriteSheetAnimationFrame(1),
                                new SpriteSheetAnimationFrame(13),
                                new SpriteSheetAnimationFrame(1)
                            }
                        }
                    },
                    {
                        "run_left", new SpriteSheetAnimationCycle
                        {
                            IsLooping = true,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(14),
                                new SpriteSheetAnimationFrame(15),
                                new SpriteSheetAnimationFrame(16),
                                new SpriteSheetAnimationFrame(15)

                            }
                        }
                    },
                    {
                        "run_right", new SpriteSheetAnimationCycle
                        {
                            IsLooping = true,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(14),
                                new SpriteSheetAnimationFrame(15),
                                new SpriteSheetAnimationFrame(16),
                                new SpriteSheetAnimationFrame(15)
                            }
                        }
                    },
                    {
                        "run_up", new SpriteSheetAnimationCycle
                        {
                            IsLooping = true,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(17),
                                new SpriteSheetAnimationFrame(18),
                                new SpriteSheetAnimationFrame(19),
                                new SpriteSheetAnimationFrame(18)
                            }
                        }
                    },
                    {
                        "carry_down", new SpriteSheetAnimationCycle
                        {
                            IsLooping = true,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(20),
                                new SpriteSheetAnimationFrame(21),
                                new SpriteSheetAnimationFrame(22),
                                new SpriteSheetAnimationFrame(21)
                            }
                        }
                    },
                    {
                        "carry_left", new SpriteSheetAnimationCycle
                        {
                            IsLooping = true,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(23),
                                new SpriteSheetAnimationFrame(24),
                                new SpriteSheetAnimationFrame(25),
                                new SpriteSheetAnimationFrame(24)
                            }
                        }
                    },
                    {
                        "carry_right", new SpriteSheetAnimationCycle
                        {
                            IsLooping = true,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(23),
                                new SpriteSheetAnimationFrame(24),
                                new SpriteSheetAnimationFrame(25),
                                new SpriteSheetAnimationFrame(24)
                            }
                        }
                    },
                    {
                        "carry_up", new SpriteSheetAnimationCycle
                        {
                            IsLooping = true,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(26),
                                new SpriteSheetAnimationFrame(27),
                                new SpriteSheetAnimationFrame(28),
                                new SpriteSheetAnimationFrame(27)
                            }
                        }
                    },
                    {
                        "carry_down_idle", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(29)
                            }
                        }
                    },
                    {
                        "carry_left_idle", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(30)
                            }
                        }
                    },
                    {
                        "carry_right_idle", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(30)
                            }
                        }
                    },
                    {
                        "carry_up_idle", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(31)
                            }
                        }
                    },
                    {
                        "pack_down", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(32),
                                new SpriteSheetAnimationFrame(33),
                                new SpriteSheetAnimationFrame(34)
                            }
                        }
                    },
                    {
                        "pack_left", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(35),
                                new SpriteSheetAnimationFrame(36),
                                new SpriteSheetAnimationFrame(37)
                            }
                        }
                    },
                    {
                        "pack_right", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(35),
                                new SpriteSheetAnimationFrame(36),
                                new SpriteSheetAnimationFrame(37)
                            }
                        }
                    },
                    {
                        "pack_up", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(38),
                                new SpriteSheetAnimationFrame(39),
                                new SpriteSheetAnimationFrame(40)
                            }
                        }
                    },
                    {
                        "sickle_down", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(41),
                                new SpriteSheetAnimationFrame(42),
                                new SpriteSheetAnimationFrame(43),
                                new SpriteSheetAnimationFrame(44)
                            }
                        }
                    },
                    {
                        "sickle_up", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(45),
                                new SpriteSheetAnimationFrame(46),
                                new SpriteSheetAnimationFrame(47),
                                new SpriteSheetAnimationFrame(48)
                            }
                        }
                    },
                    {
                        "sickle_left", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(49),
                                new SpriteSheetAnimationFrame(50),
                                new SpriteSheetAnimationFrame(51),
                                new SpriteSheetAnimationFrame(52)
                            }
                        }
                    },
                    {
                        "sickle_right", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(49),
                                new SpriteSheetAnimationFrame(50),
                                new SpriteSheetAnimationFrame(51),
                                new SpriteSheetAnimationFrame(52)
                            }
                        }
                    },
                    {
                        "hoe_down", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(53),
                                new SpriteSheetAnimationFrame(54),
                                new SpriteSheetAnimationFrame(55),
                                new SpriteSheetAnimationFrame(56)
                            }
                        }
                    },
                    {
                        "hoe_up", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(57),
                                new SpriteSheetAnimationFrame(58),
                                new SpriteSheetAnimationFrame(59),
                                new SpriteSheetAnimationFrame(60)
                            }
                        }
                    },
                    {
                        "hoe_left", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(61),
                                new SpriteSheetAnimationFrame(62),
                                new SpriteSheetAnimationFrame(63),
                                new SpriteSheetAnimationFrame(64)
                            }
                        }
                    },
                    {
                        "hoe_right", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(61),
                                new SpriteSheetAnimationFrame(62),
                                new SpriteSheetAnimationFrame(63),
                                new SpriteSheetAnimationFrame(64)
                            }
                        }
                    },
                    {
                        "hammer_down", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(65),
                                new SpriteSheetAnimationFrame(66),
                                new SpriteSheetAnimationFrame(67),
                                new SpriteSheetAnimationFrame(68)
                            }
                        }
                    },
                    {
                        "hammer_up", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(69),
                                new SpriteSheetAnimationFrame(70),
                                new SpriteSheetAnimationFrame(71),
                                new SpriteSheetAnimationFrame(72)
                            }
                        }
                    },
                    {
                        "hammer_left", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(73),
                                new SpriteSheetAnimationFrame(74),
                                new SpriteSheetAnimationFrame(75),
                                new SpriteSheetAnimationFrame(76)
                            }
                        }
                    },
                    {
                        "hammer_right", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(73),
                                new SpriteSheetAnimationFrame(74),
                                new SpriteSheetAnimationFrame(75),
                                new SpriteSheetAnimationFrame(76)
                            }
                        }
                    },
                    {
                        "axe_down", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(77),
                                new SpriteSheetAnimationFrame(78),
                                new SpriteSheetAnimationFrame(79),
                                new SpriteSheetAnimationFrame(80)
                            }
                        }
                    },
                    {
                        "axe_up", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(81),
                                new SpriteSheetAnimationFrame(82),
                                new SpriteSheetAnimationFrame(83),
                                new SpriteSheetAnimationFrame(84)
                            }
                        }
                    },
                    {
                        "axe_left", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(85),
                                new SpriteSheetAnimationFrame(86),
                                new SpriteSheetAnimationFrame(87),
                                new SpriteSheetAnimationFrame(88)
                            }
                        }
                    },
                    {
                        "axe_right", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(85),
                                new SpriteSheetAnimationFrame(86),
                                new SpriteSheetAnimationFrame(87),
                                new SpriteSheetAnimationFrame(88)
                            }
                        }
                    },
                    {
                        "watering-can_down", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(89),
                                new SpriteSheetAnimationFrame(90),
                                new SpriteSheetAnimationFrame(90),
                                new SpriteSheetAnimationFrame(90)
                            }
                        }
                    },
                    {
                        "watering-can_up", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(91),
                                new SpriteSheetAnimationFrame(92),
                                new SpriteSheetAnimationFrame(92),
                                new SpriteSheetAnimationFrame(92)
                            }
                        }
                    },
                    {
                        "watering-can_left", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(93),
                                new SpriteSheetAnimationFrame(94),
                                new SpriteSheetAnimationFrame(94),
                                new SpriteSheetAnimationFrame(94)
                            }
                        }
                    },
                    {
                        "watering-can_right", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(93),
                                new SpriteSheetAnimationFrame(94),
                                new SpriteSheetAnimationFrame(94),
                                new SpriteSheetAnimationFrame(94)
                            }
                        }
                    },
                    {
                        "seeds_down", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(95),
                                new SpriteSheetAnimationFrame(96),
                                new SpriteSheetAnimationFrame(97),
                                new SpriteSheetAnimationFrame(98),
                                new SpriteSheetAnimationFrame(99),
                                new SpriteSheetAnimationFrame(100),
                                new SpriteSheetAnimationFrame(101),
                                new SpriteSheetAnimationFrame(102),
                                new SpriteSheetAnimationFrame(103),
                                new SpriteSheetAnimationFrame(104),
                                new SpriteSheetAnimationFrame(105),
                                new SpriteSheetAnimationFrame(106)
                            }
                        }
                    },
                    {
                        "seeds_up", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(107),
                                new SpriteSheetAnimationFrame(108),
                                new SpriteSheetAnimationFrame(109),
                                new SpriteSheetAnimationFrame(110),
                                new SpriteSheetAnimationFrame(111),
                                new SpriteSheetAnimationFrame(112),
                                new SpriteSheetAnimationFrame(113),
                                new SpriteSheetAnimationFrame(114),
                                new SpriteSheetAnimationFrame(115)
                            }
                        }
                    },
                    {
                        "seeds_left", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(116),
                                new SpriteSheetAnimationFrame(117),
                                new SpriteSheetAnimationFrame(118),
                                new SpriteSheetAnimationFrame(119),
                                new SpriteSheetAnimationFrame(120),
                                new SpriteSheetAnimationFrame(121),
                                new SpriteSheetAnimationFrame(122),
                                new SpriteSheetAnimationFrame(123),
                                new SpriteSheetAnimationFrame(124)
                            }
                        }
                    },
                    {
                        "seeds_right", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(116),
                                new SpriteSheetAnimationFrame(117),
                                new SpriteSheetAnimationFrame(118),
                                new SpriteSheetAnimationFrame(119),
                                new SpriteSheetAnimationFrame(120),
                                new SpriteSheetAnimationFrame(121),
                                new SpriteSheetAnimationFrame(122),
                                new SpriteSheetAnimationFrame(123),
                                new SpriteSheetAnimationFrame(124)
                            }
                        }
                    },
                    {
                        "hold", new SpriteSheetAnimationCycle
                        {
                            IsLooping = false,
                            IsPingPong = false,
                            FrameDuration = frameDuration,
                            Frames =
                            {
                                new SpriteSheetAnimationFrame(125)
                            }
                        }
                    }
                }
            };

            var characterSpriteAnimation = new AnimatedSprite(characterAnimationFactory, "walk_right");

            _sprite = characterSpriteAnimation;
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
            _holdingItemSprites.Add("hammer", new Sprite(content.Load<Texture2D>("maps/tools-room/items/hammer")));
            _holdingItemSprites.Add("hoe", new Sprite(content.Load<Texture2D>("maps/tools-room/items/hoe")));
            _holdingItemSprites.Add("sickle", new Sprite(content.Load<Texture2D>("maps/tools-room/items/sickle")));
            _holdingItemSprites.Add("watering-can", new Sprite(content.Load<Texture2D>("maps/tools-room/items/watering-can")));
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
                _breakableObject = null;
                return;
            }

            var keyboardState = Keyboard.GetState();

            if (interactable.Breakable)
            {
                _breakableObject = interactable;
            }

            if (_carryingObject == null && 
                keyboardState.IsKeyDown(Keys.V) && 
                !_isInteractButtonDown && 
                (interactable.Carryable || interactable.Packable) &&
                interactable.Interacts)
            {
                _isInteractButtonDown = true;


                _carryingObject = interactable;

                var carryingParameters = new { X = _carryingPosition.X, Y = _carryingPosition.Y };

                if (interactable.Packable)
                {
                    carryingParameters = new { X = _packingPosition.X, Y = _packingPosition.Y };
                }

                _tweener.Tween(interactable, carryingParameters, 0.25f)
                        .Ease(Ease.ExpoInOut)
                        .OnBegin(() =>
                        {
                            Freeze();
                            _carryingObject.Planked = false;
                            _carryingObject.Priority = Priority;
                            _isCarrying = true;
                            _isPacking = true;
                        });


            }
            else if(keyboardState.IsKeyDown(Keys.V) && 
                    !_isInteractButtonDown && 
                    interactable.IsNPC && 
                    interactable.Interacts &&
                !_busy)
            {
                _busy = true;

                var npc = interactable as NPC;
                if(npc.DeploysMenu)
                {
                    switch (npc.DeployableMenu) {
                        case NPC.NPCMenu.YesNo:
                            _mapScreen.ShowMessage(npc.Message, () =>
                            {
                                _mapScreen.ShowYesNoMessage(npc.Strings[0], 
                                                            npc.Strings[1], 
                                                            () =>
                                                            {
                                                                npc.Callbacks[0]();
                                                            },
                                                            () =>
                                                            {
                                                                npc.Callbacks[1]();
                                                            });
                            });

                            break;
                    }
                }
                else
                {
                    _mapScreen.ShowMessage(npc.Message, () =>
                    {
                        _npcCoolDown = true;
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

                    /*System.Diagnostics.Debug.WriteLine(ActionBoundingRectangle.Center.ToString());
                    System.Diagnostics.Debug.WriteLine(cellAtPosition.BoundingBox.Center.ToString());
                    System.Diagnostics.Debug.WriteLine(grid.CollisionGridSet.GridPosition.ToString());
                    System.Diagnostics.Debug.WriteLine(targetPosition.ToString());*/

                    var interactablesButSoil = _entityManager
                                                .Entities
                                                .Where(e => { return e is Interactable && !(e is Soil); })
                                                .Cast<Interactable>().ToArray();
                    bool collides = false;
                    foreach(var interactable in interactablesButSoil) {
                        if(interactable.BoundingRectangle.Contains(new Point2(targetPosition.X, targetPosition.Y)))
                        {
                            collides = true;
                            break;
                        }
                    }

                    if (!collides)
                    {
                        if(_carryingObject is SmallRock || _carryingObject is WoodPiece)
                        {
                            var soilSegments = _entityManager.Entities
                                                         .Where(e => { return e is Soil; })
                                                         .Cast<Soil>().ToArray();

                            foreach(var soilSegment in soilSegments)
                            {
                                if(soilSegment.BoundingRectangle.Contains(new Point2(targetPosition.X, targetPosition.Y)))
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
                               new { X = targetPosition.X, Y = targetPosition.Y },
                               0.125f)
                                   .Ease(Ease.ExpoInOut)
                                   .OnComplete(() =>
                                   {
                                       UnFreeze();
                                       _carryingObject.Planked = true;
                                       _carryingObject.Priority = 0;
                                       _carryingObject = null;
                                       _isCarrying = false;
                                   });

                    }
                    break;
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

                        if (soilPosition == targetPositionVector && !soilPiece.HasGrown)
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

                            if (soilPosition == targetPositionVector)
                            {
                                var currentTool = HarvestMoon.Instance.GetCurrentTool();
                                if (currentTool.Contains("grass"))
                                {
                                    soilPiece.Plant("grass", 0, HarvestMoon.Instance.Season);
                                }

                                if (currentTool.Contains("turnip"))
                                {
                                    soilPiece.Plant("turnip", 0, HarvestMoon.Instance.Season);
                                }

                                if (currentTool.Contains("potato"))
                                {
                                    soilPiece.Plant("potato", 0, HarvestMoon.Instance.Season);
                                }

                                if (currentTool.Contains("tomato"))
                                {
                                    soilPiece.Plant("tomato", 0, HarvestMoon.Instance.Season);
                                }

                                if (currentTool.Contains("corn"))
                                {
                                    soilPiece.Plant("corn", 0, HarvestMoon.Instance.Season);
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
                                totalWaitTime *= 2;
                                break;
                        }
                        
                    }
                }

                if (_toolTimer >= totalWaitTime)
                {
                    _toolTimer = 0.0f;
                    _isTooling = false;

                    if(currentTool == "hoe")
                    {
                        Hoe();
                    }
                    else if(currentTool == "watering-can")
                    {
                        Water();
                    }

                    if (currentTool.Contains("seeds"))
                    {
                        Plant();
                    }
                    
                    if(_breakableObject != null)
                    {
                        //try_break(_breakableObject, _breakingPower);
                    }

                    UnFreeze();
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

                    UnFreeze();
                }
            }

            var keyboardState = Keyboard.GetState();
            float movementSpeed = _defaultWalkSpeed;
            bool movementHit = false;
            bool isRunning = false;

            if (keyboardState.IsKeyUp(Keys.V))
            {
                _isInteractButtonDown = false;
            }

            if (keyboardState.IsKeyUp(Keys.X))
            {
                _isToolButtonDown = false;
            }

            if (keyboardState.IsKeyUp(Keys.Z))
            {
                _isHoldButtonDown = false;
            }


            if (keyboardState.IsKeyDown(Keys.X) && !_isToolButtonDown && !_isCarrying && !_isFrozen)
            {
                _isToolButtonDown = true;


                if(currentTool != default(string))
                {
                    _isTooling = true;
                    Freeze();
                }
                
                //_breakPower++;
            }

            if (keyboardState.IsKeyDown(Keys.Z) && !_isHoldButtonDown && !_isCarrying && !_isFrozen)
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
                if (keyboardState.IsKeyDown(Keys.C) && !_isCarrying)
                {
                    isRunning = true;
                    movementHit = true;
                }

                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    _playerFacing = Facing.UP;
                    movementHit = true;
                }

                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    _playerFacing = Facing.DOWN;
                    movementHit = true;
                }

                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    _playerFacing = Facing.LEFT;
                    movementHit = true;
                }

                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    _playerFacing = Facing.RIGHT;
                    movementHit = true;
                }

                if (movementHit)
                {
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

            if (currentTool != default(string))
            {
                if (currentTool.Contains("seeds"))
                {
                    currentTool = "seeds";
                }
            }
            

            switch (_playerFacing)
            {
                case Facing.UP:
                    if (movementHit)
                    {
                        if (_isCarrying)
                        {
                            _sprite.Play("carry_up");
                        }
                        else if (isRunning)
                        {
                            _sprite.Play("run_up");
                        }
                        else
                        {
                            _sprite.Play("walk_up");
                        }

                    }
                    else
                    {
                        if (_isCarrying && _isPacking)
                        {
                            _sprite.Play("pack_up");
                        }
                        else if (_isCarrying)
                        {
                            _sprite.Play("carry_up_idle");
                        }
                        else if (_isTooling)
                        {
                            _sprite.Play(currentTool + "_" + "up");
                        }
                        else if (_isHolding)
                        {
                            _sprite.Play("hold");
                        }
                        else
                        {
                            _sprite.Play("walk_up_idle");
                        }
                    }
                    break;

                case Facing.DOWN:
                    if (movementHit)
                    {
                        if (_isCarrying)
                        {
                            _sprite.Play("carry_down");
                        }
                        else if (isRunning)
                        {
                            _sprite.Play("run_down");
                        }
                        else
                        {
                            _sprite.Play("walk_down");
                        }
                    }
                    else
                    {
                        if (_isCarrying && _isPacking)
                        {
                            _sprite.Play("pack_down");
                        }
                        else if (_isCarrying)
                        {
                            _sprite.Play("carry_down_idle");
                        }
                        else if (_isTooling)
                        {
                            _sprite.Play(currentTool + "_" + "down");
                        }
                        else if (_isHolding)
                        {
                            _sprite.Play("hold");
                        }
                        else
                        {
                            _sprite.Play("walk_down_idle");
                        }
                    }
                    break;

                case Facing.LEFT:
                    if (movementHit)
                    {
                        if (_isCarrying)
                        {
                            _sprite.Play("carry_left");
                        }
                        else if (isRunning)
                        {
                            _sprite.Play("run_left");
                        }
                        else
                        {
                            _sprite.Play("walk_left");
                        }
                    }
                    else
                    {
                        if (_isCarrying && _isPacking)
                        {
                            _sprite.Play("pack_left");
                        }
                        else if (_isCarrying)
                        {
                            _sprite.Play("carry_left_idle");
                        }
                        else if (_isTooling)
                        {
                            _sprite.Play(currentTool + "_" + "left");
                        }
                        else if (_isHolding)
                        {
                            _sprite.Play("hold");
                        }
                        else
                        {
                            _sprite.Play("walk_left_idle");
                        }
                    }
                    break;

                case Facing.RIGHT:
                    if (movementHit)
                    {
                        if (_isCarrying)
                        {
                            _sprite.Play("carry_right");
                        }
                        else if (isRunning)
                        {
                            _sprite.Play("run_right");
                        }
                        else
                        {
                            _sprite.Play("walk_right");
                        }
                    }
                    else
                    {
                        if (_isCarrying && _isPacking)
                        {
                            _sprite.Play("pack_right");
                        }
                        else if (_isCarrying)
                        {
                            _sprite.Play("carry_right_idle");
                        }
                        else if (_isTooling)
                        {
                            _sprite.Play(currentTool + "_" + "right");
                        }
                        else if (_isHolding)
                        {
                            _sprite.Play("hold");
                        }
                        else
                        {
                            _sprite.Play("walk_right_idle");
                        }

                    }

                    break;
            }

            if (keyboardState.IsKeyDown(Keys.V) && !_isInteractButtonDown)
            {
                if (_isCarrying && _carryingObject != null && !_isFrozen)
                {
                    _isInteractButtonDown = true;

                    Drop();
                }
                
            }

            if(_carryingObject != null && _isCarrying)
            {
                _carryingObject.X = _carryingPosition.X;
                _carryingObject.Y = _carryingPosition.Y;
            }

            Position += Velocity * deltaSeconds;



            _sprite.Update(deltaSeconds);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_playerFacing == Facing.RIGHT)
            {
                _sprite.Effect = SpriteEffects.FlipHorizontally;
            }
            else
            {
                _sprite.Effect = SpriteEffects.None;
            }

            spriteBatch.Draw(_sprite, Position, 0.0f, new Vector2(1.8f, 1.8f));

            if (_isHolding)
            {
                var currentTool = HarvestMoon.Instance.GetCurrentTool();

                var currentToolSprite = _holdingItemSprites[currentTool];

                spriteBatch.Draw(currentToolSprite, _carryingPosition);
            }

        }
    }

}
