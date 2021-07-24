using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Popup;
using DG.Tweening;

public class PopupGameComplete : PopupBase
{
    public TextEffectLevelComplete levelCompleteEffect;

    public override void Show()
    {
        canClose = false;
        levelCompleteEffect.gameObject.SetActive(false);

        StartCoroutine(ShowCoroutine());
    }

    public IEnumerator ShowCoroutine()
    {
        yield return new WaitForSeconds(0.25f);

        levelCompleteEffect.gameObject.SetActive(true);
        levelCompleteEffect.Play();

        yield return new WaitForSeconds(3.2f);

        canClose = true;
        CloseInternal();
    }

    public override void Close(bool forceDestroying = true)
    {
        TerminateInternal(forceDestroying);
    }
}
