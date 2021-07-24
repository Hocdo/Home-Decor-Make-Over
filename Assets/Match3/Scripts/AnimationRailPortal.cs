using UnityEngine;

public class AnimationRailPortal : AnimationObject
{
	private void Awake()
	{
		aniType = AnimationController.IdleAnimationType.RailPortal;
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
