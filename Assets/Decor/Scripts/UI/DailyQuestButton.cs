using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyQuestButton : MonoBehaviour
{
    public GameObject noticeObject;

    public void Awake()
    {
        noticeObject.SetActive(CheckQuestClaimAvailable());
    }

    public void ButtonPress()
    {
        var popup = Popup.PopupSystem.GetOpenBuilder().
                  SetType(PopupType.PopupDailyQuest).
                  SetCurrentPopupBehaviour(Popup.CurrentPopupBehaviour.Close).
                  Open();
        popup.PreAnimateHideEvent = () =>
        {
            noticeObject.SetActive(CheckQuestClaimAvailable());
        };
    }

    public bool CheckQuestClaimAvailable()
    {
        QuestData questData = PlayerData.current.questData;
        for (int i = 0; i < questData.allQuest.Count; i++)
        {
            Quest quest = questData.allQuest[i];

            if (quest.Claimed() == false && quest.IsCompleted() == true)
            {
                return true;
            }
        }

        return false;
    }
}
