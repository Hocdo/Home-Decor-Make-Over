using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreventDestroyingOnLoad : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
