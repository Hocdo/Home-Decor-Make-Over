using DG.Tweening;
using PathologicalGames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMain : MonoBehaviour
{
	public static bool played = false;
	private struct FromToCoordinate
	{
		public int FromX;

		public int FromY;

		public int ToX;

		public int ToY;
	}

	public class Solution
	{
		public int aiBackStoneCount;

		public int aiBackStoneIncludeRescueGingerManCount;

		public int aiBackStoneNullCount;

		public int boxCombinationCount;

		public bool[] boxCombinationPos = new bool[4];

		public int color;

		public int count;

		public bool h;

		public int itemBlock33Count;

		public int itemBlock41Count;

		public int itemBlock51Count;

		public int negH;

		public int negV;

		public int posH;

		public int posV;

		public int potential;

		public bool v;

		public int x;

		public int y;

		public bool IsCreateItemBlock()
		{
			if (boxCombinationCount > 0 || itemBlock33Count > 0 || itemBlock41Count > 0 || itemBlock51Count > 0)
			{
				return true;
			}
			return false;
		}
	}

	public class Move
	{
		public int fromX;

		public int fromY;

		public int potencial;

		public Solution solution;

		public int toX;

		public int toY;
	}

	public static GameMain main;

	private readonly List<Solution> solutions = new List<Solution>();

	public Camera UIGameCamera;

	public Camera GameEffectCamera;

	public static readonly float BounceTweenTime = 0.3f;

	public static readonly float BlockDropDelayTime = 0.15f;

	private static int DefaultSortingLayerID;

	private static int GameEffectSortingLayerID;

	public static bool rewardMove3ByADStart;

	public MeshRenderer bgRenderer;

	public Match3.TutorialController tutorialController;

	public Sprite SpriteFilledMilk;

	[HideInInspector]
	public bool doingStartCameraTween;

	[HideInInspector]
	public int animate;

	[HideInInspector]
	public int matching;

	[HideInInspector]
	public int rolling;

	[HideInInspector]
	public int gravity;

	[HideInInspector]
	public int lastMovementId;

	[HideInInspector]
	public int swapEvent;

	[HideInInspector]
	public float timeLeft;

	[HideInInspector]
	public int eventCount;

	[HideInInspector]
	public bool IsMovingNextMap;

	[HideInInspector]
	public bool isPlaying;

	[HideInInspector]
	public bool isBonusTime;

	[HideInInspector]
	public bool isGameResult;

	[HideInInspector]
	public bool isLockDrop;

	[HideInInspector]
	public bool reachedTheTarget;

	[HideInInspector]
	public bool firstChipGeneration;

	[HideInInspector]
	public bool isTurnResultEnd = true;

	[HideInInspector]
	public bool isJustGenerateChip;

	[HideInInspector]
	public int fillingMilkEffectCount;

	[HideInInspector]
	public int throwingMoveEffectCount;

	[HideInInspector]
	public bool isFirstBoardSetting;

	public int targetBringDownCount;

	public int createdBringDownCount;

	public int targetBringDownRemainCount;

	private bool isCollectMode;

	private bool waitContinue;

	[NonSerialized]
	public int[] countOfEachTargetCount = new int[31];

	private readonly int[] countOfEachTargetCountPreValue = new int[31];

	[HideInInspector]
	public bool IsUseBooster;

	[HideInInspector]
	public int[] UsedBoosterCountForSync;

	[HideInInspector]
	public int[] UsedBoosterCount;

	private int remainMoveCount;

	private GameFailResultReason failReason;

	public int ObsPauseTurnCount;

	public GameObject PrefabTutorialOutlineEffect;

	public GameObject PrefabTutorialGuideCursor;

	private int shuffleOccurredCount;

	public int targetOreoMilkHeight;

	public int curOreoMilkHeight = -1;

	public bool canMakeOreoCracker;

	public bool IsGettingItem;

	public int CreatedTokenCount;

	public bool outOfGame;

	private int score;

	public int StarPoint;

	public bool PauseControl;

	private VSTurn currentTurn;

	private bool dirtyChangeTurn;

	private int moveCount;

	private bool isRecvLevelFailBonus;

	public float TestValueRadius;

	public float TestValueForce;

	private bool isStarting;

	public float movingSlotTime = BoardManager.MOVING_SLOT_TIME;

	public Ease movingSlotEase = Ease.Linear;

	public static bool onFirstAction;

	public static float throwFlyingTime = 0.5f;

	private int throwingCollectItemCount;

	private readonly float throwingIntervalTime = 0.2f;

	[HideInInspector]
	public static float CrowMovingTime = 0.583f;

	public int magicalCrowHitCount;

	private readonly float MoveRailAnimationDuration = 0.4f;

	public static float waitingTimeMovingBoardByOne = 0.4f;

	private float waitingTimeOfMoveBoard = 0.2f;

	private readonly Dictionary<BoardPosition, BoardPosition> moveGeneratorPosition = new Dictionary<BoardPosition, BoardPosition>();

	private readonly float RoationBoardAnimationDuration = 0.5f;

	[HideInInspector]
	public bool canBombCandyFactory;

	[HideInInspector]
	public bool completeBombCandyFactory = true;

	[HideInInspector]
	public bool canMakeChocolateJail;

	public int chameleonColorChanged;

	private readonly Vector3 CPUCursorOffset = new Vector3(25f, -25f, 0f);

	public int shuffleOrder;

	private readonly bool[] isChecked = new bool[MapData.MaxWidth * MapData.MaxHeight];

	public bool isConnectedSweetRoad;

	public bool isConnectedOnlySweetRoad;

	private AudioSource endSound;

	public int SpewingCandyFactoryCount;

	public bool canSwap = false;

	public bool isTutorial = false;

	public readonly int[] findBoxCombinationOffsetX = new int[4]
	{
		-1,
		1,
		1,
		-1
	};

	public readonly int[] findBoxCombinationOffsetY = new int[4]
	{
		1,
		-1,
		1,
		-1
	};

	private Move bestMove;

	private readonly float showTextEffectBaseDelayTime = 0.9f;

	public int Score
	{
		get
		{
			return score;
		}
		set
		{
			score = value;
			Match3GameUI.Instance.SetTargetScore(score);
		}
	}

	public VSTurn CurrentTurn => currentTurn;

	public int MoveCount
	{
		get
		{
			return moveCount;
		}
		private set
		{
			moveCount = Mathf.Max(0, value);
			Match3GameUI.Instance.SetMoveCount(moveCount);
		}
	}

	public int ComboCount
	{
		get;
		private set;
	}

	public GameMain()
	{
		ComboCount = 0;
	}

	private Comparison<Slot> chipCollectDelayTimeSortingComparison = (x, y) => 
	{
		Vector2 p1 = x.transform.localPosition;
		Vector2 p2 = y.transform.localPosition;

		if ((p1.x - p1.y) < (p2.x - p2.y)) return -1;
		else return 1;
	};

	private void Awake()
	{
		main = this;
		UsedBoosterCountForSync = new int[5];
		UsedBoosterCount = new int[5];
		DefaultSortingLayerID = SortingLayer.NameToID("Default");
		GameEffectSortingLayerID = SortingLayer.NameToID("GameEffect");
	}

	private void OnDestroy()
	{
		DOTween.KillAll();
	}

	private void OnApplicationQuit()
	{
		//AppEventTracker.LogEventUserProperty_TimePlay(Time.realtimeSinceStartup);
	}

	public void StartProto(MapData mapData)
	{
		AppTempData.watch_ads_reward_3moves_count = 0;

		for (int i = 0; i < 35; i++)
		{
			var bonusCoin = Instantiate(bonusCoinPrefab.gameObject).GetComponent<ThrownBonusCoin>();
			bonusCoin.gameObject.SetActive(false);
			thrownBonusCoinPool.Add(bonusCoin);
		}

		var playerData = PlayerData.current;
		if (playerData.match3Data.level >= 44 && playerData.tempData.push_level_gequal44_event == false)
		{
			playerData.tempData.push_level_gequal44_event = true;
			//AppEventTracker.PushEventLevelUnlock_Gequal44();
			Firebase.Analytics.FirebaseAnalytics.LogEvent("Feature_selected_level_unlock_44");
		}

		played = true;

		MapData.main = mapData;
		MapData.main.currentBoardDataIndex = 0;
		MapData.main.currentLineDataIndex = 0;
		isRecvLevelFailBonus = false;
		AppEventManager.m_TempBox.InitGamePlay();
		BoardManager.main.StartBoard();
		main.StartSession(MapData.main.target);

		AudioManager.Instance.CrossOut(2f, true);

		Match3GameUI.Instance.Reset(0, MapData.main.moveCount, MapData.main.collectBlocks, mapData.gid);

		Match3GameUI.Instance.Hide(false);

		var popupMessageStart = Popup.PopupSystem.Instance.ShowPopup(PopupType.PopupGameMessageStart, Popup.CurrentPopupBehaviour.Close, false, false);
		popupMessageStart.PostAnimateHideEvent = OnMessageStartCompleted;

		//CPanelGameUI.Instance.ShowUITween();
		SoundManager.StopMusicImmediately();

		float delay = 0f;
		//switch (mapData.target)
		//{
		//case GoalTarget.Score:
		//	SoundSFX.Play(SFXIndex.GameStartGoalTargetNormal);
		//	delay = 3.6f;
		//	break;
		//case GoalTarget.SweetRoad:
		//	SoundSFX.Play(SFXIndex.GameStartGoalTargetSweetRoad);
		//	delay = 3.6f;
		//	break;
		//case GoalTarget.BringDown:
		//	SoundSFX.Play(SFXIndex.GameStartGoalTargetBringDown);
		//	delay = 3.6f;
		//	break;
		//case GoalTarget.RescueGingerMan:
		//	SoundSFX.Play(SFXIndex.GameStartGoalTargetGingerman);
		//	delay = 4.366f;
		//	break;
		//case GoalTarget.RescueVS:
		//	SoundSFX.Play(SFXIndex.GameStartGoalTargetVS);
		//	delay = 4.366f;
		//	break;
		//case GoalTarget.Jelly:
		//	SoundSFX.Play(SFXIndex.GameStartGoalTargetJelly);
		//	delay = 2.7f;
		//	break;
		//case GoalTarget.Digging:
		//	SoundSFX.Play(SFXIndex.GameStartGoalTargetDigging);
		//	delay = 2.7f;
		//	break;
		//}
		delay = 1.5f;
		StartCoroutine(SoundConnectDelay(delay));
		doingStartCameraTween = true;
		BoardManager.main.slotGroup.transform.DOScale(new Vector3(0.7f, 0.7f, 1f), 0.8f).SetEase(Ease.OutQuad).SetDelay(0.5f)
			.From()
			.OnComplete(delegate
			{
				doingStartCameraTween = false;
			});
        //UIGameBG.transform.DOScale(new Vector3(1.3f, 1.3f, 1f), 0.8f).SetEase(Ease.OutQuad).SetDelay(0.1f)
        //	.From();

//#if UNITY_EDITOR
//        moveCount = 2;
//#else
//		//...complile error trigger
//#endif
    }

    private IEnumerator SoundConnectDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		PlayGameBGM();
	}

	private void PlayGameBGM()
	{
		SoundManager.PlayConnection("BGM");
	}

	private void OnMessageStartCompleted()
	{
		//	MonoSingleton<AdNetworkManager>.Instance.DestoryBanner();
		ProcessAfterOnMessageStartCompleted();
		Match3GameUI.Instance.UpdateTextBoosterCount();		
	}

	public int[] headstartBooster = new int[3];

	private IEnumerator PlaceHeadStartBooster()
	{
		isBonusTime = true;

		int createIndex = 0;
		int createCount = 0;

		List<SlotForChip> target = new List<SlotForChip>();
		Chip[] allChips2 = BoardManager.main.slotGroup.GetComponentsInChildren<Chip>();

		canSwap = false;

		yield return new WaitForSeconds(0.75f);

		if (allChips2.Length > 0)
		{
			target.Clear();

			allChips2 = BoardManager.main.slotGroup.GetComponentsInChildren<Chip>();
			Chip[] array2 = allChips2;
			foreach (Chip chip2 in array2)
			{
				if (!chip2.destroying && (bool)chip2.parentSlot && (!chip2.parentSlot.slot || chip2.parentSlot.slot.canBeCrush))
				{
					if (chip2.chipType == ChipType.SimpleChip)
					{
						target.Add(chip2.parentSlot);
					}
				}
			}

			Utils.Shuffle(target);

			int targetIndex = 0;

			if (headstartBooster[1] > 0)
			{
				for (int i = 0; i < 2; i++)
				{
					int x = target[targetIndex + i].slot.x;
					int y = target[targetIndex + i].slot.y;

					SoundSFX.PlayCap(SFXIndex.Fusion);
					var powerupChip = BoardManager.main.AddPowerup(x, y, i == 0 ? Powerup.ColorHBomb : Powerup.ColorVBomb);
					target[targetIndex + i].SetChip(powerupChip);

					yield return new WaitForSeconds(0.4f);
				}

				targetIndex += 2;
			}

			if (headstartBooster[2] > 0)
			{
				for (int i = 0; i < 2; i++)
				{
					int x = target[targetIndex + i].slot.x;
					int y = target[targetIndex + i].slot.y;

					SoundSFX.PlayCap(SFXIndex.Fusion);
					var powerupChip = BoardManager.main.AddPowerup(x, y, Powerup.SimpleBomb);
					target[targetIndex + i].SetChip(powerupChip);

					yield return new WaitForSeconds(0.4f);
				}
			}
		}

		canSwap = true;
	}

	private void ProcessAfterOnMessageStartCompleted()
	{
		canSwap = false;

		Action<object> GenerateEvent = (param) =>
		{
			if (MapData.main.target == GoalTarget.Jelly)
			{
				BoardManager.main.GenerateJellyFirst();
				BoardManager.main.GenerateEmptyTile();
			}
			if (MapData.main.target == GoalTarget.SweetRoad)
			{
				isPlaying = false;
				StartCoroutine(firstDyanmicBoard(1f));
			}
			else
			{
				StartCoroutine(firstDyanmicBoard());
			}

			Match3GameUI.Instance.Show(true);

			int rewardMoveCount = 0;
			if (rewardMove3ByADStart) rewardMoveCount += 3;
			if (headstartBooster[0] > 0) rewardMoveCount += 3;

			if (rewardMoveCount > 0)
			{
				this.ExecuteAfterSeconds(0.35f, () =>
				{
					rewardMove3ByADStart = false;
					Match3GameUI.Instance.ThrowPurchasedItemEffect(SpawnStringEffectType.SuccessBuyMove, rewardMoveCount);
				});

				this.ExecuteAfterSeconds(2f, () =>
				{
					StartCoroutine(PlaceHeadStartBooster());
					tutorialController.StartWithGID(MapData.main.gid);
				});
			}
			else
			{
				StartCoroutine(PlaceHeadStartBooster());
				tutorialController.StartWithGID(MapData.main.gid);
			}
		};

		//int tutorialIndex = MonoSingleton<ServerDataTable>.Instance.GetTutorialIndex(MapData.main.gid);
		//if (tutorialIndex != -1)
		//{
		//    PopupLiteTutorial popupLiteTutorial = Popup.PopupSystem.Instance.ShowPopup(PopupType.PopupLiteTutorial) as PopupLiteTutorial;
		//    popupLiteTutorial.SetData(tutorialIndex);
		//    popupLiteTutorial.AcceptEvent = GenerateEvent;
		//}
		//else
		{
			GenerateEvent(null);
		}
	}

	private void SkipTutorial()
	{
		isPlaying = true;
		ProcessAfterOnMessageStartCompleted();
	}

	public void StartProto(int level)
	{
		MapData.main = new MapData(level);
		StartProto(MapData.main);

		//moveCount = 2;
	}

	public static void Reset()
	{
		PoolManager.PoolGameEffect.DespawnAll();
		PoolManager.PoolGameBlocks.DespawnAll();
		if ((bool)BoardManager.main && (bool)BoardManager.main.slotGroup)
		{
			Destroy(BoardManager.main.slotGroup);
		}
		main.animate = 0;
		main.gravity = 0;
		main.matching = 0;
		main.rolling = 0;
		main.isTurnResultEnd = true;
		main.eventCount = 0;
		main.lastMovementId = 0;
		main.swapEvent = 0;
		main.score = 0;
		main.firstChipGeneration = true;
		main.outOfGame = false;
		ResetBoard();
		main.isPlaying = true;
		main.isGameResult = false;
		main.MoveCount = MapData.main.moveCount;
		main.timeLeft = 0f;
		main.isBonusTime = false;
		main.currentTurn = VSTurn.Player;
		main.PauseControl = false;
		main.shuffleOccurredCount = 0;
		for (int i = 0; i < main.countOfEachTargetCount.Length; i++)
		{
			main.countOfEachTargetCount[i] = 0;
		}
		for (int j = 0; j < main.countOfEachTargetCountPreValue.Length; j++)
		{
			main.countOfEachTargetCountPreValue[j] = 0;
		}
		for (int k = 0; k < main.UsedBoosterCountForSync.Length; k++)
		{
			main.UsedBoosterCountForSync[k] = 0;
		}
		for (int l = 0; l < main.UsedBoosterCount.Length; l++)
		{
			main.UsedBoosterCount[l] = 0;
		}
		main.reachedTheTarget = false;
		main.IsUseBooster = false;


	}

	public static void ResetBoard()
	{
		main.fillingMilkEffectCount = 0;
	}

	public void OnPressButtonExit()
	{
		SoundSFX.Play(SFXIndex.ButtonClick);
	}

	private void OnEventExitYes()
	{
		BoardManager.main.RemoveTutorialOutlineEffect();
		AppEventManager.m_TempBox.didPlayerQuit = -1;
		ProcessGameQuit(IsExitRetry: false);
	}

	public void OnEventGameRetry()
	{
		AppEventManager.m_TempBox.didPlayerQuit = 1;
		ProcessGameQuit(IsExitRetry: true);
	}

	private void ProcessGameQuit(bool IsExitRetry)
	{
		Popup.PopupSystem.Instance.CloseAllPopupsImmediately();
		ForceQuitProgress(IsExitRetry);
	}

	private void ForceQuitProgress(bool IsExitRetry)
	{
		isPlaying = false;
		isGameResult = true;
		Popup.PopupSystem.Instance.CloseAllPopupsImmediately();

		StopAllCoroutines();
		BoardManager.main.RemoveBoard();
		//if (MonoSingleton<SceneControlManager>.Instance.OldSceneType == SceneType.MapTool)
		//{
		//	BackToMapTool();
		//}
		//else
		{
			//AppEventManager.m_TempBox.didClearLastPlayedLevel = 0;
			//AppEventManager.m_TempBox.sessionGameFailCount++;

			//if (IsExitRetry && MonoSingleton<SceneControlManager>.Instance.OldSceneType != SceneType.MapTool && !IsGettingItem)
			//{
			//	//MonoSingleton<AppEventManager>.Instance.SendAppEventLevelResetWithoutUsingMove(MapData.main.gid);
			//}

			//if (IsExitRetry)
			//{
			//	MonoSingleton<PopupManager>.Instance.CloseAllPopup();
			//	MonoSingleton<SceneControlManager>.Instance.RemoveCurrentScene();
			//	MonoSingleton<SceneControlManager>.Instance.LoadScene(SceneType.Game, SceneChangeEffect.Color);
			//}
			//else if (MonoSingleton<SceneControlManager>.Instance.OldSceneType != SceneType.MapTool)
			//{
			//	GotoGameToLobby(isWin: false);				
			//}
		}
		//MonoSingleton<AdNetworkManager>.Instance.ShowInterstitialAdOrRewardAd();
	}

	public void IncreaseMoveCount(int count)
	{
		MoveCount += count;
	}

	public void TurnStart()
	{
		StartCombo();
		for (int i = 0; i < BoardManager.main.boardData.listMovingSlot.Count; i++)
		{
			BoardManager.main.boardData.listMovingSlot[i].TurnStartGamePlay();
		}
		for (int j = 0; j < BoardManager.main.boardData.listRotationSlot.Count; j++)
		{
			BoardManager.main.boardData.listRotationSlot[j].TurnStartGamePlay();
		}
		BoardManager.main.RemoveTutorialOutlineEffect();
	}

	public void TurnEnd(bool passToBossTurn = true)
	{
		SetBestMovesNull();
		isTurnResultEnd = false;
		MoveCount--;
		AppEventManager.m_TempBox.UsedMoveCount++;
		if (passToBossTurn)
		{
		}
		StartCoroutine(waitPlayResult());
	}

	public void TurnEndAfterUsingBooster()
	{
		SetBestMovesNull();
		BoardManager.main.RemoveTutorialOutlineEffect();
		StartCoroutine(waitPlayResultAfterUsingBooster());
	}

	private IEnumerator waitPlayResultAfterUsingBooster(bool firstMove = false)
	{
		if (!isGameResult && !isConnectedSweetRoad)
		{
			isTurnResultEnd = false;
			yield return null;
			yield return StartCoroutine(Utils.WaitFor(CanIWait, 0.1f));
			yield return StartCoroutine(waitDynamicBoardCommon(firstMove));
		}

		isTurnResultEnd = true;
		EventCounter();
		TurnStart();
	}

	private IEnumerator waitPlayResult()
	{
		yield return null;
		yield return StartCoroutine(Utils.WaitFor(CanIWait, 0.1f));
		if (!isGameResult && !isConnectedSweetRoad && !IsGameSuccess(isPreValueCheck: true))
		{
			yield return StartCoroutine(waitDynamicBoard());
			if (!isGameResult && MoveCount == 5)
			{
				yield return StartCoroutine(ShowTextEffect(SpawnStringEffectType.TextEffect5Move));
			}
		}

		isTurnResultEnd = true;
	}

	private IEnumerator waitConnectedSweetRoad()
	{
		if (MapData.main.target == GoalTarget.SweetRoad)
		{
			yield return StartCoroutine(Utils.WaitFor(CanIWait, 0.1f));
			while (!CheckRemainSlotHaveToFillMilk() || !IsCompletedThrowCollectItem() || !completeBombCandyFactory)
			{
				yield return null;
			}
			if (isConnectedSweetRoad)
			{
				yield return StartCoroutine(BoardManager.main.GoNextMapOrClear());
				isConnectedOnlySweetRoad = false;
				isConnectedSweetRoad = false;
			}
		}
	}

	public bool CheckDiggingCondition()
	{
		bool result = false;
		int num = BoardManager.main.SearchHighestDigBlock();
		if (num < 2)
		{
			movingSlotTime = BoardManager.MOVING_SLOT_TIME * 0.3f;
			movingSlotEase = Ease.Linear;
		}
		else
		{
			movingSlotTime = BoardManager.MOVING_SLOT_TIME;
			movingSlotEase = Ease.OutBounce;
		}
		if (num < 3 && !BoardManager.main.DigCheckLineDataIndexEnd())
		{
			result = true;
		}
		return result;
	}

	private IEnumerator firstDyanmicBoard(float delayTime = 0f)
	{
		isFirstBoardSetting = true;
		yield return null;
		yield return StartCoroutine(waitDynamicBoard(firstMove: true, delayTime));
		//if (MonoSingleton<PlayerDataManager>.Instance.lastLevelStreakFailCount == 10)
		//{
		//	yield return StartCoroutine(main.playWithBuffItems(6, 1));
		//}
		//MonoSingleton<PlayerDataManager>.Instance.SaveBoosterData();
		isTurnResultEnd = true;

		isFirstBoardSetting = false;
	}

	public IEnumerator playWithBuffItems(int buffID, int rewardCount)
	{
		yield return StartCoroutine(Utils.WaitFor(CanIWait, 0.1f));
		SimpleChip[] chips = BoardManager.main.slotGroup.GetComponentsInChildren<SimpleChip>();
		if (chips.Length == 0)
		{
			yield break;
		}
		Powerup p;
		switch (buffID)
		{
			default:
				yield break;
			case 2:
				p = Powerup.CandyChip;
				break;
			case 3:
				p = Powerup.ColorHBomb;
				break;
			case 4:
				p = Powerup.ColorVBomb;
				break;
			case 5:
				p = Powerup.SimpleBomb;
				break;
			case 6:
				p = Powerup.RainbowBomb;
				break;
			case 12:
				Match3GameUI.Instance.ThrowPurchasedItemEffect(SpawnStringEffectType.SuccessBuyMove, rewardCount);
				yield break;
		}
		SimpleChip chip = null;
		while (chip == null)
		{
			chip = chips[UnityEngine.Random.Range(0, chips.Length - 1)];
			if (chip != null && chip.parentSlot.slot.GetBlock() != null && MapData.IsBlockTypeIncludingChip(chip.parentSlot.slot.GetBlock().blockType))
			{
				chip = null;
			}
		}
		SlotForChip slot = chip.parentSlot;
		if (!(slot == null))
		{
			BoardManager.main.AddPowerup(slot.slot.x, slot.slot.y, p);
			SoundSFX.Play(SFXIndex.MakeDailyRewardBuffItem);
			GameObject objEffect = ContentAssistant.main.GetItem("Eff_DB_booster");
			if ((bool)objEffect)
			{
				objEffect.transform.position = BoardManager.main.GetSlotPosition(slot.slot.x, slot.slot.y);
				UnityEngine.Object.Destroy(objEffect, 2f);
			}
			yield return new WaitForSeconds(1f);
		}
	}

	private IEnumerator waitDynamicBoardCommon(bool moveBoardForce = false)
	{
		while (!completeBombCandyFactory)
		{
			yield return null;
		}
		if (isConnectedSweetRoad || isGameResult)
		{
			yield break;
		}
		if (!moveBoardForce && BoardManager.main.listRailStartSlot.Count > 0)
		{
			yield return StartCoroutine(DoMoveRailBoard());
		}
		if (BoardManager.main.boardData.listMovingSlot.Count > 0)
		{
			if (moveBoardForce && onFirstAction)
			{
				Coroutine coroutine = null;
				for (int i = 0; i < BoardManager.main.boardData.listMovingSlot.Count; i++)
				{
					MapDataMovingSlot movingSlot = BoardManager.main.boardData.listMovingSlot[i];
					coroutine = StartCoroutine(DoScaleMovingBoard(movingSlot));
				}
				yield return coroutine;
			}
			else
			{
				yield return StartCoroutine(DoMoveBoard(moveBoardForce));
			}
		}
		if (BoardManager.main.boardData.listRotationSlot.Count > 0)
		{
			yield return StartCoroutine(DoRotationBoard(moveBoardForce));
		}
		if (!moveBoardForce && BoardManager.main.listCrowBlock.Count > 0)
		{
			yield return StartCoroutine(DoMoveCrow());
		}
		SetBestMovesNull();
	}

	private IEnumerator waitDynamicBoard(bool firstMove = false, float delayTime = 0f)
	{
		while (!Slot.isFirstJellyComplete)
		{
			yield return null;
		}
		if (delayTime > 0f)
		{
			yield return new WaitForSeconds(delayTime);
		}
		isTurnResultEnd = false;
		if (!isGameResult)
		{
			if (ObsPauseTurnCount == 0)
			{
				yield return StartCoroutine(waitDynamicBoardCommon(firstMove));
				if (BoardManager.main.listPastryBag.Count > 0)
				{
					yield return StartCoroutine(DoShotPastryBag());
				}
				if (BoardManager.main.listSlime.Count > 0)
				{
					yield return StartCoroutine(DoBreedingSlime());
				}
			}
			else
			{
				ObsPauseTurnCount--;
			}
		}
		EventCounter();
	}

	public IEnumerator DoMatchEndProcess()
	{
		yield return StartCoroutine(Utils.WaitFor(CanIWait, 0.1f));
		if (BoardManager.main.listCandyFactory.Count > 0)
		{
			yield return StartCoroutine(DoBombCandyFactory());
		}
		yield return StartCoroutine(Utils.WaitFor(main.CanISpewCandyFactory, 0.2f));
		if (MapData.main.target == GoalTarget.Digging && CheckDiggingCondition())
		{
			yield return StartCoroutine(BoardManager.main.ProcessDigMoveNextLine());
		}
	}

	public void Continue()
	{
		waitContinue = false;
		isPlaying = true;
		isGameResult = false;
	}

	public void DecreaseCollectCandy(Vector3 startPos, int id, float delayTime)
	{
		if (countOfEachTargetCountPreValue[id] > 0)
		{
			PrevThrowCollectItem((CollectBlockType)id);
			GameObject spawnEffectObject = SpawnStringEffect.GetSpawnEffectObject((SpawnStringEffectType)(97 + id));
			spawnEffectObject.transform.position = startPos;
			ThrowCollectItem(spawnEffectObject, (CollectBlockType)id, delayTime, PoolManager.PoolGameEffect);
		}
		else
		{
		}
	}

	public bool IdNeedCollected(int id)
	{
		return id < countOfEachTargetCountPreValue.Length && countOfEachTargetCountPreValue[id] > 0;
	}

	public void DecreaseCollectMakeSpecialBlock(CollectBlockType collectBlockType, Vector3 startPos, int id)
	{
		if (countOfEachTargetCountPreValue[(int)collectBlockType] != 0)
		{
			GameObject gameObject = null;
			switch (collectBlockType)
			{
				case CollectBlockType.MakeCandyChip:
					gameObject = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.CollectMakeCandyBlock);
					break;
				case CollectBlockType.MakeRainbowChip:
					gameObject = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.CollectMakeRainbowBlock);
					break;
				case CollectBlockType.MakeSimpleBomb:
				case CollectBlockType.MakeHBomb:
				case CollectBlockType.MakeVBomb:
					gameObject = SpawnStringEffect.GetSpawnEffectObject((SpawnStringEffectType)(91 + id));
					break;
			}
			PrevThrowCollectItem(collectBlockType);
			if ((bool)gameObject)
			{
				gameObject.transform.localScale = Vector3.one;
				gameObject.transform.position = startPos;
				ThrowCollectItem(gameObject, collectBlockType, 0f, PoolManager.PoolGameEffect);
			}
		}
	}

	public void DecreaseCollect(CollectBlockType collectBlockType, bool countPrevValue = false)
	{
		if (countPrevValue)
		{
			PrevThrowCollectItem(collectBlockType);
		}

		//if (collectBlockType == CollectBlockType.RescueFriend)
		//{
		//	BoardManager.main.remainRescueFriendInBoard--;
		//}

		if (countOfEachTargetCount[(int)collectBlockType] > 0)
		{
			countOfEachTargetCount[(int)collectBlockType]--;
			main.Score += MonoSingleton<ScoreManager>.Instance.GetScoreUnit(ScoreType.CollectItem);

			//if (collectBlockType == CollectBlockType.RescueFriend && MonoSingleton<SceneControlManager>.Instance.OldSceneType != SceneType.MapTool)
			//{
			//	CPanelGameUI.Instance.UpdateCollect(collectBlockType, BoardManager.main.remainRescueFriendInBoard);
			//}
			//else
			{
				Match3GameUI.Instance.UpdateCollect(collectBlockType, countOfEachTargetCount[(int)collectBlockType]);
			}
		}
		//else if (collectBlockType == CollectBlockType.SweetRoadConnect)
		//{
		//	CPanelGameUI.Instance.UpdateCollect(CollectBlockType.SweetRoadConnect, -1);
		//}
	}

	public void PrevThrowCollectItem(CollectBlockType collectBlockType)
	{
		if (countOfEachTargetCountPreValue[(int)collectBlockType] > 0)
		{
			countOfEachTargetCountPreValue[(int)collectBlockType]--;
		}
		if (collectBlockType == CollectBlockType.ChocolateJail)
		{
			canMakeChocolateJail = false;
		}
	}

	public float CalculateThrowingTime(Vector3 startPos, Vector3 targetPos)
	{
		float num = 3f;
		float num2 = 0f;
		float f = (targetPos - startPos).magnitude / BoardManager.slotoffset;
		return throwFlyingTime * Mathf.Pow(f, 1f / num) + num2;
	}

	public void ThrowCollectItem(GameObject objThrowItem, CollectBlockType collectBlockType, float startDelay = 0f, SpawnPool pool = null)
	{
		//Debug.Log("Collect " + Time.realtimeSinceStartup);

		throwingCollectItemCount++;

		Vector3 position = objThrowItem.transform.position;
		Vector3 collectObjectGameCameraPosition = Match3GameUI.Instance.GetCollectObjectGameCameraPosition(collectBlockType);
		float duration = ((int)collectBlockType <= 4) ? 0.7f : CalculateThrowingTime(position, collectObjectGameCameraPosition);

		if (countOfEachTargetCount[(int)collectBlockType] == 0 && collectBlockType != CollectBlockType.SweetRoadConnect)
		{
			StartCoroutine(WaitCompleteThrowCollectItem(duration, objThrowItem, collectBlockType, pool, isCollectItem: false));
			return;
		}

		StartCoroutine(ResetThrowingTerm());

		//Vector3 localScale = objThrowItem.transform.localScale;

		//SpriteRenderer[] componentsInChildren = objThrowItem.GetComponentsInChildren<SpriteRenderer>();
		//foreach (SpriteRenderer spriteRenderer in componentsInChildren)
		//{
		//	spriteRenderer.color = Color.white;
		//}
		if (collectBlockType != CollectBlockType.PocketCandy)
		{
			SoundSFX.PlayCapDelay(this, duration, SFXIndex.CollectGet);
		}

		var throwComp = objThrowItem.GetComponent<ThrowCollectItem>();
		if (throwComp)
		{
			throwComp.SetupAnimation(position, collectObjectGameCameraPosition, duration, startDelay);
		}
		else
		{
			objThrowItem.transform.DOMove(collectObjectGameCameraPosition, duration).SetEase(Ease.InOutCubic).SetDelay(startDelay);
			Debug.LogWarning("Missing throw script " + objThrowItem.name);
		}


		//SpriteRenderer[] componentsInChildren2 = objThrowItem.GetComponentsInChildren<SpriteRenderer>();
		//foreach (SpriteRenderer target in componentsInChildren2)
		//{
		//	target.DOFade(0f, 0.1f).SetDelay(num + num2 - num3);
		//}
		//if (collectBlockType == CollectBlockType.Crow || collectBlockType == CollectBlockType.MagicalCrow)
		//{
		//	StartCoroutine(Utils.LookAtTarget(objThrowItem, num));
		//}
		//if (collectBlockType == CollectBlockType.RescueGingerMan)
		//{
		//	objThrowItem.transform.DOScale(localScale * 1.18f, num / 2f).SetDelay(startDelay);
		//	objThrowItem.transform.DOScale(0.1f, num / 2f).SetDelay(num / 2f + startDelay);
		//}
		//else if (collectBlockType >= CollectBlockType.RescueGingerMan)
		//{
		//	objThrowItem.transform.DOScale(localScale * 1.18f, num / 2f).SetDelay(startDelay);
		//	objThrowItem.transform.DOScale(0.5f, num / 2f).SetDelay(num / 2f + startDelay);
		//}
		StartCoroutine(WaitCompleteThrowCollectItem(duration + startDelay, objThrowItem, collectBlockType, pool));
	}

	private IEnumerator ResetThrowingTerm()
	{
		yield return null;
	}

	private IEnumerator WaitCompleteThrowCollectItem(float duration, GameObject destroyItem, CollectBlockType collectBlockType, SpawnPool pool, bool isCollectItem = true)
	{
		yield return new WaitForSeconds(duration);
		throwingCollectItemCount--;
		RemoveThrowCollectItem(destroyItem, pool);
		if (isCollectItem)
		{
			DecreaseCollect(collectBlockType);
		}
	}

	private void RemoveThrowCollectItem(GameObject destroyItem, SpawnPool pool)
	{
		if (pool == null)
		{
			UnityEngine.Object.Destroy(destroyItem);
			return;
		}
		destroyItem.transform.localScale = Vector3.one;
		//SpriteRenderer[] componentsInChildren = destroyItem.GetComponentsInChildren<SpriteRenderer>();
		//foreach (SpriteRenderer spriteRenderer in componentsInChildren)
		//{
		//	spriteRenderer.color = Color.white;
		//}
		pool.Despawn(destroyItem.transform);
	}

	public void StartSession(GoalTarget sessionType)
	{
		StopAllCoroutines();
		if (MapData.main.target != GoalTarget.Jelly)
		{
			Slot.isFirstJellyComplete = true;
		}
		StartCoroutine(GamePlaySession());
		if (sessionType == GoalTarget.SweetRoad)
		{
			StartCoroutine(SweetRoadRoutine());
		}
		StartCoroutine(ShowingHintRoutine());

		StartCoroutine(FindingSolutionsRoutine());

		SetBestMovesNull();
		MonoSingleton<AnimationController>.Instance.ProcessAnimation();

		//moveCount = 2;
	}

	private int currentBonusCoin;

	public ThrownBonusCoin bonusCoinPrefab;

	public void AddBonusCoin(int add)
	{
		currentBonusCoin += add;
		Match3GameUI.Instance.progressInfoView.UpdateBonusCoin(currentBonusCoin, true);
	}

	public void ResultGameWin()
	{
		if (!isGameResult)
		{
			MonoSingleton<UIManager>.Instance.CancelBooster();
			isPlaying = false;
			isGameResult = true;

			currentBonusCoin = 300;

			DoStartSweetFestival();
		}
	}

	public void ResultGameLose(GameFailResultReason reason)
	{
		if (!isGameResult)
		{
			MonoSingleton<UIManager>.Instance.CancelBooster();
			//SoundSFX.Play(SFXIndex.GameFailPopup);
			isPlaying = false;
			isGameResult = true;
			PopupFiveMoveMore(reason);
		}
	}

	private void DoGameResultProcess(bool isSuccess, GameFailResultReason reason)
	{
		outOfGame = true;

		if (isSuccess)
		{
			OpenClearPopup();
		}
		else
		{
			ProcessGameLose();
		}
	}

	private void OpenClearPopup()
	{
		//AppEventTracker.LogEventLevelStatus(PlayerData.current.match3Data.level,
		//	Analytics.Feature_LEVEL_STATUS.ACTION_TYPE._end,
		//	Analytics.Feature_LEVEL_STATUS.STATUS_PLAY._normal,
		//	Analytics.Feature_LEVEL_STATUS.RESULT._win);

		GlobalEventObserver.InvokeFinishLevelEvent(true, remainMoveCount);

		// WIN GAME
		PlayerData playerData = PlayerData.current;
		playerData.match3Data.level++;

		//AppEventTracker.LogEventUserProperty_UnlockLevel(playerData.match3Data.level);

		if (playerData.match3Data.level >= 44 && playerData.tempData.push_level_gequal44_event == false)
		{
			playerData.tempData.push_level_gequal44_event = true;
			//AppEventTracker.PushEventLevelUnlock_Gequal44();
			Firebase.Analytics.FirebaseAnalytics.LogEvent("Feature_selected_level_unlock_44");

		}

		SoundManager.Instance.offTheSFX = true;

		SoundManager.StopMusic();
		// preload next level
		MapData.main = new MapData(PlayerData.current.match3Data.level);

		Match3GameUI.Instance.OnFestivalEnd();
		gameCameraTargetDelegate.GetMaterial().DOVector(new Vector4(1f, 1f, 1f, 0f), "_Color", 0.5f).
			OnComplete(ShowPopupPiggyBank);
	}

	private void ShowPopupPiggyBank()
    {
		if (PiggyBankUtility.Available())
        {
			AudioManager.Instance.PlaySFX(AudioClipId.PiggyBankCollect);

			Popup.PopupSystem.GetOpenBuilder().
						SetType(PopupType.PopupPiggyBankCollect).
						SetCurrentPopupBehaviour(Popup.CurrentPopupBehaviour.Close).
						Open().
						PostAnimateHideEvent = ShowPopupWin;
		}
        else
        {
			ShowPopupWin();
		}
	}

	private void ShowPopupWin()
    {
		//SoundManager.CrossOut(0.5f, SoundManager.Instance.audios[0]);
		AudioManager.Instance.PlaySFX(AudioClipId.GameWin);
		PopupGameWin popupGameWin = Popup.PopupSystem.Instance.ShowPopup(PopupType.PopupGameWin, Popup.CurrentPopupBehaviour.Close, false) as PopupGameWin;
		Popup.PopupSystem.Instance.SetBackBlockerPressedEventOfCurrentPopup(popupGameWin.OnPressContinue);
		popupGameWin.SetBonusCoin(currentBonusCoin);
		popupGameWin.CloseEvent = () =>
		{
			SoundManager.StopSFX();
			SoundManager.Instance.offTheSFX = false;
		};
	}

	private void PopupFiveMoveMore(GameFailResultReason reason)
	{
		// LOSE GAME		

		failReason = reason;

		SoundManager.StopMusic();

		//SoundManager.CrossOut(0.5f, SoundManager.Instance.audios[0]);

		AudioManager.Instance.PlaySFX(AudioClipId.GameFail);

		var popupGameLose = Popup.PopupSystem.Instance.ShowPopup(PopupType.PopupGameLose, Popup.CurrentPopupBehaviour.Close, false, true);
		popupGameLose.AcceptEvent = (param) =>
		{
			int bonusMoveCount = (int)param;
			Match3GameUI.Instance.ThrowPurchasedItemEffect(SpawnStringEffectType.SuccessBuyMove, bonusMoveCount);
		};
		popupGameLose.DenyEvent = (param) =>
		{
			PlayerData playerData = PlayerData.current;
			playerData.AddStamina(-1);

			playerData.tempData.loseLevelCount++;

            if (playerData.tempData.loseLevelCount == 25)
            {
				//AppEventTracker.PushEventLoseLevelCount_Gequa25();
				Firebase.Analytics.FirebaseAnalytics.LogEvent("feature_selecterd_level_fail_01");
			}

            //AppEventTracker.LogEventLevelStatus(PlayerData.current.match3Data.level,
            //	Analytics.Feature_LEVEL_STATUS.ACTION_TYPE._end,
            //	Analytics.Feature_LEVEL_STATUS.STATUS_PLAY._normal,
            //	Analytics.Feature_LEVEL_STATUS.RESULT._lose);

            GlobalEventObserver.InvokeFinishLevelEvent(false, 0);

			var popupGameRetry = Popup.PopupSystem.Instance.ShowPopup(PopupType.PopupGameStart, Popup.CurrentPopupBehaviour.Close, false, true) as PopupGameStart;
		};
	}

	private void ProcessGameLose()
	{
		int num = 0;
		for (int i = 0; i < main.UsedBoosterCount.Length; i++)
		{
			num += main.UsedBoosterCount[i];
		}
		OpenFailPopup();
	}

	private void OpenFailPopup()
	{
		//if (MonoSingleton<SceneControlManager>.Instance.OldSceneType == SceneType.MapTool)
		//{
		//	BackToMapTool();
		//	return;
		//}
		//MonoSingleton<SceneControlManager>.Instance.LoadScene(SceneType.Lobby, SceneChangeEffect.Color);
		//MonoSingleton<AdNetworkManager>.Instance.ShowInterstitialAdOrRewardAd();
	}

	private Coroutine festivalCoroutine;

	private void DoStartSweetFestival()
	{
		//SoundSFX.Play(SFXIndex.SweetFestivalFireworks);

		festivalCoroutine = StartCoroutine(ProcessSweetFestival());
	}

	private IEnumerator DoMoveCrow()
	{
		magicalCrowHitCount = 0;
		yield return StartCoroutine(Utils.WaitFor(CanIWait, 0.1f));
		if (!isGameResult)
		{
			animate++;
			isPlaying = false;
			isLockDrop = true;
			foreach (Crow item in BoardManager.main.listCrowBlock)
			{
				if ((bool)item)
				{
					item.MoveCrow();
				}
			}
			yield return new WaitForSeconds(CrowMovingTime + 0.1f);
			if (magicalCrowHitCount > 0)
			{
				yield return new WaitForSeconds(0.4f);
			}
			SlotGravity.Reshading();
			animate--;
			isPlaying = true;
			isLockDrop = false;
			yield return StartCoroutine(Utils.WaitFor(CanIWait, 0.1f));
		}
	}

	private IEnumerator DoMoveRailBoard()
	{
		yield return StartCoroutine(Utils.WaitFor(CanIWait, 0.1f));
		animate++;
		isPlaying = false;
		isLockDrop = true;
		foreach (Slot item in BoardManager.main.listRailStartSlot)
		{
			Slot slot = item;
			Slot railNextSlot = item.railNextSlot;
			Chip bakChip = slot.GetChip();
			BlockInterface bakBlock = slot.GetBlock();
			bool bakGravity = slot.gravity;
			do
			{
				Chip c = bakChip;
				BlockInterface b = bakBlock;
				bool g = bakGravity;
				bakChip = railNextSlot.GetChip();
				bakBlock = railNextSlot.GetBlock();
				bakGravity = railNextSlot.gravity;
				if ((bool)b)
				{
					b.slot = railNextSlot;
				}
				railNextSlot.SetChip(c);
				railNextSlot.SetBlock(b);
				railNextSlot.gravity = g;
				if (Mathf.Abs(slot.x - railNextSlot.x) + Mathf.Abs(slot.y - railNextSlot.y) > 1)
				{
					int num = BoardManager.main.boardData.railImage[railNextSlot.x, railNextSlot.y];
					int num2 = 0;
					int num3 = 0;
					switch (num)
					{
						case 1:
						case 6:
						case 11:
							num2 = -1;
							break;
						case 2:
						case 8:
						case 9:
							num2 = 1;
							break;
					}
					switch (num)
					{
						case 3:
						case 7:
						case 10:
							num3 = 1;
							break;
						case 4:
						case 5:
						case 12:
							num3 = -1;
							break;
					}
					Vector3 slotPosition = BoardManager.main.GetSlotPosition(railNextSlot.x + num2, railNextSlot.y + num3);
					if ((bool)c)
					{
						c.transform.position = slotPosition;
					}
					if ((bool)b)
					{
						b.transform.position = slotPosition;
					}
				}
				if ((bool)c)
				{
					c.transform.DOMove(railNextSlot.transform.position, MoveRailAnimationDuration);
				}
				if ((bool)b)
				{
					b.transform.DOMove(railNextSlot.transform.position, MoveRailAnimationDuration);
				}
				slot = railNextSlot;
				railNextSlot = slot.railNextSlot;
			}
			while (slot != item);
		}
		yield return new WaitForSeconds(MoveRailAnimationDuration);
		SlotGravity.Reshading();
		animate--;
		isPlaying = true;
		isLockDrop = false;
		yield return StartCoroutine(Utils.WaitFor(CanIWait, 0.1f));
	}

	private IEnumerator DoMoveBoard(bool firstMove = false)
	{
		yield return StartCoroutine(Utils.WaitFor(CanIWait, 0.1f));
		if (isGameResult)
		{
			yield break;
		}
		animate++;
		isPlaying = false;
		isLockDrop = true;
		moveGeneratorPosition.Clear();
		for (int i = 0; i < BoardManager.main.boardData.listMovingSlot.Count; i++)
		{
			MapDataMovingSlot mapDataMovingSlot = BoardManager.main.boardData.listMovingSlot[i];
			if (mapDataMovingSlot.IsTriggerOnGamePlay() || firstMove)
			{
				new List<FromToCoordinate>();
				SoundSFX.Play(SFXIndex.GameBoardMove);
				List<FromToCoordinate> movingPathEachSlot = GetMovingPathEachSlot(mapDataMovingSlot);
				foreach (FromToCoordinate item in movingPathEachSlot)
				{
					FromToCoordinate current = item;
					waitingTimeOfMoveBoard = BoardManager.main.MoveSlot(current.FromX, current.FromY, current.ToX, current.ToY, waitingTimeMovingBoardByOne);
					moveGeneratorPosition.Add(new BoardPosition(current.FromX, current.FromY), new BoardPosition(current.ToX, current.ToY));
				}
			}
		}
		if (moveGeneratorPosition.Count > 0)
		{
			BoardManager.main.OffBoardOutline();
			yield return new WaitForSeconds(waitingTimeOfMoveBoard);
			BoardManager.main.DrawBoardOutline();
			BoardManager.main.FadeBoardOutline(isFadeIn: true);
			for (int j = 0; j < BoardManager.main.boardData.listMovingSlot.Count; j++)
			{
				if (BoardManager.main.boardData.listMovingSlot[j].IsTriggerOnGamePlay() || firstMove)
				{
					BoardManager.main.boardData.listMovingSlot[j].MoveEndGamePlay();
					BoardManager.main.DrawMovingSlotOutlineEffect(BoardManager.main.boardData.listMovingSlot[j]);
				}
			}
			foreach (BoardPosition key2 in moveGeneratorPosition.Keys)
			{
				BoardPosition current2 = key2;
				BoardPosition key = moveGeneratorPosition[current2];
				if (BoardManager.main.bringDownGenerator[current2.x, current2.y])
				{
					BoardManager.main.bringDownGenerator[key.x, key.y] = true;
					BoardManager.main.bringDownGenerator[current2.x, current2.y] = false;
				}
				if (BoardManager.main.dicGeneratorDrop.ContainsKey(current2) && !BoardManager.main.dicGeneratorDrop.ContainsKey(key))
				{
					BoardManager.main.dicGeneratorDrop.Add(key, BoardManager.main.dicGeneratorDrop[current2]);
					BoardManager.main.dicGeneratorDrop.Remove(current2);
				}
				if (BoardManager.main.dicGeneratorSpecialDrop.ContainsKey(current2) && !BoardManager.main.dicGeneratorSpecialDrop.ContainsKey(key))
				{
					BoardManager.main.dicGeneratorSpecialDrop.Add(key, BoardManager.main.dicGeneratorSpecialDrop[current2]);
					BoardManager.main.dicGeneratorSpecialDrop.Remove(current2);
				}
			}
			BoardManager.main.RefreshNearSlots();
			if (countOfEachTargetCount[6] > 0)
			{
				for (int k = 0; k < BoardManager.main.boardData.width; k++)
				{
					for (int l = 0; l < BoardManager.main.boardData.height; l++)
					{
						CheckRescueGingerMan(k, l);
					}
				}
			}
		}
		animate--;
		isPlaying = true;
		isLockDrop = false;
		yield return StartCoroutine(Utils.WaitFor(CanIWait, 0.1f));
	}

	private List<FromToCoordinate> GetMovingPathEachSlot(MapDataMovingSlot movingSlot)
	{
		FromToCoordinate fromToCoordinate = default(FromToCoordinate);
		List<FromToCoordinate> list = new List<FromToCoordinate>();
		list.Clear();
		if (movingSlot.isMoveReverse)
		{
			fromToCoordinate.FromX = movingSlot.startX;
			fromToCoordinate.FromY = movingSlot.startY;
			fromToCoordinate.ToX = movingSlot.targetX;
			fromToCoordinate.ToY = movingSlot.targetY;
		}
		else
		{
			fromToCoordinate.FromX = movingSlot.targetX;
			fromToCoordinate.FromY = movingSlot.targetY;
			fromToCoordinate.ToX = movingSlot.startX;
			fromToCoordinate.ToY = movingSlot.startY;
		}
		int num = fromToCoordinate.FromX - fromToCoordinate.ToX;
		int num2 = fromToCoordinate.FromY - fromToCoordinate.ToY;
		for (int i = 0; i < movingSlot.width; i++)
		{
			for (int j = 0; j < movingSlot.height; j++)
			{
				FromToCoordinate item = default(FromToCoordinate);
				if (num > 0)
				{
					item.FromX = fromToCoordinate.ToX + movingSlot.width - 1 - i;
					item.ToX = fromToCoordinate.ToX + movingSlot.width - 1 - i + num;
				}
				else
				{
					item.FromX = fromToCoordinate.ToX + i;
					item.ToX = fromToCoordinate.ToX + i + num;
				}
				if (num2 < 0)
				{
					item.FromY = fromToCoordinate.ToY - movingSlot.height + 1 + j;
					item.ToY = fromToCoordinate.ToY - movingSlot.height + 1 + j + num2;
				}
				else
				{
					item.FromY = fromToCoordinate.ToY - j;
					item.ToY = fromToCoordinate.ToY - j + num2;
				}
				list.Add(item);
			}
		}
		return list;
	}

	private IEnumerator DoScaleMovingBoard(MapDataMovingSlot movingSlot)
	{
		GameObject objCenter = new GameObject("scale_center");
		objCenter.transform.SetParent(BoardManager.main.slotGroup.transform);
		Vector3 centerPosition = BoardManager.main.GetSlotPosition(movingSlot.startX, movingSlot.startY);
		objCenter.transform.position = centerPosition;
		new List<FromToCoordinate>();
		List<FromToCoordinate> eachSlotPath = GetMovingPathEachSlot(movingSlot);
		foreach (FromToCoordinate item in eachSlotPath)
		{
			FromToCoordinate current = item;
			BoardManager.main.GetSlotFromSlotPosition(current.FromX, current.FromY).transform.SetParent(objCenter.transform);
		}
		yield return BoardManager.main.TweenScaleSlot(objCenter);
		foreach (FromToCoordinate item2 in eachSlotPath)
		{
			FromToCoordinate current2 = item2;
			BoardManager.main.GetSlotFromSlotPosition(current2.FromX, current2.FromY).transform.parent = BoardManager.main.slotGroup.transform;
		}
		UnityEngine.Object.Destroy(objCenter);
	}

	private IEnumerator DoRotationBoard(bool firstMove = false)
	{
		yield return StartCoroutine(Utils.WaitFor(CanIWait, 0.1f));
		if (!isGameResult)
		{
			for (int i = 0; i < BoardManager.main.boardData.listRotationSlot.Count; i++)
			{
				if (firstMove && onFirstAction)
				{
					StartCoroutine(DoScaleRotationBoard(BoardManager.main.boardData.listRotationSlot[i]));
				}
				else
				{
					StartCoroutine(DoRotationBoardEachBoard(BoardManager.main.boardData.listRotationSlot[i], firstMove));
				}
			}
		}
		yield return StartCoroutine(Utils.WaitFor(CanIWait, 0.1f));
	}

	private IEnumerator DoScaleRotationBoard(MapDataRotationSlot rotationSlot)
	{
		GameObject objCenter = new GameObject("scale_center");
		objCenter.transform.SetParent(BoardManager.main.slotGroup.transform);
		Vector3 centerPosition = BoardManager.main.GetSlotPosition(rotationSlot.centerX, rotationSlot.centerY);
		objCenter.transform.position = centerPosition;
		Dictionary<int, Slot> curSlots = BoardManager.main.slots;
		Dictionary<int, Slot> replacedSlots = new Dictionary<int, Slot>();
		int startX = rotationSlot.centerX - rotationSlot.size / 2;
		int startY = rotationSlot.centerY + rotationSlot.size / 2 - (rotationSlot.isGrid ? 1 : 0);
		int w = rotationSlot.size;
		int h = rotationSlot.size;
		for (int i = 0; i < h; i++)
		{
			for (int j = 0; j < w; j++)
			{
				int index = (startX + (w - j - 1)) * MapData.MaxHeight + (startY - i);
				if (curSlots.ContainsKey(index))
				{
					Slot s = curSlots[index];
					if ((bool)s)
					{
						replacedSlots.Add(index, s);
						s.transform.SetParent(objCenter.transform);
					}
				}
			}
		}
		yield return BoardManager.main.TweenScaleSlot(objCenter);
		foreach (Slot value in replacedSlots.Values)
		{
			value.transform.parent = BoardManager.main.slotGroup.transform;
		}
		UnityEngine.Object.Destroy(objCenter);
	}

	public IEnumerator DoBombCandyFactory()
	{
		canBombCandyFactory = true;
		completeBombCandyFactory = false;
		while (!completeBombCandyFactory)
		{
			for (int i = 0; i < BoardManager.main.listCandyFactory.Count; i++)
			{
				if (!BoardManager.main.listCandyFactory[i].completeBombCandyFactory)
				{
					i = 0;
					yield return null;
				}
				while (!CanIWait())
				{
					yield return null;
				}
			}
			completeBombCandyFactory = true;
		}
		canBombCandyFactory = false;
	}

	private IEnumerator DoShotPastryBag()
	{
		yield return StartCoroutine(Utils.WaitFor(CanIWait, 0.1f));
		yield return StartCoroutine(Utils.WaitFor(CanIGravity, 0.1f));
		Coroutine coroutine = null;
		foreach (PastryBag pb in BoardManager.main.listPastryBag)
		{
			if (canMakeChocolateJail)
			{
				coroutine = StartCoroutine(pb.ShotChocolate());
			}
			yield return null;
		}
		if (coroutine != null)
		{
			yield return coroutine;
		}
		canMakeChocolateJail = true;
	}

	private IEnumerator DoBreedingSlime()
	{
		yield return StartCoroutine(Utils.WaitFor(CanIWait, 0.1f));
		foreach (GreenSlimeParent pb in BoardManager.main.listSlime)
		{
			pb.Breeding();
			yield return null;
		}
	}

	private IEnumerator DoRotationBoardEachBoard(MapDataRotationSlot rotationSlot, bool firstMove = false)
	{
		if (((rotationSlot == null || !rotationSlot.IsTriggerOnGamePlay()) && !firstMove) || BoardManager.main == null || BoardManager.main.slotGroup == null)
		{
			yield break;
		}
		animate++;
		isPlaying = false;
		GameObject objCenter = new GameObject("rotation_center");
		objCenter.transform.SetParent(BoardManager.main.slotGroup.transform);
		Vector3 centerPosition = BoardManager.main.GetSlotPosition(rotationSlot.centerX, rotationSlot.centerY);
		if (rotationSlot.isGrid)
		{
			centerPosition -= (BoardManager.main.GetSlotPosition(1, 1) - BoardManager.main.GetSlotPosition(0, 0)) / 2f;
		}
		objCenter.transform.position = centerPosition;
		Dictionary<int, Slot> curSlots = BoardManager.main.slots;
		Dictionary<int, Slot> replacedSlots = new Dictionary<int, Slot>();
		List<BoardPosition> movedBoardPositionTo = new List<BoardPosition>();
		List<int> listFromChip = new List<int>();
		List<IBlockType> listFromBlockType = new List<IBlockType>();
		int startX = rotationSlot.centerX - rotationSlot.size / 2;
		int startY = rotationSlot.centerY + rotationSlot.size / 2 - (rotationSlot.isGrid ? 1 : 0);
		int w = rotationSlot.size;
		int h = rotationSlot.size;
		SoundSFX.PlayCap(SFXIndex.GameBoardRotate);
		Vector3 one = Vector3.one;
		float scaleStartTweenDuration = 0.24f;
		Vector3 targetRotation;
		if (!rotationSlot.isClockwork)
		{
			targetRotation = new Vector3(0f, 0f, 90f);
			for (int i = 0; i < h; i++)
			{
				for (int j = 0; j < w; j++)
				{
					int index2 = (startX + (w - j - 1)) * MapData.MaxHeight + (startY - i);
					int newX2 = startX + i;
					int newY2 = startY - j;
					int newKey2 = newX2 * MapData.MaxHeight + newY2;
					if (!curSlots.ContainsKey(index2))
					{
						BoardManager.main.boardData.slots[newX2, newY2] = false;
						continue;
					}
					Slot s2 = curSlots[index2];
					if ((bool)s2)
					{
						s2.x = newX2;
						s2.y = newY2;
						replacedSlots.Add(newKey2, s2);
						s2.transform.SetParent(objCenter.transform);
						if ((bool)s2.GetChip())
						{
							s2.GetChip().transform.DORotate(Vector3.zero, RoationBoardAnimationDuration).SetDelay(scaleStartTweenDuration);
						}
						if ((bool)s2.GetBlock())
						{
							s2.GetBlock().transform.DORotate(Vector3.zero, RoationBoardAnimationDuration).SetDelay(scaleStartTweenDuration);
						}
					}
					else
					{
						replacedSlots.Add(newKey2, null);
					}
					BoardManager.main.boardData.slots[newX2, newY2] = true;
					movedBoardPositionTo.Add(new BoardPosition(newX2, newY2));
					listFromChip.Add(BoardManager.main.boardData.chips[startX + (w - j - 1), startY - i]);
					listFromBlockType.Add(BoardManager.main.boardData.blocks[startX + (w - j - 1), startY - i]);
					curSlots[index2] = null;
					curSlots.Remove(index2);
				}
			}
		}
		else
		{
			targetRotation = new Vector3(0f, 0f, 270f);
			for (int k = 0; k < h; k++)
			{
				for (int l = 0; l < w; l++)
				{
					int index2 = (startX + l) * MapData.MaxHeight + startY - (h - k - 1);
					int newX2 = startX + k;
					int newY2 = startY - l;
					int newKey2 = newX2 * MapData.MaxHeight + newY2;
					if (!curSlots.ContainsKey(index2))
					{
						BoardManager.main.boardData.slots[newX2, newY2] = false;
						continue;
					}
					Slot s2 = curSlots[index2];
					if ((bool)s2)
					{
						s2.x = newX2;
						s2.y = newY2;
						replacedSlots.Add(newKey2, s2);
						s2.transform.SetParent(objCenter.transform);
						if ((bool)s2.GetChip())
						{
							s2.GetChip().transform.DORotate(Vector3.zero, RoationBoardAnimationDuration).SetDelay(scaleStartTweenDuration);
						}
						if ((bool)s2.GetBlock())
						{
							s2.GetBlock().transform.DORotate(Vector3.zero, RoationBoardAnimationDuration).SetDelay(scaleStartTweenDuration);
						}
					}
					else
					{
						replacedSlots.Add(newKey2, null);
					}
					BoardManager.main.boardData.slots[newX2, newY2] = true;
					movedBoardPositionTo.Add(new BoardPosition(newX2, newY2));
					listFromChip.Add(BoardManager.main.boardData.chips[startX + l, startY - (h - k - 1)]);
					listFromBlockType.Add(BoardManager.main.boardData.blocks[startX + l, startY - (h - k - 1)]);
					curSlots[index2] = null;
					curSlots.Remove(index2);
				}
			}
		}
		BoardManager.main.FadeBoardOutline(isFadeIn: false);
		if ((bool)objCenter)
		{
			SpriteRenderer[] componentsInChildren = objCenter.GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer spriteRenderer in componentsInChildren)
			{
				if ((bool)spriteRenderer && spriteRenderer.sortingLayerID == DefaultSortingLayerID)
				{
					spriteRenderer.sortingLayerID = GameEffectSortingLayerID;
				}
			}
			yield return objCenter.transform.DOScale(1.2f, scaleStartTweenDuration).SetEase(Ease.OutBack).WaitForCompletion();
			yield return objCenter.transform.DORotate(targetRotation, RoationBoardAnimationDuration).WaitForCompletion();
			yield return objCenter.transform.DOScale(1f, scaleStartTweenDuration).SetDelay(0.1f).WaitForCompletion();
			SpriteRenderer[] componentsInChildren2 = objCenter.GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer spriteRenderer2 in componentsInChildren2)
			{
				if ((bool)spriteRenderer2 && spriteRenderer2.sortingLayerID == GameEffectSortingLayerID)
				{
					spriteRenderer2.sortingLayerID = DefaultSortingLayerID;
				}
			}
		}
		if (movedBoardPositionTo.Count > 0)
		{
			for (int num = 0; num < movedBoardPositionTo.Count; num++)
			{
				int[,] chips = BoardManager.main.boardData.chips;
				BoardPosition boardPosition = movedBoardPositionTo[num];
				int x = boardPosition.x;
				BoardPosition boardPosition2 = movedBoardPositionTo[num];
				chips[x, boardPosition2.y] = listFromChip[num];
				IBlockType[,] blocks = BoardManager.main.boardData.blocks;
				BoardPosition boardPosition3 = movedBoardPositionTo[num];
				int x2 = boardPosition3.x;
				BoardPosition boardPosition4 = movedBoardPositionTo[num];
				blocks[x2, boardPosition4.y] = listFromBlockType[num];
			}
		}
		BoardPosition generatorPosition = default(BoardPosition);
		foreach (int key in replacedSlots.Keys)
		{
			if (!(replacedSlots[key] == null))
			{
				if (curSlots.ContainsKey(key))
				{
					curSlots[key] = replacedSlots[key];
				}
				else
				{
					curSlots.Add(key, replacedSlots[key]);
				}
				generatorPosition.x = replacedSlots[key].x;
				generatorPosition.y = replacedSlots[key].y;
				SlotGenerator sg = replacedSlots[key].GetComponent<SlotGenerator>();
				if (BoardManager.main.boardData.dicGeneratorDropBlock.ContainsKey(generatorPosition) || BoardManager.main.boardData.dicGeneratorSpecialDropBlock.ContainsKey(generatorPosition))
				{
					if (!sg)
					{
						sg = replacedSlots[key].gameObject.AddComponent<SlotGenerator>();
					}
					if ((bool)sg)
					{
						sg.enabled = true;
						sg.slot = replacedSlots[key];
						sg.slotForChip = replacedSlots[key].slotForChip;
						replacedSlots[key].generator = true;
					}
				}
				else if ((bool)sg)
				{
					replacedSlots[key].generator = false;
					sg.enabled = false;
				}
				replacedSlots[key].transform.parent = BoardManager.main.slotGroup.transform;
				Transform transform = replacedSlots[key].transform;
				Vector3 localPosition = replacedSlots[key].transform.localPosition;
				float x3 = Mathf.Round(localPosition.x);
				Vector3 localPosition2 = replacedSlots[key].transform.localPosition;
				float y = Mathf.Round(localPosition2.y);
				Vector3 localPosition3 = replacedSlots[key].transform.localPosition;
				transform.localPosition = new Vector3(x3, y, localPosition3.z);
				replacedSlots[key].transform.localRotation = Quaternion.identity;
				if ((bool)replacedSlots[key].GetChip())
				{
					replacedSlots[key].GetChip().transform.localRotation = Quaternion.identity;
				}
				if ((bool)replacedSlots[key].GetBlock())
				{
					replacedSlots[key].GetBlock().transform.localRotation = Quaternion.identity;
				}
			}
		}
		BoardManager.main.DrawBoardOutline();
		BoardManager.main.FadeBoardOutline(isFadeIn: true);
		rotationSlot.ClearEffects();
		BoardManager.main.DrawRotationSlotOutlineEffect(rotationSlot);
		BoardManager.main.RefreshNearSlots();
		UnityEngine.Object.Destroy(objCenter);
		if (countOfEachTargetCount[6] > 0)
		{
			for (int num2 = 0; num2 < BoardManager.main.boardData.width; num2++)
			{
				for (int num3 = 0; num3 < BoardManager.main.boardData.height; num3++)
				{
					CheckRescueGingerMan(num2, num3);
				}
			}
		}
		yield return new WaitForSeconds(0.2f);
		rotationSlot?.MoveEndGamePlay();
		replacedSlots?.Clear();
		animate--;
		isPlaying = true;
	}

	private IEnumerator DoChameleonChangeColor()
	{
		yield return StartCoroutine(Utils.WaitFor(CanIWait, 0.1f));
		if (!isGameResult && (bool)BoardManager.main && (bool)BoardManager.main.slotGroup)
		{
			SimpleChip[] allChips = BoardManager.main.slotGroup.GetComponentsInChildren<SimpleChip>();
			isPlaying = false;
			chameleonColorChanged = 0;
			SimpleChip[] array = allChips;
			foreach (SimpleChip simpleChip in array)
			{
				simpleChip.DoDynamic();
			}
			if (chameleonColorChanged > 0)
			{
				yield return new WaitForSeconds(0.8f);
			}
			isPlaying = true;
			yield return null;
			yield return StartCoroutine(Utils.WaitFor(CanIWait, 0.1f));
		}
	}

	public void ActiveNumberChocolate(int inOrder)
	{
		if (BoardManager.main.listNumberChocolate.Count > 0)
		{
			foreach (NumberChocolateBlock item in BoardManager.main.listNumberChocolate)
			{
				item.ActiveNumberChocolate(inOrder);
			}
		}
	}

	private IEnumerator ShowChangeTurnUI(VSTurn newTurnPlayer)
	{
		if (newTurnPlayer != 0)
		{
			yield return new WaitForSeconds(0.1f);
		}
	}

	public bool IsGameSuccess(bool isPreValueCheck = false)
	{
		reachedTheTarget = false;
		bool flag = true;
		if (isCollectMode)
		{
			if (!isPreValueCheck)
			{
				for (int i = 0; i < countOfEachTargetCount.Length; i++)
				{
					if (countOfEachTargetCount[i] > 0)
					{
						flag = false;
						break;
					}
				}
			}
			else
			{
				for (int j = 0; j < countOfEachTargetCountPreValue.Length; j++)
				{
					if (countOfEachTargetCountPreValue[j] > 0)
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				reachedTheTarget = true;
			}
			if (!isCollectMode || reachedTheTarget)
			{
				return true;
			}
		}
		return false;
	}

	private IEnumerator GamePlaySession()
	{
		isCollectMode = false;
		targetBringDownCount = targetBringDownRemainCount;

		for (int i = 0; i < countOfEachTargetCount.Length; i++)
		{
			countOfEachTargetCount[i] = 0;
		}

		for (int j = 0; j < countOfEachTargetCountPreValue.Length; j++)
		{
			countOfEachTargetCountPreValue[j] = 0;
		}

		for (int k = 0; k < MapData.main.collectBlocks.Length; k++)
		{
			if (string.IsNullOrEmpty(MapData.main.collectBlocks[k].blockType) || MapData.main.collectBlocks[k].count <= 0)
			{
				continue;
			}

			CollectBlockType collectBlockType = MapData.main.collectBlocks[k].GetCollectBlockType();
			if (collectBlockType != CollectBlockType.Null)
			{
				if (MapData.main.collectBlocks[k].count > 0)
				{
					countOfEachTargetCount[(int)collectBlockType] = (countOfEachTargetCountPreValue[(int)collectBlockType] = MapData.main.collectBlocks[k].count);
					isCollectMode = true;
				}
				if (collectBlockType == CollectBlockType.BringDown || collectBlockType == CollectBlockType.OreoCracker)
				{
					targetBringDownCount = (targetBringDownRemainCount = MapData.main.collectBlocks[k].count);
				}
			}
		}

		AppEventManager.m_TempBox.SetTotalCollectCount(countOfEachTargetCount);
		ActiveNumberChocolate(1);

		while (true)
		{
			if (!isPlaying)
			{
				yield return null;
				continue;
			}

			while (!CanIWait() || !completeBombCandyFactory)
			{
				yield return null;
			}

			if (IsGameSuccess())
			{
				ResultGameWin();
				yield break;
			}

			if (IsGameSuccess(isPreValueCheck: true))
			{
				break;
			}

			if (MoveCount <= 0 && isTurnResultEnd && !reachedTheTarget)
			{
				ResultGameLose(GameFailResultReason.MissionFailed);
				waitContinue = true;

				while (waitContinue)
				{
					yield return null;
				}
			}

			while (!CanINextTurn())
			{
				yield return null;
			}

			if (!isGameResult)
			{
				yield return Utils.WaitFor(CanINextTurn, 0.1f);

				while (fillingMilkEffectCount != 0)
				{
					yield return null;
				}

				if (isPlaying && eventCount > shuffleOrder)
				{
					shuffleOrder = eventCount;
					yield return StartCoroutine(Shuffle(f: false));
				}

				yield return Utils.WaitFor(CanINextTurn, 0.1f);
			}
			yield return null;
		}

		do
		{
			yield return null;
		}
		while (!IsGameSuccess());

		ResultGameWin();
	}

	public void DoGameContinue()
	{
		PlayGameBGM();
		Continue();
	}

	private IEnumerator<float> FindingSolutionsRoutine()
	{
		while (true)
		{
			if ((isPlaying || isBonusTime) && BoardManager.main.slotGroup != null)
			{
				List<Solution> solutions = FindSolutions();
				if (solutions.Count > 0)
				{
					Solution bestSolution = solutions[0];
					foreach (Solution item in solutions)
					{
						if (item.potential > bestSolution.potential)
						{
							bestSolution = item;
						}
					}
					MatchSolution(bestSolution);
#if ENABLE_MEC
					yield return Timing.WaitForOneFrame;
#else
					yield return 0.1f;
#endif
				}
				else
				{
#if ENABLE_MEC
					yield return Timing.WaitForOneFrame;
#else
					yield return 0.1f;
#endif
				}
			}
			else
			{
#if ENABLE_MEC
					yield return Timing.WaitForOneFrame;
#else
				yield return 0.1f;
#endif
			}
		}
	}

	private IEnumerator SweetRoadRoutine()
	{
		yield return new WaitForSeconds(0.6f);
		while (true)
		{
			for (int i = 0; i < isChecked.Length; i++)
			{
				isChecked[i] = false;
			}
			if (isConnectedSweetRoad && isTurnResultEnd)
			{
				yield return StartCoroutine(waitConnectedSweetRoad());
			}
			if (!isConnectedSweetRoad && (CheckSweetRoad(BoardManager.main.boardData.gateEnterX, BoardManager.main.boardData.gateEnterY) || RemainCheckSweetRoad()))
			{
				isConnectedSweetRoad = true;
			}
			CheckRemainSlotHaveToFillMilk();
			yield return null;
		}
	}

	private bool CheckRemainSlotHaveToFillMilk()
	{
		Side side = Side.Null;
		for (int i = 0; i < MapData.MaxWidth; i++)
		{
			for (int j = 0; j < MapData.MaxHeight; j++)
			{
				Slot slot = BoardManager.main.GetSlot(i, j);
				if (!slot || slot.IsExistRockCandy() || slot.isRailRoad)
				{
					continue;
				}
				for (int k = 0; k < Utils.allSides.Length; k++)
				{
					side = Utils.allSides[k];
					if (!(slot[side] == null) && (side != Side.TopLeft || ((bool)slot[Side.Top] && slot[Side.Top].isFillSweet) || ((bool)slot[Side.Left] && slot[Side.Left].isFillSweet)) && (side != Side.TopRight || ((bool)slot[Side.Top] && slot[Side.Top].isFillSweet) || ((bool)slot[Side.Right] && slot[Side.Right].isFillSweet)) && (side != Side.BottomLeft || ((bool)slot[Side.Bottom] && slot[Side.Bottom].isFillSweet) || ((bool)slot[Side.Left] && slot[Side.Left].isFillSweet)) && (side != Side.BottomRight || ((bool)slot[Side.Bottom] && slot[Side.Bottom].isFillSweet) || ((bool)slot[Side.Right] && slot[Side.Right].isFillSweet)) && slot[side].IsFilledSweet && !slot.IsFilledSweet)
					{
						slot.FillMilk();
						break;
					}
				}
			}
		}
		for (int l = 0; l < MapData.MaxWidth; l++)
		{
			for (int m = 0; m < MapData.MaxHeight; m++)
			{
				Slot slot2 = BoardManager.main.GetSlot(l, m);
				if (!slot2 || slot2.IsExistRockCandy() || slot2.isRailRoad)
				{
					continue;
				}
				for (int n = 0; n < Utils.straightSides.Length; n++)
				{
					side = Utils.straightSides[n];
					if (!(slot2[side] == null) && slot2[side].IsFilledSweet && !slot2.IsFilledSweet)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	private bool CheckSweetRoad(int x, int y)
	{
		if (!isPlaying)
		{
			return false;
		}
		isChecked[x * MapData.MaxHeight + y] = true;
		Slot slot = BoardManager.main.GetSlot(x, y);
		if (!slot)
		{
			return false;
		}
		if (slot.IsExistRockCandy() || slot.isRailRoad)
		{
			return false;
		}
		slot.FillMilk();
		BlockInterface block = slot.GetBlock();
		if ((bool)block && block is RescueFriend)
		{
			RescueFriend rescueFriend = block as RescueFriend;
			if ((bool)rescueFriend)
			{
				rescueFriend.Rescue();
			}
		}
		if ((bool)block && block is ConnectionObj)
		{
			ConnectionObj connectionObj = block as ConnectionObj;
			if ((bool)connectionObj)
			{
				connectionObj.Rescue();
			}
		}
		Side side = Side.Null;
		for (int i = 0; i < Utils.allSides.Length; i++)
		{
			side = Utils.allSides[i];
			int num = x + Utils.SideOffsetX(side);
			int num2 = y + Utils.SideOffsetY(side);
			if ((bool)slot[side] && (bool)BoardManager.main.GetSlot(num, num2) && !isChecked[num * MapData.MaxHeight + num2] && (side != Side.TopLeft || ((bool)slot[Side.Top] && slot[Side.Top].isFillSweet) || ((bool)slot[Side.Left] && slot[Side.Left].isFillSweet)) && (side != Side.TopRight || ((bool)slot[Side.Top] && slot[Side.Top].isFillSweet) || ((bool)slot[Side.Right] && slot[Side.Right].isFillSweet)) && (side != Side.BottomLeft || ((bool)slot[Side.Bottom] && slot[Side.Bottom].isFillSweet) || ((bool)slot[Side.Left] && slot[Side.Left].isFillSweet)) && (side != Side.BottomRight || ((bool)slot[Side.Bottom] && slot[Side.Bottom].isFillSweet) || ((bool)slot[Side.Right] && slot[Side.Right].isFillSweet)) && slot.IsEnableContinuousDiagonalFill() && CheckSweetRoad(num, num2))
			{
				return true;
			}
		}
		if (BoardManager.main.boardData.IsExitPosition(x, y))
		{
			if (!isConnectedOnlySweetRoad)
			{
				isConnectedOnlySweetRoad = true;
			}
			if (BoardManager.main.remainRescueFriendInBoard == 0)
			{
				return true;
			}
		}
		return false;
	}

	private bool RemainCheckSweetRoad()
	{
		for (int i = 0; i < isChecked.Length; i++)
		{
			if (!isChecked[i])
			{
				int x = i / MapData.MaxWidth;
				int y = i % MapData.MaxWidth;
				Slot slot = BoardManager.main.GetSlot(x, y);
				if ((bool)slot && slot.IsFilledSweet && CheckSweetRoad(x, y))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void CheckRescueGingerMan(int x, int y)
	{
		RescueGingerManSize rescueGingerManSize = BoardManager.main.boardData.rescueGinerManSize[x, y];
		if (rescueGingerManSize == RescueGingerManSize.Null)
		{
			return;
		}
		int num = x - BoardManager.main.boardData.rescueGingerManSubPosX[x, y];
		int num2 = y + BoardManager.main.boardData.rescueGingerManSubPosY[x, y];
		for (int i = 0; i < Utils.GetRescueGingerManSizeHeight(rescueGingerManSize); i++)
		{
			for (int j = 0; j < Utils.GetRescueGingerManSizeWidth(rescueGingerManSize); j++)
			{
				Slot slotFromSlotPosition = BoardManager.main.GetSlotFromSlotPosition(num + j, num2 - i);
				if ((bool)slotFromSlotPosition && (slotFromSlotPosition.GetBlock() is ChocolateJail || slotFromSlotPosition.GetBlock() is GreenSlimeChild || slotFromSlotPosition.GetBlock() is SpriteDrink || slotFromSlotPosition.IsExistRockCandy()))
				{
					return;
				}
			}
		}
		Slot slotFromSlotPosition2 = BoardManager.main.GetSlotFromSlotPosition(num, num2);
		if (!(slotFromSlotPosition2 == null))
		{
			string key = $"{slotFromSlotPosition2.x}x{slotFromSlotPosition2.y}";
			if (BoardManager.main.ObjRescueGingerMan.ContainsKey(key))
			{
				StartCoroutine(AnimateRescueGingerManObject(BoardManager.main.ObjRescueGingerMan[key], rescueGingerManSize));
				BoardManager.main.ObjRescueGingerMan.Remove(key);
			}
		}
	}

	private IEnumerator AnimateRescueGingerManObject(GameObject objBear, RescueGingerManSize size)
	{
		if (!(objBear == null))
		{
			PrevThrowCollectItem(CollectBlockType.RescueGingerMan);
			if (Utils.GetRescueGingerManSizeWidth(size) > Utils.GetRescueGingerManSizeHeight(size))
			{
				objBear.GetComponent<Animator>().SetTrigger("Rotation");
			}
			else
			{
				objBear.GetComponent<Animator>().SetTrigger("NoRotation");
			}
			SpriteRenderer[] componentsInChildren = objBear.GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer spriteRenderer in componentsInChildren)
			{
				spriteRenderer.gameObject.layer = LayerMask.NameToLayer("GameEffect");
			}
			//SoundSFX.Play(SFXIndex.GingerManRemove);
			AudioManager.Play_SFX(AudioClipId.CollectLamp);
			yield return new WaitForSeconds(1f);
			ThrowCollectItem(objBear, CollectBlockType.RescueGingerMan, 0.1f);
		}
	}

	private List<Chip> FindPowerups()
	{
		List<Chip> list = new List<Chip>();
		RainbowBomb[] array = UnityEngine.Object.FindObjectsOfType<RainbowBomb>();
		foreach (RainbowBomb rainbowBomb in array)
		{
			list.Add(rainbowBomb.gameObject.GetComponent<Chip>());
		}
		SimpleBomb[] array2 = UnityEngine.Object.FindObjectsOfType<SimpleBomb>();
		foreach (SimpleBomb simpleBomb in array2)
		{
			list.Add(simpleBomb.gameObject.GetComponent<Chip>());
		}
		return list;
	}

	private List<ThrownBonusCoin> thrownBonusCoinPool = new List<ThrownBonusCoin>();

	private bool poolDisabled;

	private int spawnCount;

	public CameraTargetDelegate gameCameraTargetDelegate;

	public void SkipFestival()
	{
		if (festivalCoroutine != null && isFestivalPlaying == true)
		{
			currentBonusCoin = 300 + remainMoveCount * 10;
			StopCoroutine(festivalCoroutine);
			festivalCoroutine = null;
			isPlaying = false;
			isFestivalPlaying = false;

			PopupOpenWin();
		}

		poolDisabled = true;

		for (int i = 0; i < thrownBonusCoinPool.Count; i++)
		{
			thrownBonusCoinPool[i].gameObject.SetActive(false);
		}
	}

	private bool isFestivalPlaying = false;

	private IEnumerator ProcessSweetFestival()
	{
		isFestivalPlaying = true;

		Popup.PopupSystem.Instance.CloseAllPopupsImmediately();
		var popup = Popup.PopupSystem.Instance.ShowPopup(PopupType.PopupGameComplete, Popup.CurrentPopupBehaviour.Close, false, true);

		yield return new WaitForSeconds(0.75f);

		AudioManager.Instance.PlaySFX(AudioClipId.LevelPass);

		yield return new WaitForSeconds(1.65f);

		Match3GameUI.Instance.OnFestivalStart();
		Match3GameUI.Instance.progressInfoView.UpdateBonusCoin(currentBonusCoin);

		Powerup[] createPowerupOrder = new Powerup[3]
		{
			Powerup.ColorHBomb,
			Powerup.ColorVBomb,
			Powerup.SimpleBomb
		};

		isBonusTime = true;

		remainMoveCount = MoveCount;
		int createIndex = 0;
		int stepCount = 0;

		List<SlotForChip> target = new List<SlotForChip>();
		List<SlotForChip> listDestroyTarget = new List<SlotForChip>();
		AudioSource loopSound = null;
		Chip[] allChips2 = BoardManager.main.slotGroup.GetComponentsInChildren<Chip>();

		yield return new WaitForSeconds(0.75f);

		if (allChips2.Length > 0)
		{
			listDestroyTarget.Clear();
			Chip[] array = allChips2;

			foreach (Chip chip in array)
			{
				if ((chip.chipType == ChipType.HBomb || chip.chipType == ChipType.VBomb || chip.chipType == ChipType.SimpleBomb || chip.chipType == ChipType.RainbowBomb || chip.chipType == ChipType.CandyChip) && (!chip.parentSlot || !chip.parentSlot.slot || chip.parentSlot.slot.canBeCrush))
				{
					listDestroyTarget.Add(chip.parentSlot);
				}
			}

			//if (listDestroyTarget.Count != 0 || MoveCount != 0)
			//{
			//	yield return StartCoroutine(ShowTextEffect(SpawnStringEffectType.TextEffectSweetParty));
			//	loopSound = SoundSFX.Play(SFXIndex.SweetFestivalLoop, loop: true);
			//}

			BoardManager.main.boardData.AddChipTypeInRandomList();
			while (true)
			{
				if (!CanIWait())
				{
					yield return 0;
					continue;
				}
				listDestroyTarget.Clear();
				target.Clear();
				allChips2 = BoardManager.main.slotGroup.GetComponentsInChildren<Chip>();
				Chip[] array2 = allChips2;
				foreach (Chip chip2 in array2)
				{
					if (!chip2.destroying && (bool)chip2.parentSlot && (!chip2.parentSlot.slot || chip2.parentSlot.slot.canBeCrush))
					{
						if (chip2.chipType == ChipType.HBomb || chip2.chipType == ChipType.VBomb || chip2.chipType == ChipType.SimpleBomb || chip2.chipType == ChipType.RainbowBomb || chip2.chipType == ChipType.CandyChip)
						{
							listDestroyTarget.Add(chip2.parentSlot);
						}
						else if (chip2.chipType == ChipType.SimpleChip)
						{
							target.Add(chip2.parentSlot);
						}
					}
				}

				Utils.Shuffle(target);
				for (int i = 0; i < target.Count; i++)
				{
					if (MoveCount <= 0)
					{
						break;
					}
					int x = target[i].slot.x;
					int y = target[i].slot.y;
					MoveCount--;
					//main.Score += MonoSingleton<ScoreManager>.Instance.GetScoreUnit(ScoreType.RemainMove);
					SoundSFX.PlayCap(SFXIndex.Fusion);
					var powerupChip = BoardManager.main.AddPowerup(x, y, createPowerupOrder[createIndex]);
					target[i].SetChip(powerupChip);

					powerupChip.CrushAction = () =>
					{
						if (poolDisabled == false)
						{
							var bonusCoin = thrownBonusCoinPool[spawnCount++];
							bonusCoin.gameObject.SetActive(true);
							bonusCoin.Play(powerupChip.transform.position, Match3GameUI.Instance.GetBonusCoinGameCameraPosition());
						}
					};

					listDestroyTarget.Add(target[i]);
					int num;
					createIndex = (num = createIndex + 1);
					if (num == createPowerupOrder.Length)
					{
						createIndex = 0;
					}
					yield return new WaitForSeconds(0.16f);
				}

				foreach (SlotForChip sc in listDestroyTarget)
				{
					if ((bool)sc && (bool)sc.chip && sc.chip.chipType != ChipType.SimpleChip && !sc.chip.destroying)
					{
						sc.chip.DestroyChip();

						if (stepCount == 0)
						{
							yield return new WaitForSeconds(0.15f);
						}
					}
				}

				if (listDestroyTarget.Count == 0 && MoveCount == 0)
				{
					break;
				}

				stepCount++;
				yield return new WaitForSeconds(0.4f);
			}
			if ((bool)loopSound)
			{
				loopSound.Stop();
				if (MapData.IsHardLevel(MapData.main.gid))
				{
					endSound = SoundSFX.Play(SFXIndex.HardLevelEnd);
				}
				else
				{
					endSound = SoundSFX.Play(SFXIndex.SweetFestivalEnd);
				}
			}
			while (!CanIWait())
			{
				yield return 0;
			}
			yield return new WaitForSeconds(0.4f);
		}

		isFestivalPlaying = false;

		PopupOpenWin();
	}

	private void PopupOpenWin()
	{
		DoGameResultProcess(isSuccess: true, GameFailResultReason.Clear);
	}

	public bool CanIAnimate()
	{
		return gravity == 0 && matching == 0 && rolling == 0;
	}

	public bool CanIMatch()
	{
		return animate == 0 && gravity == 0 && rolling == 0;
	}

	public bool CanINextTurn()
	{
		return isTurnResultEnd && animate == 0 && matching == 0 && gravity == 0 && rolling == 0;
	}

	public bool CanIShuffle()
	{
		return animate == 0 && matching == 0 && gravity == 0 && rolling == 0 && fillingMilkEffectCount == 0 && isTurnResultEnd && !isLockDrop && !isJustGenerateChip;
	}

	public bool CanIGravity()
	{
		return (animate == 0 || gravity > 0) && !isLockDrop;
	}

	public bool CanIWait()
	{
		return animate == 0 && matching == 0 && gravity == 0 && rolling == 0 && fillingMilkEffectCount == 0 && throwingMoveEffectCount == 0;
	}

	public bool IsCompletedThrowCollectItem()
	{
		return throwingCollectItemCount == 0;
	}

	public bool CanMoveBoard()
	{
		return fillingMilkEffectCount == 0;
	}

	public bool CanISpewCandyFactory()
	{
		return SpewingCandyFactoryCount == 0;
	}

	private void AddSolution(Solution s)
	{
		solutions.Add(s);
	}

	public void EventCounter()
	{
		eventCount++;
	}

	private bool IsCanMixMoves()
	{
		BombPair bombPair = new BombPair(ChipType.SimpleChip, ChipType.SimpleChip);
		int num = 0;
		int num2 = 0;
		for (num = 0; num < MapData.MaxWidth - 1; num++)
		{
			for (num2 = 0; num2 < MapData.MaxHeight; num2++)
			{
				if (!BoardManager.main.boardData.slots[num, num2] || !BoardManager.main.boardData.slots[num + 1, num2] || BoardManager.main.boardData.tunnel[num, num2] != 0 || BoardManager.main.boardData.tunnel[num + 1, num2] != 0 || BoardManager.main.boardData.wallsV[num, num2])
				{
					continue;
				}
				SlotForChip slotForChip = BoardManager.main.GetSlot(num, num2).slotForChip;
				if (slotForChip.chip != null)
				{
					bombPair.a = slotForChip.chip.chipType;
				}
				else
				{
					bombPair.a = ChipType.None;
				}
				slotForChip = BoardManager.main.GetSlot(num + 1, num2).slotForChip;
				if (slotForChip.chip != null)
				{
					bombPair.b = slotForChip.chip.chipType;
				}
				else
				{
					bombPair.a = ChipType.None;
				}
				if ((BoardManager.main.boardData.blocks[num, num2] == IBlockType.None || bombPair.b == ChipType.CandyChip) && (BoardManager.main.boardData.blocks[num + 1, num2] == IBlockType.None || bombPair.a == ChipType.CandyChip))
				{
					if (bombPair.a == ChipType.CandyChip && bombPair.b == ChipType.CandyChip)
					{
						return true;
					}
					if ((bombPair.a == ChipType.CandyChip && bombPair.b == ChipType.SimpleChip) || (bombPair.b == ChipType.CandyChip && bombPair.a == ChipType.SimpleChip))
					{
						return true;
					}
					if (BombMixEffect.ContainsPair(bombPair))
					{
						return true;
					}
				}
			}
		}
		for (num = 0; num < MapData.MaxWidth; num++)
		{
			for (num2 = 0; num2 < MapData.MaxHeight - 1; num2++)
			{
				if (!BoardManager.main.boardData.slots[num, num2] || !BoardManager.main.boardData.slots[num, num2 + 1] || BoardManager.main.boardData.tunnel[num, num2] != 0 || BoardManager.main.boardData.tunnel[num, num2 + 1] != 0 || BoardManager.main.boardData.wallsH[num, num2])
				{
					continue;
				}
				SlotForChip slotForChip = BoardManager.main.GetSlot(num, num2).slotForChip;
				if (slotForChip.chip != null)
				{
					bombPair.a = slotForChip.chip.chipType;
				}
				else
				{
					bombPair.a = ChipType.None;
				}
				slotForChip = BoardManager.main.GetSlot(num, num2 + 1).slotForChip;
				if (slotForChip.chip != null)
				{
					bombPair.b = slotForChip.chip.chipType;
				}
				else
				{
					bombPair.a = ChipType.None;
				}
				if ((BoardManager.main.boardData.blocks[num, num2] == IBlockType.None || bombPair.b == ChipType.CandyChip) && (BoardManager.main.boardData.blocks[num, num2 + 1] == IBlockType.None || bombPair.a == ChipType.CandyChip))
				{
					if (bombPair.a == ChipType.CandyChip && bombPair.b == ChipType.CandyChip)
					{
						return true;
					}
					if ((bombPair.a == ChipType.CandyChip && bombPair.b == ChipType.SimpleChip) || (bombPair.b == ChipType.CandyChip && bombPair.a == ChipType.SimpleChip))
					{
						return true;
					}
					if (BombMixEffect.ContainsPair(bombPair))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	private List<Move> FindMoves(bool cpuAI = false)
	{
		List<Move> list = new List<Move>();
		if (!BoardManager.main.gameObject.activeSelf)
		{
			return list;
		}
		int num = 0;
		int num2 = 0;
		ChipType chipType = ChipType.None;
		ChipType chipType2 = ChipType.None;
		for (num = 0; num < MapData.MaxWidth - 1; num++)
		{
			for (num2 = 0; num2 < MapData.MaxHeight; num2++)
			{
				if (!BoardManager.main.boardData.slots[num, num2] || !BoardManager.main.boardData.slots[num + 1, num2] || BoardManager.main.boardData.tunnel[num, num2] != 0 || BoardManager.main.boardData.tunnel[num + 1, num2] != 0 || BoardManager.main.boardData.blocks[num, num2] != 0 || BoardManager.main.boardData.blocks[num + 1, num2] != 0 || BoardManager.main.boardData.wallsV[num, num2])
				{
					continue;
				}
				Move move = new Move();
				move.fromX = num;
				move.fromY = num2;
				move.toX = num + 1;
				move.toY = num2;
				SlotForChip slotForChip = BoardManager.main.GetSlot(move.fromX, move.fromY).slotForChip;
				chipType = ((!(slotForChip.chip == null)) ? slotForChip.chip.chipType : ChipType.None);
				slotForChip = BoardManager.main.GetSlot(move.toX, move.toY).slotForChip;
				chipType2 = ((!(slotForChip.chip == null)) ? slotForChip.chip.chipType : ChipType.None);
				if (BombMixEffect.ContainsPair(chipType, chipType2))
				{
					list.Add(move);
				}
				else
				{
					if (BoardManager.main.boardData.chips[num, num2] == BoardManager.main.boardData.chips[num + 1, num2])
					{
						continue;
					}
					Move move2 = new Move();
					move2.fromX = num;
					move2.fromY = num2;
					move2.toX = num + 1;
					move2.toY = num2;
					AnalizSwap(move2);
					slotForChip = BoardManager.main.GetSlot(move2.fromX, move2.fromY).slotForChip;
					chipType = ((!(slotForChip.chip == null)) ? slotForChip.chip.chipType : ChipType.None);
					Solution solution = slotForChip.MatchAnaliz(cpuAI);
					if (solution != null)
					{
						move2.potencial += solution.potential;
						move2.solution = solution;
					}
					slotForChip = BoardManager.main.GetSlot(move2.toX, move2.toY).slotForChip;
					solution = slotForChip.MatchAnaliz(cpuAI);
					chipType2 = ((!(slotForChip.chip == null)) ? slotForChip.chip.chipType : ChipType.None);
					if (solution != null)
					{
						if (solution != null && (move2.potencial < solution.potential || move2.solution == null))
						{
							move2.solution = solution;
						}
						if (solution != null)
						{
							move2.potencial += solution.potential;
						}
					}
					AnalizSwap(move2);
					if (move2.solution != null && (move2.potencial != 0 || (chipType != ChipType.SimpleChip && chipType2 != ChipType.SimpleChip)))
					{
						list.Add(move2);
					}
				}
			}
		}
		for (num = 0; num < MapData.MaxWidth; num++)
		{
			for (num2 = 0; num2 < MapData.MaxHeight - 1; num2++)
			{
				if (!BoardManager.main.boardData.slots[num, num2] || !BoardManager.main.boardData.slots[num, num2 + 1] || BoardManager.main.boardData.tunnel[num, num2] != 0 || BoardManager.main.boardData.tunnel[num, num2 + 1] != 0 || BoardManager.main.boardData.blocks[num, num2] != 0 || BoardManager.main.boardData.blocks[num, num2 + 1] != 0 || BoardManager.main.boardData.wallsH[num, num2])
				{
					continue;
				}
				Move move = new Move();
				move.fromX = num;
				move.fromY = num2;
				move.toX = num;
				move.toY = num2 + 1;
				SlotForChip slotForChip = BoardManager.main.GetSlot(move.fromX, move.fromY).slotForChip;
				chipType = ((!(slotForChip.chip == null)) ? slotForChip.chip.chipType : ChipType.None);
				slotForChip = BoardManager.main.GetSlot(move.toX, move.toY).slotForChip;
				chipType2 = ((!(slotForChip.chip == null)) ? slotForChip.chip.chipType : ChipType.None);
				if (BombMixEffect.ContainsPair(chipType, chipType2))
				{
					list.Add(move);
				}
				else
				{
					if (BoardManager.main.boardData.chips[num, num2] == BoardManager.main.boardData.chips[num, num2 + 1])
					{
						continue;
					}
					Move move2 = new Move();
					move2.fromX = num;
					move2.fromY = num2;
					move2.toX = num;
					move2.toY = num2 + 1;
					AnalizSwap(move2);
					slotForChip = BoardManager.main.GetSlot(move2.fromX, move2.fromY).slotForChip;
					chipType = ((!(slotForChip.chip == null)) ? slotForChip.chip.chipType : ChipType.None);
					Solution solution = slotForChip.MatchAnaliz(cpuAI);
					if (solution != null)
					{
						move2.potencial += solution.potential;
						move2.solution = solution;
					}
					slotForChip = BoardManager.main.GetSlot(move2.toX, move2.toY).slotForChip;
					solution = slotForChip.MatchAnaliz(cpuAI);
					chipType2 = ((!(slotForChip.chip == null)) ? slotForChip.chip.chipType : ChipType.None);
					if (solution != null)
					{
						if (solution != null && (move2.potencial < solution.potential || move2.solution == null))
						{
							move2.solution = solution;
						}
						if (solution != null)
						{
							move2.potencial += solution.potential;
						}
					}
					AnalizSwap(move2);
					if (move2.solution != null && (move2.potencial != 0 || (chipType != ChipType.SimpleChip && chipType2 != ChipType.SimpleChip)))
					{
						list.Add(move2);
					}
				}
			}
		}
		return list;
	}

	private bool IsExistCandyChipOrRainbowChip()
	{
		foreach (KeyValuePair<int, Slot> slot in BoardManager.main.slots)
		{
			Slot value = slot.Value;
			if ((bool)value)
			{
				Chip chip = value.GetChip();
				if ((bool)chip && (chip.chipType == ChipType.CandyChip || chip.chipType == ChipType.RainbowBomb) && value.GetBlock() == null)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void AnalizSwap(Move move)
	{
		Chip chip = BoardManager.main.GetSlot(move.fromX, move.fromY).GetChip();
		Chip chip2 = BoardManager.main.GetSlot(move.toX, move.toY).GetChip();
		if ((bool)chip && (bool)chip2)
		{
			SlotForChip parentSlot = chip2.parentSlot;
			chip.parentSlot.SetChip(chip2);
			parentSlot.SetChip(chip);
		}
	}

	private void MatchSolution(Solution solution)
	{
		EventCounter();
		int x = solution.x;
		int y = solution.y;
		Slot centerSlot = BoardManager.main.GetSlot(x, y);
		if (!centerSlot)
		{
			return;
		}
		Chip centerChip = BoardManager.main.GetSlot(x, y).slotForChip.GetChip();
		if (centerChip == null)
		{
			return;
		}
		int width = BoardManager.main.boardData.width;
		int height = BoardManager.main.boardData.height;
		int num = -1;
		int num2 = -1;
		int num3 = -1;
		int num4 = -1;
		if (!centerSlot.canBeCrush || !centerChip.IsMatchable())
		{
			return;
		}
		int[] array = new int[4];
		int[] array2 = new int[4];
		List<Slot> list = new List<Slot>();
		int num5 = -1;
		if (solution.boxCombinationCount > 0)
		{
			Slot[] array3 = new Slot[4];
			int num6 = -1;
			int num7 = -1;
			for (int i = 0; i < 4; i++)
			{
				array[i] = -1;
				array2[i] = -1;
				if (!solution.boxCombinationPos[i])
				{
					continue;
				}
				int num8 = x + findBoxCombinationOffsetX[i];
				int num9 = y + findBoxCombinationOffsetY[i];
				if (num8 < 0 || num8 >= width || num9 < 0 || num9 >= height)
				{
					continue;
				}
				array3[0] = BoardManager.main.GetSlot(num8, num9);
				array3[1] = BoardManager.main.GetSlot(x, num9);
				array3[2] = BoardManager.main.GetSlot(num8, y);
				array3[3] = BoardManager.main.GetSlot(x, y);
				int num10 = 0;
				for (int j = 0; j < array3.Length && (bool)array3[j] && array3[j].canBeCrush && (bool)array3[j].GetChip() && array3[j].GetChip().IsMatchable() && array3[j].GetChip().id == BoardManager.main.GetSlot(x, y).GetChip().id; j++)
				{
					int num11 = num8;
					int num12 = num9;
					switch (j)
					{
						case 1:
							num11 = x;
							break;
						case 2:
							num12 = y;
							break;
						case 3:
							num11 = x;
							num12 = y;
							break;
					}
					if (array3[j].GetChip().movementID > num5)
					{
						num5 = array3[j].GetChip().movementID;
						num6 = num11;
						num7 = num12;
					}
					num10++;
				}
				if (num10 == 4)
				{
					for (int k = 0; k < array3.Length; k++)
					{
						list.Add(array3[k]);
					}
					array[i] = num6;
					array2[i] = num7;
				}
			}
		}
		List<Slot> targetSlotList = new List<Slot>();
		if (solution.h)
		{
			for (int l = x + 1; l < width; l++)
			{
				Slot slot2 = BoardManager.main.GetSlot(l, y);
				if (!slot2 || !slot2.canBeCrush || !slot2.GetChip())
				{
					break;
				}
				Chip chip2 = slot2.GetChip();
				if (chip2.id != centerChip.id || !chip2.IsMatchable())
				{
					break;
				}
				if (chip2.movementID > num3)
				{
					num3 = chip2.movementID;
					num = l;
					num2 = y;
					num4 = chip2.id;
				}
				targetSlotList.Add(slot2);
			}
		}
		if (solution.h)
		{
			for (int l = x - 1; l >= 0; l--)
			{
				Slot slot2 = BoardManager.main.GetSlot(l, y);
				if (!slot2 || !slot2.canBeCrush || !slot2.GetChip())
				{
					break;
				}
				Chip chip = slot2.GetChip();
				if (chip.id != centerChip.id || !chip.IsMatchable())
				{
					break;
				}
				if (chip.movementID > num3)
				{
					num3 = chip.movementID;
					num = l;
					num2 = y;
					num4 = chip.id;
				}
				targetSlotList.Add(slot2);
			}
		}
		if (solution.v)
		{
			for (int m = y + 1; m < height; m++)
			{
				Slot slot2 = BoardManager.main.GetSlot(x, m);
				if (!slot2 || !slot2.canBeCrush || !slot2.GetChip())
				{
					break;
				}
				Chip chip = slot2.GetChip();
				if (chip.id != centerChip.id || !chip.IsMatchable())
				{
					break;
				}
				if (chip.movementID > num3)
				{
					num3 = chip.movementID;
					num = x;
					num2 = m;
					num4 = chip.id;
				}
				targetSlotList.Add(slot2);
			}
		}
		if (solution.v)
		{
			for (int m = y - 1; m >= 0; m--)
			{
				Slot slot2 = BoardManager.main.GetSlot(x, m);
				if (!slot2 || !slot2.canBeCrush || !slot2.GetChip())
				{
					break;
				}
				Chip chip = slot2.GetChip();
				if (chip.id != centerChip.id || !chip.IsMatchable())
				{
					break;
				}
				if (chip.movementID > num3)
				{
					num3 = chip.movementID;
					num = x;
					num2 = m;
					num4 = chip.id;
				}
				targetSlotList.Add(slot2);
			}
		}
		if (centerChip.movementID > num3)
		{
			num3 = centerChip.movementID;
			num = x;
			num2 = y;
			num4 = centerChip.id;
		}
		bool notGenerateSpecial = true;
		if ((bool)centerSlot)
		{
			targetSlotList.Add(centerSlot);
		}
		bool flag2 = false;
		if (MapData.main.target == GoalTarget.Jelly)
		{
			for (int n = 0; n < targetSlotList.Count; n++)
			{
				Slot slot2 = targetSlotList[n];
				if ((bool)slot2 && slot2.IsPaintedJelly)
				{
					flag2 = true;
					break;
				}
			}
		}

		targetSlotList.Sort(chipCollectDelayTimeSortingComparison);
		for (int i = 0, t = 0; i < targetSlotList.Count; i++)
		{
			Slot slot = targetSlotList[i];
			Chip chip = slot.GetChip();
			if (IdNeedCollected(chip.id))
            {
				chip.collectDelayTime = t * 0.15f;
				t++;

				if (solution.count >= 4 && slot.x == num && slot.y == num2)
                {
					chip.collectDelayTime += Chip.combineBombDuration;
                }
			}	
		}

		// generate special
		if (solution.count >= 4)
		{
			if (solution.count >= 5 && ((solution.v && !solution.h) || (!solution.v && solution.h) || solution.negH + solution.posH >= 4 || solution.negV + solution.posV >= 4))
			{
				notGenerateSpecial = false;
				list.Remove(BoardManager.main.GetSlot(num, num2));
				IncCombo();
				BoardManager boardManager = BoardManager.main;
				int x2 = num;
				int y2 = num2;
				bool radius = true;
				ScoreType scoreType = ScoreType.MakeItem5x1;
				bool includeCrushChip = true;
				int addScoreBlockCount = solution.count - 1;
				int fromCrushId = num4;
				bool doPaintJelly = flag2;
				boardManager.SlotCrush(x2, y2, radius, scoreType, includeCrushChip, addScoreBlockCount, fromCrushId, Side.Null, doPaintJelly);
				Slot slot2 = BoardManager.main.GetSlot(num, num2);
				Chip chip2 = slot2.GetChip();

				if ((bool)chip2)
				{
					chip2.HideChip();
				}
				BoardManager.main.GetNewRainbowBomb(num, num2, BoardManager.main.GetSlot(num, num2).transform.position, isCombination: true);
				DecreaseCollectMakeSpecialBlock(CollectBlockType.MakeRainbowChip, BoardManager.main.GetSlot(num, num2).transform.position, 0);
				if (targetSlotList.Contains(slot2))
				{
					targetSlotList.Remove(slot2);
				}
				SoundSFX.PlayCap(SFXIndex.Fusion);
				if (num3 == lastMovementId && CurrentTurn == VSTurn.Player)
				{
					AppEventManager.m_TempBox.howManyMakeBlock5x1++;
				}
			}
			else if (solution.v && solution.h)
			{
				notGenerateSpecial = false;
				list.Remove(BoardManager.main.GetSlot(num, num2));
				IncCombo();
				BoardManager boardManager2 = BoardManager.main;
				int fromCrushId = num;
				int addScoreBlockCount = num2;
				bool doPaintJelly = true;
				ScoreType scoreType = ScoreType.MakeItem3x3;
				bool includeCrushChip = true;
				int y2 = solution.count - 1;
				int x2 = num4;
				bool radius = flag2;
				boardManager2.SlotCrush(fromCrushId, addScoreBlockCount, doPaintJelly, scoreType, includeCrushChip, y2, x2, Side.Null, radius);
				Slot slot2 = BoardManager.main.GetSlot(num, num2);
				Chip chip2 = slot2.GetChip();

				if ((bool)chip2)
				{
					chip2.HideChip();
				}
				BoardManager.main.GetNewBomb(num, num2, BoardManager.main.GetSlot(num, num2).transform.position, solution.color, isCombination: true);
				DecreaseCollectMakeSpecialBlock(CollectBlockType.MakeSimpleBomb, BoardManager.main.GetSlot(num, num2).transform.position, solution.color);
				if (targetSlotList.Contains(slot2))
				{
					targetSlotList.Remove(slot2);
				}
				SoundSFX.PlayCap(SFXIndex.Fusion);
				if (num3 == lastMovementId && CurrentTurn == VSTurn.Player)
				{
					AppEventManager.m_TempBox.howManyMakeBlock3x3++;
				}
			}
			else if (solution.v && !solution.h)
			{
				notGenerateSpecial = false;
				list.Remove(BoardManager.main.GetSlot(num, num2));
				IncCombo();
				BoardManager boardManager3 = BoardManager.main;
				int x2 = num;
				int y2 = num2;
				bool radius = true;
				ScoreType scoreType = ScoreType.MakeItem4x1;
				bool includeCrushChip = true;
				int addScoreBlockCount = solution.count - 1;
				int fromCrushId = num4;
				bool doPaintJelly = flag2;
				boardManager3.SlotCrush(x2, y2, radius, scoreType, includeCrushChip, addScoreBlockCount, fromCrushId, Side.Null, doPaintJelly);
				Slot slot2 = BoardManager.main.GetSlot(num, num2);
				Chip chip2 = slot2.GetChip();

				if ((bool)chip2)
				{
					chip2.HideChip();
				}
				BoardManager.main.GetNewColorHBomb(num, num2, BoardManager.main.GetSlot(num, num2).transform.position, solution.color, isCombination: true);
				DecreaseCollectMakeSpecialBlock(CollectBlockType.MakeHBomb, BoardManager.main.GetSlot(num, num2).transform.position, solution.color);
				if (targetSlotList.Contains(slot2))
				{
					targetSlotList.Remove(slot2);
				}
				SoundSFX.PlayCap(SFXIndex.Fusion);
				if (num3 == lastMovementId && CurrentTurn == VSTurn.Player)
				{
					AppEventManager.m_TempBox.howManyMakeBlock4x1++;
				}
			}
			else if (!solution.v && solution.h)
			{
				notGenerateSpecial = false;
				list.Remove(BoardManager.main.GetSlot(num, num2));
				IncCombo();
				BoardManager boardManager4 = BoardManager.main;
				int fromCrushId = num;
				int addScoreBlockCount = num2;
				bool doPaintJelly = true;
				ScoreType scoreType = ScoreType.MakeItem4x1;
				bool includeCrushChip = true;
				int y2 = solution.count - 1;
				int x2 = num4;
				bool radius = flag2;
				boardManager4.SlotCrush(fromCrushId, addScoreBlockCount, doPaintJelly, scoreType, includeCrushChip, y2, x2, Side.Null, radius);
				Slot slot2 = BoardManager.main.GetSlot(num, num2);
				Chip chip2 = slot2.GetChip();
	
				if ((bool)chip2)
				{
					chip2.HideChip();
				}
				BoardManager.main.GetNewColorVBomb(num, num2, BoardManager.main.GetSlot(num, num2).transform.position, solution.color, isCombination: true);
				DecreaseCollectMakeSpecialBlock(CollectBlockType.MakeVBomb, BoardManager.main.GetSlot(num, num2).transform.position, solution.color);
				if (targetSlotList.Contains(slot2))
				{
					targetSlotList.Remove(slot2);
				}
				SoundSFX.PlayCap(SFXIndex.Fusion);
				if (num3 == lastMovementId && CurrentTurn == VSTurn.Player)
				{
					AppEventManager.m_TempBox.howManyMakeBlock4x1++;
				}
			}
		}

		if (solution.boxCombinationCount > 0)
		{
			for (int num13 = 0; num13 < list.Count; num13++)
			{
				if (!(list[num13] == null) && !targetSlotList.Contains(list[num13]))
				{
					targetSlotList.Add(list[num13]);
				}
			}
			if (MapData.main.target == GoalTarget.Jelly)
			{
				for (int num14 = 0; num14 < targetSlotList.Count; num14++)
				{
					Slot slot2 = targetSlotList[num14];
					if ((bool)slot2 && slot2.IsPaintedJelly)
					{
						flag2 = true;
						break;
					}
				}
			}
		}

		for (int i = 0; i < targetSlotList.Count; i++)
		{
			Slot currentSlot = targetSlotList[i];
			if (!currentSlot)
			{
				continue;
			}

			if (notGenerateSpecial)
			{
				bool radius = true;
				ScoreType scoreType = ScoreType.None;
				bool includeCrushChip = true;
				int addScoreBlockCount = num4;

				BoardManager.main.SlotCrush(currentSlot.x, currentSlot.y, radius, scoreType, includeCrushChip, 0, num4, Side.Null, flag2);

				continue;
			}

			Chip currentChip = currentSlot.GetChip();

			if ((bool)currentChip)
			{
				bool includeCrushChip = true;
				ScoreType scoreType = ScoreType.None;
				bool radius = currentChip.chipType != ChipType.SimpleChip;

				if (!BoardManager.main.SlotCrush(currentSlot.x, currentSlot.y, includeCrushChip, scoreType, radius, 0, num4, Side.Null, flag2) && currentChip.chipType == ChipType.SimpleChip)
				{
					currentChip.CombineBomb(num, num2);
				}
			}
		}

		if (notGenerateSpecial)
		{
			IncCombo();
			//BoardManager.main.SetScore(num, num2, ScoreType.Match3, num4);
		}
		list.Clear();
		targetSlotList.Clear();
	}

	public int GetMovementID()
	{
		lastMovementId++;
		return lastMovementId;
	}

	public IEnumerator ShuffleBooster()
	{
		if (!isPlaying || !CanIShuffle())
		{
			yield break;
		}
		yield return new WaitForSeconds(0.2f);
		if (!CanIShuffle() || !isPlaying)
		{
			yield break;
		}
		List<Move> moves = FindMoves();
		if (rolling > 0 || isGameResult)
		{
			yield break;
		}
		isPlaying = false;
		Slot[] slots = UnityEngine.Object.FindObjectsOfType<Slot>();
		Dictionary<Slot, Vector3> positions = new Dictionary<Slot, Vector3>();
		Slot[] array = slots;
		foreach (Slot slot in array)
		{
			positions.Add(slot, slot.transform.position);
		}
		float t2 = 0f;
		List<Solution> solutions = FindSolutions();
		int itrn = 0;
		bool f = true;
		SlotForChip[] sc = UnityEngine.Object.FindObjectsOfType<SlotForChip>();
		while (f || moves.Count == 0 || solutions.Count > 0)
		{
			if (itrn > 1000)
			{
				yield break;
			}
			EventCounter();
			f = false;
			for (int j = 0; j < sc.Length; j++)
			{
				int targetID = UnityEngine.Random.Range(0, j - 1);
				if ((bool)sc[j].chip && (bool)sc[targetID].chip && sc[j].chip.chipType == ChipType.SimpleChip && sc[targetID].chip.chipType == ChipType.SimpleChip)
				{
					BlockInterface b = sc[j].chip.parentSlot.slot.GetBlock();
					if (!b && sc[j].chip.chipType == ChipType.SimpleChip)
					{
						AnimationAssistant.main.SwapTwoItemNow(sc[j].chip, sc[targetID].chip);
					}
				}
			}
			moves = FindMoves();
			solutions = FindSolutions();
			itrn++;
		}
		yield return null;
		Vector3[] firstPos = new Vector3[slots.Length];
		Vector3[] tempPos = new Vector3[slots.Length];
		for (int k = 0; k < slots.Length; k++)
		{
			Chip gChip = slots[k].GetChip();
			if ((bool)gChip && gChip.chipType == ChipType.SimpleChip)
			{
				firstPos[k] = gChip.transform.position;
				tempPos[k] = gChip.transform.position;
			}
		}
		while (t2 < 1f)
		{
			t2 += Time.unscaledDeltaTime;
			for (int l = 0; l < slots.Length; l++)
			{
				BlockInterface b = slots[l].GetBlock();
				if (!b)
				{
					Chip gChip = slots[l].GetChip();
					Vector3 a = firstPos[l];
					Vector3 position = BoardManager.main.slotGroup.transform.position;
					if ((bool)gChip && gChip.chipType == ChipType.SimpleChip)
					{
						Vector3 a2 = (a + position) * 0.5f;
						a2 = ((!(a.y >= 0f)) ? (a2 - new Vector3(-1f, 0f, 0f)) : (a2 - new Vector3(1f, 0f, 0f)));
						Vector3 a3 = a - a2;
						Vector3 b4 = position - a2;
						gChip.transform.position = Vector3.Slerp(a3, b4, t2);
						gChip.transform.position += a2;
						tempPos[l] = gChip.transform.position;
					}
				}
			}
			yield return 0;
		}
		t2 = 0f;
		while (t2 < 1f)
		{
			t2 += Time.unscaledDeltaTime * 2f;
			for (int m = 0; m < slots.Length; m++)
			{
				BlockInterface b = slots[m].GetBlock();
				if (!b)
				{
					Chip gChip = slots[m].GetChip();
					if ((bool)gChip && gChip.chipType == ChipType.SimpleChip)
					{
						gChip.transform.position = Vector3.Lerp(tempPos[m], positions[slots[m]], t2);
					}
				}
			}
			yield return 0;
		}
		isPlaying = true;
		SetBestMovesNull();
	}

	public IEnumerator Shuffle(bool f)
	{
		bool force = f;
		if (!isPlaying || !CanIShuffle())
		{
			yield break;
		}
		yield return new WaitForSeconds(0.2f);
		if (!CanIShuffle() || !isPlaying)
		{
			yield break;
		}
		List<Move> moves = FindMoves();
		if ((moves.Count > 0 && !force) || rolling > 0 || IsCanMixMoves() || isGameResult)
		{
			yield break;
		}
		isPlaying = false;
		yield return StartCoroutine(ShowTextEffect(SpawnStringEffectType.TextEffectShuffle));
		Slot[] slots = UnityEngine.Object.FindObjectsOfType<Slot>();
		Dictionary<Slot, Vector3> positions = new Dictionary<Slot, Vector3>();
		Slot[] array = slots;
		foreach (Slot slot in array)
		{
			positions.Add(slot, slot.transform.position);
		}
		float t2 = 0f;
		while (t2 < 1f)
		{
			t2 += Time.unscaledDeltaTime * 3f;
			for (int j = 0; j < slots.Length; j++)
			{
				BlockInterface b = slots[j].GetBlock();
				if (!b)
				{
					Chip gChip = slots[j].GetChip();
					if ((bool)gChip && gChip.chipType == ChipType.SimpleChip)
					{
						gChip.transform.position = Vector3.Lerp(positions[slots[j]], BoardManager.main.slotGroup.transform.position, t2);
					}
				}
			}
			yield return 0;
		}
		List<Solution> solutions = FindSolutions();
		int itrn = 0;
		SlotForChip[] sc = UnityEngine.Object.FindObjectsOfType<SlotForChip>();
		while (f || moves.Count == 0 || solutions.Count > 0)
		{
			if (itrn > 1000)
			{
				ResultGameLose(GameFailResultReason.ShuffleLimit);
				yield break;
			}
			f = false;
			EventCounter();
			for (int k = 0; k < sc.Length; k++)
			{
				int targetID = UnityEngine.Random.Range(0, k - 1);
				if ((bool)sc[k].chip && (bool)sc[targetID].chip && sc[k].chip.chipType == ChipType.SimpleChip && sc[targetID].chip.chipType == ChipType.SimpleChip)
				{
					BlockInterface b = sc[k].chip.parentSlot.slot.GetBlock();
					if (!b && sc[k].chip.chipType == ChipType.SimpleChip)
					{
						AnimationAssistant.main.SwapTwoItemNow(sc[k].chip, sc[targetID].chip);
					}
				}
			}
			moves = FindMoves();
			solutions = FindSolutions();
			itrn++;
		}
		t2 = 0f;
		while (t2 < 1f)
		{
			t2 += Time.unscaledDeltaTime * 3f;
			for (int l = 0; l < slots.Length; l++)
			{
				BlockInterface b = slots[l].GetBlock();
				if (!b)
				{
					Chip gChip = slots[l].GetChip();
					if ((bool)gChip && gChip.chipType == ChipType.SimpleChip)
					{
						gChip.transform.position = Vector3.Lerp(BoardManager.main.slotGroup.transform.position, positions[slots[l]], t2);
					}
				}
			}
			yield return 0;
		}
		isPlaying = true;
		shuffleOccurredCount++;
	}

	private List<Solution> FindSolutions()
	{
		List<Solution> list = new List<Solution>();
		for (int i = 0; i < MapData.MaxWidth; i++)
		{
			for (int j = 0; j < MapData.MaxHeight; j++)
			{
				if (!BoardManager.main.boardData.slots[i, j])
				{
					continue;
				}
				Slot slot = BoardManager.main.GetSlot(i, j);
				if ((bool)slot)
				{
					Solution solution = slot.slotForChip.MatchAnaliz();
					if (solution != null)
					{
						list.Add(solution);
					}
				}
			}
		}
		return list;
	}

	private IEnumerator ShowingHintRoutine()
	{
		int hintOrder = 0;
		float delay = 3.5f;
		while (true)
		{
			float duration = 0f;
			yield return StartCoroutine(Utils.WaitFor(CanIWait, 0.5f));
			while (duration < delay && CanIWait())
			{
				duration += Time.deltaTime;
				yield return null;
			}
			if (duration >= delay && eventCount > hintOrder)
			{
				hintOrder = eventCount;
				ShowHint();
			}
		}
	}

	public void SetBestMovesNull()
	{
		bestMove = null;
	}

	private void ShowHint()
	{
		if (!isPlaying || BoardManager.main == null || IsMovingNextMap || MapData.main == null || main == null)
		{
			return;
		}
		List<Move> list = FindMoves();
		if (list == null)
		{
			return;
		}
		if (list.Count == 0)
		{
			if (IsExistCandyChipOrRainbowChip() && BoardManager.main.slots != null)
			{
				Chip chip = null;
				using (Dictionary<int, Slot>.Enumerator enumerator = BoardManager.main.slots.GetEnumerator())
				{
					while (enumerator.MoveNext() && chip == null)
					{
						Slot value = enumerator.Current.Value;
						if ((bool)value)
						{
							Chip chip2 = value.GetChip();
							if ((bool)chip2 && chip2.chipType == ChipType.CandyChip)
							{
								chip = chip2;
							}
						}
					}
				}
				if ((bool)chip)
				{
					chip.LookAtMe(eventCount);
				}
			}
			return;
		}
		if (BoardManager.main.enabledTutorialOutlineEffect)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].solution != null && ((list[i].fromX == BoardManager.main.boardData.tutorial1X && list[i].fromY == BoardManager.main.boardData.tutorial1Y && list[i].toX == BoardManager.main.boardData.tutorial2X && list[i].toY == BoardManager.main.boardData.tutorial2Y) || (list[i].fromX == BoardManager.main.boardData.tutorial2X && list[i].fromY == BoardManager.main.boardData.tutorial2Y && list[i].toX == BoardManager.main.boardData.tutorial1X && list[i].toY == BoardManager.main.boardData.tutorial1Y)))
				{
					bestMove = list[i];
				}
			}
		}
		if (bestMove == null)
		{
			for (int j = 0; j < list.Count; j++)
			{
				if (list[j].solution != null && list[j].solution.itemBlock51Count > 0)
				{
					bestMove = list[j];
				}
			}
		}
		if (bestMove == null)
		{
			for (int k = 0; k < list.Count; k++)
			{
				if (list[k].solution != null && (list[k].solution.itemBlock33Count > 0 || list[k].solution.itemBlock41Count > 0 || list[k].solution.boxCombinationCount > 0))
				{
					bestMove = list[k];
				}
			}
		}
		if (bestMove == null)
		{
			int num = 0;
			int num2 = 3;
			for (int l = 0; l < list.Count; l++)
			{
				if (list[l].solution != null)
				{
					if (list[l].solution.potential > num2)
					{
						num2 = list[l].solution.potential;
						num = l;
					}
					continue;
				}
				Slot slot = BoardManager.main.GetSlot(list[l].fromX, list[l].fromY);
				Slot slot2 = BoardManager.main.GetSlot(list[l].toX, list[l].toY);
				if (!slot || !slot2)
				{
					continue;
				}
				Chip chip3 = slot.GetChip();
				Chip chip4 = slot2.GetChip();
				if ((bool)chip3 && (bool)chip4)
				{
					int potential = chip3.GetPotential();
					int potential2 = chip4.GetPotential();
					int num3 = potential * potential2;
					if (num3 == 25)
					{
						num3 = 100;
					}
					if (num3 > num2)
					{
						num2 = num3;
						num = l;
					}
				}
			}
			if (num2 > 3 && num < list.Count)
			{
				bestMove = list[num];
			}
		}
		if (bestMove == null)
		{
			bestMove = list[UnityEngine.Random.Range(0, list.Count)];
		}
		if (bestMove != null && bestMove.solution == null)
		{
			Slot slot3 = BoardManager.main.GetSlot(bestMove.fromX, bestMove.fromY);
			Slot slot4 = BoardManager.main.GetSlot(bestMove.toX, bestMove.toY);
			if ((bool)slot3 && (bool)slot4)
			{
				Chip chip5 = slot3.GetChip();
				Chip chip6 = slot4.GetChip();
				if ((bool)chip5)
				{
					chip5.LookAtMe(eventCount);
				}
				if ((bool)chip6)
				{
					chip6.LookAtMe(eventCount);
				}
			}
		}
		else
		{
			if (bestMove == null || bestMove.solution == null)
			{
				return;
			}
			if (bestMove.solution.h)
			{
				for (int m = bestMove.solution.x - bestMove.solution.negH; m <= bestMove.solution.x + bestMove.solution.posH; m++)
				{
					if (m != bestMove.solution.x)
					{
						ShowHintLookAtMeEffect(m, bestMove.solution.y);
					}
				}
			}
			if (bestMove.solution.v)
			{
				for (int n = bestMove.solution.y - bestMove.solution.negV; n <= bestMove.solution.y + bestMove.solution.posV; n++)
				{
					if (n != bestMove.solution.y)
					{
						ShowHintLookAtMeEffect(bestMove.solution.x, n);
					}
				}
			}
			if (bestMove.solution.boxCombinationCount > 0)
			{
				for (int num4 = 0; num4 < 4; num4++)
				{
					if (bestMove.solution.boxCombinationPos[num4])
					{
						int x = bestMove.solution.x + findBoxCombinationOffsetX[num4];
						int y = bestMove.solution.y + findBoxCombinationOffsetY[num4];
						ShowHintLookAtMeEffect(x, bestMove.solution.y);
						ShowHintLookAtMeEffect(bestMove.solution.x, y);
						ShowHintLookAtMeEffect(x, y);
					}
				}
			}
			if (bestMove.fromX != bestMove.solution.x || bestMove.fromY != bestMove.solution.y)
			{
				ShowHintLookAtMeEffect(bestMove.fromX, bestMove.fromY);
			}
			if (bestMove.toX != bestMove.solution.x || bestMove.toY != bestMove.solution.y)
			{
				ShowHintLookAtMeEffect(bestMove.toX, bestMove.toY);
			}
		}
	}

	private void ShowHintLookAtMeEffect(int x, int y)
	{
		if (BoardManager.main.GetSlot(x, y) != null && BoardManager.main.GetSlot(x, y).GetChip() != null)
		{
			BoardManager.main.GetSlot(x, y).GetChip().LookAtMe(eventCount);
		}
	}

	public void StartCombo()
	{
		ComboCount = 0;
	}

	public void IncCombo()
	{
		ComboCount++;
		int index = Mathf.Min(9, ComboCount - 1);
		SoundSFX.Play((SFXIndex)index);
	}

	public IEnumerator ShowTextEffect(SpawnStringEffectType effectTextType)
	{
		float delayTime = showTextEffectBaseDelayTime;
		if (BoardManager.main.slotGroup == null)
		{
			yield break;
		}
		SoundSFX.Play(SFXIndex.SlidePopupShow);
		SFXIndex index = SFXIndex.SlidePopupHide;
		float delay = showTextEffectBaseDelayTime;
		SoundSFX.Play(index, loop: false, delay);
		switch (effectTextType)
		{
			case SpawnStringEffectType.TextEffectShuffle:
				SoundSFX.Play(SFXIndex.Shuffle);
				break;
			case SpawnStringEffectType.TextEffectSweetParty:
				SoundSFX.Play(SFXIndex.SweetFestivalStart);
				break;
		}
		animate++;
		GameObject objText = SpawnStringEffect.GetSpawnEffectObject(effectTextType);
		objText.transform.position = Camera.main.transform.position;
		yield return null;
		if (effectTextType == SpawnStringEffectType.TextEffectSweetParty)
		{
			GameObject item = ContentAssistant.main.GetItem("Eff_Finale");
			if ((bool)item)
			{
				Transform transform = item.transform;
				Vector3 position = BoardManager.main.slotGroup.transform.position;
				transform.position = new Vector3(0f, position.y - BoardManager.slotoffset * 4f, 800f);
				UnityEngine.Object.Destroy(item, 2.2f);
				delayTime = 1.2f;
			}
		}
		PoolManager.PoolGameEffect.Despawn(objText.transform, showTextEffectBaseDelayTime);
		yield return new WaitForSeconds(delayTime);
		animate--;
	}
}
