using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Xspace.Son;

namespace MenuSample.Inputs
{

    public class InputState : GameComponent
    {

        private static KeyboardState _currentKeyboardState;
        private static KeyboardState _lastKeyboardState;

        public static KeyboardState CurrentKeyboardState
        {
            get { return _currentKeyboardState; }
        }


        public InputState(Game game)
            : base(game)
        {
            _currentKeyboardState = new KeyboardState();
            _lastKeyboardState = new KeyboardState();
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _lastKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();
            AudioPlayer.Update();
        }

        private static bool IsNewKeyPress(Keys key)
        {
            return (_currentKeyboardState.IsKeyDown(key) &&
                    _lastKeyboardState.IsKeyUp(key));
        }


        public static bool IsMenuSelect()
        {
            return IsNewKeyPress(Keys.Enter);
        }

        public static bool IsMenuCancel()
        {
            return IsNewKeyPress(Keys.Escape);
        }

        public static bool IsMenuUp()
        {
            return IsNewKeyPress(Keys.Up);
        }

        public static bool IsMenuDown()
        {
            return IsNewKeyPress(Keys.Down);
        }

        public static bool IsPauseGame()
        {
            return IsNewKeyPress(Keys.Escape);
        }

    }
}
