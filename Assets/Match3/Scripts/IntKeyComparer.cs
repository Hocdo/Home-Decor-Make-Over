using System.Collections.Generic;

public class IntKeyComparer : Singleton<IntKeyComparer>, IEqualityComparer<int>
{
	public bool Equals(int x, int y)
	{
		return x == y;
	}

	public int GetHashCode(int obj)
	{
		return obj;
	}
}
