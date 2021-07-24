using System;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Threading;

namespace Decor
{
    public class GameDecorController : MonoBehaviour
    {
        public CameraSetup cameraSetup;

        public DesignController designController;

        public TutorialController tutorialController;

        public DialogueController dialogueController;

        public PostDecorCompleteEffect postDecorCompleteEffect;

        public GameUITransitionDirector uiTransitionDirector;

        public Camera uiCamera;

        [Header("Random Dialogs")]
        public DialogueData randomUnlockDialogs;

        private RoomData roomData;

        private UnlockedRoomData currentRoomData;

        private DesignItemView[] items;

        private IconTouchHandler tutorialIconTouchHandler;

        private int randomUnlockDialogsCount = 0;

        void Awake()
        {
            //Model.Instance.Load();

            PlayerData playerData = PlayerData.current;

            roomData = RoomDataTable.Instance.GetRoomDataWithId(playerData.homeDesignData.currentRoomId);
            currentRoomData = playerData.homeDesignData.GetCurrentUnlockedRoomData();
            currentRoomData.enterTimeCount++;

            // load room asset bundle -> load room prefab -> load all items
            var roomAssetBundle = roomData.GetAssetBundle();
            var roomGameobject = Instantiate(roomAssetBundle.LoadAsset<GameObject>(RoomData.RoomPrefabName));
            var itemsParent = roomGameobject.transform.Find(RoomData.ItemsOfRoomName).transform;

            var roomFitScreen = roomGameobject.GetComponent<RoomFitScreen>();
            if (roomFitScreen)
            {
                cameraSetup.Execute(
                    roomFitScreen.minCameraRatio, roomFitScreen.maxCameraOrthoSize,
                    roomFitScreen.maxCameraRatio, roomFitScreen.minCameraOrthoSize);
            }
            else
            {
                cameraSetup.Execute();
            }

            int itemCount = itemsParent.childCount;
            if (roomData.maxItemCount != itemCount)
            {
                Debug.LogWarning("Room Data Max Item Count and Item Count are not match");
                roomData.maxItemCount = itemCount;
            }    

            items = new DesignItemView[itemCount];
            DesignItemData[] itemDataArray = new DesignItemData[itemCount];
            for (int i = 0; i < itemCount; i++)
            {
                items[i] = itemsParent.GetChild(i).GetComponent<DesignItemView>();
                items[i].GetIcon().GetComponent<SpriteRenderer>().sortingLayerName = "Icon";
                itemDataArray[i] = items[i].primaryData;

                if (itemDataArray[i].unlockedCountToUnlock >= 100)
                    itemDataArray[i].unlockedCountToUnlock = 0;

                // Chose "Sofa" for tutorial
                if (items[i].primaryData.displayName.Equals("Sofa"))
                {
                    tutorialIconTouchHandler = items[i].GetIcon().GetComponent<IconTouchHandler>();
                }
            }

            Model.Instance.UpdateCurrentRoomItemData(itemDataArray);
            
            // setup and bind dependencies
            designController.Initialize(items);
            designController.roomName = roomData.name;
            designController.IsTouchEnabled = true;
            PrepareBgTextureEvent = () => designController.SetActiveIconsVisible(false, false);

            dialogueController.Initialize();

            uiTransitionDirector.SetActiveIconsVisible = designController.SetActiveIconsVisibleWithAnim;

            designController.UnlockItemFinishEvent = OnItemUnlocked;
            designController.UnlockItemFailedEvent = (item) => 
            {
                Popup.PopupSystem.Instance.ShowPopup(PopupType.PopupRequirePlayMatch3, Popup.CurrentPopupBehaviour.Close, true, true)
                .AcceptEvent = (param) => ShowPopupMatch3Preparing();
            };

            // check if next room can be unlocked (in case we update database)
            CheckCanUnlockNextRoom();  
        }        
        private void Start()
        {
            var playerData = PlayerData.current;

            EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.CoinChange, playerData.cointCount);
            EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.GemChange, playerData.gemCount);

            // Show BEGIN DIALOGS and TUTORIAL if there is NO item unlocked and the user have NOT enter this room YET
            if (currentRoomData.boughtItemData.Count == 0 && currentRoomData.enterTimeCount == 1)
            {
                if (Model.Instance.playRoomData.id == 1)
                {
                    ShowStartDialogs();

                    dialogueController.FinishEvent += tutorialController.OnDialogFinish;
                    tutorialController.ScheduleFirstTime_EnterLivingRoom(tutorialIconTouchHandler);
                }              
            }
        
            if (!AudioManager.Instance.musicSource.isPlaying)
                AudioManager.Instance.PlayMusic(AudioClipId.DecorMusic);

            AudioManager.Instance.CrossIn(1f);

            if (!Popup.PopupSystem.IsInstanceDestroyed && Popup.PopupSystem.IsInstanceExisting)
            {
                Popup.PopupSystem.Instance.ShowPopupEvent += OnPopupShow;
                Popup.PopupSystem.Instance.ClearPopupEvent += OnPopupClear;
            }

            HandleStartPopups();
        }

        private void OnDestroy()
        {
            if (!Popup.PopupSystem.IsInstanceDestroyed && Popup.PopupSystem.IsInstanceExisting)
            {
                Popup.PopupSystem.Instance.ShowPopupEvent -= OnPopupShow;
                Popup.PopupSystem.Instance.ClearPopupEvent -= OnPopupClear;
            }

            EventDispatcher<UITransitionEventId>.Instance.ClearEvent();
            EventDispatcher<GlobalEventId>.Instance.ClearEvent();

            roomData.UnloadAssetBundle(true);

            //AudioManager.Instance.StopMusic();
            PrepareBgTextureEvent = null;
        }

        private void OnPopupShow(Popup.PopupBase popup)
        {
            uiTransitionDirector.ForceCloseAllView(false);
            designController.SetActiveIconsVisibleWithAnim(false);
        }

        private void OnPopupClear()
        {
            uiTransitionDirector.ForceCloseAllView(true);
            designController.SetActiveIconsVisibleWithAnim(true);
        }

        private void OnItemUnlocked(DesignItemView item)
        {
            CheckCanUnlockNextRoom();

            bool playUnlockDialog = true;

            if (!ShowUnlockItemDialogs(item))
            {
                if (randomUnlockDialogsCount == 0)
                {
                    playUnlockDialog = ShowRandomUnlockDialogs();
                    randomUnlockDialogsCount = UnityEngine.Random.Range(1, 3);
                }
                else
                {
                    randomUnlockDialogsCount--;
                    playUnlockDialog = false;                   
                }
            }

            if (designController.IsAllItemsUnlock())
            {
                if (currentRoomData.roomId == 1)
                {
                    // AppEventTracker.PushEventFinishRoom1();
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("Feature_selected_unlockroom1");
                }

                if (playUnlockDialog)
                {
                    dialogueController.CombineCurrentWithDialogs(roomData.completeDialogs);                        
                    dialogueController.FinishEvent = PlayPostDecorCompleteEffect;
                }                    
                else
                {
                    if (ShowEndDialogs())
                        dialogueController.FinishEvent = PlayPostDecorCompleteEffect;
                    else
                        PlayPostDecorCompleteEffect();
                }
            }
        }

        private void HandleStartPopups()
        {
            //#if UNITY_EDITOR
            //            Popup.PopupSystem.GetOpenBuilder().
            //                            SetType(PopupType.PopupDailyBonus).
            //                            Open();
            //#endif
//#if UNITY_EDITOR
//            {
//                Popup.PopupSystem.GetOpenBuilder().
//                SetType(PopupType.PopupDailyBonus).
//                SetCurrentPopupBehaviour(Popup.CurrentPopupBehaviour.HideTemporary).
//                SetDelayTime(1.5f).
//                SetBackBlockerEvent(null).
//                Open();

//                return;
//            }
//#endif

            //#if !UNITY_EDITOR
            if (GameMain.played == false) return;
//#endif


            var playerData = PlayerData.current;

            bool monthlyPackAvailable = MonthlyPackUtility.Available();
            bool weeklyPackAvailable = WeeklyPackUtility.Available();
            bool starterPackAvailable = StarterPackUtility.Available();

            bool dailyBonusAvailable = DailyBonusUtility.Available() && playerData.match3Data.level >= 4;

            bool ratingAvailable = playerData.appRated == false && (playerData.match3Data.level + 20 - 6) % 20 == 0;

            if (monthlyPackAvailable && playerData.match3Data.level == MonthlyPackUtility.match3LevelToShow)
            {
                Popup.PopupSystem.GetOpenBuilder().
                    SetType(PopupType.PopupMonthlyPack).
                    SetCurrentPopupBehaviour(Popup.CurrentPopupBehaviour.HideTemporary).
                    SetDelayTime(1.5f).
                    Open();

                return;
            }

            if (weeklyPackAvailable && playerData.match3Data.level == WeeklyPackUtility.match3LevelToShow)
            {
                Popup.PopupSystem.GetOpenBuilder().
                    SetType(PopupType.PopupWeeklyPack).
                    SetCurrentPopupBehaviour(Popup.CurrentPopupBehaviour.HideTemporary).
                    SetDelayTime(1.5f).
                    Open();

                return;
            }

            if (starterPackAvailable && playerData.match3Data.level == StarterPackUtility.match3LevelToShow)
            {
                Popup.PopupSystem.GetOpenBuilder().
                    SetType(PopupType.PopupStarterPack).
                    SetCurrentPopupBehaviour(Popup.CurrentPopupBehaviour.HideTemporary).
                    SetDelayTime(1.5f).
                    Open();

                return;
            }

            if (ratingAvailable)
            {
                Popup.PopupSystem.GetOpenBuilder().
                    SetType(PopupType.PopupRating).
                    SetCurrentPopupBehaviour(Popup.CurrentPopupBehaviour.HideTemporary).
                    SetBackBlockerEvent(null).
                    SetDelayTime(1.5f).Open();
            }
            else if (dailyBonusAvailable)
            {
                Popup.PopupSystem.GetOpenBuilder().
                SetType(PopupType.PopupDailyBonus).
                SetCurrentPopupBehaviour(Popup.CurrentPopupBehaviour.HideTemporary).
                SetDelayTime(1.5f).
                SetBackBlockerEvent(null).
                Open();
            }
            else
            {
                int prob = UnityEngine.Random.Range(0, 100);
                if (prob < 50)
                {
                    if (ServiceUtility.InternetAvailable)
                        Popup.PopupSystem.GetOpenBuilder().SetType(PopupType.PopupFreeGemAds).SetCurrentPopupBehaviour(Popup.CurrentPopupBehaviour.HideTemporary).SetDelayTime(1.5f).Open();
                }
                else if (prob < 80)
                {
                    List<PopupType> availablePopupTypes = new List<PopupType>();
                    if (monthlyPackAvailable) availablePopupTypes.Add(PopupType.PopupMonthlyPack);
                    if (weeklyPackAvailable) availablePopupTypes.Add(PopupType.PopupWeeklyPack);
                    if (starterPackAvailable) availablePopupTypes.Add(PopupType.PopupStarterPack);

                    if (availablePopupTypes.Count > 0)
                    {
                        Popup.PopupSystem.GetOpenBuilder().
                            SetType(availablePopupTypes[UnityEngine.Random.Range(0, availablePopupTypes.Count)]).
                            SetCurrentPopupBehaviour(Popup.CurrentPopupBehaviour.HideTemporary).
                            SetDelayTime(1.5f).
                            Open();
                    }              
                }
            }
        }

        private bool ShowStartDialogs()
        {
            if (roomData.startDialogs != null && roomData.startDialogs.Length > 0)
            {
                dialogueController.ShowDialogues(roomData.startDialogs, 1.5f);
                return true;
            }

            return false;
        }

        private bool ShowEndDialogs()
        {
            if (roomData.completeDialogs != null && roomData.completeDialogs.Length > 0)
            {
                dialogueController.ShowDialogues(roomData.completeDialogs, 1.5f);
                return true;
            }

            return false;
        }

        private bool ShowRandomUnlockDialogs()
        {     
            dialogueController.ShowDialogues(new Dialogue[] { randomUnlockDialogs.GetRandom() }, 1f);

            return true;
        }
        
        private bool ShowUnlockItemDialogs(DesignItemView item)
        {
            var itemDialogComponent = item.GetComponent<DesignItemDialogue>();

            if (itemDialogComponent != null && itemDialogComponent.dialogues != null && itemDialogComponent.dialogues.Length != 0)
            {
                dialogueController.ShowDialogues(itemDialogComponent.dialogues, 1.0f);
                return true;
            }

            return false;
        }

        private void PlayPostDecorCompleteEffect()
        {
            AudioSource musicSource = AudioManager.Instance.musicSource;

            // pause music
            AudioManager.Instance.CrossOut(1f);

            designController.IsTouchEnabled = false;

            this.ExecuteAfterSeconds(0.15f, () => uiTransitionDirector.ForceCloseAllView(false));

            Action PlayCompleteEvent = () =>
            {
                designController.IsTouchEnabled = true;
                uiTransitionDirector.ForceCloseAllView(true);
                dialogueController.FinishEvent = null;

                AudioManager.Instance.CrossIn(3f).SetDelay(1.5f);

                if (Model.Instance.playRoomData.id == 1)
                {
                    tutorialController.ScheduleFirstTime_CompleteLivingRoom();
                }
            };

            postDecorCompleteEffect.Setup(items);
            postDecorCompleteEffect.Play(0.25f, PlayCompleteEvent);
        }

        private void CheckCanUnlockNextRoom()
        {
            if (roomData.nextRoomId > 0 && designController.GetUnlockedItemCount() >= roomData.maxItemCount)
            {
                PlayerData.current.homeDesignData.SetUnlockRoom(roomData.nextRoomId);
            }
        }

        private void ShowPopupMatch3Preparing()
        {
            if (PlayerData.current.stamina.Available())
            {
                Popup.PopupSystem.GetOpenBuilder()
               .SetType(PopupType.PopupGameStart)
               .Open();
            }
            else
            {
                Popup.PopupSystem.GetOpenBuilder()
              .SetType(PopupType.PopupStaminaStore)
              .Open();
            }        
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && Popup.PopupSystem.Instance.IsShowingPopup() == false)
                Popup.PopupSystem.GetOpenBuilder()
                .SetType(PopupType.PopupQuitGame)
                .Open();
        }

       
        private static Action PrepareBgTextureEvent;

        public static void PrepareBgTexture()
        {
            PrepareBgTextureEvent?.Invoke();

            Camera mainCamera = Camera.main;
            mainCamera.targetTexture = DecorBlurImageManager.GetSourceTexture();
            mainCamera.Render();
            mainCamera.targetTexture = null;
        }
    }
}
