using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using Popup;

public class PopupGameStart : PopupBase
{
	[Header("Common")]
	public Text TextLevel;

	public Text PlayButtonText;

	public Text Play3MovesButtonText;

	[Header("Goal")]
    public Transform GoalGroupTransform;

    public GameObject GoalSample;

    [Header("Booster")]
    public Material disableMaterial;

	public HeadStartBoosterView[] headStartBoosterViews;

	private bool isRetryState = false;

	private bool reloadMatch3Scene = false;

	private bool[] boosterSelected;

	private PlayerData playerData;

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

		if (forceDestroying)
        {
			if (isRetryState)
			{
				AnimationController.Instance.ClearAnimationQueueGoalAndStopAllCoroutines();

				if (!reloadMatch3Scene)
				{
					if (PlayerData.current.match3Data.level >= RemoteConfig.levelStartAds && PlayerData.current.noAds == false)
                        AdvertisementManager.Instance.ShowInterstitial(
							() => LoadSceneUtility.LoadScene(LoadSceneUtility.HomeDesignSceneName),
							() =>
							{
								//Firebase.Analytics.FirebaseAnalytics.LogEvent("InterstitialAd", "back_decor", "True");
							},
							() =>
							{
								//Firebase.Analytics.FirebaseAnalytics.LogEvent("InterstitialAd", "back_decor", "False");
							});
				
				else
				{
					LoadSceneUtility.LoadScene(LoadSceneUtility.HomeDesignSceneName);
				}

					SoundManager.StopMusic();
					canvasGroup.interactable = false;
				}
			}
		}
		

		//PopupAnimationUtility.AnimadeAlpha(GetComponent<CanvasGroup>(), Ease.Linear, 1f, 0f, 0.1f, 0f, false);
		//PopupAnimationUtility.AnimateScale(transform, Ease.OutQuart, 1f, 0.8f, 0.1f, 0f)
		//	.OnComplete(() =>
		//	{
		//		TerminateInternal(forceDestroying);

		//		if (isRetryState)
		//              {
		//			AnimationController.Instance.ClearAnimationQueueGoalAndStopAllCoroutines();

		//			if (!reloadMatch3Scene)
		//                  {
		//				Model.Instance.playerData.stamina.Add(-1);

		//				if (Model.Instance.playerData.match3Data.level >= 5)
		//					AdvertisementManager.Instance.ShowInterstitial();

		//				SoundManager.StopMusic();
		//                      LoadSceneUtility.LoadScene("HomeDesignScene");
		//				canvasGroup.interactable = false;
		//			}						
		//		}
		//	});
	}

	public void Start()
    {
		playerData = PlayerData.current;

		TextLevel.text = CustomLocalization.
			Get("level") + " " + PlayerData.current.match3Data.level.ToString();

		if (LoadSceneUtility.CurrentSceneName.Equals(LoadSceneUtility.Match3SceneName))
        {
			isRetryState = true;
			PlayButtonText.text = CustomLocalization.Get("retry");
			Play3MovesButtonText.text = CustomLocalization.Get("retry");
		}

		InitGoal();

		InitBooster();
	}

	private void InitGoal()
    {
		var collectBlocks = MapData.main.collectBlocks;

		var collectViews = new List<CollectBlockView>();
		for (int j = 0; j < collectBlocks.Length; j++)
		{
			if (string.IsNullOrEmpty(collectBlocks[j].blockType) || collectBlocks[j].count <= 0 || !(GoalSample != null))
			{
				continue;
			}

			GameObject gameObject = (j == 0) ? GoalSample : Instantiate(GoalSample);
			if (!gameObject)
			{
				continue;
			}

			CollectBlockView component = gameObject.GetComponent<CollectBlockView>();
			collectViews.Add(component);
			if (!component)
			{
				continue;
			}

			if (!string.IsNullOrEmpty(collectBlocks[j].blockType) && collectBlocks[j].count > 0)
			{
				component.gameObject.transform.SetParent(GoalGroupTransform, worldPositionStays: false);
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
					component.SetData(collectBlockType, collectBlocks[j].count, sprite);
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

	private void InitBooster()
    {
		boosterSelected = new bool[headStartBoosterViews.Length];
		var boosterCounts = playerData.match3Data.headStartBoosterCounts;
		
		for (int i = 0; i < headStartBoosterViews.Length; i++)
        {
            if (playerData.match3Data.level >= BoosterUtility.GetHeadStartUnlockLevel(i))
            {
                headStartBoosterViews[i].SetCount(boosterCounts[i]);
            }
			else
            {
                headStartBoosterViews[i].Disable(disableMaterial);
            }
		}
	}

	private void ProcessBoosterCount()
    {
		for (int i = 0; i < headStartBoosterViews.Length; i++)
		{
			if (boosterSelected[i])
            {
				Match3.HeadStartBoosterType type = (Match3.HeadStartBoosterType)i;
				playerData.match3Data.AddHeadStartBooster(type, -1);
				GameMain.main.headstartBooster[i] = 1;
				
				GlobalEventObserver.InvokeUseHeadStartBoosterEvent(type);
			}
            else
            {
				GameMain.main.headstartBooster[i] = 0;
			}
		}
	}

	public void OnBoosterPressed(int index)
    {
		var boosterCounts = playerData.match3Data.headStartBoosterCounts;

        if (headStartBoosterViews[index].IsAvailable())
        {
            if (boosterCounts[index] > 0)
            {
                boosterSelected[index] = !boosterSelected[index];
                headStartBoosterViews[index].SetSelected(boosterSelected[index]);

				if (boosterSelected[index])
                {
					SoundSFX.Play(SFXIndex.GameItemButtonClickHammer);
				}
                else
                {
					AudioManager.Instance.PlaySFX(AudioClipId.ClickFailed);
				}
            }
            else
            {
                int cost = 20;

                if (playerData.gemCount >= cost)
                {
                    AudioManager.Instance.PlaySFX(AudioClipId.Purchased);

                    playerData.AddGem(-cost);
                    EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.GemChange, playerData.gemCount);

                    boosterCounts[index]++;
                    headStartBoosterViews[index].SetCount(boosterCounts[index]);
                }
                else
                {
                    PopupSystem.Instance.ShowPopup(PopupType.PopupGetMoreGems, CurrentPopupBehaviour.HideTemporary, true, true);
                }
            }
        }
        else
        {
			AudioManager.Instance.PlaySFX(AudioClipId.ClickFailed);

			var str = CustomLocalization.Get("booster_lock");
            PopupUtility.OpenPopupLiteMesage(str.Replace("xxx", BoosterUtility.GetHeadStartUnlockLevel(index).ToString()));
        }	
	}

    public void OnEventButtonGamePlay()
    {
		if (isRetryState)
		{
			AnimationController.Instance.ClearAnimationQueueGoalAndStopAllCoroutines();	
		}
		else
		{
			Decor.GameDecorController.PrepareBgTexture();
		}

		if (playerData.stamina.Available())
        {
			//AppEventTracker.LogEventLevelStatus(PlayerData.current.match3Data.level,
			//Analytics.Feature_LEVEL_STATUS.ACTION_TYPE._start,
			//Analytics.Feature_LEVEL_STATUS.STATUS_PLAY._normal,
			//Analytics.Feature_LEVEL_STATUS.RESULT.NONE);

			reloadMatch3Scene = true;
			Model.Instance.UpdatePlayerHomeDesignData();

			Action LoadSceneAction = () =>
			{
				LoadSceneUtility.LoadScene(LoadSceneUtility.Match3SceneName, () =>
				{
					PopupSystem.Instance.CloseAllPopupsImmediately();
					SoundManager.StopMusic();
				}, ProcessBoosterCount);
			};

			if (isRetryState)
            {
                if (PlayerData.current.match3Data.level >= RemoteConfig.levelStartAds && PlayerData.current.noAds == false)
                {
                    AdvertisementManager.Instance.ShowInterstitial(LoadSceneAction,
                        () =>
                        {							
							//Firebase.Analytics.FirebaseAnalytics.LogEvent("InterstitialAd", "retry", "True");
						},
                        () =>
                        {
							
							//Firebase.Analytics.FirebaseAnalytics.LogEvent("InterstitialAd", "retry", "False");
						});
                }
                else
                {
					LoadSceneAction();
				}
			}
            else
            {
				LoadSceneAction();
            }	

			canvasGroup.blocksRaycasts = false;
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

	public void OnEventButtonGamePlay3Moves()
    {
		if (playerData.stamina.Available())
		{
			Action RewardedVideoReward = () =>
			{
				GameMain.rewardMove3ByADStart = true;

				if (isRetryState)
				{
					AnimationController.Instance.ClearAnimationQueueGoalAndStopAllCoroutines();
				}
				else
				{
					Decor.GameDecorController.PrepareBgTexture();
				}

				reloadMatch3Scene = true;
				Model.Instance.UpdatePlayerHomeDesignData();
				LoadSceneUtility.LoadScene(LoadSceneUtility.Match3SceneName, () =>
				{
					PopupSystem.Instance.CloseAllPopupsImmediately();
					SoundManager.StopMusic();
				}, ProcessBoosterCount);

				canvasGroup.blocksRaycasts = false;

				//AppEventTracker.LogEventRewardAd("match3_start_+3", true);

				playerData.tempData.extra3MovesRewardCount++;
                if (playerData.tempData.extra3MovesRewardCount == 5)
                {					
					Firebase.Analytics.FirebaseAnalytics.LogEvent("Feature_selected_5_ads_3move");
                }

                //AppEventTracker.LogEventLevelStatus(PlayerData.current.match3Data.level,
                //Analytics.Feature_LEVEL_STATUS.ACTION_TYPE._start,
                //Analytics.Feature_LEVEL_STATUS.STATUS_PLAY._watch_ads,
                //Analytics.Feature_LEVEL_STATUS.RESULT.NONE);
            };

            Action RewardedVideoFailed = () =>
            {
                //AppEventTracker.LogEventRewardAd("match3_start_+3", false);
            };

            AdvertisementManager.Instance.ShowRewardedVideo(RewardedVideoReward, RewardedVideoFailed);
            
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
}
