using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class SoundManagerTools
{
	private static readonly System.Random random = new System.Random();

	public static void Shuffle<T>(ref List<T> theList)
	{
		int num = theList.Count;
		while (num > 1)
		{
			num--;
			int index = random.Next(num + 1);
			T value = theList[index];
			theList[index] = theList[num];
			theList[num] = value;
		}
	}

	public static void ShuffleTwo<T, K>(ref List<T> theList, ref List<K> otherList)
	{
		int num = theList.Count;
		while (num > 1)
		{
			num--;
			int index = random.Next(num + 1);
			T value = theList[index];
			theList[index] = theList[num];
			theList[num] = value;
			if (otherList.Count == theList.Count)
			{
				K value2 = otherList[index];
				otherList[index] = otherList[num];
				otherList[num] = value2;
			}
		}
	}

	public static void make2D(ref AudioSource theAudioSource)
	{
		theAudioSource.spatialBlend = 0f;
	}

	public static void make3D(ref AudioSource theAudioSource)
	{
		theAudioSource.spatialBlend = 1f;
	}

	public static float VaryWithRestrictions(this float theFloat, float variance, float minimum = 0f, float maximum = 1f)
	{
		float num = theFloat * (1f + variance);
		float num2 = theFloat * (1f - variance);
		if (num > maximum)
		{
			num = maximum;
		}
		if (num2 < minimum)
		{
			num2 = minimum;
		}
		return UnityEngine.Random.Range(num2, num);
	}

	public static float Vary(this float theFloat, float variance)
	{
		float max = theFloat * (1f + variance);
		float min = theFloat * (1f - variance);
		return UnityEngine.Random.Range(min, max);
	}

	public static FieldInfo[] GetAllFieldInfos(this Type type)
	{
		if (type == null)
		{
			return new FieldInfo[0];
		}
		BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		return (from f in type.GetFields(bindingAttr).Concat(type.BaseType.GetAllFieldInfos())
			where !f.IsDefined(typeof(HideInInspector), inherit: true)
			select f).ToArray();
	}
}
