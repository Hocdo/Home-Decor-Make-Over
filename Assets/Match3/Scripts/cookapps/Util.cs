using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace cookapps
{
	public static class Util
	{
		public static int RandomRange(int _min, int _max)
		{
			if (_min == _max - 1)
			{
				return _min;
			}
			return UnityEngine.Random.Range(_min, _max - 1);
		}

		public static float RandomRange(float _min, float _max)
		{
			return UnityEngine.Random.Range(_min, _max);
		}

		public static void Shuffle<T>(IList<T> _list)
		{
			int num = _list.Count;
			System.Random random = new System.Random();
			while (num > 1)
			{
				int index = random.Next(0, num) % num;
				num--;
				T value = _list[index];
				_list[index] = _list[num];
				_list[num] = value;
			}
		}

		public static T[,] ResizeArray<T>(T[,] original, int rows, int cols)
		{
			T[,] array = new T[rows, cols];
			int num = Math.Min(rows, original.GetLength(0));
			int num2 = Math.Min(cols, original.GetLength(1));
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num2; j++)
				{
					array[i, j] = original[i, j];
				}
			}
			return array;
		}

		public static string GetFilePath(string filename)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				string text = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
				text = text.Substring(0, text.LastIndexOf('/'));
				return Path.Combine(Path.Combine(text, "Documents"), filename);
			}
			if (Application.platform == RuntimePlatform.Android)
			{
				string persistentDataPath = Application.persistentDataPath;
				persistentDataPath = persistentDataPath.Substring(0, persistentDataPath.LastIndexOf('/'));
				return Path.Combine(persistentDataPath, filename);
			}
			return Application.persistentDataPath;
		}

		public static string GetFileStreamingAssetsPath(string fileName)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				return Path.Combine(Application.dataPath + "/Raw", fileName);
			}
			if (Application.platform == RuntimePlatform.Android)
			{
				return Path.Combine("jar:file://" + Application.dataPath + "!/assets/", fileName);
			}
			return Application.streamingAssetsPath;
		}
	}
}
