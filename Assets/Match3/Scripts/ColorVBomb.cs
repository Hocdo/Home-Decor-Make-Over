using PathologicalGames;
using System.Collections;
using UnityEngine;

public class ColorVBomb : Chip
{
	public override void Awake()
	{
		base.Awake();
		chipType = ChipType.VBomb;
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
		GameObject spawnBlockObjectVBombMake = SpawnStringBlock.GetSpawnBlockObjectVBombMake(id);
		spawnBlockObjectVBombMake.transform.position = base.transform.position;
		spawnBlockObjectVBombMake.transform.parent = base.transform;
		spawnBlockObjectVBombMake.transform.localScale = Vector3.one;
		Utils.EnableAllSpriteRenderer(spawnBlockObjectVBombMake);
		canMove = false;
		StartCoroutine(WaitCreateEffect(spawnBlockObjectVBombMake));
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
			GameObject crushEffect = SpawnStringEffect.GetSpawnEffectObjectVBombCrush(id);
			crushEffect.transform.position = base.transform.position;
			PoolManager.PoolGameEffect.Despawn(crushEffect.transform, 1.2f);
			yield return new WaitForSeconds(0.2f);
		}
		ParentRemove();

		GlobalEventObserver.InvokeTriggerSpecialBombEvent(chipType);

		int height = BoardManager.main.boardData.height;
		bool IsPaintedJellyT = isPaintJelly;
		bool IsPaintedJellyB = isPaintJelly;
		for (int i = 1; i < height; i++)
		{
			int yT = sy + i;
			int yB = sy - i;
			Slot newSlotT = BoardManager.main.GetSlot(sx, yT);
			Slot newSlotB = BoardManager.main.GetSlot(sx, yB);
			if (newSlotT != null && newSlotT.IsPaintedJelly)
			{
				IsPaintedJellyT = true;
			}
			if (newSlotB != null && newSlotB.IsPaintedJelly)
			{
				IsPaintedJellyB = true;
			}
			if (yT >= 0 && yT < height)
			{
				BoardManager.main.SlotCrush(sx, yT, radius: false, ScoreType.ChipCrushByItemBlock, includeCrushChip: true, 0, id, Side.Top, IsPaintedJellyT);
			}
			if (yB >= 0 && yB < height)
			{
				BoardManager.main.SlotCrush(sx, yB, radius: false, ScoreType.ChipCrushByItemBlock, includeCrushChip: true, 0, id, Side.Bottom, IsPaintedJellyB);
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
