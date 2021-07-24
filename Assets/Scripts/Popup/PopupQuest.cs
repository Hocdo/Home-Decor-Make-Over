using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using DG.Tweening;
using Popup;
using System.Text;

public class PopupQuest : PopupBase
{
    public Transform main;

    public CollectParticles collectParticles;

    public RectTransform currentGem;

    public Text gemCountText;

    public Text timeText;

    public Image progressImage;

    public Transform questViewParent;

    public Button gift3QuestButton;

    public Button gift7QuestButton;

    public Button gift10QuestButton;

    private QuestData questData;

    private Coroutine timeCoroutine;

    private bool canClaim = true;

    private bool canPlayAddCoinSFX;

    private int completedQuestCount = 0;

    private UIEdgeSnapPosition currentGemEdgeSnap;

    public override void Show()
    {
        canClose = false;
        GetComponent<CanvasGroup>().alpha = 1f;
        PopupAnimationUtility.AnimateScale(main, Ease.OutBack, 0.25f, 1f, 0.25f, 0f).OnComplete(() => canClose = true);

        currentGemEdgeSnap.SetPositionVisibility(false);
        currentGemEdgeSnap.Show(0.25f);
    }

    public override void Close(bool forceDestroying = true)
    {
        PreAnimateHideEvent?.Invoke();

        TerminateInternal(forceDestroying);
    }

    private void Awake()
    {
        currentGemEdgeSnap = new UIEdgeSnapPosition(currentGem, new Vector2(0f, 1.2f));        

        questData = PlayerData.current.questData;

        UpdateAllQuestViews();

        gemCountText.text = PlayerData.current.gemCount.ToString();

        gift3QuestButton.gameObject.SetActive(!questData.unboxProgress1);
        gift7QuestButton.gameObject.SetActive(!questData.unboxProgress2);
        gift10QuestButton.gameObject.SetActive(!questData.unboxProgress3);

        gift3QuestButton.onClick.AddListener(UnboxProgress_3Quests);
        gift7QuestButton.onClick.AddListener(UnboxProgress_7Quests);
        gift10QuestButton.onClick.AddListener(UnboxProgress_10Quests);
    }

    private void UpdateAllQuestViews()
    {
        int questCount = Mathf.Min(questData.allQuest.Count, questViewParent.childCount);
        completedQuestCount = 0;

        for (int i = 0; i < questData.allQuest.Count; i++)
        {
            if (questData.allQuest[i].IsCompleted())
            {
                completedQuestCount++;
            }
        }

        for (int i = 0; i < questCount; i++)
        {
            QuestUIView questView = GetQuestView(i);
            questView.SetupWithQuest(questData.allQuest[i], i, OnClaim, OnRefresh);

            if (questData.allQuest[i].Claimed())
            {
                questView.OnQuestClaimed();
            }
        }

        progressImage.fillAmount = completedQuestCount / 10f;
        timeCoroutine = StartCoroutine(TimeUpdate());

        bool giftAnimProgress1 = (completedQuestCount >= 3);
        bool giftAnimProgress2 = (completedQuestCount >= 7);
        bool giftAnimProgress3 = (completedQuestCount >= 10);

        gift3QuestButton.GetComponent<Animation>().enabled = giftAnimProgress1;
        gift7QuestButton.GetComponent<Animation>().enabled = giftAnimProgress2;
        gift10QuestButton.GetComponent<Animation>().enabled = giftAnimProgress3;
    }

    private QuestUIView GetQuestView(int idx)
    {
        return questViewParent.GetChild(idx).GetComponent<QuestUIView>();
    }

    public void OnClaim(int idx)
    {
        if (canClaim == false) return;

        canClaim = false;
        canClose = false;

        AudioManager.Play_SFX(AudioClipId.Purchased);

        Quest quest = questData.allQuest[idx];
        quest.Claim();

        int currentGemCount = PlayerData.current.gemCount;
        int rewardCount = quest.GetConfig().rewardGemCount;
        PlayerData.current.AddGem(rewardCount);
        EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.GemChange, PlayerData.current.gemCount);

        UpdateAllQuestViews();

        DOTween.To(() => currentGemCount, (value) => currentGemCount = value, PlayerData.current.gemCount, collectParticles.GetDuration())
            .SetDelay(collectParticles.GetParticleMoveDuration())
            .OnStart(() =>
            {
                StartCoroutine(PlayAddGemSFXCoroutine(rewardCount));
            })
            .OnUpdate(() => gemCountText.text = currentGemCount.ToString())
            .OnComplete(() =>
            {
                canPlayAddCoinSFX = false;
                canClose = true;
                canClaim = true;
            });

        Vector3 targetPosition = collectParticles.targetTransform.position;
        Vector3 sourcePosition = GetQuestView(idx).gemIcon.transform.position;
        collectParticles.transform.position = sourcePosition;
        collectParticles.SetRotateAngle(Mathf.Atan2(targetPosition.y - sourcePosition.y, targetPosition.x - sourcePosition.x) * Mathf.Rad2Deg - 90f);
        collectParticles.SetSpawnRate(rewardCount * 3f);
        collectParticles.Play();
    }

    public void OnRefresh(int idx)
    {
        Action RewardedVideoReward = () =>
        {
            int newGroupIndex = questData.groupIndices[idx];
            var newQuest = QuestPool.Instance.GetNewQuest(questData.poolIndices[idx], ref newGroupIndex);
            newQuest.Open();

            questData.allQuest[idx] = newQuest;
            questData.groupIndices[idx] = newGroupIndex;

            GetQuestView(idx).SetupWithQuest(questData.allQuest[idx], idx, OnClaim, OnRefresh);

            //AppEventTracker.LogEventRewardAd("refresh_quest", true);
            Firebase.Analytics.FirebaseAnalytics.LogEvent("refresh_quest", new Firebase.Analytics.Parameter("Rewarded_video_status", "true"));
        };

        Action RewardedVideoFailed = () =>
        {
            //AppEventTracker.LogEventRewardAd("refresh_quest", false);
            Firebase.Analytics.FirebaseAnalytics.LogEvent("refresh_quest", new Firebase.Analytics.Parameter("Rewarded_video_status", "false"));
        };

        AdvertisementManager.Instance.ShowRewardedVideo(RewardedVideoReward, RewardedVideoFailed);

    }

    private IEnumerator PlayAddGemSFXCoroutine(int maxCount)
    {
        int count = 0;
        var waitForInterval = new WaitForSeconds(0.15f);

        canPlayAddCoinSFX = true;
        while (canPlayAddCoinSFX)
        {
            AudioManager.Instance.PlaySFX(AudioClipId.AddCoin);
            count++;

            if (count == maxCount)
                break;

            yield return waitForInterval;
        }
    }

    private void UnboxProgress_3Quests()
    {
        if (completedQuestCount < 3) return;

        Action rewardEvent = () =>
        {
            UpdateAllQuestViews();
            questData.unboxProgress1 = true;

            var boosterType = QuestUtility.GetRandoms(new List<Match3.HeadStartBoosterType>{
                Match3.HeadStartBoosterType.Move,
                Match3.HeadStartBoosterType.Rocket,
                Match3.HeadStartBoosterType.Bomb }, 1)[0];

            PlayerData.current.match3Data.AddHeadStartBooster(boosterType, 1);

            AudioManager.Instance.PlaySFX(AudioClipId.Purchased);
            var popupReward = PopupSystem.GetOpenBuilder().
                SetType(PopupType.PopupReward).
                SetCurrentPopupBehaviour(CurrentPopupBehaviour.Close).
                Open<PopupReward>();

            popupReward.Add((RewardType)((int)boosterType + (int)RewardType.StartBoosterMove), 1);
        };

        if (PlayerData.current.match3Data.level >= RemoteConfig.levelStartAds && PlayerData.current.noAds == false)
            AdvertisementManager.Instance.ShowInterstitial(rewardEvent);
        else
            rewardEvent();
    }

    private void UnboxProgress_7Quests()
    {
        if (completedQuestCount < 7) return;

        Action rewardEvent = () =>
        {
            UpdateAllQuestViews();
            questData.unboxProgress2 = true;

            var boosterType = QuestUtility.GetRandoms(new List<Match3.BoosterType>{
                Match3.BoosterType.Hammer,
                Match3.BoosterType.HBomb,
                Match3.BoosterType.VBomb,
                Match3.BoosterType.CandyPack }, 1)[0];

            PlayerData.current.match3Data.AddIngameBooster(boosterType, 1);

            AudioManager.Instance.PlaySFX(AudioClipId.Purchased);
            var popupReward = PopupSystem.GetOpenBuilder().
                SetType(PopupType.PopupReward).
                SetCurrentPopupBehaviour(CurrentPopupBehaviour.Close).
                Open<PopupReward>();

            RewardType rewardType = RewardType.IngameBoosterHammer;

            if (boosterType == Match3.BoosterType.Hammer)
                rewardType = RewardType.IngameBoosterHammer;
            else if (boosterType == Match3.BoosterType.HBomb)
                rewardType = RewardType.IngameBoosterHRocket;
            else if (boosterType == Match3.BoosterType.VBomb)
                rewardType = RewardType.IngameBoosterVRocket;
            else if (boosterType == Match3.BoosterType.CandyPack)
                rewardType = RewardType.IngameBoosterCandyPack;

            popupReward.Add(rewardType, 1);
        };

        if (PlayerData.current.match3Data.level >= RemoteConfig.levelStartAds && PlayerData.current.noAds == false)
            AdvertisementManager.Instance.ShowInterstitial(rewardEvent);
        else
            rewardEvent();
    }

    private void UnboxProgress_10Quests()
    {
        if (completedQuestCount < 10) return;

        Action rewardEvent = () =>
        {
            UpdateAllQuestViews();
            questData.unboxProgress3 = true;

            var boosterTypes = QuestUtility.GetRandoms(new List<Match3.HeadStartBoosterType>{
                Match3.HeadStartBoosterType.Move,
                Match3.HeadStartBoosterType.Rocket,
                Match3.HeadStartBoosterType.Bomb }, 2);

            PlayerData.current.stamina.AddInfinity(15 * 60);
            PlayerData.current.match3Data.AddHeadStartBooster(boosterTypes[0], 1);
            PlayerData.current.match3Data.AddHeadStartBooster(boosterTypes[1], 1);

            AudioManager.Instance.PlaySFX(AudioClipId.Purchased);
            var popupReward = PopupSystem.GetOpenBuilder().
                SetType(PopupType.PopupReward).
                SetCurrentPopupBehaviour(CurrentPopupBehaviour.Close).
                Open<PopupReward>();

            popupReward.Add(RewardType.CurrencyStaminaInf, 15 * 60);
            popupReward.Add((RewardType)((int)boosterTypes[0] + (int)RewardType.StartBoosterMove), 1);
            popupReward.Add((RewardType)((int)boosterTypes[1] + (int)RewardType.StartBoosterMove), 1);
        };

        //if (PlayerData.current.match3Data.level >= RemoteConfig.level_full_ads && PlayerData.current.noAds == false)
        //    AdvertisementManager.Instance.ShowInterstitial(rewardEvent);
        //else
            rewardEvent();
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
        timeText.text = stringBuilder.ToString();

        yield return new WaitForSeconds(bias);

        while (true)
        {
            DateTimeUtility.ToHourMinuteSecond(stringBuilder, (int)remainingTime);
            timeText.text = stringBuilder.ToString();

            remainingTime -= 1f;

            yield return waitFor1s;

            if (remainingTime <= 0f)
            {
                CloseInternal();
            }
        }
    }
}

