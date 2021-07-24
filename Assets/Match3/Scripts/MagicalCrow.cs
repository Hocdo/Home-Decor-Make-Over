using DG.Tweening;
using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicalCrow : Crow
{
	protected override IEnumerator RemoveCrow()
	{
		BoardManager.main.listCrowBlock.Remove(this);
		float removeEffTime = 0.583f;
		GameMain.main.PrevThrowCollectItem(CollectBlockType.MagicalCrow);
		ObjHide.SetActive(value: false);
		GameObject beAttackedEffect = SpawnStringGamePlaying.GetSpawnObject(SpawnStringGamePlayingType.MagicalCrowBeAttacked);
		beAttackedEffect.transform.position = base.transform.position;
		PoolManager.PoolGamePlaying.Despawn(beAttackedEffect.transform, removeEffTime);
		yield return new WaitForSeconds(removeEffTime);
		GameObject removeEffect = SpawnStringGamePlaying.GetSpawnObject(SpawnStringGamePlayingType.MagicalCrowRemove);
		removeEffect.transform.position = base.transform.position;
		removeEffect.transform.localScale = Vector3.one;
		SpriteRenderer[] componentsInChildren = removeEffect.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer spriteRenderer in componentsInChildren)
		{
			spriteRenderer.gameObject.layer = LayerMask.NameToLayer("GameEffect");
			if (GameMain.main.countOfEachTargetCount[16] == 0)
			{
				spriteRenderer.DOFade(0f, removeEffTime);
			}
		}
		if (GameMain.main.countOfEachTargetCount[16] == 0)
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
		GameMain.main.ThrowCollectItem(removeEffect, CollectBlockType.MagicalCrow, 0.1f, PoolManager.PoolGamePlaying);
		Crush();
		slot.gravity = true;
		slot.SetBlock(null);
		SlotGravity.Reshading();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	protected override IEnumerator DoMoveCrow()
	{
		List<Slot> attackSpecialSlot = new List<Slot>();
		List<Side> attackSideList = new List<Side>();
		Side[] straightSides = Utils.straightSides;
		Slot nearSlot;
		foreach (Side side in straightSides)
		{
			nearSlot = BoardManager.main.GetNearSlot(slot.x, slot.y, side);
			if ((bool)nearSlot)
			{
				BlockInterface block = nearSlot.GetBlock();
				if (!block && (bool)nearSlot.GetChip() && nearSlot.GetChip().targetAttakMagicCrow == null && nearSlot.GetChip().chipType != ChipType.BringDown && nearSlot.GetChip().chipType != ChipType.SimpleChip)
				{
					attackSpecialSlot.Add(nearSlot);
					attackSideList.Add(side);
				}
			}
		}
		if (attackSpecialSlot.Count > 0)
		{
			GameMain.main.magicalCrowHitCount++;
			int ran = Random.Range(0, attackSpecialSlot.Count);
			Slot attackSlot = attackSpecialSlot[ran];
			Side attackSide = attackSideList[ran];
			if (attackSide == Side.Left || attackSide == Side.Right)
			{
				dir = attackSide;
			}
			if ((bool)attackSlot.GetChip())
			{
				attackSlot.GetChip().targetAttakMagicCrow = this;
			}
			if (dir == Side.Right)
			{
				ObjHide.transform.localScale = new Vector3(-0.8f, 0.8f, 0.8f);
			}
			else if (dir == Side.Left)
			{
				ObjHide.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
			}
			ObjHide.SetActive(value: false);
			GameObject hitEffect = SpawnStringGamePlaying.GetSpawnObject(SpawnStringGamePlayingType.MagicalCrowAttack);
			if ((bool)hitEffect)
			{
				hitEffect.transform.position = base.transform.position;
				if (dir == Side.Right)
				{
					hitEffect.transform.localScale = new Vector3(-0.8f, 0.8f, 0.8f);
				}
				else if (dir == Side.Left)
				{
					hitEffect.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
				}
				PoolManager.PoolGamePlaying.Despawn(hitEffect.transform, 1.117f);
			}
			yield return new WaitForSeconds(0.26f);
			SoundSFX.PlayCap(SFXIndex.CrowBoomBlock);
			yield return new WaitForSeconds(0.34f);
			GameObject removeEffect = SpawnStringGamePlaying.GetSpawnObject(SpawnStringGamePlayingType.MagicalCrowAttackedBlock);
			if ((bool)removeEffect)
			{
				removeEffect.transform.position = attackSlot.transform.position;
				PoolManager.PoolGamePlaying.Despawn(removeEffect.transform, 0.667f);
			}
			yield return new WaitForSeconds(0.1f);
			if ((bool)attackSlot.GetChip())
			{
				attackSlot.GetChip().HideChip();
			}
			yield return new WaitForSeconds(0.4f);
			ObjHide.SetActive(value: true);
			yield break;
		}
		yield return null;
		List<Slot> swappableSlot = new List<Slot>();
		List<Side> swappableSide = new List<Side>();
		Side[] straightSides2 = Utils.straightSides;
		foreach (Side side2 in straightSides2)
		{
			nearSlot = BoardManager.main.GetNearSlot(slot.x, slot.y, side2);
			if ((bool)nearSlot)
			{
				BlockInterface block2 = nearSlot.GetBlock();
				if (!block2 && (bool)nearSlot.GetChip() && !nearSlot.GetChip().isLockOnCrowMove && nearSlot.GetChip().targetAttakMagicCrow == null)
				{
					swappableSlot.Add(nearSlot);
					swappableSide.Add(side2);
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
			ObjHide.transform.localScale = new Vector3(-0.8f, 0.8f, 0.8f);
		}
		else if (dir == Side.Left)
		{
			ObjHide.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
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
			GameObject moveEffect = SpawnStringGamePlaying.GetSpawnObject(SpawnStringGamePlayingType.MagicalCrowMove);
			moveEffect.transform.position = base.transform.position;
			if (dir == Side.Right)
			{
				moveEffect.transform.localScale = new Vector3(-1f, 1f, 1f);
			}
			else if (dir == Side.Left)
			{
				moveEffect.transform.localScale = new Vector3(1f, 1f, 1f);
			}
			moveEffect.transform.DOMove(nearSlot.transform.position, GameMain.CrowMovingTime).SetEase(Ease.Linear);
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

	private IEnumerator DoMoveNormal()
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
			ObjHide.transform.localScale = new Vector3(-0.8f, 0.8f, 0.8f);
		}
		else if (dir == Side.Left)
		{
			ObjHide.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
		}
		if (!nearSlot)
		{
			yield break;
		}
		Chip c = nearSlot.GetChip();
		if ((bool)c)
		{
			c.isLockOnCrowMove = true;
		}
		ObjHide.SetActive(value: false);
		SoundSFX.PlayCap(SFXIndex.CrowMove);
		GameObject moveEffect = SpawnStringGamePlaying.GetSpawnObject(SpawnStringGamePlayingType.MagicalCrowMove);
		moveEffect.transform.position = base.transform.position;
		if (dir == Side.Right)
		{
			moveEffect.transform.localScale = new Vector3(-1f, 1f, 1f);
		}
		else if (dir == Side.Left)
		{
			moveEffect.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		moveEffect.transform.DOMove(nearSlot.transform.position, GameMain.CrowMovingTime).SetEase(Ease.Linear);
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
		List<Slot> attackSpecialSlot = new List<Slot>();
		List<Side> attackSideList = new List<Side>();
		Side[] straightSides2 = Utils.straightSides;
		foreach (Side side2 in straightSides2)
		{
			nearSlot = BoardManager.main.GetNearSlot(slot.x, slot.y, side2);
			if ((bool)nearSlot)
			{
				BlockInterface block2 = nearSlot.GetBlock();
				if (!block2 && (bool)nearSlot.GetChip() && nearSlot.GetChip().targetAttakMagicCrow == null && nearSlot.GetChip().chipType != ChipType.BringDown && nearSlot.GetChip().chipType != ChipType.SimpleChip)
				{
					attackSpecialSlot.Add(nearSlot);
					attackSideList.Add(side2);
				}
			}
		}
		if (attackSpecialSlot.Count > 0)
		{
			GameMain.main.magicalCrowHitCount++;
			int ran = Random.Range(0, attackSpecialSlot.Count);
			Slot attackSlot = attackSpecialSlot[ran];
			Side attackSide = attackSideList[ran];
			if (attackSide == Side.Left || attackSide == Side.Right)
			{
				dir = attackSide;
			}
			if ((bool)attackSlot.GetChip())
			{
				attackSlot.GetChip().targetAttakMagicCrow = this;
			}
			if (dir == Side.Right)
			{
				ObjHide.transform.localScale = new Vector3(-0.8f, 0.8f, 0.8f);
			}
			else if (dir == Side.Left)
			{
				ObjHide.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
			}
			ObjHide.SetActive(value: false);
			GameObject hitEffect = SpawnStringGamePlaying.GetSpawnObject(SpawnStringGamePlayingType.MagicalCrowAttack);
			if ((bool)hitEffect)
			{
				hitEffect.transform.position = base.transform.position;
				if (dir == Side.Right)
				{
					hitEffect.transform.localScale = new Vector3(-0.8f, 0.8f, 0.8f);
				}
				else if (dir == Side.Left)
				{
					hitEffect.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
				}
				PoolManager.PoolGamePlaying.Despawn(hitEffect.transform, 1.117f);
			}
			yield return new WaitForSeconds(0.26f);
			SoundSFX.PlayCap(SFXIndex.CrowBoomBlock);
			yield return new WaitForSeconds(0.34f);
			GameObject removeEffect = SpawnStringGamePlaying.GetSpawnObject(SpawnStringGamePlayingType.MagicalCrowAttackedBlock);
			if ((bool)removeEffect)
			{
				removeEffect.transform.position = attackSlot.transform.position;
				PoolManager.PoolGamePlaying.Despawn(removeEffect.transform, 0.667f);
			}
			yield return new WaitForSeconds(0.1f);
			if ((bool)attackSlot.GetChip())
			{
				attackSlot.GetChip().HideChip();
			}
			yield return new WaitForSeconds(0.4f);
			ObjHide.SetActive(value: true);
		}
		ObjHide.SetActive(value: true);
		if ((bool)c)
		{
			c.isLockOnCrowMove = false;
		}
	}

	public override void Initialize()
	{
		base.Initialize();
		aniType = AnimationController.IdleAnimationType.MagicalCrow;
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
		if (type != AnimationController.IdleAnimationType.Crow)
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
