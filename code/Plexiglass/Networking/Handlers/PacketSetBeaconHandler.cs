using Infiniminer;
using Plexiglass.Client;
using Plexiglass.Networking.Packets;
using Plexiglass.Client.States;

namespace Plexiglass.Networking.Handlers
{
    public class PacketSetBeaconHandler : IPacketHandler<PacketSetBeacon>
    {
        public object HandlePacket(PacketSetBeacon packet, IPropertyBag propertyBag = null, IStateMachine gameInstance = null)
        {
            if (packet.Text == "")
            {
                if (propertyBag != null && propertyBag.TeamContainer.BeaconList.ContainsKey(packet.Position))
                    propertyBag.TeamContainer.BeaconList.Remove(packet.Position);
            }
            else
            {
                var newBeacon = new Beacon
                {
                    ID = packet.Text,
                    Team = packet.Team
                };

                propertyBag?.TeamContainer.BeaconList.Add(packet.Position, newBeacon);
            }

            return null;
        }
    }
}
