using System;

[Serializable]
public class ReqPacketSaveMapData : ReqPacketBase
{
	public string ab_test = "a";

	public string board_data;

	public string drop;

	public string drop_block_prob;

	public int gid;

	public string map_param;

	public int mid;

	public string moving;

	public string road;

	public string rotation;

	public ReqPacketSaveMapData(int gid, int mid, GoalTarget goalTarget, MapBoardData boardData)
	{
		this.gid = gid;
		this.mid = mid;
		road = boardData.GetRoadJsonFormat();
		drop_block_prob = boardData.GetDropBlockProbJsonFormat();
		moving = boardData.GetMovingJsonFormat();
		rotation = boardData.GetRotaitonJsonFormat();
		board_data = boardData.GetBoardJsonFormat(goalTarget);
		drop = boardData.GetGeneratorDropJsonFormat();
		if (drop.Length == 0)
		{
			drop = boardData.GetGeneratorSpecialDropJsonFormat();
		}
		else
		{
			drop = drop + "," + boardData.GetGeneratorSpecialDropJsonFormat();
		}
		map_param = boardData.GetMapParamJsonFormat();
		ab_test = "a";
	}
}
