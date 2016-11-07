using Plexiglass.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plexiglass.Client;
using Plexiglass.Client.States;

namespace Plexiglass.Networking.Handlers
{
    public class PacketResourceUpdateHandler : IPacketHandler<PacketResourceUpdate>
    {
        public object HandlePacket(PacketResourceUpdate packet, IPropertyBag propertyBag = null, IStateMachine gameInstance = null)
        {
            propertyBag.PlayerContainer.playerOre = packet.newOre;
            propertyBag.PlayerContainer.playerCash = packet.newCash;
            propertyBag.PlayerContainer.playerWeight = packet.newWeight;
            propertyBag.PlayerContainer.playerOreMax = packet.newOreMax;
            propertyBag.PlayerContainer.playerWeightMax = packet.newWeightMax;
            propertyBag.TeamContainer.teamOre = packet.newOreTeam;
            propertyBag.TeamContainer.teamRedCash = packet.newTeamRedCash;
            propertyBag.TeamContainer.teamBlueCash = packet.newTeamBlueCash;

            return null;
        }
    }
}
