 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerializeEventHandler : MonoBehaviour
{
    public bool saveData = true;

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);      
    }

    public void OnApplicationQuit()
    {
        SaveData();
    }

    public void OnApplicationPause(bool pause)
    {
        if (pause)
        {            
            SaveData();
        }
    }

    public void SaveData()
    {
#if !UNITY_EDITOR
        saveData = true;
#endif
        if (saveData)
        {
            if (LoadSceneUtility.CurrentSceneName == LoadSceneUtility.HomeDesignSceneName || LoadSceneUtility.CurrentSceneName == LoadSceneUtility.Match3SceneName)
            {
                if (Model.Instance.isLoaded)
                {
                    if (Model.Instance.UpdatePlayerHomeDesignData())
                    {
                        Model.Instance.Save();
                    }
                }                    
            }           

            PlayerPrefs.Save();
        }
    }
}
