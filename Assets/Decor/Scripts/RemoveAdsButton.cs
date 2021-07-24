using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveAdsButton : MonoBehaviour
{
    private void Awake()
    {
        if (PlayerData.current.noAds == true)
            gameObject.SetActive(false);
    }

    public void ButtonPress()
    {
        var popupRemoveAds = Popup.PopupSystem.GetOpenBuilder().
                   SetType(PopupType.PopupRemoveAds).
                   SetCurrentPopupBehaviour(Popup.CurrentPopupBehaviour.Close).
                   Open(); 
    }
}
