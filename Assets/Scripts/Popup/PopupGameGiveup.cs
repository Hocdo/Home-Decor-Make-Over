using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using Popup;

public class PopupGameGiveup : PopupBase
{
	public Text TitleText;

	public Transform GoalGroupTransform;

	public CollectBlockView collectViewSample;

	private string sceneName;

	public override void Show()
	{
		canClose = false;
		PopupAnimationUtility.AnimateScale(transform, Ease.OutBack, 0.25f, 1f, 0.25f, 0f).OnComplete(() => canClose = true);
	}

	public override void Close(bool forceDestroying = true)
	{
		TerminateInternal(forceDestroying);

		//PopupAnimationUtility.AnimadeAlpha(GetComponent<CanvasGroup>(), Ease.Linear, 1f, 0f, 0.1f, 0f, false);
		//PopupAnimationUtility.AnimateScale(transform, Ease.OutQuart, 1f, 0.8f, 0.1f, 0f)
		//	.OnComplete(() =>
		//	{
		//		TerminateInternal(forceDestroying);
		//	});
	}

	public void Start()
	{
		var collectBlocks = MapData.main.collectBlocks;
		List<CollectBlockView> collectViews = new List<CollectBlockView>();

		for (int j = 0; j < collectBlocks.Length; j++)
		{
			if (string.IsNullOrEmpty(collectBlocks[j].blockType) || collectBlocks[j].count <= 0 || !(collectViewSample != null))
			{
				continue;
			}

			CollectBlockView component = (j == 0) ? collectViewSample
				: Instantiate(collectViewSample).GetComponent<CollectBlockView>();
			collectViews.Add(component);

			if (!component)
			{
				continue;
			}

			if (!string.IsNullOrEmpty(collectBlocks[j].blockType) && collectBlocks[j].count > 0)
			{
				component.gameObject.transform.SetParent(collectViewSample.transform.parent, worldPositionStays: false);
				CollectBlockType collectBlockType = collectBlocks[j].GetCollectBlockType();

				Sprite sprite;
				switch (collectBlockType)
				{
					case CollectBlockType.NormalRed:
					case CollectBlockType.NormalOrange:
					case CollectBlockType.NormalYellow:
					case CollectBlockType.NormalGreen:
					case CollectBlockType.NormalBlue:
					case CollectBlockType.NormalPurple:
						sprite = CollectIconTable.Instance.GetSprite(MapData.main.collectBlocks[j].blockType);
						break;
					default:
						sprite = CollectIconTable.Instance.GetSprite(MapData.main.collectBlocks[j].blockType);
						break;
					case CollectBlockType.Null:
						continue;
				}

				if (sprite != null)
				{
					int num = GameMain.main.countOfEachTargetCount[(int)collectBlockType];

					component.SetData(collectBlockType, collectBlocks[j].count, sprite);

					if (num >= 0)
						component.targetCountText.text = (collectBlocks[j].count - num).ToString();
				}
			}
			else
			{
				DestroyImmediate(component);
			}
		}

		for (int i = 0; i < collectViews.Count; i++)
        {
			collectViews[i].UpdateSize();
		}
	}

	

	private void LoadNewScene()
    {
		LoadSceneUtility.LoadScene(sceneName, () =>
		{
			AnimationController.Instance.ClearAnimationQueueGoalAndStopAllCoroutines();
			PopupSystem.Instance.CloseAllPopupsImmediately();
			SoundManager.StopMusic();
		});
	}

	public void PressConfirm()
	{
		var playerData = PlayerData.current;

		if (playerData.stamina.Available())
        {
			AcceptEvent?.Invoke(null);

			sceneName = LoadSceneUtility.Match3SceneName;

			PlayerData.current.AddStamina(-1);

			playerData.tempData.loseLevelCount++;
            if (playerData.tempData.loseLevelCount == 25)
            {
				//AppEventTracker.PushEventLoseLevelCount_Gequa25();
				Firebase.Analytics.FirebaseAnalytics.LogEvent("feature_selecterd_level_fail_01");
			}

            if (PlayerData.current.match3Data.level >= RemoteConfig.levelStartAds && PlayerData.current.noAds == false)
                AdvertisementManager.Instance.ShowInterstitial(LoadNewScene,
				  ()=> Firebase.Analytics.FirebaseAnalytics.LogEvent("InterstitialAd", "Replay","True"),
				  () => Firebase.Analytics.FirebaseAnalytics.LogEvent("InterstitialAd", "Replay", "False"));
            else
                LoadNewScene();

			GlobalEventObserver.InvokeFinishLevelEvent(false, 0);

			canvasGroup.blocksRaycasts = false;

			//AppEventTracker.LogEventLevelStatus(PlayerData.current.match3Data.level,
			//Analytics.Feature_LEVEL_STATUS.ACTION_TYPE._start,
			//Analytics.Feature_LEVEL_STATUS.STATUS_PLAY._normal,
			//Analytics.Feature_LEVEL_STATUS.RESULT.NONE);	
		}
        else
        {
			Popup.PopupSystem.
				GetOpenBuilder().
				SetType(PopupType.PopupStaminaStore).
				SetCurrentPopupBehaviour(CurrentPopupBehaviour.HideTemporary).
				Open();
		}		
	}

	public void PressHomeDesign()
    {
		var playerData = PlayerData.current;
		playerData.tempData.loseLevelCount++;
        if (playerData.tempData.loseLevelCount == 25)
        {
			Firebase.Analytics.FirebaseAnalytics.LogEvent("feature_selecterd_level_fail_01");
		}

        AcceptEvent?.Invoke(null);

		sceneName = LoadSceneUtility.HomeDesignSceneName;

		PlayerData.current.AddStamina(-1);

        if (PlayerData.current.match3Data.level >= RemoteConfig.levelStartAds && PlayerData.current.noAds == false)
            AdvertisementManager.Instance.ShowInterstitial(LoadNewScene,
				() => Firebase.Analytics.FirebaseAnalytics.LogEvent("InterstitialAd", "back_decor", "True"),
				  () => Firebase.Analytics.FirebaseAnalytics.LogEvent("InterstitialAd", "back_decor", "False"));
		else
            LoadNewScene();

		GlobalEventObserver.InvokeFinishLevelEvent(false, 0);

		canvasGroup.blocksRaycasts = false;
	}
}
