using System;
using Microsoft.Xna.Framework.Input;
using Wobble.Logging;

namespace Wobble.Input
{
    public class JoystickManager
    {
        /// <summary>
        ///     The current joystick state
        /// </summary>
        public static JoystickState CurrentState { get; private set; }

        /// <summary>
        ///     The joystick state of the previous frame
        /// </summary>
        public static JoystickState PreviousState { get; private set; }

        /// <summary>
        ///     Set to true if we encountered the weird exception and stopped updating the state.
        /// </summary>
        private static bool Broken { get; set; }

        /// <summary>
        ///     Keeps our joystick states updated each frame
        /// </summary>
        internal static void Update()
        {
            PreviousState = CurrentState;

            if (Broken)
            {
                CurrentState = new JoystickState();
                return;
            }

            try
            {
                CurrentState = Joystick.GetState(0); // Get data for "player 1".
            }
            catch (OverflowException e)
            {
                Logger.Error(e, LogType.Runtime);
                Logger.Warning("Encountered the weird joystick exception. Disabling joystick support.", LogType.Runtime);
                Broken = true;
            }
        }

        /// <summary>
        ///     If the button was pressed and released - useful for actions that require a single button press.
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public static bool IsUniqueButtonPress(int button)
        {
            if (button >= CurrentState.Buttons.Length)
                return false;

            return CurrentState.Buttons[button] == ButtonState.Pressed && PreviousState.Buttons[button] == ButtonState.Released;
        }

        /// <summary>
        ///     If a button was previously pressed down and then released.
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public static bool IsUniqueButtonRelease(int button)
        {
            if (button >= CurrentState.Buttons.Length)
                return false;

            return CurrentState.Buttons[button] == ButtonState.Released && PreviousState.Buttons[button] == ButtonState.Pressed;
        }
    }
}