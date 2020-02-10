using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using Microsoft.Xna.Framework.Content;
using HarvestMoon.Entities.General;
using System;
using HarvestMoon.Entities.Items;
using HarvestMoon.Animation;

namespace HarvestMoon.Entities
{
    public class Fooder : NPC
    {
        private Jack _player;
        private ContentManager _content;

        private readonly AnimatedSprite _sprite;

        private bool _fed;

        public int Index { get; set; }

        public Fooder(Jack player, ContentManager content, Vector2 initialPosition, Size2 size): base(initialPosition, size)
        {
            _player = player;
            _content = content;

            BoundingRectangle = new RectangleF(new Vector2(initialPosition.X - size.Width * 0.5f,
                                                            initialPosition.Y - size.Height * 0.5f),
                                               new Size2(size.Width, size.Height));

            X = initialPosition.X;
            Y = initialPosition.Y;

            Planked = true;

            BoundingBoxEnabled = true;

            Carryable = false;

            Interacts = true;

            TypeName = "fooder";

            var cropItems = AnimationLoader.LoadAnimatedSprite(content,
                                                                 "animations/iconSet",
                                                                 "animations/cropItemsMap",
                                                                 "wheatItem",
                                                                 1.0f / 7.5f,
                                                                 false);

            _sprite = cropItems;


            _sprite.Play("grass_normal");

            Index = -1;
        }

        public void Feed(bool fed)
        {
            _fed = fed;
        }

        public override void Interact(Item item, Action onInteractionStart, Action onInteractionEnd)
        {
            if(item is Wheat)
            {
                item.Destroy();
                _fed = true;
            }


            onInteractionStart();
            onInteractionEnd();
        }



        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_fed)
            {
                spriteBatch.Draw(_sprite, BoundingRectangle.Center, 0.0f, new Vector2(1, 1));
            }
        }
    }

}
