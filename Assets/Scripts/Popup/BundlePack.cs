using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BundlePack : MonoBehaviour
{
    [Header("Data")]
    public string productId;

    public float priceInUsd;

    [Header("Refs")]
    public Button buyButton;

    public Text priceText;

    public Action<BundlePack> BuyEvent;

    public int index = 0;

    public void SetPrice(string price)
    {
        buyButton.onClick.AddListener(PressBuyButton);
        priceText.text = price;
    }

    public void PressBuyButton()
    {
        BuyEvent?.Invoke(this);
    }
}
