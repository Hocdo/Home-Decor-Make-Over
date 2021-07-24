using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestUseInGameBoosterConfig", menuName = "ScriptableObjects/Quest/QuestUseInGameBoosterConfig", order = 1)]
public class QuestUseInGameBoosterConfig : QuestConfigIntTarget
{
    public Match3.BoosterType[] boosterTypes;

    public override Quest CreateQuestInstance()
    {
        return new QuestUseInGameBooster { config = this };
    }
};

[Serializable]
public class QuestUseInGameBooster : Quest
{
    public int currentCount;

    [NonSerialized]
    public QuestUseInGameBoosterConfig config;

    public override void Close()
    {
        GlobalEventObserver.UseIngameBoosterEvent -= OnInGameBoosterUse;  
    }

    public override void Open()
    {
        GlobalEventObserver.UseIngameBoosterEvent += OnInGameBoosterUse;
    }

    public override QuestConfig GetConfig()
    {
        return config;
    }

    private void OnInGameBoosterUse(Match3.BoosterType aboosterType)
    {
        for (int i = 0; i < config.boosterTypes.Length; i++)
            if (config.boosterTypes[i] == aboosterType)
            {
                if (IsCompleted() == false) currentCount++;
                break;
            }
    }

    public override void SetConfig(QuestConfig cf)
    {
        config = cf as QuestUseInGameBoosterConfig;
    }

    public override bool IsCompleted()
    {
        return currentCount >= config.targetCount;
    }
};
