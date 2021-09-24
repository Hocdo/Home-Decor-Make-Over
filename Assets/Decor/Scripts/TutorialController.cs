using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Match3;

namespace Decor
{
    public class TutorialController : MonoBehaviour
    {
        public TutorialView view;

        [Header("First time enter")]
        public SelectedItemInfoUIView selectItemInfo;

        public Button variantButton;

        public Button applyButton;

        public Image coinIconImage;

        public Button playButton;

        [Header("First time complete decor")]
        public Button enterRoomPageButton;

        public RoomScrollPageView roomScrollPageView;

        private int dialogFinishCount = 0;

        public void Start()
        {           
            int match3Level = PlayerData.current.match3Data.level;

            if (match3Level == 15 || match3Level == 17)
            {
                Popup.PopupSystem.Instance.ShowPopupEvent += ScheduleHeadStartBooster;
            }
        }

        public void OnDestroy()
        {
            int match3Level = PlayerData.current.match3Data.level;

            if (match3Level == 15 || match3Level == 17)
            {
                if (!Popup.PopupSystem.IsInstanceDestroyed && Popup.PopupSystem.IsInstanceExisting)
                {
                    Popup.PopupSystem.Instance.ShowPopupEvent -= ScheduleHeadStartBooster;
                }
            }

            if (GameMain.main!= null)
            {
                GameMain.main.isTutorial = false;
            }   
        }

        private void SkipTutorial()
        {
            TutorialManager.Instance.ClearTask();           
            view.Hide(false);
        }

        public void OnDialogFinish()
        {
            dialogFinishCount++;

            if (dialogFinishCount == 2)
            {
                TouchTask task5 = new TouchTask();
                task5.startTime = 0.35f;
                task5.touchedPanel = view.blockingPanel;
                task5.OnPredicateTrue = () =>
                {
                    view.SetInvisibleActive();
                };
                task5.OnStart = () =>
                {
                    view.HighlightTarget(coinIconImage.GetComponent<RectTransform>(), new Vector2(2.5f, 0.5f));
                    view.SetTextColor("room1_start_tut_5", "93D400");
                    view.Show();
                    view.SetCharacterPosition(new Vector2(110, 110));
                };
                task5.OnTrigger = () => { };

                PressButtonTask task6 = new PressButtonTask();
                task6.startTime = 0.2f;
                task6.triggerButton = view.triggerButton;
                task6.OnStart = () =>
                {
                    view.HighlightTarget(playButton.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector3(0f, 0.5f, 0f));
                    view.SetHand(playButton.transform);
                    view.SetText("room1_start_tut_6");
                    view.SetCharacterPosition(new Vector2(110, 110));
                };
                task6.OnTrigger = () =>
                {
                    playButton.onClick.Invoke();
                    view.Hide(false);
                };
            }
        }

        public void ScheduleFirstTime_EnterLivingRoom(IconTouchHandler targetIconTouchHandler)
        {
            PressButtonTask task1 = new PressButtonTask();
            task1.startTime = 0.5f;
            task1.triggerButton = view.triggerButton;
            task1.Predicate = () => dialogFinishCount == 1;
            task1.OnPredicateTrue = () =>
            {
                view.SetInvisibleActive();
            };
            task1.OnStart = () =>
            {
                
                view.HighlightTarget(targetIconTouchHandler.GetComponent<SpriteRenderer>(), new Vector2(2.5f, 2.5f));
                view.SetHand(targetIconTouchHandler.transform);
                view.SetTextColor("room1_start_tut_1", "93D400");
                view.SetCharacterPosition(new Vector2(140, 130));
                view.Show();
            };
            task1.OnTrigger = () =>
            {
                targetIconTouchHandler.OnPointerClickAction?.Invoke();
                view.Hide(true);
            };

            PressButtonTask task2 = new PressButtonTask();
            task2.startTime = 0.5f;
            task2.triggerButton = view.triggerButton;
            task2.Predicate = () => selectItemInfo.isActiveAndEnabled;
            task2.OnStart = () =>
            {
                view.HighlightTarget(selectItemInfo.GetComponent<RectTransform>(), new Vector2(2f, 2f)/*, new Vector3(-3.1f,0f,0f)*/);
                view.SetHand(selectItemInfo.transform, new Vector2(6f, -4f));
                view.SetTextColor("room1_start_tut_2", "93D400");
                view.SetCharacterPosition(new Vector2(110, 110));
                view.Show();
            };
            task2.OnTrigger = () =>
            {
                selectItemInfo.OnPointerClick(null);
                view.Hide(true);
            };

            PressButtonTask task3 = new PressButtonTask();
            task3.startTime = 0.5f;
            task3.triggerButton = view.triggerButton;
            task3.Predicate = () => variantButton.isActiveAndEnabled;
            task3.OnStart = () =>
            {
                view.HighlightTarget(variantButton.GetComponent<RectTransform>(), new Vector2(1.5f, 1.5f));
                view.SetHand(variantButton.transform, new Vector2(3f, -3f));
                view.SetTextColor("room1_start_tut_3", "93D400");
                view.SetCharacterPosition(new Vector2(110, 110));
                view.Show();

            };
            task3.OnTrigger = () =>
            {
                variantButton.onClick.Invoke();
                //view.Hide(true);
            };

            PressButtonTask task4 = new PressButtonTask();
            task4.startTime = 0f;
            task4.triggerButton = view.triggerButton;
            task4.Predicate = () => applyButton.isActiveAndEnabled;
            task4.OnStart = () =>
            {
                view.HighlightTarget(applyButton.GetComponent<RectTransform>(), new Vector2(1.5f, 1.5f));
                view.SetHand(applyButton.transform, new Vector2(1f, -1f));
                view.SetText("room1_start_tut_4");
                view.SetCharacterPosition(new Vector2(110, 110));
            };
            task4.OnTrigger = () =>
            {
                applyButton.onClick.Invoke();
                view.Hide(false);
            };
        }

        public void ScheduleFirstTime_CompleteLivingRoom()
        {
            PressButtonTask task1 = new PressButtonTask();
            task1.startTime = 0.5f;
            task1.triggerButton = view.triggerButton;
            task1.OnPredicateTrue = () =>
            {
                view.SetInvisibleActive();
            };
            task1.OnStart = () =>
            {
                view.HighlightTarget(enterRoomPageButton.GetComponent<RectTransform>(), new Vector2(1f, 1f));
                view.SetHand(enterRoomPageButton.transform);
                view.SetText("room1_end_tut_1");
                view.SetCharacterPosition(new Vector2(186, 240));
                view.Show();
            };
            task1.OnTrigger = () =>
            {
                enterRoomPageButton.onClick?.Invoke();
                view.Hide(true);
            };

            PressButtonTask task2 = new PressButtonTask();
            task2.startTime = 0.5f;
            task2.triggerButton = view.triggerButton;
            task2.Predicate = () => roomScrollPageView.GetRoomPageElement(1) != null;
            task2.OnStart = () =>
            {
                Button roomButton = roomScrollPageView.GetRoomPageElement(1).button;

                view.HighlightTarget(roomButton.GetComponent<RectTransform>(), new Vector2(1f, 1f));
                view.SetHand(roomButton.transform);
                view.SetTextColor("room1_end_tut_2", "93D400");
                view.SetCharacterPosition(new Vector2(122, 310));
                view.Show();
            };
            task2.OnTrigger = () =>
            {
                roomScrollPageView.GetRoomPageElement(1).button.onClick?.Invoke();
                view.Hide(true);
            };

            PressButtonTask task3 = new PressButtonTask();
            task3.startTime = 0.5f;
            task3.triggerButton = view.triggerButton;
            task3.OnStart = () =>
            {
                view.HighlightTarget(roomScrollPageView.enterButton.GetComponent<RectTransform>(), new Vector2(1f, 1f));
                view.SetHand(roomScrollPageView.enterButton.transform);
                view.SetText("room1_end_tut_3");
                view.SetCharacterPosition(new Vector2(293, 163));
                view.Show();
            };
            task3.OnTrigger = () =>
            {
                roomScrollPageView.enterButton.onClick.Invoke();
                view.Hide(false);
            };
        }

        private void ScheduleHeadStartBooster(Popup.PopupBase popup)
        {
            if (popup.type == PopupType.PopupGameStart)
            {
                var popupGameStart = popup as PopupGameStart;

                int match3Level = PlayerData.current.match3Data.level;

                if (match3Level == 15)
                {
                    Tut_Lv15(popupGameStart);
                }
                else if (match3Level == 17)
                {
                    Tut_Lv17(popupGameStart);
                }
            }
        }

        private void Tut_Lv15(PopupGameStart popupStart)
        {            
            int boosterIdx = (int)HeadStartBoosterType.Rocket;
            if (PlayerData.current.match3Data.headStartBoosterCounts[boosterIdx] <= 0 || PlayerData.current.match3Data.showTutorialStartBooster[boosterIdx] == true)
            {
                return;
            }

            var tutorialView = TutorialManager.Instance.tutorialView;
            tutorialView.SetInvisibleActive();
            tutorialView.showSkipButton = false;

            Button boosterButton = popupStart.headStartBoosterViews[boosterIdx].iconImage.GetComponent<Button>();

            var touchTask = new PressButtonTask();
            touchTask.triggerButton = tutorialView.triggerButton;
            touchTask.startTime = 1f;
            touchTask.OnStart = () =>
            {
                Vector2 rocketScrPos = Popup.PopupSystem.Instance.popupCamera.WorldToScreenPoint(boosterButton.transform.position);
                tutorialView.Show();
                tutorialView.SetHand(rocketScrPos, "TutorialHandTouch");
                tutorialView.SetText("headstar_15_1");
                tutorialView.SetCharacterPosition(new Vector2(-390f, 117f));

                tutorialView.ClearMasks();
                var circleMask = tutorialView.PlaceCircleMask(rocketScrPos, 75f/108f);
                tutorialView.SetTriggerInvisible(rocketScrPos, circleMask.GetComponent<SpriteRenderer>());
            };
            touchTask.OnTrigger = () =>
            {
                boosterButton.onClick?.Invoke();

                tutorialView.Hide(true);
                tutorialView.ClearMasks();
            };

            TutorialManager.Instance.AddTask(touchTask);

            Button playButton = popupStart.transform.Find("ButtonPlay").GetComponent<Button>();

            var touchTask2 = new PressButtonTask();
            touchTask2.triggerButton = tutorialView.triggerButton;
            touchTask2.startTime = 1f;
            touchTask2.OnStart = () =>
            {
                Vector2 startButtonScrPos = Popup.PopupSystem.Instance.popupCamera.WorldToScreenPoint(playButton.transform.position);
                tutorialView.Show();
                tutorialView.SetHand(startButtonScrPos, "TutorialHandTouch");
                tutorialView.SetText("headstar_15_2");
                tutorialView.SetCharacterPosition(new Vector2(-390f, 117f));

                tutorialView.ClearMasks();
                var circleMask = tutorialView.PlaceImageMask(startButtonScrPos, playButton.GetComponent<Image>());
                tutorialView.SetTriggerInvisible(startButtonScrPos, playButton.GetComponent<Image>());
            };
            touchTask2.OnTrigger = () =>
            {
                playButton.onClick?.Invoke();
                tutorialView.Hide(false);
                tutorialView.ClearMasks();

                PlayerData.current.match3Data.showTutorialStartBooster[(int)Match3.HeadStartBoosterType.Rocket] = true;
            };

            TutorialManager.Instance.AddTask(touchTask2);
        }

        private void Tut_Lv17(PopupGameStart popupStart)
        {
            int boosterIdx = (int)HeadStartBoosterType.Bomb;
            if (PlayerData.current.match3Data.headStartBoosterCounts[boosterIdx] <= 0 || PlayerData.current.match3Data.showTutorialStartBooster[boosterIdx] == true)
            {
                return;
            }

            var tutorialView = TutorialManager.Instance.tutorialView;
            tutorialView.SetInvisibleActive();
            tutorialView.showSkipButton = false;

            Button boosterButton = popupStart.headStartBoosterViews[boosterIdx].iconImage.GetComponent<Button>();

            var touchTask = new PressButtonTask();
            touchTask.triggerButton = tutorialView.triggerButton;
            touchTask.startTime = 1f;
            touchTask.OnStart = () =>
            {
                Vector2 rocketScrPos = Popup.PopupSystem.Instance.popupCamera.WorldToScreenPoint(boosterButton.transform.position);
                tutorialView.Show();
                tutorialView.SetHand(rocketScrPos, "TutorialHandTouch");
                tutorialView.SetText("headstar_17_1");
                tutorialView.SetCharacterPosition(new Vector2(-390f, 117f));

                tutorialView.ClearMasks();
                var circleMask = tutorialView.PlaceCircleMask(rocketScrPos, 75f / 108f);
                tutorialView.SetTriggerInvisible(rocketScrPos, circleMask.GetComponent<SpriteRenderer>());
            };
            touchTask.OnTrigger = () =>
            {
                boosterButton.onClick?.Invoke();

                tutorialView.Hide(true);
                tutorialView.ClearMasks();
            };

            TutorialManager.Instance.AddTask(touchTask);

            Button playButton = popupStart.transform.Find("ButtonPlay").GetComponent<Button>();

            var touchTask2 = new PressButtonTask();
            touchTask2.triggerButton = tutorialView.triggerButton;
            touchTask2.startTime = 1f;
            touchTask2.OnStart = () =>
            {
                Vector2 startButtonScrPos = Popup.PopupSystem.Instance.popupCamera.WorldToScreenPoint(playButton.transform.position);
                tutorialView.Show();
                tutorialView.SetHand(startButtonScrPos, "TutorialHandTouch");
                tutorialView.SetText("headstar_17_2");
                tutorialView.SetCharacterPosition(new Vector2(-390f, 117f));

                tutorialView.ClearMasks();
                var circleMask = tutorialView.PlaceImageMask(startButtonScrPos, playButton.GetComponent<Image>());
                tutorialView.SetTriggerInvisible(startButtonScrPos, playButton.GetComponent<Image>());
            };
            touchTask2.OnTrigger = () =>
            {
                playButton.onClick?.Invoke();
                tutorialView.Hide(false);
                tutorialView.ClearMasks();

                PlayerData.current.match3Data.showTutorialStartBooster[(int)Match3.HeadStartBoosterType.Bomb] = true;
            };

            TutorialManager.Instance.AddTask(touchTask2);
        }
    }
}