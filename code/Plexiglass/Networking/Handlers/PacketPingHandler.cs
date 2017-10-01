using Plexiglass.Client;
using Plexiglass.Networking.Packets;
using Plexiglass.Client.States;

namespace Plexiglass.Networking.Handlers
{
    public class PacketPingHandler : IPacketHandler<PacketPing>
    {
        public object HandlePacket(PacketPing packet, IPropertyBag propertyBag = null, IStateMachine gameInstance = null)
        {
            return "Ping: 0x" + packet.GetPingValue().ToString("X6");
        }
    }
}
