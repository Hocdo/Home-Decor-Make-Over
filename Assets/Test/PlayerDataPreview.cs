using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerDataPreview : MonoBehaviour
{
#if UNITY_EDITOR
    public PlayerData playerData;

    public void Save()
    {
        string filePath = PlayerData.GetFilePath();
        string playerDataJson = JsonUtility.ToJson(playerData);
        File.WriteAllText(filePath, playerDataJson);
    }

    public void Load()
    {
        string filePath = PlayerData.GetFilePath();

        FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate);
        StreamReader sr = new StreamReader(fileStream);
        string playerDataJson = sr.ReadToEnd();
        sr.Close();
        fileStream.Close();

        playerData = JsonUtility.FromJson<PlayerData>(playerDataJson);

        if (playerData == null)
        {
            playerData = new PlayerData();
            playerData.localFirstActiveTime = System.DateTime.Now.ToString();
            playerData.enterGameTimeCount = 0;
            playerData.cointCount = 350;
            playerData.gemCount = 0;
            playerData.stamina.count = 5;

            playerData.homeDesignData.currentRoomId = 1;
            playerData.homeDesignData.SetUnlockRoom(1);

            playerData.match3Data.level = 1;
        }

        if (playerData.match3Data.ingameBooster == null || playerData.match3Data.ingameBooster.Length < 5)
        {
            playerData.match3Data.ingameBooster = new int[5];

            playerData.match3Data.ingameBooster[0] = 2;
            playerData.match3Data.ingameBooster[1] = 2;
            playerData.match3Data.ingameBooster[2] = 2;
            playerData.match3Data.ingameBooster[3] = 2;
            playerData.match3Data.ingameBooster[4] = 2;
        }

        if (playerData.match3Data.headStartBoosterCounts == null || playerData.match3Data.headStartBoosterCounts.Length < 3)
        {
            playerData.match3Data.headStartBoosterCounts = new int[3];

            playerData.match3Data.headStartBoosterCounts[0] = 2;
            playerData.match3Data.headStartBoosterCounts[1] = 2;
            playerData.match3Data.headStartBoosterCounts[2] = 2;
        }
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerDataPreview))]
public class PlayerDataPreviewEditor : Editor
{
    PlayerDataPreview playerDataPreview;

    void OnEnable()
    {
        playerDataPreview = target as PlayerDataPreview;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Load"))
        {
            playerDataPreview.Load();
        }

        if (GUILayout.Button("Save"))
        {
            playerDataPreview.Save();
        }
    }
}
#endif
