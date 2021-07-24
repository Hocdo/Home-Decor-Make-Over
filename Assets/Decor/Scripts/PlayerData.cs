using System;
using System.Collections.Generic;
using Decor;
using Match3;
using UnityEngine;

[Serializable]
public class PlayerData
{
    #region StaticConfig
#if UNITY_EDITOR
    private static readonly string directory = "D:/";
#else
    private static readonly string directory = Application.persistentDataPath;
#endif
    private static string playerDataFileName = "playerdata" + ".txt";

    public static string GetFilePath()
    {
        return directory + playerDataFileName;
    }
    #endregion

    public static PlayerData current;

    public string localFirstActiveTime; 
    public string localLastActiveTime;

    public int enterGameTimeCount;
    public int cointCount;
    public int gemCount;

    public int freeGemAdCounter;

    public bool purchasedStartPack;
    public string starterPackExpiredTime;

    public int weeklyPackPurchaseNo = -5;
    public int monthlyPackPurchaseNo = -5;

    public string lastReceiveDailyBonusTime;
    public int currentDailyBonusDay;
    public string nextFreeSpinAdsTime;

    public string lastSpinTime;

    public bool noAds = false;
    public bool appRated = false;
    
    public Stamina stamina = new Stamina();

    public PiggyBankData piggyBankData = new PiggyBankData();

    public PlayerMatch3Data match3Data = new PlayerMatch3Data();
    public PlayerHomeDesignData homeDesignData = new PlayerHomeDesignData();

    public QuestData questData = new QuestData();

    public TempData tempData = new TempData();

    public void AddGem(int count)
    {
        gemCount += count;

        if (gemCount < 0)
            gemCount = 0;

        if (count > 0)
        {
            tempData.earnedGemCount += count;

            if (tempData.earnedGemCount >= 800 && tempData.push_earnedGemCount_event == false)
            {
                tempData.push_earnedGemCount_event = true;
                //AppEventTracker.PushEventGemEarned_Gequal800();
                Firebase.Analytics.FirebaseAnalytics.LogEvent("Feature_selected_earn_gem_800");
            }

            //AppEventTracker.LogEventGem(Analytics.Feature_GEM.TYPE._in, count);
        }
        else
        {
            //AppEventTracker.LogEventGem(Analytics.Feature_GEM.TYPE._out, -count);
        }

        //AppEventTracker.LogEventUserProperty_Gem(gemCount);

        GlobalEventObserver.InvokeChangeGemEvent(count);
    }

    public void AddCoin(int count)
    {
        cointCount += count;

        if (cointCount < 0)
            cointCount = 0;

        //if (count > 0)
        //{
            //AppEventTracker.LogEventCoin(Analytics.Feature_COIN.TYPE._in, count);
        //}
        //else
        //{
            //AppEventTracker.LogEventCoin(Analytics.Feature_COIN.TYPE._out, -count);
       // }

        //AppEventTracker.LogEventUserProperty_Coin(cointCount);
    }

    public void AddStamina(int count)
    {
        stamina.Add(count);
    }
}
