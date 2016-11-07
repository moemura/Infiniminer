using Infiniminer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plexiglass.Networking.Packets
{
    public class PacketBlockSet : IPacket
    {
        public uint x, y, z;
        public uint blockType;

        public const int PACKET_SIZE = 16;

        public PacketBlockSet()
        {
            x = y = z = 0;
            blockType = 0;
        }

        public PacketBlockSet(uint x = 0, uint y = 0, uint z = 0, uint blockType = 0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.blockType = blockType;
        }

        public void Deserialize(byte[] data)
        {
            x = BitConverter.ToUInt32(data, 0);
            y = BitConverter.ToUInt32(data, 4);
            z = BitConverter.ToUInt32(data, 8);
            blockType = BitConverter.ToUInt32(data, 12);
        }

        public PacketDirectionality GetDirectionality()
        {
            return PacketDirectionality.SERVER_TO_CLIENT;
        }

        public uint GetPacketID()
        {
            return (uint)InfiniminerMessage.BlockSet;
        }

        public byte[] Serialize()
        {
            byte[] data = new byte[PACKET_SIZE];
            BitConverter.GetBytes(x).CopyTo(data, 0);
            BitConverter.GetBytes(y).CopyTo(data, 4);
            BitConverter.GetBytes(z).CopyTo(data, 8);
            BitConverter.GetBytes(blockType).CopyTo(data, 12);
            return data;
        }
    }
}
