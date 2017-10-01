using Plexiglass.Networking.Packets;
using Infiniminer;
using Plexiglass.Client;
using Plexiglass.Client.States;
using Microsoft.Xna.Framework;
using Plexiglass.Client.Engine;

namespace Plexiglass.Networking.Handlers
{
    public class PacketPlayerDeadHandler : IPacketHandler<PacketPlayerDead>
    {
        public object HandlePacket(PacketPlayerDead packet, IPropertyBag propertyBag = null, IStateMachine gameInstance = null)
        {
            if (propertyBag == null || !propertyBag.PlayerList.ContainsKey(packet.PlayerId)) return null;

            var player = propertyBag.PlayerList[packet.PlayerId];

            player.Alive = false;

            propertyBag.GetEngine<IParticleEngine>("particleEngine").CreateBloodSplatter(player.Position, player.Team == PlayerTeam.Red ? Color.Red : Color.Blue);

            if (packet.PlayerId != propertyBag.PlayerContainer.PlayerMyId)
                propertyBag.PlaySound(InfiniminerSound.Death, player.Position);

            return null;
        }
    }
}
