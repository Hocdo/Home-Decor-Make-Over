using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeGemAdsButton : MonoBehaviour
{
    public void ButtonPressed()
    {
        Action RewardedVideoReward = () =>
        {
            var popupFreeGemsByAd = Popup.PopupSystem.GetOpenBuilder()
           .SetType(PopupType.PopupFreeGemAds)
           .Open<PopupFreeGemAds>();
            popupFreeGemsByAd.AutoReward();

            //AppEventTracker.LogEventRewardAd("free_gem_ad", true);
        };

        Action RewardedVideoFailed = () =>
        {
           // AppEventTracker.LogEventRewardAd("free_gem_ad", false);
        };

        AdvertisementManager.Instance.ShowRewardedVideo(RewardedVideoReward, RewardedVideoFailed);
    }
}
