using Infiniminer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plexiglass.Networking.Packets
{
    public class PacketPlayerSetTeam : IPacket
    {
        public uint playerId;
        public byte playerTeam;

        public const int PACKET_SIZE = 5;

        public PacketPlayerSetTeam()
        {
            playerId = 0;
            playerTeam = 0;
        }

        public PacketPlayerSetTeam(uint playerId = 0, byte playerTeam = 0)
        {
            this.playerId = playerId;
            this.playerTeam = playerTeam;
        }

        public void Deserialize(byte[] data)
        {
            playerId = BitConverter.ToUInt32(data, 0);
            playerTeam = data[4];
        }

        public PacketDirectionality GetDirectionality()
        {
            return PacketDirectionality.SERVER_TO_CLIENT;
        }

        public uint GetPacketID()
        {
            return (uint)InfiniminerMessage.PlayerSetTeam;
        }

        public byte[] Serialize()
        {
            byte[] data = new byte[PACKET_SIZE];

            BitConverter.GetBytes(playerId).CopyTo(data, 0);
            data[4] = playerTeam;

            return data;
        }
    }
}
