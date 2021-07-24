using UnityEngine;

public class MilkCarton : BlockInterface
{
	public override bool EnableBoosterHammer => true;

	public override bool EnableBoosterCandyCrane => false;

	public override bool EnableBoosterCandyPack => true;

	public override void Initialize()
	{
		slot.gravity = false;
	}

	public override void BlockCrush(int fromCrushId = -1, int subId = -1)
	{
		if (!destroying)
		{
			int height = ++GameMain.main.curOreoMilkHeight;
			BoardManager.main.GenerateMilkTileRow(height);
			if (GameMain.main.curOreoMilkHeight == GameMain.main.targetOreoMilkHeight)
			{
				destroying = true;
				GameMain.main.canMakeOreoCracker = true;
				foreach (GameObject item in BoardManager.main.listCowBell)
				{
					UnityEngine.Object.Destroy(item);
				}
				BoardManager.main.listCowBell.Clear();
				foreach (MilkCarton item2 in BoardManager.main.listMilkCarton)
				{
					item2.DestroyMilkCarton();
				}
				BoardManager.main.listMilkCarton.Clear();
			}
		}
	}

	public override bool CanBeCrushedByNearSlot()
	{
		return false;
	}

	public void DestroyMilkCarton()
	{
		Crush();
		slot.gravity = true;
		slot.SetBlock(null);
		SlotGravity.Reshading();
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
