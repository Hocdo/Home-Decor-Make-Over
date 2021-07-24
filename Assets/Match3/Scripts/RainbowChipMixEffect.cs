using DG.Tweening;
using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowChipMixEffect : BombMixEffect
{
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
		if (parentSlot == null)
		{
			yield break;
		}
		int sx = parentSlot.slot.x;
		int sy = parentSlot.slot.y;
		bool isPaintJelly = parentSlot.slot.IsPaintedJelly;
		base.transform.position = parentSlot.transform.position;
		GameObject useEffect = SpawnStringBlock.GetSpawnBlockObject(SpawnStringBlockType.RainbowBombUse);
		useEffect.transform.position = base.transform.position;
		useEffect.transform.localScale = Chip.baseScale;
		GameMain.main.EventCounter();
		GameMain.main.isLockDrop = true;
		yield return new WaitForSeconds(0.1f);
		Chip[] allChips = BoardManager.main.slotGroup.GetComponentsInChildren<Chip>();
		List<SlotForChip> target = new List<SlotForChip>();
		if (base.targetId == -1)
		{
			List<Chip>[] array = new List<Chip>[6];
			int[] array2 = new int[6];
			Chip[] array3 = allChips;
			foreach (Chip chip in array3)
			{
				if (!chip.destroying && (bool)chip.parentSlot && (!chip.parentSlot.slot || chip.parentSlot.slot.canBeCrush) && chip.id >= 0)
				{
					array2[chip.id]++;
					if (array[chip.id] == null)
					{
						array[chip.id] = new List<Chip>();
					}
					array[chip.id].Add(chip);
				}
			}
			int num = 0;
			int num2 = 0;
			for (int j = 0; j < 6; j++)
			{
				if (array2[j] > num2)
				{
					num2 = array2[j];
					num = j;
				}
			}
			if (array[num] != null)
			{
				foreach (Chip item in array[num])
				{
					target.Add(item.parentSlot);
				}
			}
		}
		else
		{
			Chip[] array4 = allChips;
			foreach (Chip chip2 in array4)
			{
				if (!chip2.destroying && (bool)chip2.parentSlot && (!chip2.parentSlot.slot || chip2.parentSlot.slot.canBeCrush) && chip2.id == base.targetId)
				{
					target.Add(chip2.parentSlot);
				}
			}
		}
		if (target != null)
		{
			GameMain.main.EventCounter();
			Utils.Shuffle(target);
			AudioSource loopSound = SoundSFX.Play(SFXIndex.RainbowBombLoop, loop: true);
			SoundSFX.Play(SFXIndex.RainbowBombStart);
			Vector2 startPosition = new Vector2(sx, sy);
			Vector2 targetPosition = Vector2.zero;
			for (int j = 0; j < target.Count; j++)
			{
				if ((bool)target[j].chip)
				{
					targetPosition.x = target[j].chip.parentSlot.slot.x;
					targetPosition.y = target[j].chip.parentSlot.slot.y;
					float distance = Vector2.Distance(startPosition, targetPosition);
					GameObject spawnEffectObjectRainbowThrow = SpawnStringEffect.GetSpawnEffectObjectRainbowThrow(target[j].chip.id);
					spawnEffectObjectRainbowThrow.transform.position = base.transform.position;
					float throwTime = Mathf.Lerp(0.6f, 1.3f, distance / 11f);
					float zoomScale = Mathf.Lerp(0.6f, 1.8f, distance / 11f);
					SoundSFX.PlayCap(SFXIndex.RainbowBombThrowEffect);
					spawnEffectObjectRainbowThrow.transform.DOMove(target[j].chip.transform.position, throwTime).SetEase(Ease.InOutCubic);
					Transform[] componentsInChildren = spawnEffectObjectRainbowThrow.transform.GetComponentsInChildren<Transform>();
					foreach (Transform transform in componentsInChildren)
					{
						transform.transform.localScale = Chip.baseScale;
						transform.DOScale(zoomScale, throwTime / 2f).SetEase(Ease.InCubic);
						transform.DOScale(0.6f, throwTime / 2f).SetDelay(throwTime / 2f);
					}
					PoolManager.PoolGameEffect.Despawn(spawnEffectObjectRainbowThrow.transform, throwTime);
					BoardManager main = BoardManager.main;
					float delay = throwTime;
					int x = target[j].chip.parentSlot.slot.x;
					int y = target[j].chip.parentSlot.slot.y;
					bool radius = true;
					ScoreType scoreType = ScoreType.ChipCrushByItemBlock;
					int targetId = base.targetId;
					main.DelaySlotCrush(delay, x, y, radius, scoreType, includeCrushChip: true, 0, targetId, Side.Null, isPaintJelly);
					SoundSFX.Play(SFXIndex.RainbowBombDamagedBlock, loop: false, throwTime);
				}
				yield return new WaitForSeconds(0.1f);
			}
			yield return new WaitForSeconds(1f);
			if ((bool)loopSound)
			{
				loopSound.Stop();
			}
			SoundSFX.Play(SFXIndex.RainbowBombEnd);
		}
		GameMain.main.isLockDrop = false;
		if (!isCandyMix)
		{
			ParentRemove();
		}
		BoardManager.main.SlotCrush(sx, sy, radius: false, ScoreType.Match3, includeCrushChip: false);
		PoolManager.PoolGameBlocks.Despawn(useEffect.transform);
		destroingLock = false;
		StartCoroutine(DestroyChipFunction());
	}

	public void StartMixEffectWithoutRainbowEffect()
	{
		StartCoroutine(MixEffectWithoutRainbowEffect());
	}

	private IEnumerator MixEffectWithoutRainbowEffect()
	{
		if (parentSlot == null)
		{
			yield break;
		}
		int sx = parentSlot.slot.x;
		int sy = parentSlot.slot.y;
		bool isPaintJelly = parentSlot.slot.IsPaintedJelly;
		base.transform.position = parentSlot.transform.position;
		GameMain.main.EventCounter();
		GameMain.main.isLockDrop = true;
		yield return new WaitForSeconds(0.1f);
		Chip[] allChips = BoardManager.main.slotGroup.GetComponentsInChildren<Chip>();
		List<SlotForChip> target = new List<SlotForChip>();
		if (base.targetId == -1)
		{
			List<Chip>[] array = new List<Chip>[6];
			int[] array2 = new int[6];
			Chip[] array3 = allChips;
			foreach (Chip chip in array3)
			{
				if (!chip.destroying && (bool)chip.parentSlot && (!chip.parentSlot.slot || chip.parentSlot.slot.canBeCrush) && chip.id >= 0)
				{
					array2[chip.id]++;
					if (array[chip.id] == null)
					{
						array[chip.id] = new List<Chip>();
					}
					array[chip.id].Add(chip);
				}
			}
			int num = 0;
			int num2 = 0;
			for (int j = 0; j < 6; j++)
			{
				if (array2[j] > num2)
				{
					num2 = array2[j];
					num = j;
				}
			}
			if (array[num] != null)
			{
				foreach (Chip item in array[num])
				{
					target.Add(item.parentSlot);
				}
			}
		}
		else
		{
			Chip[] array4 = allChips;
			foreach (Chip chip2 in array4)
			{
				if (!chip2.destroying && (bool)chip2.parentSlot && (!chip2.parentSlot.slot || chip2.parentSlot.slot.canBeCrush) && chip2.id == base.targetId)
				{
					target.Add(chip2.parentSlot);
				}
			}
		}
		if (target != null)
		{
			GameMain.main.EventCounter();
			Utils.Shuffle(target);
			AudioSource loopSound = SoundSFX.Play(SFXIndex.RainbowBombLoop, loop: true);
			SoundSFX.Play(SFXIndex.RainbowBombStart);
			Vector2 startPosition = new Vector2(sx, sy);
			Vector2 targetPosition = Vector2.zero;
			for (int j = 0; j < target.Count; j++)
			{
				if ((bool)target[j].chip)
				{
					targetPosition.x = target[j].chip.parentSlot.slot.x;
					targetPosition.y = target[j].chip.parentSlot.slot.y;
					float distance = Vector2.Distance(startPosition, targetPosition);
					GameObject spawnEffectObjectRainbowThrow = SpawnStringEffect.GetSpawnEffectObjectRainbowThrow(target[j].chip.id);
					spawnEffectObjectRainbowThrow.transform.position = base.transform.position;
					float throwTime = Mathf.Lerp(0.6f, 1.3f, distance / 11f);
					float zoomScale = Mathf.Lerp(0.6f, 1.8f, distance / 11f);
					SoundSFX.PlayCap(SFXIndex.RainbowBombThrowEffect);
					spawnEffectObjectRainbowThrow.transform.DOMove(target[j].chip.transform.position, throwTime).SetEase(Ease.InOutCubic);
					Transform[] componentsInChildren = spawnEffectObjectRainbowThrow.transform.GetComponentsInChildren<Transform>();
					foreach (Transform transform in componentsInChildren)
					{
						transform.transform.localScale = Chip.baseScale;
						transform.DOScale(zoomScale, throwTime / 2f).SetEase(Ease.InCubic);
						transform.DOScale(0.6f, throwTime / 2f).SetDelay(throwTime / 2f);
					}
					PoolManager.PoolGameEffect.Despawn(spawnEffectObjectRainbowThrow.transform, throwTime);
					BoardManager main = BoardManager.main;
					float delay = throwTime;
					int x = target[j].chip.parentSlot.slot.x;
					int y = target[j].chip.parentSlot.slot.y;
					bool radius = true;
					ScoreType scoreType = ScoreType.ChipCrushByItemBlock;
					int targetId = base.targetId;
					main.DelaySlotCrush(delay, x, y, radius, scoreType, includeCrushChip: true, 0, targetId, Side.Null, isPaintJelly);
					SoundSFX.Play(SFXIndex.RainbowBombDamagedBlock, loop: false, throwTime);
				}
				yield return new WaitForSeconds(0.1f);
			}
			yield return new WaitForSeconds(1f);
			if ((bool)loopSound)
			{
				loopSound.Stop();
			}
			SoundSFX.Play(SFXIndex.RainbowBombEnd);
		}
		GameMain.main.isLockDrop = false;
		if (!isCandyMix)
		{
			ParentRemove();
		}
		BoardManager.main.SlotCrush(sx, sy, radius: false, ScoreType.Match3, includeCrushChip: false);
		destroingLock = false;
		StartCoroutine(DestroyChipFunction());
	}
}
