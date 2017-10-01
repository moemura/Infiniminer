using Infiniminer;
using System;

namespace Plexiglass.Networking.Packets
{
    public class PacketBlockSet : IPacket
    {
        public uint X, Y, Z;
        public uint BlockType;

        public const int PACKET_SIZE = 16;

        public PacketBlockSet()
        {
            X = Y = Z = 0;
            BlockType = 0;
        }

        public PacketBlockSet(uint x = 0, uint y = 0, uint z = 0, uint blockType = 0)
        {
            X = x;
            Y = y;
            Z = z;
            BlockType = blockType;
        }

        public void Deserialize(byte[] data)
        {
            X = BitConverter.ToUInt32(data, 0);
            Y = BitConverter.ToUInt32(data, 4);
            Z = BitConverter.ToUInt32(data, 8);
            BlockType = BitConverter.ToUInt32(data, 12);
        }

        public PacketDirectionality GetDirectionality()
        {
            return PacketDirectionality.SERVER_TO_CLIENT;
        }

        public uint GetPacketId()
        {
            return (uint)InfiniminerMessage.BlockSet;
        }

        public byte[] Serialize()
        {
            byte[] data = new byte[PACKET_SIZE];
            BitConverter.GetBytes(X).CopyTo(data, 0);
            BitConverter.GetBytes(Y).CopyTo(data, 4);
            BitConverter.GetBytes(Z).CopyTo(data, 8);
            BitConverter.GetBytes(BlockType).CopyTo(data, 12);
            return data;
        }
    }
}
