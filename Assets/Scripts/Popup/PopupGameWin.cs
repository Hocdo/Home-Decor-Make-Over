using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Popup;
using Spine.Unity;
using Spine;

public class PopupGameWin : PopupBase
{
    public AnimationCurve bigCoinAnimCurve;

    public TextEffectVictory victoryEffect;

    public Transform bigCoinTransform;

    public CanvasGroup continueCanvasGroup;

    public RectTransform characterTransform;

    public SkeletonGraphic characterGraphic;

    [Header("Coin")]
    public CanvasGroup bonusCoinCanvasGroup;

    public RectTransform bonusGemRectTransform;

    public RectTransform currentCoin;

    public Transform sourceCoinTransform;

    public Transform targetCoinTransform;

    public Transform coinOrbitControlTransform;

    public Button moreCoinButton;

    public Button moreGemButton;

    public Text orText;

    public Text currentCoinText;

    public Image bonusCoinImage;

    public Text bonusCoinText;

    public GameObject coinSample;

    public float coinFlyingTime = 1f;

    public float coinFlyingInterval = 0.2f;

    [Header("Gem")]
    public CanvasGroup bonusGemCanvasGroup;

    public RectTransform currentGem;

    public Transform sourceGemTransform;

    public Transform targetGemTransform;

    public Transform gemOrbitControlTransform;

    public Text currentGemText;

    public Image bonusGemImage;

    public Text bonusGemText;

    public GameObject gemSample;

    private GameObject[] coinGameObjects;

    private GameObject[] gemGameObjects;

    public Action CloseEvent;

    private int currentCoinCount;

    private int bonusCoinCount;

    private int targetCoinCount;

    private int currentGemCount;

    private int bonusGemCount;

    private int targetGemCount;

    private bool playingAddCoinSFX;

    private bool canGetMoreCoin = true;

    private bool canGetMoreGem = true;

    private bool canPressContinue = true;

    private UIEdgeSnapPosition currentCoinEdgeSnap;

    private UIEdgeSnapPosition currentGemEdgeSnap;

    private UIEdgeSnapPosition characterEdgeSnap;

    public void SetBonusCoin(int abonusCoinCount)
    {        
        currentCoinCount = PlayerData.current.cointCount;
        bonusCoinCount = abonusCoinCount;
        targetCoinCount = currentCoinCount + bonusCoinCount;

        currentGemCount = PlayerData.current.gemCount;
        bonusGemCount = 1;
        targetGemCount = currentGemCount + bonusGemCount;

        currentCoinText.text = currentCoinCount.ToString();
        bonusCoinText.text = bonusCoinCount.ToString();

        currentGemText.text = currentGemCount.ToString();
        bonusGemText.text = bonusGemCount.ToString();

        coinGameObjects = new GameObject[5];
        for (int i = 0; i < coinGameObjects.Length; i++)
        {
            coinGameObjects[i] = (i == 0) ? coinSample : Instantiate(coinSample, transform);
            coinGameObjects[i].transform.position = bonusCoinImage.transform.position;
        }

        gemGameObjects = new GameObject[4];
        for (int i = 0; i < gemGameObjects.Length; i++)
        {
            gemGameObjects[i] = (i == 0) ? gemSample : Instantiate(gemSample, transform);
            gemGameObjects[i].transform.position = bonusCoinImage.transform.position;
        }
    }

    public override void Show()
    {
        canPressContinue = false;
        victoryEffect.gameObject.SetActive(false);

        float currentScale = bigCoinTransform.localScale.x;
        bigCoinTransform.localScale = Vector3.zero;
        bigCoinTransform.DOScale(currentScale, 0.75f).SetEase(bigCoinAnimCurve);

        bigCoinTransform.localRotation = Quaternion.Euler(0f, 0f, 15f);
        bigCoinTransform.DOLocalRotateQuaternion(Quaternion.Euler(0f, 0f, 0f), 0.75f).SetEase(bigCoinAnimCurve);

        canClose = false;
        currentCoinEdgeSnap = new UIEdgeSnapPosition(currentCoin, new Vector2(0f, 1.2f));
        currentCoinEdgeSnap.SetPositionVisibility(false);
        currentCoinEdgeSnap.Show(0.5f).OnComplete(() => 
        {          
            canClose = true; 
        });

        currentGemEdgeSnap = new UIEdgeSnapPosition(currentGem, new Vector2(0f, 1.2f));
        currentGemEdgeSnap.SetPositionVisibility(false);
        currentGemEdgeSnap.Show(0.5f);

        this.ExecuteAfterSeconds(0.75f, () => 
        {
            victoryEffect.gameObject.SetActive(true);
            victoryEffect.Play();
        });

        bonusCoinCanvasGroup.alpha = 0f;
        bonusCoinCanvasGroup.DOFade(1f, 0.5f).SetDelay(0.75f);
        bonusCoinCanvasGroup.gameObject.transform.localScale = 0.35f * Vector3.one;
        bonusCoinCanvasGroup.gameObject.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetDelay(0.75f);

        bonusGemCanvasGroup.alpha = 0f;
        bonusGemCanvasGroup.DOFade(1f, 0.5f).SetDelay(0.75f);
        bonusGemCanvasGroup.gameObject.transform.localScale = 0.35f * Vector3.one;
        bonusGemCanvasGroup.gameObject.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetDelay(0.75f);

        var moreCoinCanvasGroup = moreCoinButton.GetComponent<CanvasGroup>();
        moreCoinCanvasGroup.alpha = 0f;
        moreCoinCanvasGroup.DOFade(1f, 0.5f).SetDelay(0.95f);

        var color = orText.color;
        orText.color = new Color(color.r, color.g, color.b, 0f);
        orText.DOFade(1f, 0.5f).SetDelay(1.1f);

        var moreGemCanvasGroup = moreGemButton.GetComponent<CanvasGroup>();
        moreGemCanvasGroup.alpha = 0f;
        moreGemCanvasGroup.DOFade(1f, 0.5f).SetDelay(1.25f);

        //var color = continueText.color;
        //continueText.color = new Color(color.r, color.g, color.b, 0f);
        continueCanvasGroup.DOFade(1f, 0.5f).SetDelay(3.5f)
            .OnComplete(() => canPressContinue = true);

        characterEdgeSnap = new UIEdgeSnapPosition(characterTransform, new Vector2(0f, -1.2f));
        characterEdgeSnap.SetPositionVisibility(false);
        characterEdgeSnap.Show(0.5f).SetEase(Ease.OutBack);

        characterGraphic.AnimationState.AddAnimation(0, "win", true, 0f);
    }

    public void OnPressContinue()
    {
        if (canPressContinue == false) return;
        canPressContinue = false;

        canvasGroup.blocksRaycasts = false;
        StartCoroutine(AddBonusCoinCoroutine());
        StartCoroutine(AddBonusGemCoroutine());          
    }
    
    public void GetMore50PercentCoin()
    {
        if (canGetMoreCoin == false) return;

        Action RewardedVideoReward = () =>
        {
            moreCoinButton.gameObject.SetActive(false);
            moreGemButton.gameObject.SetActive(false);
            orText.gameObject.SetActive(false);

            continueCanvasGroup.DOKill();
            continueCanvasGroup.DOFade(0f, 0.2f);

            canGetMoreCoin = false;
            canGetMoreGem = false;
            canPressContinue = false;
            int newBonusCoin = (int)(1.5f * bonusCoinCount);

            DOTween.To(() => bonusCoinCount, (value) => bonusCoinCount = value, newBonusCoin, 1f).
            OnUpdate(() => bonusCoinText.text = bonusCoinCount.ToString()).
            OnComplete(() =>
            {
                canPressContinue = true;
                targetCoinCount = currentCoinCount + bonusCoinCount;
            });

            AudioManager.Instance.PlaySFX(AudioClipId.DailyBonus);

            //AppEventTracker.LogEventRewardAd("match3win_bonuscoin50%", true);
            Firebase.Analytics.FirebaseAnalytics.LogEvent("match3win_bonuscoin50","status", "true");
        };

        Action RewardedVideoFailed = () =>
        {
            //AppEventTracker.LogEventRewardAd("match3win_bonuscoin50%", false);
            Firebase.Analytics.FirebaseAnalytics.LogEvent("match3win_bonuscoin50", "status", "false");
        };

        AdvertisementManager.Instance.ShowRewardedVideo(RewardedVideoReward, RewardedVideoFailed);
    }

    public void GetMoreGems()
    {
        if (canGetMoreGem == false) return;

        Action RewardedVideoReward = () =>
        {
            moreCoinButton.gameObject.SetActive(false);
            moreGemButton.gameObject.SetActive(false);
            orText.gameObject.SetActive(false);

            continueCanvasGroup.DOKill();
            continueCanvasGroup.DOFade(0f, 0.2f);

            canGetMoreCoin = false;
            canGetMoreGem = false;
            canPressContinue = true;
            bonusGemCount += UnityEngine.Random.Range(1, 4);
            targetGemCount = currentGemCount + bonusGemCount;

            bonusGemText.text = bonusGemCount.ToString();

            AudioManager.Instance.PlaySFX(AudioClipId.DailyBonus);

            //AppEventTracker.LogEventRewardAd("match3win_bonusgem", true);
        };

        Action RewardedVideoFailed = () =>
        {
           // AppEventTracker.LogEventRewardAd("match3win_bonusgem%", false);
        };

        AdvertisementManager.Instance.ShowRewardedVideo(RewardedVideoReward, RewardedVideoFailed);
    }

    private IEnumerator AddBonusCoinCoroutine()
    {
        PlayerData.current.AddCoin(bonusCoinCount);

        canGetMoreCoin = false;
        float coinBonusDuration = coinFlyingInterval * (coinGameObjects.Length - 1);

        DOTween.To(() => bonusCoinCount, (value) => bonusCoinCount = value, 0, coinBonusDuration)
            .OnUpdate(() => bonusCoinText.text = bonusCoinCount.ToString()).OnComplete(() => 
            {
                bonusCoinText.DOColor(Color.clear, 0.3f);
                bonusCoinImage.DOColor(Color.clear, 0.3f);
            });

        DOTween.To(() => currentCoinCount, (value) => currentCoinCount = value, targetCoinCount, coinBonusDuration)
            .OnUpdate(() => currentCoinText.text = currentCoinCount.ToString()).SetDelay(coinFlyingTime);

        Vector3 targetPosition = targetCoinTransform.position;
        Vector3 sourcePosition = sourceCoinTransform.position;
        Vector3 controlPosition = coinOrbitControlTransform.position;

        for (int i = 0; i < coinGameObjects.Length; i++)
        {
            GameObject coin = coinGameObjects[i];
            var trail = coin.transform.GetChild(0).GetComponent<TrailRenderer>();
            float scale = coinGameObjects[0].transform.localScale.x * 1f;
            float width = trail.startWidth * 1f;

            coin.transform.position = sourcePosition;
            coin.SetActive(true);
            trail.DOResize(width, width, coinFlyingTime);

            float t = 0;
            DOTween.To(() => t, (value) => t = value, 1f, coinFlyingTime).
                SetEase(Ease.InOutSine).
                OnUpdate(() => coin.transform.position = (1 - t) * (1 - t) * sourcePosition + 2f * (1 - t) * t * controlPosition + t * t * targetPosition);
            //coin.transform.DOMove(targetPosition, coinFlyingTime).SetEase(Ease.InOutSine);

            coin.transform.DOScale(scale, coinFlyingTime).OnComplete(() => 
            {
                coin.GetComponent<SpriteRenderer>().enabled = false;
                coin.transform.GetChild(1).GetComponent<ParticleSystem>().Stop(false);
                AudioManager.Instance.PlaySFX(AudioClipId.AddCoin);
            });

            yield return new WaitForSeconds(coinFlyingInterval);        
        }

        yield return new WaitForSeconds(coinFlyingTime + coinFlyingInterval * coinGameObjects.Length + 0.25f);

        CloseEvent?.Invoke();

        AnimationController.Instance.ClearAnimationQueueGoalAndStopAllCoroutines();

        if (PlayerData.current.match3Data.level >= RemoteConfig.levelStartAds && PlayerData.current.noAds == false)
            AdvertisementManager.Instance.ShowInterstitial(LoadHomeDesignScene,
                () => Firebase.Analytics.FirebaseAnalytics.LogEvent("InterstitialAd", "win", "True"),
                () => Firebase.Analytics.FirebaseAnalytics.LogEvent("InterstitialAd", "win", "False"));
        else
            LoadHomeDesignScene();
    }

    private IEnumerator AddBonusGemCoroutine()
    {
        PlayerData.current.AddGem(bonusGemCount);
        int addGem = bonusGemCount;

        float gemBonusDuration = coinFlyingInterval * (bonusGemCount - 1);
        DOTween.To(() => bonusGemCount, (value) => bonusGemCount = value, 0, gemBonusDuration)
            .OnUpdate(() => bonusCoinText.text = bonusCoinCount.ToString()).OnComplete(() =>
            {
                bonusCoinText.DOColor(Color.clear, 0.3f);
                bonusCoinImage.DOColor(Color.clear, 0.3f);
            });

        bonusGemText.color = Color.clear;
        bonusGemImage.color = Color.clear;

        DOTween.To(() => currentGemCount, (value) => currentGemCount = value, targetGemCount, gemBonusDuration).SetEase(Ease.Linear)
            .OnUpdate(() => currentGemText.text = currentGemCount.ToString()).SetDelay(coinFlyingTime);

        Vector3 targetPosition = targetGemTransform.position;
        Vector3 sourcePosition = sourceGemTransform.position;
        Vector3 controlPosition = gemOrbitControlTransform.position;

        
        for (int i = 0; i < addGem; i++)
        {
            Debug.Log("Gem " + i);

            GameObject gem = gemGameObjects[i];
            gem.transform.position = sourcePosition;
            gem.SetActive(true);
            float t = 0;
            DOTween.To(() => t, (value) => t = value, 1f, coinFlyingTime).
                SetEase(Ease.InOutSine).
                OnUpdate(() => gem.transform.position = (1 - t) * (1 - t) * sourcePosition + 2f * (1 - t) * t * controlPosition + t * t * targetPosition).
                OnComplete(() =>
                 {
                     gem.GetComponent<SpriteRenderer>().enabled = false;
                     AudioManager.Instance.PlaySFX(AudioClipId.AddGem);
                 });

            yield return new WaitForSeconds(coinFlyingInterval);
        }
    }

    private void LoadHomeDesignScene()
    {
        LoadSceneUtility.LoadScene(LoadSceneUtility.HomeDesignSceneName, () =>
        {
            SoundManager.StopMusicImmediately();
            PopupSystem.Instance.CloseAllPopupsImmediately();
        });
    }
}
