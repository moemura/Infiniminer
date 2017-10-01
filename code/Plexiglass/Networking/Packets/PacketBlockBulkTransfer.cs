using Infiniminer;
using System;

namespace Plexiglass.Networking.Packets
{
    public class PacketBlockBulkTransfer : IPacket
    {
        public bool IsCompressed;
        public uint X, Y;
        public ushort[,] BlockList;

        public const int Y_SIZE = 16;
        public const int Z_SIZE = 64;

        public const int TOTAL_BLOCKLIST_SIZE = (Y_SIZE * Z_SIZE * 2);
        public const int WITHOUT_BLOCKLIST_SIZE = 9;
        public const int PACKET_SIZE = WITHOUT_BLOCKLIST_SIZE + TOTAL_BLOCKLIST_SIZE;


        public PacketBlockBulkTransfer()
        {
            X = 0;
            Y = 0;
            IsCompressed = false;
            BlockList = new ushort[Y_SIZE,Z_SIZE];
        }

        public PacketBlockBulkTransfer(uint x = 0, uint y = 0, bool isCompressed = false, ushort[,] blockList = null)
        {
            X = x;
            Y = y;
            IsCompressed = isCompressed;
            if (blockList != null && blockList.Length != Y_SIZE * Z_SIZE)
                throw new Exception("Blocklist is of an invalid size!");
            BlockList = blockList;
        }

        public void Deserialize(byte[] data)
        {
            IsCompressed = Convert.ToBoolean(data[0]);
            X = BitConverter.ToUInt32(data, 1);
            Y = BitConverter.ToUInt32(data, 5);

            var iterator = WITHOUT_BLOCKLIST_SIZE;

            for(var y = 0;y < Y_SIZE;y++)
            {
                for(var z = 0;z < Z_SIZE;z++)
                {
                    BlockList[y, z] = BitConverter.ToUInt16(data, iterator);
                    iterator += 2;
                }
            }
        }

        public PacketDirectionality GetDirectionality()
        {
            return PacketDirectionality.SERVER_TO_CLIENT;
        }

        public uint GetPacketId()
        {
            return (uint)InfiniminerMessage.BlockBulkTransfer;
        }

        public byte[] Serialize()
        {
            byte[] data = new byte[PACKET_SIZE];
            data[0] = Convert.ToByte(IsCompressed);
            BitConverter.GetBytes(X).CopyTo(data, 1);
            BitConverter.GetBytes(Y).CopyTo(data, 5);

            var iterator = WITHOUT_BLOCKLIST_SIZE;

            for (var y = 0; y < Y_SIZE; y++)
            {
                for (var z = 0; z < Z_SIZE; z++)
                {
                    BitConverter.GetBytes(BlockList[y, z]).CopyTo(data, iterator);
                    iterator += 2;
                }
            }
            return data;
        }
    }
}
