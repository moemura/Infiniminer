using Plexiglass.Networking.Packets;
using Plexiglass.Client;
using Plexiglass.Client.States;
using Infiniminer;

namespace Plexiglass.Networking.Handlers
{
    public class PacketPlayerUpdateHandler : IPacketHandler<PacketPlayerUpdate>
    {
        public object HandlePacket(PacketPlayerUpdate packet, IPropertyBag propertyBag = null, IStateMachine gameInstance = null)
        {
            if (propertyBag == null || !propertyBag.PlayerList.ContainsKey(packet.PlayerId)) return null;

            var player = propertyBag.PlayerList[packet.PlayerId];
            player.UpdatePosition(packet.PlayerPosition, propertyBag.CurrentGameTime.TotalGameTime.TotalSeconds);
            player.Heading = packet.PlayerHeading;
            player.Tool = (PlayerTools) packet.PlayerTool;
            player.UsingTool = packet.PlayerUsingTool;
            player.Score = (uint)(packet.PlayerScore * 100);

            return null;
        }
    }
}
