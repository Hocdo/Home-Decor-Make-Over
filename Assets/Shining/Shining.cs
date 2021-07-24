using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Shining : MonoBehaviour
{
	[SerializeField] float width = 0.2f;
	[SerializeField] float shiningTime = 1f;

	Image image;
    SpriteRenderer sprite;

    WaitForSeconds waitForSeconds;

	void Awake ()
	{
		image = GetComponent<Image> ();
        sprite = GetComponent<SpriteRenderer>();
		waitForSeconds = new WaitForSeconds (3f);
	}

	void OnEnable ()
	{
        if (image != null)
        {
            StartCoroutine(Shine());
        }

        if (sprite != null)
        {
            StartCoroutine(Shine2());
        }
    }

	IEnumerator Shine ()
	{
		while (true)
		{
			float currentTime = 0;
			float speed = 1f / shiningTime;
			image.material.SetFloat ("_Width", width);
			while (currentTime <= shiningTime)
			{
				currentTime += Time.deltaTime;
				float value = Mathf.Lerp (0, 1, speed * currentTime);
				image.material.SetFloat ("_TimeController", value);
				yield return null;
			}
			yield return waitForSeconds;
			image.material.SetFloat ("_Width", 0);
		}
	}

    IEnumerator Shine2()
    {
        while (true)
        {
            float currentTime = 0;
            float speed = 1f / shiningTime;
            sprite.material.SetFloat("_Width", width);
            while (currentTime <= shiningTime)
            {
                currentTime += Time.deltaTime;
                float value = Mathf.Lerp(0, 1, speed * currentTime);
                sprite.material.SetFloat("_TimeController", value);
                yield return null;
            }
            yield return waitForSeconds;
            sprite.material.SetFloat("_Width", 0);
        }
    }
}