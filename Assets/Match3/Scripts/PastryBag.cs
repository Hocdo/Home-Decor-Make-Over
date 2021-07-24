using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3;

public class PastryBag : BlockInterface
{
	private int currentTurnCount;

	private readonly Vector3 effectPosOffset = new Vector3(0f, 0f, -1f);

	private bool isCrushed;

	private bool isStunned;

	public int makeChocolateCount = 1;

	public int makeTurnCount = 1;

	private GameObject mObjCrushedEffect;

	private GameObject objCrushedEffect;

	private readonly Vector3 PosOffset = new Vector3(1.7f, 10f, 0f);

	private Coroutine shotCoroutine;

	public override bool EnableBoosterHammer
	{
		get
		{
			if (isCrushed)
			{
				return false;
			}
			return true;
		}
	}

	public override bool EnableBoosterCandyCrane => false;

	public override bool EnableBoosterCandyPack => false;

	public IEnumerator ShotChocolate()
	{
		if (isCrushed)
		{
			yield break;
		}
		if (isStunned)
		{
			isStunned = false;
			yield break;
		}
		if (mObjCrushedEffect != null)
		{
			PoolManager.PoolGameEffect.Despawn(mObjCrushedEffect.transform);
			mObjCrushedEffect = null;
			GameObject objStunEndEffect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.PastryBagStunEnd);
			objStunEndEffect.transform.position = base.transform.position + effectPosOffset;
			PoolManager.PoolGameEffect.Despawn(objStunEndEffect.transform, 0.15f);
			yield return new WaitForSeconds(0.15f);
			Utils.EnableAllSpriteRenderer(base.gameObject);
		}
		if (currentTurnCount++ >= makeTurnCount)
		{
			currentTurnCount = 0;
			shotCoroutine = StartCoroutine(DoShotChocolate());
			yield return shotCoroutine;
			shotCoroutine = null;
		}
	}

	public IEnumerator StunnedByCrush()
	{
		isStunned = true;
		if (shotCoroutine == null && !(mObjCrushedEffect != null))
		{
			Utils.DisableAllSpriteRenderer(base.gameObject);
			GameObject objStunStartEffect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.PastryBagStunStart);
			objStunStartEffect.transform.position = base.transform.position + effectPosOffset;
			PoolManager.PoolGameEffect.Despawn(objStunStartEffect.transform, 0.583f);
			SoundSFX.PlayCap(SFXIndex.PastryBagStun);
			yield return new WaitForSeconds(0.583f);
			mObjCrushedEffect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.PastryBagStunLoop);
			mObjCrushedEffect.transform.position = base.transform.position + effectPosOffset;
		}
	}

	private IEnumerator DoShotChocolate()
	{
		SimpleChip[] allChips = BoardManager.main.slotGroup.GetComponentsInChildren<SimpleChip>();
		List<SimpleChip> targetChips = new List<SimpleChip>();
		BoardPosition pos = default(BoardPosition);
		SimpleChip[] array = allChips;
		foreach (SimpleChip simpleChip in array)
		{
			if ((bool)simpleChip && !(simpleChip.parentSlot == null) && !(simpleChip.parentSlot.slot.GetBlock() != null) && BoardManager.main.boardData.blocks[simpleChip.parentSlot.slot.x, simpleChip.parentSlot.slot.y] != IBlockType.ChocolateJail && BoardManager.main.boardData.blocks[simpleChip.parentSlot.slot.x, simpleChip.parentSlot.slot.y] != IBlockType.GreenSlime && BoardManager.main.boardData.blocks[simpleChip.parentSlot.slot.x, simpleChip.parentSlot.slot.y] != IBlockType.GreenSlimeChild && !BoardManager.main.boardData.rail[simpleChip.parentSlot.slot.x, simpleChip.parentSlot.slot.y] && BoardManager.main.boardData.tunnel[simpleChip.parentSlot.slot.x, simpleChip.parentSlot.slot.y] == 0)
			{
				pos.x = simpleChip.parentSlot.slot.x;
				pos.y = simpleChip.parentSlot.slot.y;
				if (!BoardManager.main.boardData.dicGeneratorDropBlock.ContainsKey(pos) && !BoardManager.main.boardData.dicGeneratorSpecialDropBlock.ContainsKey(pos))
				{
					targetChips.Add(simpleChip);
				}
			}
		}
		if (targetChips.Count > 0)
		{
			Utils.Shuffle(targetChips);
			Slot targetSlot = targetChips[0].parentSlot.slot;
			GameObject jail = ContentAssistant.main.GetItem("ChocolateJail");
			jail.transform.position = targetSlot.transform.position;
			jail.name = "ChocolateJail_" + targetSlot.x + "x" + targetSlot.y;
			jail.transform.parent = targetSlot.transform;
			ChocolateJail cj = jail.GetComponent<ChocolateJail>();
			targetSlot.SetBlock(cj);
			cj.slot = targetSlot;
			cj.Initialize();
			Utils.DisableAllSpriteRenderer(jail);
			GameMain.main.isPlaying = false;
			GameMain.main.isLockDrop = true;
			Utils.DisableAllSpriteRenderer(base.gameObject);
			GameObject objShotEffect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.PastryBagAction);
			objShotEffect.transform.position = base.transform.position + effectPosOffset;
			PoolManager.PoolGameEffect.Despawn(objShotEffect.transform, 1.667f);
			SoundSFX.PlayCapDelay(this, 0.416f, SFXIndex.PastryBagAction);
			yield return new WaitForSeconds(0.567f);
			StartCoroutine(ThrowChocolateDrop(targetSlot.transform.position));
			yield return new WaitForSeconds(1.08f);
			Utils.EnableAllSpriteRenderer(base.gameObject);
			yield return new WaitForSeconds(0.02f);
			GameObject objMakeEffect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.ChocolateJailMake);
			objMakeEffect.transform.position = targetChips[0].transform.position + new Vector3(0f, -34f, -1f);
			PoolManager.PoolGameEffect.Despawn(objMakeEffect.transform, 1.083f);
			SoundSFX.PlayCapDelay(this, 0.333f, SFXIndex.PastryBagChocolateJailCreate);
			yield return new WaitForSeconds(1.083f);
			Utils.EnableAllSpriteRenderer(jail);
			GameMain.main.isPlaying = true;
			GameMain.main.isLockDrop = false;
			SlotGravity.Reshading();
			if (isCrushed)
			{
				shotCoroutine = null;
				StartCoroutine(CrushByMagicHammer());
			}
		}
	}

	public void CrushByBooster(BoosterType boosterType)
	{
		if (boosterType == BoosterType.Hammer)
		{
			StartCoroutine(CrushByMagicHammer());
		}
	}

	public IEnumerator CrushByMagicHammer()
	{
		isCrushed = true;
		if (shotCoroutine == null)
		{
			if (mObjCrushedEffect != null)
			{
				PoolManager.PoolGameEffect.Despawn(mObjCrushedEffect.transform);
				mObjCrushedEffect = null;
				GameObject objStunEndEffect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.PastryBagStunEnd);
				objStunEndEffect.transform.position = base.transform.position + effectPosOffset;
				PoolManager.PoolGameEffect.Despawn(objStunEndEffect.transform, 0.15f);
				yield return new WaitForSeconds(0.15f);
				Utils.EnableAllSpriteRenderer(base.gameObject);
			}
			Utils.DisableAllSpriteRenderer(base.gameObject);
			GameObject objDamagedEffect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.PastryBagDamaged);
			objDamagedEffect.transform.SetParent(base.transform);
			objDamagedEffect.transform.localScale = Vector3.one;
			objDamagedEffect.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			objDamagedEffect.transform.position = base.transform.position + effectPosOffset;
			PoolManager.PoolGameEffect.Despawn(objDamagedEffect.transform, 0.833f);
			yield return new WaitForSeconds(0.833f);
			objCrushedEffect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.PastryBagCrushed);
			objCrushedEffect.transform.SetParent(base.transform);
			objCrushedEffect.transform.localScale = Vector3.one;
			objCrushedEffect.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			objCrushedEffect.transform.position = base.transform.position + effectPosOffset;
		}
	}

	private IEnumerator ThrowChocolateDrop(Vector3 targetPos)
	{
		GameObject objDropEffect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.ThrowingChocolateDrop);
		Vector3 vector = base.transform.position + new Vector3(0f, 30f, -1f);
		objDropEffect.transform.position = vector;
		Vector3 startPos = vector;
		float elapse_time = 0f;
		while (elapse_time < 1f)
		{
			vector = base.transform.position;
			float x = (vector.x + targetPos.x) * 0.5f;
			float y = targetPos.y;
			Vector3 position = base.transform.position;
			float y2;
			if (y <= position.y)
			{
				Vector3 position2 = base.transform.position;
				y2 = position2.y + 300f;
			}
			else
			{
				y2 = targetPos.y + 300f;
			}
			Vector3 pointPos = new Vector3(x, y2, 0f);
			objDropEffect.transform.position = Utils.Bezier(elapse_time, startPos, pointPos, targetPos);
			elapse_time += Time.deltaTime;
			yield return null;
		}
		PoolManager.PoolGameEffect.Despawn(objDropEffect.transform, 0.5f);
	}

	public override void Initialize()
	{
		base.transform.position = base.transform.position + PosOffset;
		slot.gravity = false;
		aniType = AnimationController.IdleAnimationType.PastryBag;
	}

	public override void BlockCrush(int fromCrushId = -1, int subId = -1)
	{
		Crush();
		if (!BoosterMagicHammer.isMagicHammerUsing && !isStunned && !isCrushed)
		{
			StartCoroutine(StunnedByCrush());
		}
	}

	public override bool CanBeCrushedByNearSlot()
	{
		return false;
	}

	public override void PlayIdleAnimation(AnimationController.IdleAnimationType type, AnimationProperty prop)
	{
		if (animator != null)
		{
			animator.SetTrigger(AnimationController.IdleAnimationName);
		}
	}
}
