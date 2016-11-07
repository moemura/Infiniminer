using Infiniminer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plexiglass.Networking.Packets
{
    public class PacketTriggerAnimation : IPacket
    {
        public float animationTime;

        public PacketTriggerAnimation()
        {
            animationTime = 0.0f;
        }

        public PacketTriggerAnimation(float animationTime)
        {
            this.animationTime = animationTime;
        }

        public void Deserialize(byte[] data)
        {
            this.animationTime = BitConverter.ToSingle(data, 0);
        }

        public PacketDirectionality GetDirectionality()
        {
            return PacketDirectionality.SERVER_TO_CLIENT;
        }

        public uint GetPacketID()
        {
            return (uint)InfiniminerMessage.TriggerConstructionGunAnimation;
        }

        public byte[] Serialize()
        {
            return BitConverter.GetBytes(animationTime);
        }
    }
}
