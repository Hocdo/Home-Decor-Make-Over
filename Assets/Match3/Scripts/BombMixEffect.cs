using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombMixEffect : Chip
{
	private static readonly Dictionary<BombPair, SpawnStringBlockType> data = new Dictionary<BombPair, SpawnStringBlockType>();

	public static bool isInit;

	private static Slot lockGravitySlot;

	private static Slot lockGravitySlotFrom;

	[HideInInspector]
	public bool destroingLock = true;

	[HideInInspector]
	public int targetId;

	[HideInInspector]
	public int subId;

	public static void Initialize()
	{
		if (!isInit)
		{
			data.Add(new BombPair(ChipType.SimpleBomb, ChipType.SimpleBomb), SpawnStringBlockType.SimpleSimpleMixEffect);
			data.Add(new BombPair(ChipType.VBomb, ChipType.VBomb), SpawnStringBlockType.HVBombMixEffect);
			data.Add(new BombPair(ChipType.HBomb, ChipType.HBomb), SpawnStringBlockType.HVBombMixEffect);
			data.Add(new BombPair(ChipType.HBomb, ChipType.VBomb), SpawnStringBlockType.HVBombMixEffect);
			data.Add(new BombPair(ChipType.VBomb, ChipType.SimpleBomb), SpawnStringBlockType.SimpleVBombMixEffect);
			data.Add(new BombPair(ChipType.HBomb, ChipType.SimpleBomb), SpawnStringBlockType.SimpleHBombMixEffect);
			data.Add(new BombPair(ChipType.RainbowBomb, ChipType.SimpleChip), SpawnStringBlockType.RainbowChipMixEffect);
			data.Add(new BombPair(ChipType.RainbowBomb, ChipType.RainbowBomb), SpawnStringBlockType.RainbowRainbowMixEffect);
			data.Add(new BombPair(ChipType.RainbowBomb, ChipType.SimpleBomb), SpawnStringBlockType.RainbowSimpleMixEffect);
			data.Add(new BombPair(ChipType.RainbowBomb, ChipType.HBomb), SpawnStringBlockType.RainbowHBombMixEffect);
			data.Add(new BombPair(ChipType.RainbowBomb, ChipType.VBomb), SpawnStringBlockType.RainbowVBombMixEffect);
			data.Add(new BombPair(ChipType.CandyChip, ChipType.CandyChip), SpawnStringBlockType.LargeCandyMixEffect);
			data.Add(new BombPair(ChipType.CandyChip, ChipType.HBomb), SpawnStringBlockType.SmallCandyHVBombMixEffect);
			data.Add(new BombPair(ChipType.CandyChip, ChipType.VBomb), SpawnStringBlockType.SmallCandyHVBombMixEffect);
			data.Add(new BombPair(ChipType.CandyChip, ChipType.SimpleBomb), SpawnStringBlockType.SmallCandySimpleBombMixEffect);
			data.Add(new BombPair(ChipType.CandyChip, ChipType.RainbowBomb), SpawnStringBlockType.SmallCandyRainbowBombMixEffect);
			isInit = true;
		}
	}

	public override void OnSpawned()
	{
		base.OnSpawned();
		destroingLock = true;
	}

	public static bool ContainsPair(ChipType pa, ChipType pb)
	{
		return ContainsPair(new BombPair(pa, pb));
	}

	public static bool ContainsPair(BombPair pair)
	{
		return data.ContainsKey(pair);
	}

	public static void Mix(ChipType pa, ChipType pb, SlotForChip slot, SlotForChip slotFrom, int targetId, int subId, Side swapDirection)
	{
		Mix(new BombPair(pa, pb), slot, slotFrom, targetId, subId, swapDirection);
	}

	private static void Mix(BombPair pair, SlotForChip slot, SlotForChip slotFrom, int targetId, int subId, Side swapDirection)
	{
		if (ContainsPair(pair))
		{
			if (MapData.main.target == GoalTarget.Jelly && (bool)slot && (bool)slotFrom && (bool)slot.slot && (bool)slotFrom.slot && (slot.slot.IsPaintedJelly | slotFrom.slot.IsPaintedJelly))
			{
				slot.slot.PaintJelly();
				slotFrom.slot.PaintJelly();
			}
			lockGravitySlot = slot.slot;
			lockGravitySlotFrom = slotFrom.slot;
			if ((bool)lockGravitySlot)
			{
				lockGravitySlot.PauseGravity(0f);
			}
			if ((bool)lockGravitySlotFrom)
			{
				lockGravitySlotFrom.PauseGravity(0f);
			}
			Transform transform = SpawnStringBlock.GetSpawnBlockObject(data[pair]).transform;
			BombMixEffect component = transform.GetComponent<BombMixEffect>();
			slot.SetChip(component);
			component.transform.localPosition = Vector3.zero;
			component.id = (component.targetId = targetId);
			component.subId = subId;
			component.StartMixEffect();
			if ((pair.a == ChipType.CandyChip && pair.b == ChipType.SimpleBomb) || (pair.a == ChipType.SimpleBomb && pair.b == ChipType.CandyChip))
			{
				SoundSFX.Play(SFXIndex.Fusion, loop: false, 0.2f);
			}
			else
			{
				SoundSFX.PlayCap(SFXIndex.Fusion);
			}
			if (component is LargeCandyMixEffect)
			{
				LargeCandyMixEffect largeCandyMixEffect = component as LargeCandyMixEffect;
				largeCandyMixEffect.transform.position = slot.transform.position;
				largeCandyMixEffect.Rolling(swapDirection, slot.slot, slotFrom.slot);
			}
			else if (component is SmallCandyMixEffect)
			{
				slot.slot.ResumeGravity();
				slotFrom.slot.ResumeGravity();
				SmallCandyMixEffect smallCandyMixEffect = component as SmallCandyMixEffect;
				smallCandyMixEffect.transform.position = slot.transform.position;
				smallCandyMixEffect.CreateMix(swapDirection);
			}
			else if ((bool)lockGravitySlotFrom)
			{
				lockGravitySlotFrom.PauseGravity(0.24f);
			}
		}
	}

	public virtual void StartMixEffect()
	{
	}

	public override IEnumerator DestroyChipFunction()
	{
		while (destroingLock)
		{
			yield return null;
		}
		if ((bool)lockGravitySlot)
		{
			lockGravitySlot.ResumeGravity();
		}
		if ((bool)lockGravitySlotFrom)
		{
			lockGravitySlotFrom.ResumeGravity();
		}
		if (!isCandyMix)
		{
			ParentRemove();
		}
		PoolManager.PoolGameBlocks.Despawn(base.gameObject.transform);
	}
}
