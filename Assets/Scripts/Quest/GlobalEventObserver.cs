using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalEventObserver 
{
    public static event Action<Match3.HeadStartBoosterType> UseHeadStartBoosterEvent;

    public static event Action<Match3.BoosterType> UseIngameBoosterEvent;

    public static event Action<ChipType> TriggerSpecialBombEvent;

    public static event Action<bool, int> FinishLevelEvent;

    public static event Action<int> ChangeGemEvent;

    public static event Action DecorUnlockEvent;

    public static void InvokeUseHeadStartBoosterEvent(Match3.HeadStartBoosterType type)
    {
        UseHeadStartBoosterEvent?.Invoke(type);
    }

    public static void InvokeUseIngameBoosterEvent(Match3.BoosterType type)
    {
        UseIngameBoosterEvent?.Invoke(type);
    }

    public static void InvokeFinishLevelEvent(bool result, int remainingMove)
    {
        FinishLevelEvent?.Invoke(result, remainingMove);
    }

    public static void InvokeTriggerSpecialBombEvent(ChipType chipType)
    {
        TriggerSpecialBombEvent?.Invoke(chipType);
    }

    public static void InvokeChangeGemEvent(int change)
    {
        ChangeGemEvent?.Invoke(change);
    }

    public static void InvokeDecorUnlockEvent()
    {
        DecorUnlockEvent?.Invoke();
    }
}
