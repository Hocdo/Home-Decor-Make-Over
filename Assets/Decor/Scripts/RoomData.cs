using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomData", menuName = "ScriptableObjects/RoomData", order = 1)]
public class RoomData : ScriptableObject
{
    public static readonly string RoomPrefabName = "room";

    public static readonly string ItemsOfRoomName = "items";

    public string assetBundleName;

    public int id;

    public int nextRoomId;

    public int amountOfItemToUnlockNextRoom;

    public string name;

    public int maxItemCount;

    public Sprite circleSprite;

    public string description;

    public Dialogue[] startDialogs;

    public Dialogue[] completeDialogs;

    public AssetBundle GetAssetBundle()
    {
        if (!assetBundle)
            assetBundle = RoomAssetBundleManager.Instance.GetRoomAssetBundle(assetBundleName);

        return assetBundle;
    }

    public void UnloadAssetBundle(bool unloadAllObjects)
    {
        RoomAssetBundleManager.Instance.UnloadRoomAssetBundle(assetBundleName, unloadAllObjects);
        assetBundle = null;
    }

    private AssetBundle assetBundle;
}
