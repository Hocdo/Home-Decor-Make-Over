using UnityEngine;

[RequireComponent(typeof(Slot))]
[RequireComponent(typeof(SlotForChip))]
public class SlotGenerator : MonoBehaviour
{
	private int generatorDropIndex;

	public Slot slot;

	public SlotForChip slotForChip;

	private Vector3 vCreateOffset = Vector3.zero;

	private void Awake()
	{
		slot = GetComponent<Slot>();
		slot.generator = true;
		slotForChip = GetComponent<SlotForChip>();
		switch (slot.DropDir)
		{
		case DropDirection.Down:
			vCreateOffset = new Vector3(0f, BoardManager.slotoffset, 0f);
			break;
		case DropDirection.Up:
			vCreateOffset = new Vector3(0f, 0f - BoardManager.slotoffset, 0f);
			break;
		case DropDirection.Right:
			vCreateOffset = new Vector3(0f - BoardManager.slotoffset, 0f, 0f);
			break;
		case DropDirection.Left:
			vCreateOffset = new Vector3(BoardManager.slotoffset, 0f, 0f);
			break;
		}
	}

	private void Update()
	{
		if (GameMain.main == null || BoardManager.main == null || MapData.main == null || slot == null || slotForChip == null)
		{
			return;
		}
		GameMain.main.isJustGenerateChip = false;
		if (!GameMain.main.CanIGravity() || (bool)slotForChip.GetChip() || (bool)slot.GetBlock() || slot.isPausingGravity)
		{
			return;
		}
		if (GameMain.main.createdBringDownCount - (GameMain.main.targetBringDownCount - GameMain.main.targetBringDownRemainCount) < MapData.main.keepShowBringDownObjectCount && GameMain.main.createdBringDownCount < GameMain.main.targetBringDownCount && BoardManager.main.bringDownGenerator[slot.x, slot.y])
		{
			if (MapData.main.target == GoalTarget.BringDown)
			{
				BoardManager.main.GetBringDownChip(slot.x, slot.y, base.transform.position + vCreateOffset);
				return;
			}
			if (MapData.main.target == GoalTarget.CollectCracker && GameMain.main.canMakeOreoCracker)
			{
				BoardManager.main.GetOreoCrackerChip(slot.x, slot.y, base.transform.position + vCreateOffset);
				return;
			}
		}
		Chip chip = null;
		BoardPosition key = new BoardPosition(slot.x, slot.y);
		if (BoardManager.main.dicGeneratorSpecialDrop.ContainsKey(key))
		{
			if (BoardManager.main.dicGeneratorSpecialDrop[key].totalProb == 0 || GameMain.main.isGameResult)
			{
				chip = BoardManager.main.GetNewSimpleChip(slot.x, slot.y, base.transform.position + vCreateOffset);
				if ((bool)chip)
				{
					SimpleChip simpleChip = (SimpleChip)chip;
					simpleChip.AttachOCCToken();
				}
			}
			else
			{
				int num = UnityEngine.Random.Range(0, BoardManager.main.dicGeneratorSpecialDrop[key].totalProb);
				int num2 = 0;
				for (int i = 0; i < MapData.MaxDropProbList; i++)
				{
					num2 += BoardManager.main.dicGeneratorSpecialDrop[key].dropBlocks[i].prob;
					if (BoardManager.main.dicGeneratorSpecialDrop[key].dropBlocks[i].prob <= 0 || num >= num2)
					{
						continue;
					}
					ChipType chipType = BoardManager.main.dicGeneratorSpecialDrop[key].dropBlocks[i].chipType;
					int num3 = Mathf.Max(0, BoardManager.main.dicGeneratorSpecialDrop[key].dropBlocks[i].chipColor - 1);
					Powerup powerup = MapData.ChipTypeToPowerup(chipType);
					if (powerup == Powerup.Chameleon)
					{
						num3 = BoardManager.main.boardData.GetRandomChip();
						powerup = Powerup.None;
					}
					if (powerup == Powerup.None)
					{
						chip = BoardManager.main.GetNewSimpleChip(slot.x, slot.y, base.transform.position + vCreateOffset, num3);
						if ((bool)chip)
						{
							SimpleChip simpleChip2 = (SimpleChip)chip;
							simpleChip2.AttachOCCToken();
						}
					}
					else
					{
						Chip chip2 = BoardManager.main.AddPowerup(slot.x, slot.y, powerup, num3);
						if ((bool)chip2)
						{
							chip = chip2;
							chip.transform.position = base.transform.position + vCreateOffset;
						}
					}
					break;
				}
			}
		}
		else if (generatorDropIndex < MapData.MaxDropList)
		{
			if (BoardManager.main.dicGeneratorDrop.ContainsKey(key))
			{
				ChipType chipType2 = BoardManager.main.dicGeneratorDrop[key].dropBlocks[generatorDropIndex].chipType;
				int chipColor = BoardManager.main.dicGeneratorDrop[key].dropBlocks[generatorDropIndex].chipColor;
				chipColor = ((chipColor != 0) ? (chipColor - 1) : BoardManager.main.boardData.GetRandomChip());
				Powerup powerup2 = MapData.ChipTypeToPowerup(chipType2);
				if (powerup2 == Powerup.None)
				{
					chip = BoardManager.main.GetNewSimpleChip(slot.x, slot.y, base.transform.position + vCreateOffset, chipColor);
					if ((bool)chip)
					{
						SimpleChip simpleChip3 = (SimpleChip)chip;
						simpleChip3.AttachOCCToken();
					}
				}
				else
				{
					Chip chip3 = BoardManager.main.AddPowerup(slot.x, slot.y, powerup2, chipColor);
					if ((bool)chip3)
					{
						chip = chip3;
						chip.transform.position = base.transform.position + vCreateOffset;
					}
				}
				generatorDropIndex++;
			}
		}
		else
		{
			chip = BoardManager.main.GetNewSimpleChip(slot.x, slot.y, base.transform.position + vCreateOffset);
			if ((bool)chip)
			{
				SimpleChip simpleChip4 = (SimpleChip)chip;
				simpleChip4.AttachOCCToken();
			}
		}
		BoardManager.main.SetChipPositionNextDownChip(slot.x, slot.y);
		GameMain.main.isJustGenerateChip = true;
	}
}
