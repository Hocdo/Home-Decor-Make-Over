using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using DG.Tweening;
using Popup;

public class PopupQuitGame : PopupBase
{
    public override void Show()
    {
        canClose = false;
        GetComponent<CanvasGroup>().alpha = 1f;
        PopupAnimationUtility.AnimateScale(transform, Ease.OutBack, 0.25f, 1f, 0.25f, 0f).OnComplete(() => canClose = true);
    }

    public override void Close(bool forceDestroying = true)
    {
        PreAnimateHideEvent?.Invoke();

        TerminateInternal(forceDestroying);
    }

    public void QuitButtonPress()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ResumeButtonPress()
    {
        CloseInternal();
    }
}
