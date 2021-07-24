using System;

[Serializable]
public class Stamina
{
    public static int MaxStaminaCount = 5;
    public static int StaminaFillDuration = 1800;

    public int count;

    public string lastTimeAdded;
    public string lastTimeInfinityAdded;

    public int infinityTime;

    public int GetRemainingTime()
    {
        int remainingDuration = 0;
        DateTime lastTimeStaminaAdded;

        if (string.IsNullOrEmpty(lastTimeAdded))
            lastTimeStaminaAdded = DateTimeUtility.GetUtcNow();
        else
            lastTimeStaminaAdded = DateTimeUtility.Get(lastTimeAdded);


        TimeSpan staminaTimeSpan = DateTimeUtility.GetUtcNow() - lastTimeStaminaAdded;
        int pastSeconds = (int)staminaTimeSpan.TotalSeconds;
        int addedStamina = (int)(pastSeconds / StaminaFillDuration);

        count += addedStamina;

        if (count >= MaxStaminaCount)
        {
            count = MaxStaminaCount;
        }
        else
        {
            remainingDuration = StaminaFillDuration - (int)(pastSeconds - addedStamina * StaminaFillDuration);
            lastTimeAdded = lastTimeStaminaAdded.AddSeconds((double)(addedStamina * StaminaFillDuration)).ToString();
        }

        return remainingDuration;
    }

    public int GetInfinityRemainingTime()
    {
        int remainingInfinityTime = 0;

        if (infinityTime >= 0)
        {
            DateTime lastDateTimeAdded = DateTimeUtility.Get(lastTimeInfinityAdded);
            TimeSpan staminaTimeSpan = DateTimeUtility.GetUtcNow() - lastDateTimeAdded;
            int deltaTime = (int)staminaTimeSpan.TotalSeconds;

            remainingInfinityTime = infinityTime - deltaTime;
            if (remainingInfinityTime < 0)
            {
                remainingInfinityTime = 0;
                infinityTime = -1;
            }
        }

        return remainingInfinityTime;
    }

    public void OnRemainingTimeZero()
    {
        Add(1);
        lastTimeAdded = DateTimeUtility.GetUtcNow().ToString();
    }

    public bool Available()
    {
        return GetInfinityRemainingTime() > 0 || count > 0;
    }

    public void Add(int add, bool ignoreInfinity = false)
    {
        if (add >= 0)
        {
            count += add;
            if (count >= MaxStaminaCount)
            {
                count = MaxStaminaCount;
            }
        }
        else
        {
            if (ignoreInfinity == true || GetInfinityRemainingTime() <= 0)
            {
                int prevStaminaCount = count;
                count += add;
                if (count < 0) count = 0;

                if (count < MaxStaminaCount && prevStaminaCount == MaxStaminaCount)
                {
                    lastTimeAdded = DateTimeUtility.GetUtcNow().ToString();
                }
            }
        }
    }

    public void AddInfinity(int timeInSec)
    {
        int remainingInfinityTime = GetInfinityRemainingTime();
        if (remainingInfinityTime > 0)
        {
            infinityTime += timeInSec;
        }
        else
        {
            infinityTime = timeInSec;
            lastTimeInfinityAdded = DateTimeUtility.GetUtcNow().ToString();
        }
    }

    public bool IsFull()
    {
        return count >= MaxStaminaCount;
    }
}
