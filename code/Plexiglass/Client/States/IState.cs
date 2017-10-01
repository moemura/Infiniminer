using Infiniminer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Plexiglass.Client.States
{
    public interface IState
    {
        IStateMachine Sm { get; set; }
        IPropertyBag P { get; set; }

        void OnEnter(string oldState);
        void OnLeave(string newState);
        string OnUpdate(GameTime gameTime, KeyboardState keyState, MouseState mouseState);
        void OnRenderAtEnter(GraphicsDevice graphicsDevice);
        void OnRenderAtUpdate(GraphicsDevice graphicsDevice, GameTime gameTime);
        void OnCharEntered(ICharacterEventArgs e);
        void OnKeyDown(Keys key);
        void OnMouseDown(MouseButton button, int x, int y);
        void OnMouseUp(MouseButton button, int x, int y);
        void OnMouseScroll(int scrollWheelValue);

        //================== BEGIN PLEXIGLASS FUNCTIONS ====================\\
        void PrecacheContent();
    }
}
