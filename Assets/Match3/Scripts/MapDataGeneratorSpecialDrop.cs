using System.Text;

public class MapDataGeneratorSpecialDrop
{
	public MapDataDropProbBlock[] dropBlocks = new MapDataDropProbBlock[MapData.MaxDropProbList];

	public int generatorParseX;

	public int generatorParseY;

	public int totalProb;

	public MapDataGeneratorSpecialDrop()
	{
		for (int i = 0; i < dropBlocks.Length; i++)
		{
			dropBlocks[i] = new MapDataDropProbBlock(ChipType.SimpleChip, 0, 0);
		}
	}

	public MapDataGeneratorSpecialDrop(int genX, int genY)
		: this()
	{
		generatorParseX = genX;
		generatorParseY = genY;
	}

	public string GetJsonFormat()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("GS|");
		stringBuilder.Append(generatorParseX);
		stringBuilder.Append(generatorParseY);
		stringBuilder.Append("=");
		int num = 0;
		for (int i = 0; i < dropBlocks.Length; i++)
		{
			if (dropBlocks[i].prob > 0)
			{
				if (num > 0)
				{
					stringBuilder.Append(":");
				}
				stringBuilder.Append(MapData.GetChipJsonFormat(dropBlocks[i].chipType, dropBlocks[i].chipColor));
				stringBuilder.Append("-" + dropBlocks[i].prob);
				num++;
			}
		}
		if (num == 0)
		{
			return string.Empty;
		}
		return stringBuilder.ToString();
	}
}
