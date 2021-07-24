using PathologicalGames;
using System.Collections;
using UnityEngine;

public class ColorHBomb : Chip
{
	public override void Awake()
	{
		base.Awake();
		chipType = ChipType.HBomb;
		aniType = AnimationController.IdleAnimationType.ColorHVBomb;
	}

	public override void OnClick()
	{
		base.OnClick();
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
		GameObject spawnBlockObjectHBombMake = SpawnStringBlock.GetSpawnBlockObjectHBombMake(id);
		spawnBlockObjectHBombMake.transform.position = base.transform.position;
		spawnBlockObjectHBombMake.transform.parent = base.transform;
		spawnBlockObjectHBombMake.transform.localScale = Vector3.one;
		Utils.EnableAllSpriteRenderer(spawnBlockObjectHBombMake);
		canMove = false;
		StartCoroutine(WaitCreateEffect(spawnBlockObjectHBombMake));
	}

	private IEnumerator WaitCreateEffect(GameObject objEffect)
	{
		yield return new WaitForSeconds(GameMain.BlockDropDelayTime);
		canMove = true;
		yield return new WaitForSeconds(0.3f);
		objEffect.transform.parent = PoolManager.PoolGameBlocks.transform;
		PoolManager.PoolGameBlocks.Despawn(objEffect.transform);
		if (!destroying)
		{
			Utils.EnableAllSpriteRenderer(base.gameObject);
		}
	}

	public override IEnumerator DestroyChipFunction()
	{
        canMove = false;
		base.IsMatching = true;
		int sx = parentSlot.slot.x;
		int sy = parentSlot.slot.y;
		bool isPaintJelly = parentSlot.slot.IsPaintedJelly;
		SoundSFX.PlayCap((SFXIndex)(42));
		Utils.DisableAllSpriteRenderer(base.gameObject);
		if (id != -1)
		{
			GameObject crushEffect = SpawnStringEffect.GetSpawnEffectObjectHBombCrush(id);
			crushEffect.transform.position = base.transform.position;
			PoolManager.PoolGameEffect.Despawn(crushEffect.transform, 1.2f);
			yield return new WaitForSeconds(0.2f);
		}
		ParentRemove();

		GlobalEventObserver.InvokeTriggerSpecialBombEvent(chipType);

		int width = BoardManager.main.boardData.width;
		bool IsPaintedJellyL = isPaintJelly;
		bool IsPaintedJellyR = isPaintJelly;
		for (int i = 1; i < width; i++)
		{
			int xL = sx - i;
			int xR = sx + i;
			Slot newSlotL = BoardManager.main.GetSlot(xL, sy);
			Slot newSlotR = BoardManager.main.GetSlot(xR, sy);
			if (newSlotL != null && newSlotL.IsPaintedJelly)
			{
				IsPaintedJellyL = true;
			}
			if (newSlotR != null && newSlotR.IsPaintedJelly)
			{
				IsPaintedJellyR = true;
			}
			if (xL >= 0 && xL < width)
			{
				BoardManager.main.SlotCrush(xL, sy, radius: false, ScoreType.ChipCrushByItemBlock, includeCrushChip: true, 0, id, Side.Left, IsPaintedJellyL);
			}
			if (xR >= 0 && xR < width)
			{
				BoardManager.main.SlotCrush(xR, sy, radius: false, ScoreType.ChipCrushByItemBlock, includeCrushChip: true, 0, id, Side.Right, IsPaintedJellyR);
			}
			yield return new WaitForSeconds(0.04f);
		}
		base.IsMatching = false;
		BoardManager.main.SlotCrush(sx, sy, radius: false, ScoreType.CrushItem4x1, includeCrushChip: false);
		if (PoolManager.PoolGameBlocks.IsSpawned(base.transform))
		{
			PoolManager.PoolGameBlocks.Despawn(base.transform);
		}
	}
}
