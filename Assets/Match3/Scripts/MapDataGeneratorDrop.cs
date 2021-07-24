using System.Text;

public class MapDataGeneratorDrop
{
	public MapDataDropBlock[] dropBlocks = new MapDataDropBlock[MapData.MaxDropList];

	public int generatorParseX;

	public int generatorParseY;

	public MapDataGeneratorDrop()
	{
		for (int i = 0; i < dropBlocks.Length; i++)
		{
			dropBlocks[i] = new MapDataDropBlock(ChipType.SimpleChip, 0);
		}
	}

	public MapDataGeneratorDrop(int genX, int genY)
		: this()
	{
		generatorParseX = genX;
		generatorParseY = genY;
	}

	public bool IsAllRandomValue()
	{
		for (int i = 0; i < dropBlocks.Length; i++)
		{
			if (dropBlocks[i].chipType != ChipType.SimpleChip || dropBlocks[i].chipColor != 0)
			{
				return false;
			}
		}
		return true;
	}

	public string GetJsonFormat()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (IsAllRandomValue())
		{
			return string.Empty;
		}
		stringBuilder.Append(generatorParseX);
		stringBuilder.Append(generatorParseY);
		stringBuilder.Append("=");
		for (int i = 0; i < dropBlocks.Length; i++)
		{
			if (i > 0)
			{
				stringBuilder.Append(":");
			}
			if (dropBlocks[i].chipType == ChipType.SimpleChip && dropBlocks[i].chipColor == 0)
			{
				stringBuilder.Append("0");
			}
			else
			{
				stringBuilder.Append(MapData.GetChipJsonFormat(dropBlocks[i].chipType, dropBlocks[i].chipColor));
			}
		}
		return stringBuilder.ToString();
	}
}
