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
using System.IO;
using InfiniminerShared;
using Plexiglass.Client;
using Plexiglass.Client.Engine;
using Plexiglass.Exceptions;
using Lidgren.Network;

namespace Infiniminer
{
    public class PropertyBag : IPropertyBag
    {
        // Game engines.
        private Dictionary<string, Tuple<IEngine, Type>> Engines;

        // Network stuff.
        public NetClient NetClient { get; set; }
        public Dictionary<uint, Player> PlayerList { get; set; }
        public bool[,] MapLoadProgress { get; set; }
        public string serverName = "";

        //Input stuff.
        public KeyBindHandler keyBinds = null;

        // Player variables.
        public PlayerContainer PlayerContainer { get; set; }
        public SettingsContainer SettingsContainer { get; set; }
        // Team variables.
        public TeamContainer TeamContainer { get; set; }

        // Screen effect stuff.
        private Random randGen = new Random();

        //Team colour stuff
        public bool customColours = false;
        public Color red = Defines.IM_RED;
        public Color blue = Defines.IM_BLUE;
        public string redName = "Red";
        public string blueName = "Blue";

        // Sound stuff.
        public Dictionary<InfiniminerSound, SoundEffect> soundList = new Dictionary<InfiniminerSound, SoundEffect>();

        // Chat stuff.
        public ChatContainer ChatContainer { get; set; }

        public PropertyBag(InfiniminerGame gameInstance)
        {
            // Initialize our network device.
            NetPeerConfiguration netConfig = new NetPeerConfiguration("InfiniminerPlus");
            netConfig.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            netConfig.EnableMessageType(NetIncomingMessageType.ErrorMessage);
            netConfig.EnableMessageType(NetIncomingMessageType.DebugMessage);
            netConfig.EnableMessageType(NetIncomingMessageType.WarningMessage);
            netClient = new NetClient(netConfig);
            //netClient.SimulatedMinimumLatency = 0.1f;
            //netClient.SimulatedLatencyVariance = 0.05f;
            //netClient.SimulatedLoss = 0.1f;
            //netClient.SimulatedDuplicates = 0.05f;
            netClient.Start();

            // Initialize engines.
            Engines = new Dictionary<string, Tuple<IEngine, Type>>();

            RegisterEngine(new BlockEngine(gameInstance), "blockEngine");
            RegisterEngine(new InterfaceEngine(gameInstance), "interfaceEngine");
            RegisterEngine(new PlayerEngine(gameInstance), "playerEngine");
            RegisterEngine(new SkyplaneEngine(gameInstance), "skyplaneEngine");
            RegisterEngine(new ParticleEngine(gameInstance), "particleEngine");

            PlayerContainer = new PlayerContainer();
            SettingsContainer = new SettingsContainer();
            ChatContainer = new ChatContainer();
            TeamContainer = new TeamContainer();
            TeamContainer.BeaconList = new Dictionary<Vector3, Beacon>();
        
            PlayerList = new Dictionary<uint, Player>();

            // Create a camera.
            PlayerContainer.PlayerCamera = new Camera(gameInstance.GraphicsDevice);
            UpdateCamera();

            // Load sounds.
            if (!gameInstance.NoSound)
            {
                soundList[InfiniminerSound.DigDirt] = gameInstance.Content.Load<SoundEffect>("sounds/dig-dirt");
                soundList[InfiniminerSound.DigMetal] = gameInstance.Content.Load<SoundEffect>("sounds/dig-metal");
                soundList[InfiniminerSound.Ping] = gameInstance.Content.Load<SoundEffect>("sounds/ping");
                soundList[InfiniminerSound.ConstructionGun] = gameInstance.Content.Load<SoundEffect>("sounds/build");
                soundList[InfiniminerSound.Death] = gameInstance.Content.Load<SoundEffect>("sounds/death");
                soundList[InfiniminerSound.CashDeposit] = gameInstance.Content.Load<SoundEffect>("sounds/cash");
                soundList[InfiniminerSound.ClickHigh] = gameInstance.Content.Load<SoundEffect>("sounds/click-loud");
                soundList[InfiniminerSound.ClickLow] = gameInstance.Content.Load<SoundEffect>("sounds/click-quiet");
                soundList[InfiniminerSound.GroundHit] = gameInstance.Content.Load<SoundEffect>("sounds/hitground");
                soundList[InfiniminerSound.Teleporter] = gameInstance.Content.Load<SoundEffect>("sounds/teleport");
                soundList[InfiniminerSound.Jumpblock] = gameInstance.Content.Load<SoundEffect>("sounds/jumpblock");
                soundList[InfiniminerSound.Explosion] = gameInstance.Content.Load<SoundEffect>("sounds/explosion");
                soundList[InfiniminerSound.RadarHigh] = gameInstance.Content.Load<SoundEffect>("sounds/radar-high");
                soundList[InfiniminerSound.RadarLow] = gameInstance.Content.Load<SoundEffect>("sounds/radar-low");
                soundList[InfiniminerSound.RadarSwitch] = gameInstance.Content.Load<SoundEffect>("sounds/switch");
            }
        }

        // TODO: Proper block class with Team variant
        public PlayerTeam TeamFromBlock(BlockType bt)
        {
            switch (bt)
            {
                case BlockType.TransBlue:
                case BlockType.SolidBlue:
                case BlockType.BeaconBlue:
                case BlockType.BankBlue:
                    return PlayerTeam.Blue;
                case BlockType.TransRed:
                case BlockType.SolidRed:
                case BlockType.BeaconRed:
                case BlockType.BankRed:
                    return PlayerTeam.Red;
                default:
                    return PlayerTeam.None;
            }
        }

        public void SaveMap()
        {
            string filename = "saved_" + serverName.Replace(" ","") + "_" + (UInt64)DateTime.Now.ToBinary() + ".lvl";
            FileStream fs = new FileStream(filename, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            for (int x = 0; x < 64; x++)
                for (int y = 0; y < 64; y++)
                    for (int z = 0; z < 64; z++)
                        sw.WriteLine((byte)GetEngine<BlockEngine>("blockEngine").blockList[x, y, z] + "," + (byte)TeamFromBlock(GetEngine<BlockEngine>("blockEngine").blockList[x, y, z]));//(byte)blockEngine.blockCreatorTeam[x, y, z]);
            sw.Close();
            fs.Close();
            addChatMessage("Map saved to " + filename, ChatMessageType.SayAll, 10f);//DateTime.Now.ToUniversalTime());
        }

        public void KillPlayer(string deathMessage)
        {
            if (netClient.ConnectionStatus != NetConnectionStatus.Connected)
                return;

            PlaySound(InfiniminerSound.Death);
            PlayerContainer.PlayerPosition = new Vector3(randGen.Next(2, 62), 66, randGen.Next(2, 62));
            PlayerContainer.PlayerVelocity = Vector3.Zero;
            PlayerContainer.PlayerDead = true;
            screenEffect = ScreenEffect.Death;
            screenEffectCounter = 0;

            NetOutgoingMessage msgBuffer = netClient.CreateMessage();
            msgBuffer.Write((byte)InfiniminerMessage.PlayerDead);
            msgBuffer.Write(deathMessage);
            netClient.SendMessage(msgBuffer, NetDeliveryMethod.ReliableUnordered);
        }

        public void RespawnPlayer()
        {
            if (netClient.ConnectionStatus != NetConnectionStatus.Connected)
                return;

            PlayerContainer.PlayerDead = false;

            // Respawn a few blocks above a safe position above altitude 0.
            bool positionFound = false;

            // Try 20 times; use a potentially invalid position if we fail.
            for (int i = 0; i < 20; i++)
            {
                // Pick a random starting point.
                Vector3 startPos = new Vector3(randGen.Next(2, 62), 63, randGen.Next(2, 62));

                // See if this is a safe place to drop.
                for (startPos.Y = 63; startPos.Y >= 54; startPos.Y--)
                {
                    BlockType blockType = GetEngine<BlockEngine>("blockEngine").BlockAtPoint(startPos);
                    if (blockType == BlockType.Lava)
                        break;
                    else if (blockType != BlockType.None)
                    {
                        // We have found a valid place to spawn, so spawn a few above it.
                        PlayerContainer.PlayerPosition = startPos + Vector3.UnitY * 5;
                        positionFound = true;
                        break;
                    }
                }

                // If we found a position, no need to try anymore!
                if (positionFound)
                    break;
            }

            // If we failed to find a spawn point, drop randomly.
            if (!positionFound)
                PlayerContainer.PlayerPosition = new Vector3(randGen.Next(2, 62), 66, randGen.Next(2, 62));

            // Drop the player on the middle of the block, not at the corner.
            PlayerContainer.PlayerPosition += new Vector3(0.5f, 0, 0.5f);

            // Zero out velocity and reset camera and screen effects.
            PlayerContainer.PlayerVelocity = Vector3.Zero;
            screenEffect = ScreenEffect.None;
            screenEffectCounter = 0;
            UpdateCamera();

            // Tell the server we have respawned.
            NetOutgoingMessage msgBuffer = netClient.CreateMessage();
            msgBuffer.Write((byte)InfiniminerMessage.PlayerAlive);
            netClient.SendMessage(msgBuffer, NetDeliveryMethod.ReliableUnordered);
        }

        public void PlaySound(InfiniminerSound sound)
        {
            if (soundList.Count == 0)
                return;

            soundList[sound].Play(SettingsContainer.VolumeLevel, 0.0f, 0.0f);
        }

        public void PlaySound(InfiniminerSound sound, Vector3 position)
        {
            if (soundList.Count == 0)
                return;

            float distance = (position - PlayerContainer.PlayerPosition).Length();
            float volume = Math.Max(0, 10 - distance) / 10.0f * SettingsContainer.VolumeLevel;
            volume = volume > 1.0f ? 1.0f : volume < 0.0f ? 0.0f : volume;
            soundList[sound].Play(volume, 0.0f, 0.0f);
        }

        public void PlaySoundForEveryone(InfiniminerSound sound, Vector3 position)
        {
            if (netClient.ConnectionStatus != NetConnectionStatus.Connected)
                return;

            // The PlaySound message can be used to instruct the server to have all clients play a directional sound.
            NetOutgoingMessage msgBuffer = netClient.CreateMessage();
            msgBuffer.Write((byte)InfiniminerMessage.PlaySound);
            msgBuffer.Write((byte)sound);
            msgBuffer.Write(position);
            netClient.SendMessage(msgBuffer, NetDeliveryMethod.ReliableUnordered);
        }

        public void addChatMessage(string chatString, ChatMessageType chatType, float timestamp)
        {
            string[] text = chatString.Split(' ');
            string textFull = "";
            string textLine = "";
            int newlines = 0;

            float curWidth = 0;
            for (int i = 0; i < text.Length; i++)
            {//each(string part in text){
                string part = text[i];
                if (i != text.Length - 1)
                    part += ' '; //Correct for lost spaces
                float incr = GetEngine<InterfaceEngine>("interfaceEngine").uiFont.MeasureString(part).X;
                curWidth += incr;
                if (curWidth > 1024 - 64) //Assume default resolution, unfortunately
                {
                    if (textLine.IndexOf(' ') < 0)
                    {
                        curWidth = 0;
                        textFull = textFull + "\n" + textLine;
                        textLine = "";
                    }
                    else
                    {
                        curWidth = incr;
                        textFull = textFull + "\n" + textLine;
                        textLine = part;
                    }
                    newlines++;
                }
                else
                {
                    textLine = textLine + part;
                }
            }
            if (textLine != "")
            {
                textFull += "\n" + textLine;
                newlines++;
            }

            if (textFull == "")
                textFull = chatString;

            ChatMessage chatMsg = new ChatMessage(textFull, chatType, 10,newlines);
            
            ChatContainer.ChatBuffer.Insert(0, chatMsg);
            ChatContainer.ChatFullBuffer.Insert(0, chatMsg);
            PlaySound(InfiniminerSound.ClickLow);
        }

        //public void Teleport()
        //{
        //    float x = (float)randGen.NextDouble() * 74 - 5;
        //    float z = (float)randGen.NextDouble() * 74 - 5;
        //    //playerPosition = playerHomeBlock + new Vector3(0.5f, 3, 0.5f);
        //    playerPosition = new Vector3(x, 74, z);
        //    screenEffect = ScreenEffect.Teleport;
        //    screenEffectCounter = 0;
        //    UpdateCamera();
        //}

        // Version used during updates.
        public void UpdateCamera(GameTime gameTime)
        {
            // If we have a gameTime object, apply screen jitter.
            if (screenEffect == ScreenEffect.Explosion)
            {
                if (gameTime != null)
                {
                    screenEffectCounter += gameTime.ElapsedGameTime.TotalSeconds;
                    // For 0 to 2, shake the camera.
                    if (screenEffectCounter < 2)
                    {
                        Vector3 newPosition = PlayerContainer.PlayerCamera.Position;
                        newPosition.X += (float)(2 - screenEffectCounter) * (float)(randGen.NextDouble() - 0.5) * 0.5f;
                        newPosition.Y += (float)(2 - screenEffectCounter) * (float)(randGen.NextDouble() - 0.5) * 0.5f;
                        newPosition.Z += (float)(2 - screenEffectCounter) * (float)(randGen.NextDouble() - 0.5) * 0.5f;
                        if (!GetEngine<BlockEngine>("blockEngine").SolidAtPointForPlayer(newPosition) && (newPosition - PlayerContainer.PlayerPosition).Length() < 0.7f)
                            PlayerContainer.PlayerCamera.Position = newPosition;
                    }
                    // For 2 to 3, move the camera back.
                    else if (screenEffectCounter < 3)
                    {
                        Vector3 lerpVector = PlayerContainer.PlayerPosition - PlayerContainer.PlayerCamera.Position;
                        PlayerContainer.PlayerCamera.Position += 0.5f * lerpVector;
                    }
                    else
                    {
                        screenEffect = ScreenEffect.None;
                        screenEffectCounter = 0;
                        PlayerContainer.PlayerCamera.Position = PlayerContainer.PlayerPosition;
                    }
                }
            }
            else
            {
                PlayerContainer.PlayerCamera.Position = PlayerContainer.PlayerPosition;
            }
            PlayerContainer.PlayerCamera.Update();
        }

        public void UpdateCamera()
        {
            UpdateCamera(null);
        }

        public void DepositLoot()
        {
            if (netClient.ConnectionStatus != NetConnectionStatus.Connected)
                return;

            NetOutgoingMessage msgBuffer = netClient.CreateMessage();
            msgBuffer.Write((byte)InfiniminerMessage.DepositCash);
            netClient.SendMessage(msgBuffer, NetDeliveryMethod.ReliableUnordered);
        }

        public void DepositOre()
        {
            if (netClient.ConnectionStatus != NetConnectionStatus.Connected)
                return;

            NetOutgoingMessage msgBuffer = netClient.CreateMessage();
            msgBuffer.Write((byte)InfiniminerMessage.DepositOre);
            netClient.SendMessage(msgBuffer, NetDeliveryMethod.ReliableUnordered);
        }

        public void WithdrawOre()
        {
            if (netClient.ConnectionStatus != NetConnectionStatus.Connected)
                return;

            NetOutgoingMessage msgBuffer = netClient.CreateMessage();
            msgBuffer.Write((byte)InfiniminerMessage.WithdrawOre);
            netClient.SendMessage(msgBuffer, NetDeliveryMethod.ReliableUnordered);
        }

        public void SetPlayerTeam(PlayerTeam playerTeam)
        {
            if (netClient.ConnectionStatus != NetConnectionStatus.Connected)
                return;

            PlayerContainer.PlayerTeam = playerTeam;

            NetOutgoingMessage msgBuffer = netClient.CreateMessage();
            msgBuffer.Write((byte)InfiniminerMessage.PlayerSetTeam);
            msgBuffer.Write((byte)playerTeam);
            netClient.SendMessage(msgBuffer, NetDeliveryMethod.ReliableUnordered);
        }

        public bool allWeps = false; //Needs to be true on sandbox servers, though that requires a server mod

        public void equipWeps()
        {
            PlayerContainer.PlayerToolSelected = 0;
            PlayerContainer.PlayerBlockSelected = 0;
            // TODO: PlayerClass to interface/class definitions.
            if (allWeps)
            {
                PlayerContainer.PlayerTools = new PlayerTools[5] { PlayerTools.Pickaxe,
                PlayerTools.ConstructionGun,
                PlayerTools.DeconstructionGun,
                PlayerTools.ProspectingRadar,
                PlayerTools.Detonator };

                PlayerContainer.PlayerBlocks = new BlockType[12] {   PlayerContainer.PlayerTeam == PlayerTeam.Red ? BlockType.SolidRed : BlockType.SolidBlue,
                                             PlayerContainer.PlayerTeam == PlayerTeam.Red ? BlockType.TransRed : BlockType.TransBlue,
                                             BlockType.Road,
                                             BlockType.Ladder,
                                             BlockType.Jump,
                                             BlockType.Shock,
                                             PlayerContainer.PlayerTeam == PlayerTeam.Red ? BlockType.BeaconRed : BlockType.BeaconBlue,
                                             PlayerContainer.PlayerTeam == PlayerTeam.Red ? BlockType.BankRed : BlockType.BankBlue,
                                             BlockType.Explosive,
                                             BlockType.Road,
                                             BlockType.Lava,
                                             BlockType.Dirt };
            }
            else
            {
                switch (PlayerContainer.PlayerClass)
                {
                    case PlayerClass.Prospector:
                        PlayerContainer.PlayerTools = new PlayerTools[3] {  PlayerTools.Pickaxe,
                                                        PlayerTools.ConstructionGun,
                                                        PlayerTools.ProspectingRadar     };
                        PlayerContainer.PlayerBlocks = new BlockType[4] {   PlayerContainer.PlayerTeam == PlayerTeam.Red ? BlockType.SolidRed : BlockType.SolidBlue,
                                                        PlayerContainer.PlayerTeam == PlayerTeam.Red ? BlockType.TransRed : BlockType.TransBlue,
                                                        PlayerContainer.PlayerTeam == PlayerTeam.Red ? BlockType.BeaconRed : BlockType.BeaconBlue,
                                                        BlockType.Ladder    };
                        break;

                    case PlayerClass.Miner:
                        PlayerContainer.PlayerTools = new PlayerTools[2] {  PlayerTools.Pickaxe,
                                                        PlayerTools.ConstructionGun     };
                        PlayerContainer.PlayerBlocks = new BlockType[3] {   PlayerContainer.PlayerTeam == PlayerTeam.Red ? BlockType.SolidRed : BlockType.SolidBlue,
                                                        PlayerContainer.PlayerTeam == PlayerTeam.Red ? BlockType.TransRed : BlockType.TransBlue,
                                                        BlockType.Ladder    };
                        break;

                    case PlayerClass.Engineer:
                        PlayerContainer.PlayerTools = new PlayerTools[3] {  PlayerTools.Pickaxe,
                                                        PlayerTools.ConstructionGun,     
                                                        PlayerTools.DeconstructionGun   };
                        PlayerContainer.PlayerBlocks = new BlockType[9] {   PlayerContainer.PlayerTeam == PlayerTeam.Red ? BlockType.SolidRed : BlockType.SolidBlue,
                                                        BlockType.TransRed,
                                                        BlockType.TransBlue, //playerTeam == PlayerTeam.Red ? BlockType.TransRed : BlockType.TransBlue, //Only need one entry due to right-click
                                                        BlockType.Road,
                                                        BlockType.Ladder,
                                                        BlockType.Jump,
                                                        BlockType.Shock,
                                                        PlayerContainer.PlayerTeam == PlayerTeam.Red ? BlockType.BeaconRed : BlockType.BeaconBlue,
                                                        PlayerContainer.PlayerTeam == PlayerTeam.Red ? BlockType.BankRed : BlockType.BankBlue  };
                        break;

                    case PlayerClass.Sapper:
                        PlayerContainer.PlayerTools = new PlayerTools[3] {  PlayerTools.Pickaxe,
                                                        PlayerTools.ConstructionGun,
                                                        PlayerTools.Detonator     };
                        PlayerContainer.PlayerBlocks = new BlockType[4] {   PlayerContainer.PlayerTeam == PlayerTeam.Red ? BlockType.SolidRed : BlockType.SolidBlue,
                                                        PlayerContainer.PlayerTeam == PlayerTeam.Red ? BlockType.TransRed : BlockType.TransBlue,
                                                        BlockType.Ladder,
                                                        BlockType.Explosive     };
                        break;
                }
            }
        }

        public void SetPlayerClass(PlayerClass playerClass)
        {
            if (this.PlayerContainer.PlayerClass != playerClass)
            {
                if (netClient.ConnectionStatus != NetConnectionStatus.Connected)
                    return;

                this.PlayerContainer.PlayerClass = playerClass;

                NetOutgoingMessage msgBuffer = netClient.CreateMessage();
                msgBuffer.Write((byte)InfiniminerMessage.SelectClass);
                msgBuffer.Write((byte)playerClass);
                netClient.SendMessage(msgBuffer, NetDeliveryMethod.ReliableUnordered);

                PlayerContainer.PlayerToolSelected = 0;
                PlayerContainer.PlayerBlockSelected = 0;

                equipWeps();
            }
            this.KillPlayer("");
            this.RespawnPlayer();
        }

        public void FireRadar()
        {
            if (netClient.ConnectionStatus != NetConnectionStatus.Connected)
                return;

            PlayerContainer.PlayerToolCooldown = GetToolCooldown(PlayerTools.ProspectingRadar);

            NetOutgoingMessage msgBuffer = netClient.CreateMessage();
            msgBuffer.Write((byte)InfiniminerMessage.UseTool);
            msgBuffer.Write(PlayerContainer.PlayerPosition);
            msgBuffer.Write(PlayerContainer.PlayerCamera.GetLookVector());
            msgBuffer.Write((byte)PlayerTools.ProspectingRadar);
            msgBuffer.Write((byte)BlockType.None);
            netClient.SendMessage(msgBuffer, NetDeliveryMethod.ReliableUnordered);
        }

        public void FirePickaxe()
        {
            if (netClient.ConnectionStatus != NetConnectionStatus.Connected)
                return;

            PlayerContainer.PlayerToolCooldown = GetToolCooldown(PlayerTools.Pickaxe);

            NetOutgoingMessage msgBuffer = netClient.CreateMessage();
            msgBuffer.Write((byte)InfiniminerMessage.UseTool);
            msgBuffer.Write(PlayerContainer.PlayerPosition);
            msgBuffer.Write(PlayerContainer.PlayerCamera.GetLookVector());
            msgBuffer.Write((byte)PlayerTools.Pickaxe);
            msgBuffer.Write((byte)BlockType.None);
            netClient.SendMessage(msgBuffer, NetDeliveryMethod.ReliableUnordered);
        }

        public void FireConstructionGun(BlockType blockType)
        {
            FireConstructionGun(blockType, false);
        }

        public void FireConstructionGun(BlockType blockType, bool alternate)
        {
            if (netClient.ConnectionStatus != NetConnectionStatus.Connected)
                return;

            PlayerContainer.PlayerToolCooldown = GetToolCooldown(PlayerTools.ConstructionGun);
            PlayerContainer.ConstructionGunAnimation = -5;

            // Send the message.
            NetOutgoingMessage msgBuffer = netClient.CreateMessage();
            msgBuffer.Write((byte)InfiniminerMessage.UseTool);
            msgBuffer.Write(PlayerContainer.PlayerPosition);
            msgBuffer.Write(PlayerContainer.PlayerCamera.GetLookVector());
            msgBuffer.Write((byte)PlayerTools.ConstructionGun);
            BlockType nb = blockType;
            if (alternate)
            {
                switch (nb)
                {
                    // Code allows to use alternate colour of everything, but it's only enabled for translucents
                    /*case BlockType.BankBlue: nb = BlockType.BankRed; break;
                    case BlockType.BeaconBlue: nb = BlockType.BeaconRed; break;
                    case BlockType.SolidBlue: nb = BlockType.SolidRed; break;*/
                    case BlockType.TransBlue: nb = BlockType.TransRed; break;

                    /*case BlockType.BankRed: nb = BlockType.BankBlue; break;
                    case BlockType.BeaconRed: nb = BlockType.BeaconBlue; break;
                    case BlockType.SolidRed: nb = BlockType.SolidBlue; break;*/
                    case BlockType.TransRed: nb = BlockType.TransBlue; break;
                    default: break;//Nothing
                }
            }
            msgBuffer.Write((byte)nb);
            netClient.SendMessage(msgBuffer, NetDeliveryMethod.ReliableUnordered);
        }

        public void FireDeconstructionGun()
        {
            if (netClient.ConnectionStatus != NetConnectionStatus.Connected)
                return;

            PlayerContainer.PlayerToolCooldown = GetToolCooldown(PlayerTools.DeconstructionGun);
            PlayerContainer.ConstructionGunAnimation = -5;

            // Send the message.
            NetOutgoingMessage msgBuffer = netClient.CreateMessage();
            msgBuffer.Write((byte)InfiniminerMessage.UseTool);
            msgBuffer.Write(PlayerContainer.PlayerPosition);
            msgBuffer.Write(PlayerContainer.PlayerCamera.GetLookVector());
            msgBuffer.Write((byte)PlayerTools.DeconstructionGun);
            msgBuffer.Write((byte)BlockType.None);
            netClient.SendMessage(msgBuffer, NetDeliveryMethod.ReliableUnordered);
        }

        public void FireDetonator()
        {
            if (netClient.ConnectionStatus != NetConnectionStatus.Connected)
                return;

            PlayerContainer.PlayerToolCooldown = GetToolCooldown(PlayerTools.Detonator);

            // Send the message.
            NetOutgoingMessage msgBuffer = netClient.CreateMessage();
            msgBuffer.Write((byte)InfiniminerMessage.UseTool);
            msgBuffer.Write(PlayerContainer.PlayerPosition);
            msgBuffer.Write(PlayerContainer.PlayerCamera.GetLookVector());
            msgBuffer.Write((byte)PlayerTools.Detonator);
            msgBuffer.Write((byte)BlockType.None);
            netClient.SendMessage(msgBuffer, NetDeliveryMethod.ReliableUnordered);
        }

        public void ToggleRadar()
        {
            PlayerContainer.PlayerRadarMute = !PlayerContainer.PlayerRadarMute;
            PlaySound(InfiniminerSound.RadarSwitch);
        }

        public void ReadRadar(ref float distanceReading, ref float valueReading)
        {
            valueReading = 0;
            distanceReading = 30;

            // Scan out along the camera axis for 30 meters.
            for (int i = -3; i <= 3; i++)
                for (int j = -3; j <= 3; j++)
                {
                    Matrix rotation = Matrix.CreateRotationX((float)(i * Math.PI / 128)) * Matrix.CreateRotationY((float)(j * Math.PI / 128));
                    Vector3 scanPoint = PlayerContainer.PlayerPosition;
                    Vector3 lookVector = Vector3.Transform(PlayerContainer.PlayerCamera.GetLookVector(), rotation);
                    for (int k = 0; k < 60; k++)
                    {
                        BlockType blockType = GetEngine<BlockEngine>("blockEngine").BlockAtPoint(scanPoint);
                        if (blockType == BlockType.Gold)
                        {
                            distanceReading = Math.Min(distanceReading, 0.5f * k);
                            valueReading = Math.Max(valueReading, 200);
                        }
                        else if (blockType == BlockType.Diamond)
                        {
                            distanceReading = Math.Min(distanceReading, 0.5f * k);
                            valueReading = Math.Max(valueReading, 1000);
                        }
                        scanPoint += 0.5f * lookVector;
                    }
                }
        }

        // Returns true if the player is able to use a bank right now.
        public bool AtBankTerminal()
        {
            // Figure out what we're looking at.
            Vector3 hitPoint = Vector3.Zero;
            Vector3 buildPoint = Vector3.Zero;
            if (!GetEngine<BlockEngine>("blockEngine").RayCollision(PlayerContainer.PlayerPosition, PlayerContainer.PlayerCamera.GetLookVector(), 2.5f, 25, ref hitPoint, ref buildPoint))
                return false;

            // If it's a valid bank object, we're good!
            BlockType blockType = GetEngine<BlockEngine>("blockEngine").BlockAtPoint(hitPoint);
            if (blockType == BlockType.BankRed && PlayerContainer.PlayerTeam == PlayerTeam.Red)
                return true;
            if (blockType == BlockType.BankBlue && PlayerContainer.PlayerTeam == PlayerTeam.Blue)
                return true;
            return false;
        }

        public float GetToolCooldown(PlayerTools tool)
        {
            switch (tool)
            {
                case PlayerTools.Pickaxe: return 0.55f;
                case PlayerTools.Detonator: return 0.01f;
                case PlayerTools.ConstructionGun: return 0.5f;
                case PlayerTools.DeconstructionGun: return 0.5f;
                case PlayerTools.ProspectingRadar: return 0.5f;
                default: return 0;
            }
        }

        public void SendPlayerUpdate()
        {
            if (netClient.ConnectionStatus != NetConnectionStatus.Connected)
                return;

            NetOutgoingMessage msgBuffer = netClient.CreateMessage();
            msgBuffer.Write((byte)InfiniminerMessage.PlayerUpdate);
            msgBuffer.Write(PlayerContainer.PlayerPosition);
            msgBuffer.Write(PlayerContainer.PlayerCamera.GetLookVector());
            msgBuffer.Write((byte)PlayerContainer.PlayerTools[PlayerContainer.PlayerToolSelected]);
            msgBuffer.Write(PlayerContainer.PlayerToolCooldown > 0.001f);
            netClient.SendMessage(msgBuffer, NetDeliveryMethod.Unreliable);
        }

        // ===================== BEGIN PLEXIGLASS CODE ====================== \\\

        public void RegisterEngine<T>(T engine, string engineName)
            where T: IEngine
        {
            if (Engines.ContainsKey(engineName))
                throw new DuplicateKeyException("Engine with name " + engineName + " was already registered!");
            Engines.Add(engineName, new Tuple<IEngine, Type>(engine, typeof(T)));
        }

        public T GetEngine<T>(string engineName)
            where T: IEngine
        {
            if(!Engines.ContainsKey(engineName))
            {
                throw new KeyNotFoundException("Engine " + engineName + " hasn't been registered!");
            }

            if(Engines[engineName].Item2 != typeof(T))
            {
                throw new InvalidCastException("Engine was unable to be cast to the specified type! Requested: " + typeof(T).FullName + " Actual: " + Engines[engineName].Item2.FullName);
            }

            return (T)Engines[engineName].Item1;
        }
    }
}
