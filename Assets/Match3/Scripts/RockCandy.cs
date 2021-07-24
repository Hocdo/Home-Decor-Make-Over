using DG.Tweening;
using System.Collections;
using UnityEngine;

public class RockCandy : MonoBehaviour
{
	private readonly float effSpeed = 0.5f;

	public SpriteRenderer effSr;

	private int level;

	public Sprite[] sprites;

	public SpriteRenderer sr;

	public int Level
	{
		get
		{
			return level;
		}
		set
		{
			if (level > value)
			{
				StartCoroutine(RemoveEffect(value));
				return;
			}
			if (level < value)
			{
				StartCoroutine(CreateEffect(value));
				return;
			}
			if (level > 0)
			{
				sr.sprite = sprites[level - 1];
			}
			sr.enabled = ((level != 0) ? true : false);
		}
	}

	private IEnumerator RemoveEffect(int value)
	{
		if (level > 1 && value >= 1)
		{
			effSr.sprite = sprites[level - 1];
			level = Mathf.Min(value, sprites.Length);
			sr.sprite = sprites[level - 1];
			effSr.transform.DOScale(0f, effSpeed);
			yield return new WaitForSeconds(effSpeed);
			effSr.transform.DOScale(1f, 0f);
			effSr.sprite = null;
			yield break;
		}
		level = Mathf.Min(value, sprites.Length);
		base.transform.DOScale(0f, effSpeed);
		yield return new WaitForSeconds(effSpeed);
		base.transform.DOScale(1f, 0f);
		if (level > 0)
		{
			sr.sprite = sprites[level - 1];
		}
		sr.enabled = ((level != 0) ? true : false);
	}

	private IEnumerator CreateEffect(int value)
	{
		if (level > 0)
		{
			int preLevel = level;
			sr.sprite = sprites[level - 1];
			level = Mathf.Min(value, sprites.Length);
			if (level != preLevel)
			{
				effSr.transform.localScale = Vector3.zero;
				effSr.sprite = sprites[level - 1];
				effSr.transform.DOScale(1f, effSpeed);
				yield return new WaitForSeconds(effSpeed);
				sr.sprite = sprites[level - 1];
				effSr.sprite = null;
			}
		}
		else
		{
			level = Mathf.Min(value, sprites.Length);
			base.transform.localScale = Vector3.zero;
			sr.sprite = sprites[level - 1];
			sr.enabled = ((level != 0) ? true : false);
			base.transform.DOScale(1f, effSpeed);
			yield return new WaitForSeconds(effSpeed);
		}
	}

	public void Remove()
	{
		if (Level > 0)
		{
			Level = 0;
		}
	}

	public bool Crush()
	{
		if (Level > 0)
		{
			Level--;
			if (Level == 0)
			{
				GameMain.main.DecreaseCollect(CollectBlockType.RockCandy, countPrevValue: true);
				return true;
			}
		}
		return false;
	}

	public void Fill()
	{
		Level++;
	}
}
