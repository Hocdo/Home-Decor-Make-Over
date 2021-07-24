using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestUseHeadStartBoosterConfig", menuName = "ScriptableObjects/Quest/QuestUseHeadStartBoosterConfig", order = 1)]
public class QuestUseHeadStartBoosterConfig : QuestConfigIntTarget
{
    public Match3.HeadStartBoosterType[] boosterTypes;

    public override Quest CreateQuestInstance()
    {
        return new QuestUseHeadStartBooster { config = this };
    }
};

[Serializable]
public class QuestUseHeadStartBooster : Quest
{
    public int currentCount;

    [NonSerialized]
    public QuestUseHeadStartBoosterConfig config;

    public override void Close()
    {
        GlobalEventObserver.UseHeadStartBoosterEvent -= OnHeadStartBoosterUse;   
    }

    public override void Open()
    {
        GlobalEventObserver.UseHeadStartBoosterEvent += OnHeadStartBoosterUse;
    }

    public override QuestConfig GetConfig()
    {
        return config;
    }

    private void OnHeadStartBoosterUse(Match3.HeadStartBoosterType aboosterType)
    {
        for (int i = 0; i < config.boosterTypes.Length; i++)
            if (config.boosterTypes[i] == aboosterType)
            {
                if (IsCompleted() == false)
                    currentCount++;

                break;
            }
    }

    public override void SetConfig(QuestConfig cf)
    {
        config = cf as QuestUseHeadStartBoosterConfig;
    }

    public override bool IsCompleted()
    {
        return currentCount >= config.targetCount;
    }
};

