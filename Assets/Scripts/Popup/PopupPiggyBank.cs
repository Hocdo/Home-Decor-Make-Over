using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using DG.Tweening;
using Popup;
using System.Text;

public class PopupPiggyBank : PopupBase
{
	public Button playButton;

	public Button buyButton;

    public Text priceText;

    public GameObject pigNormal;

    public GameObject pigFull;

    public Transform lightTransform;

    [Header("Time")]
	public GameObject timeObject;

    public Text timeText;

    [Header("Progress")]
	public Text minGemText;

	public Text maxGemText;

	public Image progressImage;

	[Header("Text")]
    public Text gemText;

    public Text smashText;

	public Text normalText;

    private Coroutine timeUpdateCoroutine;

	private PiggyBankData piggyBankData;

    private string productId;

    private float priceUsd;

    private void Start()
    {
		piggyBankData = PlayerData.current.piggyBankData;
        productId = PiggyBankUtility.GetIapProductId();
        priceUsd = PiggyBankUtility.GetIapDefaultPriceUsd();

        gemText.text = piggyBankData.gemCount.ToString();
		IntegerRange smashRange = PiggyBankUtility.GetSmashRange(piggyBankData.level);
		minGemText.text = smashRange.min.ToString();
		maxGemText.text = smashRange.max.ToString();

		progressImage.fillAmount = (float)piggyBankData.gemCount / smashRange.max;

		float percentOfMin = (float)smashRange.min / smashRange.max;
		minGemText.rectTransform.anchoredPosition = 
			new Vector2(progressImage.rectTransform.rect.width * percentOfMin, minGemText.rectTransform.anchoredPosition.y);

		bool canSmash = PiggyBankUtility.CanSmash();

		playButton.gameObject.SetActive(!canSmash);
		buyButton.gameObject.SetActive(canSmash);

		normalText.gameObject.SetActive(!canSmash);
		smashText.gameObject.SetActive(canSmash);

        pigNormal.SetActive(!canSmash);
        pigFull.SetActive(canSmash);

        lightTransform.gameObject.SetActive(canSmash);

        if (canSmash == false)
		{
			timeObject.SetActive(false);
			normalText.text = normalText.text.Replace("300", smashRange.min.ToString());    
        }
		else 
		{
            StartCoroutine(LightRotate());
            timeUpdateCoroutine = StartCoroutine(TimeUpdate());

            Product product = IAPManager.Instance.GetProduct(productId);

            if (product != null)
            {
                string localizedPriceString = product.metadata.localizedPriceString;
                priceText.text = localizedPriceString;
            }
            else
            {
                priceText.text = "$" + priceUsd.ToString();
            }
        }
	}

    public override void Show()
	{
        canClose = false;
        PopupAnimationUtility.AnimateScale(transform, Ease.OutBack, 0.25f, 1f, 0.25f, 0f).OnComplete(() => canClose = true);
    }

	public override void Close(bool forceDestroying = true)
	{
		TerminateInternal(forceDestroying);
	}

    private IEnumerator LightRotate()
    {
        while (true)
        {
            lightTransform.Rotate(new Vector3(0f, 0f, 30 * Time.deltaTime), Space.Self);

            yield return null;
        }
    }

    public void PressPlayButton()
	{
		Popup.PopupSystem.GetOpenBuilder().
						SetType(PopupType.PopupGameStart).
						SetCurrentPopupBehaviour(Popup.CurrentPopupBehaviour.Close).
						Open();
	}

	public void PressBuyButton()
    {
        IAPManager.Instance.PurchaseButtonClick(productId, PurchaseGemSucessful);
    }

    public void BuyButtonPress()
    {
       
    }

    public void PurchaseGemSucessful(PurchaseEventArgs args)
    {
        AudioManager.Instance.PlaySFX(AudioClipId.Purchased);
        int bonusCount = piggyBankData.gemCount;

        PlayerData playerData = PlayerData.current;
        playerData.AddGem(bonusCount);

        PiggyBankUtility.OnSmashSucessful();

        AcceptEvent?.Invoke(null);

        playerData.tempData.spentIAP += priceUsd;
        if (playerData.tempData.spentIAP >= 5f && playerData.tempData.push_spentIAP_event == false)
        {
            playerData.tempData.push_spentIAP_event = true;
            //AppEventTracker.PushEventSpentIAP_Gequal_5Dollar();
            Firebase.Analytics.FirebaseAnalytics.LogEvent("Feature_selected_IAPspend_5usd");
        }
        //AppEventTracker.LogEventIap(productId, priceUsd.ToString());

        var popupReward = PopupSystem.GetOpenBuilder().
            SetType(PopupType.PopupReward).
            SetCurrentPopupBehaviour(CurrentPopupBehaviour.Close).
            Open<PopupReward>();

        popupReward.Add(RewardType.CurrencyGem, bonusCount)
            .CompleteAction = () => EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.GemAnimChange, new int[] { playerData.gemCount, 1000 });

        popupReward.Align(2f, 2f);
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

        float timeToPeriodExpire = PiggyBankUtility.Get_RemainingTime_To_PeriodExpired();

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
                timeText.text = "00:00:00";
                break;
            }
        }

        timeUpdateCoroutine = null;
    }
}