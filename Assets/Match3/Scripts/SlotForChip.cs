using UnityEngine;

[RequireComponent(typeof(Slot))]
public class SlotForChip : MonoBehaviour
{
	public Chip chip;

	public Slot slot;

	public Slot this[Side index] => slot.nearSlot[index];

	private void Awake()
	{
		slot = GetComponent<Slot>();
	}

	public Chip GetChip()
	{
		return chip;
	}

	public void SetChip(Chip c)
	{
		if (c == null)
		{
			return;
		}
		if ((bool)chip)
		{
			chip.parentSlot = null;
		}
		chip = c;
		if ((bool)chip)
		{
			if ((bool)chip.transform.parent)
			{
				chip.transform.parent = base.transform;
			}
			if ((bool)chip.parentSlot)
			{
				chip.parentSlot.chip = null;
				BoardManager.main.boardData.chips[chip.parentSlot.slot.x, chip.parentSlot.slot.y] = -1;
			}
			chip.parentSlot = this;
			BoardManager.main.boardData.chips[slot.x, slot.y] = chip.id;
			if (c.chipType == ChipType.BringDown)
			{
				BoardManager.main.boardData.chips[slot.x, slot.y] = 2;
			}
		}
	}

	public GameMain.Solution MatchAnaliz(bool cpuAI = false)
	{
		if (!GetChip())
		{
			return null;
		}
		if (!GetChip().IsMatchable())
		{
			return null;
		}
		if (GetChip().IsMuteki)
		{
			return null;
		}
		int x = this.slot.x;
		int y = this.slot.y;
		int width = BoardManager.main.boardData.width;
		int height = BoardManager.main.boardData.height;
		GameMain.Solution solution = null;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		for (x = this.slot.x + 1; x < width; x++)
		{
			Slot slot = BoardManager.main.GetSlot(x, this.slot.y);
			if (!slot || !slot.canBeCrush || !slot.GetChip() || slot.GetChip().id != chip.id || !slot.GetChip().IsMatchable())
			{
				break;
			}
			num6 += slot.GetChip().GetPotential();
			num2++;
		}
		for (x = this.slot.x - 1; x >= 0; x--)
		{
			Slot slot = BoardManager.main.GetSlot(x, this.slot.y);
			if (!slot || !slot.canBeCrush || !slot.GetChip() || slot.GetChip().id != chip.id || !slot.GetChip().IsMatchable())
			{
				break;
			}
			num6 += slot.GetChip().GetPotential();
			num4++;
		}
		for (y = this.slot.y + 1; y < height; y++)
		{
			Slot slot = BoardManager.main.GetSlot(this.slot.x, y);
			if (!slot || !slot.canBeCrush || !slot.GetChip() || slot.GetChip().id != chip.id || !slot.GetChip().IsMatchable())
			{
				break;
			}
			num5 += slot.GetChip().GetPotential();
			num++;
		}
		for (y = this.slot.y - 1; y >= 0; y--)
		{
			Slot slot = BoardManager.main.GetSlot(this.slot.x, y);
			if (!slot || !slot.canBeCrush || !slot.GetChip() || slot.GetChip().id != chip.id || !slot.GetChip().IsMatchable())
			{
				break;
			}
			num5 += slot.GetChip().GetPotential();
			num3++;
		}
		if (num2 + num4 >= 2 || num + num3 >= 2)
		{
			if (solution == null)
			{
				solution = new GameMain.Solution();
			}
			solution.h = (num2 + num4 >= 2);
			solution.v = (num + num3 >= 2);
			solution.x = this.slot.x;
			solution.y = this.slot.y;
			solution.count = 1;
			solution.count += (solution.v ? (num + num3) : 0);
			solution.count += (solution.h ? (num2 + num4) : 0);
			solution.color = chip.id;
			solution.posH = num2;
			solution.negH = num4;
			solution.posV = num;
			solution.negV = num3;
			if (solution.v)
			{
				solution.potential += num5;
			}
			if (solution.h)
			{
				solution.potential += num6;
			}
			solution.potential += chip.GetPotential();
			if (num2 + num4 >= 4)
			{
				solution.itemBlock51Count++;
			}
			else if (num2 + num4 >= 3)
			{
				solution.itemBlock41Count++;
			}
			if (num + num3 >= 4)
			{
				solution.itemBlock51Count++;
			}
			else if (num + num3 >= 3)
			{
				solution.itemBlock41Count++;
			}
			if (num2 + num4 >= 2 && num + num3 >= 2)
			{
				solution.itemBlock33Count++;
			}
			if (cpuAI)
			{
				if (solution.h)
				{
					for (x = this.slot.x - num4; x <= this.slot.x + num2; x++)
					{
						Slot slot = BoardManager.main.GetSlot(x, this.slot.y);
						if (!slot.IsExistRockCandy())
						{
							solution.aiBackStoneNullCount++;
							if (BoardManager.main.boardData.rescueGinerManSize[x, this.slot.y] != 0)
							{
								solution.aiBackStoneIncludeRescueGingerManCount++;
							}
						}
						else
						{
							solution.aiBackStoneCount++;
						}
					}
				}
				if (solution.v)
				{
					for (y = this.slot.y - num3; y <= this.slot.y + num; y++)
					{
						Slot slot = BoardManager.main.GetSlot(this.slot.x, y);
						if (!slot.IsExistRockCandy())
						{
							solution.aiBackStoneNullCount++;
							if (BoardManager.main.boardData.rescueGinerManSize[this.slot.x, y] != 0)
							{
								solution.aiBackStoneIncludeRescueGingerManCount++;
							}
						}
						else
						{
							solution.aiBackStoneCount++;
						}
					}
				}
			}
		}
		return solution;
	}
}
