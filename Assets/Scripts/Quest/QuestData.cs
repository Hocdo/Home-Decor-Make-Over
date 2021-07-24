using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestData
{
    public string currentDateTime;

    public bool unboxProgress1 = false;
    public bool unboxProgress2 = false;
    public bool unboxProgress3 = false;

    public List<string> data = new List<string>();
    public List<string> types = new List<string>();
    public List<int> groupIndices = new List<int>();
    public List<int> poolIndices = new List<int>();

    [NonSerialized]
    public List<Quest> allQuest = new List<Quest>();
}
