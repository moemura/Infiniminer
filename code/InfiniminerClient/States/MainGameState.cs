using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using StateMasher;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Lidgren.Network;

namespace Infiniminer.States
{
    public class MainGameState : State
    {
        const float MOVESPEED = 3.5f;
        const float GRAVITY = -8.0f;
        const float JUMPVELOCITY = 4.0f;
        const float CLIMBVELOCITY = 2.5f;
        const float DIEVELOCITY = 15.0f;

        string nextState = null;
        bool mouseInitialized = false;

        BlockEngine blockEngine = null;
        SkyplaneEngine skyplaneEngine = null;
        PlayerEngine playerEngine = null;
        InterfaceEngine interfaceEngine = null;
        ParticleEngine particleEngine = null;

        public override void OnEnter(string oldState)
        {
            Sm.IsMouseVisible = false;
        }

        public override void OnLeave(string newState)
        {
            P.ChatContainer.ChatEntryBuffer = "";
            P.ChatContainer.ChatMode = ChatMessageType.None;
        }

        public override string OnUpdate(GameTime gameTime, KeyboardState keyState, MouseState mouseState)
        {
            // Update network stuff.
            (Sm as InfiniminerGame).UpdateNetwork(gameTime);

            // Update the current screen effect.
            P.screenEffectCounter += gameTime.ElapsedGameTime.TotalSeconds;

            // Update engines.
            skyplaneEngine.Update(gameTime);
            playerEngine.Update(gameTime);
            interfaceEngine.Update(gameTime);
            particleEngine.Update(gameTime);

            // Count down the tool cooldown.
            if (P.PlayerContainer.PlayerToolCooldown > 0)
            {
                P.PlayerContainer.PlayerToolCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (P.PlayerContainer.PlayerToolCooldown <= 0)
                    P.PlayerContainer.PlayerToolCooldown = 0;
            }

            // Moving the mouse changes where we look.
            if (Sm.WindowHasFocus())
            {
                if (mouseInitialized)
                {
                    int dx = mouseState.X - Sm.GraphicsDevice.Viewport.Width / 2;
                    int dy = mouseState.Y - Sm.GraphicsDevice.Viewport.Height / 2;

                    if ((Sm as InfiniminerGame).InvertMouseYAxis)
                        dy = -dy;

                    P.PlayerContainer.PlayerCamera.Yaw -= dx * P.SettingsContainer.MouseSensitivity;
                    P.PlayerContainer.PlayerCamera.Pitch = (float)Math.Min(Math.PI * 0.49, Math.Max(-Math.PI * 0.49, P.PlayerContainer.PlayerCamera.Pitch - dy * P.SettingsContainer.MouseSensitivity));
                }
                else
                {
                    mouseInitialized = true;
                }
                Mouse.SetPosition(Sm.GraphicsDevice.Viewport.Width / 2, Sm.GraphicsDevice.Viewport.Height / 2);
            }
            else
                mouseInitialized = false;

            // Digging like a freaking terrier! Now for everyone!
            if (mouseInitialized && mouseState.LeftButton == ButtonState.Pressed && !P.PlayerContainer.PlayerDead && P.PlayerContainer.PlayerToolCooldown == 0 && P.PlayerContainer.PlayerTools[P.PlayerContainer.PlayerToolSelected] == PlayerTools.Pickaxe)
            {
                P.FirePickaxe();
                P.PlayerContainer.PlayerToolCooldown = P.GetToolCooldown(PlayerTools.Pickaxe) * (P.PlayerContainer.PlayerClass == PlayerClass.Miner ? 0.4f : 1.0f);
            }

            // Prospector radar stuff.
            if (!P.PlayerContainer.PlayerDead && P.PlayerContainer.PlayerToolCooldown == 0 && P.PlayerContainer.PlayerTools[P.PlayerContainer.PlayerToolSelected] == PlayerTools.ProspectingRadar)
            {
                float oldValue = P.PlayerContainer.RadarValue;
                P.ReadRadar(ref P.PlayerContainer.RadarDistance, ref P.PlayerContainer.RadarValue);
                if (P.PlayerContainer.RadarValue != oldValue)
                {
                    if (P.PlayerContainer.RadarValue == 200)
                        P.PlaySound(InfiniminerSound.RadarLow);
                    if (P.PlayerContainer.RadarValue == 1000)
                        P.PlaySound(InfiniminerSound.RadarHigh);
                }
            }

            // Update the player's position.
            if (!P.PlayerContainer.PlayerDead)
                UpdatePlayerPosition(gameTime, keyState);

            // Update the camera regardless of if we're alive or not.
            P.UpdateCamera(gameTime);

            return nextState;
        }

        private void UpdatePlayerPosition(GameTime gameTime, KeyboardState keyState)
        {
            // Double-speed move flag, set if we're on road.
            bool movingOnRoad = false;
            bool sprinting = false;

            // Apply "gravity".
            P.PlayerContainer.PlayerVelocity.Y += GRAVITY * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector3 footPosition = P.PlayerContainer.PlayerPosition + new Vector3(0f, -1.5f, 0f);
            Vector3 headPosition = P.PlayerContainer.PlayerPosition + new Vector3(0f, 0.1f, 0f);
            if (blockEngine.SolidAtPointForPlayer(footPosition) || blockEngine.SolidAtPointForPlayer(headPosition))
            {
                BlockType standingOnBlock = blockEngine.BlockAtPoint(footPosition);
                BlockType hittingHeadOnBlock = blockEngine.BlockAtPoint(headPosition);

                // If we"re hitting the ground with a high velocity, die!
                if (standingOnBlock != BlockType.None && P.PlayerContainer.PlayerVelocity.Y < 0)
                {
                    float fallDamage = Math.Abs(P.PlayerContainer.PlayerVelocity.Y) / DIEVELOCITY;
                    if (fallDamage >= 1)
                    {
                        P.PlaySoundForEveryone(InfiniminerSound.GroundHit, P.PlayerContainer.PlayerPosition);
                        P.KillPlayer(Defines.deathByFall);//"WAS KILLED BY GRAVITY!");
                        return;
                    }
                    else if (fallDamage > 0.5)
                    {
                        // Fall damage of 0.5 maps to a screenEffectCounter value of 2, meaning that the effect doesn't appear.
                        // Fall damage of 1.0 maps to a screenEffectCounter value of 0, making the effect very strong.
                        if (standingOnBlock != BlockType.Jump)
                        {
                            P.screenEffect = ScreenEffect.Fall;
                            P.screenEffectCounter = 2 - (fallDamage - 0.5) * 4;
                            P.PlaySoundForEveryone(InfiniminerSound.GroundHit, P.PlayerContainer.PlayerPosition);
                        }
                    }
                }

                // If the player has their head stuck in a block, push them down.
                if (blockEngine.SolidAtPointForPlayer(headPosition))
                {
                    int blockIn = (int)(headPosition.Y);
                    P.PlayerContainer.PlayerPosition.Y = (float)(blockIn - 0.15f);
                }

                // If the player is stuck in the ground, bring them out.
                // This happens because we're standing on a block at -1.5, but stuck in it at -1.4, so -1.45 is the sweet spot.
                if (blockEngine.SolidAtPointForPlayer(footPosition))
                {
                    int blockOn = (int)(footPosition.Y);
                    P.PlayerContainer.PlayerPosition.Y = (float)(blockOn + 1 + 1.45);
                }

                P.PlayerContainer.PlayerVelocity.Y = 0;

                // Logic for standing on a block.
                switch (standingOnBlock)
                {
                    case BlockType.Jump:
                        P.PlayerContainer.PlayerVelocity.Y = 2.5f * JUMPVELOCITY;
                        P.PlaySoundForEveryone(InfiniminerSound.Jumpblock, P.PlayerContainer.PlayerPosition);
                        break;

                    case BlockType.Road:
                        movingOnRoad = true;
                        break;

                    case BlockType.Lava:
                        P.KillPlayer(Defines.deathByLava);
                        return;
                }

                // Logic for bumping your head on a block.
                switch (hittingHeadOnBlock)
                {
                    case BlockType.Shock:
                        P.KillPlayer(Defines.deathByElec);
                        return;

                    case BlockType.Lava:
                        P.KillPlayer(Defines.deathByLava);
                        return;
                }
            }
            P.playerPosition += P.PlayerContainer.PlayerVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Death by falling off the map.
            if (P.PlayerContainer.PlayerPosition.Y < -30)
            {
                P.KillPlayer(Defines.deathByMiss);
                return;
            }

            // Pressing forward moves us in the direction we"re looking.
            Vector3 moveVector = Vector3.Zero;

            if (P.ChatContainer.ChatMode == ChatMessageType.None)
            {
                if ((Sm as InfiniminerGame).keyBinds.IsPressed(Buttons.Forward))//keyState.IsKeyDown(Keys.W))
                    moveVector += P.PlayerContainer.PlayerCamera.GetLookVector();
                if ((Sm as InfiniminerGame).keyBinds.IsPressed(Buttons.Backward))//keyState.IsKeyDown(Keys.S))
                    moveVector -= P.PlayerContainer.PlayerCamera.GetLookVector();
                if ((Sm as InfiniminerGame).keyBinds.IsPressed(Buttons.Right))//keyState.IsKeyDown(Keys.D))
                    moveVector += P.PlayerContainer.PlayerCamera.GetRightVector();
                if ((Sm as InfiniminerGame).keyBinds.IsPressed(Buttons.Left))//keyState.IsKeyDown(Keys.A))
                    moveVector -= P.PlayerContainer.PlayerCamera.GetRightVector();
                //Sprinting
                if ((Sm as InfiniminerGame).keyBinds.IsPressed(Buttons.Sprint))//keyState.IsKeyDown(Keys.LeftShift) || keyState.IsKeyDown(Keys.RightShift))
                    sprinting = true;
            }

            if (moveVector.X != 0 || moveVector.Z != 0)
            {
                // "Flatten" the movement vector so that we don"t move up/down.
                moveVector.Y = 0;
                moveVector.Normalize();
                moveVector *= MOVESPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (movingOnRoad)
                    moveVector *= 2;
                // Sprinting doubles speed, even if already on road
                if (sprinting)
                    moveVector *= 1.5f;

                // Attempt to move, doing collision stuff.
                if (TryToMoveTo(moveVector, gameTime)) { }
                else if (!TryToMoveTo(new Vector3(0, 0, moveVector.Z), gameTime)) { }
                else if (!TryToMoveTo(new Vector3(moveVector.X, 0, 0), gameTime)) { }
            }
        }

        private bool TryToMoveTo(Vector3 moveVector, GameTime gameTime)
        {
            // Build a "test vector" that is a little longer than the move vector.
            float moveLength = moveVector.Length();
            Vector3 testVector = moveVector;
            testVector.Normalize();
            testVector = testVector * (moveLength + 0.1f);

            // Apply this test vector.
            Vector3 movePosition = P.PlayerContainer.PlayerPosition + testVector;
            Vector3 midBodyPoint = movePosition + new Vector3(0, -0.7f, 0);
            Vector3 lowerBodyPoint = movePosition + new Vector3(0, -1.4f, 0);

            if (!blockEngine.SolidAtPointForPlayer(movePosition) && !blockEngine.SolidAtPointForPlayer(lowerBodyPoint) && !blockEngine.SolidAtPointForPlayer(midBodyPoint))
            {
                P.PlayerContainer.PlayerPosition = P.PlayerContainer.PlayerPosition + moveVector;
                return true;
            }

            // It's solid there, so while we can't move we have officially collided with it.
            BlockType lowerBlock = blockEngine.BlockAtPoint(lowerBodyPoint);
            BlockType midBlock = blockEngine.BlockAtPoint(midBodyPoint);
            BlockType upperBlock = blockEngine.BlockAtPoint(movePosition);

            // It's solid there, so see if it's a lava block. If so, touching it will kill us!
            if (upperBlock == BlockType.Lava || lowerBlock == BlockType.Lava || midBlock == BlockType.Lava)
            {
                P.KillPlayer(Defines.deathByLava);
                return true;
            }

            // If it's a ladder, move up.
            if (upperBlock == BlockType.Ladder || lowerBlock == BlockType.Ladder || midBlock == BlockType.Ladder)
            {
                P.PlayerContainer.PlayerVelocity.Y = CLIMBVELOCITY;
                Vector3 footPosition = P.PlayerContainer.PlayerPosition + new Vector3(0f, -1.5f, 0f);
                if (blockEngine.SolidAtPointForPlayer(footPosition))
                    P.PlayerContainer.PlayerPosition.Y += 0.1f;
                return true;
            }

            return false;
        }

        public override void OnRenderAtEnter(GraphicsDevice graphicsDevice)
        {

        }

        public override void OnRenderAtUpdate(GraphicsDevice graphicsDevice, GameTime gameTime)
        {
            skyplaneEngine.Render(graphicsDevice);
            particleEngine.Render(graphicsDevice);
            playerEngine.Render(graphicsDevice);
            blockEngine.Render(graphicsDevice, gameTime);
            playerEngine.RenderPlayerNames(graphicsDevice);
            interfaceEngine.Render(graphicsDevice);

            Sm.Window.Title = "Infiniminer";
        }

        DateTime startChat = DateTime.Now;
        public override void OnCharEntered(EventInput.CharacterEventArgs e)
        {
            if ((int)e.Character < 32 || (int)e.Character > 126) //From space to tilde
                return; //Do nothing
            if (P.ChatContainer.ChatMode != ChatMessageType.None)
            {
                //Chat delay to avoid entering the "start chat" key, an unfortunate side effect of the new key bind system
                TimeSpan diff = DateTime.Now - startChat;
                if (diff.Milliseconds >= 2)
                    if (!(Keyboard.GetState().IsKeyDown(Keys.LeftControl) || Keyboard.GetState().IsKeyDown(Keys.RightControl)))
                    {
                        P.ChatContainer.ChatEntryBuffer += e.Character;
                    }
            }
        }

        private void HandleInput(Buttons input)
        {
            switch (input)
            {
                case Buttons.Fire:
                    if (P.PlayerContainer.PlayerToolCooldown <= 0)
                    {
                        switch (P.PlayerContainer.PlayerTools[P.PlayerContainer.PlayerToolSelected])
                        {
                            // Disabled as everyone speed-mines now.
                            //case PlayerTools.Pickaxe:
                            //    if (_P.playerClass != PlayerClass.Miner)
                            //        _P.FirePickaxe();
                            //    break;

                            case PlayerTools.ConstructionGun:
                                P.FireConstructionGun(P.PlayerContainer.PlayerBlocks[P.PlayerContainer.PlayerBlockSelected]);//, !(button == MouseButton.LeftButton));//_P.FireConstructionGun(_P.playerBlocks[_P.playerBlockSelected]);
                                break;

                            case PlayerTools.DeconstructionGun:
                                P.FireDeconstructionGun();
                                break;

                            case PlayerTools.Detonator:
                                P.PlaySound(InfiniminerSound.ClickHigh);
                                P.FireDetonator();
                                break;

                            case PlayerTools.ProspectingRadar:
                                P.FireRadar();
                                break;
                        }
                    }
                    break;
                case Buttons.Jump:
                    {
                        Vector3 footPosition = P.PlayerContainer.PlayerPosition + new Vector3(0f, -1.5f, 0f);
                        if (blockEngine.SolidAtPointForPlayer(footPosition) && P.PlayerContainer.PlayerVelocity.Y == 0)
                        {
                            P.PlayerContainer.PlayerVelocity.Y = JUMPVELOCITY;
                            float amountBelowSurface = ((ushort)footPosition.Y) + 1 - footPosition.Y;
                            P.PlayerContainer.PlayerPosition.Y += amountBelowSurface + 0.01f;
                        }
                    }
                    break;
                    //TODO: OPTIMIZE PLEASE
                case Buttons.ToolUp:
                    P.PlaySound(InfiniminerSound.ClickLow);
                    P.PlayerContainer.PlayerToolSelected += 1;
                    if (P.PlayerContainer.PlayerToolSelected >= P.PlayerContainer.PlayerTools.Length)
                        P.PlayerContainer.PlayerToolSelected = 0;
                    break;
                case Buttons.ToolDown:
                    P.PlaySound(InfiniminerSound.ClickLow);
                    P.PlayerContainer.PlayerToolSelected -= 1;
                    if (P.PlayerContainer.PlayerToolSelected < 0)
                        P.PlayerContainer.PlayerToolSelected = P.PlayerContainer.PlayerTools.Length;
                    break;
                case Buttons.Tool1:
                    P.PlayerContainer.PlayerToolSelected = 0;
                    P.PlaySound(InfiniminerSound.ClickLow);
                    if (P.PlayerContainer.PlayerToolSelected >= P.PlayerContainer.PlayerTools.Length)
                        P.PlayerContainer.PlayerToolSelected = P.PlayerContainer.PlayerTools.Length - 1;
                    break;
                case Buttons.Tool2:
                    P.PlayerContainer.PlayerToolSelected = 1;
                    P.PlaySound(InfiniminerSound.ClickLow);
                    if (P.PlayerContainer.PlayerToolSelected >= P.PlayerContainer.PlayerTools.Length)
                        P.PlayerContainer.PlayerToolSelected = P.PlayerContainer.PlayerTools.Length - 1;
                    break;
                case Buttons.Tool3:
                    P.PlayerContainer.PlayerToolSelected = 2;
                    P.PlaySound(InfiniminerSound.ClickLow);
                    if (P.PlayerContainer.PlayerToolSelected >= P.PlayerContainer.PlayerTools.Length)
                        P.PlayerContainer.PlayerToolSelected = P.PlayerContainer.PlayerTools.Length - 1;
                    break;
                case Buttons.Tool4:
                    P.PlayerContainer.PlayerToolSelected = 3;
                    P.PlaySound(InfiniminerSound.ClickLow);
                    if (P.PlayerContainer.PlayerToolSelected >= P.PlayerContainer.PlayerTools.Length)
                        P.PlayerContainer.PlayerToolSelected = P.PlayerContainer.PlayerTools.Length - 1;
                    break;
                case Buttons.Tool5:
                    P.PlayerContainer.PlayerToolSelected = 4;
                    P.PlaySound(InfiniminerSound.ClickLow);
                    if (P.PlayerContainer.PlayerToolSelected >= P.PlayerContainer.PlayerTools.Length)
                        P.PlayerContainer.PlayerToolSelected = P.PlayerContainer.PlayerTools.Length - 1;
                    break;
                case Buttons.BlockUp:
                    if (P.PlayerContainer.PlayerTools[P.PlayerContainer.PlayerToolSelected] == PlayerTools.ConstructionGun)
                    {
                        P.PlaySound(InfiniminerSound.ClickLow);
                        P.PlayerContainer.PlayerBlockSelected += 1;
                        if (P.PlayerContainer.PlayerBlockSelected >= P.PlayerContainer.PlayerBlocks.Length)
                            P.PlayerContainer.PlayerBlockSelected = 0;
                    }
                    break;
                case Buttons.BlockDown:
                    if (P.PlayerContainer.PlayerTools[P.PlayerContainer.PlayerToolSelected] == PlayerTools.ConstructionGun)
                    {
                        P.PlaySound(InfiniminerSound.ClickLow);
                        P.PlayerContainer.PlayerBlockSelected -= 1;
                        if (P.PlayerContainer.PlayerBlockSelected < 0)
                            P.PlayerContainer.PlayerBlockSelected = P.PlayerContainer.PlayerBlocks.Length-1;
                    }
                    break;
                case Buttons.Deposit:
                    if (P.AtBankTerminal())
                    {
                        P.DepositOre();
                        P.PlaySound(InfiniminerSound.ClickHigh);
                    }
                    break;
                case Buttons.Withdraw:
                    if (P.AtBankTerminal())
                    {
                        P.WithdrawOre();
                        P.PlaySound(InfiniminerSound.ClickHigh);
                    }
                    break;
                case Buttons.Ping:
                    {
                        NetOutgoingMessage msgBuffer = P.netClient.CreateMessage();
                        msgBuffer.Write((byte)InfiniminerMessage.PlayerPing);
                        msgBuffer.Write(P.PlayerContainer.PlayerMyId);
                        P.netClient.SendMessage(msgBuffer, NetDeliveryMethod.ReliableUnordered);
                    }
                    break;
                case Buttons.ChangeClass:
                    nextState = "Infiniminer.States.ClassSelectionState";
                    break;
                case Buttons.ChangeTeam:
                    nextState = "Infiniminer.States.TeamSelectionState";
                    break;
                case Buttons.SayAll:
                    P.ChatContainer.ChatMode = ChatMessageType.SayAll;
                    startChat = DateTime.Now;
                    break;
                case Buttons.SayTeam:
                    P.ChatContainer.ChatMode = P.PlayerContainer.PlayerTeam == PlayerTeam.Red ? ChatMessageType.SayRedTeam : ChatMessageType.SayBlueTeam;
                    startChat = DateTime.Now;
                    break;
            }
        }

        public override void OnKeyDown(Keys key)
        {
            // Exit!
            if (key == Keys.Y && Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                P.netClient.Disconnect("Client disconnected.");
                nextState = "Infiniminer.States.ServerBrowserState";
            }

            // Pixelcide!
            if (key == Keys.K && Keyboard.GetState().IsKeyDown(Keys.Escape) && !P.playerDead)
            {
                P.KillPlayer(Defines.deathBySuic);//"HAS COMMMITTED PIXELCIDE!");
                return;
            }

            //Map saving!
            if ((Keyboard.GetState().IsKeyDown(Keys.LeftControl) || Keyboard.GetState().IsKeyDown(Keys.RightControl)) && key == Keys.S)
            {
                P.SaveMap();
                return;
            }

            if (P.ChatContainer.ChatMode != ChatMessageType.None)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftControl) || Keyboard.GetState().IsKeyDown(Keys.RightControl))
                {
                    if (key == Keys.V)
                    {
                        P.ChatContainer.ChatEntryBuffer += System.Windows.Forms.Clipboard.GetText();
                        return;
                    }
                    else if (key == Keys.C)
                    {
                        System.Windows.Forms.Clipboard.SetText(P.ChatContainer.ChatEntryBuffer);
                        return;
                    }
                    else if (key == Keys.X)
                    {
                        System.Windows.Forms.Clipboard.SetText(P.ChatContainer.ChatEntryBuffer);
                        P.ChatContainer.ChatEntryBuffer = "";
                        return;
                    }
                }
                // Put the characters in the chat buffer.
                if (key == Keys.Enter)
                {
                    // If we have an actual message to send, fire it off at the server.
                    if (P.ChatContainer.ChatEntryBuffer.Length > 0)
                    {
                        if (P.netClient.ConnectionStatus == NetConnectionStatus.Connected)
                        {
                            NetOutgoingMessage msgBuffer = P.netClient.CreateMessage();
                            msgBuffer.Write((byte)InfiniminerMessage.ChatMessage);
                            msgBuffer.Write((byte)P.ChatContainer.ChatMode);
                            msgBuffer.Write(P.ChatContainer.ChatEntryBuffer);
                            P.netClient.SendMessage(msgBuffer, NetDeliveryMethod.ReliableOrdered);
                        }
                        else
                        {
                            P.addChatMessage("Not connected to server.", ChatMessageType.SayAll, 10);
                        }
                    }

                    P.ChatContainer.ChatEntryBuffer = "";
                    P.ChatContainer.ChatMode = ChatMessageType.None;
                }
                else if (key == Keys.Back)
                {
                    if (P.ChatContainer.ChatEntryBuffer.Length > 0)
                        P.ChatContainer.ChatEntryBuffer = P.ChatContainer.ChatEntryBuffer.Substring(0, P.ChatContainer.ChatEntryBuffer.Length - 1);
                }
                else if (key == Keys.Escape)
                {
                    P.ChatContainer.ChatEntryBuffer = "";
                    P.ChatContainer.ChatMode = ChatMessageType.None;
                }
                return;
            }else if (!P.PlayerContainer.PlayerDead)
                HandleInput((Sm as InfiniminerGame).keyBinds.GetBound(key));
            
        }

        public override void OnKeyUp(Keys key)
        {

        }

        public override void OnMouseDown(MouseButton button, int x, int y)
        {
            // If we're dead, come back to life.
            if (P.PlayerContainer.PlayerDead && P.screenEffectCounter > 2)
                P.RespawnPlayer();
            else if (!P.PlayerContainer.PlayerDead)
                HandleInput((Sm as InfiniminerGame).keyBinds.GetBound(button));
        }

        public override void OnMouseUp(MouseButton button, int x, int y)
        {

        }

        public override void OnMouseScroll(int scrollDelta)
        {
            if (P.PlayerContainer.PlayerDead)
                return;
            else
            {
                if (scrollDelta >= 120)
                {
                    Console.WriteLine("Handling input for scroll up...");
                    HandleInput((Sm as InfiniminerGame).keyBinds.GetBound(MouseButton.WheelUp));//.keyBinds.GetBound(button));
                }
                else if (scrollDelta <= -120)
                {
                    HandleInput((Sm as InfiniminerGame).keyBinds.GetBound(MouseButton.WheelDown));
                }
            }
        }
    }
}
