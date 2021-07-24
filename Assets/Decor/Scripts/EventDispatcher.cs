using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDispatcher<T> : Singleton<EventDispatcher<T>>
{
    public void RegisterEvent(T eventId, Action<object> callback)
    {
        if (!allCallbacks.ContainsKey(eventId))
        {
            allCallbacks.Add(eventId, null);
        }

        allCallbacks[eventId] += callback;
    }

    public void RemoveEvent(T eventId, Action<object> callback)
    {
        if (allCallbacks.ContainsKey(eventId))
        {
            allCallbacks[eventId] -= callback;
        }
    }

    public void NotifyEvent(T eventId, object param = null)
    {
#if UNITY_EDITOR
        Debug.Log(eventId);
#endif
        if (!allCallbacks.ContainsKey(eventId))
        {
            return;
        }
        if (allCallbacks.ContainsKey(eventId))
        {
            allCallbacks[eventId]?.Invoke(param);
        }
    }

    public void ClearEvent()
    {
        allCallbacks.Clear();
    }

    private Dictionary<T, Action<object>> allCallbacks = new Dictionary<T, Action<object>>();
}