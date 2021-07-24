using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PopupDarkEffect : Popup.PopupBackgroundEffect
{
    public RawImage image;

    public Color enabledColor = new Color(0f, 0f, 0f, 0.647f);

    public Color disabledColor = Color.clear;

    public override bool IsActive()
    {
        return image.gameObject.activeSelf;
    }

    public override void SetActive(bool flag)
    {
        if (flag)
        {
            image.gameObject.SetActive(true);
            image.DOKill();
            image.color = disabledColor;
            image.DOColor(enabledColor, enabledDuration);
        }
        else
        {
            image.DOKill();
            image.color = enabledColor;
            image.DOColor(disabledColor, disabledDuration).OnComplete(() => image.gameObject.SetActive(false));
        }
    }

    public override void SetActiveImmediately(bool flag)
    {
        if (flag)
        {
            image.DOKill();
            image.color = enabledColor;
        }
        else
        {
            image.DOKill();
            image.color = disabledColor;
        }
    }
}
