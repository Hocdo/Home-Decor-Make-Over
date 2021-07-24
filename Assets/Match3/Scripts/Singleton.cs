public class Singleton<T> where T : class, new()
{
	protected static T s_Instance;

	public static T Instance
	{
		get
		{
			if (s_Instance == null)
			{
				s_Instance = new T();
			}
			return s_Instance;
		}
	}
}
