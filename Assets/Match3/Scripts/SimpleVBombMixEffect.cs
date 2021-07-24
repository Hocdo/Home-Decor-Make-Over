using PathologicalGames;
using System.Collections;
using UnityEngine;

public class SimpleVBombMixEffect : BombMixEffect
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
		base.IsMatching = true;
		GameMain.main.EventCounter();
		int sx = parentSlot.slot.x;
		int sy = parentSlot.slot.y;
		bool isPaintJelly = parentSlot.slot.IsPaintedJelly;
		if (base.id != -1)
		{
			GameObject spawnEffectObjectVBombBombMixCrush = SpawnStringEffect.GetSpawnEffectObjectVBombBombMixCrush(base.id);
			spawnEffectObjectVBombBombMixCrush.transform.position = base.transform.position;
			PoolManager.PoolGameEffect.Despawn(spawnEffectObjectVBombBombMixCrush.transform, 1.6f);
		}
		SoundSFX.Play(SFXIndex.HVBombSimpleBombMix);
		yield return new WaitForSeconds(0.3f);
		int width = BoardManager.main.boardData.width;
		int height = BoardManager.main.boardData.height;
		for (int i = 0; i < height; i++)
		{
			for (int j = sx - 1; j <= sx + 1; j++)
			{
				if (j < 0 || j >= width)
				{
					continue;
				}
				int num = sy + i;
				int num2 = sy - i;
				if (num != sy || j != sx)
				{
					if (num >= 0 && num < height)
					{
						BoardManager main = BoardManager.main;
						int x = j;
						int y = num;
						bool radius = false;
						ScoreType scoreType = ScoreType.ChipCrushByItemBlock;
						int id = base.id;
						main.SlotCrush(x, y, radius, scoreType, includeCrushChip: true, 0, id, Side.Top, isPaintJelly, subId);
					}
					if (num2 >= 0 && num2 < height && i != 0)
					{
						BoardManager main2 = BoardManager.main;
						int id = j;
						int y = num2;
						bool radius = false;
						ScoreType scoreType = ScoreType.ChipCrushByItemBlock;
						int x = base.id;
						main2.SlotCrush(id, y, radius, scoreType, includeCrushChip: true, 0, x, Side.Bottom, isPaintJelly, subId);
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
