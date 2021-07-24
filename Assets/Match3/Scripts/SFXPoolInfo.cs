using System.Collections.Generic;
using UnityEngine;

public class SFXPoolInfo
{
	public int currentIndexInPool;

	public int prepoolAmount;

	public float baseVolume = 1f;

	public float volumeVariation;

	public float pitchVariation;

	public List<float> timesOfDeath = new List<float>();

	public List<GameObject> ownedAudioClipPool = new List<GameObject>();

	public SFXPoolInfo(int index, int minAmount, List<float> times, List<GameObject> pool, float baseVol = 1f, float volVar = 0f, float pitchVar = 0f)
	{
		currentIndexInPool = index;
		prepoolAmount = minAmount;
		timesOfDeath = times;
		ownedAudioClipPool = pool;
		baseVolume = baseVol;
		volumeVariation = volVar;
		pitchVariation = pitchVar;
	}
}
