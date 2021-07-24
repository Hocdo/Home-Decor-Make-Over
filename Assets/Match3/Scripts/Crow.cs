using DG.Tweening;
using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crow : BlockInterface
{
	private static readonly string idleAnimationMethodName = "randomIdleAnimation";

	protected Side dir;

	protected int eventCountBorn;

	public GameObject ObjHide;

	public override bool EnableBoosterHammer => true;

	public override bool EnableBoosterCandyCrane => false;

	public override bool EnableBoosterCandyPack => true;

	protected virtual IEnumerator RemoveCrow()
	{
		BoardManager.main.listCrowBlock.Remove(this);
		float removeEffTime = 0.583f;
		SoundSFX.PlayCap(SFXIndex.CrowHit);
		GameMain.main.PrevThrowCollectItem(CollectBlockType.Crow);
		ObjHide.SetActive(value: false);
		GameObject beAttackedEffect = SpawnStringGamePlaying.GetSpawnObject(SpawnStringGamePlayingType.CrowBeAttacked);
		beAttackedEffect.transform.position = base.transform.position;
		PoolManager.PoolGamePlaying.Despawn(beAttackedEffect.transform, removeEffTime);
		yield return new WaitForSeconds(removeEffTime);
		GameObject removeEffect = SpawnStringGamePlaying.GetSpawnObject(SpawnStringGamePlayingType.CrowRemove);
		removeEffect.transform.position = base.transform.position;
		removeEffect.transform.localScale = Vector3.one;
		SpriteRenderer[] componentsInChildren = removeEffect.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer spriteRenderer in componentsInChildren)
		{
			spriteRenderer.gameObject.layer = LayerMask.NameToLayer("GameEffect");
			if (GameMain.main.countOfEachTargetCount[9] == 0)
			{
				spriteRenderer.DOFade(0f, removeEffTime);
			}
		}
		if (GameMain.main.countOfEachTargetCount[9] == 0)
		{
			Transform transform = removeEffect.transform;
			Vector3 localPosition = removeEffect.transform.localPosition;
			transform.DOLocalMoveX(localPosition.x - 68f, removeEffTime, snapping: true);
			Transform transform2 = removeEffect.transform;
			Vector3 localPosition2 = removeEffect.transform.localPosition;
			transform2.DOLocalMoveY(localPosition2.y + 68f, removeEffTime, snapping: true);
			GameMain.main.StartCoroutine(Utils.LookAtTarget(removeEffect, removeEffTime));
			SoundSFX.PlayCap(SFXIndex.CrowFlyShort);
		}
		else
		{
			SoundSFX.PlayCap(SFXIndex.CrowFly);
		}
		GameMain.main.ThrowCollectItem(removeEffect, CollectBlockType.Crow, 0.1f, PoolManager.PoolGamePlaying);
		Crush();
		slot.gravity = true;
		slot.SetBlock(null);
		SlotGravity.Reshading();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void MoveCrow()
	{
		StartCoroutine(DoMoveCrow());
	}

	protected virtual IEnumerator DoMoveCrow()
	{
		List<Slot> swappableSlot = new List<Slot>();
		List<Side> swappableSide = new List<Side>();
		Side[] straightSides = Utils.straightSides;
		Slot nearSlot;
		foreach (Side side in straightSides)
		{
			nearSlot = BoardManager.main.GetNearSlot(slot.x, slot.y, side);
			if ((bool)nearSlot)
			{
				BlockInterface block = nearSlot.GetBlock();
				if (!block && (bool)nearSlot.GetChip() && !nearSlot.GetChip().isLockOnCrowMove)
				{
					swappableSlot.Add(nearSlot);
					swappableSide.Add(side);
				}
			}
		}
		if (swappableSlot.Count <= 0)
		{
			yield break;
		}
		int RandDir = Random.Range(0, swappableSlot.Count);
		nearSlot = swappableSlot[RandDir];
		Side swapSide = swappableSide[RandDir];
		if (swapSide == Side.Left || swapSide == Side.Right)
		{
			dir = swapSide;
		}
		if (dir == Side.Right)
		{
			ObjHide.transform.localScale = new Vector3(-0.9f, 0.9f, 0.9f);
		}
		else if (dir == Side.Left)
		{
			ObjHide.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
		}
		if ((bool)nearSlot)
		{
			Chip c = nearSlot.GetChip();
			if ((bool)c)
			{
				c.isLockOnCrowMove = true;
			}
			ObjHide.SetActive(value: false);
			SoundSFX.PlayCap(SFXIndex.CrowMove);
			GameObject moveEffect = SpawnStringGamePlaying.GetSpawnObject(SpawnStringGamePlayingType.CrowMove);
			moveEffect.transform.position = base.transform.position;
			if (dir == Side.Right)
			{
				moveEffect.transform.localScale = new Vector3(-1f, 1f, 1f);
			}
			else if (dir == Side.Left)
			{
				moveEffect.transform.localScale = new Vector3(1f, 1f, 1f);
			}
			float delay = 0.0833f;
			moveEffect.transform.DOMove(nearSlot.transform.position, GameMain.CrowMovingTime - delay).SetEase(Ease.Linear).SetDelay(delay);
			PoolManager.PoolGamePlaying.Despawn(moveEffect.transform, GameMain.CrowMovingTime);
			if ((bool)c)
			{
				c.transform.DOMove(slot.transform.position, GameMain.CrowMovingTime).SetEase(Ease.Linear);
			}
			yield return new WaitForSeconds(GameMain.CrowMovingTime);
			BlockInterface b = nearSlot.GetBlock();
			bool g = nearSlot.gravity;
			Vector3 posA = nearSlot.transform.position;
			Vector3 posB = slot.transform.position;
			slot.SetBlock(b);
			slot.SetChip(c);
			slot.gravity = g;
			if ((bool)b)
			{
				b.transform.position = posB;
			}
			if ((bool)c)
			{
				c.transform.position = posB;
			}
			nearSlot.SetBlock(this);
			slot = nearSlot;
			nearSlot.gravity = false;
			base.transform.position = posA;
			ObjHide.SetActive(value: true);
			if ((bool)c)
			{
				c.isLockOnCrowMove = false;
			}
		}
	}

	public override void Initialize()
	{
		slot.gravity = false;
		eventCountBorn = GameMain.main.eventCount;
		aniType = AnimationController.IdleAnimationType.Crow;
	}

	public override void BlockCrush(int fromCrushId = -1, int subId = -1)
	{
		if (!destroying && eventCountBorn != GameMain.main.eventCount)
		{
			eventCountBorn = GameMain.main.eventCount;
			destroying = true;
			StartCoroutine(RemoveCrow());
		}
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
