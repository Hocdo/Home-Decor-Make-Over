using PathologicalGames;
using UnityEngine;

public static class SpawnStringEffect
{
	private static readonly DictEnumKeyGenericVal<string> dicSpawnStringType;

	static SpawnStringEffect()
	{
		dicSpawnStringType = new DictEnumKeyGenericVal<string>();
		dicSpawnStringType.Add(0, "EffSimpleRemoveR");
		dicSpawnStringType.Add(1, "EffSimpleRemoveO");
		dicSpawnStringType.Add(2, "EffSimpleRemoveY");
		dicSpawnStringType.Add(3, "EffSimpleRemoveG");
		dicSpawnStringType.Add(4, "EffSimpleRemoveB");
		dicSpawnStringType.Add(5, "EffSimpleRemoveP");
		dicSpawnStringType.Add(6, "EffBombRemoveR");
		dicSpawnStringType.Add(7, "EffBombRemoveO");
		dicSpawnStringType.Add(8, "EffBombRemoveY");
		dicSpawnStringType.Add(9, "EffBombRemoveG");
		dicSpawnStringType.Add(10, "EffBombRemoveB");
		dicSpawnStringType.Add(11, "EffBombRemoveP");
		dicSpawnStringType.Add(12, "EffHBombRemoveR");
		dicSpawnStringType.Add(13, "EffHBombRemoveO");
		dicSpawnStringType.Add(14, "EffHBombRemoveY");
		dicSpawnStringType.Add(15, "EffHBombRemoveG");
		dicSpawnStringType.Add(16, "EffHBombRemoveB");
		dicSpawnStringType.Add(17, "EffHBombRemoveP");
		dicSpawnStringType.Add(18, "EffVBombRemoveR");
		dicSpawnStringType.Add(19, "EffVBombRemoveO");
		dicSpawnStringType.Add(20, "EffVBombRemoveY");
		dicSpawnStringType.Add(21, "EffVBombRemoveG");
		dicSpawnStringType.Add(22, "EffVBombRemoveB");
		dicSpawnStringType.Add(23, "EffVBombRemoveP");
		dicSpawnStringType.Add(24, "EffRainbowThrow");
		dicSpawnStringType.Add(25, "EffBombBombMix");
		dicSpawnStringType.Add(26, "EffHBombBombMix");
		dicSpawnStringType.Add(27, "EffVBombBombMix");
		dicSpawnStringType.Add(28, "EffHVBombMix");
		dicSpawnStringType.Add(29, "EffRainbowRainbowMixRemove");
		dicSpawnStringType.Add(37, "Eff_jam_yellow_attack");
		dicSpawnStringType.Add(43, "Eff_jam_yellow_damage");
		dicSpawnStringType.Add(80, "ScoreEffect");
		dicSpawnStringType.Add(81, "Eff_bd_light");
		dicSpawnStringType.Add(82, "Obstacle_bd_cap");
		dicSpawnStringType.Add(83, "Obstacle_bd_bag");
		dicSpawnStringType.Add(84, "Obstacle_bd_shoes");
		dicSpawnStringType.Add(85, "Eff_bd_taillight");
		dicSpawnStringType.Add(86, "Eff_Collect_Obastacle");
		dicSpawnStringType.Add(91, "Eff_Collect_Red");
		dicSpawnStringType.Add(92, "Eff_Collect_Orange");
		dicSpawnStringType.Add(93, "Eff_Collect_Yellow");
		dicSpawnStringType.Add(94, "Eff_Collect_Green");
		dicSpawnStringType.Add(95, "Eff_Collect_Blue");
		dicSpawnStringType.Add(96, "Eff_Collect_Purple");
		dicSpawnStringType.Add(97, "Eff_Collect_block_Red");
		dicSpawnStringType.Add(98, "Eff_Collect_block_Orange");
		dicSpawnStringType.Add(99, "Eff_Collect_block_Yellow");
		dicSpawnStringType.Add(100, "Eff_Collect_block_Green");
		dicSpawnStringType.Add(101, "Eff_Collect_block_Blue");
		dicSpawnStringType.Add(102, "Eff_Collect_block_Purple");
		dicSpawnStringType.Add(104, "CollectMakeSpecialRainbowBlock");
		dicSpawnStringType.Add(105, "Eff_txt_5move");
		dicSpawnStringType.Add(106, "Eff_txt_shuffle");
		dicSpawnStringType.Add(107, "Eff_txt_sweet_party");
		dicSpawnStringType.Add(108, "Eff_Jam_make");
		dicSpawnStringType.Add(113, "Obstacle_chocolatejail_make");
		dicSpawnStringType.Add(116, "Eff_gauge_item");
		dicSpawnStringType.Add(134, "Eff_Success_Buy_Move");
		dicSpawnStringType.Add(135, "Eff_Success_Buy_magic_hammer");
		dicSpawnStringType.Add(137, "Eff_Success_Buy_candy_pack");
		dicSpawnStringType.Add(139, "Eff_Success_Buy_H_Bomb");
		dicSpawnStringType.Add(140, "Eff_Success_Buy_V_Bomb");
		dicSpawnStringType.Add(141, "Eff_text_add_stuffs");
		dicSpawnStringType.Add(142, "Eff_move_booster");
		dicSpawnStringType.Add(144, "Eff_Rainbow_idle");
	}

	public static string Get(SpawnStringEffectType spawnStringType)
	{
		if (!dicSpawnStringType.ContainsKey((int)spawnStringType))
		{
			return string.Empty;
		}
		return dicSpawnStringType[(int)spawnStringType];
	}

	public static GameObject GetSpawnEffectObject(SpawnStringEffectType spawnStringType)
	{
		return PoolManager.PoolGameEffect.Spawn(Get(spawnStringType)).gameObject;
	}

	public static GameObject GetSpawnEffectObjectSimpleChipCrush(int chipID)
	{
		return GetSpawnEffectObject((SpawnStringEffectType)chipID);
	}

	public static GameObject GetSpawnEffectObjectSimpleBombCrush(int chipID)
	{
		return GetSpawnEffectObject((SpawnStringEffectType)(6 + chipID));
	}

	public static GameObject GetSpawnEffectObjectHBombCrush(int chipID)
	{
		return GetSpawnEffectObject((SpawnStringEffectType)(12 + chipID));
	}

	public static GameObject GetSpawnEffectObjectVBombCrush(int chipID)
	{
		return GetSpawnEffectObject((SpawnStringEffectType)(18 + chipID));
	}

	public static GameObject GetSpawnEffectObjectRainbowThrow(int chipID)
	{
		return GetSpawnEffectObject(SpawnStringEffectType.RainbowThrow);
	}

	public static GameObject GetSpawnEffectObjectHBombBombMixCrush(int chipID)
	{
		return GetSpawnEffectObject(SpawnStringEffectType.HBombBombMixCrush);
	}

	public static GameObject GetSpawnEffectObjectVBombBombMixCrush(int chipID)
	{
		return GetSpawnEffectObject(SpawnStringEffectType.VBombBombMixCrush);
	}

	public static GameObject GetSpawnEffectObjectHVBombMixCrush(int chipID)
	{
		return GetSpawnEffectObject(SpawnStringEffectType.HVBombMixCrush);
	}

	public static GameObject GetSpawnEffectObjectJamShoot(int chipID)
	{
		return GetSpawnEffectObject((SpawnStringEffectType)(32 + chipID));
	}

	public static GameObject GetSpawnEffectObjectJamSpread(int chipID)
	{
		return GetSpawnEffectObject((SpawnStringEffectType)(38 + chipID));
	}

	public static GameObject GetSpawnEffectObjectBringDownSwallow(int blockID)
	{
		return GetSpawnEffectObject((SpawnStringEffectType)(82 + blockID));
	}

	public static GameObject GetSpawnEffectObjectSmallCandyHVMixMake(int chipID)
	{
		return GetSpawnEffectObject((SpawnStringEffectType)(44 + chipID));
	}

	public static GameObject GetSpawnEffectObjectSmallCandyHVMixRemove(int chipID)
	{
		return GetSpawnEffectObject((SpawnStringEffectType)(50 + chipID));
	}

	public static GameObject GetSpawnEffectObjectSmallCandyHVMixLine(int chipID)
	{
		return GetSpawnEffectObject((SpawnStringEffectType)(56 + chipID));
	}

	public static GameObject GetSpawnEffectObjectSmallCandyBombMixMake(int chipID)
	{
		return GetSpawnEffectObject((SpawnStringEffectType)(62 + chipID));
	}

	public static GameObject GetSpawnEffectObjectSmallCandyBombMixRemove(int chipID)
	{
		return GetSpawnEffectObject((SpawnStringEffectType)(68 + chipID));
	}

	public static GameObject GetSpawnEffectObjectSmallCandyBombMixLine(int chipID)
	{
		return GetSpawnEffectObject((SpawnStringEffectType)(74 + chipID));
	}
}
