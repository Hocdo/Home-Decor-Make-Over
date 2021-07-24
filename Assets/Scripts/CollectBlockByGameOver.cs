using UnityEngine;
using UnityEngine.UI;

public class CollectBlockByGameOver : MonoBehaviour
{
	public Image ImageTarget;

	public GameObject ObjCheck;

	public Text TextTargetCount;

	public Text TextTreasureHeader;

	public Text TextTreasureFooter;

	public void SetData(CollectBlockType _collectType, int remainCount, int targetCount, Sprite _targetSprite)
	{
		if ((bool)ObjCheck)
		{
			ObjCheck.SetActive(value: false);
		}
		ImageTarget.rectTransform.sizeDelta = new Vector2(_targetSprite.rect.width, _targetSprite.rect.height);
		ImageTarget.sprite = _targetSprite;
		UpdateTargetCount(remainCount, targetCount);
	}

	public void SetDataForTreasure(int remainCount, int targetCount)
	{
		TextTreasureHeader.text = $"{remainCount}  more";
	}

	private void UpdateTargetCount(int _remainCount, int _targetCount)
	{
		if (_targetCount < 0)
		{
			_targetCount = 0;
		}
		if ((bool)TextTargetCount)
		{
			if (_targetCount == 0 && (bool)ObjCheck)
			{
				ObjCheck.SetActive(value: true);
				TextTargetCount.gameObject.SetActive(value: false);
				ImageTarget.transform.localScale = new Vector3(1.3f, 1.3f, 1f);
			}
			else
			{
				TextTargetCount.gameObject.SetActive(value: true);
				TextTargetCount.text = $"<color=#ff0000ff>{_remainCount} </color>/{_targetCount}";
				ImageTarget.transform.localScale = new Vector3(1.6f, 1.6f, 1f);
			}
		}
	}
}
