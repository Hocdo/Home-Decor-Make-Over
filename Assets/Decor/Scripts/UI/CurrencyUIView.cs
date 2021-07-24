using Decor;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyUIView : MonoBehaviour
{
    [Header("Coin")]
    public Text coinText;

    private int coinCount;

    [Header("Gem")]
    public Text gemText;

    private int gemCount;

    [Header("Stamina")]
    public Image staminaInfinityImage;

    public Text staminaText;

    public Text staminaStatusText;

    public Text staminaFullText;

    [Header("Star")]
    public Text starText;

    private StringBuilder stringBuilder = new StringBuilder();

    [NonSerialized]
    public Stamina stamina;

    private int staminaRemainingTime;
    private int staminaInfinityRemainingTime;

    private void Awake()
    {
        EventDispatcher<GlobalEventId>.Instance.RegisterEvent(GlobalEventId.CoinChange, UpdateCoin);
        EventDispatcher<GlobalEventId>.Instance.RegisterEvent(GlobalEventId.GemChange, UpdateGem);
        EventDispatcher<GlobalEventId>.Instance.RegisterEvent(GlobalEventId.StaminaChange, UpdateStamina);

        EventDispatcher<GlobalEventId>.Instance.RegisterEvent(GlobalEventId.GemAnimChange, UpdateGemAnim);
        EventDispatcher<GlobalEventId>.Instance.RegisterEvent(GlobalEventId.CoinAnimChange, UpdateCoinAnim);
    }

    private void OnDestroy()
    {
        EventDispatcher<GlobalEventId>.Instance.RemoveEvent(GlobalEventId.CoinChange, UpdateCoin);
        EventDispatcher<GlobalEventId>.Instance.RemoveEvent(GlobalEventId.GemChange, UpdateGem);
        EventDispatcher<GlobalEventId>.Instance.RemoveEvent(GlobalEventId.StaminaChange, UpdateStamina);

        EventDispatcher<GlobalEventId>.Instance.RemoveEvent(GlobalEventId.GemAnimChange, UpdateGemAnim);
        EventDispatcher<GlobalEventId>.Instance.RemoveEvent(GlobalEventId.CoinAnimChange, UpdateCoinAnim);
    }

    public void Start()
    {
        staminaText.gameObject.SetActive(true);
        staminaRemainingTime = stamina.GetRemainingTime();
        staminaInfinityRemainingTime = stamina.GetInfinityRemainingTime();

        StartCoroutine(DecreaseStaminaDurationCoroutine());
    }

    public void OnApplicationFocus(bool focus)
    {
        UpdateStamina();
    }

    public void OnApplicationPause(bool pause)
    {
        UpdateStamina();
    }

#if UNITY_EDITOR
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            stamina.Add(-1);
            UpdateStamina();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            stamina.Add(+1);
            UpdateStamina();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            stamina.AddInfinity(900);
            staminaInfinityRemainingTime = stamina.GetInfinityRemainingTime();
        }
    }
#endif

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
        if (staminaInfinityRemainingTime > 0)
        {
            staminaInfinityRemainingTime -= 1;

            staminaInfinityImage.enabled = true;
            staminaText.enabled = false;

            DateTimeUtility.ToMinuteSecond(stringBuilder, staminaInfinityRemainingTime);
            staminaStatusText.text = stringBuilder.ToString();
            staminaFullText.gameObject.SetActive(false);
        }
        else
        {
            staminaInfinityImage.enabled = false;
            staminaText.enabled = true;

            if (stamina.IsFull())
            {
                staminaFullText.gameObject.SetActive(true);
                staminaStatusText.text = "";
                staminaRemainingTime = Stamina.StaminaFillDuration;
            }
            else
            {
                staminaRemainingTime -= 1;
                if (staminaRemainingTime < 0)
                {
                    stamina.OnRemainingTimeZero();

                    if (stamina.IsFull())
                        staminaStatusText.text = "";

                    staminaRemainingTime = Stamina.StaminaFillDuration;
                }

                DateTimeUtility.ToMinuteSecond(stringBuilder, staminaRemainingTime);
                staminaStatusText.text = stringBuilder.ToString();
                staminaFullText.gameObject.SetActive(false);
            }

            UpdateStaminaText(stamina.count);
        }
    }

    private void UpdateStamina(object param = null)
    {
        staminaRemainingTime = stamina.GetRemainingTime();
        staminaInfinityRemainingTime = stamina.GetInfinityRemainingTime();
        StaminaSchedule();
    }

    public void PlayChangeCoinAnim(float duration, int delta)
    {
        DOTween.To(() => coinCount, (value) => coinCount = value, Mathf.Max(0, coinCount + delta), duration)
            .OnUpdate(() => UpdateCoin(coinCount));
    }

    void UpdateCoin(object param)
    {
        UpdateCoin((int)param);
    }

    void UpdateCoinAnim(object param)
    {
        int[] args = (int[])param;
        int targetCount = args[0];
        float duration = args[1] / 1000f;

        DOTween.To(() => coinCount, (value) => coinCount = value, targetCount, duration)
            .OnUpdate(() => coinText.text = coinCount.ToString());
    }

    void UpdateGemAnim(object param)
    {
        int[] args = (int[])param;
        int targetCount = args[0];
        float duration = args[1] / 1000f;

        DOTween.To(() => gemCount, (value) => gemCount = value, targetCount, duration)
            .OnUpdate(() => gemText.text = gemCount.ToString());
    }

    void UpdateGem(object param)
    {
        UpdateGem((int)param);
    }

    void UpdateStar(object param)
    {
        UpdateStar((int)param);
    }

    public void UpdateCoin(int value)
    {
        coinCount = value;
        coinText.text = value.ToString();
    }

    public void UpdateGem(int value)
    {
        gemCount = value;
        gemText.text = value.ToString();
    }

    public void UpdateStaminaText(int value)
    {
        staminaText.text = value.ToString();
    }

    public void UpdateStar(int value)
    {
        starText.text = value.ToString();
    }  

    public void OnCoinClicked()
    {
        Popup.PopupSystem.GetOpenBuilder()
              .SetType(PopupType.PopupRequirePlayMatch3)
              .Open();
    }

    public void OnGemClicked()
    {
        Popup.PopupSystem.GetOpenBuilder()
              .SetType(PopupType.PopupGemStore)
              .Open();
    }

    public void OnStaminaClicked()
    {
        if (stamina.IsFull() == false && stamina.GetInfinityRemainingTime() <= 0)
        {
            var popupStaminaStore = Popup.PopupSystem.GetOpenBuilder()
               .SetType(PopupType.PopupStaminaStore)
               .Open<PopupStaminaStore>();

            if (popupStaminaStore)
                popupStaminaStore.UpdateStaminaTimeText(staminaStatusText.text);
        }
    }
}
