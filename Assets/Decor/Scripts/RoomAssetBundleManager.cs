using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomAssetBundleManager : Singleton<RoomAssetBundleManager>
{
    private Dictionary<string, AssetBundle> assetBundleDict = new Dictionary<string, AssetBundle>();

    public RoomAssetBundleManager()
    {

    }

    public AssetBundle GetRoomAssetBundle(string name)
    {
      
        AssetBundle assetBundle;

        if (assetBundleDict.ContainsKey(name))
        {
            return assetBundleDict[name];
        }
        else
        {
            assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + name);

            if (assetBundle)
            {
                assetBundleDict.Add(name, assetBundle);
                Debug.Log("Load assetbundle sucessful : " + name);
            }              
            else
                Debug.LogError("Load assetbundle failed : " + name);
        }

        return assetBundle;
    }

    public void UnloadRoomAssetBundle(string name, bool unloadAllObjects)
    {
        if (assetBundleDict.ContainsKey(name))
        {
            assetBundleDict[name].Unload(unloadAllObjects);
            assetBundleDict.Remove(name);
           
            Debug.Log("Assetbundle has been unloaded : " + name);
        }
        else
        {
            Debug.LogError("Assetbundle not been loaded before : " + name);
        }
    }
}
