using Plexiglass.Client;
using Plexiglass.Client.States;

namespace Plexiglass.Networking
{
    public interface IPacketHandler<TP> where TP: IPacket
    {
        object HandlePacket(TP packet, IPropertyBag propertyBag = null, IStateMachine gameInstance = null);
    }
}
