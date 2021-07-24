using PathologicalGames;
using System.Collections;
using UnityEngine;

public class SimpleHBombMixEffect : BombMixEffect
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
		while (parentSlot == null)
		{
			yield return 0;
		}
		base.transform.position = parentSlot.transform.position;
		bool isPaintJelly = parentSlot.slot.IsPaintedJelly;
		base.IsMatching = true;
		GameMain.main.EventCounter();
		int sx = parentSlot.slot.x;
		int sy = parentSlot.slot.y;
		if (base.id != -1)
		{
			GameObject spawnEffectObjectHBombBombMixCrush = SpawnStringEffect.GetSpawnEffectObjectHBombBombMixCrush(base.id);
			spawnEffectObjectHBombBombMixCrush.transform.position = base.transform.position;
			PoolManager.PoolGameEffect.Despawn(spawnEffectObjectHBombBombMixCrush.transform, 1.6f);
		}
		SoundSFX.Play(SFXIndex.HVBombSimpleBombMix);
		yield return new WaitForSeconds(0.3f);
		int width = BoardManager.main.boardData.width;
		int height = BoardManager.main.boardData.height;
		for (int i = 0; i < width; i++)
		{
			for (int j = sy - 1; j <= sy + 1; j++)
			{
				if (j < 0 || j >= height)
				{
					continue;
				}
				int num = sx - i;
				int num2 = sx + i;
				if (num != sx || j != sy)
				{
					if (num >= 0 && num < width)
					{
						BoardManager main = BoardManager.main;
						int x = num;
						int y = j;
						bool radius = false;
						ScoreType scoreType = ScoreType.ChipCrushByItemBlock;
						int id = base.id;
						main.SlotCrush(x, y, radius, scoreType, includeCrushChip: true, 0, id, Side.Left, isPaintJelly, subId);
					}
					if (num2 >= 0 && num2 < width && i != 0)
					{
						BoardManager main2 = BoardManager.main;
						int id = num2;
						int y = j;
						bool radius = false;
						ScoreType scoreType = ScoreType.ChipCrushByItemBlock;
						int x = base.id;
						main2.SlotCrush(id, y, radius, scoreType, includeCrushChip: true, 0, x, Side.Right, isPaintJelly, subId);
					}
				}
			}
			yield return new WaitForSeconds(0.05f);
		}
		base.IsMatching = false;
		BoardManager.main.SlotCrush(sx, sy, radius: false, ScoreType.MixItem4x1to3x3, includeCrushChip: false);
		destroingLock = false;
		StartCoroutine(DestroyChipFunction());
	}
}
