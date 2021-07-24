using System;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Popup;

public class PopupStaminaStore : PopupBase
{
    public Text staminaTimeText;

    public Text staminaCountText;

    private int staminaRemainingTime;

    private Stamina stamina;

    private StringBuilder stringBuilder = new StringBuilder();

    public void UpdateStaminaTimeText(string str)
    {
        staminaTimeText.text = str;
    }

    public void UpdateStaminaCountText(int count)
    {
        staminaCountText.text = count.ToString();
    }

    private void UpdateStamina()
    {
        stamina = PlayerData.current.stamina;
        staminaRemainingTime = stamina.GetRemainingTime();

        UpdateStaminaCountText(stamina.count);
        UpdateStaminaTimeText(stringBuilder.ToString());
    }

    public void OnApplicationFocus(bool focus)
    {
        UpdateStamina();
    }

    public void OnApplicationPause(bool pause)
    {
        UpdateStamina();
    }

    private void Awake()
    {
        UpdateStamina();

        StartCoroutine(DecreaseStaminaDurationCoroutine());
    }

    public override void Show()
    {       
        canClose = false;

        PopupAnimationUtility.AnimateScale(transform, Ease.OutBack, 0.25f, 1f, 0.25f, 0f).OnComplete(() => canClose = true);
    }

    public IEnumerator DecreaseStaminaDurationCoroutine()
    {
        var waitFor1s = new WaitForSeconds(1f);

        while (true)
        {
            StaminaSchedule();

            yield return waitFor1s;
        }
    }

    private void StaminaSchedule()
    {
        if (stamina.IsFull())
        {
            UpdateStaminaTimeText("Full");
        }
        else
        {
            staminaRemainingTime -= 1;
            if (staminaRemainingTime < 0)
            {
                //stamina.OnRemainingTimeZero();
                UpdateStaminaCountText(stamina.count);

                if (stamina.IsFull())
                    UpdateStaminaTimeText("Full");

                staminaRemainingTime = Stamina.StaminaFillDuration;
            }

            DateTimeUtility.ToMinuteSecond(stringBuilder, staminaRemainingTime);
            UpdateStaminaTimeText(stringBuilder.ToString());
        }
    }

    public override void Close(bool forceDestroying = true)
    {
        PopupAnimationUtility.AnimadeAlpha(GetComponent<CanvasGroup>(), Ease.Linear, 1f, 0f, 0.1f, 0f, false);
        PopupAnimationUtility.AnimateScale(transform, Ease.OutQuart, 1f, 0.8f, 0.1f, 0f)
            .OnComplete(() =>
            {
                TerminateInternal(forceDestroying);
            });
    }

    public void Refill()
    {
        if (PlayerData.current.gemCount >= 60)
        {
            AudioManager.Instance.PlaySFX(AudioClipId.Purchased);
            PlayerData.current.AddGem(-60);
            PlayerData.current.AddStamina(5);
            EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.GemChange, PlayerData.current.gemCount);
            EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.StaminaChange);
            AcceptEvent?.Invoke(null);
            CloseInternal();
        }
        else
        {
            PopupSystem.Instance.ShowPopup(PopupType.PopupGetMoreGems);
        }
    }

    public void ReceiveLife()
    {
        Action RewardedVideoReward = () =>
        {
            AudioManager.Instance.PlaySFX(AudioClipId.Purchased);
            PlayerData.current.AddStamina(1);
            EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.GemChange, PlayerData.current.gemCount);
            EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.StaminaChange);
            AcceptEvent?.Invoke(null);
            CloseInternal();

            //AppEventTracker.LogEventRewardAd("stamina_store", true);
            Firebase.Analytics.FirebaseAnalytics.LogEvent("stamina_store", "status","true");
        };

        Action RewardedVideoFailed = () =>
        {
            //AppEventTracker.LogEventRewardAd("stamina_store", false);
            Firebase.Analytics.FirebaseAnalytics.LogEvent("stamina_store", "status", "false");
        };

        AdvertisementManager.Instance.ShowRewardedVideo(RewardedVideoReward, RewardedVideoFailed);
    }
}

