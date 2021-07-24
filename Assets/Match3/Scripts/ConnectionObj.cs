using UnityEngine;

public class ConnectionObj : BlockInterface
{
	public override bool EnableBoosterHammer => false;

	public override bool EnableBoosterCandyCrane => false;

	public override bool EnableBoosterCandyPack => false;

	public void Rescue()
	{
		if (!destroying)
		{
			destroying = true;
			base.gameObject.layer = LayerMask.NameToLayer("GameEffect");
			GameMain.main.ThrowCollectItem(base.gameObject, CollectBlockType.SweetRoadConnect, 0.1f);
			Crush();
			slot.gravity = true;
			slot.SetBlock(null);
			SlotGravity.Reshading();
		}
	}

	public override void Initialize()
	{
		slot.gravity = false;
		slot.RockCandyRemove();
	}

	public override void BlockCrush(int fromCrushId = -1, int subId = -1)
	{
	}

	public override bool CanBeCrushedByNearSlot()
	{
		return false;
	}
}
