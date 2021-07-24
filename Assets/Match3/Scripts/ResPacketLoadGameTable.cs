using System;

[Serializable]
public class ResPacketLoadGameTable : ResPacketBase
{
	public PacketDataSpecCoin[] m_SPEC_COIN;

	public PacketDataSpecItem m_SPEC_ITEM;

	public PacketGameLevelData[] m_SPEC_GAME;

	public PacketGameMapData[] m_SPEC_MAP;

	public PacketDataOption[] m_SPEC_OPTION;
}
