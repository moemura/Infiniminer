using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plexiglass.Networking
{
    public interface IPacketRegistry
    {
        void RegisterPacket<P, H>()
            where P : IPacket
            where H : IPacketHandler<P>;

        object HandlePacket<P>(P packet)
            where P : IPacket;

        PacketDirectionality Directionality { get; set; }
    }
}
