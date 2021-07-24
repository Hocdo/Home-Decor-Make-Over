using DG.Tweening;

public class AnimationBringdownEntry : AnimationObject
{
	private void Awake()
	{
		aniType = AnimationController.IdleAnimationType.BringdownEntryExit;
	}

	public override void PlayIdleAnimation(AnimationController.IdleAnimationType type, AnimationProperty prop)
	{
		DOTweenAnimation[] componentsInChildren = base.transform.GetComponentsInChildren<DOTweenAnimation>();
		foreach (DOTweenAnimation dOTweenAnimation in componentsInChildren)
		{
			dOTweenAnimation.DORestart(fromHere: true);
		}
	}
}
