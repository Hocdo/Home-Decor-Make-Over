using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Quest 
{
    [SerializeField]
    protected bool claimed = false;

    public void Claim()
    {
        claimed = true;
    }

    public bool Claimed() { return claimed; }

    public abstract bool IsCompleted();

    public abstract void Open();

    public abstract void Close();

    public abstract void SetConfig(QuestConfig cf);

    public abstract QuestConfig GetConfig();
}








