using System.Collections.Generic;

public class DictEnumKeyGenericVal<ValueType> : Dictionary<int, ValueType> where ValueType : class
{
	public DictEnumKeyGenericVal()
		: base((IEqualityComparer<int>)Singleton<IntKeyComparer>.Instance)
	{
	}

	public ValueType Get(int keyValue)
	{
		return base[keyValue];
	}
}
