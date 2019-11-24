using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace HarvestMoon.Entities
{
    public class Sickle : Tool
    {
        public Sickle(ContentManager content, Vector2 initialPosition)
        : base("sickle", content, initialPosition, "maps/tools-room/items/sickle")
        {
        }

        public override void Update(GameTime gameTime)
        {

        }

    }

}
