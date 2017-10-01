using Plexiglass.Networking.Packets;
using Plexiglass.Client;
using Plexiglass.Client.States;

namespace Plexiglass.Networking.Handlers
{
    public class PacketResourceUpdateHandler : IPacketHandler<PacketResourceUpdate>
    {
        public object HandlePacket(PacketResourceUpdate packet, IPropertyBag propertyBag = null, IStateMachine gameInstance = null)
        {
            if (propertyBag == null) return null;

            propertyBag.PlayerContainer.PlayerOre = packet.NewOre;
            propertyBag.PlayerContainer.PlayerCash = packet.NewCash;
            propertyBag.PlayerContainer.PlayerWeight = packet.NewWeight;
            propertyBag.PlayerContainer.PlayerOreMax = packet.NewOreMax;
            propertyBag.PlayerContainer.PlayerWeightMax = packet.NewWeightMax;
            propertyBag.TeamContainer.TeamOre = packet.NewOreTeam;
            propertyBag.TeamContainer.TeamRedCash = packet.NewTeamRedCash;
            propertyBag.TeamContainer.TeamBlueCash = packet.NewTeamBlueCash;

            return null;
        }
    }
}
