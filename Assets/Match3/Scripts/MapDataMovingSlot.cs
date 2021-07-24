using PathologicalGames;
using System.Collections.Generic;
using UnityEngine;

public class MapDataMovingSlot
{
	private bool isTrigger;

	public List<GameObject> listEffects = new List<GameObject>();

	public int startX;

	public int startY;

	public int targetX;

	public int targetY;

	public int width;

	public int height;

	public bool isMoveReverse
	{
		get;
		private set;
	}

	public MapDataMovingSlot(int sx, int sy, int tx, int ty, int w, int h)
	{
		startX = sx;
		startY = sy;
		targetX = tx;
		targetY = ty;
		width = w;
		height = h;
	}

	public void InitForGamePlay()
	{
		isTrigger = false;
		isMoveReverse = false;
		ClearEffects();
	}

	public void ClearEffects()
	{
		foreach (GameObject listEffect in listEffects)
		{
			if ((bool)listEffect)
			{
				PoolManager.PoolGamePlaying.Despawn(listEffect.transform);
			}
		}
		listEffects.Clear();
	}

	public void TurnStartGamePlay()
	{
		isTrigger = false;
	}

	public void MoveEndGamePlay()
	{
		isMoveReverse = !isMoveReverse;
	}

	public bool IsTriggerOnGamePlay()
	{
		return isTrigger;
	}

	public void CheckTriggerCrushSlot(int slotX, int slotY)
	{
		if ((!isMoveReverse && slotX >= startX && slotX < startX + width && slotY <= startY && slotY > startY - height) || (isMoveReverse && slotX >= targetX && slotX < targetX + width && slotY <= targetY && slotY > targetY - height))
		{
			isTrigger = true;
		}
	}

	public string GetJsonFormat()
	{
		return $"{startX}{startY}{targetX}{targetY}:{width}{height}";
	}
}
