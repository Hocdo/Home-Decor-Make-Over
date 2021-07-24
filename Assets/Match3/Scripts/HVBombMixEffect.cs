using PathologicalGames;
using System.Collections;
using UnityEngine;

public class HVBombMixEffect : BombMixEffect
{
	private bool isCompleteHLine;

	private bool isCompleteVLine;

	private int sx;

	private int sy;

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
		sx = parentSlot.slot.x;
		sy = parentSlot.slot.y;
		bool isPaintJelly = parentSlot.slot.IsPaintedJelly;
		base.IsMatching = true;
		GameMain.main.EventCounter();
		if (id != -1)
		{
			if (!isCandyMix)
			{
				GameObject spawnEffectObjectHVBombMixCrush = SpawnStringEffect.GetSpawnEffectObjectHVBombMixCrush(id);
				spawnEffectObjectHVBombMixCrush.transform.position = base.transform.position;
				PoolManager.PoolGameEffect.Despawn(spawnEffectObjectHVBombMixCrush.transform, 1.5f);
			}
			else
			{
				GameObject spawnEffectObjectSmallCandyHVMixRemove = SpawnStringEffect.GetSpawnEffectObjectSmallCandyHVMixRemove(id);
				spawnEffectObjectSmallCandyHVMixRemove.transform.position = base.transform.position;
				PoolManager.PoolGameEffect.Despawn(spawnEffectObjectSmallCandyHVMixRemove.transform, 1.5f);
			}
		}
		SoundSFX.Play(SFXIndex.HVBombMix);
		Slot lockGravitySlot = parentSlot.slot;
		if ((bool)lockGravitySlot)
		{
			lockGravitySlot.PauseGravity(0f);
		}
		if (!isCandyMix)
		{
			ParentRemove();
		}
		yield return new WaitForSeconds(0.15f);
		StartCoroutine(RemoveHLine(sx, sy, isPaintJelly));
		StartCoroutine(RemoveVLine(sx, sy, isPaintJelly));
		while (!isCompleteHLine || !isCompleteVLine)
		{
			yield return null;
		}
		yield return new WaitForSeconds(0.12f);
		base.IsMatching = false;
		BoardManager.main.SlotCrush(sx, sy, radius: false, ScoreType.MixItem4x1to4x1, includeCrushChip: false);
		if (!isCandyMix)
		{
			ParentRemove();
		}
		destroingLock = false;
		if ((bool)lockGravitySlot)
		{
			lockGravitySlot.ResumeGravity();
		}
		base.gameObject.transform.parent = PoolManager.PoolGameBlocks.transform;
		PoolManager.PoolGameBlocks.Despawn(base.gameObject.transform);
	}

	private IEnumerator RemoveHLine(int sx, int sy, bool isPaintJelly)
	{
		isCompleteHLine = false;
		for (int i = 1; i < BoardManager.main.boardData.width; i++)
		{
			int xL = sx - i;
			int xR = sx + i;
			bool isPause = false;
			if (xL >= 0 && xL < BoardManager.main.boardData.width)
			{
				BoardManager main = BoardManager.main;
				int x = xL;
				bool radius = false;
				ScoreType scoreType = ScoreType.ChipCrushByItemBlock;
				int id = base.id;
				main.SlotCrush(x, sy, radius, scoreType, includeCrushChip: true, 0, id, Side.Left, isPaintJelly, subId);
				isPause = true;
			}
			if (xR >= 0 && xR < BoardManager.main.boardData.width)
			{
				BoardManager main2 = BoardManager.main;
				int id = xR;
				bool radius = false;
				ScoreType scoreType = ScoreType.ChipCrushByItemBlock;
				int x = base.id;
				main2.SlotCrush(id, sy, radius, scoreType, includeCrushChip: true, 0, x, Side.Right, isPaintJelly, subId);
				isPause = true;
			}
			if (isPause)
			{
				yield return new WaitForSeconds(0.04f);
			}
		}
		isCompleteHLine = true;
	}

	private IEnumerator RemoveVLine(int sx, int sy, bool isPaintJelly)
	{
		isCompleteVLine = false;
		for (int i = 1; i < BoardManager.main.boardData.height; i++)
		{
			int yT = sy + i;
			int yB = sy - i;
			bool isPause = false;
			if (yT >= 0 && yT < BoardManager.main.boardData.height)
			{
				BoardManager main = BoardManager.main;
				int y = yT;
				bool radius = false;
				ScoreType scoreType = ScoreType.ChipCrushByItemBlock;
				int id = base.id;
				main.SlotCrush(sx, y, radius, scoreType, includeCrushChip: true, 0, id, Side.Top, isPaintJelly, subId);
				isPause = true;
			}
			if (yB >= 0 && yB < BoardManager.main.boardData.height)
			{
				BoardManager main2 = BoardManager.main;
				int y = yB;
				bool radius = false;
				ScoreType scoreType = ScoreType.ChipCrushByItemBlock;
				int id2 = base.id;
				main2.SlotCrush(sx, y, radius, scoreType, includeCrushChip: true, 0, id2, Side.Bottom, isPaintJelly, subId);
				isPause = true;
			}
			if (isPause)
			{
				yield return new WaitForSeconds(0.04f);
			}
		}
		isCompleteVLine = true;
	}
}
