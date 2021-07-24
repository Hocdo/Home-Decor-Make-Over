using System;
using System.Collections.Generic;
using System.Reflection.Emit;

public class EnumComparer<T> : IEqualityComparer<T> where T : struct, IComparable, IConvertible, IFormattable
{
	public static readonly EnumComparer<T> Instance;

	private static readonly Func<T, int> casterFunc;

	private static readonly Func<T, T, bool> comparerFunc;

	static EnumComparer()
	{
		casterFunc = sGenerateGetHashCodeMethod();
		comparerFunc = sGenerateEqualsMethod();
		Instance = new EnumComparer<T>();
	}

	private EnumComparer()
	{
	}

	public bool Equals(T x, T y)
	{
		return comparerFunc(x, y);
	}

	public int GetHashCode(T obj)
	{
		return casterFunc(obj);
	}

	private static Func<T, T, bool> sGenerateEqualsMethod()
	{
		Type typeFromHandle = typeof(T);
		DynamicMethod dynamicMethod = new DynamicMethod("DynamicEquals", typeof(bool), new Type[2]
		{
			typeFromHandle,
			typeFromHandle
		}, typeFromHandle, skipVisibility: true);
		ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
		iLGenerator.Emit(OpCodes.Ldarg_0);
		iLGenerator.Emit(OpCodes.Ldarg_1);
		iLGenerator.Emit(OpCodes.Ceq);
		iLGenerator.Emit(OpCodes.Ret);
		return (Func<T, T, bool>)dynamicMethod.CreateDelegate(typeof(Func<T, T, bool>));
	}

	private static Func<T, int> sGenerateGetHashCodeMethod()
	{
		Type typeFromHandle = typeof(T);
		DynamicMethod dynamicMethod = new DynamicMethod("DynamicGetHashCode", typeof(int), new Type[1]
		{
			typeFromHandle
		}, typeFromHandle, skipVisibility: true);
		ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
		iLGenerator.Emit(OpCodes.Ldarg_0);
		iLGenerator.Emit(OpCodes.Ret);
		return (Func<T, int>)dynamicMethod.CreateDelegate(typeof(Func<T, int>));
	}
}
