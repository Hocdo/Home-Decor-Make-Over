using PathologicalGames;
using UnityEngine;

public class SmallCandyHVBombMixEffect : SmallCandyAfterMixEffect
{
	public override void CreateMix(Side sideDirection)
	{
		float num = 0.5f;
		GameObject spawnEffectObjectSmallCandyHVMixMake = SpawnStringEffect.GetSpawnEffectObjectSmallCandyHVMixMake(id);
		spawnEffectObjectSmallCandyHVMixMake.transform.position = base.transform.position + new Vector3(0f, 0f, -1f);
		PoolManager.PoolGameEffect.Despawn(spawnEffectObjectSmallCandyHVMixMake.transform, num);
		StartCoroutine(WaitCreateMix(num, sideDirection));
	}

	protected override void AfterRollingObjectCreate()
	{
		if ((bool)objRollingCandy)
		{
			objMixAddEffect = SpawnStringEffect.GetSpawnEffectObjectSmallCandyHVMixLine(id);
			objMixAddEffect.transform.parent = objRollingCandy.transform;
			objMixAddEffect.transform.localPosition = new Vector3(0f, 0f, -2f);
		}
	}

	protected override Chip GetAfterChipEffect()
	{
		GameObject spawnBlockObject = SpawnStringBlock.GetSpawnBlockObject(SpawnStringBlockType.CandyAfterMixEffectHVBomb);
		spawnBlockObject.transform.position = objRollingCandy.transform.position;
		Chip component = spawnBlockObject.GetComponent<HVBombMixEffect>();
		component.isCandyMix = true;
		component.id = id;
		return component;
	}
}
