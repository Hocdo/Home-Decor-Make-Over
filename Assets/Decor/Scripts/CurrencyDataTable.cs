using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CurrencyDataTable", menuName = "ScriptableObjects/CurrencyDataTable", order = 1)]
public class CurrencyDataTable : ScriptableObject
{
    protected static CurrencyDataTable instance;

    public static CurrencyDataTable Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.LoadAll<CurrencyDataTable>("")[0];
            }

            return instance;
        }
    }

    public Sprite GetCurrencySprite(CurrencyType type)
    {
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i].type == type)
            {
                return data[i].sprite;
            }
        }

        return null;
    }

    public CurrencyInfo[] data;

    [Serializable]
    public struct CurrencyInfo
    {
        public CurrencyType type;
        public Sprite sprite;
    }
}

[Serializable]
public class Currency
{
    public CurrencyType type = CurrencyType.Coin;
    public int value;
}
