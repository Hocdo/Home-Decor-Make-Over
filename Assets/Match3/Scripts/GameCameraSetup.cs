using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCameraSetup : MonoBehaviour
{
    public float minSize = 325;

    public float maxSize = 400;

    public Camera effectCamera;

    void Start()
    {
        float minRatio = 4f / 3f;
        float maxRatio = 21f / 9f;

        var camera = GetComponent<Camera>();
        float ratio = Mathf.Pow((camera.aspect - minRatio) / (maxRatio - minRatio), 0.3f);
        camera.orthographicSize = maxSize + (ratio) * (minSize - maxSize);

        transform.localPosition -= new Vector3(7f, 0f, 0f);

        effectCamera.orthographicSize = camera.orthographicSize;
    }

    void Update()
    {
        
    }
}
