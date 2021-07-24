using DG.Tweening;
using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeCandyMixEffect : BombMixEffect
{
	private static readonly int RollingCount = 4;

	private Slot holdingSlotA;

	private Slot holdingSlotB;

	public bool IsPaintedJelly;

	private readonly List<Slot> listTrampledSlotGravity = new List<Slot>();

	private GameObject objRollingCandy;

	public float RollingMoveDelay = 0.2f;

	private AudioSource rollingSound;

	private int size = 3;

	private int startSlotX;

	private int startSlotY;

	private static readonly int[] rollingMoveBlockOrderSideRight_5 = new int[10]
	{
		2,
		2,
		2,
		1,
		2,
		0,
		2,
		-1,
		2,
		-2
	};

	private static readonly int[] rollingMoveBlockOrderSideLeft_5 = new int[10]
	{
		-2,
		2,
		-2,
		1,
		-2,
		0,
		-2,
		-1,
		-2,
		-2
	};

	private static readonly int[] rollingMoveBlockOrderSideTop_5 = new int[10]
	{
		-2,
		2,
		-1,
		2,
		0,
		2,
		1,
		2,
		2,
		2
	};

	private static readonly int[] rollingMoveBlockOrderSideBottom_5 = new int[10]
	{
		-2,
		-2,
		-1,
		-2,
		0,
		-2,
		1,
		-2,
		2,
		-2
	};

	private static readonly int[] rollingMoveBlockOrderSideRight_4 = new int[8]
	{
		1,
		2,
		1,
		1,
		1,
		0,
		1,
		-1
	};

	private static readonly int[] rollingMoveBlockOrderSideLeft_4 = new int[8]
	{
		-2,
		2,
		-2,
		1,
		-2,
		0,
		-2,
		-1
	};

	private static readonly int[] rollingMoveBlockOrderSideTop_4 = new int[8]
	{
		-2,
		2,
		-1,
		2,
		0,
		2,
		1,
		2
	};

	private static readonly int[] rollingMoveBlockOrderSideBottom_4 = new int[8]
	{
		-2,
		-1,
		-1,
		-1,
		0,
		-1,
		1,
		-1
	};

	private static readonly int[] rollingMoveBlockOrderSideRight = new int[6]
	{
		1,
		1,
		1,
		0,
		1,
		-1
	};

	private static readonly int[] rollingMoveBlockOrderSideLeft = new int[6]
	{
		-1,
		1,
		-1,
		0,
		-1,
		-1
	};

	private static readonly int[] rollingMoveBlockOrderSideTop = new int[6]
	{
		-1,
		1,
		0,
		1,
		1,
		1
	};

	private static readonly int[] rollingMoveBlockOrderSideBottom = new int[6]
	{
		-1,
		-1,
		0,
		-1,
		1,
		-1
	};

	public override void OnSpawned()
	{
		base.OnSpawned();
		size = 3;
	}

	public void Rolling(Side sideDirection, Slot holdingSlotA, Slot holdingSlotB)
	{
		if ((bool)parentSlot && (bool)parentSlot.slot)
		{
			startSlotX = parentSlot.slot.x;
			startSlotY = parentSlot.slot.y;
			IsPaintedJelly = parentSlot.slot.IsPaintedJelly;
		}
		this.holdingSlotA = holdingSlotA;
		this.holdingSlotB = holdingSlotB;
		GameMain.main.rolling++;
		ParentRemove();
		StartCoroutine(preRollingProcess(sideDirection));
		SoundSFX.PlayCap(SFXIndex.CandyBombStart);
		rollingSound = SoundSFX.Play(SFXIndex.CandyBombCandyBombMixRollingLoop);
	}

	private IEnumerator preRollingProcess(Side sideDirection)
	{
		int sx = startSlotX;
		int sy = startSlotY;
		float makingTime = 0.5f;
		GameObject objMake = SpawnStringBlock.GetSpawnBlockObject(SpawnStringBlockType.LargeCandyMake);
		objMake.transform.position = base.transform.position + new Vector3(0f, 0f, -1f);
		PoolManager.PoolGameBlocks.Despawn(objMake.transform, makingTime);
		yield return new WaitForSeconds(0.3f);
		holdingSlotA.ResumeGravity();
		holdingSlotB.ResumeGravity();
		if (MapData.main.target == GoalTarget.Jelly)
		{
			for (int i = 0; i < 3; i++)
			{
				if (IsPaintedJelly)
				{
					break;
				}
				for (int j = 0; j < 3; j++)
				{
					if (IsPaintedJelly)
					{
						break;
					}
					int x = sx - 1 + i;
					int y = sy + 1 - j;
					Slot slot = BoardManager.main.GetSlot(x, y);
					if ((bool)slot && slot.IsPaintedJelly)
					{
						IsPaintedJelly = true;
					}
				}
			}
		}
		yield return null;
		for (int k = 0; k < 3; k++)
		{
			for (int l = 0; l < 3; l++)
			{
				int num = sx - 1 + k;
				int num2 = sy + 1 - l;
				Slot slot2 = BoardManager.main.GetSlot(num, num2);
				if ((bool)slot2)
				{
					BoardManager main = BoardManager.main;
					int x2 = num;
					int y2 = num2;
					bool radius = false;
					ScoreType scoreType = ScoreType.Match3;
					bool includeCrushChip = true;
					int id = base.id;
					main.SlotCrush(x2, y2, radius, scoreType, includeCrushChip, 0, id, Side.Null, IsPaintedJelly);
					listTrampledSlotGravity.Add(slot2);
					slot2.PauseGravity(0f);
				}
			}
		}
		yield return new WaitForSeconds(0.2f);
		objRollingCandy = SpawnStringBlock.GetSpawnBlockObject(SpawnStringBlockType.LargeCandyRolling);
		objRollingCandy.transform.position = base.transform.position;
		objRollingCandy.transform.localScale = Vector3.one;
		StartCoroutine(RollingProcess(sideDirection));
	}

	private IEnumerator RollingProcess(Side sideDirection)
	{
		if (!objRollingCandy)
		{
			yield break;
		}
		int num;
		int sx = num = startSlotX;
		int tx = num;
		int sy = num = startSlotY;
		int ty = num;
		bool stopRolling = false;
		int rollingCount = MapData.MaxWidth;
		int[] rollingMoveBlockOrder;
		int[] rollingMoveBlockOrderMirror;
		switch (sideDirection)
		{
		case Side.Left:
			objRollingCandy.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));
			tx = Mathf.Max(1, tx - rollingCount);
			rollingMoveBlockOrder = rollingMoveBlockOrderSideLeft;
			rollingMoveBlockOrderMirror = rollingMoveBlockOrderSideRight;
			if (sx < tx)
			{
				stopRolling = true;
			}
			break;
		case Side.Right:
			objRollingCandy.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -90f));
			tx = Mathf.Min(BoardManager.main.boardData.width - 2, tx + rollingCount);
			rollingMoveBlockOrder = rollingMoveBlockOrderSideRight;
			rollingMoveBlockOrderMirror = rollingMoveBlockOrderSideLeft;
			if (sx > tx)
			{
				stopRolling = true;
			}
			break;
		case Side.Top:
			objRollingCandy.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			ty = Mathf.Min(BoardManager.main.boardData.height - 2, ty + rollingCount);
			rollingMoveBlockOrder = rollingMoveBlockOrderSideTop;
			rollingMoveBlockOrderMirror = rollingMoveBlockOrderSideBottom;
			if (sy > ty)
			{
				stopRolling = true;
			}
			break;
		case Side.Bottom:
			objRollingCandy.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
			ty = Mathf.Max(1, ty - rollingCount);
			rollingMoveBlockOrder = rollingMoveBlockOrderSideBottom;
			rollingMoveBlockOrderMirror = rollingMoveBlockOrderSideTop;
			if (sy < ty)
			{
				stopRolling = true;
			}
			break;
		default:
			GameMain.main.rolling--;
			GameMain.main.EventCounter();
			yield break;
		}
		while (!stopRolling && (sx != tx || sy != ty))
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
			objRollingCandy.transform.DOMove(BoardManager.main.GetSlotPosition(sx, sy) + new Vector3(0f, 0f, -1f), RollingMoveDelay).SetEase(Ease.Linear);
			Camera.main.transform.DOShakePosition(RollingMoveDelay, 0.1f);
			yield return new WaitForSeconds(RollingMoveDelay);
			if (MapData.main.target == GoalTarget.Jelly)
			{
				for (int i = 0; i < 3; i++)
				{
					if (IsPaintedJelly)
					{
						break;
					}
					int num2 = rollingMoveBlockOrder[i * 2];
					int num3 = rollingMoveBlockOrder[i * 2 + 1];
					Slot slot = BoardManager.main.GetSlot(sx + num2, sy + num3);
					if ((bool)slot && slot.IsPaintedJelly)
					{
						IsPaintedJelly = true;
					}
				}
			}
			for (int j = 0; j < 3; j++)
			{
				int num4 = rollingMoveBlockOrder[j * 2];
				int num5 = rollingMoveBlockOrder[j * 2 + 1];
				Slot slot2 = BoardManager.main.GetSlot(sx + num4, sy + num5);
				if ((bool)slot2)
				{
					BoardManager main = BoardManager.main;
					num = sx + num4;
					int y = sy + num5;
					bool radius = false;
					ScoreType scoreType = ScoreType.Match3;
					bool includeCrushChip = true;
					int id = base.id;
					main.SlotCrush(num, y, radius, scoreType, includeCrushChip, 0, id, Side.Null, IsPaintedJelly);
				}
				Slot slot3 = BoardManager.main.GetSlot(oldX + rollingMoveBlockOrderMirror[j * 2], oldY + rollingMoveBlockOrderMirror[j * 2 + 1]);
				if ((bool)slot3)
				{
					slot3.ResumeGravity();
				}
				if ((bool)slot2)
				{
					if (!listTrampledSlotGravity.Contains(slot2))
					{
						listTrampledSlotGravity.Add(slot2);
					}
					slot2.PauseGravity(0f);
				}
			}
		}
		if (MapData.main.target == GoalTarget.Jelly)
		{
			for (int k = 0; k < 3; k++)
			{
				if (IsPaintedJelly)
				{
					break;
				}
				for (int l = 0; l < 3; l++)
				{
					if (IsPaintedJelly)
					{
						break;
					}
					int x = sx - 1 + k;
					int y2 = sy + 1 - l;
					Slot slot4 = BoardManager.main.GetSlot(x, y2);
					if ((bool)slot4 && slot4.IsPaintedJelly)
					{
						IsPaintedJelly = true;
					}
				}
			}
		}
		yield return null;
		for (int m = 0; m < 3; m++)
		{
			for (int n = 0; n < 3; n++)
			{
				int num6 = sx - 1 + m;
				int num7 = sy + 1 - n;
				Slot slot5 = BoardManager.main.GetSlot(num6, num7);
				if ((bool)slot5)
				{
					if (!stopRolling)
					{
						BoardManager main2 = BoardManager.main;
						int id = num6;
						int y = num7;
						bool includeCrushChip = false;
						ScoreType scoreType = ScoreType.Match3;
						bool radius = true;
						num = base.id;
						main2.SlotCrush(id, y, includeCrushChip, scoreType, radius, 0, num, Side.Null, IsPaintedJelly);
					}
					if (listTrampledSlotGravity.Contains(slot5))
					{
						slot5.ResumeGravity();
					}
				}
			}
		}
		PoolManager.PoolGameBlocks.Despawn(objRollingCandy.transform);
		SlotGravity.Reshading();
		GameObject crushEffect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.LargeCandyCrush);
		crushEffect.transform.position = objRollingCandy.transform.position;
		PoolManager.PoolGameEffect.Despawn(crushEffect.transform, 0.6f);
		PoolManager.PoolGameBlocks.Despawn(base.gameObject.transform);
		GameMain.main.rolling--;
		GameMain.main.EventCounter();
		if ((bool)rollingSound)
		{
			rollingSound.Stop();
		}
		SoundSFX.Play(SFXIndex.CandyBombCandyBombMixRollingEnd);
	}

	private void SetOrderRollingMoveBlock(Side sideDirection, out int[] order, out int[] orderMirror)
	{
		switch (sideDirection)
		{
		case Side.Left:
			if (size == 3)
			{
				order = rollingMoveBlockOrderSideLeft;
				orderMirror = rollingMoveBlockOrderSideRight;
			}
			else if (size == 4)
			{
				order = rollingMoveBlockOrderSideLeft_4;
				orderMirror = rollingMoveBlockOrderSideRight_4;
			}
			else
			{
				order = rollingMoveBlockOrderSideLeft_5;
				orderMirror = rollingMoveBlockOrderSideRight_5;
			}
			break;
		case Side.Right:
			if (size == 3)
			{
				order = rollingMoveBlockOrderSideRight;
				orderMirror = rollingMoveBlockOrderSideLeft;
			}
			else if (size == 4)
			{
				order = rollingMoveBlockOrderSideRight_4;
				orderMirror = rollingMoveBlockOrderSideLeft_4;
			}
			else
			{
				order = rollingMoveBlockOrderSideRight_5;
				orderMirror = rollingMoveBlockOrderSideLeft_5;
			}
			break;
		case Side.Top:
			if (size == 3)
			{
				order = rollingMoveBlockOrderSideTop;
				orderMirror = rollingMoveBlockOrderSideBottom;
			}
			else if (size == 4)
			{
				order = rollingMoveBlockOrderSideTop_4;
				orderMirror = rollingMoveBlockOrderSideBottom_4;
			}
			else
			{
				order = rollingMoveBlockOrderSideTop_5;
				orderMirror = rollingMoveBlockOrderSideBottom_5;
			}
			break;
		case Side.Bottom:
			if (size == 3)
			{
				order = rollingMoveBlockOrderSideBottom;
				orderMirror = rollingMoveBlockOrderSideTop;
			}
			else if (size == 4)
			{
				order = rollingMoveBlockOrderSideBottom_4;
				orderMirror = rollingMoveBlockOrderSideTop_4;
			}
			else
			{
				order = rollingMoveBlockOrderSideBottom_5;
				orderMirror = rollingMoveBlockOrderSideTop_5;
			}
			break;
		default:
			order = new int[size * 2];
			orderMirror = new int[size * 2];
			break;
		}
	}
}
