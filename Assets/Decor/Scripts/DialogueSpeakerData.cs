using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSpeakerData", menuName = "ScriptableObjects/DialogueSpeakerData", order = 1)]
public class DialogueSpeakerData : ScriptableObject
{
    public string name;

    public GameObject avatarPrefab;
}
