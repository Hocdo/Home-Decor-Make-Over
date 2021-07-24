using System.Collections.Generic;

public class ByteKeyComparer : Singleton<ByteKeyComparer>, IEqualityComparer<byte>
{
	public bool Equals(byte x, byte y)
	{
		return x == y;
	}

	public int GetHashCode(byte obj)
	{
		return obj;
	}
}
