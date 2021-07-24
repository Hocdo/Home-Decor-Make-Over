using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using Popup;

public class PopupLiteMessage : PopupBase
{
    [SerializeField]
    private Text messageText;

    public float showDuration = 2f;

    public float fadeOutDuration = 0.5f;

    private Tween fadeInTween;

    private Tween fadeOutTween;

    private GameObject parentGameObject;

    public void SetText(string messageString)
    {
        if (parentGameObject == null)
            parentGameObject = transform.parent.gameObject;

        messageText.text = messageString;
        messageText.font = CustomLocalization.GetFont();

        if (fadeOutTween != null)
        {
            fadeOutTween.Kill();
            fadeOutTween = null;
        }

        if (parentGameObject.activeSelf == false || fadeInTween == null)
        {
            parentGameObject.SetActive(true);
            fadeInTween = canvasGroup.DOFade(1f, 0.35f);
        }

        fadeOutTween = canvasGroup.DOFade(0f, 0.5f).
            SetDelay(2f).
            OnStart(() => fadeInTween = null).
            OnComplete(() => parentGameObject.SetActive(false));
    }

    public void ForceClose()
    {
        if (parentGameObject != null && parentGameObject.activeSelf)
        {
            parentGameObject.SetActive(false);
            canvasGroup.DOKill();
            fadeInTween = null;
            fadeOutTween = null;
        }
    }
}

