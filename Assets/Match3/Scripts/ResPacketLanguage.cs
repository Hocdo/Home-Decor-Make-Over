using System;

[Serializable]
public class ResPacketLanguage : ResPacketBase, IPacketBaseError
{
	public PacketDataLanguage m_SPEC_LANG;

	public bool IsError()
	{
		return m_SPEC_LANG.IsError();
	}
}
