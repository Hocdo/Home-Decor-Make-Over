using UnityEngine;

public class FruitsBoxDragEnable : MonoBehaviour
{
	public enum PutType
	{
		None,
		FruitsBox,
		Chip
	}

	public Slot parentSlot;

	public PutType putType;

	private void Start()
	{
		FruitsBox component = GetComponent<FruitsBox>();
		if (component != null)
		{
			putType = PutType.FruitsBox;
			return;
		}
		Chip component2 = GetComponent<Chip>();
		if (component2 != null)
		{
			putType = PutType.Chip;
		}
	}

	public void SetParentSlot()
	{
		if (putType == PutType.FruitsBox)
		{
			parentSlot = GetComponent<FruitsBox>().slot;
		}
		else if (putType == PutType.Chip)
		{
			parentSlot = GetComponent<Chip>().parentSlot.slot;
		}
	}
}
