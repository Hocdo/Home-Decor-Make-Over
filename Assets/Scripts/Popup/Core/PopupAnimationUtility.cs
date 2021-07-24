using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public static class PopupAnimationUtility
{
    public static Tween AnimateScale(Transform buttonTransform, Ease ease, float startScale, float targetScale, float duration, float delayTime, bool doKill = true)
    {
        if (doKill)
            buttonTransform.DOKill();
        buttonTransform.localScale = Vector3.one * startScale;
        Tween tween = buttonTransform.DOScale(targetScale, duration);
        tween.SetEase(ease);

        if (delayTime > 0f) 
            tween.SetDelay(delayTime);

        return tween;
    }

    public static Tween AnimadeAlpha(CanvasGroup canvasGroup, Ease ease, float sourceAlpha, float targetAlpha, float duration, float delayTime, bool doKill = true)
    {
        if (doKill)
            canvasGroup.DOKill();

        canvasGroup.alpha = sourceAlpha;
        Tween tween = canvasGroup.DOFade(targetAlpha, duration);
        tween.SetEase(ease);

        if (delayTime > 0f)
            tween.SetDelay(delayTime);

        return tween;
    }
}
