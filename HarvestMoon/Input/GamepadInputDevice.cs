using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarvestMoon.Input
{
    public class GamepadInputDevice : InputDevice
    {
        public override bool IsKeyDown(Keys keyType)
        {
            var gamePadState = GamePad.GetState(PlayerIndex.One);
            bool keyDown = false;

            switch (keyType)
            {
                case Keys.Up:
                    if (gamePadState.IsButtonDown(Buttons.DPadUp) || gamePadState.ThumbSticks.Left.Y > 0)
                    {
                        keyDown = true;
                    }
                    break;

                case Keys.Down:
                    if (gamePadState.IsButtonDown(Buttons.DPadDown) || gamePadState.ThumbSticks.Left.Y < 0)
                    {
                        keyDown = true;
                    }
                    break;

                case Keys.Left:
                    if (gamePadState.IsButtonDown(Buttons.DPadLeft) || gamePadState.ThumbSticks.Left.X < 0)
                    {
                        keyDown = true;
                    }
                    break;

                case Keys.Right:
                    if (gamePadState.IsButtonDown(Buttons.DPadRight) || gamePadState.ThumbSticks.Left.X > 0)
                    {
                        keyDown = true;
                    }
                    break;

                case Keys.A:
                    if (gamePadState.IsButtonDown(Buttons.A))
                    {
                        keyDown = true;
                    }
                    break;

                case Keys.B:
                    if (gamePadState.IsButtonDown(Buttons.B))
                    {
                        keyDown = true;
                    }
                    break;

                case Keys.X:
                    if (gamePadState.IsButtonDown(Buttons.X))
                    {
                        keyDown = true;
                    }
                    break;

                case Keys.Y:
                    if (gamePadState.IsButtonDown(Buttons.Y))
                    {
                        keyDown = true;
                    }
                    break;
            }

            return keyDown;
        }

        public override bool IsKeyUp(Keys keyType)
        {
            var gamePadState = GamePad.GetState(PlayerIndex.One);
            bool keyUp = false;

            switch (keyType)
            {
                case Keys.Up:
                    if (gamePadState.IsButtonUp(Buttons.DPadUp))
                    {
                        keyUp = true;
                    }
                    break;

                case Keys.Down:
                    if (gamePadState.IsButtonUp(Buttons.DPadDown))
                    {
                        keyUp = true;
                    }
                    break;

                case Keys.Left:
                    if (gamePadState.IsButtonUp(Buttons.DPadLeft))
                    {
                        keyUp = true;
                    }
                    break;

                case Keys.Right:
                    if (gamePadState.IsButtonUp(Buttons.DPadRight))
                    {
                        keyUp = true;
                    }
                    break;

                case Keys.A:
                    if (gamePadState.IsButtonUp(Buttons.A))
                    {
                        keyUp = true;
                    }
                    break;

                case Keys.B:
                    if (gamePadState.IsButtonUp(Buttons.B))
                    {
                        keyUp = true;
                    }
                    break;

                case Keys.X:
                    if (gamePadState.IsButtonUp(Buttons.X))
                    {
                        keyUp = true;
                    }
                    break;

                case Keys.Y:
                    if (gamePadState.IsButtonUp(Buttons.Y))
                    {
                        keyUp = true;
                    }
                    break;
            }

            return keyUp;

        }
    }
}
