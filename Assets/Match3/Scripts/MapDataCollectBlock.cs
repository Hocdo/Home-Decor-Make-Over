public class MapDataCollectBlock
{
	public string blockType;

	public int count;

	public MapDataCollectBlock(string type, int n)
	{
		blockType = type;
		count = n;
	}

	public string GetJsonFormat()
	{
		if (string.IsNullOrEmpty(blockType) || count == 0)
		{
			return string.Empty;
		}
		return blockType + ":" + count;
	}

	public CollectBlockType GetCollectBlockType()
	{
		return GetCollectBlockTypeFromString(blockType);
	}

	public static CollectBlockType GetCollectBlockTypeFromString(string blockType)
	{
		switch (blockType)
		{
		case "N1":
			return CollectBlockType.NormalRed;
		case "N2":
			return CollectBlockType.NormalOrange;
		case "N3":
			return CollectBlockType.NormalYellow;
		case "N4":
			return CollectBlockType.NormalGreen;
		case "N5":
			return CollectBlockType.NormalBlue;
		case "N6":
			return CollectBlockType.NormalPurple;
		case "N0D":
			return CollectBlockType.BringDown;
		case "RB":
			return CollectBlockType.RescueGingerMan;
		case "O1":
			return CollectBlockType.ChocolateJail;
		case "O2":
		case "O2.1":
		case "O2.2":
		case "O2.3":
			return CollectBlockType.Crunky;
		case "O4":
			return CollectBlockType.Crow;
		case "O7":
			return CollectBlockType.MagicalCrow;
		case "O5":
			return CollectBlockType.RescueFriend;
		case "O6":
			return CollectBlockType.PastryBag;
		case "B":
		case "B1":
		case "B2":
			return CollectBlockType.RockCandy;
		case "J":
			return CollectBlockType.Jelly;
		case "N0R":
			return CollectBlockType.Chameleon;
		case "N0L":
			return CollectBlockType.MakeCandyChip;
		case "N0H":
			return CollectBlockType.MakeHBomb;
		case "N0V":
			return CollectBlockType.MakeVBomb;
		case "N0W":
			return CollectBlockType.MakeRainbowChip;
		case "N0B":
			return CollectBlockType.MakeSimpleBomb;
		case "O8":
			return CollectBlockType.DiggingCandy;
		case "O8.3":
			return CollectBlockType.DiggingTreasure_G3;
		case "O8.2":
			return CollectBlockType.DiggingTreasure_G2;
		case "O8.1":
			return CollectBlockType.DiggingTreasure_G1;
		case "N0O":
			return CollectBlockType.OreoCracker;
		case "O11":
		case "O11.1":
		case "O11.2":
		case "O11.3":
		case "O11.4":
		case "O11.5":
		case "O11.6":
			return CollectBlockType.CarbonatedDrink;
		case "N0P":
			return CollectBlockType.PocketCandy;
		case "O13":
		case "O13.0":
		case "O13.1":
		case "O13.2":
		case "O13.3":
		case "O13.4":
		case "O13.5":
		case "O13.6":
			return CollectBlockType.NumberChocolate;
		default:
			return CollectBlockType.Null;
		}
	}
}
