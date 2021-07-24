using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioListener))]
[RequireComponent(typeof(AudioSource))]
public class AudioAssistant : MonoBehaviour
{
	public static AudioAssistant main;

	private static readonly Dictionary<string, AudioClip[]> data = new Dictionary<string, AudioClip[]>();

	private static readonly List<string> mixBuffer = new List<string>();

	private static readonly float mixBufferClearDelay = 0.05f;

	public AudioClip[] bombCrush;

	public AudioClip[] buy;

	public AudioClip[] chipCrush;

	public AudioClip[] chipHit;

	public AudioClip[] colorBombCrush;

	public AudioClip[] createBomb;

	public AudioClip[] createColorBomb;

	public AudioClip[] createCrossBomb;

	public AudioClip[] crossBombCrush;

	private string currentTrack;

	public AudioClip fieldMusic;

	public AudioClip menuMusic;

	private AudioSource music;

	public float musicVolume = 1f;

	public bool mute;

	private AudioSource sfx;

	public AudioClip[] swapFailed;

	public AudioClip[] swapSuccess;

	public AudioClip[] timeWarrning;

	public AudioClip[] youLose;

	public AudioClip[] youWin;

	private void Awake()
	{
		main = this;
		AudioSource[] components = GetComponents<AudioSource>();
		music = components[0];
		sfx = components[1];
		data.Clear();
		data.Add("ChipHit", chipHit);
		data.Add("ChipCrush", chipCrush);
		data.Add("BombCrush", bombCrush);
		data.Add("CrossBombCrush", crossBombCrush);
		data.Add("ColorBombCrush", colorBombCrush);
		data.Add("SwapSuccess", swapSuccess);
		data.Add("SwapFailed", swapFailed);
		data.Add("CreateBomb", createBomb);
		data.Add("CreateCrossBomb", createCrossBomb);
		data.Add("CreateColorBomb", createColorBomb);
		data.Add("TimeWarrning", timeWarrning);
		data.Add("YouWin", youWin);
		data.Add("YouLose", youLose);
		data.Add("Buy", buy);
		StartCoroutine(MixBufferRoutine());
		mute = (PlayerPrefs.GetInt("Mute") == 1);
	}

	private IEnumerator MixBufferRoutine()
	{
		float time = 0f;
		while (true)
		{
			time += Time.unscaledDeltaTime;
			yield return 0;
			if (time >= mixBufferClearDelay)
			{
				mixBuffer.Clear();
				time = 0f;
			}
		}
	}

	public void PlayMusic(string track)
	{
		if (track != string.Empty)
		{
			currentTrack = track;
		}
		AudioClip to = null;
		switch (track)
		{
		case "Menu":
			to = main.menuMusic;
			break;
		case "Field":
			to = main.fieldMusic;
			break;
		}
		StartCoroutine(main.CrossFade(to));
	}

	private IEnumerator CrossFade(AudioClip to)
	{
		float delay2 = 1f;
		if (music.clip != null)
		{
			while (delay2 > 0f)
			{
				music.volume = delay2 * musicVolume;
				delay2 -= Time.unscaledDeltaTime;
				yield return 0;
			}
		}
		music.clip = to;
		if (to == null || mute)
		{
			music.Stop();
			yield break;
		}
		delay2 = 0f;
		if (!music.isPlaying)
		{
			music.Play();
		}
		while (delay2 < 1f)
		{
			music.volume = delay2 * musicVolume;
			delay2 += Time.unscaledDeltaTime;
			yield return 0;
		}
		music.volume = musicVolume;
	}

	public static void Shot(string clip)
	{
		if (data.ContainsKey(clip) && !mixBuffer.Contains(clip) && data[clip].Length != 0)
		{
			mixBuffer.Add(clip);
			main.sfx.PlayOneShot(data[clip][Random.Range(0, data[clip].Length)]);
		}
	}

	public void MuteButton()
	{
		mute = !mute;
		PlayerPrefs.SetInt("Mute", mute ? 1 : 0);
		PlayerPrefs.Save();
		PlayMusic((!mute) ? currentTrack : string.Empty);
	}
}
