using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using DG.Tweening;
using Popup;

public class PopupRating : PopupBase
{
	public override void Show()
	{
		canClose = false;
		PopupAnimationUtility.AnimateScale(transform, Ease.OutBack, 0.25f, 1f, 0.25f, 0f).OnComplete(() => canClose = true);
	}

	public override void Close(bool forceDestroying = true)
	{		
		TerminateInternal(forceDestroying);
	}

	public void Rate14Stars()
    {
		CloseInternal();
    }

	public void Rate5Stars()
    {
		PlayerData.current.appRated = true;
		Application.OpenURL("https://play.google.com/store/apps/details?id=" + Application.identifier);

		CloseInternal();
	}
}
