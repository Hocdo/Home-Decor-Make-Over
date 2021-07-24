using PathologicalGames;
using System.Collections;
using UnityEngine;

public class RescueFriend : BlockInterface
{
	public override bool EnableBoosterHammer => false;

	public override bool EnableBoosterCandyCrane => false;

	public override bool EnableBoosterCandyPack => false;

	public void Rescue()
	{
		if (!destroying)
		{
			destroying = true;
			StartCoroutine(ProcessBlockCrush());
		}
	}

	private IEnumerator ProcessBlockCrush()
	{
		GameMain.main.PrevThrowCollectItem(CollectBlockType.RescueFriend);
		Utils.DisableAllSpriteRenderer(base.gameObject);
		GameObject objCrush = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.RescueFriendCrush);
		SoundSFX.Play(SFXIndex.RescueBlockRemove);
		if ((bool)objCrush)
		{
			objCrush.transform.position = base.transform.position;
			PoolManager.PoolGameEffect.Despawn(objCrush.transform, 1f);
			GameObject objCollect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.RescueFriendCollect);
			objCollect.transform.position = base.transform.position;
			yield return new WaitForSeconds(1f);
			GameMain.main.ThrowCollectItem(objCollect, CollectBlockType.RescueFriend, 0.1f);
		}
		Crush();
		slot.gravity = true;
		slot.SetBlock(null);
		SlotGravity.Reshading();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public override void Initialize()
	{
		slot.gravity = false;
		slot.RockCandyRemove();
		aniType = AnimationController.IdleAnimationType.GingermanUnderTile;
	}

	public override void BlockCrush(int fromCrushId = -1, int subId = -1)
	{
	}

	public override bool CanBeCrushedByNearSlot()
	{
		return false;
	}

	public override void PlayIdleAnimation(AnimationController.IdleAnimationType type, AnimationProperty prop)
	{
		if (prop.numOfAniObj == -1 && animator != null)
		{
			animator.SetTrigger(AnimationController.IdleAnimationName);
		}
		else
		{
			StartCoroutine(ProcessIdleAnimation(prop));
		}
	}

	private IEnumerator ProcessIdleAnimation(AnimationProperty prop)
	{
		if (animator != null)
		{
			for (int i = 0; i < prop.numOfAni; i++)
			{
				prop.aniCount++;
				yield return new WaitForSeconds(AnimationController.AnimationTerm * (float)prop.aniCount);
				animator.SetTrigger(AnimationController.IdleAnimationName);
			}
		}
	}
}
