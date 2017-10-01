using Microsoft.Xna.Framework.Content;
using Plexiglass.Exceptions;
using System;
using System.Collections.Generic;

namespace Plexiglass.Client.Content
{
    public class PlexiglassContentManager : IContentManager
    {
        private readonly Dictionary<Type, Dictionary<string, object>> contentDictionary;

        public PlexiglassContentManager()
        {
            contentDictionary = new Dictionary<Type, Dictionary<string, object>>();
        }

        public void Dispose<T>(string assetName)
        {
            if (!contentDictionary.ContainsKey(typeof(T)))
            {
                throw new KeyNotFoundException("There are no assets of type " + typeof(T).FullName + " registered!");
            }
            else
            {
                if (!contentDictionary[typeof(T)].ContainsKey(assetName))
                {
                    throw new KeyNotFoundException("There is no asset of type " + typeof(T).FullName + " with the asset handle " + assetName + " registered!");
                }
                else
                {
                    contentDictionary[typeof(T)].Remove(assetName);
                }
            }
        }

        public T LoadAndStore<T>(ContentManager manager, string assetName)
        {
            if (contentDictionary.ContainsKey(typeof(T)) && contentDictionary[typeof(T)].ContainsKey(assetName))
                return Retrieve<T>(assetName);

            var asset = manager.Load<T>(assetName);

            Store(asset, assetName);

            return asset;
        }

        public T Retrieve<T>(string assetName)
        {
            if(!contentDictionary.ContainsKey(typeof(T)))
            {
                throw new KeyNotFoundException("There are no assets of type " + typeof(T).FullName + " registered!");
            }
            else
            {
                if(!contentDictionary[typeof(T)].ContainsKey(assetName))
                {
                    throw new KeyNotFoundException("There is no asset of type " + typeof(T).FullName + " with the asset handle " + assetName + " registered!");
                }
                else
                {
                    return (T)contentDictionary[typeof(T)][assetName];
                }
            }
        }

        public void Store<T>(T asset, string assetName)
        {
            if (!contentDictionary.ContainsKey(typeof(T)))
                contentDictionary.Add(typeof(T), new Dictionary<string, object>());
            if (contentDictionary[typeof(T)].ContainsKey(assetName))
                throw new DuplicateKeyException("Asset of type " + typeof(T).FullName + " with the asset handle " + assetName + " already exists in the ContentManager!");
            contentDictionary[typeof(T)].Add(assetName, asset);
        }
    }
}
