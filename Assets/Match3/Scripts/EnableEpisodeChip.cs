using System.Collections.Generic;

public static class EnableEpisodeChip
{
	private static readonly Dictionary<ChipType, int> dictEnableEpisode;

	static EnableEpisodeChip()
	{
		dictEnableEpisode = new Dictionary<ChipType, int>();
		dictEnableEpisode.Add(ChipType.None, 0);
		dictEnableEpisode.Add(ChipType.SimpleChip, 0);
		dictEnableEpisode.Add(ChipType.BringDown, 0);
		dictEnableEpisode.Add(ChipType.SimpleBomb, 0);
		dictEnableEpisode.Add(ChipType.CrossBomb, 0);
		dictEnableEpisode.Add(ChipType.RainbowBomb, 0);
		dictEnableEpisode.Add(ChipType.HBomb, 0);
		dictEnableEpisode.Add(ChipType.VBomb, 0);
		dictEnableEpisode.Add(ChipType.DiagonalRBomb, 0);
		dictEnableEpisode.Add(ChipType.DiagonalLBomb, 0);
		dictEnableEpisode.Add(ChipType.MouseChip, 0);
		dictEnableEpisode.Add(ChipType.CandyChip, 0);
		dictEnableEpisode.Add(ChipType.ChamelonChip, 0);
		dictEnableEpisode.Add(ChipType.OreoCracker, 0);
		dictEnableEpisode.Add(ChipType.NumberChocolate, 0);
	}

	public static bool IsEnable(ChipType chipType, int episodeNo)
	{
		if (!dictEnableEpisode.ContainsKey(chipType))
		{
			return false;
		}
		if (dictEnableEpisode[chipType] > episodeNo)
		{
			return false;
		}
		return true;
	}
}
