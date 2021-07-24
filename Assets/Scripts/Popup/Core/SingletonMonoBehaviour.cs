using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    private static bool instanceDestroyed = false;

    private static bool instanceExisting = false;

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

    public static bool IsInstanceDestroyed
    {
        get { return instanceDestroyed; }
    }

    public static bool IsInstanceExisting
    {
        get { return instanceExisting; }
    }

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            instanceDestroyed = false;
            instanceExisting = true;
            DontDestroyOnLoad(instance.gameObject);
        }
        else if (instance.GetInstanceID() != GetInstanceID())
        {
            Destroy(gameObject);
        }
    }

    public virtual void OnDestroy()
    {
        instanceDestroyed = true;
        instanceExisting = false;
    }
}
