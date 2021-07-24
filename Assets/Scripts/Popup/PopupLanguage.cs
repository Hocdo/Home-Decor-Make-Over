using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Popup;
using DG.Tweening;

public class PopupLanguage : PopupBase
{
    [Serializable]
    public struct LanguageButton
    {
        public LanguageType language;
        public Button button;
    }

    public LanguageButton[] languageButtons;

    public override void Show()
    {
        if (languageButtons != null && languageButtons.Length > 0)
        {
            for (int i = 0; i < languageButtons.Length; i++)
            {
                LanguageType language = languageButtons[i].language;
                Button button = languageButtons[i].button;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => SelectLanguage(language));
            }
        }

        canClose = false;
        PopupAnimationUtility.AnimateScale(transform, Ease.OutBack, 0.25f, 1f, 0.25f, 0f).OnComplete(() => canClose = true);
    }

    public override void Close(bool forceDestroying = true)
    {
        PopupAnimationUtility.AnimadeAlpha(GetComponent<CanvasGroup>(), Ease.Linear, 1f, 0f, 0.1f, 0f, false);
        PopupAnimationUtility.AnimateScale(transform, Ease.OutQuart, 1f, 0.8f, 0.1f, 0f)
            .OnComplete(() => TerminateInternal(forceDestroying));
    }

    public void SelectLanguage(LanguageType language)
    {
        CloseInternal();

        CustomLocalization.SetLanguage(language);
    }
}
