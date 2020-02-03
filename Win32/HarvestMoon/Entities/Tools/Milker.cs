using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace HarvestMoon.Entities
{
    public class Milker : Tool
    {
        public Milker(ContentManager content, Vector2 initialPosition)
        : base("milker", content, initialPosition, "maps/tools-room/items/milker")
        {
        }

        public override void Update(GameTime gameTime)
        {

        }
    }

}
