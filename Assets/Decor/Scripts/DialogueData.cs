using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueDataList", menuName = "ScriptableObjects/DialogueDataList", order = 1)]
public class DialogueData : ScriptableObject
{
    public Dialogue[] data;

    public Dialogue GetRandom()
    {
        return data[UnityEngine.Random.Range(0, data.Length)];
    }
}

public enum DialogueSide
{
    Left,
    Right
}

[Serializable]
public class Dialogue
{
    public string content;

    public DialogueSide size = DialogueSide.Left;

    public bool avatarFlipped = false;

    public DialogueSpeakerData speakerData;

    public string GetContent()
    {
#if UNITY_EDITOR
        Debug.Log(content);
#endif

        return CustomLocalization.Get(content);
    }
}

