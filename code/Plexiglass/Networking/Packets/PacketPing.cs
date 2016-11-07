using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plexiglass.Networking.Packets
{
    public class PacketPing : IPacket
    {
        public static uint PACKET_ID = 0xFFFFFF;

        private uint pingValue;

        public PacketPing()
        {
            pingValue = 0;
        }

        public PacketPing(uint pingValue)
        {
            this.pingValue = pingValue;
        }

        public void Deserialize(byte[] data)
        {
            pingValue = (uint)((data[0] << 16) + (data[1] << 8) + data[2]);
        }

        public uint GetPacketID()
        {
            return PACKET_ID;
        }

        public byte[] Serialize()
        {
            byte[] data = new byte[3] { (byte)(pingValue >> 16 & 0xFF), (byte)(pingValue >> 8 & 0xFF), (byte)(pingValue & 0xFF) };
            return data;
        }

        public uint GetPingValue()
        {
            return pingValue;
        }

        public PacketDirectionality GetDirectionality()
        {
            return PacketDirectionality.BIDIRECTIONAL;
        }
    }
}
