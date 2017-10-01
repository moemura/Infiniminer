using Infiniminer;
using System;

namespace Plexiglass.Networking.Packets
{
    public class PacketResourceUpdate : IPacket
    {
        public uint NewOre;
        public uint NewCash;
        public uint NewWeight;
        public uint NewOreMax;
        public uint NewWeightMax;
        public uint NewOreTeam;
        public uint NewTeamRedCash;
        public uint NewTeamBlueCash;

        public const int NUMBER_OF_FIELDS = 8;

        public const int PACKET_SIZE = NUMBER_OF_FIELDS * 4;

        public void Deserialize(byte[] data)
        {
            NewOre = BitConverter.ToUInt32(data, 0);
            NewCash = BitConverter.ToUInt32(data, 4);
            NewWeight = BitConverter.ToUInt32(data, 8);
            NewOreMax = BitConverter.ToUInt32(data, 12);
            NewWeightMax = BitConverter.ToUInt32(data, 16);
            NewOreTeam = BitConverter.ToUInt32(data, 20);
            NewTeamRedCash = BitConverter.ToUInt32(data, 24);
            NewTeamBlueCash = BitConverter.ToUInt32(data, 28);
        }

        public PacketDirectionality GetDirectionality()
        {
            return PacketDirectionality.SERVER_TO_CLIENT;
        }

        public uint GetPacketId()
        {
            return (uint)InfiniminerMessage.ResourceUpdate;
        }

        public byte[] Serialize()
        {
            var data = new byte[PACKET_SIZE];
            BitConverter.GetBytes(NewOre).CopyTo(data, 0);
            BitConverter.GetBytes(NewCash).CopyTo(data, 4);
            BitConverter.GetBytes(NewWeight).CopyTo(data, 8);
            BitConverter.GetBytes(NewOreMax).CopyTo(data, 12);
            BitConverter.GetBytes(NewWeightMax).CopyTo(data, 16);
            BitConverter.GetBytes(NewOreTeam).CopyTo(data, 20);
            BitConverter.GetBytes(NewTeamRedCash).CopyTo(data, 24);
            BitConverter.GetBytes(NewTeamBlueCash).CopyTo(data, 28);
            return data;
        }
    }
}
