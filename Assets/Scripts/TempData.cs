using System;
using System.Collections.Generic;
using Decor;
using Match3;

[Serializable]
public class TempData
{
    public int rewardedVideoCount;
    public int extra3MovesRewardCount;

    public int winLevelCount;
    public int loseLevelCount;

    public bool push_level_gequal44_event;

    public int earnedGemCount;
    public bool push_earnedGemCount_event;

    public bool push_freeGems7th_event;

    public float spentIAP;
    public bool push_spentIAP_event;

    public bool push_retention_day7;
}