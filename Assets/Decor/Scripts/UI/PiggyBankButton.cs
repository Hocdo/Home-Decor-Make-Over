using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PiggyBankButton : MonoBehaviour
{
    public Button fullButton;

    public Button normalButton;

    public Text timeText;

    public Text valueText;

    private PiggyBankData piggyBankData;

    private Coroutine timeUpdateCoroutine;

    public void Start()
    {
        UpdateState();
    }

    public void OnDestroy()
    {
        string filePath = PlayerData.GetFilePath();
        string playerDataJson = JsonUtility.ToJson(PlayerData.current);
        File.WriteAllText(filePath, playerDataJson);
    }

    private void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

    public void ButtonPressed()
    {
        Popup.PopupSystem.GetOpenBuilder().                     
            SetType(PopupType.PopupPiggyBank).                       
            SetCurrentPopupBehaviour(Popup.CurrentPopupBehaviour.Close).                      
            Open().AcceptEvent = (arg) => UpdateState();
    }

    public void UpdateState()
    {
        PiggyBankUtility.Initialize();

        if (PiggyBankUtility.Available())
        {
            piggyBankData = PlayerData.current.piggyBankData;
            valueText.text = piggyBankData.gemCount + "/"
                + PiggyBankUtility.GetSmashRange(piggyBankData.level).min + "/"
                + PiggyBankUtility.GetSmashRange(piggyBankData.level).max + "-"
                + piggyBankData.level;

            if (timeUpdateCoroutine != null)
            {
                StopCoroutine(timeUpdateCoroutine);
                timeUpdateCoroutine = null;
            }

            bool canSmash = PiggyBankUtility.CanSmash();

            fullButton.gameObject.SetActive(canSmash);
            normalButton.gameObject.SetActive(!canSmash);

            if (canSmash)
            {                 
                timeUpdateCoroutine = StartCoroutine(TimeUpdate());
            }
            else
            {
                timeText.gameObject.SetActive(false);
            }
        }
        else
        {
            SetVisible(false);
        }
    }

    public void OnApplicationFocus(bool focus)
    {
        if (timeUpdateCoroutine != null)
        {
            StopCoroutine(timeUpdateCoroutine);
            timeUpdateCoroutine = null;
        }

        if (PiggyBankUtility.CanSmash())
        {              
            timeUpdateCoroutine = StartCoroutine(TimeUpdate());
        }
    }

    public void OnApplicationPause(bool pause)
    {
        if (timeUpdateCoroutine != null)
        {
            StopCoroutine(timeUpdateCoroutine);
            timeUpdateCoroutine = null;
        }

        if (PiggyBankUtility.CanSmash())
        {              
            timeUpdateCoroutine = StartCoroutine(TimeUpdate());
        }
    }

    public IEnumerator TimeUpdate()
    {
        timeText.gameObject.SetActive(true);

        float timeToNextLevelAvailable = PiggyBankUtility.Get_RemainingTime_To_NextLevelCountDown();
        if (timeToNextLevelAvailable > 0f)
        {
            yield return new WaitForSeconds(timeToNextLevelAvailable + 0.01f);
            UpdateState();
        }
        else
        {
            float timeToPeriodExpire = PiggyBankUtility.Get_RemainingTime_To_PeriodExpired();
            if (timeToPeriodExpire > 0f)
            {
                StringBuilder sb = new StringBuilder();
                var waitFor1s = new WaitForSeconds(1f);

                DateTimeUtility.ToHourMinuteSecond(sb, (int)timeToPeriodExpire);
                timeText.text = sb.ToString();

                float bias = timeToPeriodExpire - (int)timeToPeriodExpire;
                timeToPeriodExpire -= bias;
                yield return new WaitForSeconds(bias);

                while (true)
                {
                    DateTimeUtility.ToHourMinuteSecond(sb, (int)timeToPeriodExpire);
                    timeText.text = sb.ToString();

                    timeToPeriodExpire -= 1f;
                    yield return waitFor1s;

                    if (timeToPeriodExpire <= 0f)
                    {
                        SetVisible(false);
                        PiggyBankUtility.OnSmashMissed();
                    }
                }
            }
            else
            {
                float timeToPeriodAvailable = PiggyBankUtility.Get_RemainingTime_To_PeriodAvailable();
                if (timeToPeriodAvailable > 0f)
                {
                    yield return new WaitForSeconds(timeToPeriodAvailable + 0.01f);
                    UpdateState();
                }
            }
        }

        timeUpdateCoroutine = null;
    }

    public void Stash()
    {
        PiggyBankUtility.OnStash(PiggyBankUtility.GetBonusRange(piggyBankData.level).GetRandom());

        UpdateState();
    }

    public void Smash()
    {
        if (PiggyBankUtility.CanSmash())
        {
            PiggyBankUtility.OnSmashSucessful();

            UpdateState();
        }
        
    }
}
