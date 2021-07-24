using System;

[Serializable]
public class ReqPacketClearAllMapData : ReqPacketBase
{
	public string ab_test = "a";

	public int del;

	public int gid;

	public int mid;

	public ReqPacketClearAllMapData(int gid)
	{
		this.gid = gid;
		mid = 999999;
		del = 1;
		ab_test = "a";
	}
}
