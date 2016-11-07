using Microsoft.Xna.Framework.Content;
using Plexiglass.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plexiglass.Client.Content
{
    public class PlexiglassContentManager : IContentManager
    {
        private Dictionary<Type, Dictionary<string, object>> ContentDictionary;

        public PlexiglassContentManager()
        {
            ContentDictionary = new Dictionary<Type, Dictionary<string, object>>();
        }

        public void Dispose<T>(string assetName)
        {
            if (!ContentDictionary.ContainsKey(typeof(T)))
            {
                throw new KeyNotFoundException("There are no assets of type " + typeof(T).FullName + " registered!");
            }
            else
            {
                if (!ContentDictionary[typeof(T)].ContainsKey(assetName))
                {
                    throw new KeyNotFoundException("There is no asset of type " + typeof(T).FullName + " with the asset handle " + assetName + " registered!");
                }
                else
                {
                    ContentDictionary[typeof(T)].Remove(assetName);
                }
            }
        }

        public T LoadAndStore<T>(ContentManager manager, string assetName)
        {
            if (ContentDictionary.ContainsKey(typeof(T)) && ContentDictionary[typeof(T)].ContainsKey(assetName))
                return Retrieve<T>(assetName);

            var asset = manager.Load<T>(assetName);

            Store(asset, assetName);

            return asset;
        }

        public T Retrieve<T>(string assetName)
        {
            if(!ContentDictionary.ContainsKey(typeof(T)))
            {
                throw new KeyNotFoundException("There are no assets of type " + typeof(T).FullName + " registered!");
            }
            else
            {
                if(!ContentDictionary[typeof(T)].ContainsKey(assetName))
                {
                    throw new KeyNotFoundException("There is no asset of type " + typeof(T).FullName + " with the asset handle " + assetName + " registered!");
                }
                else
                {
                    return (T)ContentDictionary[typeof(T)][assetName];
                }
            }
        }

        public void Store<T>(T asset, string assetName)
        {
            if (!ContentDictionary.ContainsKey(typeof(T)))
                ContentDictionary.Add(typeof(T), new Dictionary<string, object>());
            if (ContentDictionary[typeof(T)].ContainsKey(assetName))
                throw new DuplicateKeyException("Asset of type " + typeof(T).FullName + " with the asset handle " + assetName + " already exists in the ContentManager!");
            ContentDictionary[typeof(T)].Add(assetName, asset);
        }
    }
}
