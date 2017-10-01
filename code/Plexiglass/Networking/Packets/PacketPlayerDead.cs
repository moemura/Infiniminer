using System;
using Infiniminer;

namespace Plexiglass.Networking.Packets
{
    public class PacketPlayerDead : IPacket
    {
        public uint PlayerId;

        public const int PACKET_SIZE = 4;

        public PacketPlayerDead(uint playerId = 0)
        {
            PlayerId = playerId;
        }

        public uint GetPacketId()
        {
            return (uint) InfiniminerMessage.PlayerDead;
        }

        public byte[] Serialize()
        {
            var data = new byte[PACKET_SIZE];

            BitConverter.GetBytes(PlayerId).CopyTo(data, 0);

            return data;
        }

        public void Deserialize(byte[] data)
        {
            PlayerId = BitConverter.ToUInt32(data, 0);
        }

        public PacketDirectionality GetDirectionality()
        {
            return PacketDirectionality.SERVER_TO_CLIENT;
        }
    }
}
