using PathologicalGames;
using UnityEngine;

public class SmallCandySimpleBombMixEffect : SmallCandyAfterMixEffect
{
	public override void CreateMix(Side sideDirection)
	{
		float num = 1f;
		GameObject spawnEffectObjectSmallCandyBombMixMake = SpawnStringEffect.GetSpawnEffectObjectSmallCandyBombMixMake(id);
		spawnEffectObjectSmallCandyBombMixMake.transform.position = base.transform.position + new Vector3(0f, 0f, -1f);
		PoolManager.PoolGameEffect.Despawn(spawnEffectObjectSmallCandyBombMixMake.transform, num);
		StartCoroutine(WaitCreateMix(num, sideDirection));
	}

	protected override void AfterRollingObjectCreate()
	{
		if ((bool)objRollingCandy)
		{
			objMixAddEffect = SpawnStringEffect.GetSpawnEffectObjectSmallCandyBombMixLine(id);
			objMixAddEffect.transform.parent = objRollingCandy.transform;
			objMixAddEffect.transform.localPosition = new Vector3(0f, 0f, -2f);
		}
	}

	protected override Chip GetAfterChipEffect()
	{
		GameObject spawnBlockObject = SpawnStringBlock.GetSpawnBlockObject(SpawnStringBlockType.CandyAfterMixEffectBomb);
		spawnBlockObject.transform.position = objRollingCandy.transform.position;
		Chip component = spawnBlockObject.GetComponent<SimpleBomb>();
		component.isCandyMix = true;
		component.id = id;
		return component;
	}
}
