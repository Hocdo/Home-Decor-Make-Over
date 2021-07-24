using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Popup;
using DG.Tweening;
using UnityRandom = UnityEngine.Random;

public class PopupSpin : PopupBase
{
    public Transform availablePopup;

    public Transform cooldownPopup;

    public Transform spinCircle;

    public Text cooldownTimeText;

    public Button spinAdsButton;

    public int spinGemCost = 20;

    private bool spinAvailable;

    private Coroutine timeCoroutine;
   
    private void UpdateSpinAvailable(bool forceSpin = false)
    {
        if (forceSpin)
        {
            spinAvailable = true;
        }
        else
        {
            spinAvailable = SpinUtility.Available();
        }
        
        if (spinAvailable)
        {
            availablePopup.gameObject.SetActive(true);
            cooldownPopup.gameObject.SetActive(false);
        }
        else
        {
            availablePopup.gameObject.SetActive(false);
            cooldownPopup.gameObject.SetActive(true);

            timeCoroutine = StartCoroutine(TimeUpdate());
        }
    }

    public override void Show()
    {
        UpdateSpinAvailable();

        if (SpinUtility.GetRemainingFreeSpinAdsDuration() > 0f)
        {
            spinAdsButton.gameObject.SetActive(false);
        }

        canClose = false;
        GetComponent<CanvasGroup>().alpha = 1f;
        PopupAnimationUtility.AnimateScale(transform, Ease.OutBack, 0.25f, 1f, 0.25f, 0f).OnComplete(() => canClose = true);
    }

    public override void Close(bool forceDestroying = true)
    {
        TerminateInternal(forceDestroying);
    }

    [SerializeField] Image spinBtn;
    public void StartSpin()
    {
        spinBtn.raycastTarget = false;
        StartCoroutine(SpinCoroutine());
    }

    public void ButtonSpinAds()
    {
        Action RewardedVideoReward = () =>
        {
            PlayerData.current.nextFreeSpinAdsTime = DateTimeUtility.GetUtcNow().AddHours(4).ToString();

            UpdateSpinAvailable(true);

            //AppEventTracker.LogEventRewardAd("more_spin", true);
            Firebase.Analytics.FirebaseAnalytics.LogEvent("more_spin", new Firebase.Analytics.Parameter("watch_ads", "true"));
        };

        Action RewardedVideoFailed = () =>
        {
            //AppEventTracker.LogEventRewardAd("more_spin", false);
            Firebase.Analytics.FirebaseAnalytics.LogEvent("more_spin", new Firebase.Analytics.Parameter("watch_ads", "false"));
        };
       
        AdvertisementManager.Instance.ShowRewardedVideo(RewardedVideoReward, RewardedVideoFailed);
    }

    public void ButtonSpinGem()
    {
        if (PlayerData.current.gemCount >= spinGemCost)
        {
            PlayerData.current.AddGem(-spinGemCost);
            EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.GemChange, PlayerData.current.gemCount);

            UpdateSpinAvailable(true);
        }
        else
        {
            Popup.PopupSystem.GetOpenBuilder()
             .SetType(PopupType.PopupGemStore)
             .Open();
        }
    }

    private IEnumerator SpinCoroutine()
    {
        canClose = false;

        float maxSpeed = UnityRandom.Range(840f, 1000f);
        float accelerateUp = UnityRandom.Range(4000f, 4600f);
        float accelerateDown = UnityRandom.Range(-75f, -100f);
        float speed = 0f;

        while (speed < maxSpeed)
        {
            float dt = Time.deltaTime;
            spinCircle.Rotate(new Vector3(0f, 0f, speed * dt), Space.Self);
            speed += accelerateUp * dt;

            yield return null;
        }

        while (speed > 0f)
        {
            float dt = Time.deltaTime;
            spinCircle.Rotate(new Vector3(0f, 0f, speed * dt), Space.Self);
            speed += accelerateDown * dt;

            yield return null;
        }

        float rotation = spinCircle.localEulerAngles.z;
        int index = (int)(rotation / (360f / 8f));

        PlayerData playerData = PlayerData.current;
        playerData.lastSpinTime = DateTimeUtility.GetNow().ToString();
        canClose = true;
        spinBtn.raycastTarget = true;

        var popupReward = PopupSystem.GetOpenBuilder().
            SetType(PopupType.PopupReward).
            SetCurrentPopupBehaviour(CurrentPopupBehaviour.Close).
            Open<PopupReward>();       

        if (index == 0)
        {
            playerData.AddGem(100);
            popupReward.Add(RewardType.CurrencyGem, 100)
            .CompleteAction = () => EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.GemAnimChange, new int[] { playerData.gemCount, 1000 });
        }
        else if (index == 1)
        {
            playerData.match3Data.AddIngameBooster(Match3.BoosterType.CandyPack, 1);
            popupReward.Add(RewardType.IngameBoosterCandyPack, 1);
        }
        else if (index == 2)
        {
            playerData.match3Data.AddIngameBooster(Match3.BoosterType.Shuffle, 2);
            popupReward.Add(RewardType.IngameBoosterShuffle, 2);
        }
        else if (index == 3)
        {
            playerData.match3Data.AddIngameBooster(Match3.BoosterType.Hammer, 1);
            popupReward.Add(RewardType.IngameBoosterHammer, 1);
        }
        else if (index == 4)
        {
            playerData.match3Data.AddHeadStartBooster(Match3.HeadStartBoosterType.Bomb, 1);
            popupReward.Add(RewardType.StartBoosterBomb, 1);
        }
        else if (index == 5)
        {
            playerData.match3Data.AddHeadStartBooster(Match3.HeadStartBoosterType.Move, 1);
            popupReward.Add(RewardType.StartBoosterMove, 1);
        }
        else if (index == 6)
        {
            playerData.match3Data.AddHeadStartBooster(Match3.HeadStartBoosterType.Rocket, 1);
            popupReward.Add(RewardType.StartBoosterRocket, 1);
        }
        else if (index == 7)
        {
            playerData.AddCoin(500);

            popupReward.Add(RewardType.CurrencyCoin, 500)
                .CompleteAction = () => EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.CoinAnimChange, new int[] { playerData.cointCount, 1000 });
        }

        popupReward.Align(2f, 3f);
    }

    public void OnApplicationFocus(bool focus)
    {
        if (timeCoroutine != null) StopCoroutine(timeCoroutine);
        timeCoroutine = StartCoroutine(TimeUpdate());
    }

    public void OnApplicationPause(bool pause)
    {
        if (timeCoroutine != null) StopCoroutine(timeCoroutine);
        timeCoroutine = StartCoroutine(TimeUpdate());
    }

    public IEnumerator TimeUpdate()
    {
        var waitFor1s = new WaitForSeconds(1f);
        var stringBuilder = new StringBuilder();

        float remainingTime = 24 * 3600f - (float)DateTime.Now.TimeOfDay.TotalSeconds;
        float bias = remainingTime - (int)remainingTime;
        remainingTime -= bias;

        DateTimeUtility.ToHourMinuteSecond(stringBuilder, (int)remainingTime);
        cooldownTimeText.text = stringBuilder.ToString();

        yield return new WaitForSeconds(bias);

        while (true)
        {
            DateTimeUtility.ToHourMinuteSecond(stringBuilder, (int)remainingTime);
            cooldownTimeText.text = stringBuilder.ToString();

            remainingTime -= 1f;

            yield return waitFor1s;

            if (remainingTime <= 0f)
            {
                CloseInternal();
            }
        }
    }
}

public static class SpinUtility
{
    public static bool Available()
    {
        var lastSpinTime = PlayerData.current.lastSpinTime;
        if (string.IsNullOrEmpty(lastSpinTime))
        {
            return true;
        }
        else
        {
            var lastSpinDateTime = DateTimeUtility.Get(lastSpinTime);
            var now = DateTimeUtility.GetNow();

            return lastSpinDateTime.Date != now.Date;
        }
    }

    public static float GetRemainingFreeSpinAdsDuration()
    {       
        if (!string.IsNullOrEmpty(PlayerData.current.nextFreeSpinAdsTime))
        {
            var dateTime = DateTimeUtility.Get(PlayerData.current.nextFreeSpinAdsTime);
            float duration = (float)(dateTime - DateTimeUtility.GetUtcNow()).TotalSeconds;

            return Mathf.Max(duration, 0f);
        }
        else
        {
            return 0f;
        }
    }
}


