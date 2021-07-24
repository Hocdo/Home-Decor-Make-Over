using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    [CreateAssetMenu(fileName = "BoosterTable", menuName = "ScriptableObjects/BoosterTable", order = 1)]
    public class BoosterInfoTable : ScriptableObject
    {
        private static BoosterInfoTable instance;
        
        [SerializeField] private BoosterInfo[] data;
        
        public static BoosterInfoTable Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.LoadAll<BoosterInfoTable>("")[0];
                }

                return instance;
            }
        }
        
        public BoosterInfo Find(BoosterType type)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].type == type) 
                    return data[i];
            }

            return null;
        }

        //public BoosterInfo Find(string key)
        //{
        //    for (int i = 0; i < data.Length; i++)
        //    {
        //        if (data[i].key.Equals(key))
        //            return data[i];
        //    }

        //    return null;
        //}

        [Serializable]
        public class BoosterInfo
        {
            public BoosterType type;
            public Sprite sprite;
            public string guide;
        }
    }
}
