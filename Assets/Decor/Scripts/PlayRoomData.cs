using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Decor
{
    public class PlayRoomData 
    {
        public int id;

        protected Dictionary<string, DesignItemData> designItemDataDict = new Dictionary<string, DesignItemData>();

        public void SetRoomId(int aid)
        {
            if (id.Equals(aid) == false)
            {
                id = aid;
                designItemDataDict.Clear();
            }
        }

        public void UpdateData(DesignItemData[] designItemDatas)
        {
            designItemDataDict.Clear();

            for (int i = 0; i < designItemDatas.Length; i++)
            {
                designItemDataDict.Add(designItemDatas[i].id, designItemDatas[i]);
            }
        }

        public DesignItemData GetItemData(string key)
        {
            if (designItemDataDict.TryGetValue(key, out DesignItemData item))
            {
                return item;
            }
            else
            {
                return null;
            }
        }

        public Dictionary<string, DesignItemData> GetDictionary()
        {
            return designItemDataDict;
        }
    }
}

