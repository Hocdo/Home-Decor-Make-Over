using UnityEngine;

public class AnimationBoardLine : AnimationObject
{
	private void Awake()
	{
		aniType = AnimationController.IdleAnimationType.BoardLine;
	}

	public override void PlayIdleAnimation(AnimationController.IdleAnimationType type, AnimationProperty prop)
	{
		ParticleSystem[] componentsInChildren = base.transform.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem particleSystem in componentsInChildren)
		{
			particleSystem.Play(withChildren: true);
		}
	}
}
