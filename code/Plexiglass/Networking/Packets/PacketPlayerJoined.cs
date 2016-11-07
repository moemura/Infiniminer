using Infiniminer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plexiglass.Networking.Packets
{
    public class PacketPlayerJoined : IPacket
    {
        public uint playerId;
        public string playerName;
        public bool thisIsMe;
        public bool playerAlive;

        public const int PACKET_SIZE = 6;

        public PacketPlayerJoined()
        {
            playerId = 0;
            playerName = "Player";
            thisIsMe = false;
            playerAlive = false;
        }

        public PacketPlayerJoined(uint playerId = 0, string playerName = "Player", bool thisIsMe = false, bool playerAlive = true)
        {
            this.playerId = playerId;
            this.playerName = playerName;
            this.thisIsMe = thisIsMe;
            this.playerAlive = playerAlive;
        }

        public void Deserialize(byte[] data)
        {
            playerId = BitConverter.ToUInt32(data, 0);
            var stringLen = data.Length - PACKET_SIZE;
            playerName = Encoding.UTF8.GetString(data, 4, stringLen-1);
            thisIsMe = BitConverter.ToBoolean(data, 4 + stringLen);
            playerAlive = BitConverter.ToBoolean(data, 5 + stringLen);
        }

        public PacketDirectionality GetDirectionality()
        {
            return PacketDirectionality.SERVER_TO_CLIENT;
        }

        public uint GetPacketID()
        {
            return (uint)InfiniminerMessage.PlayerJoined;
        }

        public byte[] Serialize()
        {
            var playerNameArray = Encoding.UTF8.GetBytes(playerName + '\0');
            var data = new byte[PACKET_SIZE + playerNameArray.Length];

            BitConverter.GetBytes(playerId).CopyTo(data, 0);
            playerNameArray.CopyTo(data, 4);
            BitConverter.GetBytes(thisIsMe).CopyTo(data, 4 + playerNameArray.Length);
            BitConverter.GetBytes(playerAlive).CopyTo(data, 5 + playerNameArray.Length);

            return data;
        }
    }
}
