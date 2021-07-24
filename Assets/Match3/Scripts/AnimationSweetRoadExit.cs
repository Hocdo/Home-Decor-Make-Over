public class AnimationSweetRoadExit : AnimationObject
{
	private void Awake()
	{
		aniType = AnimationController.IdleAnimationType.SweetRoadExit;
	}

	public override void PlayIdleAnimation(AnimationController.IdleAnimationType type, AnimationProperty prop)
	{
		if (animator != null)
		{
			animator.SetTrigger(AnimationController.IdleAnimationName);
		}
	}
}
