using UnityEngine;

public class ParticleEffectIncChildren : MonoBehaviour
{
	public string sortingLayer;

	public int sortingOrder;

	private void Start()
	{
		ParticleSystem[] componentsInChildren = base.transform.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem particleSystem in componentsInChildren)
		{
			particleSystem.GetComponent<Renderer>().sortingLayerName = sortingLayer;
			particleSystem.GetComponent<Renderer>().sortingOrder = sortingOrder;
		}
	}
}
