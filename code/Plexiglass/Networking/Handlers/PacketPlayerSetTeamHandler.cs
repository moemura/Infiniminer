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
    public class PacketPlayerSetTeamHandler : IPacketHandler<PacketPlayerSetTeam>
    {
        public object HandlePacket(PacketPlayerSetTeam packet, IPropertyBag propertyBag = null, IStateMachine gameInstance = null)
        {
            if (propertyBag.PlayerList.ContainsKey(packet.playerId))
            {
                Player player = propertyBag.PlayerList[packet.playerId];
                player.Team = (PlayerTeam)packet.playerTeam;
            }

            return null;
        }
    }
}
