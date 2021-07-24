using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PiggyBankTest : MonoBehaviour
{
    public Text levelText;

    public Text gemText;

    public Text timetext;

    StringBuilder sb = new StringBuilder();

    public void OnApplicationQuit()
    {
        Model.Instance.Save();
    }

    public void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Model.Instance.Save();
        }
    }

    public void Start()
    {
        Model.Instance.Load();
    }

    public void Stash()
    {
        //if (PiggyBankUtility.Available())
        //    PiggyBankUtility.OnStash();
    }

    public void Smash()
    {
        if (PiggyBankUtility.Available() && PiggyBankUtility.CanSmash())            
        {
            PiggyBankUtility.OnSmashSucessful();
        }
        else 
        {
            Debug.LogWarning("Can not smash");
        }
    }

    public void ResetPiggyBank()
    {
        PlayerData.current.piggyBankData = new PiggyBankData();
    }

    public void Update()
    {
        var piggyBankData = PlayerData.current.piggyBankData;

        levelText.text = "Lvl " + piggyBankData.level.ToString();

        gemText.text = piggyBankData.gemCount.ToString() + "/" +
            PiggyBankUtility.GetSmashRange(piggyBankData.level).min.ToString() + "/" +
            PiggyBankUtility.GetSmashRange(piggyBankData.level).max.ToString();

        if (PiggyBankUtility.Get_RemainingTime_To_NextLevelCountDown() > 0f)
        {
            int time = (int)PiggyBankUtility.Get_RemainingTime_To_NextLevelCountDown();
            DateTimeUtility.ToHourMinuteSecond(sb, time);

            timetext.text = "Next level available in : " + sb.ToString();
        }
        else
        {
            if (PiggyBankUtility.CanSmash())
            {
                int time = (int)PiggyBankUtility.Get_RemainingTime_To_PeriodExpired();
                DateTimeUtility.ToHourMinuteSecond(sb, time);

                timetext.text = "Period Duration : " + sb.ToString();
            }
            else
            {
                if (PiggyBankUtility.Get_RemainingTime_To_PeriodAvailable() > 0f)
                {
                    int time = (int)PiggyBankUtility.Get_RemainingTime_To_PeriodAvailable();
                    DateTimeUtility.ToHourMinuteSecond(sb, time);

                    timetext.text = "Available in : " + sb.ToString();
                }
                else
                {
                    timetext.text = "Not enough gem to smash";
                }               
            }
        }
        
    }
}
