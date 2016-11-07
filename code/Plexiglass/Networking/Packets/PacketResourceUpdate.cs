using Infiniminer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plexiglass.Networking.Packets
{
    public class PacketResourceUpdate : IPacket
    {
        public uint newOre;
        public uint newCash;
        public uint newWeight;
        public uint newOreMax;
        public uint newWeightMax;
        public uint newOreTeam;
        public uint newTeamRedCash;
        public uint newTeamBlueCash;

        public const int NUMBER_OF_FIELDS = 8;

        public const int PACKET_SIZE = NUMBER_OF_FIELDS * 4;

        public void Deserialize(byte[] data)
        {
            newOre = BitConverter.ToUInt32(data, 0);
            newCash = BitConverter.ToUInt32(data, 4);
            newWeight = BitConverter.ToUInt32(data, 8);
            newOreMax = BitConverter.ToUInt32(data, 12);
            newWeightMax = BitConverter.ToUInt32(data, 16);
            newOreTeam = BitConverter.ToUInt32(data, 20);
            newTeamRedCash = BitConverter.ToUInt32(data, 24);
            newTeamBlueCash = BitConverter.ToUInt32(data, 28);
        }

        public PacketDirectionality GetDirectionality()
        {
            return PacketDirectionality.SERVER_TO_CLIENT;
        }

        public uint GetPacketID()
        {
            return (uint)InfiniminerMessage.ResourceUpdate;
        }

        public byte[] Serialize()
        {
            byte[] data = new byte[PACKET_SIZE];
            BitConverter.GetBytes(newOre).CopyTo(data, 0);
            BitConverter.GetBytes(newCash).CopyTo(data, 4);
            BitConverter.GetBytes(newWeight).CopyTo(data, 8);
            BitConverter.GetBytes(newOreMax).CopyTo(data, 12);
            BitConverter.GetBytes(newWeightMax).CopyTo(data, 16);
            BitConverter.GetBytes(newOreTeam).CopyTo(data, 20);
            BitConverter.GetBytes(newTeamRedCash).CopyTo(data, 24);
            BitConverter.GetBytes(newTeamBlueCash).CopyTo(data, 28);
            return data;
        }
    }
}
