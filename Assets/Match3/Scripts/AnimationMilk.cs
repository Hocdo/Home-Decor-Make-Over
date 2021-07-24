using UnityEngine;

public class AnimationMilk : AnimationObject
{
	public GameObject milk;

	private void Awake()
	{
		aniType = AnimationController.IdleAnimationType.MIlk;
	}

	public override void Start()
	{
		base.Start();
		ParticleSystem[] componentsInChildren = milk.transform.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem particleSystem in componentsInChildren)
		{
			particleSystem.playOnAwake = false;
			particleSystem.loop = false;
			particleSystem.Stop();
		}
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
