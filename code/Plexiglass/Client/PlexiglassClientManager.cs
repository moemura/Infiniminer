using Plexiglass.Client.Content;
using Plexiglass.Client.States;
using Plexiglass.Networking;

namespace Plexiglass.Client
{
    public class PlexiglassClientManager : IClientManager
    {
        public IContentManager PlexiglassContentManager;
        public IPacketRegistry PlexiglassPacketRegistry;

        public IContentManager GetContentManager()
        {
            return PlexiglassContentManager;
        }

        public IPacketRegistry GetPacketRegistry()
        {
            return PlexiglassPacketRegistry;
        }

        public void InitializeContentManager()
        {
            PlexiglassContentManager = new PlexiglassContentManager();
        }

        public void InitializePacketRegistry(IPropertyBag propertyBag, IStateMachine gameInstance)
        {
            PlexiglassPacketRegistry = new PlexiglassPacketRegistry(PacketDirectionality.CLIENT_TO_SERVER, propertyBag, gameInstance);
        }
    }
}
