using Infiniminer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plexiglass.Networking.Packets
{
    public class PacketBlockBulkTransfer : IPacket
    {
        public bool isCompressed;
        public uint x, y;
        public ushort[,] blockList;

        public const int Y_SIZE = 16;
        public const int Z_SIZE = 64;

        public const int TOTAL_BLOCKLIST_SIZE = (Y_SIZE * Z_SIZE * 2);
        public const int WITHOUT_BLOCKLIST_SIZE = 9;
        public const int PACKET_SIZE = WITHOUT_BLOCKLIST_SIZE + TOTAL_BLOCKLIST_SIZE;


        public PacketBlockBulkTransfer()
        {
            x = 0;
            y = 0;
            isCompressed = false;
            blockList = new ushort[Y_SIZE,Z_SIZE];
        }

        public PacketBlockBulkTransfer(uint x = 0, uint y = 0, bool isCompressed = false, ushort[,] blockList = null)
        {
            this.x = x;
            this.y = y;
            this.isCompressed = isCompressed;
            if (blockList.Length != Y_SIZE * Z_SIZE)
                throw new Exception("Blocklist is of an invalid size!");
            this.blockList = blockList;
        }

        public void Deserialize(byte[] data)
        {
            isCompressed = Convert.ToBoolean(data[0]);
            x = BitConverter.ToUInt32(data, 1);
            y = BitConverter.ToUInt32(data, 5);

            var iterator = WITHOUT_BLOCKLIST_SIZE;

            for(var y = 0;y < Y_SIZE;y++)
            {
                for(var z = 0;z < Z_SIZE;z++)
                {
                    blockList[y, z] = BitConverter.ToUInt16(data, iterator);
                    iterator += 2;
                }
            }
        }

        public PacketDirectionality GetDirectionality()
        {
            return PacketDirectionality.SERVER_TO_CLIENT;
        }

        public uint GetPacketID()
        {
            return (uint)InfiniminerMessage.BlockBulkTransfer;
        }

        public byte[] Serialize()
        {
            byte[] data = new byte[PACKET_SIZE];
            data[0] = Convert.ToByte(isCompressed);
            BitConverter.GetBytes(x).CopyTo(data, 1);
            BitConverter.GetBytes(y).CopyTo(data, 5);

            var iterator = WITHOUT_BLOCKLIST_SIZE;

            for (var y = 0; y < Y_SIZE; y++)
            {
                for (var z = 0; z < Z_SIZE; z++)
                {
                    BitConverter.GetBytes(blockList[y, z]).CopyTo(data, iterator);
                    iterator += 2;
                }
            }
            return data;
        }
    }
}
