using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using DG.Tweening;
using Popup;
using System.Text;

public class PopupRemoveAds : PopupBase
{
    private static string productId = "no_ads";

    private static float priceUsd = 2.99f;

    public Text priceText;

    public override void Show()
    {
        Product product = IAPManager.Instance.GetProduct(productId);

        if (product != null)
        {
            string localizedPriceString = product.metadata.localizedPriceString;
            priceText.text = localizedPriceString;
        }
        else
        {
            priceText.text = "$" + priceUsd.ToString();
        }

        canClose = false;
        canvasGroup.alpha = 1f;
        PopupAnimationUtility.AnimateScale(transform, Ease.OutBack, 0.25f, 1f, 0.25f, 0f).OnComplete(() => canClose = true);
    }

    public override void Close(bool forceDestroying = true)
    {
        TerminateInternal(forceDestroying);
    }

    public void PressBuyButton()
    {
        IAPManager.Instance.PurchaseButtonClick(productId, PurchaseGemSucessful);
    }

    public void PurchaseGemSucessful(PurchaseEventArgs args)
    {
        AudioManager.Instance.PlaySFX(AudioClipId.Purchased);
        PlayerData.current.noAds = true;

        AcceptEvent?.Invoke(null);

        CloseInternal();

        PlayerData.current.tempData.spentIAP += priceUsd;
        //AppEventTracker.LogEventIap(productId, priceUsd.ToString());    
    }
}


