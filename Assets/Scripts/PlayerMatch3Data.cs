using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    [Serializable]
    public class PlayerMatch3Data
    {
        public int level = 1;
        public int[] ingameBooster;
        public int[] headStartBoosterCounts;

        public bool[] showTutorialStartBooster;
        public bool[] showTutorialIngameBooster;

        public string jewelPackId;
        public List<string> unlockedJewelPackIds = new List<string>();

        public int GetIngameBoosterCount(BoosterType boosterType)
        {
            int index = (int)boosterType;
            return ingameBooster[index];
        }

        public int GetHeadStartBoosterCount(HeadStartBoosterType boosterType)
        {
            int index = (int)boosterType;
            return headStartBoosterCounts[index];
        }

        public void AddIngameBooster(BoosterType boosterType, int count)
        {
            int index = (int)boosterType;
            ingameBooster[index] = Mathf.Max(0, ingameBooster[index] + count);

            //Analytics.Feature_ITEM.NAME_ITEM itemName = Analytics.Feature_ITEM.NAME_ITEM.NONE;

            //switch (boosterType)
            //{
            //    case BoosterType.CandyPack:
            //        itemName = Analytics.Feature_ITEM.NAME_ITEM._candy_pack;
            //        break;
            //    case BoosterType.Hammer:
            //        itemName = Analytics.Feature_ITEM.NAME_ITEM._magic_hammer;
            //        break;
            //    case BoosterType.Shuffle:
            //        itemName = Analytics.Feature_ITEM.NAME_ITEM._shuffe;
            //        break;
            //    case BoosterType.HBomb:
            //        itemName = Analytics.Feature_ITEM.NAME_ITEM._horizontal_rocket;
            //        break;
            //    case BoosterType.VBomb:
            //        itemName = Analytics.Feature_ITEM.NAME_ITEM._vertical_rocket;
            //        break;
            //    default:
            //        break;
            //}

            //if (count > 0)
            //{                
            //    AppEventTracker.LogEventRewardItem(itemName, count);
            //}
            //else
            //{
            //    AppEventTracker.LogEventUseItem(itemName, PlayerData.current.match3Data.level);
            //}         
        }

        public void AddHeadStartBooster(HeadStartBoosterType boosterType, int count)
        {
            int index = (int)boosterType;
            headStartBoosterCounts[index] = Mathf.Max(0, headStartBoosterCounts[index] + count);

            //Analytics.Feature_ITEM.NAME_ITEM itemName = Analytics.Feature_ITEM.NAME_ITEM.NONE;

            //switch (boosterType)
            //{
            //    case HeadStartBoosterType.Move:
            //        itemName = Analytics.Feature_ITEM.NAME_ITEM._3moves;
            //        break;
            //    case HeadStartBoosterType.Rocket:
            //        itemName = Analytics.Feature_ITEM.NAME_ITEM._rocket;
            //        break;
            //    case HeadStartBoosterType.Bomb:
            //        itemName = Analytics.Feature_ITEM.NAME_ITEM._bomb;
            //        break;
            //    default:
            //        break;
            //}

            //if (count > 0)
            //{
            //    AppEventTracker.LogEventRewardItem(itemName, count);
            //}
            //else
            //{
            //    AppEventTracker.LogEventUseItem(itemName, PlayerData.current.match3Data.level);
            //}
        }
    }
}