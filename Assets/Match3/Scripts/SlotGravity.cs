using UnityEngine;

[RequireComponent(typeof(Slot))]
[RequireComponent(typeof(SlotForChip))]
public class SlotGravity : MonoBehaviour
{
	public Chip chip;

	public bool shadow;

	public Slot slot;

	public SlotForChip slotForChip;

	private void Awake()
	{
		slot = GetComponent<Slot>();
		slotForChip = GetComponent<SlotForChip>();
	}

	private void Start()
	{
		Shading();
	}

	public static void Reshading()
	{
		if (BoardManager.main == null || BoardManager.main.slotGroup == null)
		{
			return;
		}
		SlotGravity[] componentsInChildren = BoardManager.main.slotGroup.GetComponentsInChildren<SlotGravity>();
		SlotGravity[] array = componentsInChildren;
		foreach (SlotGravity slotGravity in array)
		{
			if (slotGravity != null)
			{
				slotGravity.Shading();
			}
		}
	}

	public void Shading()
	{
		for (int i = 0; i < BoardManager.width; i++)
		{
			for (int j = 0; j < BoardManager.height; j++)
			{
				int key = i * MapData.MaxWidth + j;
				if (!BoardManager.main.slots.ContainsKey(key))
				{
					shadow = true;
					return;
				}
				Slot slot = BoardManager.main.slots[key];
				if ((bool)slot && !slot.GetBlock() && slot.teleportTarget)
				{
					break;
				}
				if (!slot || !slot.gravity || !slot.GetNearSlotDropUp() || slot.isPausingGravity)
				{
					shadow = true;
					return;
				}
			}
		}
		shadow = false;
	}

	private void Update()
	{
		GravityReaction();
	}

	public bool GravityReaction()
	{
		if (!GameMain.main.CanIGravity())
		{
			return false;
		}
		if (slotForChip == null)
		{
			return false;
		}
		chip = slotForChip.GetChip();
		if (!chip)
		{
			return false;
		}
		if (!chip.canMove)
		{
			return false;
		}
		if (base.transform.position != chip.transform.position)
		{
			return false;
		}
		Slot nearSlotDropDown = slot.GetNearSlotDropDown();
		if ((bool)nearSlotDropDown && nearSlotDropDown.gravity && !nearSlotDropDown.GetChip() && !nearSlotDropDown.isPausingGravity)
		{
			nearSlotDropDown.SetChip(chip);
			GravityReaction();
			return true;
		}
		if (!slot.DropLock)
		{
			if (UnityEngine.Random.value > 0.5f)
			{
				SlideLeft();
				SlideRight();
			}
			else
			{
				SlideRight();
				SlideLeft();
			}
		}
		return false;
	}

	private void SlideLeft()
	{
		Slot nearSlotDropDownLeft = slot.GetNearSlotDropDownLeft();
		if ((bool)nearSlotDropDownLeft && nearSlotDropDownLeft.gravity && !nearSlotDropDownLeft.isPausingGravity && !nearSlotDropDownLeft.GetChip() && nearSlotDropDownLeft.GetShadow() && !nearSlotDropDownLeft.GetChipShadow())
		{
			nearSlotDropDownLeft.SetChip(chip);
		}
	}

	private void SlideRight()
	{
		Slot nearSlotDropDownRight = slot.GetNearSlotDropDownRight();
		if ((bool)nearSlotDropDownRight && nearSlotDropDownRight.gravity && !nearSlotDropDownRight.isPausingGravity && !nearSlotDropDownRight.GetChip() && nearSlotDropDownRight.GetShadow() && !nearSlotDropDownRight.GetChipShadow())
		{
			nearSlotDropDownRight.SetChip(chip);
		}
	}
}
