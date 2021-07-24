using System.Collections.Generic;

public static class EnableEpisodeBlockType
{
	private static readonly Dictionary<IBlockType, int> dictEnableEpisode;

	static EnableEpisodeBlockType()
	{
		dictEnableEpisode = new Dictionary<IBlockType, int>();
		dictEnableEpisode.Add(IBlockType.None, 0);
		dictEnableEpisode.Add(IBlockType.ChocolateJail, 0);
		dictEnableEpisode.Add(IBlockType.FruitsBox, 0);
		dictEnableEpisode.Add(IBlockType.CandyFactory_1, 0);
		dictEnableEpisode.Add(IBlockType.CandyFactory_2, 0);
		dictEnableEpisode.Add(IBlockType.CandyFactory_3, 0);
		dictEnableEpisode.Add(IBlockType.CandyFactory_4, 0);
		dictEnableEpisode.Add(IBlockType.CandyFactory_5, 0);
		dictEnableEpisode.Add(IBlockType.CandyFactory_6, 0);
		dictEnableEpisode.Add(IBlockType.Crunky_HP1, 0);
		dictEnableEpisode.Add(IBlockType.Crunky_HP2, 0);
		dictEnableEpisode.Add(IBlockType.Crunky_HP3, 0);
		dictEnableEpisode.Add(IBlockType.Mouse, 0);
		dictEnableEpisode.Add(IBlockType.MouseBlockEnd, 0);
		dictEnableEpisode.Add(IBlockType.Weed, 0);
		dictEnableEpisode.Add(IBlockType.RescueFriend, 0);
		dictEnableEpisode.Add(IBlockType.Crow, 0);
		dictEnableEpisode.Add(IBlockType.MagicalCrow, 0);
		dictEnableEpisode.Add(IBlockType.PastryBag, 0);
		dictEnableEpisode.Add(IBlockType.Ribbon, 0);
		dictEnableEpisode.Add(IBlockType.GreenSlime, 0);
		dictEnableEpisode.Add(IBlockType.Digging_HP1, 0);
		dictEnableEpisode.Add(IBlockType.Digging_HP2, 0);
		dictEnableEpisode.Add(IBlockType.Digging_HP3, 0);
		dictEnableEpisode.Add(IBlockType.Digging_HP1_Collect, 0);
		dictEnableEpisode.Add(IBlockType.Digging_HP2_Collect, 0);
		dictEnableEpisode.Add(IBlockType.Digging_HP3_Collect, 0);
		dictEnableEpisode.Add(IBlockType.Digging_HP1_Bomb1, 0);
		dictEnableEpisode.Add(IBlockType.Digging_HP2_Bomb1, 0);
		dictEnableEpisode.Add(IBlockType.Digging_HP3_Bomb1, 0);
		dictEnableEpisode.Add(IBlockType.Digging_HP1_Bomb2, 0);
		dictEnableEpisode.Add(IBlockType.Digging_HP2_Bomb2, 0);
		dictEnableEpisode.Add(IBlockType.Digging_HP3_Bomb2, 0);
		dictEnableEpisode.Add(IBlockType.Digging_HP1_Bomb3, 0);
		dictEnableEpisode.Add(IBlockType.Digging_HP2_Bomb3, 0);
		dictEnableEpisode.Add(IBlockType.Digging_HP3_Bomb3, 0);
		dictEnableEpisode.Add(IBlockType.MilkCarton, 0);
		dictEnableEpisode.Add(IBlockType.Pocket, 0);
		dictEnableEpisode.Add(IBlockType.Yarn, 0);
		dictEnableEpisode.Add(IBlockType.Digging_HP1_Treasure_G1, 45001);
		dictEnableEpisode.Add(IBlockType.Digging_HP2_Treasure_G1, 45001);
		dictEnableEpisode.Add(IBlockType.Digging_HP3_Treasure_G1, 45001);
		dictEnableEpisode.Add(IBlockType.Digging_HP1_Treasure_G2, 45001);
		dictEnableEpisode.Add(IBlockType.Digging_HP2_Treasure_G2, 45001);
		dictEnableEpisode.Add(IBlockType.Digging_HP3_Treasure_G2, 45001);
		dictEnableEpisode.Add(IBlockType.Digging_HP1_Treasure_G3, 45001);
		dictEnableEpisode.Add(IBlockType.Digging_HP2_Treasure_G3, 45001);
		dictEnableEpisode.Add(IBlockType.Digging_HP3_Treasure_G3, 45001);
		dictEnableEpisode.Add(IBlockType.SpriteDrink_1_HP1, 0);
		dictEnableEpisode.Add(IBlockType.SpriteDrink_1_HP2, 0);
		dictEnableEpisode.Add(IBlockType.SpriteDrink_1_HP3, 0);
		dictEnableEpisode.Add(IBlockType.SpriteDrink_2_HP1, 0);
		dictEnableEpisode.Add(IBlockType.SpriteDrink_2_HP2, 0);
		dictEnableEpisode.Add(IBlockType.SpriteDrink_2_HP3, 0);
		dictEnableEpisode.Add(IBlockType.SpriteDrink_3_HP1, 0);
		dictEnableEpisode.Add(IBlockType.SpriteDrink_3_HP2, 0);
		dictEnableEpisode.Add(IBlockType.SpriteDrink_3_HP3, 0);
		dictEnableEpisode.Add(IBlockType.SpriteDrink_4_HP1, 0);
		dictEnableEpisode.Add(IBlockType.SpriteDrink_4_HP2, 0);
		dictEnableEpisode.Add(IBlockType.SpriteDrink_4_HP3, 0);
		dictEnableEpisode.Add(IBlockType.SpriteDrink_5_HP1, 0);
		dictEnableEpisode.Add(IBlockType.SpriteDrink_5_HP2, 0);
		dictEnableEpisode.Add(IBlockType.SpriteDrink_5_HP3, 0);
		dictEnableEpisode.Add(IBlockType.SpriteDrink_6_HP1, 0);
		dictEnableEpisode.Add(IBlockType.SpriteDrink_6_HP2, 0);
		dictEnableEpisode.Add(IBlockType.SpriteDrink_6_HP3, 0);
		dictEnableEpisode.Add(IBlockType.NumberChocolate, 0);
		dictEnableEpisode.Add(IBlockType.NumberChocolate_1, 0);
		dictEnableEpisode.Add(IBlockType.NumberChocolate_2, 0);
		dictEnableEpisode.Add(IBlockType.NumberChocolate_3, 0);
		dictEnableEpisode.Add(IBlockType.NumberChocolate_4, 0);
		dictEnableEpisode.Add(IBlockType.NumberChocolate_5, 0);
		dictEnableEpisode.Add(IBlockType.NumberChocolate_6, 0);
	}

	public static bool IsEnable(IBlockType blockType, int episodeNo)
	{
		return true;
	}
}
