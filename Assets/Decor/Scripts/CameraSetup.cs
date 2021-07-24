using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetup : MonoBehaviour
{
    [Header("Min Ratio")]
    public Vector2 minCameraRatio = new Vector2(4f, 3f);

    public float maxCameraOrthoSize = 35f;

    [Header("Max Ratio")]
    public Vector2 maxCameraRatio = new Vector2(21f, 9f);

    public float minCameraOrthoSize = 25f;

    [Header("Slider")]
    [Range(0f, 1f)]
    public float match;

    [Header("Cameras")]
    public Camera[] cameras;

    public void Execute()
    {
        Execute(minCameraRatio, maxCameraOrthoSize, maxCameraRatio, minCameraOrthoSize);
    }

    public void Execute(Vector2 minCameraRatio, float maxCameraOrthoSize, Vector2 maxCameraRatio, float minCameraOrthoSize)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            Camera camera = cameras[i];

            float minRatio = minCameraRatio.x / minCameraRatio.y;
            float maxRatio = maxCameraRatio.x / maxCameraRatio.y;

            Vector2 cameraSize = new Vector2(camera.aspect * camera.orthographicSize, camera.orthographicSize) * 2f;

            Vector2 minCameraSize = new Vector2(maxRatio * minCameraOrthoSize, minCameraOrthoSize) * 2f;
            Vector2 maxCameraSize = new Vector2(minRatio * maxCameraOrthoSize, maxCameraOrthoSize) * 2f;

            if (minCameraSize.x / cameraSize.x < maxCameraSize.y / cameraSize.y)
            {
                camera.orthographicSize *= minCameraSize.x / cameraSize.x;
            }
            else
            {
                camera.orthographicSize *= maxCameraSize.y / cameraSize.y;
            }
        }
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        if (cameras.Length == 0)
            return;

        Camera camera = cameras[0];
        Vector2 position = camera.transform.position;

        Vector2 cameraSize = new Vector2(camera.aspect * camera.orthographicSize, camera.orthographicSize) * 2f;

        float minRatio = minCameraRatio.x / minCameraRatio.y;
        float maxRatio = maxCameraRatio.x / maxCameraRatio.y;

        Vector2 minRatioCameraSize = new Vector2(minRatio * maxCameraOrthoSize * 2f, maxCameraOrthoSize * 2f);
        Vector2 maxRatioCameraSize = new Vector2(maxRatio * minCameraOrthoSize * 2f, minCameraOrthoSize * 2f);

        DrawRect(position - minRatioCameraSize * 0.5f, position + minRatioCameraSize * 0.5f, Color.red);
        DrawRect(position - maxRatioCameraSize * 0.5f, position + maxRatioCameraSize * 0.5f, Color.red);

        float previewSize = minCameraOrthoSize + (maxCameraOrthoSize - minCameraOrthoSize) * match;
        float previewAspect = minRatio + (maxRatio - minRatio) * (1f - match); 
        Vector2 matchRatioCameraSize = new Vector2(previewAspect * previewSize * 2f, previewSize * 2f);

        DrawRect(position - matchRatioCameraSize * 0.5f, position + matchRatioCameraSize * 0.5f, Color.green);
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
