using System;
using System.Collections;
using System.Collections.Generic;

namespace Decor
{
    [Serializable]
    public class UnlockedRoomData
    {
        public int roomId;
        public int enterTimeCount;
        public List<UnlockedDesignItemData> boughtItemData;
    }

    [Serializable]
    public class UnlockedDesignItemData
    {
        public string id;
        public int variantIndex;
        public int variantUnlockedBits;
    }

    [Serializable]
    public class PlayerHomeDesignData
    {
        public int currentRoomId;

        public List<UnlockedRoomData> unlockedRoomData = new List<UnlockedRoomData>();

        public void SetUnlockRoom(int id)
        {
            if (GetUnlockedRoomData(id) == null)
            {
                unlockedRoomData.Add(new UnlockedRoomData
                {
                    roomId = id,
                    boughtItemData = new List<UnlockedDesignItemData>()
                });
            }
        }

        public UnlockedRoomData GetUnlockedRoomData(int id)
        {
            for (int i = 0; i < unlockedRoomData.Count; i++)
            {
                if (unlockedRoomData[i].roomId == id)
                {
                    return unlockedRoomData[i];
                }
            }

            return null;
        }

        public UnlockedRoomData GetCurrentUnlockedRoomData()
        {
            return GetUnlockedRoomData(currentRoomId);
        }
    }
}

