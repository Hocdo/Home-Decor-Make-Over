using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LanguageFontTable : ScriptableObject
{
    protected static LanguageFontTable instance;

    public static LanguageFontTable Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.LoadAll<LanguageFontTable>("")[0];
                instance.GetDictionary();
            }

            return instance;
        }
    }

    private Dictionary<LanguageType, Font> GetDictionary()
    {
        dict.Clear();
        for (int i = 0; i < data.Length; i++)
        {
            dict.Add(data[i].key, data[i].font);
        }

        return dict;
    }

    public LanguageWithFont[] data;

    private Dictionary<LanguageType, Font> dict = new Dictionary<LanguageType, Font>();

    [Serializable]
    public class LanguageWithFont
    {
        public LanguageType key;
        public Font font;
    }

    public Font GetFont(LanguageType language)
    {
        if (dict.TryGetValue(language, out Font font))
        {
            return font;
        }
        else
        {
            return data[0].font;
        }        
    }
}
