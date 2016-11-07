using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace Infiniminer
{
    public class InfiniminerNetServer : NetServer
    {
        public InfiniminerNetServer(NetPeerConfiguration config)
            : base(config)
        {
        }

    }
}
