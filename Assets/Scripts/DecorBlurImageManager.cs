using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DecorBlurImageManager
{
    private static RenderTexture sourceTexture;

    private static RenderTexture targetTexture;

    public static RenderTexture GetSourceTexture()
    {
        if (sourceTexture == null)
        {
            sourceTexture = new RenderTexture(Screen.width / 4, Screen.height / 4, 16);
        }

        return sourceTexture;
    }
    
    public static RenderTexture ApplyBlur(Material mat)
    {
        if (targetTexture == null)
        {
            targetTexture = new RenderTexture(Screen.width / 4, Screen.height / 4, 0);
            targetTexture.wrapMode = TextureWrapMode.Clamp;
        }

        mat.SetVector("_TexelSize", new Vector4(1f / targetTexture.width, 1f / targetTexture.height, 0f, 0f));
        var horizontalBlurTexture = RenderTexture.GetTemporary(targetTexture.width, targetTexture.height, 0);

        Graphics.Blit(sourceTexture, horizontalBlurTexture, mat, 0);
        Graphics.Blit(horizontalBlurTexture, targetTexture, mat, 1);

        RenderTexture.ReleaseTemporary(horizontalBlurTexture);

        return targetTexture;
    }
   
}
