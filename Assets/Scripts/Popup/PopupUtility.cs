using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Popup;

public static class PopupUtility
{
    public static PopupLiteMessage popupLiteMessage;

    public static void OpenPopupLiteMesage(string messageString)
    {
        if (popupLiteMessage == null) popupLiteMessage = GetPopupLiteMessage();

        popupLiteMessage.SetText(messageString);
    }

    public static void ForceClosePopupLiteMessage()
    {
        if (popupLiteMessage == null) popupLiteMessage = GetPopupLiteMessage();

        popupLiteMessage.ForceClose();
    }

    private static PopupLiteMessage GetPopupLiteMessage()
    {
        return PopupSystem.Instance.addOnTransform.GetChild(0).GetComponent<PopupLiteMessage>();
    }
}
