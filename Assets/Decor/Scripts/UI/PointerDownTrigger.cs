using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerDownTrigger : MonoBehaviour, IPointerDownHandler
{
    public Action OnPointerDownAction;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnPointerDownAction?.Invoke();
    }
}
