using Plexiglass.Client;
using Plexiglass.Client.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plexiglass.Networking
{
    public interface IPacketHandler<P> where P: IPacket
    {
        object HandlePacket(P packet, IPropertyBag propertyBag = null, IStateMachine gameInstance = null);
    }
}
