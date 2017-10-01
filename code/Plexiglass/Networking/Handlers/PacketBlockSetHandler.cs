using Plexiglass.Networking.Packets;
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
            if (propertyBag == null) return null;

            var blockEngine = propertyBag.GetEngine<IBlockEngine>("blockEngine");
            // x, y, z, type, all bytes
            var x = packet.X;
            var y = packet.Y;
            var z = packet.Z;
            var blockType = (BlockType)packet.BlockType;
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
                gameInstance?.CheckForStandingInLava();
            }

            return null;
        }
    }
}
