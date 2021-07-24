using UnityEngine;

public class AnimationObject : MonoBehaviour
{
	protected Animator animator;

	protected AnimationController.IdleAnimationType aniType;

	public virtual void Start()
	{
		animator = GetComponent<Animator>();
		if (aniType != 0 && !MonoSingleton<AnimationController>.Instance.IsRegistedLoopAnimation(aniType))
		{
			if (GetComponent<Animator>() != null && GetComponent<Animator>().GetLayerIndex(AnimationController.NoAnimationName) != -1)
			{
				GetComponent<Animator>().Play(AnimationController.NoAnimationName);
			}
			ParticleSystem[] componentsInChildren = base.transform.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				particleSystem.playOnAwake = false;
				particleSystem.loop = false;
				particleSystem.Stop();
			}
		}
	}

	public virtual void PlayIdleAnimation(AnimationController.IdleAnimationType type, AnimationProperty prop)
	{
		if (type == aniType)
		{
		}
	}
}
