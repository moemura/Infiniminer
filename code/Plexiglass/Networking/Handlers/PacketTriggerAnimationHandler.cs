using Plexiglass.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plexiglass.Client;
using Plexiglass.Client.States;
using Infiniminer;

namespace Plexiglass.Networking.Handlers
{
    public class PacketTriggerAnimationHandler : IPacketHandler<PacketTriggerAnimation>
    {
        public object HandlePacket(PacketTriggerAnimation packet, IPropertyBag propertyBag = null, IStateMachine gameInstance = null)
        {
            propertyBag.PlayerContainer.constructionGunAnimation = packet.animationTime;
            if (propertyBag.PlayerContainer.constructionGunAnimation <= -0.1)
                propertyBag.PlaySound(InfiniminerSound.RadarSwitch);
            return null;
        }
    }
}
