using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIDailyBonusPack : MonoBehaviour
{
    public Button x2RewardButton;

    public Button claimButton;

    public Image selectImage;

    public GameObject claimedObject;

    public Text dayText;

    public Text comingsoonText;

    public void SetDay(int day)
    {
        dayText.text = CustomLocalization.Get("day") + " " + day.ToString();
    }

    public void SetCallback(UnityAction claimEvent, UnityAction x2rewardAction)
    {
        selectImage.GetComponent<Button>().onClick.AddListener(claimEvent);
        x2RewardButton.onClick.AddListener(x2rewardAction);
    }

    public void SetAsCurrent()
    {
        selectImage.gameObject.SetActive(true);
        claimedObject.SetActive(false);
        comingsoonText.gameObject.SetActive(false);
    }

    public void SetAsNext()
    {
        selectImage.gameObject.SetActive(false);
        claimedObject.SetActive(false);
        comingsoonText.gameObject.SetActive(true);
    }

    public void SetAsClaimed()
    {
        selectImage.gameObject.SetActive(false);
        claimedObject.SetActive(true);
        comingsoonText.gameObject.SetActive(false);
    }
}
