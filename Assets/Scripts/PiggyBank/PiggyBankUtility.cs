using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct IntegerRange
{
    public int min;
    public int max;

    public int GetRandom()
    {
        return UnityEngine.Random.Range(min, max + 1);
    }
}

public static class PiggyBankUtility
{
    public static int levelToShow = 12;

    public static int periodDuration = 24 * 3600;

    public static int periodCooldown = 12 * 3600;//

    public static bool Available()
    {
        PlayerData playerData = PlayerData.current;

        if (playerData.match3Data.level < levelToShow)
            return false;

        PiggyBankData piggyBankData = playerData.piggyBankData;

        if (piggyBankData.removed)
            return false;

        if (Get_RemainingTime_To_NextLevelCountDown() > 0)
            return false;

        if (Get_RemainingTime_To_PeriodExpired() <= 0 && string.IsNullOrEmpty(piggyBankData.periodExpireTime) == false)
        {
            if (Get_RemainingTime_To_PeriodAvailable() > 0)
                return false;
        }

        return true;
    }

    public static void Initialize()
    {
        PiggyBankData piggyBankData = PlayerData.current.piggyBankData;
        if (string.IsNullOrEmpty(piggyBankData.nextLevelAvailaibleTime))
        {
            piggyBankData.nextLevelAvailaibleTime = DateTimeUtility.GetUtcNow().AddSeconds(-0.01f).ToString();
        }

        if (Get_RemainingTime_To_NextLevelCountDown() > 0)
        {

        }
        else
        {
            if (CanSmash())
            {
                if (Get_RemainingTime_To_PeriodExpired() > 0)
                {

                }
                else // miss smash
                {
                    if (Get_RemainingTime_To_PeriodAvailable() > 0)
                    {
                        if (piggyBankData.enterPeriod)
                        {
                            OnSmashMissed();
                        }
                    }
                    else
                    {
                        if (piggyBankData.enterPeriod)
                        {
                            OnSmashMissed();
                        }

                        StartPeriod();
                    }
                }
            }
        }
    }

    public static bool CanSmash()
    {
        PiggyBankData piggyBankData = PlayerData.current.piggyBankData;

        return piggyBankData.gemCount >= GetSmashRange(piggyBankData.level).min;
    }

    public static void OnStash(int bonusGemCount)
    {
        PiggyBankData piggyBankData = PlayerData.current.piggyBankData;
        var smashRange = GetSmashRange(piggyBankData.level);

        if (piggyBankData.gemCount < smashRange.min && piggyBankData.gemCount + bonusGemCount >= smashRange.min)
        {
            StartPeriod();
        }

        piggyBankData.gemCount = Mathf.Min(piggyBankData.gemCount + bonusGemCount, smashRange.max);
    }

    private static void StartPeriod()
    {
        PiggyBankData piggyBankData = PlayerData.current.piggyBankData;

        piggyBankData.enterPeriod = true;
        piggyBankData.periodExpireTime = DateTimeUtility.GetUtcNow().AddSeconds(periodDuration).ToString();
        piggyBankData.periodAvailableTime = DateTimeUtility.GetUtcNow().AddSeconds(periodDuration + periodCooldown).ToString();
    }

    public static void OnSmashMissed()
    {
        PiggyBankData piggyBankData = PlayerData.current.piggyBankData;
        piggyBankData.enterPeriod = false;
        piggyBankData.missingCount++;

        //piggyBankData.periodExpireTime = DateTimeUtility.Get(piggyBankData.periodExpireTime)
        //    .AddSeconds(periodDuration + periodCooldown).ToString();
        //piggyBankData.periodAvailableTime = DateTimeUtility.Get(piggyBankData.periodAvailableTime)
        //    .AddSeconds(periodDuration + periodCooldown).ToString();

        if (piggyBankData.level == 1)
        {
            if (piggyBankData.missingCount == 6)
            {
                piggyBankData.removed = true;
            }
        }
        else if (piggyBankData.level == 2)
        {

        }
        else if (piggyBankData.level == 3)
        {
            piggyBankData.level = 2;
            piggyBankData.gemCount = 0;
        }
        else if (piggyBankData.level == 4)
        {
            piggyBankData.level = 2;
            piggyBankData.gemCount = 0;
        }
        else if (piggyBankData.level == 5)
        {
            piggyBankData.level = 4;
            piggyBankData.gemCount = GetSmashRange(4).max;

            if (piggyBankData.smashLvl5Flag && piggyBankData.missingCount >= 3)
            {
                piggyBankData.smashLvl5Flag = false;
                piggyBankData.level = 2;
                piggyBankData.gemCount = 0;
            }
        }
    }

    public static void OnSmashSucessful()
    {
        PiggyBankData piggyBankData = PlayerData.current.piggyBankData;
        piggyBankData.gemCount = 0;
        piggyBankData.missingCount = 0;

        int nextLevelCooldown = 0;

        if (piggyBankData.level == 1)
        {
            piggyBankData.level = 2;
        }
        else if (piggyBankData.level == 2)
        {
            piggyBankData.level = 3;
            nextLevelCooldown = 24 * 3600;
        }
        else if (piggyBankData.level == 3)
        {
            piggyBankData.level = 4;
            nextLevelCooldown = 48 * 3600;
        }
        else if (piggyBankData.level == 4)
        {
            nextLevelCooldown = 72 * 3600;
            piggyBankData.level = 5;
        }
        else if (piggyBankData.level == 5)
        {
            nextLevelCooldown = 24 * 3600;
            piggyBankData.smashLvl5Flag = true;
        }

        piggyBankData.nextLevelAvailaibleTime = DateTimeUtility.GetUtcNow().AddSeconds(nextLevelCooldown).ToString();
    }

    public static IntegerRange GetBonusRange(int level)
    {
        int idx = Mathf.Clamp(level - 1, 0, bonusRanges.Length);
        return bonusRanges[idx];
    }

    public static IntegerRange GetSmashRange(int level)
    {
        int idx = Mathf.Clamp(level - 1, 0, smashRanges.Length);
        return smashRanges[idx];
    }

    public static float Get_RemainingTime_To_NextLevelCountDown()
    {
        PiggyBankData piggyBankData = PlayerData.current.piggyBankData;

        if (piggyBankData.level <= 2) return -0.0001f;

        DateTime smashNextLevelTime = DateTimeUtility.Get(piggyBankData.nextLevelAvailaibleTime);
        DateTime now = DateTimeUtility.GetUtcNow();

        if (smashNextLevelTime < now)
        {
            return -0.0001f;
        }
        else
        {
            return (float)(smashNextLevelTime - now).TotalSeconds;
        }
    }

    public static float Get_RemainingTime_To_PeriodExpired()
    {
        PiggyBankData piggyBankData = PlayerData.current.piggyBankData;

        DateTime periodExpiredTime = DateTimeUtility.Get(piggyBankData.periodExpireTime);
        DateTime now = DateTimeUtility.GetUtcNow();

        if (periodExpiredTime < now)
        {
            return -0.0001f;
        }
        else
        {
            return (float)(periodExpiredTime - now).TotalSeconds;
        }
    }

    public static float Get_RemainingTime_To_PeriodAvailable()
    {
        PiggyBankData piggyBankData = PlayerData.current.piggyBankData;

        DateTime now = DateTimeUtility.GetUtcNow();
        DateTime periodAvailableTime = DateTimeUtility.Get(piggyBankData.periodAvailableTime);

        if (periodAvailableTime < now)
        {
            return -0.0001f;
        }
        else
        {
            return (float)(periodAvailableTime - now).TotalSeconds;
        }
    }

    private static bool available = true;

    private static IntegerRange[] bonusRanges = new IntegerRange[]
    {
        new IntegerRange() { min = 8, max = 10},
        new IntegerRange() { min = 20, max = 30},
        new IntegerRange() { min = 35, max = 40},
        new IntegerRange() { min = 35, max = 40},
        new IntegerRange() { min = 40, max = 50}
    };

    private static IntegerRange[] smashRanges = new IntegerRange[]
    {
        new IntegerRange() { min = 60, max = 80},
        new IntegerRange() { min = 200, max = 250},
        new IntegerRange() { min = 400, max = 500},
        new IntegerRange() { min = 500, max = 650},
        new IntegerRange() { min = 850, max = 1000}
    };

    private static float[] iapDefaultPriceUsd = new float[]
    {
        0.99f,
        2.99f,
        5.99f,
        7.99f,
        9.99f
    };

    public static string GetIapProductId()
    {
        return "piggy_bank_" + PlayerData.current.piggyBankData.level.ToString();
    }

    public static float GetIapDefaultPriceUsd()
    {
        return iapDefaultPriceUsd[PlayerData.current.piggyBankData.level - 1];
    }
}
