using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SoundConnection
{
	public string level;

	public bool isCustomLevel;

	public List<AudioClip> soundsToPlay;

	public List<float> baseVolumes = new List<float>();

	public SoundManager.PlayMethod playMethod;

	public float minDelay;

	public float maxDelay;

	public float delay;

	public SoundConnection(string lvl, params AudioClip[] audioList)
	{
		level = lvl;
		isCustomLevel = false;
		playMethod = SoundManager.PlayMethod.ContinuousPlayThrough;
		minDelay = 0f;
		maxDelay = 0f;
		delay = 0f;
		soundsToPlay = new List<AudioClip>();
		baseVolumes = new List<float>();
		foreach (AudioClip item in audioList)
		{
			if (!soundsToPlay.Contains(item))
			{
				soundsToPlay.Add(item);
				baseVolumes.Add(1f);
			}
		}
	}

	public SoundConnection(string lvl, SoundManager.PlayMethod method, params AudioClip[] audioList)
	{
		level = lvl;
		isCustomLevel = false;
		playMethod = method;
		switch (playMethod)
		{
		}
		minDelay = 0f;
		maxDelay = 0f;
		delay = 0f;
		soundsToPlay = new List<AudioClip>();
		baseVolumes = new List<float>();
		foreach (AudioClip item in audioList)
		{
			if (!soundsToPlay.Contains(item))
			{
				soundsToPlay.Add(item);
				baseVolumes.Add(1f);
			}
		}
	}

	public SoundConnection(string lvl, SoundManager.PlayMethod method, float delayPlay, params AudioClip[] audioList)
	{
		level = lvl;
		isCustomLevel = false;
		playMethod = method;
		minDelay = 0f;
		maxDelay = delayPlay;
		delay = delayPlay;
		soundsToPlay = new List<AudioClip>();
		baseVolumes = new List<float>();
		foreach (AudioClip item in audioList)
		{
			if (!soundsToPlay.Contains(item))
			{
				soundsToPlay.Add(item);
				baseVolumes.Add(1f);
			}
		}
	}

	public SoundConnection(string lvl, SoundManager.PlayMethod method, float minDelayPlay, float maxDelayPlay, params AudioClip[] audioList)
	{
		level = lvl;
		isCustomLevel = false;
		playMethod = method;
		minDelay = minDelayPlay;
		maxDelay = maxDelayPlay;
		delay = (maxDelayPlay + minDelayPlay) / 2f;
		soundsToPlay = new List<AudioClip>();
		baseVolumes = new List<float>();
		foreach (AudioClip item in audioList)
		{
			if (!soundsToPlay.Contains(item))
			{
				soundsToPlay.Add(item);
				baseVolumes.Add(1f);
			}
		}
	}

	public void SetToCustom()
	{
		isCustomLevel = true;
	}
}
