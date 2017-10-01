using Plexiglass.Networking.Packets;
using Plexiglass.Client;
using Plexiglass.Client.States;
using Infiniminer;

namespace Plexiglass.Networking.Handlers
{
    public class PacketPlayerSetTeamHandler : IPacketHandler<PacketPlayerSetTeam>
    {
        public object HandlePacket(PacketPlayerSetTeam packet, IPropertyBag propertyBag = null, IStateMachine gameInstance = null)
        {
            if (propertyBag == null || !propertyBag.PlayerList.ContainsKey(packet.PlayerId)) return null;

            var player = propertyBag.PlayerList[packet.PlayerId];
            player.Team = (PlayerTeam)packet.PlayerTeam;

            return null;
        }
    }
}
