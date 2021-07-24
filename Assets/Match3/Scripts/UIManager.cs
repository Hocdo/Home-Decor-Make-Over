using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    public delegate void EventCancelBooster();
    
    public event EventCancelBooster eventCancelBooster;

    public override void Awake()
    {
        base.Awake();      
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);      
    }

    public void Update()
    {
        if (Application.platform != RuntimePlatform.Android || !Input.GetKeyUp(KeyCode.Escape))
        {
            return;
        }
        /*if (MonoSingleton<PopupSystem>.Instance.CurrentPopupType != PopupType.PopupNone && MonoSingleton<PopupSystem>.Instance.CurrentPopup != null)
        {
            if (!MonoSingleton<PopupSystem>.Instance.CurrentPopup.DisableBackKey)
            {
                MonoSingleton<PopupSystem>.Instance.CurrentPopup.OnEventClose();
            }
        }*/
    }
   
    [ContextMenu("Hack coin")]
    public void hackCoin()
    {
        PlayerData playerData = PlayerData.current;
        playerData.AddCoin(99999);
        //EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.CoinChange, playerData.cointCount);
        EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.CoinAnimChange, new int[] { playerData.cointCount, 1000 });

    }
    [ContextMenu("Hack gem")]
    public void hackGem()
    {
        PlayerData playerData = PlayerData.current;
        playerData.AddGem(99999);
        //EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.CoinChange, playerData.cointCount);
        EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.GemAnimChange, new int[] { playerData.cointCount, 1000 });

    }
    [ContextMenu("Log event")]
    public void testEvent()
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("This is Test");
    }
    public void CancelBooster()
    {
        if (this.eventCancelBooster != null)
        {
            this.eventCancelBooster();
        }
    }
}
