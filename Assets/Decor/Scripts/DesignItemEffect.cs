using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesignItemEffect : MonoBehaviour
{
    [Header("Particle Effect")]
    public ParticleSystem particleSystem;

    public int minEmitCount = 15;

    public int maxEmitCount = 35;

    public AnimationCurve particleCountEvaluator;

    [Header("Flicker Effect")]
    public Material flickerMaterial;
    
    [Header("Blend Effect")]
    public Material blendMaterial;

    [Header("Bounce Scale Effect")]
    public AnimationCurve bounceScaleCurve;
    
    [Header("Punch Scale Effect")]
    public AnimationCurve changeScaleCurve;

    public AnimationCurve changeScaleEvaluator;

    private ParticleSystem.ShapeModule shape;

    private ParticleSystem.EmissionModule emission;

    private SpriteRenderer targetRenderer;

    private Texture2D sourceTexture;

    private Sequence flickerSequence;

    private float blendFactor;

    private DesignItemEffectType effectType;

    private Material sourceMaterial;

    private void Awake()
    {
        shape = particleSystem.shape;
        emission = particleSystem.emission;
    }

    public void SetTarget(SpriteRenderer atargetRenderer, DesignItemEffectType aeffectType)
    {
        targetRenderer = atargetRenderer;
        effectType = aeffectType;
        sourceTexture = targetRenderer.sprite.texture;

        Vector2 size = atargetRenderer.bounds.extents;
        float area = size.x * size.y;

        float t = Mathf.Clamp01(area / 1500f);
        int particleCount = (int)(Mathf.Lerp(minEmitCount, maxEmitCount, particleCountEvaluator.Evaluate(t)));
        
        ParticleSystem.Burst burst = emission.GetBurst(0);
        burst.count = particleCount;
        emission.SetBurst(0, burst);
    }

    public void PlayOnSelect()
    {
        //if (flickerSequence == null)
        //{
        //    flickerSequence = DOTween.Sequence();
        //    flickerSequence.Append(targetRenderer.DOColor(new Color(0.8f, 0.8f, 0.8f, 1f), 0.75f)).
        //        Append(targetRenderer.DOColor(Color.white, 0.75f));
        //    flickerSequence.SetLoops(-1, LoopType.Restart);
        //}
    }

    public void PlayOnDeselect()
    {
        //targetRenderer.color = Color.white;
        //flickerSequence.Kill();
        //flickerSequence = null;
    }

    public void PlayOnChange()
    {
        float a = (targetRenderer.bounds.size.x + targetRenderer.bounds.size.y) * 0.5f - 5f;
        float t = Mathf.Clamp(a / 80f, 0f, 1f);
        float scaleOffset = changeScaleEvaluator.Evaluate(t);
        
        var visualXf = targetRenderer.transform;
        visualXf.DOKill();
        visualXf.localScale = new Vector3(1f + scaleOffset, 1f - scaleOffset, 1f);
        visualXf.DOScale(1f, 0.25f).SetEase(changeScaleCurve);
    }

    public void PlayOnApply()
    {
        PlayOnDeselect();

        if (effectType == DesignItemEffectType.BounceScaleWithParticle)
        {
            var xf = targetRenderer.transform;

            Vector3 targetPosition = xf.position + new Vector3(0f, 2.5f, 0f);
            Vector3 targetScale = xf.localScale + new Vector3(-0.25f, 0, 0f);

            xf.DOKill();
            xf.DOMove(targetPosition, 0.325f).SetEase(bounceScaleCurve);
            xf.DOScale(targetScale, 0.325f).SetEase(bounceScaleCurve).
                OnComplete(PlayParticle);
        }
        else if (effectType == DesignItemEffectType.HorizontalBlendWithParticle)
        {
            Texture2D mainTex = targetRenderer.sprite.texture;

            Material blendMaterialCopy = new Material(blendMaterial);

            blendMaterialCopy.SetTexture("_SourceTex", sourceTexture);
            blendMaterialCopy.SetTexture("_MainTex", mainTex);

            sourceMaterial = targetRenderer.material;
            targetRenderer.material = blendMaterialCopy;
            blendFactor = -0.05f;

            DOTween.To(() => blendFactor, (value) => blendFactor = value, 1f, 0.5f).
                OnUpdate(() => blendMaterialCopy.SetFloat("_Progress", blendFactor)).
                OnComplete(() =>
                {
                    targetRenderer.material = sourceMaterial;

                    PlayParticle();
                });
        }
        else if (effectType == DesignItemEffectType.OnlyParticle)
        {
            PlayParticle();
        }
    }

    private void PlayParticle()
    {
        shape.spriteRenderer = targetRenderer;

        Vector2 position = transform.position;
        float z = particleSystem.transform.position.z;
        particleSystem.transform.position = new Vector3(position.x, position.y, z);

        particleSystem.Play();
    }
}
