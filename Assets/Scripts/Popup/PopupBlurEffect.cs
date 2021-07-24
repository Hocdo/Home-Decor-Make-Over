using UnityEngine;
using UnityEngine.UI;

public class PopupBlurEffect : Popup.PopupBackgroundEffect
{
    public RawImage image;

    public Material material;

    private RenderTexture sourceBlurTexture;

    private RenderTexture horBlurTexture;

    private RenderTexture targetBlurTexture;

    private Camera camera;

    void Awake()
    {
        int width = Screen.width / 4;
        int height = Screen.height / 4;
        sourceBlurTexture = new RenderTexture(width, height, 16);
        horBlurTexture = new RenderTexture(width, height, 0);
        targetBlurTexture = new RenderTexture(width, height, 0);
        image.material = material;

        enabled = false;
    }

    private void OnDestroy()
    {
        sourceBlurTexture.Release();
        horBlurTexture.Release();
        targetBlurTexture.Release();
    }

    private void LateUpdate()
    {
        Graphics.Blit(sourceBlurTexture, horBlurTexture, material, 0);
        Graphics.Blit(horBlurTexture, targetBlurTexture, material, 1);
    }

    public override bool IsActive()
    {
        return image.gameObject.activeSelf;
    }

    public override void SetActive(bool flag)
    {
        if (camera == null)
            camera = Camera.main;

        if (flag)
        {
            camera.targetTexture = sourceBlurTexture;

            image.gameObject.SetActive(true);
            image.material.SetVector("_TexelSize", new Vector4(1f / sourceBlurTexture.width, 1f / sourceBlurTexture.height, 0f, 0f));           
            image.texture = targetBlurTexture;
            image.color = new Color(0.9f, 0.9f, 0.9f, 1f);
            enabled = true;
        }
        else
        {
            camera.targetTexture = null;
            image.gameObject.SetActive(false);
            enabled = false;
        }
    }

    public override void SetActiveImmediately(bool flag)
    {
        SetActive(flag);
    }
}
