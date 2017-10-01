using Infiniminer;
using Microsoft.Xna.Framework;

namespace Plexiglass.Networking.Packets
{
    public class PacketSetBeacon : IPacket
    {
        public Vector3 Position;
        public string Text;
        public PlayerTeam Team;

        public void Deserialize(byte[] data)
        {
            Text = PacketSerializationTools.GetString(data, 0);
            Position = PacketSerializationTools.GetVector3(data, Text.Length+1);
            Team = (PlayerTeam)data[data.Length - 1];
        }

        public PacketDirectionality GetDirectionality()
        {
            return PacketDirectionality.SERVER_TO_CLIENT;
        }

        public uint GetPacketId()
        {
            return (uint)InfiniminerMessage.SetBeacon;
        }

        public byte[] Serialize()
        {
            var text = PacketSerializationTools.FromString(Text);
            var position = PacketSerializationTools.FromVector3(Position);
            var team = (byte)Team;

            var data = new byte[text.Length + position.Length + 1];

            text.CopyTo(data, 0);
            position.CopyTo(data, text.Length);
            data[text.Length + position.Length] = team;

            return data;
        }
    }
}
