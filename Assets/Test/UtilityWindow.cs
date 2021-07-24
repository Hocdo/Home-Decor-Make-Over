using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityWindow : MonoBehaviour
{
    public Transform item;

    [ContextMenu("Xe")]
    public void Execute()
    {
        for (int i = 0; i < item.childCount; i++)
        {
            item.GetChild(i).transform.localPosition *= 1f / 0.63f;
        }
    }
}
