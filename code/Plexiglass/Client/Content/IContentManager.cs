using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
