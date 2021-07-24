using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargetDelegate : MonoBehaviour
{
    public Camera targetCamera;

    public Camera renderCamera;

    public string sortingLayerName;

    public int sortingLayerOrder;

    private RenderTexture targetTexture;

    private MeshRenderer meshRenderer;

    public void Awake()
    {
        targetTexture = new RenderTexture(Screen.width, Screen.height, 16);
        targetCamera.targetTexture = targetTexture;

        float renderHeight = renderCamera.orthographicSize * 2f;

        Vector2 cameraSize = new Vector2(renderHeight * renderCamera.aspect, renderHeight);

        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.transform.localScale = new Vector3(cameraSize.x, cameraSize.y, 1f);
        Vector3 cameraPosition = renderCamera.transform.position;

        Vector3 bgPosition = transform.position;
        transform.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, bgPosition.z);

        meshRenderer.sortingLayerName = sortingLayerName;
        meshRenderer.sortingOrder = sortingLayerOrder;

        meshRenderer.material.mainTexture = targetTexture;
    }

    private void OnDestroy()
    {
        if (targetTexture != null && targetTexture.IsCreated())
        {
            targetTexture.Release();
        }
    }

    public Material GetMaterial()
    {
        return meshRenderer.material;
    }

    public Texture GetTexture()
    {
        return targetTexture;
    }
}
