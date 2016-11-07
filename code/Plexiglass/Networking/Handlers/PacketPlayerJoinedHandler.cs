using Plexiglass.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            propertyBag.PlayerList[packet.playerId] = new Player(null, (Game)gameInstance);
            propertyBag.PlayerList[packet.playerId].Handle = packet.playerName;
            propertyBag.PlayerList[packet.playerId].ID = packet.playerId;
            propertyBag.PlayerList[packet.playerId].Alive = packet.playerAlive;
            propertyBag.PlayerList[packet.playerId].AltColours = false;
            propertyBag.PlayerList[packet.playerId].redTeam = Defines.IM_RED;
            propertyBag.PlayerList[packet.playerId].blueTeam = Defines.IM_BLUE;
            if (packet.thisIsMe)
                propertyBag.PlayerContainer.playerMyId = packet.playerId;

            return null;
        }
    }
}
