using UnityEngine;

public class ChipAnimationEvent : MonoBehaviour
{
	private Animation aniComponent;

	private void PlayAnimation(string name)
	{
		if (!aniComponent)
		{
			aniComponent = GetComponent<Animation>();
		}
		if ((bool)aniComponent)
		{
			aniComponent.CrossFade(name, 0.2f);
		}
	}
}
