using Infiniminer;
using Microsoft.Xna.Framework;

namespace Plexiglass.Client.Engine
{
    public interface IBlockEngine : IEngine
    {
        BlockType[,,] BlockList { get; set; }
        BlockType[,,] DownloadList { get; set; }
        BlockType BlockAtPoint(Vector3 point);
        void RemoveBlock(uint x, uint y, uint z);
        void AddBlock(uint x, uint y, uint z, BlockType blockType);
        void DownloadComplete();
    }
}
