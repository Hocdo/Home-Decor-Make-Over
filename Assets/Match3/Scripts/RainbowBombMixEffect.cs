using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowBombMixEffect : BombMixEffect
{
	public Powerup targetBomb;

	public override void OnSpawned()
	{
		base.OnSpawned();
	}

	public override void StartMixEffect()
	{
		StartCoroutine(MixEffect());
	}

	private IEnumerator MixEffect()
	{
		while (parentSlot == null)
		{
			yield return 0;
		}
		base.transform.position = parentSlot.transform.position;
		GameObject useEffect = SpawnStringBlock.GetSpawnBlockObject(SpawnStringBlockType.RainbowBombUse);
		useEffect.transform.position = base.transform.position;
		useEffect.transform.localScale = Chip.baseScale;
		GameMain.main.isLockDrop = true;
		base.IsMatching = true;
		GameMain.main.EventCounter();
		AudioSource loopSound = SoundSFX.Play(SFXIndex.RainbowBombLoop, loop: true);
		SoundSFX.Play(SFXIndex.RainbowBombStart);
		int sx = parentSlot.slot.x;
		int sy = parentSlot.slot.y;
		bool isPaintJelly = parentSlot.slot.IsPaintedJelly;
		SimpleChip[] allSimpleChips = BoardManager.main.slotGroup.GetComponentsInChildren<SimpleChip>();
		Chip[] allChips = BoardManager.main.slotGroup.GetComponentsInChildren<Chip>();
		List<SlotForChip> target = new List<SlotForChip>();
		if (targetId == -1)
		{
			List<SimpleChip>[] array = new List<SimpleChip>[6];
			SimpleChip[] array2 = allSimpleChips;
			foreach (SimpleChip simpleChip in array2)
			{
				if (!simpleChip.destroying && (bool)simpleChip.parentSlot && (!simpleChip.parentSlot.slot || simpleChip.parentSlot.slot.canBeCrush))
				{
					if (array[simpleChip.id] == null)
					{
						array[simpleChip.id] = new List<SimpleChip>();
					}
					array[simpleChip.id].Add(simpleChip);
				}
			}
			int num = -1;
			int num2 = 0;
			for (int k = 0; k < 6; k++)
			{
				if (array[k] != null && array[k].Count > num2)
				{
					num2 = array[k].Count;
					num = k;
				}
			}
			if (num != -1)
			{
				for (int k = 0; k < array[num].Count; k++)
				{
					target.Add(array[num][k].parentSlot);
				}
			}
		}
		else
		{
			SimpleChip[] array3 = allSimpleChips;
			foreach (SimpleChip simpleChip2 in array3)
			{
				if (!simpleChip2.destroying && (bool)simpleChip2.parentSlot && (!simpleChip2.parentSlot.slot || simpleChip2.parentSlot.slot.canBeCrush) && simpleChip2.id == targetId)
				{
					target.Add(simpleChip2.parentSlot);
				}
			}
		}
		Utils.Shuffle(target);
		List<SlotForChip> listDestroyTarget = new List<SlotForChip>();
		Chip[] array4 = allChips;
		foreach (Chip chip in array4)
		{
			if ((bool)chip.parentSlot && (bool)chip.parentSlot.slot && !chip.parentSlot.slot.canBeCrush)
			{
				continue;
			}
			switch (targetBomb)
			{
			case Powerup.ColorHBomb:
			case Powerup.ColorVBomb:
				if (chip.chipType == ChipType.HBomb || chip.chipType == ChipType.VBomb)
				{
					listDestroyTarget.Add(chip.parentSlot);
				}
				break;
			case Powerup.SimpleBomb:
				if (chip.chipType == ChipType.SimpleBomb)
				{
					listDestroyTarget.Add(chip.parentSlot);
				}
				break;
			case Powerup.CandyChip:
				if (chip.chipType == ChipType.CandyChip)
				{
					listDestroyTarget.Add(chip.parentSlot);
				}
				break;
			}
		}
		for (int k = 0; k < target.Count; k++)
		{
			int x = target[k].slot.x;
			int y = target[k].slot.y;
			if (targetBomb == Powerup.ColorHBomb || targetBomb == Powerup.ColorVBomb)
			{
				targetBomb = ((Random.Range(0, 2) != 0) ? Powerup.ColorVBomb : Powerup.ColorHBomb);
			}
			target[k].SetChip(BoardManager.main.AddPowerup(x, y, targetBomb));
			listDestroyTarget.Add(target[k]);
			SoundSFX.PlayCap(SFXIndex.Fusion);
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(0.3f);
		GameMain.main.EventCounter();
		foreach (SlotForChip sc in listDestroyTarget)
		{
			if ((bool)sc && (bool)sc.chip && !sc.chip.destroying)
			{
				if ((bool)sc.slot && isPaintJelly)
				{
					sc.slot.PaintJelly();
				}
				if (sc.slot.GetBlock() != null)
				{
					sc.slot.GetBlock().BlockCrush();
				}
				sc.chip.DestroyChip();
				yield return new WaitForSeconds(0.1f);
			}
		}
		yield return new WaitForSeconds(0.1f);
		base.IsMatching = false;
		GameMain.main.isLockDrop = false;
		BoardManager.main.SlotCrush(sx, sy, radius: false, ScoreType.Match3, includeCrushChip: false);
		PoolManager.PoolGameBlocks.Despawn(useEffect.transform);
		if ((bool)loopSound)
		{
			loopSound.Stop();
		}
		SoundSFX.Play(SFXIndex.RainbowBombEnd);
		destroingLock = false;
		StartCoroutine(DestroyChipFunction());
	}
}
