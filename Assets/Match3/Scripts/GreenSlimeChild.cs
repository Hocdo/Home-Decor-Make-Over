using PathologicalGames;
using System.Collections;
using UnityEngine;

public class GreenSlimeChild : BlockInterface
{
	private SlotGravity gravity;

	private int eventCountBorn;

	private GreenSlimeParent parentSlime;

	public override bool EnableBoosterHammer => true;

	public override bool EnableBoosterCandyCrane => false;

	public override bool EnableBoosterCandyPack => true;

	public void Initialize(GreenSlimeParent parent)
	{
		if ((bool)slot)
		{
			parentSlime = parent;
			slot.gravity = false;
			gravity = slot.slotGravity;
			gravity.enabled = false;
			eventCountBorn = GameMain.main.eventCount;
		}
	}

	public override void Initialize()
	{
	}

	public override void BlockCrush(int fromCrushId = -1, int subId = -1)
	{
		if (eventCountBorn != GameMain.main.eventCount && !destroying)
		{
			destroying = true;
			eventCountBorn = GameMain.main.eventCount;
			StartCoroutine(delayCrush());
			if ((bool)parentSlime)
			{
				parentSlime.CanBreeding = false;
			}
		}
	}

	private IEnumerator delayCrush()
	{
		SoundSFX.PlayCap(SFXIndex.GreenSlimeRemove);
		GameObject objBreedingEffect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.GreenSlimePop);
		objBreedingEffect.transform.position = base.transform.position;
		PoolManager.PoolGameEffect.Despawn(objBreedingEffect.transform, 1f);
		yield return new WaitForSeconds(0.333f);
		Crush();
		slot.gravity = true;
		slot.SetBlock(null);
		gravity.enabled = true;
		SlotGravity.Reshading();
		UnityEngine.Object.Destroy(base.gameObject);
		GameMain.main.CheckRescueGingerMan(slot.x, slot.y);
	}

	public override bool CanBeCrushedByNearSlot()
	{
		return true;
	}
}
