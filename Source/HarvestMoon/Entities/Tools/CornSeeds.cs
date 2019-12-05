using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace HarvestMoon.Entities.Tools
{
    public class CornSeeds : Tool
    {
        public CornSeeds(ContentManager content, Vector2 initialPosition)
        : base("corn-seeds", content, initialPosition, "maps/tools-room/items/corn-seeds")
        {
        }

        public override void Update(GameTime gameTime)
        {

        }
    }

}
