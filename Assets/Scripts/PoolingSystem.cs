using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingSystem : MonoBehaviour
{
    [SerializeField] private List<PrefabPool> poolData;

    private Dictionary<string, PrefabPool> poolDataDict;

    void Awake()
    {

    }
}
