using Plexiglass.Networking.Packets;
using Plexiglass.Client;
using Plexiglass.Client.States;

namespace Plexiglass.Networking.Handlers
{
    public class PacketPlayerAliveHandler : IPacketHandler<PacketPlayerAlive>
    {
        public object HandlePacket(PacketPlayerAlive packet, IPropertyBag propertyBag = null, IStateMachine gameInstance = null)
        {
            if (propertyBag == null || !propertyBag.PlayerList.ContainsKey(packet.PlayerId)) return null;

            var player = propertyBag.PlayerList[packet.PlayerId];
            player.Alive = true;

            return null;
        }
    }
}
