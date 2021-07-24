using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestGemChangeConfig", menuName = "ScriptableObjects/Quest/QuestGemChangeConfig", order = 1)]
public class QuestGemChangeConfig : QuestConfigIntTarget
{
    public enum ActionType
    {
        Spend,
        Earn
    }

    public ActionType actionType;

    public override Quest CreateQuestInstance()
    {
        return new QuestGemChange() { config = this };
    }
}

[Serializable]
public class QuestGemChange : Quest
{
    public int currentCount;

    [NonSerialized]
    public QuestGemChangeConfig config;

    public override void Close()
    {
        GlobalEventObserver.ChangeGemEvent-= OnGemChange;
    }

    public override void Open()
    {
        GlobalEventObserver.ChangeGemEvent += OnGemChange;
    }

    public override QuestConfig GetConfig()
    {
        return config;
    }

    private void OnGemChange(int change)
    {
        if (config.actionType == QuestGemChangeConfig.ActionType.Earn && change > 0)
        {           
            if (IsCompleted() == false) currentCount += change;
        }
        else if (config.actionType == QuestGemChangeConfig.ActionType.Spend && change < 0)
        {
            if (IsCompleted() == false) currentCount -= change;
        }
    }

    public override void SetConfig(QuestConfig cf)
    {
        config = cf as QuestGemChangeConfig;
    }

    public override bool IsCompleted()
    {
        return currentCount >= config.targetCount;
    }
}
