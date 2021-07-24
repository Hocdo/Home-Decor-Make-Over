using System.Collections;
using UnityEngine;

public class ParticleEffect : MonoBehaviour
{
	public bool killAfterLifetime = true;

	private ParticleSystem ps;

	public string sortingLayer;

	public int sortingOrder;

	private void Start()
	{
		ps = GetComponent<ParticleSystem>();
		ps.GetComponent<Renderer>().sortingLayerName = sortingLayer;
		ps.GetComponent<Renderer>().sortingOrder = sortingOrder;
		if (killAfterLifetime)
		{
			StartCoroutine("Kill");
		}
	}

	private IEnumerator Kill()
	{
		yield return new WaitForSeconds(ps.duration + ps.startLifetime);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
