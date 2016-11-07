using Infiniminer;
using Plexiglass.Client;
using Plexiglass.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plexiglass.Client.States;

namespace Plexiglass.Networking.Handlers
{
    public class PacketSetBeaconHandler : IPacketHandler<PacketSetBeacon>
    {
        public object HandlePacket(PacketSetBeacon packet, IPropertyBag propertyBag = null, IStateMachine gameInstance = null)
        {
            if (packet.text == "")
            {
                if (propertyBag.TeamContainer.BeaconList.ContainsKey(packet.position))
                    propertyBag.TeamContainer.BeaconList.Remove(packet.position);
            }
            else
            {
                Beacon newBeacon = new Beacon();
                newBeacon.ID = packet.text;
                newBeacon.Team = packet.team;
                propertyBag.TeamContainer.BeaconList.Add(packet.position, newBeacon);
            }

            return null;
        }
    }
}
