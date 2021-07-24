using UnityEngine;

public class SmallCandyRainbowBombMixEffect : SmallCandyAfterMixEffect
{
	private GameObject objRollingEffect;

	public GameObject PrefabRollingEffect;

	protected override void AfterRollingObjectCreate()
	{
		base.AfterRollingObjectCreate();
		if ((bool)objRollingCandy)
		{
			objRollingEffect = Object.Instantiate(PrefabRollingEffect);
			objRollingEffect.transform.parent = objRollingCandy.transform;
			objRollingEffect.transform.localPosition = Vector3.zero;
		}
	}

	public override void OnDespawned()
	{
		base.OnDespawned();
		if ((bool)objRollingEffect)
		{
			UnityEngine.Object.Destroy(objRollingEffect);
		}
	}

	protected override Chip GetAfterChipEffect()
	{
		GameObject spawnBlockObject = SpawnStringBlock.GetSpawnBlockObject(SpawnStringBlockType.RainbowChipMixEffect);
		spawnBlockObject.transform.parent = base.transform.parent;
		spawnBlockObject.transform.localPosition = Vector3.zero;
		Chip component = spawnBlockObject.GetComponent<RainbowChipMixEffect>();
		component.isCandyMix = true;
		return component;
	}
}
