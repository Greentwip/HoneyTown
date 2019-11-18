using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarvestMoon.Input
{
    public abstract class InputDevice
    {
        public enum Keys
        {
            Up,
            Down,
            Left,
            Right,
            A,
            B,
            Y,
            X,
            L,
            R
        }

        public abstract bool IsKeyUp(Keys keyType);
        public abstract bool IsKeyDown(Keys keyType);
    }
}
