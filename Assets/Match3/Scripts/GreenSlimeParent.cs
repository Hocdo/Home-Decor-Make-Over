using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3;
public class GreenSlimeParent : BlockInterface
{
	public bool CanBreeding = true;

	private bool isCrushed;

	private bool isEffecting;

	private GameObject objCrushedEffect;

	private GameObject objDeathLoopEffect;

	private readonly List<GreenSlimeChild> listSimeChild = new List<GreenSlimeChild>();

	private readonly string IdleAnimationName = "Obstacle_Slime_body_idle";

	private List<Slot> availableSlot = new List<Slot>();

	private List<Slot> searchedSlot = new List<Slot>();

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

	public void Breeding()
	{
		StartCoroutine(DoBreeding());
	}

	private IEnumerator DoBreeding()
	{
		if (isCrushed)
		{
			yield break;
		}
		if (!CanBreeding)
		{
			CanBreeding = true;
			yield break;
		}
		if (objCrushedEffect != null)
		{
			PoolManager.PoolGameEffect.Despawn(objCrushedEffect.transform);
			objCrushedEffect = null;
			GameObject objStunEndEffect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.GreenSlimeParentStunEnd);
			objStunEndEffect.transform.position = base.transform.position;
			PoolManager.PoolGameEffect.Despawn(objStunEndEffect.transform, 0.333f);
			yield return new WaitForSeconds(0.333f);
			Utils.EnableAllSpriteRenderer(base.gameObject);
		}
		GameMain.main.isPlaying = false;
		GameMain.main.isLockDrop = true;
		availableSlot.Clear();
		searchedSlot.Clear();
		FindAvailableSlot(slot);
		Utils.Shuffle(availableSlot);
		if (availableSlot.Count > 0)
		{
			StartCoroutine(ShowParentBreedingEffect());
			yield return new WaitForSeconds(0.5f);
			Slot targetSlot = availableSlot[0];
			BlockInterface BreedingStartBlock = null;
			SoundSFX.PlayCapDelay(this, 0.3f, SFXIndex.GreenSlimeMake);
			int itr = 0;
			int sideIndex = 0;
			while (true)
			{
				int num;
				itr = (num = itr) + 1;
				if (num >= 150)
				{
					break;
				}
				sideIndex = Random.Range(0, 4);
				Side randSide = Utils.straightSides[sideIndex];
				Slot nearSlot = BoardManager.main.GetNearSlot(targetSlot.x, targetSlot.y, Utils.MirrorSide(randSide));
				if (nearSlot != null && nearSlot.GetBlock() != null && (nearSlot.GetBlock().blockType == IBlockType.GreenSlime || nearSlot.GetBlock().blockType == IBlockType.GreenSlimeChild))
				{
					BreedingStartBlock = nearSlot.GetBlock();
					break;
				}
			}
			if (BreedingStartBlock != null)
			{
				if (BreedingStartBlock.blockType != IBlockType.GreenSlime)
				{
					Utils.DisableAllSpriteRenderer(BreedingStartBlock.gameObject);
					GameObject spawnEffectObject = SpawnStringEffect.GetSpawnEffectObject((SpawnStringEffectType)(154 + sideIndex));
					spawnEffectObject.transform.position = BreedingStartBlock.transform.position;
					PoolManager.PoolGameEffect.Despawn(spawnEffectObject.transform, 0.667f);
				}
				GameObject objSlimeEndEffect = SpawnStringEffect.GetSpawnEffectObject((SpawnStringEffectType)(158 + sideIndex));
				objSlimeEndEffect.transform.position = BreedingStartBlock.transform.position;
				PoolManager.PoolGameEffect.Despawn(objSlimeEndEffect.transform, 0.833f);
				yield return new WaitForSeconds(0.667f);
				if (BreedingStartBlock.blockType != IBlockType.GreenSlime && (bool)BreedingStartBlock)
				{
					Utils.EnableAllSpriteRenderer(BreedingStartBlock.gameObject);
				}
			}
			if (targetSlot != null && targetSlot.GetBlock() == null)
			{
				GameObject item = ContentAssistant.main.GetItem("Obstacle_Slime_idle");
				item.transform.position = targetSlot.transform.position;
				item.name = "GreenSlimeChild_" + targetSlot.x + "x" + targetSlot.y;
				item.transform.parent = targetSlot.transform;
				GreenSlimeChild component = item.GetComponent<GreenSlimeChild>();
				targetSlot.SetBlock(component);
				component.slot = targetSlot;
				component.Initialize(this);
				listSimeChild.Add(component);
			}
		}
		GameMain.main.isPlaying = true;
		GameMain.main.isLockDrop = false;
		SlotGravity.Reshading();
	}

	private void FindAvailableSlot(Slot baseSlot)
	{
		for (int i = 0; i < 4; i++)
		{
			Side side = Utils.straightSides[i];
			Slot nearSlot = BoardManager.main.GetNearSlot(baseSlot.x, baseSlot.y, side);
			if (nearSlot != null && nearSlot.GetBlock() == null && nearSlot.GetChip() != null)
			{
				availableSlot.Add(nearSlot);
			}
			else if (nearSlot != null && nearSlot.GetBlock() != null && nearSlot.GetBlock().blockType == IBlockType.GreenSlimeChild && !searchedSlot.Contains(nearSlot))
			{
				searchedSlot.Add(nearSlot);
				FindAvailableSlot(nearSlot);
			}
		}
	}

	private IEnumerator ShowParentBreedingEffect()
	{
		Utils.DisableAllSpriteRenderer(base.gameObject);
		SoundSFX.PlayCap(SFXIndex.GreenSlimeParentBreed);
		GameObject objBreedingEffect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.GreenSlimeParentBreed);
		objBreedingEffect.transform.position = base.transform.position;
		PoolManager.PoolGameEffect.Despawn(objBreedingEffect.transform, 1.217f);
		yield return new WaitForSeconds(1.217f);
		Utils.EnableAllSpriteRenderer(base.gameObject);
		animator.Play(IdleAnimationName, -1, 0f);
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
		if (objCrushedEffect != null)
		{
			PoolManager.PoolGameEffect.Despawn(objCrushedEffect.transform);
			objCrushedEffect = null;
			GameObject objStunEndEffect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.GreenSlimeParentStunEnd);
			objStunEndEffect.transform.position = base.transform.position;
			PoolManager.PoolGameEffect.Despawn(objStunEndEffect.transform, 0.333f);
			yield return new WaitForSeconds(0.333f);
			Utils.EnableAllSpriteRenderer(base.gameObject);
		}
		Utils.DisableAllSpriteRenderer(base.gameObject);
		SoundSFX.PlayCap(SFXIndex.GreenSlimeParentHit);
		GameObject objDeathStartEffect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.GreenSlimeParentDeathStart);
		objDeathStartEffect.transform.SetParent(base.transform);
		objDeathStartEffect.transform.localScale = Vector3.one;
		objDeathStartEffect.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		objDeathStartEffect.transform.position = base.transform.position;
		PoolManager.PoolGameEffect.Despawn(objDeathStartEffect.transform, 0.417f);
		yield return new WaitForSeconds(0.417f);
		objDeathLoopEffect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.GreenSlimeParentDeathLoop);
		objDeathLoopEffect.transform.SetParent(base.transform);
		objDeathLoopEffect.transform.localScale = Vector3.one;
		objDeathLoopEffect.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		objDeathLoopEffect.transform.position = base.transform.position;
	}

	public IEnumerator StunnedByCrush()
	{
		if (!(objCrushedEffect != null))
		{
			CanBreeding = false;
			isEffecting = true;
			Utils.DisableAllSpriteRenderer(base.gameObject);
			SoundSFX.PlayCap(SFXIndex.GreenSlimeParentHit);
			GameObject objStunStartEffect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.GreenSlimeParentStunStart);
			objStunStartEffect.transform.position = base.transform.position;
			PoolManager.PoolGameEffect.Despawn(objStunStartEffect.transform, 0.167f);
			yield return new WaitForSeconds(0.167f);
			SoundSFX.PlayCap(SFXIndex.GreenSlimeParentStun);
			objCrushedEffect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.GreenSlimeParentStunLoop);
			objCrushedEffect.transform.SetParent(base.transform);
			objCrushedEffect.transform.localScale = Vector3.one;
			objCrushedEffect.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			objCrushedEffect.transform.position = base.transform.position;
			isEffecting = false;
		}
	}

	public override void Initialize()
	{
		slot.gravity = false;
		listSimeChild.Clear();
	}

	public override void BlockCrush(int fromCrushId = -1, int subId = -1)
	{
		if (!BoosterMagicHammer.isMagicHammerUsing && !isCrushed && !isEffecting)
		{
			StartCoroutine(StunnedByCrush());
		}
	}

	public override bool CanBeCrushedByNearSlot()
	{
		return false;
	}
}
