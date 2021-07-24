using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestPool", menuName = "ScriptableObjects/Quest/QuestPool", order = 1)]
public class QuestPool : ScriptableObject
{
    protected static QuestPool instance = null;

    public static QuestPool Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.LoadAll<QuestPool>("")[0];
                instance.BuildDictionary();
            }

            return instance;
        }
    }

    private void BuildDictionary()
    {
        dict.Clear();

        for (int i = 0; i < questGroups.Count; i++)
        {
            QuestGroup questGroup = questGroups[i];
            for (int j = 0; j < questGroup.data.Length; j++)
            {
                QuestConfig questConfig = questGroup.data[j];

                if (!dict.ContainsKey(questConfig.name))
                {
                    dict.Add(questConfig.name, questConfig);
                }
            }
        }
    }

    public Dictionary<string, QuestConfig> GetDictionary()
    {
        return dict;
    }

    public QuestConfig GetQuestConfig(string name)
    {
        if (dict.ContainsKey(name))
            return dict[name];
        else
            return null;
    }

    public List<Quest> BuildQuestList(List<int> groupIndices, List<int> poolIndices)
    {
        List<Quest> questList = new List<Quest>();

        questList.Add(questGroups[0].GetRandom(out int idx0).CreateQuestInstance());
        groupIndices.Add(idx0);
        poolIndices.Add(0);

        questList.Add(questGroups[1].GetRandom(out int idx1).CreateQuestInstance());
        groupIndices.Add(idx1);
        poolIndices.Add(1);

        questList.Add(questGroups[2].GetRandom(out int idx2).CreateQuestInstance());
        groupIndices.Add(idx2);
        poolIndices.Add(2);

        List<int> groupIndices1 = GetRandoms<int>(new List<int>(){ 3, 4, 5, 6 }, 2);
        questList.Add(questGroups[groupIndices1[0]].GetRandom(out int idx3).CreateQuestInstance());
        groupIndices.Add(idx3);
        poolIndices.Add(groupIndices1[0]);

        questList.Add(questGroups[groupIndices1[1]].GetRandom(out int idx4).CreateQuestInstance());
        groupIndices.Add(idx4);
        poolIndices.Add(groupIndices1[1]);

        List<int> groupIndices2 = GetRandoms<int>(new List<int>() { 7, 8, 9, 10, 11, 12, 13 }, 5);
        questList.Add(questGroups[groupIndices2[0]].GetRandom(out int idx5).CreateQuestInstance());
        groupIndices.Add(idx5);
        poolIndices.Add(groupIndices2[0]);

        questList.Add(questGroups[groupIndices2[1]].GetRandom(out int idx6).CreateQuestInstance());
        groupIndices.Add(idx6);
        poolIndices.Add(groupIndices2[1]);

        questList.Add(questGroups[groupIndices2[2]].GetRandom(out int idx7).CreateQuestInstance());
        groupIndices.Add(idx7);
        poolIndices.Add(groupIndices2[2]);

        questList.Add(questGroups[groupIndices2[3]].GetRandom(out int idx8).CreateQuestInstance());
        groupIndices.Add(idx8);
        poolIndices.Add(groupIndices2[3]);

        questList.Add(questGroups[groupIndices2[4]].GetRandom(out int idx9).CreateQuestInstance());
        groupIndices.Add(idx9);
        poolIndices.Add(groupIndices2[4]);

        return questList;
    }

    public Quest GetNewQuest(int poolIndex, ref int groupIndex)
    {
        poolIndex = Mathf.Clamp(poolIndex, 0, questGroups.Count - 1);
        groupIndex = (groupIndex + 1) % questGroups[poolIndex].data.Length;

        return questGroups[poolIndex].data[groupIndex].CreateQuestInstance();
    }

    public List<QuestGroup> questGroups;

    private Dictionary<string, QuestConfig> dict = new Dictionary<string, QuestConfig>();

    [Serializable]
    public class QuestGroup
    {
        public QuestConfig[] data;

        public QuestConfig GetRandom(out int index)
        {
            index = UnityEngine.Random.Range(0, data.Length);
            return data[index];
        }
    }

    public List<T> GetRandoms<T>(List<T> inList, int count)
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
};