using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestDecorConfig", menuName = "ScriptableObjects/Quest/QuestDecorConfig", order = 1)]
public class QuestDecorConfig : QuestConfigIntTarget
{
    public override Quest CreateQuestInstance()
    {
        return new QuestDecor() { config = this };
    }
};

[Serializable]
public class QuestDecor : Quest
{
    public int currentCount;

    [NonSerialized]
    public QuestDecorConfig config;

    public override void Close()
    {
        GlobalEventObserver.DecorUnlockEvent -= OnDecorUnlock;
    }

    public override void Open()
    {
        GlobalEventObserver.DecorUnlockEvent += OnDecorUnlock;
    }

    public override QuestConfig GetConfig()
    {
        return config;
    }

    private void OnDecorUnlock()
    {
        if (IsCompleted() == false) 
            currentCount++;
    }

    public override void SetConfig(QuestConfig cf)
    {
        config = cf as QuestDecorConfig;
    }

    public override bool IsCompleted()
    {
        return currentCount >= config.targetCount;
    }
};



