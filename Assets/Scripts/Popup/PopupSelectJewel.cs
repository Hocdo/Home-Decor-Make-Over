using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using Popup;

public class PopupSelectJewel : PopupBase
{
	private PlayerData playerData;

	public JewelUnlockView[] jewelUnlockViews;

	private string currentSelectedId;

	public override void Show()
	{
		canClose = false;
		GetComponent<CanvasGroup>().alpha = 1f;
		PopupAnimationUtility.AnimateScale(transform, Ease.OutBack, 0.25f, 1f, 0.25f, 0f).OnComplete(() => canClose = true);
	}

	public override void Close(bool forceDestroying = true)
	{
		PopupUtility.ForceClosePopupLiteMessage();

		TerminateInternal(forceDestroying);
	}

	public void Start()
	{
		playerData = PlayerData.current;
		currentSelectedId = playerData.match3Data.jewelPackId;

		for (int i = 0; i < jewelUnlockViews.Length; i++)
        {
			JewelUnlockView jewelView = jewelUnlockViews[i];
			jewelView.index = i;

			if (playerData.match3Data.unlockedJewelPackIds.Contains(jewelView.id))
            {
				jewelView.SetUnlocked(true);

				if (playerData.match3Data.jewelPackId == jewelView.id)
				{
					jewelView.SetSelected(true);
				}
                else
                {
					jewelView.SetSelected(false);
                }
			}
            else
            {
				jewelView.UnlockEvent = OnPackSelected;

				jewelView.SetUnlocked(false);
				jewelView.SetSelected(false);
			}	
		}
	}

	public void OnPackSelected(int index)
    {
		if (jewelUnlockViews[index].isUnlocked)
        {
			playerData.match3Data.jewelPackId = jewelUnlockViews[index].id;
			jewelUnlockViews[index].SetSelected(true);

			for (int i = 0; i < jewelUnlockViews.Length; i++)
			{
				JewelUnlockView jewelView = jewelUnlockViews[i];

				if (jewelUnlockViews[i].index != index)
				{
					jewelUnlockViews[i].SetSelected(false);
				}
			}
		}
	}

	public void SelectButtonPress()
    {
		if (currentSelectedId == PlayerData.current.match3Data.jewelPackId)
        {
			CloseInternal();
        }
        else
        {
			LoadSceneUtility.LoadScene(LoadSceneUtility.Match3SceneName, () =>
			{
				AnimationController.Instance.ClearAnimationQueueGoalAndStopAllCoroutines();
				PopupSystem.Instance.CloseAllPopupsImmediately();
				SoundManager.StopMusic();
			});
		}
    }
}

