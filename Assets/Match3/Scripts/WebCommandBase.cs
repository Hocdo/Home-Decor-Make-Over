using System.Collections.Generic;

public class WebCommandBase
{
	protected static int GenKey;

	protected static object GenKeyLock = new object();

	public static float DefaultTimeout = 10f;

	protected string AES128Key = string.Empty;

	public int CommandKey = -1;

	public Dictionary<string, string> dicFormTable;

	public bool IgnoreNetworkOfflineCheck;

	public bool IgnoreNetworkOrder;

	public bool IsGameEndPacket;

	public bool IsGameStartPacket;

	public bool IsGameUseItemPacket;

	public bool IsRetComressed;

	public bool IsRetry;

	public ReqPacketBase reqBase;

	public bool requestBodyJson;

	public string RequestString;

	public byte[] ResponseByteData;

	public string ResponseStringData;

	public float TimeOut;

	public string URL;

	public void SetResponseStringData(string data)
	{
		ResponseStringData = data;
		UnPack(IsRetComressed);
	}

	public void SetResponseByteData(byte[] bytes)
	{
		ResponseByteData = bytes;
		UnPack(IsRetComressed);
	}

	protected void MakeKey()
	{
		lock (GenKeyLock)
		{
			GenKey++;
			CommandKey = GenKey;
		}
	}

	public virtual void Pack()
	{
	}

	public virtual void UnPack(bool compressed)
	{
	}

	public virtual void Complete()
	{
	}
}
