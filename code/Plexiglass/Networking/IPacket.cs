namespace Plexiglass.Networking
{

    public static class PacketDirectionalityExt
    {
        public static bool FuzzyEquals(PacketDirectionality lhs, PacketDirectionality rhs)
        {
            switch(lhs)
            {
                case PacketDirectionality.BIDIRECTIONAL:
                    return rhs == PacketDirectionality.SERVER_TO_CLIENT || rhs == PacketDirectionality.CLIENT_TO_SERVER;
                case PacketDirectionality.POLYDIRECTIONAL:
                    return true;
                case PacketDirectionality.SERVER_TO_CLIENT:
                case PacketDirectionality.CLIENT_TO_SERVER:
                    return lhs == rhs || rhs == PacketDirectionality.BIDIRECTIONAL || rhs == PacketDirectionality.POLYDIRECTIONAL;
                case PacketDirectionality.CLIENT_TO_CLIENT:
                    return lhs == rhs || rhs == PacketDirectionality.POLYDIRECTIONAL;
                case PacketDirectionality.ANTIDIRECTIONAL:
                    return false;
                default:
                    return false;
            }
        }
    }

    public enum PacketDirectionality
    {
        ANTIDIRECTIONAL  = 0,
        CLIENT_TO_SERVER = 1,
        SERVER_TO_CLIENT = 2,
        CLIENT_TO_CLIENT = 4,
        BIDIRECTIONAL    = 8,
        POLYDIRECTIONAL  = 16
    }

    public interface IPacket
    {
        uint GetPacketId();
        byte[] Serialize();
        void Deserialize(byte[] data);
        PacketDirectionality GetDirectionality();
    }
}
