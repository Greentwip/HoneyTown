using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace HarvestMoon.Entities
{
    public class PotatoSeeds : Tool
    {
        public PotatoSeeds(ContentManager content, Vector2 initialPosition)
        : base("potato-seeds", content, initialPosition, "maps/tools-room/items/potato-seeds")
        {
        }

        public override void Update(GameTime gameTime)
        {

        }
    }

}
