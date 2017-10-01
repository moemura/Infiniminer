using Infiniminer;
using Microsoft.Xna.Framework;
using System;

namespace Plexiglass.Networking.Packets
{
    public class PacketTriggerExplosion : IPacket
    {
        public Vector3 BlockPos;

        public const int PACKET_SIZE = 12;

        public PacketTriggerExplosion()
        {
            BlockPos = Vector3.Zero;
        }

        public PacketTriggerExplosion(Vector3 blockPos)
        {
            BlockPos = blockPos;
        }

        public void Deserialize(byte[] data)
        {
            BlockPos = new Vector3(BitConverter.ToSingle(data, 0), BitConverter.ToSingle(data, 4), BitConverter.ToSingle(data, 8));
        }

        public PacketDirectionality GetDirectionality()
        {
            return PacketDirectionality.SERVER_TO_CLIENT;
        }

        public uint GetPacketId()
        {
            return (uint)InfiniminerMessage.TriggerExplosion;
        }

        public byte[] Serialize()
        {
            byte[] data = new byte[PACKET_SIZE];

            BitConverter.GetBytes(BlockPos.X).CopyTo(data, 0);
            BitConverter.GetBytes(BlockPos.Y).CopyTo(data, 4);
            BitConverter.GetBytes(BlockPos.Z).CopyTo(data, 8);

            return data;
        }
    }
}
