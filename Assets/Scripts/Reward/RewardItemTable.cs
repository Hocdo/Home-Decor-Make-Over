using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RewardItemTable", menuName = "ScriptableObjects/RewardItemTable", order = 1)]
public class RewardItemTable : ScriptableObject
{
    protected static RewardItemTable instance;

    public static RewardItemTable Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.LoadAll<RewardItemTable>("")[0];
                instance.GetDictionary();
            }

            return instance;
        }
    }

    public Dictionary<RewardType, GameObject> GetDictionary()
    {
        for (int i = 0; i < data.Length; i++)
        {            
            rewardItemData.Add(data[i].key, data[i].prefab);
        }

        return rewardItemData;
    }

    private Dictionary<RewardType, GameObject> rewardItemData = new Dictionary<RewardType, GameObject>();

    public GameObject Get(RewardType key)
    {
        if (rewardItemData == null || rewardItemData.Count == 0) GetDictionary();

        return GameObject.Instantiate(rewardItemData[key]);
    }

    public RewardItemWithKey[] data;

    [Serializable]
    public class RewardItemWithKey
    {
        public RewardType key;
        public GameObject prefab;
    }
}


