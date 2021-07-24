using System.Collections;
using UnityEngine;

public class AnimationGingermanUnderTile : AnimationObject
{
	private void Awake()
	{
		aniType = AnimationController.IdleAnimationType.GingermanUnderTile;
	}

	public override void PlayIdleAnimation(AnimationController.IdleAnimationType type, AnimationProperty prop)
	{
		if (prop.numOfAniObj == -1 && animator != null)
		{
			SetTriggerIdleAnimation();
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
				SetTriggerIdleAnimation();
			}
		}
	}

	private void SetTriggerIdleAnimation()
	{
		if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Rotation_90"))
		{
			animator.SetTrigger(AnimationController.IdleAnimationName);
		}
	}
}
