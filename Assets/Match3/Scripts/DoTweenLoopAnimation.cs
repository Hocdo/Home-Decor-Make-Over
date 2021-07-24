using DG.Tweening;
using System.Collections;
using UnityEngine;

public class DoTweenLoopAnimation : MonoBehaviour
{
	public float delay;

	private IEnumerator Start()
	{
		DOTweenAnimation aniTween = base.gameObject.GetComponent<DOTweenAnimation>();
		if ((bool)aniTween)
		{
			while (true)
			{
				yield return new WaitForSeconds(delay);
				aniTween.DORestart();
			}
		}
	}
}
