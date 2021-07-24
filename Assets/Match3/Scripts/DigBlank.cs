using UnityEngine;

public class DigBlank : BlockInterface
{
	private Animator changeAnimator;

	private int eventCountBorn;

	public override bool EnableBoosterHammer => false;

	public override bool EnableBoosterCandyCrane => false;

	public override bool EnableBoosterCandyPack => false;

	public override void Initialize()
	{
		slot.gravity = false;
		eventCountBorn = GameMain.main.eventCount;
		changeAnimator = GetComponent<Animator>();
	}

	public override void BlockCrush(int fromCrushId = -1, int subId = -1)
	{
	}

	public override bool CanBeCrushedByNearSlot()
	{
		return true;
	}
}
