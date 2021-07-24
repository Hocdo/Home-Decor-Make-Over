using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CollectIconTable : ScriptableObject
{
    [Serializable]
    public class Element
    {
        public string id;
        public Sprite sprite;
    }
    
    protected static CollectIconTable instance;

    public static CollectIconTable Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.LoadAll<CollectIconTable>("")[0];
                instance.BuildDictionary();
            }

            return instance;
        }
    }

    private void BuildDictionary()
    {
        for (int i = 0; i < elements.Length; i++)
        {
            if (!dictionary.ContainsKey(elements[i].id))
            {
                dictionary.Add(elements[i].id, elements[i].sprite);
            }
            else
            {
                Debug.LogError("Duplicate collect icon ID: " + elements[i].id);
            }
        }
    }

    private Dictionary<string, Sprite> dictionary = new Dictionary<string, Sprite>();

    [SerializeField] private Element[] elements;

    public Sprite GetSprite(string name)
    {
        string jewelPackIdPostFix = name + "_" + PlayerData.current.match3Data.jewelPackId;

        Sprite sprite = null;

        if (dictionary.TryGetValue(jewelPackIdPostFix, out sprite))
        {

        }
        else if (dictionary.TryGetValue(name, out sprite))
        {

        }

        return sprite;
    }

#if UNITY_EDITOR
    [ContextMenu("Generate Id")]
    public void GenerateId()
    {
        for (int i = 0; i < elements.Length; i++)
        {
            if (elements[i].sprite != null) elements[i].id = elements[i].sprite.name;
        }
    }
#endif
}
