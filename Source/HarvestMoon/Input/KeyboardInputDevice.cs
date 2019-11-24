using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarvestMoon.Input
{

    public class KeyboardInputDevice : InputDevice
    {
        public override bool IsKeyDown(Keys keyType)
        {
            var keyboardState = Keyboard.GetState();
            bool keyDown = false;

            switch (keyType)
            {
                case Keys.Up:
                    if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                    {
                        keyDown = true;
                    }
                    break;

                case Keys.Down:
                    if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                    {
                        keyDown = true;
                    }
                    break;

                case Keys.Left:
                    if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
                    {
                        keyDown = true;
                    }
                    break;

                case Keys.Right:
                    if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
                    {
                        keyDown = true;
                    }
                    break;

                case Keys.A:
                    if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.V))
                    {
                        keyDown = true;
                    }
                    break;

                case Keys.B:
                    if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.C))
                    {
                        keyDown = true;
                    }
                    break;

                case Keys.X:
                    if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.X))
                    {
                        keyDown = true;
                    }
                    break;

                case Keys.Y:
                    if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Z))
                    {
                        keyDown = true;
                    }
                    break;
            }

            return keyDown;
        }

        public override bool IsKeyUp(Keys keyType)
        {
            var keyboardState = Keyboard.GetState();
            bool keyUp = false;

            switch (keyType)
            {
                case Keys.Up:
                    if (keyboardState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Up))
                    {
                        keyUp = true;
                    }
                    break;

                case Keys.Down:
                    if (keyboardState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Down))
                    {
                        keyUp = true;
                    }
                    break;

                case Keys.Left:
                    if (keyboardState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Left))
                    {
                        keyUp = true;
                    }
                    break;

                case Keys.Right:
                    if (keyboardState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Right))
                    {
                        keyUp = true;
                    }
                    break;

                case Keys.A:
                    if (keyboardState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.V))
                    {
                        keyUp = true;
                    }
                    break;

                case Keys.B:
                    if (keyboardState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.C))
                    {
                        keyUp = true;
                    }
                    break;

                case Keys.X:
                    if (keyboardState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.X))
                    {
                        keyUp = true;
                    }
                    break;

                case Keys.Y:
                    if (keyboardState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Z))
                    {
                        keyUp = true;
                    }
                    break;
            }

            return keyUp;

        }
    }

}
