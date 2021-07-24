using PathologicalGames;
using System.Collections;
using UnityEngine;

public class SimpleSimpleMixEffect : BombMixEffect
{
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
		base.IsMatching = true;
		GameMain.main.EventCounter();
		sx = parentSlot.slot.x;
		sy = parentSlot.slot.y;
		bool isPaintJelly = parentSlot.slot.IsPaintedJelly;
		int width = BoardManager.main.boardData.width;
		int height = BoardManager.main.boardData.height;
		GameObject spawnEffectObject = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.BombBombMixCrush);
		spawnEffectObject.transform.position = base.transform.position;
		PoolManager.PoolGameEffect.Despawn(spawnEffectObject.transform, 2.0f);

		yield return new WaitForSeconds(0.5f);

		SoundSFX.Play(SFXIndex.SimpleSimpleBombMix);
		for (int i = -2; i <= 2; i++)
		{
			for (int j = -2; j <= 2; j++)
			{
				int num = sx + j;
				int num2 = sy + i;
				if (num >= 0 && num < width && num2 >= 0 && num2 < height)
				{
					BoardManager main = BoardManager.main;
					int x = num;
					int y = num2;
					bool radius = false;
					ScoreType scoreType = ScoreType.ChipCrushByItemBlock;
					int id = base.id;
					Side crushDir = FindRollingDirection(j, i);
					main.SlotCrush(x, y, radius, scoreType, includeCrushChip: true, 0, id, crushDir, isPaintJelly, subId);
				}
			}
		}
		yield return new WaitForSeconds(1f);
		base.IsMatching = false;
		BoardManager.main.SlotCrush(sx, sy, radius: false, ScoreType.MixItem3x3to3x3, includeCrushChip: false);
		destroingLock = false;
		StartCoroutine(DestroyChipFunction());
	}

	private Side FindRollingDirection(int x, int y)
	{
		Side[] array = new Side[2];
		if (x > 0)
		{
			array[0] = Side.Right;
		}
		else if (x < 0)
		{
			array[0] = Side.Left;
		}
		else
		{
			array[0] = Side.Null;
		}
		if (y > 0)
		{
			array[1] = Side.Top;
		}
		else if (y < 0)
		{
			array[1] = Side.Bottom;
		}
		else
		{
			array[1] = Side.Null;
		}
		if (Mathf.Abs(x) == Mathf.Abs(y))
		{
			if (Random.Range(0, 2) == 0)
			{
				return array[0];
			}
			return array[1];
		}
		return (Mathf.Abs(x) >= Mathf.Abs(y)) ? array[0] : array[1];
	}
}
