using Plexiglass.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plexiglass.Client;
using Plexiglass.Client.States;
using Plexiglass.Client.Engine;
using Infiniminer;
using Microsoft.Xna.Framework;

namespace Plexiglass.Networking.Handlers
{
    public class PacketBlockSetHandler : IPacketHandler<PacketBlockSet>
    {
        public object HandlePacket(PacketBlockSet packet, IPropertyBag propertyBag = null, IStateMachine gameInstance = null)
        {
            IBlockEngine blockEngine = propertyBag.GetEngine<IBlockEngine>("blockEngine");
            // x, y, z, type, all bytes
            uint x = packet.x;
            uint y = packet.y;
            uint z = packet.z;
            BlockType blockType = (BlockType)packet.blockType;
            if (blockType == BlockType.None)
            {
                if (blockEngine.BlockAtPoint(new Vector3(x, y, z)) != BlockType.None)
                    blockEngine.RemoveBlock(x, y, z);
            }
            else
            {
                if (blockEngine.BlockAtPoint(new Vector3(x, y, z)) != BlockType.None)
                    blockEngine.RemoveBlock(x, y, z);
                blockEngine.AddBlock(x, y, z, blockType);
                gameInstance.CheckForStandingInLava();
            }

            return null;
        }
    }
}
