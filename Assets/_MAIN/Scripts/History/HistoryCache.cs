using System.Collections.Generic;
using UnityEngine;

namespace HISTORY
{
    // this script is for saving the history states in a cache so that we can just grab the previous states and load it from cache
    // instead of having to get load it from Resources and use more system memory
    public class HistoryCache
    {
        // We make eveything this here public static so that we can access it at anytime
        // here we have a tuple as the value, because we want to also check and have an index to see how long it has been
        // since we last accessed the state and then remove it if it hasnt been accessed for a while.
        public static Dictionary<string, (object asset, int staleIndex)> loadedAssets = new Dictionary<string, (object asset, int staleIndex)>();

        // here T represents what type we are going to be receiving because we dont know what the type is
        // then once we find the key value pair from the dictionary, we then load it and cast it to type T (whatever the type the resource is)
        public static T TryLoadObject<T>(string key)
        {
            object resource = null;

            if (loadedAssets.ContainsKey(key))
                resource = (T)loadedAssets[key].asset;
            else
            {
                resource = Resources.Load(key);
                if (resource != null)
                {
                    // we start at 0 index because it's new
                    loadedAssets[key] = (resource, 0);
                }
            }

            if (resource != null)
            {
                if (resource is T)
                    return (T)resource;
                else
                    Debug.LogWarning($"Retrieved object '{key} was not the expected type");
            }

            Debug.LogWarning($"Could not load asset from cache '{key}");
            return default(T);
        }


    }

}
