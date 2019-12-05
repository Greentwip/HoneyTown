using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace HarvestMoon.Entities.Tools
{
    public class TomatoSeeds : Tool
    {
        public TomatoSeeds(ContentManager content, Vector2 initialPosition)
        : base("tomato-seeds", content, initialPosition, "maps/tools-room/items/tomato-seeds")
        {
        }

        public override void Update(GameTime gameTime)
        {

        }
    }

}
