using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Decor
{
    public class DialogueController : MonoBehaviour
    {
        public DialogueView view;

        public Action StartEvent;

        public Action FinishEvent;

        private List<Dialogue> currentDialogueList;

        private int currentDialogueIndex;

        public void Initialize()
        {
            view.OnDialogueCloseAction = ShowNextDialogue;
            view.OnSkipAction = DialogueFinish;
        }

        public void Clear()
        {

        }

        public bool IsShowingDialogue()
        {
            return view.isActiveAndEnabled;
        }

        public void CombineCurrentWithDialogs(Dialogue[] dialogues)
        {
            currentDialogueList.AddRange(dialogues);
        }

        public void ShowDialogues(Dialogue[] dialogues, float delayTime = 0f)
        {
            if (dialogues != null && dialogues.Length > 0)
            {
                currentDialogueIndex = 0;
                currentDialogueList = dialogues.ToList();
                view.gameObject.SetActive(true);

                StartEvent?.Invoke();

                EventDispatcher<UITransitionEventId>.Instance.NotifyEvent(UITransitionEventId.ShowDialog);

                this.ExecuteAfterSeconds(delayTime, ShowNextDialogue);
            }
            else
            {
                DialogueFinish();
            }
        }

        private void ShowNextDialogue()
        {
            int dialogueCount = currentDialogueList.Count;
            if (currentDialogueIndex < dialogueCount)
            {
                Dialogue dialogue = currentDialogueList[currentDialogueIndex++];
                view.ShowDialogue(dialogue.GetContent(), dialogue.size == DialogueSide.Left ? -1 : 1,
                    currentDialogueIndex == 1, currentDialogueIndex == dialogueCount);
            }
            else
            {
                DialogueFinish();
            }
        }

        private void DialogueFinish()
        {
            view.Finish();

            EventDispatcher<UITransitionEventId>.Instance.NotifyEvent(UITransitionEventId.HideDialog);

            FinishEvent?.Invoke();
        }
    }
}
