using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardItemController : MonoBehaviour
{
    public Action CompleteAction;

    public AnimationCurve timeCurve;

    [Header("References")]
    public SpriteRenderer spriteRenderer;

    public TextMesh textMesh;

    public ParticleSystem parentParticleSystem;

    public ParticleSystem glowParticleSystem;

    [HideInInspector]
    public RewardType type;

    public virtual void SetCount(string count)
    {
        textMesh.text = count;
    }

    public virtual void SetAsCollectedMode()
    {
        textMesh.gameObject.SetActive(false);
        glowParticleSystem.gameObject.SetActive(false);
    }

    public virtual void CollectWithTarget(Vector3 targetPosition, float delay)
    {
        StartCoroutine(CollectWithTargetCoroutine(targetPosition, delay));
    }

    protected virtual IEnumerator CollectWithTargetCoroutine(Vector3 targetPosition, float delay)
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        float startScale = transform.localScale.x;
        Vector3 sourcePosition = transform.position;
        Vector3 controlPosition = new Vector3(targetPosition.x, sourcePosition.y, sourcePosition.z);
        float time = 0f;
        float duration = 0.65f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            if (t > 1f) t = 1f;

            float oneminust = 1f - t;
            Vector3 position = oneminust * oneminust * sourcePosition + 2f * oneminust * t * controlPosition + t * t * targetPosition;
            transform.localPosition = position;

            float scale = startScale * (1f - t * 0.5f);
            transform.localScale = new Vector3(scale, scale, scale);

            yield return null;
        }

        AudioManager.Instance.PlaySFX(AudioClipId.AddReward);

        CompleteAction?.Invoke();

        parentParticleSystem.Stop(true);
        spriteRenderer.gameObject.SetActive(false);

        Destroy(gameObject, 5f);
    }
}
