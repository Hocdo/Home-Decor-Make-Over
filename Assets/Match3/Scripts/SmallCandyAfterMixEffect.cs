using System.Collections;

public class SmallCandyAfterMixEffect : SmallCandyMixEffect
{
	public override void OnSpawned()
	{
		base.OnSpawned();
		isAfterEffect = true;
	}

	protected override void RemoveEffect()
	{
		StartCoroutine(AfterEffect());
	}

	protected override void EndRolling()
	{
	}

	protected virtual Chip GetAfterChipEffect()
	{
		return null;
	}

	protected virtual IEnumerator AfterEffect()
	{
		base.RemoveEffect();
		Chip c = GetAfterChipEffect();
		if (c == null)
		{
			yield break;
		}
		c.parentSlot = lastPositionSlot.slotForChip;
		if (c is RainbowChipMixEffect)
		{
			RainbowChipMixEffect srm = c as RainbowChipMixEffect;
			srm.StartMixEffect();
			while (srm.destroingLock)
			{
				yield return null;
			}
		}
		else if (c is HVBombMixEffect)
		{
			HVBombMixEffect hVBombMixEffect = c as HVBombMixEffect;
			hVBombMixEffect.StartMixEffect();
		}
		else
		{
			yield return StartCoroutine(c.DestroyChipFunction());
		}
		isAfterEffect = false;
		base.EndRolling();
	}
}
