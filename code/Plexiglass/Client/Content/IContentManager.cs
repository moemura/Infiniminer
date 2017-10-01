using Microsoft.Xna.Framework.Content;

namespace Plexiglass.Client.Content
{
    public interface IContentManager
    {
        T Retrieve<T>(string assetName);
        void Store<T>(T asset, string assetName);
        T LoadAndStore<T>(ContentManager content, string assetName);
        void Dispose<T>(string assetName);
    }
}
