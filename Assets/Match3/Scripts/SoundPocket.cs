using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("AntiLunchBox/SoundPocket")]
public class SoundPocket : MonoBehaviour
{
	public string pocketName = "Pocket";

	public SoundPocketType pocketType;

	public List<AudioClip> pocketClips = new List<AudioClip>();

	public List<string> sfxGroups = new List<string>();

	public List<string> clipToGroupKeys = new List<string>();

	public List<string> clipToGroupValues = new List<string>();

	public List<int> sfxPrePoolAmounts = new List<int>();

	public List<float> sfxBaseVolumes = new List<float>();

	public List<float> sfxVolumeVariations = new List<float>();

	public List<float> sfxPitchVariations = new List<float>();

	private Dictionary<string, string> clipsInGroups = new Dictionary<string, string>();

	public bool showAsGrouped;

	public List<bool> showSFXDetails = new List<bool>();

	public int groupAddIndex;

	public int autoPrepoolAmount;

	public float autoBaseVolume = 1f;

	public float autoVolumeVariation;

	public float autoPitchVariation;

	private void Awake()
	{
		Setup();
		DestroyMe();
	}

	public void Setup()
	{
		SetupDictionaries();
		switch (pocketType)
		{
		case SoundPocketType.Subtractive:
			if (SoundManager.Instance.currentPockets.Count == 1 && SoundManager.Instance.currentPockets[0] == pocketName)
			{
				return;
			}
			SoundManager.DeleteSFX();
			SoundManager.Instance.currentPockets.Clear();
			break;
		default:
			if (SoundManager.Instance.currentPockets.Contains(pocketName))
			{
				return;
			}
			break;
		}
		for (int i = 0; i < pocketClips.Count; i++)
		{
			AudioClip audioClip = pocketClips[i];
			if (clipsInGroups.ContainsKey(audioClip.name))
			{
				SoundManager.SaveSFX(audioClip, clipsInGroups[audioClip.name]);
			}
			else
			{
				SoundManager.SaveSFX(audioClip);
			}
			SoundManager.ApplySFXAttributes(audioClip, sfxPrePoolAmounts[i], sfxBaseVolumes[i], sfxVolumeVariations[i], sfxPitchVariations[i]);
		}
		SoundManager.Instance.currentPockets.Add(pocketName);
	}

	private void SetupDictionaries()
	{
		clipsInGroups.Clear();
		for (int i = 0; i < clipToGroupKeys.Count; i++)
		{
			clipsInGroups.Add(clipToGroupKeys[i], clipToGroupValues[i]);
		}
	}

	public void DestroyMe()
	{
		if (base.gameObject.GetComponents<Component>().Length - base.gameObject.GetComponents<Transform>().Length == 1)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			UnityEngine.Object.Destroy(this);
		}
	}
}
