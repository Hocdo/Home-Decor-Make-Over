using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioSettingInterface
{
    public static void SetMusicEnabled(bool flag)
    {
        AudioManager.Instance.SetMusicEnable(flag);
        SoundManager.Instance.mutedMusic = !flag;
    }

    public static void SetSFXEnabled(bool flag)
    {
        AudioManager.Instance.SetEffectEnable(flag);
        SoundManager.Instance.mutedSFX = !flag;
    }

    public static void StopMusic()
    {

    }

    public static void StopSFX()
    {

    }
}
