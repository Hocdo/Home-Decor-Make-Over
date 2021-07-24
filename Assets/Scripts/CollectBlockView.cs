using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectBlockView : MonoBehaviour
{
	protected CollectBlockType collectType;

	public Image targetImage;

	protected int targetCount;

	public Text targetCountText;

	private Vector2 size;

	public virtual void UpdateSize()
    {
		if (collectType == CollectBlockType.RescueGingerMan)
		{
			float width = targetImage.rectTransform.sizeDelta.x;
			float w = width * 23f / 38f;
			float h = w / 27f * 50f;
			size = new Vector2(w, h);
			targetImage.rectTransform.sizeDelta = new Vector2(w, h);
		}       
	}

	public virtual void SetData(CollectBlockType type, int atargetCount, Sprite targetSprite)
	{		
		targetCount = atargetCount;
		collectType = type;
		targetImage.sprite = targetSprite;

		UpdateTargetCount(targetCount);
	}

	public virtual void UpdateTargetCount(int atargetCount)
	{
		targetCount = Mathf.Max(0, atargetCount);
		targetCountText.text = ((collectType != CollectBlockType.RescueFriend) ? targetCount.ToString() : BoardManager.main.remainRescueFriendInBoard.ToString());
	}

	public static Sprite GetCollectSprite(CollectBlockType collectBlockType, string blockTypeStr)
    {
		Sprite sprite;
		switch (collectBlockType)
		{
			case CollectBlockType.NormalRed:
			case CollectBlockType.NormalOrange:
			case CollectBlockType.NormalYellow:
			case CollectBlockType.NormalGreen:
			case CollectBlockType.NormalBlue:
			case CollectBlockType.NormalPurple:
				sprite = CollectIconTable.Instance.GetSprite(blockTypeStr);
				break;
			default:
				sprite = CollectIconTable.Instance.GetSprite(blockTypeStr);
				break;
			case CollectBlockType.Null:
				return null;
		}

		return sprite;
	}
}
