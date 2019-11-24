using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace HarvestMoon.Entities
{
    public class WateringCan : Tool
    {
        public WateringCan(ContentManager content, Vector2 initialPosition)
        : base("watering-can", content, initialPosition, "maps/tools-room/items/watering-can")
        {
        }

        public override void Update(GameTime gameTime)
        {

        }
     }

}
