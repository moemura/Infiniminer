using Infiniminer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plexiglass.Client.States
{
    public interface IState
    {
        IStateMachine _SM { get; set; }
        IPropertyBag _P { get; set; }

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
