using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3;

public static class BoosterUtility
{
    private static int[] HeadStarUnlockLevel = new int[] { 0, 15, 17 };

    private static Dictionary<BoosterType, int> IngameUnlockLevel = new Dictionary<Match3.BoosterType, int>
        {
            {BoosterType.Shuffle, 0},
            {BoosterType.Hammer, 11},
            {BoosterType.HBomb, 13},
            {BoosterType.VBomb, 13},
            {BoosterType.CandyPack, 20}
        };

    public static int GetHeadStartUnlockLevel(int index)
    {
        if (HeadStarUnlockLevel.Length > index)
            return HeadStarUnlockLevel[index];

        return 0;
    }

    public static int GetIngameUnlockLevel(BoosterType type)
    {
        if (IngameUnlockLevel.ContainsKey(type))
            return IngameUnlockLevel[type];

        return 0;
    }
}
