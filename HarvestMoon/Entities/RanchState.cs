using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HarvestMoon.Entities
{
    public class RanchState : EntityManager
    {
        public bool IsLoaded { get; set; }
        public RanchState(): base()
        {
        }
    }
}
