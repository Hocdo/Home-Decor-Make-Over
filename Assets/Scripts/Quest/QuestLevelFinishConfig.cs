using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestLevelFinishConfig", menuName = "ScriptableObjects/Quest/QuestLevelFinishConfig", order = 1)]
public class QuestLevelFinishConfig : QuestConfigIntTarget
{
    public enum FinishState
    {
        Win,
        Lose
    }

    public FinishState finishState;

    public int targetRemainingMove;

    public bool withStreak;

    public override Quest CreateQuestInstance()
    {
        return new QuestLevelFinish() { config = this };
    }
};

[Serializable]
public class QuestLevelFinish : Quest
{ 
    public int currentCount;

    [NonSerialized]
    public QuestLevelFinishConfig config;

    public override void Close()
    {
        GlobalEventObserver.FinishLevelEvent -= OnLevelFinish;
    }

    public override void Open()
    {
        GlobalEventObserver.FinishLevelEvent += OnLevelFinish;
    }

    public override QuestConfig GetConfig()
    {
        return config;
    }

    private void OnLevelFinish(bool result, int remainingMove)
    {
        if (config.finishState == QuestLevelFinishConfig.FinishState.Win)
        {
            if (result == true)
            {
                if (remainingMove >= config.targetRemainingMove)
                    if (IsCompleted() == false)
                        currentCount++;
            }
            else
            {
                if (config.withStreak)
                {
                    currentCount = 0;
                }
            }
        }
        else if (config.finishState == QuestLevelFinishConfig.FinishState.Lose)
        {
            if (result == false)
            {
                if (IsCompleted() == false)
                    currentCount++;
            }
            else
            {
                if (config.withStreak)
                {
                    currentCount = 0;
                }
            }
        }
    }

    public override void SetConfig(QuestConfig cf)
    {
        config = cf as QuestLevelFinishConfig;
    }

    public override bool IsCompleted()
    {
        return currentCount >= config.targetCount;
    }
}
