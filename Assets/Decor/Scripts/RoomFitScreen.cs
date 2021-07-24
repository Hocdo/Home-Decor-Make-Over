using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomFitScreen : MonoBehaviour
{
    [Header("Min Ratio")]
    public Vector2 minCameraRatio = new Vector2(4f, 3f);

    public float maxCameraOrthoSize = 35f;

    [Header("Max Ratio")]
    public Vector2 maxCameraRatio = new Vector2(21f, 9f);

    public float minCameraOrthoSize = 25f;

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        float minRatio = minCameraRatio.x / minCameraRatio.y;
        float maxRatio = maxCameraRatio.x / maxCameraRatio.y;

        Vector2 minRatioCameraSize = new Vector2(minRatio * maxCameraOrthoSize * 2f, maxCameraOrthoSize * 2f);
        Vector2 maxRatioCameraSize = new Vector2(maxRatio * minCameraOrthoSize * 2f, minCameraOrthoSize * 2f);

        Vector2 position = transform.position;

        DrawRect(position - minRatioCameraSize * 0.5f, position + minRatioCameraSize * 0.5f, Color.red);
        DrawRect(position - maxRatioCameraSize * 0.5f, position + maxRatioCameraSize * 0.5f, Color.red);
    }

    void DrawRect(Vector2 p1, Vector2 p2, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawLine(new Vector3(p1.x, p1.y, -80f), new Vector3(p1.x, p2.y, -80f));
        Gizmos.DrawLine(new Vector3(p1.x, p1.y, -80f), new Vector3(p2.x, p1.y, -80f));
        Gizmos.DrawLine(new Vector3(p2.x, p2.y, -80f), new Vector3(p1.x, p2.y, -80f));
        Gizmos.DrawLine(new Vector3(p2.x, p2.y, -80f), new Vector3(p2.x, p1.y, -80f));
    }
#endif
}
