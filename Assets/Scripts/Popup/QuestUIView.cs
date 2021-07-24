using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUIView : MonoBehaviour
{
    public GameObject gemIcon;

    public Text descriptionText;

    public Text progressText;

    public Text rewardText;

    public Image progressBar;

    public Button claimButton;

    public Button changeButton;

    public GameObject tick;

    private int index;

    public void OnQuestClaimed()
    {
        tick.gameObject.SetActive(true);
        changeButton.gameObject.SetActive(false);
        claimButton.gameObject.SetActive(false);       
    }

    public void SetupWithQuest(Quest quest, int idx, Action<int> ClaimAction, Action<int> RefreshAction)
    {
        index = idx;
        var desc = quest.GetQuestDescription();

        descriptionText.text = desc[0].ToString();
        rewardText.text = "x" + quest.GetConfig().rewardGemCount.ToString();

        descriptionText.font = CustomLocalization.GetFont();

        int a = (int)desc[1];
        int b = (int)desc[2];

        progressBar.fillAmount = (float)a / b;
        progressText.text = a.ToString() + "/" + b.ToString();

        bool questCompleted = quest.IsCompleted();

        changeButton.gameObject.SetActive(!questCompleted);
        claimButton.gameObject.SetActive(questCompleted);
        tick.gameObject.SetActive(false);

        claimButton.onClick.RemoveAllListeners();
        claimButton.onClick.AddListener(() => ClaimAction(index));

        changeButton.onClick.RemoveAllListeners();
        changeButton.onClick.AddListener(() => RefreshAction(index));
    }
}
