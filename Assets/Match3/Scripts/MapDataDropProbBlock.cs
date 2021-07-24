public class MapDataDropProbBlock : MapDataDropBlock
{
	public int prob;

	public MapDataDropProbBlock(ChipType chipType, int chipColor, int prob)
		: base(chipType, chipColor)
	{
		this.prob = prob;
	}
}
