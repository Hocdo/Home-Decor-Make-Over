using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarterPackButton : MonoBehaviour
{
    public Transform lightingTransform;

    public Button button;

    public Text starterPackTimeText;

    private Coroutine timeCoroutine;

    void Start()
    {
        if (StarterPackUtility.Available())
        {
            timeCoroutine = StartCoroutine(TimeUpdate());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void StarterPackButtonPressed()
    {
        var popupStarterPack = Popup.PopupSystem.GetOpenBuilder()
           .SetType(PopupType.PopupStarterPack)
           .Open();

        popupStarterPack.AcceptEvent = (arg) => gameObject.SetActive(false);
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

        float remainingTime = StarterPackUtility.GetRemainingTimeInSeconds();
        float bias = remainingTime - (int)remainingTime;
        remainingTime -= bias;

        DateTimeUtility.ToHourMinuteSecond(stringBuilder, (int)remainingTime);
        starterPackTimeText.text = stringBuilder.ToString();

        yield return new WaitForSeconds(bias);

        while (true)
        {
            DateTimeUtility.ToHourMinuteSecond(stringBuilder, (int)remainingTime);
            starterPackTimeText.text = stringBuilder.ToString();

            remainingTime -= 1f;

            yield return waitFor1s;

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
