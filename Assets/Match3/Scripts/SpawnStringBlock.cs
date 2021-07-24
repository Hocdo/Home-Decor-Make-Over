using PathologicalGames;
using UnityEngine;

public static class SpawnStringBlock
{
	private static readonly DictEnumKeyGenericVal<string> dicSpawnStringType;

	static SpawnStringBlock()
	{
		dicSpawnStringType = new DictEnumKeyGenericVal<string>();
		dicSpawnStringType.Add(0, "EffSimpleR");
		dicSpawnStringType.Add(1, "EffSimpleO");
		dicSpawnStringType.Add(2, "EffSimpleY");
		dicSpawnStringType.Add(3, "EffSimpleG");
		dicSpawnStringType.Add(4, "EffSimpleB");
		dicSpawnStringType.Add(5, "EffSimpleP");
		dicSpawnStringType.Add(6, "EffBombR");
		dicSpawnStringType.Add(7, "EffBombO");
		dicSpawnStringType.Add(8, "EffBombY");
		dicSpawnStringType.Add(9, "EffBombG");
		dicSpawnStringType.Add(10, "EffBombB");
		dicSpawnStringType.Add(11, "EffBombP");
		dicSpawnStringType.Add(12, "EffHBombR");
		dicSpawnStringType.Add(13, "EffHBombO");
		dicSpawnStringType.Add(14, "EffHBombY");
		dicSpawnStringType.Add(15, "EffHBombG");
		dicSpawnStringType.Add(16, "EffHBombB");
		dicSpawnStringType.Add(17, "EffHBombP");
		dicSpawnStringType.Add(18, "EffVBombR");
		dicSpawnStringType.Add(19, "EffVBombO");
		dicSpawnStringType.Add(20, "EffVBombY");
		dicSpawnStringType.Add(21, "EffVBombG");
		dicSpawnStringType.Add(22, "EffVBombB");
		dicSpawnStringType.Add(23, "EffVBombP");
		dicSpawnStringType.Add(24, "EffRainbow");
		dicSpawnStringType.Add(25, "EffRainbowUse");
		dicSpawnStringType.Add(27, "BringDownObjectHat");
		dicSpawnStringType.Add(28, "BringDownObjectBag");
		dicSpawnStringType.Add(29, "BringDownObjectShoes");
		dicSpawnStringType.Add(30, "SimpleSimpleMixEffect");
		dicSpawnStringType.Add(31, "HVBombMixEffect");
		dicSpawnStringType.Add(32, "SimpleHBombMixEffect");
		dicSpawnStringType.Add(33, "SimpleVBombMixEffect");
		dicSpawnStringType.Add(34, "RainbowChipMixEffect");
		dicSpawnStringType.Add(35, "RainbowRainbowMixEffect");
		dicSpawnStringType.Add(36, "RainbowSimpleMixEffect");
		dicSpawnStringType.Add(37, "RainbowHBombMixEffect");
		dicSpawnStringType.Add(38, "RainbowVBombMixEffect");
		dicSpawnStringType.Add(52, "EffBombMakeR");
		dicSpawnStringType.Add(53, "EffBombMakeO");
		dicSpawnStringType.Add(54, "EffBombMakeY");
		dicSpawnStringType.Add(55, "EffBombMakeG");
		dicSpawnStringType.Add(56, "EffBombMakeB");
		dicSpawnStringType.Add(57, "EffBombMakeP");
		dicSpawnStringType.Add(58, "EffHBombMakeR");
		dicSpawnStringType.Add(59, "EffHBombMakeO");
		dicSpawnStringType.Add(60, "EffHBombMakeY");
		dicSpawnStringType.Add(61, "EffHBombMakeG");
		dicSpawnStringType.Add(62, "EffHBombMakeB");
		dicSpawnStringType.Add(63, "EffHBombMakeP");
		dicSpawnStringType.Add(64, "EffVBombMakeR");
		dicSpawnStringType.Add(65, "EffVBombMakeO");
		dicSpawnStringType.Add(66, "EffVBombMakeY");
		dicSpawnStringType.Add(67, "EffVBombMakeG");
		dicSpawnStringType.Add(68, "EffVBombMakeB");
		dicSpawnStringType.Add(69, "EffVBombMakeP");
		dicSpawnStringType.Add(71, "EffRainbowMake");
		dicSpawnStringType.Add(73, "EffRainbowRainbowMixMake");
	}

	public static string Get(SpawnStringBlockType spawnStringType)
	{
		if (!dicSpawnStringType.ContainsKey((int)spawnStringType))
		{
			return string.Empty;
		}
		return dicSpawnStringType[(int)spawnStringType];
	}

	public static GameObject GetSpawnBlockObject(SpawnStringBlockType spawnStringType)
	{
		return PoolManager.PoolGameBlocks.Spawn(Get(spawnStringType)).gameObject;
	}

	public static GameObject GetSpawnBlockObjectSimpleChip(int chipID)
	{
		return GetSpawnBlockObject((SpawnStringBlockType)chipID);
	}

	public static GameObject GetSpawnBlockObjectSimpleBomb(int chipID)
	{
		return GetSpawnBlockObject((SpawnStringBlockType)(6 + chipID));
	}

	public static GameObject GetSpawnBlockObjectHBomb(int chipID)
	{
		return GetSpawnBlockObject((SpawnStringBlockType)(12 + chipID));
	}

	public static GameObject GetSpawnBlockObjectVBomb(int chipID)
	{
		return GetSpawnBlockObject((SpawnStringBlockType)(18 + chipID));
	}

	public static GameObject GetSpawnBlockObjectPlusMoveChip(int chipID)
	{
		return GetSpawnBlockObject((SpawnStringBlockType)(46 + chipID));
	}

	public static GameObject GetSpawnBlockObjectBombMake(int chipID)
	{
		return GetSpawnBlockObject((SpawnStringBlockType)(52 + chipID));
	}

	public static GameObject GetSpawnBlockObjectHBombMake(int chipID)
	{
		return GetSpawnBlockObject((SpawnStringBlockType)(58 + chipID));
	}

	public static GameObject GetSpawnBlockObjectVBombMake(int chipID)
	{
		return GetSpawnBlockObject((SpawnStringBlockType)(64 + chipID));
	}

	public static GameObject GetSpawnBlockObjectBringDown(int blockID)
	{
		return GetSpawnBlockObject((SpawnStringBlockType)(27 + blockID));
	}
}
