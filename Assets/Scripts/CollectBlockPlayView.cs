using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CollectBlockPlayView : CollectBlockView
{
	public GameObject check;

	private float targetCountTextScale;

	private float targetImageScale;

    public override void UpdateSize()
    {
        base.UpdateSize();

        if (collectType == CollectBlockType.DiggingCandy || collectType == CollectBlockType.DiggingTreasure_G1 || collectType == CollectBlockType.DiggingTreasure_G2 || collectType == CollectBlockType.DiggingTreasure_G3)
        {
            targetImage.transform.localScale = 1.15f * Vector3.one;
            targetImageScale = targetImage.transform.localScale.x;
        }
    }

    public override void SetData(CollectBlockType type, int atargetCount, Sprite targetSprite)
	{       
        targetCountTextScale = targetCountText.transform.localScale.x;
        targetImageScale = targetImage.transform.localScale.x;

        check.SetActive(false);

		base.SetData(type, atargetCount, targetSprite); 
    }

	public override void UpdateTargetCount(int atargetCount)
	{
		base.UpdateTargetCount(atargetCount);

		if (collectType == CollectBlockType.SweetRoadConnect)
		{
			targetCountText.gameObject.SetActive(false);
			if (GameMain.main.isConnectedOnlySweetRoad && (bool)check)
			{
				check.SetActive(true);
			}
			return;
		}

		targetImage.gameObject.transform.DOScale(targetImageScale * 1.5f, 0.4f * 0.5f);
		targetImage.gameObject.transform.DOScale(targetImageScale, 0.4f).SetDelay(0.4f * 0.5f);

		Sequence sequence = DOTween.Sequence();
		sequence.Append(targetCountText.transform.DOScale(targetCountTextScale * 1.5f, 0.2f));
		sequence.Append(targetCountText.transform.DOScale(targetCountTextScale, 0.2f));
		sequence.Play();

		if (targetCount == 0)
		{
			check.SetActive(value: true);
			check.transform.localScale = Vector3.zero;
			check.transform.DOScale(1f, 0.3f);
			targetCountText.gameObject.SetActive(false);
		}
	}
}
