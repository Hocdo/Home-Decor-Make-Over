using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Popup;

public class PopupRequireChallenge : PopupBase
{
	public override void Show()
	{
		canClose = false;
		PopupAnimationUtility.AnimateScale(transform, Ease.OutBack, 0.25f, 1f, 0.25f, 0f).OnComplete(() => canClose = true);
	}

	public override void Close(bool forceDestroying = true)
	{
		PopupAnimationUtility.AnimadeAlpha(GetComponent<CanvasGroup>(), Ease.Linear, 1f, 0f, 0.1f, 0f, false);
		PopupAnimationUtility.AnimateScale(transform, Ease.OutQuart, 1f, 0.8f, 0.1f, 0f)
			.OnComplete(() => TerminateInternal(forceDestroying));
	}

	public void GetMoreChallenges()
    {
        Popup.PopupSystem.Instance.ShowPopup(PopupType.PopupGameStart, CurrentPopupBehaviour.Close, true, true);
    }
}
