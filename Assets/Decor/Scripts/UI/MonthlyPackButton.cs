using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonthlyPackButton : MonoBehaviour
{
    public Transform lightingTransform;

    public Button button;

    public Text timeText;

    private Coroutine timeCoroutine;

    void Start()
    {
        if (MonthlyPackUtility.Available())
        {
            timeCoroutine = StartCoroutine(TimeUpdate());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void ButtonPressed()
    {
        var popupMonthlyPack = Popup.PopupSystem.GetOpenBuilder()
           .SetType(PopupType.PopupMonthlyPack)
           .Open();

        popupMonthlyPack.AcceptEvent = (arg) => gameObject.SetActive(false);
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

        float remainingTime = MonthlyPackUtility.GetRemainingTimeInSeconds();
        float bias = 0f;

        if (remainingTime <= 86400f)
        {
            DateTimeUtility.ToHourMinuteSecond(stringBuilder, (int)remainingTime);
            bias = remainingTime - (int)remainingTime;
            remainingTime -= bias;
        }
        else
        {
            DateTimeUtility.ToDayHour(stringBuilder, remainingTime);
            bias = remainingTime - ((int)(remainingTime / 3600f)) * 3600f;
            remainingTime -= bias;
        }

        timeText.text = stringBuilder.ToString();

        yield return new WaitForSeconds(bias);

        while (true)
        {
            DateTimeUtility.ToDayHour(stringBuilder, remainingTime);
            timeText.text = stringBuilder.ToString();

            if (remainingTime <= 86400f)
            {
                remainingTime -= 1f;

                yield return waitFor1s;
            }
            else
            {
                remainingTime -= 3600f;

                yield return new WaitForSeconds(3600f);
            }

            if (remainingTime <= 0f)
            {
                gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        lightingTransform.Rotate(new Vector3(0f, 0f, -30f * Time.deltaTime), Space.Self);
    }
}
