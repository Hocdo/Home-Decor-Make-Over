using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IconTouchHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
{
    public Action OnPointerClickAction;

    private float idlePositionY;

    public void Start()
    {
        idlePositionY = transform.localPosition.y;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnPointerClickAction?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void Update()
    {
        Vector3 position = transform.localPosition;
        position.y = idlePositionY + Mathf.Sin(position.x + Time.time * 2f) * 0.06f;

        transform.localPosition = position;
    }
}
