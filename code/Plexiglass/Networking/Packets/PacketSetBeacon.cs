using Infiniminer;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plexiglass.Networking.Packets
{
    public class PacketSetBeacon : IPacket
    {
        public Vector3 position;
        public string text;
        public PlayerTeam team;

        public void Deserialize(byte[] data)
        {
            text = PacketSerializationTools.GetString(data, 0);
            position = PacketSerializationTools.GetVector3(data, text.Length+1);
            team = (PlayerTeam)data[data.Length - 1];
        }

        public PacketDirectionality GetDirectionality()
        {
            return PacketDirectionality.SERVER_TO_CLIENT;
        }

        public uint GetPacketID()
        {
            return (uint)InfiniminerMessage.SetBeacon;
        }

        public byte[] Serialize()
        {
            var text = PacketSerializationTools.FromString(this.text);
            var position = PacketSerializationTools.FromVector3(this.position);
            byte team = (byte)this.team;

            byte[] data = new byte[text.Length + position.Length + 1];

            text.CopyTo(data, 0);
            position.CopyTo(data, text.Length);
            data[text.Length + position.Length] = team;

            return data;
        }
    }
}
