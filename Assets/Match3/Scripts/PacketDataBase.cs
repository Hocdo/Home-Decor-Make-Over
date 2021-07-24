using System;

[Serializable]
public class PacketDataBase
{
	private static readonly string ResultSuccess = "success";

	public int error;

	public string result;

	public bool IsError()
	{
		if (string.Compare(result, ResultSuccess) == 0)
		{
			return false;
		}
		return true;
	}
}
