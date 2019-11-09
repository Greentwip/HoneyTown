using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace HarvestMoon.Entities
{
    public class Hoe : Tool
    {
        public Hoe(ContentManager content, Vector2 initialPosition)
        : base("hoe", content, initialPosition, "maps/tools-room/items/hoe")
        {
        }

        public override void Update(GameTime gameTime)
        {

        }
    }

}
