using UnityEngine;

namespace antilunchbox
{
	public class Singleton<T> : MonoBehaviour where T : Singleton<T>
	{
		private static T _mInstance;

		protected static Singleton<T> mInstance
		{
			get
			{
				if (!(Object)_mInstance)
				{
					T[] array = UnityEngine.Object.FindObjectsOfType(typeof(T)) as T[];
					if (array.Length != 0)
					{
						if (array.Length == 1)
						{
							_mInstance = array[0];
							_mInstance.gameObject.name = typeof(T).Name;
							return _mInstance;
						}
						T[] array2 = array;
						for (int i = 0; i < array2.Length; i++)
						{
							T val = array2[i];
							UnityEngine.Object.Destroy(val.gameObject);
						}
					}
					GameObject gameObject = new GameObject(typeof(T).Name, typeof(T));
					_mInstance = gameObject.GetComponent<T>();
					Object.DontDestroyOnLoad(gameObject);
				}
				return _mInstance;
			}
			set
			{
				_mInstance = (value as T);
			}
		}
	}
}
