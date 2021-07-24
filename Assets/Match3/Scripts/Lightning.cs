using System.Collections;
using UnityEngine;

public class Lightning : MonoBehaviour
{
	private Vector3 a;

	private Vector3 b;

	public int bend = 2;

	public Vector3[] bendPoint;

	public Color color;

	private bool destroing;

	private float distance;

	public Transform end;

	private readonly float frequency = 20f;

	private float lastTime = -100f;

	private LineRenderer line;

	public string sortingLayer;

	public int sortingOrder;

	public Transform start;

	private void Start()
	{
		line = GetComponent<LineRenderer>();
		bendPoint = new Vector3[bend];
		line.SetColors(color, color);
		line.SetVertexCount(bend + 2);
		line.GetComponent<Renderer>().sortingLayerName = sortingLayer;
		line.sortingOrder = sortingOrder;
	}

	private void Update()
	{
		if (end == null || !end.gameObject.activeSelf || start == null || !start.gameObject.activeSelf)
		{
			Remove();
			return;
		}
		if (!destroing)
		{
			a = start.position;
			b = end.position;
		}
		distance = (a - b).magnitude;
		if (lastTime + 1f / frequency < Time.time)
		{
			lastTime = Time.time;
			for (int i = 0; i < bendPoint.Length; i++)
			{
				bendPoint[i] = new Vector3((2f * UnityEngine.Random.value - 1f) * 0.1f * distance, (2f * UnityEngine.Random.value - 1f) * 0.1f * distance, 0f);
			}
		}
		line.SetPosition(0, a);
		for (int j = 1; j < bend + 1; j++)
		{
			line.SetPosition(j, Vector3.Lerp(a, b, 1f * (float)j / (float)(bend + 1)) + bendPoint[j - 1]);
		}
		line.SetPosition(bend + 1, b);
	}

	public void Remove()
	{
		StartCoroutine(FadeOut());
	}

	private IEnumerator FadeOut()
	{
		if (!destroing)
		{
			destroing = true;
			while (GetComponent<Animation>().isPlaying)
			{
				yield return 0;
			}
			GetComponent<Animation>().Play("LightningFadeOut");
			while (GetComponent<Animation>().isPlaying)
			{
				yield return 0;
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public static Lightning CreateLightning(int bend, Transform start, Transform end, Color color)
	{
		Lightning item = ContentAssistant.main.GetItem<Lightning>("Lightning");
		item.transform.parent = BoardManager.main.slotGroup.transform;
		item.transform.localPosition += new Vector3(0f, 0f, -1f);
		item.bend = bend;
		item.start = start;
		item.end = end;
		item.color = color;
		return item;
	}
}
