using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExecuteUtility 
{
    public static void ExecuteAfterSeconds(this MonoBehaviour caller, float time, Action action)
    {
        caller.StartCoroutine(ExecuteAfterSecondsCoroutine(time, action));
    }

    public static void ExecuteNextFrame(this MonoBehaviour caller, Action action)
    {
        caller.StartCoroutine(ExecuteNextFrameCoroutine(action));
    }

    public static void ExecuteWithCondition(this MonoBehaviour caller, float time, Func<bool> condition, Action action)
    {
        caller.StartCoroutine(ExecuteWithConditionCoroutine(time, condition, action));
    }

    private static IEnumerator ExecuteAfterSecondsCoroutine(float time, Action action)
    {
        yield return new WaitForSeconds(time);

        action?.Invoke();
    }

    private static IEnumerator ExecuteWithConditionCoroutine(float time, Func<bool> condition, Action action)
    {
        yield return new WaitUntil(condition);

        action?.Invoke();
    }

    private static IEnumerator ExecuteNextFrameCoroutine(Action action)
    {
        yield return null;

        action?.Invoke();
    }
}
