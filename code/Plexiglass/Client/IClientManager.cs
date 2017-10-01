using Plexiglass.Client.Content;
using Plexiglass.Client.States;
using Plexiglass.Networking;

namespace Plexiglass.Client
{
    public interface IClientManager
    {
        void InitializeContentManager();
        IContentManager GetContentManager();
        IPacketRegistry GetPacketRegistry();
        void InitializePacketRegistry(IPropertyBag propertyBag, IStateMachine gameInstance);
    }
}
