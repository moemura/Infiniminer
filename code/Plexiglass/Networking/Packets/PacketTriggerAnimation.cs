using Infiniminer;
using System;

namespace Plexiglass.Networking.Packets
{
    public class PacketTriggerAnimation : IPacket
    {
        public float AnimationTime;

        public PacketTriggerAnimation()
        {
            AnimationTime = 0.0f;
        }

        public PacketTriggerAnimation(float animationTime)
        {
            AnimationTime = animationTime;
        }

        public void Deserialize(byte[] data)
        {
            AnimationTime = BitConverter.ToSingle(data, 0);
        }

        public PacketDirectionality GetDirectionality()
        {
            return PacketDirectionality.SERVER_TO_CLIENT;
        }

        public uint GetPacketId()
        {
            return (uint)InfiniminerMessage.TriggerConstructionGunAnimation;
        }

        public byte[] Serialize()
        {
            return BitConverter.GetBytes(AnimationTime);
        }
    }
}
