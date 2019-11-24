using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace HarvestMoon.Entities
{
    public class Hammer : Tool
    {
        public Hammer(ContentManager content, Vector2 initialPosition)
        : base("hammer", content, initialPosition, "maps/tools-room/items/hammer")
        {
        }

        public override void Update(GameTime gameTime)
        {

        }
    }

}
