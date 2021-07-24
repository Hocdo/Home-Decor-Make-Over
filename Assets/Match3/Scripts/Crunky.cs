using UnityEngine;

public class Crunky : BlockInterface
{
	private Animator changeAnimator;

	private int eventCountBorn;

	public int level = 1;

	public override bool EnableBoosterHammer => true;

	public override bool EnableBoosterCandyCrane => false;

	public override bool EnableBoosterCandyPack => true;

	public override void Initialize()
	{
		slot.gravity = false;
		eventCountBorn = GameMain.main.eventCount;
		changeAnimator = GetComponent<Animator>();
	}

	public override void BlockCrush(int fromCrushId = -1, int subId = -1)
	{
		if (eventCountBorn != GameMain.main.eventCount)
		{
			eventCountBorn = GameMain.main.eventCount;
			level--;
			SoundSFX.PlayCap(SFXIndex.CrackerRemove);
			changeAnimator.SetTrigger("SetLevelDown");
			if (level == 0 && !destroying)
			{
				destroying = true;
				GameMain.main.DecreaseCollect(CollectBlockType.Crunky, countPrevValue: true);
				Crush();
				slot.gravity = true;
				slot.SetBlock(null);
				SlotGravity.Reshading();
				UnityEngine.Object.Destroy(base.gameObject, 1f);
			}
		}
	}

	public override bool CanBeCrushedByNearSlot()
	{
		return true;
	}
}
