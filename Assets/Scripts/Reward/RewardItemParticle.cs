using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardItemParticle : RewardItemController
{
    public CollectParticles collectParticles;

    public override void SetAsCollectedMode()
    {
        base.SetAsCollectedMode();

        parentParticleSystem.gameObject.SetActive(false);
        spriteRenderer.gameObject.SetActive(false);
    }

    protected override IEnumerator CollectWithTargetCoroutine(Vector3 targetPosition, float delay)
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        collectParticles.targetPosition = targetPosition;
        Vector3 sourcePosition = transform.position;

        collectParticles.transform.position = sourcePosition;
        collectParticles.SetRotateAngle(Mathf.Atan2(targetPosition.y - sourcePosition.y, targetPosition.x - sourcePosition.x) * Mathf.Rad2Deg - 90f);
        collectParticles.SetSpawnRate(20);
        collectParticles.Play();

        yield return new WaitForSeconds(collectParticles.lifeTime);

        CompleteAction?.Invoke();  

        Destroy(gameObject, 3f);
    }
}
