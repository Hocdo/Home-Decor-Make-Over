using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    public AudioClipTable audioClipTable;

    public AudioSource effectsSource;
    public AudioSource musicSource;

    private static AudioManager instance = null;

    private Dictionary<AudioClipId, AudioClip> audioClipDict;

    public static AudioManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            //audioClipDict = AudioClipTable.Instance.GetDictionary();
            audioClipDict = audioClipTable.GetDictionary();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void SetMusicEnable(bool enabled)
    {
        musicSource.mute = !enabled;
    }

    public void SetEffectEnable(bool enabled)
    {
        effectsSource.mute = !enabled;
    }

    public void StopMusic()
    {
        if (musicSource)
            musicSource.Stop();
    }

    public void StopEffect()
    {
        if (effectsSource)
            effectsSource.Stop();
    }

    public Tween CrossOut(float duration, bool stop = false)
    {
        return DOTween.To(() => musicSource.volume, (value) => musicSource.volume = value, 0f, duration).OnComplete(() => { if (stop) musicSource.Stop(); });
    }

    public Tween CrossIn(float duration)
    {
        return DOTween.To(() => musicSource.volume, (value) => musicSource.volume = value, 1f, duration);
    }

    public static void Play_SFX(AudioClipId key)
    {
        instance.PlaySFX(key);
    }

//#if UNITY_EDITOR
    public void PlaySFX(AudioClipId key)
    {
        if (audioClipDict.ContainsKey(key))
        {
            //Debug.Log("Play sfx: " + key);
            PlaySFX(audioClipDict[key]);
        }
        else
        {
            Debug.LogWarning("Audio Clip key " + key + " is not exist");
        }
    }

    public void PlayMusic(AudioClipId key, bool forceReplay = false)
    {
        if (audioClipDict.ContainsKey(key))
        {
            //Debug.Log("Play music: " + key);
            PlayMusic(audioClipDict[key], forceReplay);
        }
        else
        {
            Debug.LogWarning("Audio Clip key " + key + " is not exist");
        }
    }
//#else
//    //public void PlaySFX(AudioClipId key)
//    //{
//    //    PlaySFX(audioClipDict[key]);
//    //}

//    //public void PlayMusic(AudioClipId key)
//    //{
//    //    PlayMusic(audioClipDict[key]);
//    //}

//#endif
    public void PlaySFX(AudioClip clip)
    {
        effectsSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip, bool forceReplay = false)
    {
        if (musicSource.isPlaying && !forceReplay) return;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySFX(AudioSource audioSource)
    {
        audioSource.Play();
    }

    public void PlaySFX(AudioSource audioSource, AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
