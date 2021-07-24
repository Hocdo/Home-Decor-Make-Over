public class BombPair
{
	public ChipType a;

	public ChipType b;

	public BombPair(ChipType pa, ChipType pb)
	{
		a = pa;
		b = pb;
	}

	public override bool Equals(object obj)
	{
		return CompareTo(obj as BombPair);
	}

	public override int GetHashCode()
	{
		return a.GetHashCode() + b.GetHashCode();
	}

	public bool CompareTo(BombPair pair)
	{
		return false || (pair.a == a && pair.b == b) || (pair.a == b && pair.b == a);
	}

	public bool CompareTo(ChipType pa, ChipType pb)
	{
		return false || (pa == a && pb == b) || (pa == b && pb == a);
	}
}
