using Infiniminer;
using System;

namespace Plexiglass.Networking.Packets
{
    public class PacketPlayerSetTeam : IPacket
    {
        public uint PlayerId;
        public byte PlayerTeam;

        public const int PACKET_SIZE = 5;

        public PacketPlayerSetTeam()
        {
            PlayerId = 0;
            PlayerTeam = 0;
        }

        public PacketPlayerSetTeam(uint playerId = 0, byte playerTeam = 0)
        {
            PlayerId = playerId;
            PlayerTeam = playerTeam;
        }

        public void Deserialize(byte[] data)
        {
            PlayerId = BitConverter.ToUInt32(data, 0);
            PlayerTeam = data[4];
        }

        public PacketDirectionality GetDirectionality()
        {
            return PacketDirectionality.SERVER_TO_CLIENT;
        }

        public uint GetPacketId()
        {
            return (uint)InfiniminerMessage.PlayerSetTeam;
        }

        public byte[] Serialize()
        {
            byte[] data = new byte[PACKET_SIZE];

            BitConverter.GetBytes(PlayerId).CopyTo(data, 0);
            data[4] = PlayerTeam;

            return data;
        }
    }
}
