namespace Plexiglass.Networking.Packets
{
    public class PacketPing : IPacket
    {
        public static uint PacketId = 0xFFFFFF;

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

        public uint GetPacketId()
        {
            return PacketId;
        }

        public byte[] Serialize()
        {
            byte[] data = { (byte)(pingValue >> 16 & 0xFF), (byte)(pingValue >> 8 & 0xFF), (byte)(pingValue & 0xFF) };
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
