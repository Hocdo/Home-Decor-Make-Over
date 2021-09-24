using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Decor
{
    public class PostDecorCompleteEffect : MonoBehaviour
    {
        [Header("Confetti")]
        public TextEffectDecorComplete textEffect;

        public ParticleSystem confettiParticleSystem;

        [Header("Decor Items")]
        public Material blendMaterial;

        public Material spriteDefaultMaterial;

        public Material brightMaterial;

        public int minEmitCount = 8;

        public int maxEmitCount = 30;

        public AnimationCurve particleCountEvaluator;

        public AnimationCurve itemBrightCurve;

        public ParticleSystem particleEffectSample;

        [Header("Bright")]
        public GameObject brightEffect;

        public AnimationCurve brightCurve;

        public float brightDuration = 0.5f;

        private DesignItemView[] items;

        private int[] cachedVariantIndices;

        private List<ParticleSystem> cachedParticleSystems = new List<ParticleSystem>();

        public Action PressContinueAction;

        public void Setup(DesignItemView[] aitems)
        {
            items = aitems;

            cachedParticleSystems.Clear();
            cachedVariantIndices = new int[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                cachedVariantIndices[i] = items[i].primaryData.variantIndex;

                var spriteRenderers = items[i].GetSpriteRenderers();
                for (int p = 0; p < spriteRenderers.Length; p++)
                {
                    ParticleSystem ps = Instantiate(particleEffectSample);
                    ps.gameObject.SetActive(true);
                    cachedParticleSystems.Add(ps);

                    Vector2 size = spriteRenderers[p].bounds.extents;
                    float area = size.x * size.y;
                    float t = Mathf.Clamp01(area / 1500f);
                    int particleCount = (int)(Mathf.Lerp(minEmitCount, maxEmitCount, particleCountEvaluator.Evaluate(t)));

                    var emission = ps.emission;
                    ParticleSystem.Burst burst = emission.GetBurst(0);
                    burst.count = particleCount;
                    emission.SetBurst(0, burst);

                    var shape = ps.shape;
                    shape.spriteRenderer = spriteRenderers[p];
                }
            }          
        }

        public void Play(float delayTime, Action CompleteEvent)
        {
            StartCoroutine(PlayCoroutine(delayTime, CompleteEvent));
        }

        public IEnumerator PlayCoroutine(float delayTime, Action CompleteEvent)
        {
            yield return new WaitForSeconds(delayTime);

            brightMaterial.SetFloat("_Brightness", 1f);
            brightMaterial.SetColor("_BrightColor", Color.white);
            SetMaterial(brightMaterial);

            float cameraSize = Camera.main.orthographicSize;

            textEffect.transform.parent.gameObject.SetActive(true);
            textEffect.transform.parent.localScale *= cameraSize / 25f;

            Vector3 particlePosition = confettiParticleSystem.transform.position;
            confettiParticleSystem.transform.position = new Vector3(particlePosition.x, -cameraSize, particlePosition.y);

            brightMaterial.DOFloat(1.075f, "_Brightness", 0.5f).
                SetDelay(0.35f).
                OnComplete(() => AudioManager.Instance.PlaySFX(AudioClipId.DecorComplete));

            yield return new WaitForSeconds(5.5f);

            brightMaterial.DOFloat(2f, "_Brightness", 1f);
            brightMaterial.DOFloat(1f, "_Brightness", 1f).SetDelay(1f);

            Material textMaterial = textEffect.GetComponent<MeshRenderer>().material;
            textMaterial.DOColor(new Color(1f, 1f, 1f, 0f), 1f).
                OnComplete(() => textEffect.gameObject.SetActive(false));

            yield return new WaitForSeconds(0.5f);

            float intervalTime = 0.5f;
            for (int i = 0; i < items.Length; i++)
            {
                DesignItemView item = items[i];
                if (item.HasDefaultVariant())
                {
                    items[i].SetVariant(-1);
                }
                else
                {
                    item.SetVisualActive(false);
                }
            }

            yield return new WaitForSeconds(3f);

            brightMaterial.DOFloat(1f, "_Brightness", 0.5f);

            // decor items
            SetMaterial(brightMaterial/*, true*/);

            for (int i = 0, pidx = 0; i < items.Length; i++)
            {
                DesignItemView item = items[i];
                var spriteRenderers = items[i].GetSpriteRenderers();
                item.SetVisualActive(true);

                if (item.effectType == DesignItemEffectType.BounceScaleWithParticle)
                {
                    item.SetVariant(cachedVariantIndices[i]);

                    for (int p = 0; p < spriteRenderers.Length; p++, pidx++)
                    {
                        SpriteRenderer sr = spriteRenderers[p];

                        sr.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                        sr.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack);

                        cachedParticleSystems[pidx].Play();

                        AudioManager.Instance.PlaySFX(AudioClipId.ItemApply);

                        sr.material.DOFloat(1.15f, "_Brightness", 0.5f)
                            .SetEase(itemBrightCurve);
                    }
                }
                else if (item.effectType == DesignItemEffectType.HorizontalBlendWithParticle)
                {
                    Texture2D[] textures = new Texture2D[spriteRenderers.Length];
                    for (int p = 0; p < spriteRenderers.Length; p++)
                    {
                        if (spriteRenderers[p].sprite != null)
                            textures[p] = spriteRenderers[p].sprite.texture;
                    }

                    item.SetVariant(cachedVariantIndices[i]);

                    for (int p = 0; p < spriteRenderers.Length; p++, pidx++)
                    {
                        SpriteRenderer sr = spriteRenderers[p];

                        Texture2D mainTex = sr.sprite.texture;

                        Material blendMaterialCopy = new Material(blendMaterial);

                        blendMaterialCopy.SetTexture("_SourceTex", textures[p]);
                        blendMaterialCopy.SetTexture("_MainTex", mainTex);

                        var sourceMaterial = sr.material;
                        sr.material = blendMaterialCopy;

                        int p_idx = pidx;
                        blendMaterialCopy.SetFloat("_Progress", 0f);
                        blendMaterialCopy.DOFloat(1f, "_Progress", 0.6f).
                            OnComplete(() =>
                            {
                                sr.material = sourceMaterial;

                                cachedParticleSystems[p_idx].Play();
                                AudioManager.Instance.PlaySFX(AudioClipId.ItemApply);
                            });

                        blendMaterialCopy.DOFloat(1.15f, "_Brightness", 0.6f)
                            .SetEase(itemBrightCurve);
                    }
                }
                else
                {
                    item.SetVariant(cachedVariantIndices[i]);

                    for (int p = 0; p < spriteRenderers.Length; p++, pidx++)
                    {
                        spriteRenderers[p].material.DOFloat(1.15f, "_Brightness", 0.5f)
                            .SetEase(itemBrightCurve);
                    }
                }               

                yield return new WaitForSeconds(intervalTime);

                intervalTime = Mathf.Max(0.25f, intervalTime - 0.1f);
            }

            yield return new WaitForSeconds(0.5f);

            SetMaterial(brightMaterial);

            // bright
            float extraBrightness = 0f;
            DOTween.To(() => extraBrightness, (value) => extraBrightness = value, 0.135f, brightDuration)
                .SetEase(brightCurve)
                .OnUpdate(() =>
                {
                    brightMaterial.SetFloat("_Brightness", 1f + extraBrightness);
                });

            Color extraBrightColor = Color.clear;
            DOTween.To(() => extraBrightColor, (value) => extraBrightColor = value, new Color(0f, 0f, -0.17647f, 0f), brightDuration)
                .SetEase(brightCurve)
                .OnUpdate(() =>
                {
                    brightMaterial.SetColor("_BrightColor", Color.white + extraBrightColor);
                });

            yield return new WaitForSeconds(0.15f);

            AudioManager.Instance.PlaySFX(AudioClipId.BrightStart);

            brightEffect.SetActive(true);

            yield return new WaitForSeconds(4.5f);

            SetMaterial(spriteDefaultMaterial);

            CompleteEvent?.Invoke();
        }

        private void SetMaterial(Material material, bool createCopy = false)
        {
            for (int i = 0, pidx = 0; i < items.Length; i++)
            {
                var spriteRenderers = items[i].GetSpriteRenderers();

                if (createCopy)
                {
                    Material copyMaterial = new Material(material);
                 
                    for (int p = 0; p < spriteRenderers.Length; p++, pidx++)
                    {
                        spriteRenderers[p].material = copyMaterial;
                    }
                }
                else
                {
                    for (int p = 0; p < spriteRenderers.Length; p++, pidx++)
                    {
                        spriteRenderers[p].material = material;
                    }
                }  
            }
        }
    }
}
