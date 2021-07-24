using DG.Tweening;
using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
	public bool gravity;

	public bool canBeCrush = true;

	public bool canBeControl = true;

	public bool isPausingGravity;

	public bool generator;

	public bool teleportTarget;

	public bool isRailRoad;

	public bool isSafeObs;

	public Slot railNextSlot;

	public bool IsControlLock;

	private readonly float fillingMilkEffectTime = 1.017f;

	private BlockInterface block;

	private bool initialized;

	public int x;

	public int y;

	private RockCandy rockCandy;

	private DropDirection dropDirection;

	public bool isFillSweet;

	public bool IsFilledSweet;

	private SpriteRenderer MilkLayer;

	public bool IsPaintedJelly;

	private SpriteRenderer JellyLayer;

	private SpriteRenderer EmptyLayer;

	private readonly List<Side> fillingSide = new List<Side>();

	private int fillMilkFrameCount;

	public Dictionary<Side, Slot> nearSlot = new Dictionary<Side, Slot>(EnumComparer<Side>.Instance);

	public Dictionary<Side, bool> wallMask = new Dictionary<Side, bool>();

	public Dictionary<Side, bool> dropWallMask = new Dictionary<Side, bool>();

	public SlotForChip slotForChip;

	public SlotGravity slotGravity;

	public bool bringDownEndSlot;

	private SpriteRenderer SlotSpriteRenderer;

	public static int jellyModeTileTestLogicIndex;

	private readonly float delayPaintJellyTime = 1.4f;

	public float jellyEffectSpeed = 1f;

	public static bool isFirstJellyComplete;

	private bool isInfinityPauseGravity;

	public DropDirection DropDir
	{
		get
		{
			return dropDirection;
		}
		private set
		{
			dropDirection = value;
		}
	}

	public bool DropLock
	{
		get;
		private set;
	}

	public Slot this[Side index] => nearSlot[index];

	public void FillMilk()
	{
		if (IsFilledSweet)
		{
			return;
		}
		Vector3 zero = Vector3.zero;
		Side side = Side.Null;
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		bool flag = false;
		if (x == BoardManager.main.boardData.gateEnterX && y == BoardManager.main.boardData.gateEnterY)
		{
			Side gateDirection = BoardManager.main.GetGateDirection(x, y);
			if (!fillingSide.Contains(gateDirection))
			{
				switch (gateDirection)
				{
				case Side.Top:
					fillingSide.Add(gateDirection);
					list.Add(180);
					break;
				case Side.Bottom:
					fillingSide.Add(gateDirection);
					list.Add(0);
					break;
				case Side.Left:
					fillingSide.Add(gateDirection);
					list.Add(-90);
					break;
				case Side.Right:
					fillingSide.Add(gateDirection);
					list.Add(90);
					break;
				}
			}
		}
		for (int i = 0; i < Utils.allSides.Length; i++)
		{
			side = Utils.allSides[i];
			if (nearSlot[side] == null || fillingSide.Contains(side) || !nearSlot[side].IsFilledSweet)
			{
				continue;
			}
			switch (side)
			{
			case Side.Top:
				fillingSide.Add(side);
				list.Add(180);
				break;
			case Side.Bottom:
				fillingSide.Add(side);
				list.Add(0);
				break;
			case Side.Left:
				fillingSide.Add(side);
				list.Add(-90);
				break;
			case Side.Right:
				fillingSide.Add(side);
				list.Add(90);
				break;
			case Side.TopLeft:
				if (list.Count <= 0 && (((bool)nearSlot[Side.Top] && nearSlot[Side.Top].isFillSweet) || ((bool)nearSlot[Side.Left] && nearSlot[Side.Left].isFillSweet)))
				{
					flag = true;
					fillingSide.Add(side);
					list2.Add(180);
				}
				break;
			case Side.TopRight:
				if (list.Count <= 0 && (((bool)nearSlot[Side.Top] && nearSlot[Side.Top].isFillSweet) || ((bool)nearSlot[Side.Right] && nearSlot[Side.Right].isFillSweet)))
				{
					flag = true;
					fillingSide.Add(side);
					list2.Add(90);
				}
				break;
			case Side.BottomLeft:
				if (list.Count <= 0 && (((bool)nearSlot[Side.Bottom] && nearSlot[Side.Bottom].isFillSweet) || ((bool)nearSlot[Side.Left] && nearSlot[Side.Left].isFillSweet)))
				{
					flag = true;
					fillingSide.Add(side);
					list2.Add(-90);
				}
				break;
			case Side.BottomRight:
				if (list.Count <= 0 && (((bool)nearSlot[Side.Bottom] && nearSlot[Side.Bottom].isFillSweet) || ((bool)nearSlot[Side.Right] && nearSlot[Side.Right].isFillSweet)))
				{
					flag = true;
					fillingSide.Add(side);
					list2.Add(0);
				}
				break;
			}
		}
		MilkLayer.enabled = false;
		MilkLayer.gameObject.transform.GetChild(0).gameObject.SetActive(value: false);
		if (flag)
		{
			for (int j = 0; j < list2.Count; j++)
			{
				zero.z = list2[j];
				GameObject spawnEffectObject = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.FillingSweetMilkDiagonal);
				if ((bool)spawnEffectObject)
				{
					spawnEffectObject.transform.position = base.transform.position;
					spawnEffectObject.transform.localRotation = Quaternion.Euler(zero);
					PoolManager.PoolGameEffect.Despawn(spawnEffectObject.transform, fillingMilkEffectTime);
					GameMain.main.fillingMilkEffectCount++;
					Invoke("InvokeBubbleEffectDespawn", fillingMilkEffectTime);
				}
			}
		}
		else
		{
			for (int k = 0; k < list.Count; k++)
			{
				zero.z = list[k];
				GameObject spawnEffectObject2 = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.FillingSweetMilk);
				if ((bool)spawnEffectObject2)
				{
					spawnEffectObject2.transform.position = base.transform.position;
					spawnEffectObject2.transform.localRotation = Quaternion.Euler(zero);
					PoolManager.PoolGameEffect.Despawn(spawnEffectObject2.transform, fillingMilkEffectTime);
					GameMain.main.fillingMilkEffectCount++;
					Invoke("InvokeBubbleEffectDespawn", fillingMilkEffectTime);
				}
			}
		}
		StartCoroutine(FillingMilk());
		StartCoroutine(waitNextFrameMilkFill());
	}

	private void InvokeBubbleEffectDespawn()
	{
		GameMain.main.fillingMilkEffectCount = Mathf.Max(0, GameMain.main.fillingMilkEffectCount - 1);
	}

	private IEnumerator waitNextFrameMilkFill()
	{
		yield return new WaitForEndOfFrame();
		isFillSweet = true;
	}

	private IEnumerator FillingMilk()
	{
		fillMilkFrameCount = 0;
		IsFilledSweet = false;
		yield return new WaitForSeconds(0.12f);
		fillMilkFrameCount++;
		yield return new WaitForSeconds(0.12f);
		MilkLayer.sprite = GameMain.main.SpriteFilledMilk;
		MilkLayer.enabled = true;
		MilkLayer.gameObject.transform.GetChild(0).gameObject.SetActive(value: true);
		IsFilledSweet = true;
		if (BoardManager.main.boardData.gateExitX == x && BoardManager.main.boardData.gateExitY == y)
		{
			BoardManager.main.StartEffectSweetRoadGateFillMilk(isEnterGate: false);
		}
	}

	public bool IsEnableContinuousDiagonalFill()
	{
		if (fillMilkFrameCount >= 1)
		{
			return true;
		}
		return false;
	}

	private void Awake()
	{
		slotForChip = GetComponent<SlotForChip>();
		slotGravity = GetComponent<SlotGravity>();
		SlotSpriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void Dig()
	{
		MoveGrid();
		SetPositionByGrid();
	}

	private void SetPositionByGrid()
	{
		base.transform.position = BoardManager.main.GetSlotPosition(x, y);
	}

	private void MoveGrid()
	{
		y++;
		if (y <= 6)
		{
		}
	}

	public void Initialize()
	{
		if (!initialized)
		{
			Side[] allSides = Utils.allSides;
			foreach (Side side in allSides)
			{
				nearSlot.Add(side, BoardManager.main.GetNearSlot(x, y, side));
			}
			Side[] straightSides = Utils.straightSides;
			foreach (Side key in straightSides)
			{
				wallMask.Add(key, value: false);
			}
			Side[] straightSides2 = Utils.straightSides;
			foreach (Side key2 in straightSides2)
			{
				dropWallMask.Add(key2, value: false);
			}
			if (MapData.main.target == GoalTarget.SweetRoad || MapData.main.target == GoalTarget.CollectCracker)
			{
				GameObject item = ContentAssistant.main.GetItem("MilkLayer");
				MilkLayer = item.GetComponent<SpriteRenderer>();
				item.transform.parent = base.transform;
				item.transform.localPosition = Vector3.zero;
				MilkLayer.enabled = false;
				MilkLayer.gameObject.transform.GetChild(0).gameObject.SetActive(value: false);
			}
			isPausingGravity = false;
			initialized = true;
		}
	}

	public void GenerateEmptyLayer()
	{
		if (jellyModeTileTestLogicIndex != 0)
		{
			GameObject gameObject = null;
			if (jellyModeTileTestLogicIndex == 1)
			{
				gameObject = ContentAssistant.main.GetItem("JellyLayer");
				EmptyLayer = gameObject.GetComponent<SpriteRenderer>();
			}
			else if (jellyModeTileTestLogicIndex == 2)
			{
				gameObject = ContentAssistant.main.GetItem("Obstacle_D_Blank");
				EmptyLayer = gameObject.GetComponentInChildren<SpriteRenderer>();
			}
			else if (jellyModeTileTestLogicIndex == 3)
			{
				gameObject = ContentAssistant.main.GetItem("Obstacle_pastrybag_tray");
				gameObject.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
				EmptyLayer = gameObject.GetComponentInChildren<SpriteRenderer>();
				EmptyLayer.sortingOrder = -150;
			}
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			EmptyLayer.enabled = false;
		}
	}

	public void EnableEmptyLayer()
	{
		EmptyLayer.enabled = true;
	}

	public void DisableEmptyLayer()
	{
		EmptyLayer.enabled = false;
	}

	public void GenerateJellyLayer()
	{
		GameObject item = ContentAssistant.main.GetItem("JellyLayer");
		JellyLayer = item.GetComponent<SpriteRenderer>();
		item.transform.parent = base.transform;
		item.transform.localPosition = Vector3.zero;
		JellyLayer.enabled = false;
	}

	public void PaintJelly(bool decreaseCollect = true)
	{
		if (isRailRoad || IsPaintedJelly || MapData.main.target != GoalTarget.Jelly)
		{
			return;
		}
		IsPaintedJelly = true;
		if (decreaseCollect)
		{
			GameMain.main.DecreaseCollect(CollectBlockType.Jelly);
		}
		GameObject spawnEffectObject = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.JellyLayerMake);
		if ((bool)spawnEffectObject)
		{
			SpriteRenderer[] componentsInChildren = spawnEffectObject.GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer spriteRenderer in componentsInChildren)
			{
				spriteRenderer.sortingLayerID = SortingLayer.NameToID("Default");
			}
			spawnEffectObject.transform.parent = base.transform;
			spawnEffectObject.transform.localPosition = Vector3.zero;
			spawnEffectObject.transform.localScale = Vector3.one;
			PoolManager.PoolGameEffect.Despawn(spawnEffectObject.transform, delayPaintJellyTime);
		}
		SoundSFX.PlayCap(SFXIndex.JellyMake);
		StartCoroutine(delayPaintJelly());
	}

	private IEnumerator delayPaintJelly()
	{
		yield return new WaitForSeconds(delayPaintJellyTime);
		JellyLayer.enabled = true;
	}

	public void PaintJellyFirst()
	{
		if (!isRailRoad && !IsPaintedJelly && MapData.main.target == GoalTarget.Jelly)
		{
			IsPaintedJelly = true;
			StartCoroutine(delayPaintJellyFirst());
		}
	}

	private IEnumerator delayPaintJellyFirst()
	{
		yield return new WaitForSeconds(1f);
		SoundSFX.PlayCap(SFXIndex.JellyMake);
		//Camera.main.depth = 1f;
		GameObject effect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.JamShootPurple);
		effect.transform.parent = base.transform;
		effect.transform.position = Match3GameUI.Instance.GetCollectObjectGameCameraPosition(CollectBlockType.Jelly);
		effect.transform.DOScale(1f, 0.74f).SetEase(Ease.OutBack);
		Vector3 startPos = effect.transform.position;
		Vector3 targetPos = BoardManager.main.GetSlotPosition(x, y);
		float elapse_time = 0f;
		Vector3 startScale = new Vector3(1f, 1f, 1f);
		Vector3 pointScale = new Vector3(3f, 3f, 3f);
		Vector3 targetScale = new Vector3(1f, 1f, 1f);
		while (elapse_time < 1f)
		{
			Vector3 position = base.transform.position;
			float num = (position.x + targetPos.x) * 0.5f;
			float num2 = targetPos.y;
			Vector3 position2 = base.transform.position;
			float num3;
			if (num2 <= position2.y)
			{
				Vector3 position3 = base.transform.position;
				num3 = position3.y + 300f;
			}
			else
			{
				num3 = targetPos.y + 300f;
			}
			Vector3 pointPos = new Vector3(num, num3, 0f);
			effect.transform.position = Utils.Bezier(elapse_time, startPos, pointPos, targetPos);
			effect.transform.localScale = Utils.Bezier(elapse_time, startScale, pointScale, targetScale);
			elapse_time += Time.deltaTime * jellyEffectSpeed;
			yield return null;
		}
		PoolManager.PoolGameEffect.Despawn(effect.transform);
		GameObject objSpreadEffect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.JamSpreadPurple);
		objSpreadEffect.transform.position = targetPos;
		PoolManager.PoolGameEffect.Despawn(objSpreadEffect.transform, 1.9f);
		SoundSFX.PlayCap(SFXIndex.JamDrop);
		JellyLayer.enabled = true;
		//Camera.main.depth = 0f;
		isFirstJellyComplete = true;
	}

	public void FillMilkTile()
	{
		MilkLayer.enabled = true;
		MilkLayer.gameObject.transform.GetChild(0).gameObject.SetActive(value: true);
	}

	public void SetChip(Chip c)
	{
		if ((bool)slotForChip)
		{
			if (c == null)
			{
				BoardManager.main.boardData.chips[x, y] = -1;
				slotForChip.chip = null;
			}
			else
			{
				BoardManager.main.boardData.chips[x, y] = c.id;
				slotForChip.SetChip(c);
			}
		}
	}

	public void GenerateRockCandy(int level)
	{
		GameObject item = ContentAssistant.main.GetItem("RockCandy");
		item.transform.parent = base.transform;
		item.transform.localPosition = Vector3.zero;
		rockCandy = item.GetComponent<RockCandy>();
		rockCandy.Level = level;
	}

	public void RockCandyCrush()
	{
		if ((bool)rockCandy && rockCandy.Crush())
		{
			GameMain.main.CheckRescueGingerMan(x, y);
		}
	}

	public void RockCandyFill()
	{
	}

	public void RockCandyRemove()
	{
		if ((bool)rockCandy)
		{
			rockCandy.Remove();
		}
	}

	public bool IsExistRockCandy()
	{
		if ((bool)rockCandy && rockCandy.Level > 0)
		{
			return true;
		}
		return false;
	}

	public void SetSlotBackground(int index)
	{
		SlotSpriteRenderer.sprite = BoardManager.main.SpritesSlotBackground[index];
	}

	public void SetRailImage(int index)
	{
	}

	public Chip GetChip()
	{
		if ((bool)slotForChip)
		{
			return slotForChip.GetChip();
		}
		return null;
	}

	public BlockInterface GetBlock()
	{
		return block;
	}

	public void SetBlock(BlockInterface b)
	{
		if ((bool)b)
		{
			b.transform.parent = base.transform;
			BoardManager.main.boardData.blocks[x, y] = b.blockType;
		}
		else
		{
			BoardManager.main.boardData.blocks[x, y] = IBlockType.None;
		}
		block = b;
	}

	public void SetDropDirection(DropDirection dir)
	{
		DropDir = dir;
	}

	public Slot GetNearSlotDropHead()
	{
		Side[] straightSides = Utils.straightSides;
		foreach (Side side in straightSides)
		{
			DropDirection dropDirection = DropDirection.Down;
			switch (side)
			{
			case Side.Top:
				dropDirection = DropDirection.Down;
				break;
			case Side.Bottom:
				dropDirection = DropDirection.Up;
				break;
			case Side.Left:
				dropDirection = DropDirection.Right;
				break;
			case Side.Right:
				dropDirection = DropDirection.Left;
				break;
			}
			if ((bool)nearSlot[side] && nearSlot[side].dropDirection == dropDirection)
			{
				return nearSlot[side];
			}
		}
		return null;
	}

	public Slot GetNearSlotDropUp()
	{
		switch (DropDir)
		{
		case DropDirection.Down:
			return nearSlot[Side.Top];
		case DropDirection.Up:
			return nearSlot[Side.Bottom];
		case DropDirection.Right:
			return nearSlot[Side.Left];
		case DropDirection.Left:
			return nearSlot[Side.Right];
		default:
			return null;
		}
	}

	public Slot GetNearSlotDropDown()
	{
		switch (DropDir)
		{
		case DropDirection.Down:
			return nearSlot[Side.Bottom];
		case DropDirection.Up:
			return nearSlot[Side.Top];
		case DropDirection.Right:
			return nearSlot[Side.Right];
		case DropDirection.Left:
			return nearSlot[Side.Left];
		default:
			return null;
		}
	}

	public Slot GetNearSlotDropLeft()
	{
		switch (DropDir)
		{
		case DropDirection.Down:
			return nearSlot[Side.Left];
		case DropDirection.Up:
			return nearSlot[Side.Right];
		case DropDirection.Right:
			return nearSlot[Side.Bottom];
		case DropDirection.Left:
			return nearSlot[Side.Top];
		default:
			return null;
		}
	}

	public Slot GetNearSlotDropRight()
	{
		switch (DropDir)
		{
		case DropDirection.Down:
			return nearSlot[Side.Right];
		case DropDirection.Up:
			return nearSlot[Side.Left];
		case DropDirection.Right:
			return nearSlot[Side.Top];
		case DropDirection.Left:
			return nearSlot[Side.Bottom];
		default:
			return null;
		}
	}

	public Slot GetNearSlotDropDownLeft()
	{
		switch (DropDir)
		{
		case DropDirection.Down:
			return nearSlot[Side.BottomLeft];
		case DropDirection.Up:
			return nearSlot[Side.TopRight];
		case DropDirection.Right:
			return nearSlot[Side.BottomRight];
		case DropDirection.Left:
			return nearSlot[Side.TopLeft];
		default:
			return null;
		}
	}

	public Slot GetNearSlotDropDownRight()
	{
		switch (DropDir)
		{
		case DropDirection.Down:
			return nearSlot[Side.BottomRight];
		case DropDirection.Up:
			return nearSlot[Side.TopLeft];
		case DropDirection.Right:
			return nearSlot[Side.TopRight];
		case DropDirection.Left:
			return nearSlot[Side.BottomLeft];
		default:
			return null;
		}
	}

	public Side GetSideSlotHead()
	{
		switch (DropDir)
		{
		case DropDirection.Down:
			return Side.Top;
		case DropDirection.Up:
			return Side.Bottom;
		case DropDirection.Right:
			return Side.Left;
		case DropDirection.Left:
			return Side.Right;
		default:
			return Side.Null;
		}
	}

	public void SetDropLock(bool _dropLock)
	{
		DropLock = _dropLock;
	}

	public bool IsExistDropWall()
	{
		switch (DropDir)
		{
		case DropDirection.Down:
			return dropWallMask[Side.Bottom];
		case DropDirection.Up:
			return dropWallMask[Side.Top];
		case DropDirection.Right:
			return dropWallMask[Side.Right];
		case DropDirection.Left:
			return dropWallMask[Side.Left];
		default:
			return false;
		}
	}

	public bool GetShadow()
	{
		if ((bool)slotGravity)
		{
			return slotGravity.shadow;
		}
		return false;
	}

	public bool GetChipShadow()
	{
		Slot nearSlotDropHead = GetNearSlotDropHead();
		Slot slot = nearSlotDropHead;
		while (true)
		{
			if (!nearSlotDropHead)
			{
				return false;
			}
			if (!nearSlotDropHead.gravity || nearSlotDropHead.isPausingGravity)
			{
				return false;
			}
			if ((bool)nearSlotDropHead.GetChip())
			{
				break;
			}
			nearSlotDropHead = nearSlotDropHead.GetNearSlotDropHead();
			if (slot == nearSlotDropHead)
			{
				return true;
			}
		}
		return true;
	}

	public void SetWall(Side side)
	{
		wallMask[side] = true;
		if ((bool)nearSlot[side])
		{
			nearSlot[side].nearSlot[Utils.MirrorSide(side)] = null;
		}
		nearSlot[side] = null;
	}

	public void SetDropWall(Side side)
	{
		dropWallMask[side] = true;
	}

	public void PauseGravity(float time)
	{
		StopCoroutine(WaitPauseGravity(0f));
		isPausingGravity = true;
		if (time > 0f)
		{
			isInfinityPauseGravity = false;
			StartCoroutine(WaitPauseGravity(time));
		}
		else
		{
			isInfinityPauseGravity = true;
		}
	}

	public void ResumeGravity()
	{
		StopCoroutine(WaitPauseGravity(0f));
		isPausingGravity = false;
		isInfinityPauseGravity = false;
	}

	private IEnumerator WaitPauseGravity(float time)
	{
		if (time > 0f)
		{
			yield return new WaitForSeconds(time);
		}
		if (!isInfinityPauseGravity)
		{
			isPausingGravity = false;
		}
	}
}
