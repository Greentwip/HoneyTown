using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace HarvestMoon.Entities
{
    public class GrassSeeds : Tool
    {
        public GrassSeeds(ContentManager content, Vector2 initialPosition)
        : base("grass-seeds", content, initialPosition, "maps/tools-room/items/grass-seeds")
        {
        }

        public override void Update(GameTime gameTime)
        {

        }
    }

}
