using PathologicalGames;
using UnityEngine;

public static class SpawnStringGamePlaying
{
	private static readonly DictEnumKeyGenericVal<string> dicSpawnStringType;

	static SpawnStringGamePlaying()
	{
		dicSpawnStringType = new DictEnumKeyGenericVal<string>();
		dicSpawnStringType.Add(0, "Eff_board_light_04");
		dicSpawnStringType.Add(1, "Eff_Boss_stick_appear");
		dicSpawnStringType.Add(2, "Eff_Boss_stick");
		dicSpawnStringType.Add(3, "Eff_witch_hit");
		dicSpawnStringType.Add(4, "Eff_Crow_move");
		dicSpawnStringType.Add(5, "Eff_Crow_attack");
		dicSpawnStringType.Add(6, "Eff_Crow_remove");
		dicSpawnStringType.Add(7, "Eff_MC_move");
		dicSpawnStringType.Add(8, "Eff_MC_attack");
		dicSpawnStringType.Add(9, "Eff_MC_remove");
		dicSpawnStringType.Add(10, "Eff_MC_hit");
		dicSpawnStringType.Add(11, "Eff_MC_magic");
		dicSpawnStringType.Add(12, "Effect_Digging_Collect");
		dicSpawnStringType.Add(13, "Effect_Digging_Collect_Treasure_G1");
		dicSpawnStringType.Add(14, "Effect_Digging_Collect_Treasure_G2");
		dicSpawnStringType.Add(15, "Effect_Digging_Collect_Treasure_G3");
		dicSpawnStringType.Add(16, "Obstacle_Pocket_collect_02");
	}

	public static string Get(SpawnStringGamePlayingType spawnStringType)
	{
		if (!dicSpawnStringType.ContainsKey((int)spawnStringType))
		{
			return string.Empty;
		}
		return dicSpawnStringType[(int)spawnStringType];
	}

	public static GameObject GetSpawnObject(SpawnStringGamePlayingType spawnStringType)
	{
		return PoolManager.PoolGamePlaying.Spawn(Get(spawnStringType)).gameObject;
	}
}
