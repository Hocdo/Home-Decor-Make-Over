using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFreeGemAdReward : MonoBehaviour
{
    public Image selectImage;

    public Image bgImage;

    public Text rewardText;

    public GameObject claimedObject;

    public Image GetIconImage()
    {
        return transform.Find("Image").GetComponent<Image>();
    }

    public void SetAsCurrent(int rewardCount)
    {
        rewardText.gameObject.SetActive(true);
        bgImage.gameObject.SetActive(true);
        selectImage.gameObject.SetActive(true);
        claimedObject.SetActive(false);

        rewardText.text = "x" + rewardCount.ToString();
    }

    public void SetAsNext(int rewardCount)
    {
        rewardText.gameObject.SetActive(true);
        bgImage.gameObject.SetActive(true);
        selectImage.gameObject.SetActive(false);
        claimedObject.SetActive(false);

        rewardText.text = "x" + rewardCount.ToString();
    }

    public void SetAsClaimed()
    {
        rewardText.gameObject.SetActive(false);
        bgImage.gameObject.SetActive(false);
        selectImage.gameObject.SetActive(false);
        claimedObject.SetActive(true);
    }
}
