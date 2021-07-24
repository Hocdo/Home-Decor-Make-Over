using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestTriggerSpecialBombConfig", menuName = "ScriptableObjects/Quest/QuestTriggerSpecialBombConfig", order = 1)]
public class QuestTriggerSpecialBombConfig : QuestConfigIntTarget
{
    public enum BombType
    {
        HBomb = ChipType.HBomb,
        VBomb = ChipType.VBomb,
        SimpleBomb = ChipType.SimpleBomb,
        Rainbow = ChipType.RainbowBomb
    }

    public BombType[] bombTypes;

    public override Quest CreateQuestInstance()
    {
        return new QuestTriggerSpecialBomb() { config = this };
    }
};

[Serializable]
public class QuestTriggerSpecialBomb : Quest
{
    public int currentCount;

    [NonSerialized]
    public QuestTriggerSpecialBombConfig config;

    public override void Close()
    {
        GlobalEventObserver.TriggerSpecialBombEvent -= OnTriggerSpecialBomb;
    }

    public override void Open()
    {
        GlobalEventObserver.TriggerSpecialBombEvent += OnTriggerSpecialBomb;
    }

    public override QuestConfig GetConfig()
    {
        return config;
    }

    private void OnTriggerSpecialBomb(ChipType type)
    {
        for (int i = 0; i < config.bombTypes.Length; i++)
            if ((int)config.bombTypes[i] == (int)type)
            {
                if (IsCompleted() == false) 
                    currentCount++;

                break;
            }
    }

    public override void SetConfig(QuestConfig cf)
    {
        config = cf as QuestTriggerSpecialBombConfig;
    }

    public override bool IsCompleted()
    {
        return currentCount >= config.targetCount;
    }
};


