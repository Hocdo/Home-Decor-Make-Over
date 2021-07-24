using System;

[Serializable]
public class ResPacketBase : PacketBase
{
	public int crypt_fail;

	public int network_result;

	public int order;

	public string originalJson = string.Empty;
}
