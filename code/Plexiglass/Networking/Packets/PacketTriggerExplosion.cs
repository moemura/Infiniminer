using Infiniminer;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plexiglass.Networking.Packets
{
    public class PacketTriggerExplosion : IPacket
    {
        public Vector3 blockPos;

        public const int PACKET_SIZE = 12;

        public PacketTriggerExplosion()
        {
            blockPos = Vector3.Zero;
        }

        public PacketTriggerExplosion(Vector3 blockPos)
        {
            this.blockPos = blockPos;
        }

        public void Deserialize(byte[] data)
        {
            blockPos = new Vector3(BitConverter.ToSingle(data, 0), BitConverter.ToSingle(data, 4), BitConverter.ToSingle(data, 8));
        }

        public PacketDirectionality GetDirectionality()
        {
            return PacketDirectionality.SERVER_TO_CLIENT;
        }

        public uint GetPacketID()
        {
            return (uint)InfiniminerMessage.TriggerExplosion;
        }

        public byte[] Serialize()
        {
            byte[] data = new byte[PACKET_SIZE];

            BitConverter.GetBytes(blockPos.X).CopyTo(data, 0);
            BitConverter.GetBytes(blockPos.Y).CopyTo(data, 4);
            BitConverter.GetBytes(blockPos.Z).CopyTo(data, 8);

            return data;
        }
    }
}
