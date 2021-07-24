using System;

[Serializable]
public class ReqPacketLoadGameTable : ReqPacketBase
{
	public string data_info;

	public int patch;

	public ReqPacketLoadGameTable(int _patchValue, string _tableInfoListValue = "")
	{
		patch = _patchValue;
		data_info = _tableInfoListValue;
	}
}
