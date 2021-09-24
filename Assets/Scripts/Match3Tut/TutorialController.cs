using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Match3
{
    public class TutorialController : MonoBehaviour
    {
        public Camera gameCamera;

        public Camera uiCamera;

        public TutorialView view;

        private static TutorialController current;

        private void Awake()
        {
            current = this;
        }

        private void SkipTutorial()
        {
            TutorialManager.Instance.ClearTask();

            view.Hide(false);
            OnComplete();
            view.ClearMasks();
        }
        public void OnDestroy()
        {
            GameMain.main.isTutorial = false;           
        }
        public void StartWithGID(int gid)
        {
           
                view = TutorialManager.Instance.tutorialView;
                view.skipButton.onClick.RemoveAllListeners();
                view.skipButton.onClick.AddListener(SkipTutorial);
                view.showSkipButton = true;

                OnStart();

                if (gid == 1)
                {
                    Debug.LogWarning("Tut1");
                    Tut_1();
                }
                else if (gid == 2)
                {
                    Debug.LogWarning("Tut 2");
                    Tut_2();
                }
                else if (gid == 3)
                {
                    Debug.LogWarning("Tut 3");
                    Tut_3();
                }
                else if (gid == 4)
                {
                    Debug.LogWarning("Tut 4");
                    Tut_4();
                }
                else if (gid == 5)
                {
                    Debug.LogWarning("Tut 5");
                    Tut_5();
                }
                else if (gid == 6)
                {
                    Debug.LogWarning("Tut 6");
                    Tut_6();
                }
                else if (gid == 8)
                {
                    Debug.LogWarning("Tut 8");
                    Tut_8();
                }
                else if (gid == 11)
                {
                    Debug.LogWarning("Tut 11");
                    Tut_11();
                }
                else if (gid == 13)
                {
                    Debug.LogWarning("Tut 13");
                    Tut_13();
                }
                else if (gid == 20)
                {
                    Debug.LogWarning("Tut 20");
                    Tut_20();
                }
                else if (gid == 22)
                {
                    Debug.LogWarning("Tut 22");
                    Tut_22();
                }
                else if (gid == 29)
                {
                    Debug.LogWarning("Tut 29");
                    Tut_29();
                }
                else
                {
                    OnComplete();
                }
            
        }

        private void Tut_1()
        {
            view.SetInvisibleActive();

            // MATCH 3 THE SAME GEMS
            Vector2 position = gameCamera.WorldToScreenPoint(BoardManager.main.GetSlot(4, 5).GetChip().transform.position);

            MoveTask moveTask = new MoveTask();
            moveTask.side = MoveTask.Side.Bottom;
            moveTask.startLocation = position;
            moveTask.maxBeginTouchDistance = 40f;
            moveTask.mininumDragMagnitude = ControlAssistant.swapMinimumMagnitude;
            moveTask.touchedPanel = view.blockingPanel;
            moveTask.startTime = 0.5f;
            moveTask.OnStart = () =>
            {
                OnStart();
                view.Show();
                view.SetHand(position, "TutorialHandMoveBottom");
                view.SetText("match3_tut_1_1");
                view.SetCharacterPosition(new Vector2(-155, -158));

                Vector2 p1 = GetSlotScreenPosition(4, 5);
                Vector2 p2 = GetSlotScreenPosition(3, 4);
                Vector2 p3 = GetSlotScreenPosition(4, 4);
                Vector2 p4 = GetSlotScreenPosition(5, 4);

                view.PlaceSlotCombinedMask(p1, new Vector2(0f, -1f), 90f);
                view.PlaceSlotCombinedMask(p2, new Vector2(1f, 0f));
                view.PlaceSlotCombinedMask(p3, new Vector2(0f, 0f));
                view.PlaceSlotCombinedMask(p4, new Vector2(-1f, 0f));
            };
            moveTask.OnTrigger = () =>
            {
                Chip chip1 = BoardManager.main.GetSlot(4, 5).GetChip();
                Chip chip2 = BoardManager.main.GetSlot(4, 4).GetChip();
                AnimationAssistant.main.SwapTwoItem(chip1, chip2, Side.Bottom);

                view.Hide(true);
                view.ClearMasks();
            };

            TutorialManager.Instance.AddTask(moveTask);

            // HIGHLIGHT THE GOALS

            TouchTask touchTask = new TouchTask();
            touchTask.touchedPanel = view.blockingPanel;
            touchTask.startTime = 1.75f;
            touchTask.OnStart = () =>
            {
                view.Show();
                view.SetText("match3_tut_1_2");
                view.SetCharacterPosition(new Vector2(-96, 17));

                Vector2 p = uiCamera.WorldToScreenPoint(Match3GameUI.Instance.progressInfoView.GoalGroupTransform.GetChild(0).position);
                 //   + Match3GameUI.Instance.progressInfoView.GoalGroupTransform.GetChild(1).position) * 0.5f);

                var slotTransform = view.PlaceCircleMask(p, 2.25f / 4f);
                Vector3 scale = slotTransform.localScale;
            };
            touchTask.OnTrigger = () =>
            {
                view.Hide(false);
                OnComplete();
                view.ClearMasks();
            };

            TutorialManager.Instance.AddTask(touchTask);
        }

        private void Tut_2()
        {
            view.SetInvisibleActive();
            // HORIZONTAL ROCKET
            Vector2 position = gameCamera.WorldToScreenPoint(BoardManager.main.GetSlot(4, 5).GetChip().transform.position);

            MoveTask moveTask = new MoveTask();
            moveTask.side = MoveTask.Side.Bottom;
            moveTask.startLocation = position;
            moveTask.maxBeginTouchDistance = 40f;
            moveTask.mininumDragMagnitude = ControlAssistant.swapMinimumMagnitude;
            moveTask.touchedPanel = view.blockingPanel;
            moveTask.startTime = 1.25f;
            moveTask.OnStart = () =>
            {
                OnStart();
                view.Show();
                view.SetHand(position, "TutorialHandMoveBottom");
                view.SetText("match3_tut_2_1");
                view.SetCharacterPosition(new Vector2(-289, -141));

                Vector2 p1 = GetSlotScreenPosition(3, 4);
                Vector2 p2 = GetSlotScreenPosition(4, 4);
                Vector2 p3 = GetSlotScreenPosition(5, 4);
                Vector2 p4 = GetSlotScreenPosition(6, 4);
                Vector2 p5 = GetSlotScreenPosition(4, 5);

                view.ClearMasks();
                view.PlaceSlotCombinedMask(p1, new Vector2(1f, 0f));
                view.PlaceSlotCombinedMask(p2, new Vector2(0f, 0f));
                view.PlaceSlotCombinedMask(p3, new Vector2(0f, 0f));
                view.PlaceSlotCombinedMask(p4, new Vector2(-1f, 0f));
                view.PlaceSlotCombinedMask(p5, new Vector2(0f, -1f), 90f);
            };
            moveTask.OnTrigger = () =>
            {
                Chip chip1 = BoardManager.main.GetSlot(4, 5).GetChip();
                Chip chip2 = BoardManager.main.GetSlot(4, 4).GetChip();
                AnimationAssistant.main.SwapTwoItem(chip1, chip2, Side.Bottom);

                view.Hide(false);

                OnComplete();
                view.ClearMasks();
            };

            TutorialManager.Instance.AddTask(moveTask);

            // VERTICAL ROCKET

            //Vector2 position2 = gameCamera.WorldToScreenPoint(BoardManager.main.GetSlot(3, 2).GetChip().transform.position);

            //moveTask = new MoveTask();
            //moveTask.side = MoveTask.Side.Left;
            //moveTask.startLocation = position2;
            //moveTask.maxBeginTouchDistance = 40f;
            //moveTask.mininumDragMagnitude = ControlAssistant.swapMinimumMagnitude;
            //moveTask.touchedPanel = view.blockingPanel;
            //moveTask.startTime = 1.25f;
            //moveTask.OnStart = () =>
            //{
            //    view.Show();
            //    view.SetHand(position2, "TutorialHandMoveLeft");
            //    view.SetText("Look! You can create another Rocket");
            //    view.SetCharacterPosition(new Vector2(-15, 135));

            //    Vector2 p1 = GetSlotScreenPosition(3, 2);
            //    Vector2 p2 = GetSlotScreenPosition(2, 0);
            //    Vector2 p3 = GetSlotScreenPosition(2, 1);
            //    Vector2 p4 = GetSlotScreenPosition(2, 2);
            //    Vector2 p5 = GetSlotScreenPosition(2, 3);

            //    view.ClearSlotMasks();
            //    view.PlaceSlotCombinedMask(p1, new Vector2(-1f, 0f));
            //    view.PlaceSlotCombinedMask(p2, new Vector2(0, +1f), 90f);
            //    view.PlaceSlotCombinedMask(p3, new Vector2(0, 0f), 90f);
            //    view.PlaceSlotCombinedMask(p4, new Vector2(0, 0f), 90f);
            //    view.PlaceSlotCombinedMask(p5, new Vector2(0, -1f), 90f);
            //};
            //moveTask.OnTrigger = () =>
            //{
            //    Chip chip1 = BoardManager.main.GetSlot(3, 2).GetChip();
            //    Chip chip2 = BoardManager.main.GetSlot(2, 2).GetChip();
            //    AnimationAssistant.main.SwapTwoItem(chip1, chip2, Side.Left);
            //    view.Hide(false);

            //    OnComplete();
            //    view.ClearSlotMasks();
            //};

            //TutorialManager.Instance.AddTask(moveTask);
        }

        private void Tut_3()
        {
            view.SetInvisibleActive();
            // RAINBOW - ROCKET

            Vector2 position2 = gameCamera.WorldToScreenPoint(BoardManager.main.GetSlot(4, 6).GetChip().transform.position);

            var moveTask = new MoveTask();
            moveTask.side = MoveTask.Side.Bottom;
            moveTask.startLocation = position2;
            moveTask.maxBeginTouchDistance = 40f;
            moveTask.mininumDragMagnitude = ControlAssistant.swapMinimumMagnitude;
            moveTask.touchedPanel = view.blockingPanel;
            moveTask.startTime = 1.25f;
            moveTask.OnStart = () =>
            {
                OnStart();
                view.Show();
                view.SetHand(position2, "TutorialHandMoveBottom");
                view.SetText("match3_tut_3_1");
                view.SetCharacterPosition(new Vector2(-326, -113));

                Vector2 p1 = GetSlotScreenPosition(4, 6);
                Vector2 p2 = GetSlotScreenPosition(4, 5);
                Vector2 p3 = GetSlotScreenPosition(4, 4);
                Vector2 p4 = GetSlotScreenPosition(4, 3);
                Vector2 p5 = GetSlotScreenPosition(3, 5);
                Vector2 p6 = GetSlotScreenPosition(5, 5);

                view.ClearMasks();
                view.PlaceSlotCombinedMask(p1, new Vector2(0f, -1f), 90f);
                view.PlaceSlotCombinedMask(p2, new Vector2(0f, 0f), 90f);
                view.PlaceSlotCombinedMask(p3, new Vector2(0f, 0f), 90f);
                view.PlaceSlotCombinedMask(p4, new Vector2(0f, +1f), 90f);
                view.PlaceSlotCombinedMask(p5, new Vector2(+1f, 0f), 0f);
                view.PlaceSlotCombinedMask(p6, new Vector2(-1f, 0f), 0f);
            };
            moveTask.OnTrigger = () =>
            {
                Chip chip1 = BoardManager.main.GetSlot(4, 6).GetChip();
                Chip chip2 = BoardManager.main.GetSlot(4, 5).GetChip();
                AnimationAssistant.main.SwapTwoItem(chip1, chip2, Side.Bottom);
                view.Hide(false);

                OnComplete();
                view.ClearMasks();
            };

            TutorialManager.Instance.AddTask(moveTask);
        }

        private void Tut_4()
        {
            view.SetInvisibleActive();
            // HIGHLIGHT THE JAILS

            TouchTask touchTask = new TouchTask();
            touchTask.touchedPanel = view.blockingPanel;
            touchTask.startTime = 1.25f;
            touchTask.OnStart = () =>
            {
                view.Show();
                view.SetText("match3_tut_4_1");
                view.SetCharacterPosition(new Vector2(-326, -133));

                view.PlaceSlotMask(GetSlotScreenPosition(4, 4));
                view.PlaceSlotMask(GetSlotScreenPosition(2, 4));
                view.PlaceSlotMask(GetSlotScreenPosition(2, 6));
                view.PlaceSlotMask(GetSlotScreenPosition(6, 4));
                view.PlaceSlotMask(GetSlotScreenPosition(6, 6));
            };
            touchTask.OnTrigger = () =>
            {
                view.Hide(true);
                view.ClearMasks();
            };

            TutorialManager.Instance.AddTask(touchTask);

            // CHOCOLATE JAIL

            Vector2 position2 = gameCamera.WorldToScreenPoint(BoardManager.main.GetSlot(5, 5).GetChip().transform.position);

            var moveTask = new MoveTask();
            moveTask.side = MoveTask.Side.Left;
            moveTask.startLocation = position2;
            moveTask.maxBeginTouchDistance = 40f;
            moveTask.mininumDragMagnitude = ControlAssistant.swapMinimumMagnitude;
            moveTask.touchedPanel = view.blockingPanel;
            moveTask.startTime = 1.5f;
            moveTask.OnStart = () =>
            {
                OnStart();
                view.Show();
                view.SetHand(position2, "TutorialHandMoveLeft");
                view.SetText("match3_tut_4_2");
                view.SetCharacterPosition(new Vector2(-326, -133));

                Vector2 p1 = GetSlotScreenPosition(5, 5);
                Vector2 p2 = GetSlotScreenPosition(4, 5);
                Vector2 p3 = GetSlotScreenPosition(4, 4);
                Vector2 p4 = GetSlotScreenPosition(4, 3);

                view.ClearMasks();
                view.PlaceSlotCombinedMask(p1, new Vector2(-1f, 0f), 0f);
                view.PlaceSlotCombinedMask(p2, new Vector2(0f, -1f), 90f);
                view.PlaceSlotCombinedMask(p3, new Vector2(0f, 0f), 90f);
                view.PlaceSlotCombinedMask(p4, new Vector2(0f, +1f), 90f);
            };
            moveTask.OnTrigger = () =>
            {
                Chip chip1 = BoardManager.main.GetSlot(5, 5).GetChip();
                Chip chip2 = BoardManager.main.GetSlot(4, 5).GetChip();
                AnimationAssistant.main.SwapTwoItem(chip1, chip2, Side.Bottom);
                view.Hide(false);

                OnComplete();
                view.ClearMasks();
            };

            TutorialManager.Instance.AddTask(moveTask);
        }

        private void Tut_5()
        {
            view.SetInvisibleActive();
            // SIMPLE BOMB

            Vector2 position2 = gameCamera.WorldToScreenPoint(BoardManager.main.GetSlot(4, 3).GetChip().transform.position);

            var moveTask = new MoveTask();
            moveTask.side = MoveTask.Side.Bottom;
            moveTask.startLocation = position2;
            moveTask.maxBeginTouchDistance = 40f;
            moveTask.mininumDragMagnitude = ControlAssistant.swapMinimumMagnitude;
            moveTask.touchedPanel = view.blockingPanel;
            moveTask.startTime = 1.5f;
            moveTask.OnStart = () =>
            {
                OnStart();
                view.Show();
                view.SetHand(position2, "TutorialHandMoveBottom");
                view.SetText("match3_tut_5_1");
                view.SetCharacterPosition(new Vector2(-313, 115));

                Vector2 p1 = GetSlotScreenPosition(2, 2);
                Vector2 p2 = GetSlotScreenPosition(3, 2);
                Vector2 p3 = GetSlotScreenPosition(4, 2);
                Vector2 p4 = GetSlotScreenPosition(4, 3);
                Vector2 p5 = GetSlotScreenPosition(4, 1);
                Vector2 p6 = GetSlotScreenPosition(4, 0);

                view.ClearMasks();
                view.PlaceSlotCombinedMask(p1, new Vector2(+1f, 0f), 0f);
                view.PlaceSlotCombinedMask(p2, new Vector2(0f, 0f), 0f);
                view.PlaceSlotCombinedMask(p3, new Vector2(-1f, 0f), 0f);
                view.PlaceSlotCombinedMask(p4, new Vector2(0f, -1f), 90f);
                view.PlaceSlotCombinedMask(p5, new Vector2(0f, 0f), 90f);
                view.PlaceSlotCombinedMask(p6, new Vector2(0f, +1f), 90f);
            };
            moveTask.OnTrigger = () =>
            {
                Chip chip1 = BoardManager.main.GetSlot(4, 3).GetChip();
                Chip chip2 = BoardManager.main.GetSlot(4, 2).GetChip();
                AnimationAssistant.main.SwapTwoItem(chip1, chip2, Side.Bottom);
                view.Hide(false);

                OnComplete();
                view.ClearMasks();
            };

            TutorialManager.Instance.AddTask(moveTask);
        }

        private void Tut_6()
        {
            view.SetInvisibleActive();
            // RAINBOW
            Vector2 position = gameCamera.WorldToScreenPoint(BoardManager.main.GetSlot(4, 8).GetChip().transform.position);

            MoveTask moveTask = new MoveTask();
            moveTask.side = MoveTask.Side.Bottom;
            moveTask.startLocation = position;
            moveTask.maxBeginTouchDistance = 40f;
            moveTask.mininumDragMagnitude = ControlAssistant.swapMinimumMagnitude;
            moveTask.touchedPanel = view.blockingPanel;
            moveTask.startTime = 1.5f;
            moveTask.OnStart = () =>
            {
                OnStart();
                view.Show();
                view.SetHand(position, "TutorialHandMoveBottom");
                view.SetText("match3_tut_6_1");
                view.SetCharacterPosition(new Vector2(-289, -141));

                Vector2 p1 = GetSlotScreenPosition(2, 7);
                Vector2 p2 = GetSlotScreenPosition(3, 7);
                Vector2 p3 = GetSlotScreenPosition(4, 7);
                Vector2 p4 = GetSlotScreenPosition(5, 7);
                Vector2 p5 = GetSlotScreenPosition(6, 7);
                Vector2 p6 = GetSlotScreenPosition(4, 8);

                view.ClearMasks();
                view.PlaceSlotCombinedMask(p1, new Vector2(1f, 0f));
                view.PlaceSlotCombinedMask(p2, new Vector2(0f, 0f));
                view.PlaceSlotCombinedMask(p3, new Vector2(0f, 0f));
                view.PlaceSlotCombinedMask(p4, new Vector2(1f, 0f));
                view.PlaceSlotCombinedMask(p5, new Vector2(-1f, 0f), 0f);
                view.PlaceSlotCombinedMask(p6, new Vector2(0f, -1f), 90f);
            };
            moveTask.OnTrigger = () =>
            {
                Chip chip1 = BoardManager.main.GetSlot(4, 8).GetChip();
                Chip chip2 = BoardManager.main.GetSlot(4, 7).GetChip();
                AnimationAssistant.main.SwapTwoItem(chip1, chip2, Side.Bottom);
                view.Hide(true);
            };

            TutorialManager.Instance.AddTask(moveTask);

            // COMBINE RAINBOW WITH NORMAL JEWEL

            Vector2 position2 = gameCamera.WorldToScreenPoint(BoardManager.main.GetSlot(4, 7).GetChip().transform.position);

            moveTask = new MoveTask();
            moveTask.side = MoveTask.Side.Bottom;
            moveTask.startLocation = position2;
            moveTask.maxBeginTouchDistance = 40f;
            moveTask.mininumDragMagnitude = ControlAssistant.swapMinimumMagnitude;
            moveTask.touchedPanel = view.blockingPanel;
            moveTask.startTime = 1.25f;
            moveTask.OnStart = () =>
            {
                view.Show();
                view.SetHand(position2, "TutorialHandMoveBottom");
                view.SetText("match3_tut_6_2");
                view.SetCharacterPosition(new Vector2(-268, -82));

                Vector2 p1 = GetSlotScreenPosition(4, 7);
                Vector2 p2 = GetSlotScreenPosition(4, 6);

                view.ClearMasks();
                view.PlaceSlotCombinedMask(p1, new Vector2(0f, -1f), 90);
                view.PlaceSlotCombinedMask(p2, new Vector2(0f, +1f), 90f);
            };
            moveTask.OnTrigger = () =>
            {
                Chip chip1 = BoardManager.main.GetSlot(4, 7).GetChip();
                Chip chip2 = BoardManager.main.GetSlot(4, 6).GetChip();
                AnimationAssistant.main.SwapTwoItem(chip1, chip2, Side.Bottom);
                view.Hide(false);

                OnComplete();
                view.ClearMasks();
            };

            TutorialManager.Instance.AddTask(moveTask);
        }

        private void Tut_8()
        {
            view.SetInvisibleActive();
            // FRUIT BOXES

            Vector2 position2 = gameCamera.WorldToScreenPoint(BoardManager.main.GetSlot(4, 6).GetChip().transform.position);

            var moveTask = new MoveTask();
            moveTask.side = MoveTask.Side.Bottom;
            moveTask.startLocation = position2;
            moveTask.maxBeginTouchDistance = 40f;
            moveTask.mininumDragMagnitude = ControlAssistant.swapMinimumMagnitude;
            moveTask.touchedPanel = view.blockingPanel;
            moveTask.startTime = 0.75f;
            moveTask.OnStart = () =>
            {
                OnStart();
                view.Show();
                view.SetHand(position2, "TutorialHandMoveBottom");
                view.SetText("match3_tut_8_1");
                view.SetCharacterPosition(new Vector2(-326, -133));

                view.ClearMasks();
                view.PlaceSlotCombinedMask(GetSlotScreenPosition(4, 6), new Vector2(0f, -1f), 90f);

                view.PlaceSlotCombinedMask(GetSlotScreenPosition(3, 5), new Vector2(1f, 0f), 0f);
                view.PlaceSlotCombinedMask(GetSlotScreenPosition(4, 5), new Vector2(0f, 0f), 0f);
                view.PlaceSlotCombinedMask(GetSlotScreenPosition(5, 5), new Vector2(-1f, 0f), 0f);

                view.PlaceSlotCombinedMask(GetSlotScreenPosition(3, 4), new Vector2(1f, 0f), 0f);
                view.PlaceSlotCombinedMask(GetSlotScreenPosition(4, 4), new Vector2(0f, 0f), 0f);
                view.PlaceSlotCombinedMask(GetSlotScreenPosition(5, 4), new Vector2(-1f, 0f), 0f);
            };
            moveTask.OnTrigger = () =>
            {
                Chip chip1 = BoardManager.main.GetSlot(4, 6).GetChip();
                Chip chip2 = BoardManager.main.GetSlot(4, 5).GetChip();
                AnimationAssistant.main.SwapTwoItem(chip1, chip2, Side.Bottom);
                view.Hide(false);

                OnComplete();
                view.ClearMasks();
            };

            TutorialManager.Instance.AddTask(moveTask);
        }

        private void Tut_22()
        {
            view.SetInvisibleActive();
            // JAM

            Vector2 position2 = gameCamera.WorldToScreenPoint(BoardManager.main.GetSlot(5, 2).GetChip().transform.position);

            var moveTask = new MoveTask();
            moveTask.side = MoveTask.Side.Bottom;
            moveTask.startLocation = position2;
            moveTask.maxBeginTouchDistance = 40f;
            moveTask.mininumDragMagnitude = ControlAssistant.swapMinimumMagnitude;
            moveTask.touchedPanel = view.blockingPanel;
            moveTask.startTime = 3.5f;
            moveTask.OnStart = () =>
            {
                OnStart();
                view.Show();
                view.SetHand(position2, "TutorialHandMoveBottom");
                view.SetText("match3_tut_22_1");
                view.SetCharacterPosition(new Vector2(-354, 133));

                view.ClearMasks();

                view.PlaceSlotCombinedMask(GetSlotScreenPosition(5, 2), new Vector2(0f, -1f), 90f);
                view.PlaceSlotCombinedMask(GetSlotScreenPosition(3, 1), new Vector2(1f, 0f), 0f);
                view.PlaceSlotCombinedMask(GetSlotScreenPosition(4, 1), new Vector2(0f, 0f), 0f);
                view.PlaceSlotCombinedMask(GetSlotScreenPosition(5, 1), new Vector2(-1f, 0f), 0f);
            };
            moveTask.OnTrigger = () =>
            {
                Chip chip1 = BoardManager.main.GetSlot(5, 2).GetChip();
                Chip chip2 = BoardManager.main.GetSlot(5, 1).GetChip();
                AnimationAssistant.main.SwapTwoItem(chip1, chip2, Side.Bottom);
                view.Hide(false);

                OnComplete();
                view.ClearMasks();
            };

            TutorialManager.Instance.AddTask(moveTask);
        }

        private void Tut_29()
        {
            view.SetInvisibleActive();
            // HIGHLIGHT BRINGDOWN

            TouchTask touchTask = new TouchTask();
            touchTask.touchedPanel = view.blockingPanel;
            touchTask.startTime = 0.25f;
            touchTask.OnStart = () =>
            {
                view.Show();
                view.SetText("match3_tut_29_1");
                view.SetCharacterPosition(new Vector2(-296, -144));

                view.PlaceSlotMask(GetSlotScreenPosition(4, 3));
                view.PlaceSlotMask(GetSlotScreenPosition(1, 8));
                view.PlaceSlotMask(GetSlotScreenPosition(7, 8));
            };
            touchTask.OnTrigger = () =>
            {
                view.Hide(true);
                view.ClearMasks();
            };

            TutorialManager.Instance.AddTask(touchTask);

            Vector2 position2 = gameCamera.WorldToScreenPoint(BoardManager.main.GetSlot(5, 0).GetChip().transform.position);

            var moveTask = new MoveTask();
            moveTask.side = MoveTask.Side.Left;
            moveTask.startLocation = position2;
            moveTask.maxBeginTouchDistance = 40f;
            moveTask.mininumDragMagnitude = ControlAssistant.swapMinimumMagnitude;
            moveTask.touchedPanel = view.blockingPanel;
            moveTask.startTime = 0.5f;
            moveTask.OnStart = () =>
            {
                OnStart();
                view.Show();
                view.SetHand(position2, "TutorialHandMoveLeft");
                view.SetText("match3_tut_29_2");
                view.SetCharacterPosition(new Vector2(-185, 102));

                Vector2 p1 = GetSlotScreenPosition(5, 0);
                Vector2 p2 = GetSlotScreenPosition(4, 0);
                Vector2 p3 = GetSlotScreenPosition(4, 1);
                Vector2 p4 = GetSlotScreenPosition(4, 2);

                view.ClearMasks();
                view.PlaceSlotCombinedMask(p1, new Vector2(-1f, 0f), 0f);
                view.PlaceSlotCombinedMask(p2, new Vector2(0f, 1f), 90f);
                view.PlaceSlotCombinedMask(p3, new Vector2(0f, 0f), 90f);
                view.PlaceSlotCombinedMask(p4, new Vector2(0f, -1f), 90f);
            };
            moveTask.OnTrigger = () =>
            {
                Chip chip1 = BoardManager.main.GetSlot(5, 0).GetChip();
                Chip chip2 = BoardManager.main.GetSlot(4, 0).GetChip();
                AnimationAssistant.main.SwapTwoItem(chip1, chip2, Side.Left);
                view.Hide(false);

                OnComplete();
                view.ClearMasks();
            };

            TutorialManager.Instance.AddTask(moveTask);
        }

        private void Tut_11()
        {
            
            // MAGIC HAMMER
            int boosterIdx = (int)BoosterType.Hammer;
            if (PlayerData.current.match3Data.ingameBooster[boosterIdx] <= 0 || PlayerData.current.match3Data.showTutorialIngameBooster[boosterIdx] == true)
            {
                OnComplete();
                return;
            }

            view.SetInvisibleActive();

            Booster booster = Match3GameUI.Instance.boosterPanelView.boosterUI[1];
            Button boosterButton = booster.GetComponent<Button>();

            var touchTask = new PressButtonTask();
            touchTask.triggerButton = view.triggerButton;
            touchTask.startTime = 1.25f;
            touchTask.OnStart = () =>
            {
                OnStart();

                Vector2 hammerButtonScreenPosition = uiCamera.WorldToScreenPoint(boosterButton.transform.position);
                view.Show();
                view.SetHand(hammerButtonScreenPosition, "TutorialHandTouch");
                view.SetText("match3_tut_11_1");
                view.SetCharacterPosition(new Vector2(-17f, 47f));

                view.ClearMasks();
                var circleMask = view.PlaceCircleMask(hammerButtonScreenPosition, 0.62f);
                view.SetTriggerInvisible(hammerButtonScreenPosition, circleMask.GetComponent<SpriteRenderer>());
            };
            touchTask.OnTrigger = () =>
            {
                boosterButton.onClick?.Invoke();

                view.Hide(true);
                view.ClearMasks();
            };

            TutorialManager.Instance.AddTask(touchTask);

            var touchTask2 = new PressButtonTask();
            touchTask2.triggerButton = view.triggerButton;
            touchTask2.startTime = 1f;
            touchTask2.OnStart = () =>
            {
                Vector2 p = GetSlotScreenPosition(4, 5);

                view.Show();
                view.SetHand(p, "TutorialHandTouch");
                view.SetText("match3_tut_11_2");
                view.SetCharacterPosition(new Vector2(-246f, -74f));

                var slotMask = view.PlaceSlotMask(p);
                view.SetTriggerInvisible(p, slotMask.GetComponent<SpriteRenderer>());                                
            };
            touchTask2.OnTrigger = () =>
            {
                booster.SetForceTarget(4, 5);

                view.Hide(false);
                view.ClearMasks();

                PlayerData.current.match3Data.showTutorialIngameBooster[(int)BoosterType.Hammer] = true;

                OnComplete();
            };

            TutorialManager.Instance.AddTask(touchTask2);
        }

        private void Tut_13()
        {          
            // HORIZONTAL ROCKET
            int boosterIdx = (int)BoosterType.HBomb;
            if (PlayerData.current.match3Data.ingameBooster[boosterIdx] <= 0 || PlayerData.current.match3Data.showTutorialIngameBooster[boosterIdx] == true)
            {
                OnComplete();
                return;
            }

            view.SetInvisibleActive();

            Booster booster = Match3GameUI.Instance.boosterPanelView.boosterUI[2];
            Button boosterButton = booster.GetComponent<Button>();

            var touchTask = new PressButtonTask();
            touchTask.triggerButton = view.triggerButton;
            touchTask.startTime = 1.25f;
            touchTask.OnStart = () =>
            {
                OnStart();

                Vector2 hammerButtonScreenPosition = uiCamera.WorldToScreenPoint(boosterButton.transform.position);
                view.Show();
                view.SetHand(hammerButtonScreenPosition, "TutorialHandTouch");
                view.SetText("match3_tut_13_1");
                view.SetCharacterPosition(new Vector2(-17f, 47f));

                view.ClearMasks();
                var circleMask = view.PlaceCircleMask(hammerButtonScreenPosition, 0.62f);
                view.SetTriggerInvisible(hammerButtonScreenPosition, circleMask.GetComponent<SpriteRenderer>());
            };
            touchTask.OnTrigger = () =>
            {
                boosterButton.onClick?.Invoke();

                view.Hide(true);
                view.ClearMasks();
            };
            
            TutorialManager.Instance.AddTask(touchTask);

            var touchTask2 = new PressButtonTask();
            touchTask2.triggerButton = view.triggerButton;
            touchTask2.startTime = 1f;
            touchTask2.OnStart = () =>
            {
                Vector2 p = GetSlotScreenPosition(4, 5);

                view.Show();
                view.SetHand(p, "TutorialHandTouch");
                view.SetText("match3_tut_13_2");
                view.SetCharacterPosition(new Vector2(-246f, -74f));

                var slotMask = view.PlaceSlotMask(p);
                view.SetTriggerInvisible(p, slotMask.GetComponent<SpriteRenderer>());
            };
            touchTask2.OnTrigger = () =>
            {
                booster.SetForceTarget(4, 5);

                view.Hide(false);
                view.ClearMasks();

                PlayerData.current.match3Data.showTutorialIngameBooster[(int)BoosterType.HBomb] = true;

                OnComplete();
            };

            TutorialManager.Instance.AddTask(touchTask2);
        }

        private void Tut_20()
        {          
            // HORIZONTAL ROCKET
            int boosterIdx = (int)BoosterType.CandyPack;
            if (PlayerData.current.match3Data.ingameBooster[boosterIdx] <= 0 || PlayerData.current.match3Data.showTutorialIngameBooster[boosterIdx] == true)
            {
                OnComplete();
                return;
            }

            view.SetInvisibleActive();

            Booster booster = Match3GameUI.Instance.boosterPanelView.boosterUI[4];
            Button boosterButton = booster.GetComponent<Button>();

            var touchTask = new PressButtonTask();
            touchTask.triggerButton = view.triggerButton;
            touchTask.startTime = 1.25f;
            touchTask.OnStart = () =>
            {
                OnStart();

                Vector2 hammerButtonScreenPosition = uiCamera.WorldToScreenPoint(boosterButton.transform.position);
                view.Show();
                view.SetHand(hammerButtonScreenPosition, "TutorialHandTouch");
                view.SetText("match3_tut_20_1");
                //view.SetText("<color=#93D400>Candy Bomb</color> just been unlocked, it can destroy all elements of a game board. Let's try it out.");
                view.SetCharacterPosition(new Vector2(-17f, 47f));

                view.ClearMasks();
                var circleMask = view.PlaceCircleMask(hammerButtonScreenPosition, 0.62f);
                view.SetTriggerInvisible(hammerButtonScreenPosition, circleMask.GetComponent<SpriteRenderer>());
            };
            touchTask.OnTrigger = () =>
            {
                boosterButton.onClick?.Invoke();

                view.Hide(true);
                view.ClearMasks();
            };

            TutorialManager.Instance.AddTask(touchTask);

            var touchTask2 = new PressButtonTask();
            touchTask2.triggerButton = view.triggerButton;
            touchTask2.startTime = 1f;
            touchTask2.OnStart = () =>
            {
                Vector2 p = GetSlotScreenPosition(4, 5);

                view.Show();
                view.SetHand(p, "TutorialHandTouch");
                view.SetText("match3_tut_20_2");
                view.SetCharacterPosition(new Vector2(-246f, -74f));

                var slotMask = view.PlaceSlotMask(p);
                view.SetTriggerInvisible(p, slotMask.GetComponent<SpriteRenderer>());
            };
            touchTask2.OnTrigger = () =>
            {
                booster.SetForceTarget(4, 5);

                view.Hide(false);
                view.ClearMasks();

                PlayerData.current.match3Data.showTutorialIngameBooster[(int)BoosterType.CandyPack] = true;

                OnComplete();
            };

            TutorialManager.Instance.AddTask(touchTask2);
        }    

        public void OnStart()
        {
            GameMain.main.isTutorial = true;
        }

        public void OnComplete()
        {
            current.ExecuteNextFrame(() => 
            {                   
                GameMain.main.isTutorial = false;
            });           
        }

        private Vector2 GetSlotScreenPosition(int x, int y)
        {
            return gameCamera.WorldToScreenPoint(BoardManager.main.GetSlot(x, y).transform.position);
        }
    }
}
