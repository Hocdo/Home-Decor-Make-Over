using PathologicalGames;
using System.Collections;
using UnityEngine;

public class CandyChip : Chip
{
	public override void Awake()
	{
		base.Awake();
		chipType = ChipType.CandyChip;
		aniType = AnimationController.IdleAnimationType.SmallCandy;
	}

	public override void PlayIdleAnimation(AnimationController.IdleAnimationType type, AnimationProperty prop)
	{
		if (GetComponent<Animator>() != null)
		{
			GetComponent<Animator>().SetTrigger(AnimationController.IdleAnimationName);
			if (!prop.isLoop)
			{
				StartCoroutine(StopAnimation(prop.time));
			}
		}
	}

	private IEnumerator StopAnimation(float time)
	{
		yield return new WaitForSeconds(time);
		if (GetComponent<Animator>() != null)
		{
			GetComponent<Animator>().SetTrigger(AnimationController.IdleStopAnimationName);
		}
	}

	public override void ShowCreateEffect()
	{
		base.ShowCreateEffect();
		GameObject spawnBlockObject = SpawnStringBlock.GetSpawnBlockObject(SpawnStringBlockType.SmallCandyMake);
		spawnBlockObject.transform.position = base.transform.position;
		spawnBlockObject.transform.parent = base.transform;
		spawnBlockObject.transform.localScale = Vector3.one;
		canMove = false;
		StartCoroutine(WaitCreateEffect(spawnBlockObject));
	}

	private IEnumerator WaitCreateEffect(GameObject objEffect)
	{
		yield return new WaitForSeconds(GameMain.BlockDropDelayTime);
		canMove = true;
		yield return new WaitForSeconds(0.58f - GameMain.BlockDropDelayTime);
		objEffect.transform.parent = PoolManager.PoolGameBlocks.transform;
		PoolManager.PoolGameBlocks.Despawn(objEffect.transform);
		Utils.EnableAllSpriteRenderer(base.gameObject);
	}

	public void CrushCandyChip()
	{
		ParentRemove();
		PoolManager.PoolGameBlocks.Despawn(base.transform);
	}
}
