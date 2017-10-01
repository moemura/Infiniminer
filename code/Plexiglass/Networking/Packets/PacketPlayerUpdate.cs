using Infiniminer;
using System;
using Microsoft.Xna.Framework;

namespace Plexiglass.Networking.Packets
{
    public class PacketPlayerUpdate : IPacket
    {
        public uint PlayerId;
        public Vector3 PlayerPosition;
        public Vector3 PlayerHeading;
        public byte PlayerTool;
        public bool PlayerUsingTool;
        public ushort PlayerScore;

        public const int PACKET_SIZE = 32;


        public PacketPlayerUpdate(uint playerId = 0, 
            Vector3 playerPosition = new Vector3(), 
            Vector3 playerHeading = new Vector3(),
            byte playerTool = 0x00,
            bool playerUsingTool = false,
            ushort playerScore = 0x0000)
        {
            PlayerId = playerId;
            PlayerPosition = playerPosition;
            PlayerHeading = playerHeading;
            PlayerTool = playerTool;
            PlayerUsingTool = playerUsingTool;
            PlayerScore = playerScore;
        }

        public void Deserialize(byte[] data)
        {
            PlayerId = BitConverter.ToUInt32(data, 0);
            PlayerPosition = new Vector3(BitConverter.ToSingle(data, 4), BitConverter.ToSingle(data, 8),
                BitConverter.ToSingle(data, 12));
            PlayerHeading = new Vector3(BitConverter.ToSingle(data, 16), BitConverter.ToSingle(data, 20), 
                BitConverter.ToSingle(data, 24));
            PlayerTool = data[28];
            PlayerUsingTool = BitConverter.ToBoolean(data, 29);
            PlayerScore = BitConverter.ToUInt16(data, 30);
        }

        public PacketDirectionality GetDirectionality()
        {
            return PacketDirectionality.SERVER_TO_CLIENT;
        }

        public uint GetPacketId()
        {
            return (uint)InfiniminerMessage.PlayerJoined;
        }

        public byte[] Serialize()
        {
            var data = new byte[PACKET_SIZE];

            BitConverter.GetBytes(PlayerId).CopyTo(data, 0);

            BitConverter.GetBytes(PlayerPosition.X).CopyTo(data, 4);
            BitConverter.GetBytes(PlayerPosition.Y).CopyTo(data, 8);
            BitConverter.GetBytes(PlayerPosition.Z).CopyTo(data, 12);

            BitConverter.GetBytes(PlayerHeading.X).CopyTo(data, 16);
            BitConverter.GetBytes(PlayerHeading.Y).CopyTo(data, 20);
            BitConverter.GetBytes(PlayerHeading.Z).CopyTo(data, 24);

            data[28] = PlayerTool;

            BitConverter.GetBytes(PlayerUsingTool).CopyTo(data, 29);

            BitConverter.GetBytes(PlayerScore).CopyTo(data, 30);

            return data;
        }
    }
}
