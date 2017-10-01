using Plexiglass.Networking.Packets;
using Plexiglass.Client;
using Plexiglass.Client.States;

namespace Plexiglass.Networking.Handlers
{
    public class PacketPlayerLeftHandler : IPacketHandler<PacketPlayerLeft>
    {
        public object HandlePacket(PacketPlayerLeft packet, IPropertyBag propertyBag = null, IStateMachine gameInstance = null)
        {
            if (propertyBag == null || !propertyBag.PlayerList.ContainsKey(packet.PlayerId)) return null;

            propertyBag.PlayerList.Remove(packet.PlayerId);

            return null;
        }
    }
}
