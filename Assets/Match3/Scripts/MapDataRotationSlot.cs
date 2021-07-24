using PathologicalGames;
using System.Collections.Generic;
using UnityEngine;

public class MapDataRotationSlot
{
	public int centerX;

	public int centerY;

	public bool isClockwork = true;

	public bool isGrid;

	private bool isTrigger;

	public List<GameObject> listEffects = new List<GameObject>();

	public int size;

	public MapDataRotationSlot(bool isGrid, int x, int y, int w, bool isClockwork)
	{
		this.isGrid = isGrid;
		centerX = x;
		centerY = y;
		size = w;
		this.isClockwork = isClockwork;
	}

	public void InitForGamePlay()
	{
		isTrigger = false;
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
	}

	public bool IsTriggerOnGamePlay()
	{
		return isTrigger;
	}

	public void CheckTriggerCrushSlot(int slotX, int slotY)
	{
		int num = centerX - size / 2;
		int num2 = centerY + size / 2 - (isGrid ? 1 : 0);
		if (slotX >= num && slotX < num + size && slotY <= num2 && slotY > num2 - size)
		{
			isTrigger = true;
		}
	}

	public string GetJsonFormat()
	{
		return string.Format("{0}{1}:{2}{3}", centerX, centerY, size, (!isClockwork) ? "R" : "C");
	}
}
