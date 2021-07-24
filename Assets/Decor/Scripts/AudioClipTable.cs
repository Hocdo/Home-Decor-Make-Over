using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioTable", menuName = "ScriptableObjects/AudioTable", order = 1)]
public class AudioClipTable : ScriptableObject
{
    protected static AudioClipTable instance;

    public static AudioClipTable Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.LoadAll<AudioClipTable>("")[0];
            }

            return instance;
        }
    }

    public Dictionary<AudioClipId, AudioClip> GetDictionary()
    {
        Dictionary<AudioClipId, AudioClip> audioClipDict = new Dictionary<AudioClipId, AudioClip>();

        for (int i = 0; i < data.Length; i++)
        {
            audioClipDict.Add(data[i].key, data[i].clip);
        }

        return audioClipDict;
    }

    public AudioClipWithKey[] data;

    [Serializable]
    public class AudioClipWithKey
    {
        public AudioClipId key;
        public AudioClip clip;
    }
}

