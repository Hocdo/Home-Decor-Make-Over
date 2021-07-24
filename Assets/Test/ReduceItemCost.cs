using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Decor;

public class ReduceItemCost : MonoBehaviour
{
    [ContextMenu("Execute")]
    public void Execute()
    {
        int minCost = 0;
        int maxCost = 1;

        List<DesignItemView> items = new List<DesignItemView>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var designItem = transform.GetChild(i).GetComponent<DesignItemView>();
            var cost = designItem.primaryData.costToUnlock.value;

            minCost = Mathf.Min(minCost, cost);
            maxCost = Mathf.Max(maxCost, cost);

            items.Add(designItem);
        }

        if (maxCost <= 1800) return;

        for (int i = 0; i < items.Count; i++)
        {
            var cost = items[i].primaryData.costToUnlock.value;
            float a = Mathf.Clamp01((float)(cost - minCost) / (maxCost - minCost));
            float b = 300 + (1800 - 300) * a;
            int newCost = (int)(b / 10) * 10;

            if (newCost > cost) newCost = cost;
            

            items[i].primaryData.costToUnlock.value = newCost;

            Debug.Log(cost + " -> " + newCost);
        }
    }    
}
