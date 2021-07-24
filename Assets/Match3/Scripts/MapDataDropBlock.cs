public class MapDataDropBlock
{
	public int chipColor;

	public ChipType chipType = ChipType.SimpleChip;

	public MapDataDropBlock(ChipType chipType, int chipColor)
	{
		this.chipType = chipType;
		this.chipColor = chipColor;
	}

	public MapDataDropBlock(string strJson)
	{
	}
}
