using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace HarvestMoon.Entities
{
    public class TurnipSeeds : Tool
    {
        public TurnipSeeds(ContentManager content, Vector2 initialPosition)
        : base("turnip-seeds", content, initialPosition, "maps/tools-room/items/turnip-seeds")
        {
        }

        public override void Update(GameTime gameTime)
        {

        }
    }

}
