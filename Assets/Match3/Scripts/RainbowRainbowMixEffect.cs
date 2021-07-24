using DG.Tweening;
using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3;

public class RainbowRainbowMixEffect : BombMixEffect
{
	private int sx;

	private int sy;

	public override void OnSpawned()
	{
		base.OnSpawned();
	}

	public IEnumerator BoosterKingCandyEffect()
	{
		while (parentSlot == null)
		{
			yield return 0;
		}
		base.transform.position = parentSlot.transform.position;
		bool isPaintJelly = parentSlot.slot.IsPaintedJelly;
		base.IsMatching = true;
		GameMain.main.EventCounter();
		GameMain.main.isLockDrop = true;
		sx = parentSlot.slot.x;
		sy = parentSlot.slot.y;
		Camera.main.transform.DOShakePosition(1.3f, 20f);
		yield return new WaitForSeconds(0.12f);
		List<Slot> target = new List<Slot>();
		for (int r = 1; r < 18; r++)
		{
			int crushCount = 0;
			for (int i = sx - r; i <= sx + r; i++)
			{
				for (int j = sy - r; j <= sy + r; j++)
				{
					if ((sx - i) * (sx - i) + (sy - j) * (sy - j) <= r * r)
					{
						Slot s = BoardManager.main.GetSlot(i, j);
						if (s != null && !target.Contains(s))
						{
							target.Add(s);
							crushCount++;
							BoardManager main = BoardManager.main;
							int x = i;
							int y = j;
							bool radius = false;
							ScoreType scoreType = ScoreType.ChipCrushByItemBlock;
							bool includeCrushChip = true;
							int id = base.id;
							main.SlotCrush(x, y, radius, scoreType, includeCrushChip, 0, id, Side.Null, isPaintJelly);
							ProcessWorkingBlock(s);
						}
					}
				}
			}
			if (crushCount > 0)
			{
				yield return new WaitForSeconds(0.1f);
			}
		}
		base.IsMatching = false;
		GameMain.main.isLockDrop = false;
		destroingLock = false;
		StartCoroutine(DestroyChipFunction());
	}

	private void ProcessWorkingBlock(Slot targetSlot)
	{
		BlockInterface block = targetSlot.GetBlock();
		if ((bool)block && block.blockType >= IBlockType.CandyFactory_1 && block.blockType <= IBlockType.CandyFactory_6)
		{
			CandyFactory candyFactory = block as CandyFactory;
			if ((bool)candyFactory)
			{
				candyFactory.SpewCandyByBooster(BoosterType.CandyPack);
			}
		}
	}

	public override void StartMixEffect()
	{
		StartCoroutine(MixEffect());
	}

	private IEnumerator MixEffect()
	{
		GameObject makeEffect = SpawnStringBlock.GetSpawnBlockObject(SpawnStringBlockType.RainbowRainbowMixMake);
		makeEffect.transform.position = base.transform.position + new Vector3(-1f, 21f, 0f);
		makeEffect.transform.localScale = Chip.baseScale;
		PoolManager.PoolGameBlocks.Despawn(makeEffect.transform, 3.45f);
		while (parentSlot == null)
		{
			yield return 0;
		}
		base.transform.position = parentSlot.transform.position;
		bool isPaintJelly = parentSlot.slot.IsPaintedJelly;
		base.IsMatching = true;
		GameMain.main.EventCounter();
		GameMain.main.isLockDrop = true;
		sx = parentSlot.slot.x;
		sy = parentSlot.slot.y;
		SoundSFX.Play(SFXIndex.RainbowRainbowBombMix);
		yield return new WaitForSeconds(2.5f);
		GameObject crushEffect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.RainbowRainbowMixRemove);
		crushEffect.transform.position = base.transform.position;
		crushEffect.transform.localScale = Chip.baseScale;
		PoolManager.PoolGameEffect.Despawn(crushEffect.transform, 1.9f);
		yield return new WaitForSeconds(0.12f);
		List<Slot> target = new List<Slot>();
		for (int r = 1; r < 18; r++)
		{
			int crushCount = 0;
			for (int i = sx - r; i <= sx + r; i++)
			{
				for (int j = sy - r; j <= sy + r; j++)
				{
					if ((sx - i) * (sx - i) + (sy - j) * (sy - j) > r * r)
					{
						continue;
					}
					Slot s = BoardManager.main.GetSlot(i, j);
					if (!(s != null) || target.Contains(s))
					{
						continue;
					}
					target.Add(s);
					crushCount++;
					BoardManager main = BoardManager.main;
					int x = i;
					int y = j;
					bool radius = false;
					ScoreType scoreType = ScoreType.ChipCrushByItemBlock;
					bool includeCrushChip = true;
					int id = base.id;
					main.SlotCrush(x, y, radius, scoreType, includeCrushChip, 0, id, Side.Null, isPaintJelly);
					BlockInterface block = s.GetBlock();
					if ((bool)block && block.blockType >= IBlockType.SpriteDrink_1_HP1 && block.blockType <= IBlockType.SpriteDrink_6_HP3)
					{
						SpriteDrink spriteDrink = block as SpriteDrink;
						if ((bool)spriteDrink)
						{
							spriteDrink.CrushByForce();
						}
					}
				}
			}
			if (crushCount > 0)
			{
				yield return new WaitForSeconds(0.1f);
			}
		}
		base.IsMatching = false;
		GameMain.main.isLockDrop = false;
		destroingLock = false;
		StartCoroutine(DestroyChipFunction());
	}

	public void StartMixEffectRoundCrush()
	{
		StartCoroutine(RoundCrushMixEffect());
	}

	private IEnumerator RoundCrushMixEffect()
	{
		while (parentSlot == null)
		{
			yield return 0;
		}
		base.transform.position = parentSlot.transform.position;
		bool isPaintJelly = parentSlot.slot.IsPaintedJelly;
		base.IsMatching = true;
		GameMain.main.EventCounter();
		GameMain.main.isLockDrop = true;
		sx = parentSlot.slot.x;
		sy = parentSlot.slot.y;
		GameObject crushEffect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.RainbowRainbowMixRemove);
		crushEffect.transform.position = base.transform.position;
		crushEffect.transform.localScale = Chip.baseScale;
		PoolManager.PoolGameEffect.Despawn(crushEffect.transform, 1.9f);
		yield return new WaitForSeconds(0.12f);
		List<Slot> target = new List<Slot>();
		for (int r = 1; r < 18; r++)
		{
			int crushCount = 0;
			for (int i = sx - r; i <= sx + r; i++)
			{
				for (int j = sy - r; j <= sy + r; j++)
				{
					if ((sx - i) * (sx - i) + (sy - j) * (sy - j) <= r * r)
					{
						Slot s = BoardManager.main.GetSlot(i, j);
						if (s != null && !target.Contains(s))
						{
							target.Add(s);
							crushCount++;
							BoardManager main = BoardManager.main;
							int x = i;
							int y = j;
							bool radius = false;
							ScoreType scoreType = ScoreType.ChipCrushByItemBlock;
							bool includeCrushChip = true;
							int id = base.id;
							main.SlotCrush(x, y, radius, scoreType, includeCrushChip, 0, id, Side.Null, isPaintJelly);
						}
					}
				}
			}
			if (crushCount > 0)
			{
				yield return new WaitForSeconds(0.1f);
			}
		}
		base.IsMatching = false;
		GameMain.main.isLockDrop = false;
		BoardManager.main.SlotCrush(sx, sy, radius: false, ScoreType.ChipCrushByItemBlock, includeCrushChip: false);
		destroingLock = false;
		StartCoroutine(DestroyChipFunction());
	}
}
