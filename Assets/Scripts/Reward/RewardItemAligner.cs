using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardItemAligner 
{
    public List<Transform> currencyRewards = new List<Transform>();

    public List<Transform> playRewards = new List<Transform>();

    public RewardItemController Add(RewardType type, int count)
    {
        var rewardItem = RewardItemTable.Instance.Get(type).GetComponent<RewardItemController>();
        
        if (type == RewardType.CurrencyStamina)
        {
            StringBuilder sb = new StringBuilder();
            DateTimeUtility.ToHourMinuteSecond(sb, count);
            rewardItem.SetCount(sb.ToString());
        }
        else
        {
            rewardItem.SetCount("x" + count.ToString());
        }

        if (type == RewardType.CurrencyCoin || type == RewardType.CurrencyGem || type == RewardType.CurrencyStamina || type == RewardType.CurrencyStaminaInf)
        {
            currencyRewards.Add(rewardItem.transform);
        }
        else
        {
            playRewards.Add(rewardItem.transform);
        }

        return rewardItem;
    }

    public void Align(float titleX, float titleY)
    {
        for (int i = 0; i < currencyRewards.Count; i++)
        {
            float x = (i - (currencyRewards.Count - 1) * 0.5f) * titleX;
            float y = titleY * 0.5f;
            currencyRewards[i].transform.position = new Vector3(x, y, 0f);
        }

        for (int i = 0; i < playRewards.Count; i++)
        {
            float x = (i - (playRewards.Count - 1) * 0.5f) * titleX;
            float y = -titleY * 0.5f;
            playRewards[i].transform.position = new Vector3(x, y, 0f);
        }
    }

    public List<Transform> GetList()
    {
        var list = new List<Transform>();
        list.AddRange(currencyRewards);
        list.AddRange(playRewards);
        return list;
    }
}
