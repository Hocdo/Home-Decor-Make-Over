using System;

[Serializable]
public class PacketGameMapData : ICloneable
{
	public string board_data;

	public string drop;

	public string drop_block_prob;

	public int gid;

	public string map_param;

	public int mid;

	public string moving;

	public string road;

	public string rotation;

	public PacketGameMapData()
	{
	}

	public PacketGameMapData(int gid, int mid, GoalTarget goalTarget, MapBoardData boardData)
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
	}

	public object Clone()
	{
		PacketGameMapData packetGameMapData = new PacketGameMapData();
		packetGameMapData.gid = gid;
		packetGameMapData.mid = mid;
		packetGameMapData.road = road;
		packetGameMapData.drop_block_prob = drop_block_prob;
		packetGameMapData.moving = moving;
		packetGameMapData.rotation = rotation;
		packetGameMapData.board_data = board_data;
		packetGameMapData.drop = drop;
		packetGameMapData.map_param = map_param;
		return packetGameMapData;
	}
}
