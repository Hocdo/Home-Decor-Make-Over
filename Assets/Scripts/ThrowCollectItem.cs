using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ThrowCollectItem : MonoBehaviour
{
    public abstract void SetupAnimation(Vector3 beginPosition, Vector3 targetPosition, float duration, float delayTime);
    protected abstract IEnumerator FlyToTarget(float duration, float delayTime);
}
