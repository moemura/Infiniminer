using Plexiglass.Networking.Packets;
using Plexiglass.Client;
using Plexiglass.Client.States;
using Infiniminer;

namespace Plexiglass.Networking.Handlers
{
    public class PacketTriggerAnimationHandler : IPacketHandler<PacketTriggerAnimation>
    {
        public object HandlePacket(PacketTriggerAnimation packet, IPropertyBag propertyBag = null, IStateMachine gameInstance = null)
        {
            if (propertyBag == null) return null;

            propertyBag.PlayerContainer.ConstructionGunAnimation = packet.AnimationTime;
            if (propertyBag.PlayerContainer.ConstructionGunAnimation <= -0.1)
                propertyBag.PlaySound(InfiniminerSound.RadarSwitch);

            return null;
        }
    }
}
