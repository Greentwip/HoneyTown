using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using Microsoft.Xna.Framework.Content;

namespace HarvestMoon.Entities
{
    public class Axe : Tool
    {
        public Axe(ContentManager content, Vector2 initialPosition)
            : base("axe", content, initialPosition, "maps/tools-room/items/axe")
        {
        }

        public override void Update(GameTime gameTime)
        {

        }
    }

}
