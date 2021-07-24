using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SFXGroup
{
	public string groupName;

	public int specificCapAmount;

	public List<AudioClip> clips = new List<AudioClip>();

	public bool independentVolume;

	public bool independentPitch;

	public float volume;

	public float pitch;

	public SFXGroup(string name, int capAmount, params AudioClip[] audioclips)
	{
		groupName = name;
		specificCapAmount = capAmount;
		clips = new List<AudioClip>(audioclips);
		independentVolume = false;
		independentPitch = false;
		volume = 1f;
		pitch = 1f;
	}

	public SFXGroup(string name, params AudioClip[] audioclips)
	{
		groupName = name;
		specificCapAmount = 0;
		clips = new List<AudioClip>(audioclips);
		independentVolume = false;
		independentPitch = false;
		volume = 1f;
		pitch = 1f;
	}
}
