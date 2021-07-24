using System.Collections.Generic;

public static class EnableEpisodeCollect
{
	private static readonly Dictionary<CollectBlockType, int> dictEnableEpisode;

	static EnableEpisodeCollect()
	{
		dictEnableEpisode = new Dictionary<CollectBlockType, int>();
		dictEnableEpisode.Add(CollectBlockType.Null, 0);
		dictEnableEpisode.Add(CollectBlockType.NormalRed, 0);
		dictEnableEpisode.Add(CollectBlockType.NormalOrange, 0);
		dictEnableEpisode.Add(CollectBlockType.NormalYellow, 0);
		dictEnableEpisode.Add(CollectBlockType.NormalGreen, 0);
		dictEnableEpisode.Add(CollectBlockType.NormalBlue, 0);
		dictEnableEpisode.Add(CollectBlockType.NormalPurple, 0);
		dictEnableEpisode.Add(CollectBlockType.RescueGingerMan, 0);
		dictEnableEpisode.Add(CollectBlockType.BringDown, 0);
		dictEnableEpisode.Add(CollectBlockType.RockCandy, 0);
		dictEnableEpisode.Add(CollectBlockType.Crow, 0);
		dictEnableEpisode.Add(CollectBlockType.RescueFriend, 0);
		dictEnableEpisode.Add(CollectBlockType.Crunky, 0);
		dictEnableEpisode.Add(CollectBlockType.ChocolateJail, 0);
		dictEnableEpisode.Add(CollectBlockType.Jelly, 0);
		dictEnableEpisode.Add(CollectBlockType.Chameleon, 0);
		dictEnableEpisode.Add(CollectBlockType.PastryBag, 0);
		dictEnableEpisode.Add(CollectBlockType.MagicalCrow, 0);
		dictEnableEpisode.Add(CollectBlockType.MakeSimpleBomb, 0);
		dictEnableEpisode.Add(CollectBlockType.MakeHBomb, 0);
		dictEnableEpisode.Add(CollectBlockType.MakeVBomb, 0);
		dictEnableEpisode.Add(CollectBlockType.MakeCandyChip, 0);
		dictEnableEpisode.Add(CollectBlockType.MakeRainbowChip, 0);
		dictEnableEpisode.Add(CollectBlockType.DiggingCandy, 0);
		dictEnableEpisode.Add(CollectBlockType.DiggingTreasure_G3, 0);
		dictEnableEpisode.Add(CollectBlockType.DiggingTreasure_G2, 0);
		dictEnableEpisode.Add(CollectBlockType.DiggingTreasure_G1, 0);
		dictEnableEpisode.Add(CollectBlockType.SweetRoadConnect, 0);
		dictEnableEpisode.Add(CollectBlockType.OreoCracker, 0);
		dictEnableEpisode.Add(CollectBlockType.CarbonatedDrink, 0);
		dictEnableEpisode.Add(CollectBlockType.PocketCandy, 0);
		dictEnableEpisode.Add(CollectBlockType.NumberChocolate, 0);
	}

	public static bool IsEnable(CollectBlockType collectType, int episodeNo)
	{
		if (!dictEnableEpisode.ContainsKey(collectType))
		{
			return false;
		}
		if (dictEnableEpisode[collectType] > episodeNo)
		{
			return false;
		}
		return true;
	}
}
