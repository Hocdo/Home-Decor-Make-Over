using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : Component
{
	private static T instance;
	
	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<T>();
 
				if (instance == null)
				{
					var singletonObject = new GameObject();
					singletonObject.AddComponent<T>();
					singletonObject.name = typeof(T).Name;

					DontDestroyOnLoad(singletonObject);
				}
			}
 
			return instance;
		}
	}

	public virtual void Awake()
	{
		if (instance == null)
		{
			instance = this as T;
			DontDestroyOnLoad(instance.gameObject);
		}
		else if (instance.GetInstanceID() != GetInstanceID())
		{
			Destroy(gameObject);
		}
	}
}
