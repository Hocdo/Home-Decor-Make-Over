using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QuestUtility
{
    public static bool NeedToResetCurrentDate()
    {
        var questData = PlayerData.current.questData;

        if (string.IsNullOrEmpty(questData.currentDateTime))
        {
            ResetCurrentDate();

            return true;
        }
        else
        {
            var currentDateTime = DateTimeUtility.Get(questData.currentDateTime);

            if (currentDateTime.Date != DateTimeUtility.GetUtcNow().Date)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public static void ResetCurrentDate()
    {
        var questData = PlayerData.current.questData;
        questData.currentDateTime = DateTimeUtility.GetUtcNow().ToString();
    }

    public static void BuildQuestList()
    {
        var questData = PlayerData.current.questData;
        questData.groupIndices.Clear();
        questData.poolIndices.Clear();
        questData.unboxProgress1 = false;
        questData.unboxProgress2 = false;
        questData.unboxProgress3 = false;

        PlayerData.current.questData.allQuest = QuestPool.Instance.BuildQuestList(questData.groupIndices, questData.poolIndices);
    }

    public static void UpdateQuestList()
    {
        var questData = PlayerData.current.questData;
        for (int i = 0; i < questData.allQuest.Count; i++)
        {
            Quest quest = questData.allQuest[i];
            if (quest.IsCompleted() == false)
                quest.Open();
        }
    }

    public static object[] GetQuestDescription(this Quest quest)
    {
        object[] results = new object[3];

        if (quest is QuestLevelFinish)
        {
            var questLevelFinish = quest as QuestLevelFinish;

            if (questLevelFinish.config.finishState == QuestLevelFinishConfig.FinishState.Win)
            {
                if (questLevelFinish.config.withStreak)
                {
                    string text = CustomLocalization.Get("task_win_streak");
                    results[0] = text.Replace("xxx", questLevelFinish.config.targetCount.ToString());
                }
                else if (questLevelFinish.config.targetRemainingMove >= 5)
                {
                    string text = CustomLocalization.Get("task_win_5moves");
                    results[0] = text.Replace("xxx", questLevelFinish.config.targetCount.ToString());
                }
                else if (questLevelFinish.config.targetRemainingMove == 0)
                {
                    string text = CustomLocalization.Get("task_clear_level");
                    results[0] = text.Replace("xxx", questLevelFinish.config.targetCount.ToString());
                }
            }
            else if (questLevelFinish.config.finishState == QuestLevelFinishConfig.FinishState.Lose)
            {
                string text = CustomLocalization.Get("task_lose_level");
                results[0] = text.Replace("xxx", questLevelFinish.config.targetCount.ToString());
            }

            results[1] = questLevelFinish.currentCount;
            results[2] = questLevelFinish.config.targetCount;
        }
        else if (quest is QuestUseInGameBooster)
        {
            var questUseInGameBooster = quest as QuestUseInGameBooster;

            if (questUseInGameBooster.config.boosterTypes.Length == 5)
            {
                results[0] = "Use " + questUseInGameBooster.config.targetCount.ToString() + " In game Booster";
            }
            else if ((int)questUseInGameBooster.config.boosterTypes[0] == (int)Match3.BoosterType.Shuffle)
            {
                string text = CustomLocalization.Get("task_use_shuffle");
                results[0] = text.Replace("xxx", questUseInGameBooster.config.targetCount.ToString());
            }
            else if ((int)questUseInGameBooster.config.boosterTypes[0] == (int)Match3.BoosterType.CandyPack)
            {
                string text = CustomLocalization.Get("task_use_candy");
                results[0] = text.Replace("xxx", questUseInGameBooster.config.targetCount.ToString());
            }
            else if ((int)questUseInGameBooster.config.boosterTypes[0] == (int)Match3.BoosterType.Hammer)
            {
                string text = CustomLocalization.Get("task_use_hammer");                
                results[0] = text.Replace("xxx", questUseInGameBooster.config.targetCount.ToString());
            }
            else if ((int)questUseInGameBooster.config.boosterTypes[0] == (int)Match3.BoosterType.HBomb)
            {
                string text = CustomLocalization.Get("task_use_hrocket");              
                results[0] = text.Replace("xxx", questUseInGameBooster.config.targetCount.ToString());
            }
            else if ((int)questUseInGameBooster.config.boosterTypes[0] == (int)Match3.BoosterType.VBomb)
            {
                string text = CustomLocalization.Get("task_use_vrocket");               
                results[0] = text.Replace("xxx", questUseInGameBooster.config.targetCount.ToString());
            }

            results[1] = questUseInGameBooster.currentCount;
            results[2] = questUseInGameBooster.config.targetCount;
        }
        else if (quest is QuestUseHeadStartBooster)
        {
            var questUseHeadstartBooster = quest as QuestUseHeadStartBooster;

            if ((int)questUseHeadstartBooster.config.boosterTypes.Length == 3)
            {
                string text = CustomLocalization.Get("task_use_headstart");                
                results[0] = text.Replace("xxx", questUseHeadstartBooster.config.targetCount.ToString());
            }
            else if ((int)questUseHeadstartBooster.config.boosterTypes[0] == (int)Match3.HeadStartBoosterType.Move)
            {
                string text = CustomLocalization.Get("task_use_move");                
                results[0] = text.Replace("xxx", questUseHeadstartBooster.config.targetCount.ToString());
            }
            else if ((int)questUseHeadstartBooster.config.boosterTypes[0] == (int)Match3.HeadStartBoosterType.Rocket)
            {
                string text = CustomLocalization.Get("task_use_rocket");                
                results[0] = text.Replace("xxx", questUseHeadstartBooster.config.targetCount.ToString());
            }
            else if ((int)questUseHeadstartBooster.config.boosterTypes[0] == (int)Match3.HeadStartBoosterType.Bomb)
            {
                string text = CustomLocalization.Get("task_use_bomb");                
                results[0] = text.Replace("xxx", questUseHeadstartBooster.config.targetCount.ToString());
            }

            results[1] = questUseHeadstartBooster.currentCount;
            results[2] = questUseHeadstartBooster.config.targetCount;
        }
        else if (quest is QuestTriggerSpecialBomb)
        {
            var questTriggerSpecialBomb = quest as QuestTriggerSpecialBomb;

            if ((int)questTriggerSpecialBomb.config.bombTypes[0] == (int)ChipType.VBomb)
            {
                string text = CustomLocalization.Get("task_trigger_vrocket");          
                results[0] = text.Replace("xxx", questTriggerSpecialBomb.config.targetCount.ToString());
            }
            else if ((int)questTriggerSpecialBomb.config.bombTypes[0] == (int)ChipType.HBomb)
            {
                string text = CustomLocalization.Get("task_trigger_hrocket");             
                results[0] = text.Replace("xxx", questTriggerSpecialBomb.config.targetCount.ToString());
            }
            else if ((int)questTriggerSpecialBomb.config.bombTypes[0] == (int)ChipType.SimpleBomb)
            {
                string text = CustomLocalization.Get("task_trigger_bomb");              
                results[0] = text.Replace("xxx", questTriggerSpecialBomb.config.targetCount.ToString());
            }
            else if ((int)questTriggerSpecialBomb.config.bombTypes[0] == (int)ChipType.RainbowBomb)
            {
                string text = CustomLocalization.Get("task_trigger_rainbow");           
                results[0] = text.Replace("xxx", questTriggerSpecialBomb.config.targetCount.ToString());
            }

            results[1] = questTriggerSpecialBomb.currentCount;
            results[2] = questTriggerSpecialBomb.config.targetCount;
        }
        else if (quest is QuestGemChange)
        {
            var questGemChange = quest as QuestGemChange;

            if (questGemChange.config.actionType == QuestGemChangeConfig.ActionType.Earn)
            {
                string text = CustomLocalization.Get("task_earn_gem");            
                results[0] = text.Replace("xxx", questGemChange.config.targetCount.ToString());
            }
            else if (questGemChange.config.actionType == QuestGemChangeConfig.ActionType.Spend)
            {
                string text = CustomLocalization.Get("task_spend_gem");             
                results[0] = text.Replace("xxx", questGemChange.config.targetCount.ToString());
            }

            results[1] = questGemChange.currentCount;
            results[2] = questGemChange.config.targetCount;
        }
        else if (quest is QuestDecor)
        {
            var questGemChange = quest as QuestDecor;
            string text = CustomLocalization.Get("task_decor");         
            results[0] = text.Replace("xxx", questGemChange.config.targetCount.ToString());

            results[1] = questGemChange.currentCount;
            results[2] = questGemChange.config.targetCount;
        }

        return results;
    }

    public static List<T> GetRandoms<T>(List<T> inList, int count)
    {
        List<T> sourceList = new List<T>(inList);
        List<T> targetList = new List<T>();

        while (targetList.Count < count && sourceList.Count > 0)
        {
            int idx = UnityEngine.Random.Range(0, sourceList.Count);
            targetList.Add(sourceList[idx]);
            sourceList.RemoveAt(idx);
        }

        return targetList;
    }
}
