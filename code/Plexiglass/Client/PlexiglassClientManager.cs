using Plexiglass.Client.Content;
using Plexiglass.Client.States;
using Plexiglass.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plexiglass.Client
{
    public class PlexiglassClientManager : IClientManager
    {
        public IContentManager plexiglassContentManager = null;
        public IPacketRegistry plexiglassPacketRegistry = null;

        public PlexiglassClientManager()
        {

        }

        public IContentManager GetContentManager()
        {
            return plexiglassContentManager;
        }

        public IPacketRegistry GetPacketRegistry()
        {
            return plexiglassPacketRegistry;
        }

        public void InitializeContentManager()
        {
            plexiglassContentManager = new PlexiglassContentManager();
        }

        public void InitializePacketRegistry(IPropertyBag propertyBag, IStateMachine gameInstance)
        {
            plexiglassPacketRegistry = new PlexiglassPacketRegistry(PacketDirectionality.CLIENT_TO_SERVER, propertyBag, gameInstance);
        }
    }
}
