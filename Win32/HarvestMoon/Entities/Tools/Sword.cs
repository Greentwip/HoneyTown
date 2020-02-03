using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace HarvestMoon.Entities
{
    public class Sword : Tool
    {
        public Sword(ContentManager content, Vector2 initialPosition)
        : base("sword", content, initialPosition, "maps/tools-room/items/sword")
        {
        }

        public override void Update(GameTime gameTime)
        {

        }
    }

}
