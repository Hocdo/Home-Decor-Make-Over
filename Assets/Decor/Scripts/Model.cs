using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Decor;
using Match3;

public class Model : Singleton<Model>
{
    private PlayerData playerData;

    public bool isLoaded = false;

    public PlayRoomData playRoomData = new PlayRoomData();

    public Model()
    {

    }

    public void UpdateCurrentRoomItemData(DesignItemData[] adesignItemDatas)
    {
        if (playRoomData == null)
            playRoomData = new PlayRoomData();

        playRoomData.id = playerData.homeDesignData.currentRoomId;
        playRoomData.UpdateData(adesignItemDatas);

        List<UnlockedDesignItemData> unlockedDataList = playerData.homeDesignData.GetCurrentUnlockedRoomData().boughtItemData;
        for (int i = 0; i < unlockedDataList.Count; i++)
        {            
            var unlockedData = unlockedDataList[i];
            var designItemData = playRoomData.GetItemData(unlockedData.id);

            if (designItemData != null)
            {
                designItemData.unlocked = true;
                designItemData.variantIndex = unlockedData.variantIndex;
                designItemData.variantUnlockedBits = unlockedData.variantUnlockedBits;
            }            
        }
    }

    public bool UpdatePlayerHomeDesignData()
    {        
        if (playRoomData.id == 0) 
            return false;

        List<UnlockedDesignItemData> unlockedDataList = playerData.homeDesignData.GetCurrentUnlockedRoomData().boughtItemData;
        unlockedDataList.Clear();

        var dict = playRoomData.GetDictionary();
        foreach (var item in dict)
        {
            var designItemData = item.Value;

            if (designItemData.IsUnlocked())
            {
                UnlockedDesignItemData unlockedData = new UnlockedDesignItemData();

                unlockedData.id = designItemData.id;
                unlockedData.variantIndex = designItemData.variantIndex;
                unlockedData.variantUnlockedBits = designItemData.variantUnlockedBits;

                unlockedDataList.Add(unlockedData);
            }
        }

        playerData.homeDesignData.currentRoomId = playRoomData.id;

        return true;           
    }

    public void Reset()
    {
        playerData = new PlayerData();

        string filePath = PlayerData.GetFilePath();
        string playerDataJson = JsonUtility.ToJson(playerData);
        File.WriteAllText(filePath, playerDataJson);

    }
    public void Save()
    {
        if (playerData == null || !isLoaded) return;

        if (playerData.homeDesignData.currentRoomId <= 0) return;

        if (playRoomData != null && playRoomData.GetDictionary().Count < 15) return;

        playerData.localLastActiveTime = System.DateTime.Now.ToString();

        QuestData questData = playerData.questData;
        questData.data.Clear();
        questData.types.Clear();

        for (int i = 0; i < questData.allQuest.Count; i++)
        {
            questData.data.Add(JsonUtility.ToJson(questData.allQuest[i]));
            questData.types.Add(questData.allQuest[i].GetConfig().name);
        }

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
        if (playerData != null && string.IsNullOrEmpty(playerData.localFirstActiveTime))
        {
            playerData.localFirstActiveTime = System.DateTime.Now.ToString();
            playerData.localLastActiveTime = playerData.localFirstActiveTime;
        }

        if (playerData == null)
        {
            playerData = new PlayerData();           
            playerData.localFirstActiveTime = System.DateTime.Now.ToString();
            playerData.localLastActiveTime = playerData.localFirstActiveTime;
            playerData.enterGameTimeCount = 0;
            playerData.cointCount = 350;
            playerData.gemCount = 0;
            playerData.stamina.count = 5;

            playerData.homeDesignData.currentRoomId = 1;
            playerData.homeDesignData.SetUnlockRoom(1);

            playerData.match3Data.level = 1;          
        }

        if (playerData.match3Data.showTutorialIngameBooster == null || playerData.match3Data.showTutorialIngameBooster.Length < 5)
            playerData.match3Data.showTutorialIngameBooster = new bool[5];

        if (playerData.match3Data.ingameBooster == null || playerData.match3Data.ingameBooster.Length < 5)
        {
            playerData.match3Data.ingameBooster = new int[5];

            playerData.match3Data.ingameBooster[0] = 2;
            playerData.match3Data.ingameBooster[1] = 2;
            playerData.match3Data.ingameBooster[2] = 2;
            playerData.match3Data.ingameBooster[3] = 2;
            playerData.match3Data.ingameBooster[4] = 2;
        }

        if (playerData.match3Data.showTutorialStartBooster == null || playerData.match3Data.showTutorialStartBooster.Length < 3)
            playerData.match3Data.showTutorialStartBooster = new bool[3];

        if (playerData.match3Data.headStartBoosterCounts == null || playerData.match3Data.headStartBoosterCounts.Length < 3)
        {
            playerData.match3Data.headStartBoosterCounts = new int[3];

            playerData.match3Data.headStartBoosterCounts[0] = 2;
            playerData.match3Data.headStartBoosterCounts[1] = 2;
            playerData.match3Data.headStartBoosterCounts[2] = 2;
        }

        if (string.IsNullOrEmpty(playerData.match3Data.jewelPackId))
        {
            playerData.match3Data.jewelPackId = "jp";
        }

        if (playerData.match3Data.unlockedJewelPackIds.Count == 0)
        {
            playerData.match3Data.unlockedJewelPackIds.Add("jp");
        }

        var questData = playerData.questData;
        for (int i = 0; i < questData.data.Count; i++)
        {
            QuestConfig questConfig = QuestPool.Instance.GetQuestConfig(questData.types[i]);

            if (questConfig)
            {
                Quest quest = (Quest)JsonUtility.FromJson(questData.data[i], questConfig.CreateQuestInstance().GetType());

                if (quest != null)
                {
                    quest.SetConfig(questConfig);
                    questData.allQuest.Add(quest);
                }               
            }                
        }

        isLoaded = true;
        PlayerData.current = playerData;
    }
}

