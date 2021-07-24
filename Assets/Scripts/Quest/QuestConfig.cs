using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestConfig : ScriptableObject
{
    public int rewardGemCount = 0;

    public abstract Quest CreateQuestInstance();
}

public abstract class QuestConfigIntTarget : QuestConfig
{
    public int targetCount;
}