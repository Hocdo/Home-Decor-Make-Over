using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DesignItemTouchTrigger : MonoBehaviour, IPointerDownHandler
{   
    public Action touchAction;

    public void OnPointerDown(PointerEventData eventData)
    {
        touchAction?.Invoke();
    }
}
