using Plexiglass.Networking.Packets;
using Plexiglass.Client;
using Plexiglass.Client.States;
using Infiniminer;
using Microsoft.Xna.Framework;

namespace Plexiglass.Networking.Handlers
{
    public class PacketPlayerJoinedHandler : IPacketHandler<PacketPlayerJoined>
    {
        public object HandlePacket(PacketPlayerJoined packet, IPropertyBag propertyBag = null, IStateMachine gameInstance = null)
        {
            if (propertyBag == null) return null;

            propertyBag.PlayerList[packet.PlayerId] =
                new Player(null, (Game) gameInstance)
                {
                    Handle = packet.PlayerName,
                    ID = packet.PlayerId,
                    Alive = packet.PlayerAlive,
                    AltColours = false,
                    redTeam = Defines.IM_RED,
                    blueTeam = Defines.IM_BLUE
                };

            if (packet.ThisIsMe)
                propertyBag.PlayerContainer.PlayerMyId = packet.PlayerId;

            return null;
        }
    }
}
