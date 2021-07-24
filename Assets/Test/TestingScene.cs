using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestingScene : MonoBehaviour
{
    private LineRenderer lineRenderer;

    public void Start()
    {
       
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PopupGameWin popupGameWin = Popup.PopupSystem.Instance.ShowPopup(PopupType.PopupGameWin, Popup.CurrentPopupBehaviour.Close, false) as PopupGameWin;
            Popup.PopupSystem.Instance.SetBackBlockerPressedEventOfCurrentPopup(popupGameWin.OnPressContinue);
            popupGameWin.SetBonusCoin(320);
            popupGameWin.CloseEvent = () =>
            {
                SoundManager.StopSFX();
                SoundManager.Instance.offTheSFX = false;
            };
        }
    }
}
