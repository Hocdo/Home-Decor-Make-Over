using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UIUtility 
{
    public static float GetCanvasScale(CanvasScaler canvasScaler)
    {
        float match = canvasScaler.matchWidthOrHeight;
        float uiScaling = Mathf.Pow(Screen.width / canvasScaler.referenceResolution.x, 1f - match) * Mathf.Pow(Screen.height / canvasScaler.referenceResolution.y, match);

        return uiScaling;
    }

    public static Vector2 GetCanvasSize(CanvasScaler canvasScaler)
    {
        float match = canvasScaler.matchWidthOrHeight;
        float uiScaling = Mathf.Pow(Screen.width / canvasScaler.referenceResolution.x, 1f - match) * Mathf.Pow(Screen.height / canvasScaler.referenceResolution.y, match);
        float width = Screen.width / uiScaling;
        float height = Screen.height / uiScaling;

        Vector2 contentSize = new Vector2(width, height);

        return contentSize;
    }
}
