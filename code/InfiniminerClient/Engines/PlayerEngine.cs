using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Plexiglass.Client.Engine;

namespace Infiniminer
{
    public class PlayerEngine : IEngine
    {
        InfiniminerGame gameInstance;
        PropertyBag _P;

        BlockEngine blockEngine = null;

        public PlayerEngine(InfiniminerGame gameInstance)
        {
            this.gameInstance = gameInstance;
            blockEngine = gameInstance.propertyBag.GetEngine<BlockEngine>("blockEngine");
        }

        public void Update(GameTime gameTime)
        {
            if (_P == null)
                return;

            foreach (Player p in _P.playerList.Values)
            {
                p.StepInterpolation(gameTime.TotalGameTime.TotalSeconds);

                p.Ping -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (p.Ping < 0)
                    p.Ping = 0;

                p.TimeIdle += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (p.TimeIdle > 0.5f)
                    p.IdleAnimation = true;
                p.SpriteModel.Update(gameTime);
            }
        }

        public void Render(GraphicsDevice graphicsDevice)
        {
            // If we don't have _P, grab it from the current gameInstance.
            // We can't do this in the constructor because we are created in the property bag's constructor!
            if (_P == null)
                _P = gameInstance.propertyBag;

            foreach (Player p in _P.playerList.Values)
            {
                if (p.Alive && p.ID != _P.PlayerContainer.PlayerMyId)
                {
                    p.SpriteModel.Draw(_P.PlayerContainer.PlayerCamera.ViewMatrix,
                                       _P.PlayerContainer.PlayerCamera.ProjectionMatrix,
                                       _P.PlayerContainer.PlayerCamera.Position,
                                       _P.PlayerContainer.PlayerCamera.GetLookVector(),
                                       p.Position - Vector3.UnitY * 1.5f,
                                       p.Heading,
                                       2);
                }
            }
        }

        public void RenderPlayerNames(GraphicsDevice graphicsDevice)
        {
            // If we don't have _P, grab it from the current gameInstance.
            // We can't do this in the constructor because we are created in the property bag's constructor!
            if (_P == null)
                _P = gameInstance.propertyBag;

            foreach (Player p in _P.playerList.Values)
            {
                if (p.Alive && p.ID != _P.PlayerContainer.PlayerMyId)
                {
                    // Figure out what text we should draw on the player - only for teammates and nearby enemies
                    string playerText = "";
                    bool continueDraw=false;
                    if (p.ID != _P.PlayerContainer.PlayerMyId && p.Team == _P.PlayerContainer.PlayerTeam)
                        continueDraw = true;
                    else
                    {
                        Vector3 diff = (p.Position -_P.PlayerContainer.PlayerPosition);
                        float len = diff.Length();
                        diff.Normalize();
                        if (len<=15){
                            Vector3 hit = Vector3.Zero;
                            Vector3 build = Vector3.Zero;
                            blockEngine.RayCollision(_P.PlayerContainer.PlayerPosition + new Vector3(0f, 0.1f, 0f), diff, len, 25, ref hit, ref build);
                            if (hit == Vector3.Zero) //Why is this reversed?
                                continueDraw = true;
                        }
                    }
                    if (continueDraw)//p.ID != _P.playerMyId && p.Team == _P.playerTeam)
                    {
                        playerText = p.Handle;
                        if (p.Ping > 0)
                            playerText = "*** " + playerText + " ***";

                        p.SpriteModel.DrawText(_P.PlayerContainer.PlayerCamera.ViewMatrix,
                                               _P.PlayerContainer.PlayerCamera.ProjectionMatrix,
                                               p.Position - Vector3.UnitY * 1.5f,
                                               playerText, p.Team == PlayerTeam.Blue ? _P.blue : _P.red);//Defines.IM_BLUE : Defines.IM_RED);
                    }
                }
            }
        }
    }
}
