using Infiniminer;
using System;
using System.Text;

namespace Plexiglass.Networking.Packets
{
    public class PacketPlayerJoined : IPacket
    {
        public uint PlayerId;
        public string PlayerName;
        public bool ThisIsMe;
        public bool PlayerAlive;

        public const int PACKET_SIZE = 6;

        public PacketPlayerJoined()
        {
            PlayerId = 0;
            PlayerName = "Player";
            ThisIsMe = false;
            PlayerAlive = false;
        }

        public PacketPlayerJoined(uint playerId = 0, string playerName = "Player", bool thisIsMe = false, bool playerAlive = true)
        {
            PlayerId = playerId;
            PlayerName = playerName;
            ThisIsMe = thisIsMe;
            PlayerAlive = playerAlive;
        }

        public void Deserialize(byte[] data)
        {
            PlayerId = BitConverter.ToUInt32(data, 0);
            var stringLen = data.Length - PACKET_SIZE;
            PlayerName = Encoding.UTF8.GetString(data, 4, stringLen-1);
            ThisIsMe = BitConverter.ToBoolean(data, 4 + stringLen);
            PlayerAlive = BitConverter.ToBoolean(data, 5 + stringLen);
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
            var playerNameArray = Encoding.UTF8.GetBytes(PlayerName + '\0');
            var data = new byte[PACKET_SIZE + playerNameArray.Length];

            BitConverter.GetBytes(PlayerId).CopyTo(data, 0);
            playerNameArray.CopyTo(data, 4);
            BitConverter.GetBytes(ThisIsMe).CopyTo(data, 4 + playerNameArray.Length);
            BitConverter.GetBytes(PlayerAlive).CopyTo(data, 5 + playerNameArray.Length);

            return data;
        }
    }
}
