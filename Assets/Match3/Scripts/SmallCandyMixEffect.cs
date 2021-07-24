using DG.Tweening;
using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallCandyMixEffect : BombMixEffect
{
	private static readonly float slotDropLockTime = 0.2f;

	private static readonly int RollingCount = 4;

	protected bool isAfterEffect;

	public bool IsPaintedJelly;

	protected Slot lastPositionSlot;

	private readonly List<Slot> listTrampledSlotGravity = new List<Slot>();

	protected GameObject objMixAddEffect;

	protected GameObject objRollingCandy;

	private readonly float RollingMoveDelay = 0.2f;

	private AudioSource rollingSound;

	private Slot startSlot;

	private int startSlotX;

	private int startSlotY;

	public override void OnSpawned()
	{
		base.OnSpawned();
	}

	public virtual void CreateMix(Side sideDirection)
	{
		StartCoroutine(WaitCreateMix(0f, sideDirection));
	}

	protected IEnumerator WaitCreateMix(float waitTime, Side sideDirection)
	{
		GameMain.main.isLockDrop = true;
		yield return new WaitForSeconds(waitTime);
		GameMain.main.isLockDrop = false;
		Rolling(sideDirection);
	}

	public void Rolling(Side sideDirection)
	{
		if ((bool)parentSlot && (bool)parentSlot.slot)
		{
			startSlotX = parentSlot.slot.x;
			startSlotY = parentSlot.slot.y;
			startSlot = parentSlot.slot;
			if (sideDirection == Side.Top)
			{
				startSlot.PauseGravity(0f);
				listTrampledSlotGravity.Add(startSlot);
			}
			else
			{
				startSlot.PauseGravity(slotDropLockTime);
			}
			IsPaintedJelly = parentSlot.slot.IsPaintedJelly;
			if (!parentSlot.slot.GetBlock())
			{
				if (MapData.main.target == GoalTarget.RescueVS)
				{
					if (GameMain.main.CurrentTurn == VSTurn.Player)
					{
						BoardManager main = BoardManager.main;
						int x = startSlotX;
						int y = startSlotY;
						bool radius = false;
						ScoreType scoreType = ScoreType.ChipCrushByItemBlock;
						bool includeCrushChip = true;
						int id = base.id;
						main.SlotCrush(x, y, radius, scoreType, includeCrushChip, 0, id, Side.Null, IsPaintedJelly);
					}
					else
					{
						parentSlot.slot.RockCandyFill();
					}
				}
				else
				{
					BoardManager main2 = BoardManager.main;
					int id = startSlotX;
					int y = startSlotY;
					bool includeCrushChip = false;
					ScoreType scoreType = ScoreType.ChipCrushByItemBlock;
					bool radius = true;
					int x = base.id;
					main2.SlotCrush(id, y, includeCrushChip, scoreType, radius, 0, x, Side.Null, IsPaintedJelly);
				}
			}
		}
		ParentRemove();
		objRollingCandy = SpawnStringBlock.GetSpawnBlockObject(SpawnStringBlockType.SmallCandyRolling);
		objRollingCandy.transform.position = base.transform.position;
		AfterRollingObjectCreate();
		SoundSFX.PlayCap(SFXIndex.CandyBombStart);
		rollingSound = SoundSFX.PlayCap(SFXIndex.CandyBombRollingLoop);
		GameMain.main.rolling++;
		StartCoroutine(RollingProcess(sideDirection));
	}

	protected virtual void AfterRollingObjectCreate()
	{
	}

	protected virtual void RemoveEffect()
	{
		GameObject spawnEffectObject = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.CandyCrush);
		spawnEffectObject.transform.position = objRollingCandy.transform.position;
		PoolManager.PoolGameEffect.Despawn(spawnEffectObject.transform, 0.6f);
		PoolManager.PoolGameBlocks.Despawn(objRollingCandy.transform);
		if ((bool)objMixAddEffect)
		{
			PoolManager.PoolGameEffect.Despawn(objMixAddEffect.transform);
		}
	}

	protected virtual void EndRolling()
	{
		foreach (Slot item in listTrampledSlotGravity)
		{
			if ((bool)item)
			{
				item.ResumeGravity();
			}
		}
		if ((bool)rollingSound)
		{
			rollingSound.Stop();
		}
		SlotGravity.Reshading();
		PoolManager.PoolGameBlocks.Despawn(base.gameObject.transform);
	}

	public IEnumerator WaitRollingNextFrame(Side sideDirection)
	{
		yield return null;
		StartCoroutine(RollingProcess(sideDirection));
	}

	private IEnumerator RollingProcess(Side sideDirection)
	{
		int num;
		int sx = num = startSlotX;
		int tx = num;
		int sy = num = startSlotY;
		int ty = num;
		int rollingcount = MapData.MaxWidth;
		Slot prevTrampledSlot = BoardManager.main.GetSlot(sx, sy);
		SpriteRenderer srRolling = objRollingCandy.GetComponentInChildren<SpriteRenderer>();
		switch (sideDirection)
		{
		case Side.Left:
			tx = Mathf.Max(0, tx - rollingcount);
			srRolling.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));
			break;
		case Side.Right:
			tx = Mathf.Min(BoardManager.main.boardData.width - 1, tx + rollingcount);
			srRolling.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -90f));
			break;
		case Side.Top:
			ty = Mathf.Min(BoardManager.main.boardData.height - 1, ty + rollingcount);
			srRolling.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			break;
		case Side.Bottom:
			ty = Mathf.Max(0, ty - rollingcount);
			srRolling.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
			break;
		default:
			GameMain.main.rolling--;
			GameMain.main.EventCounter();
			yield break;
		}
		while (sx != tx || sy != ty)
		{
			int oldX = sx;
			int oldY = sy;
			if (sx < tx)
			{
				sx++;
			}
			else if (sx > tx)
			{
				sx--;
			}
			if (sy < ty)
			{
				sy++;
			}
			else if (sy > ty)
			{
				sy--;
			}
			Slot s = BoardManager.main.GetSlot(sx, sy);
			if (s == null || !s.canBeCrush)
			{
				break;
			}
			CandyChip otherCandy = s.GetChip() as CandyChip;
			BlockInterface candySlotBlock = s.GetBlock();
			if ((bool)otherCandy && candySlotBlock == null)
			{
				int x = otherCandy.parentSlot.slot.x;
				int y = otherCandy.parentSlot.slot.y;
				Vector3 position = otherCandy.transform.position;
				otherCandy.CrushCandyChip();
				startSlotX = oldX;
				startSlotY = oldY;
				StartCoroutine(ChangeLargeCandy(sideDirection, BoardManager.main.GetSlot(startSlotX, startSlotY), BoardManager.main.GetSlot(x, y)));
				Camera.main.transform.DOShakePosition(RollingMoveDelay, 3f);
				yield break;
			}
			Slot newSlot = BoardManager.main.GetSlot(sx, sy);
			if (newSlot != null && newSlot.GetBlock() != null && newSlot.GetBlock().blockType == IBlockType.Pocket)
			{
				Pocket pocket = newSlot.GetBlock() as Pocket;
				pocket.GoalIn(sideDirection);
				lastPositionSlot = newSlot;
				GameMain.main.rolling--;
				GameMain.main.EventCounter();
				RemoveEffect();
				EndRolling();
				yield break;
			}
			objRollingCandy.transform.DOMove(newSlot.transform.position + new Vector3(0f, 0f, -1f), RollingMoveDelay).SetEase(Ease.Linear);
			yield return new WaitForSeconds(RollingMoveDelay);
			if (newSlot != null)
			{
				if (newSlot.IsPaintedJelly)
				{
					IsPaintedJelly = true;
				}
				BoardManager main = BoardManager.main;
				num = sx;
				int y2 = sy;
				bool radius = false;
				ScoreType scoreType = ScoreType.ChipCrushByItemBlock;
				bool includeCrushChip = true;
				int id = base.id;
				main.SlotCrush(num, y2, radius, scoreType, includeCrushChip, 0, id, Side.Null, IsPaintedJelly);
				if (sideDirection == newSlot.GetSideSlotHead())
				{
					newSlot.PauseGravity(0f);
					listTrampledSlotGravity.Add(newSlot);
				}
				else
				{
					newSlot.PauseGravity(slotDropLockTime);
				}
			}
			prevTrampledSlot = newSlot;
		}
		lastPositionSlot = prevTrampledSlot;
		GameMain.main.rolling--;
		GameMain.main.EventCounter();
		SoundSFX.Play(SFXIndex.CandyBombRemove);
		RemoveEffect();
		EndRolling();
	}

	private IEnumerator ChangeLargeCandy(Side sideDirection, Slot toSlot, Slot fromSlot)
	{
		yield return null;
		lastPositionSlot = toSlot;
		GameMain.main.rolling--;
		GameMain.main.EventCounter();
		RemoveEffect();
		EndRolling();
		Transform t = SpawnStringBlock.GetSpawnBlockObject(SpawnStringBlockType.LargeCandyMixEffect).transform;
		LargeCandyMixEffect effect = t.GetComponent<LargeCandyMixEffect>();
		Chip c = null;
		if (((bool)toSlot.GetChip() && toSlot.GetChip().chipType == ChipType.BringDown) || ((bool)toSlot.GetBlock() && MapData.IsBlockTypeIncludingChip(toSlot.GetBlock().blockType)))
		{
			c = toSlot.GetChip();
		}
		toSlot.SetChip(effect);
		effect.transform.localPosition = Vector3.zero;
		effect.Rolling(sideDirection, toSlot, fromSlot);
		if ((bool)c)
		{
			toSlot.SetChip(c);
		}
	}
}
