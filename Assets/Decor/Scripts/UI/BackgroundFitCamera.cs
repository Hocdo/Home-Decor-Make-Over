using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundFitCamera : MonoBehaviour
{
    public CanvasScaler canvasScaler;

    void Start()
    {
        Execute();
    }


    [ContextMenu("Execute")]
    public void Execute()
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 contentSize = UIUtility.GetCanvasSize(canvasScaler);
        Vector2 bgSize = rectTransform.sizeDelta;

        float scale = Mathf.Max(contentSize.x / bgSize.x +0.01f, contentSize.y / bgSize.y+0.01f);
        rectTransform.sizeDelta = bgSize * scale;
    }
}
