using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using Microsoft.Xna.Framework.Content;
using HarvestMoon.Entities.General;
using System;
using HarvestMoon.Entities.Items;

namespace HarvestMoon.Entities
{
    public class FooderBin : NPC
    {
        private Jack _player;
        private ContentManager _content;

        public FooderBin(Jack player, ContentManager content, Vector2 initialPosition, Size2 size): base(initialPosition, size)
        {

            _player = player;
            _content = content;

            BoundingRectangle = new RectangleF(new Vector2(initialPosition.X - size.Width * 0.5f,
                                                            initialPosition.Y - size.Height * 0.5f),
                                               new Size2(size.Width, size.Height));

            X = initialPosition.X;
            Y = initialPosition.Y;

            Planked = true;

            Carryable = false;

            Interacts = true;

            TypeName = "fooder-bin";

        }

        public override void Interact(Item item, Action onInteractionStart, Action onInteractionEnd)
        {
            if(item == null)
            {
                _player.Give(new Wheat(_content, BoundingRectangle.Center));
            }

            onInteractionStart();
            onInteractionEnd();
        }



        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
        }
    }

}
