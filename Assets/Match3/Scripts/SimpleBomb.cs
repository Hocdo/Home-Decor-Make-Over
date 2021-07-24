using PathologicalGames;
using System.Collections;
using UnityEngine;

public class SimpleBomb : Chip
{
	public bool isBiggerBomb = true;

	private bool isPaintJelly;

	private bool isPaintJellyRoot;

	public override void Awake()
	{
		base.Awake();
		chipType = ChipType.SimpleBomb;
		aniType = AnimationController.IdleAnimationType.SimpleBomb;
	}

	public override void PlayIdleAnimation(AnimationController.IdleAnimationType type, AnimationProperty prop)
	{
		ParticleSystem[] componentsInChildren = base.transform.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem particleSystem in componentsInChildren)
		{
			particleSystem.Play(withChildren: true);
		}
	}

	public override void ShowCreateEffect()
	{
		base.ShowCreateEffect();
		GameObject spawnBlockObjectBombMake = SpawnStringBlock.GetSpawnBlockObjectBombMake(id);
		spawnBlockObjectBombMake.transform.position = base.transform.position;
		spawnBlockObjectBombMake.transform.parent = base.transform;

		float scale = 0.909099090f;
		spawnBlockObjectBombMake.transform.localScale = new Vector3(scale, scale, scale);
		canMove = false;
		StartCoroutine(WaitCreateEffect(spawnBlockObjectBombMake));
	}

	private IEnumerator WaitCreateEffect(GameObject objEffect)
	{
		canMove = true;
		yield return new WaitForSeconds(0.5f);
		objEffect.transform.parent = PoolManager.PoolGameBlocks.transform;
		PoolManager.PoolGameBlocks.Despawn(objEffect.transform);
		Utils.EnableAllSpriteRenderer(base.gameObject);
	}

	public override IEnumerator DestroyChipFunction()
	{
		canMove = false;
		base.IsMatching = true;
		int sx = parentSlot.slot.x;
		int sy = parentSlot.slot.y;
		isPaintJelly = parentSlot.slot.IsPaintedJelly;
		isPaintJellyRoot = isPaintJelly;
		if (!isCandyMix)
		{
			ParentRemove();
		}
		yield return null;
		SoundSFX.PlayCap(SFXIndex.SimpleBombRemove);
		Utils.DisableAllSpriteRenderer(base.gameObject);
		if (id != -1)
		{
			GameObject spawnEffectObjectSimpleBombCrush = SpawnStringEffect.GetSpawnEffectObjectSimpleBombCrush(id);
			spawnEffectObjectSimpleBombCrush.transform.position = base.transform.position;
			PoolManager.PoolGameEffect.Despawn(spawnEffectObjectSimpleBombCrush.transform, 1f);
			if (isCandyMix)
			{
				GameObject spawnEffectObjectSmallCandyBombMixRemove = SpawnStringEffect.GetSpawnEffectObjectSmallCandyBombMixRemove(id);
				spawnEffectObjectSmallCandyBombMixRemove.transform.position = base.transform.position;
				PoolManager.PoolGameEffect.Despawn(spawnEffectObjectSmallCandyBombMixRemove.transform, 1f);
			}
		}
		if (!isCandyMix)
		{
			ParentRemove();
		}
		base.IsMatching = false;
		if (PoolManager.PoolGameBlocks.IsSpawned(base.transform))
		{
			PoolManager.PoolGameBlocks.Despawn(base.transform);
		}

		GlobalEventObserver.InvokeTriggerSpecialBombEvent(chipType);

		SlotCrush(sx, sy, 0, 0);
		isPaintJelly = isPaintJellyRoot;
		SlotCrush(sx + 1, sy, 1, 0);
		SlotCrush(sx + 2, sy, 2, 0);
		isPaintJelly = isPaintJellyRoot;
		SlotCrush(sx - 1, sy, -1, 0);
		SlotCrush(sx - 2, sy, -2, 0);
		isPaintJelly = isPaintJellyRoot;
		SlotCrush(sx, sy + 1, 0, 1);
		SlotCrush(sx, sy + 2, 0, 2);
		isPaintJelly = isPaintJellyRoot;
		SlotCrush(sx, sy - 1, -1, 0);
		SlotCrush(sx, sy - 2, -2, 0);
		isPaintJelly = isPaintJellyRoot;
		SlotCrush(sx + 1, sy + 1, 1, 1);
		isPaintJelly = isPaintJellyRoot;
		SlotCrush(sx + 1, sy - 1, 1, -1);
		isPaintJelly = isPaintJellyRoot;
		SlotCrush(sx - 1, sy + 1, -1, 1);
		isPaintJelly = isPaintJellyRoot;
		SlotCrush(sx - 1, sy - 1, -1, -1);
		BoardManager.main.SlotCrush(sx, sy, radius: false, ScoreType.CrushItem5x1, includeCrushChip: false);
		yield return new WaitForSeconds(0.8f);
	}

	private void SlotCrush(int x, int y, int offsetX, int offsetY)
	{
		int width = BoardManager.main.boardData.width;
		int height = BoardManager.main.boardData.height;
		Slot slot = BoardManager.main.GetSlot(x, y);
		if (slot != null && slot.IsPaintedJelly)
		{
			isPaintJelly = true;
		}
		if (x >= 0 && x < width && y >= 0 && y < height)
		{
			BoardManager main = BoardManager.main;
			bool radius = false;
			ScoreType scoreType = ScoreType.ChipCrushByItemBlock;
			int id = base.id;
			Side crushDir = FindRollingDirection(offsetX, offsetY);
			main.SlotCrush(x, y, radius, scoreType, includeCrushChip: true, 0, id, crushDir, isPaintJelly);
		}
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
