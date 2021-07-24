using System.Collections;
using UnityEngine;

public class ChocolateJail : BlockInterface
{
	private int eventCountBorn;

	private SlotGravity gravity;

	public override bool EnableBoosterHammer => true;

	public override bool EnableBoosterCandyCrane => false;

	public override bool EnableBoosterCandyPack => true;

	private void OnDestroy()
	{
		if (gravity != null)
		{
			gravity.enabled = true;
		}
	}

	public override void Initialize()
	{
		gravity = slot.slotGravity;
		slot.gravity = false;
		gravity.enabled = false;
		eventCountBorn = GameMain.main.eventCount;
		aniType = AnimationController.IdleAnimationType.ChocolateJail;
	}

	public override void BlockCrush(int fromCrushId = -1, int subId = -1)
	{
		if (eventCountBorn != GameMain.main.eventCount && !destroying)
		{
			destroying = true;
			eventCountBorn = GameMain.main.eventCount;
			GameMain.main.PrevThrowCollectItem(CollectBlockType.ChocolateJail);
			SoundSFX.PlayCap(SFXIndex.ChocolateJailRemove);
			Utils.DisableAllSpriteRenderer(base.gameObject);
			GameObject crushPoolObject = GetCrushPoolObject();
			crushPoolObject.transform.position = base.transform.position + new Vector3(0f, 0f, -1f);
			GameMain.main.DecreaseCollect(CollectBlockType.ChocolateJail);
			DespawnCrushPoolObject(crushPoolObject, 3f);
			StartCoroutine(delayCrush());
		}
	}

	private IEnumerator delayCrush()
	{
		yield return new WaitForSeconds(0.5f);
		Crush();
		slot.gravity = true;
		slot.SetBlock(null);
		gravity.enabled = true;
		SlotGravity.Reshading();
		GameMain.main.CheckRescueGingerMan(slot.x, slot.y);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public override bool CanBeCrushedByNearSlot()
	{
		return false;
	}

	public override void PlayIdleAnimation(AnimationController.IdleAnimationType type, AnimationProperty prop)
	{
		if (prop.numOfAniObj == -1 && animator != null)
		{
			animator.SetTrigger(AnimationController.IdleAnimationName);
		}
		else
		{
			StartCoroutine(ProcessIdleAnimation(prop));
		}
	}

	private IEnumerator ProcessIdleAnimation(AnimationProperty prop)
	{
		if (animator != null)
		{
			for (int i = 0; i < prop.numOfAni; i++)
			{
				prop.aniCount++;
				yield return new WaitForSeconds(AnimationController.AnimationTerm * (float)prop.aniCount);
				animator.SetTrigger(AnimationController.IdleAnimationName);
			}
		}
	}
}
