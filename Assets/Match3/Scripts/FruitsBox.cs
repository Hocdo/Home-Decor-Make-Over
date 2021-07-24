using PathologicalGames;
using UnityEngine;

public class FruitsBox : BlockInterface
{
	public override bool EnableBoosterHammer => false;

	public override bool EnableBoosterCandyCrane => false;

	public override bool EnableBoosterCandyPack => false;

	public override void Initialize()
	{
		slot.gravity = false;
		SlotGravity.Reshading();
	}

	public override void BlockCrush(int fromCrushId = -1, int subId = -1)
	{
	}

	public override bool CanBeCrushedByNearSlot()
	{
		return false;
	}

	public void RemoveBlock()
	{
		GameObject item = ContentAssistant.main.GetItem("BlockCrush");
		item.transform.position = base.transform.position;
		slot.gravity = true;
		SlotGravity.Reshading();
		PoolManager.PoolGameBlocks.Despawn(base.transform);
	}
}
