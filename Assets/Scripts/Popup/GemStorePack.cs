using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GemStorePack : MonoBehaviour
{
    [Header("Data")]
    public string productId;

    public int rewardCount;

    public float priceInUsd;

    [Header("Refs")]
    public Button buyButton;

    public Text priceText;

    public Action<GemStorePack> BuyEvent;

    public Image GetIconImage()
    {
        return transform.Find("Image").GetComponent<Image>();
    }

    public void SetPrice(string price)
    {
        priceText.text = price;
    }

    public void PressBuyButton()
    {
        BuyEvent?.Invoke(this);
    }
}
