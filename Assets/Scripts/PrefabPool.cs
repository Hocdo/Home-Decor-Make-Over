using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PrefabPool
{
    public string name;
    public GameObject prefab;
    public int prepoolCount;

    protected Queue<GameObject> activeQueue;
    protected Queue<GameObject> inactiveQueue;

    public void PreCache(Transform parent)
    {
        if (string.IsNullOrEmpty(name))
        {
            name = prefab.name;
        }

        for (int i = 0; i < prepoolCount; i++)
        {
            GameObject go = Create(parent);
            go.name = name;
            go.SetActive(false);

            inactiveQueue.Enqueue(go);
        }
    }

    private GameObject Create()
    {
        return GameObject.Instantiate(prefab);
    }

    private GameObject Create(Transform parent)
    {
        return GameObject.Instantiate(prefab, parent);
    }

    public void Clear()
    {

    }

    public GameObject Get()
    {
        GameObject go;

        if (inactiveQueue.Count > 0)
        {
            go = inactiveQueue.Dequeue();
            go.SetActive(true);
            
            activeQueue.Enqueue(go);
        }
        else
        {
            go = Create();
            activeQueue.Enqueue(go);
        }

        return go;
    }

    public void Despawn(GameObject go)
    {
        if (activeQueue.Contains(go))
        {
            //var go = activeQueue.Dequeue();
            //go.SetActive(false);

            //inactiveQueue.Enqueue(go);
        }
    }
}
