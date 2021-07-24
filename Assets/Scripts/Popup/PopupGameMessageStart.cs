using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Popup;
using DG.Tweening;

public class PopupGameMessageStart : PopupBase
{
    public float delayTime = 2f;

    public float dropTime = 0.5f;

    public float idleTime = 1.5f;

    public float liftTime = 0.5f;

    public AnimationCurve dropCurve;

    public AnimationCurve liftCurve;

    public RectTransform bgTransform;

    public RectTransform targetTransform;

	public GameObject GoalSample;

	public Transform GoalGroupTransform;

    private Vector2 sourcePos;

	public void Start()
	{
        var collectBlocks = MapData.main.collectBlocks;
        var collectViews = new List<CollectBlockView>();

        for (int j = 0; j < collectBlocks.Length; j++)
        {
            if (string.IsNullOrEmpty(collectBlocks[j].blockType) || collectBlocks[j].count <= 0 || !(GoalSample != null))
            {
                continue;
            }

            GameObject gameObject = (j == 0) ? GoalSample : Instantiate(GoalSample);
            if (!gameObject)
            {
                continue;
            }

            CollectBlockView component = gameObject.GetComponent<CollectBlockView>();
            collectViews.Add(component);
            if (!component)
            {
                continue;
            }

            if (!string.IsNullOrEmpty(collectBlocks[j].blockType) && collectBlocks[j].count > 0)
            {
                component.gameObject.transform.SetParent(GoalGroupTransform, worldPositionStays: false);
                CollectBlockType collectBlockType = collectBlocks[j].GetCollectBlockType();

                component.SetData(collectBlockType, collectBlocks[j].count,
                    CollectBlockView.GetCollectSprite(collectBlockType, MapData.main.collectBlocks[j].blockType));
            }
            else
            {
                DestroyImmediate(component);
            }
        }

        for (int i = 0; i < collectViews.Count; i++)
        {
            collectViews[i].UpdateSize();
        }
    }

	public override void Show()
	{
        sourcePos = bgTransform.anchoredPosition;
        canClose = false;

        StartCoroutine(ShowCoroutine());
    }

    public IEnumerator ShowCoroutine()
    {
        bgTransform.DOAnchorPos(targetTransform.anchoredPosition, dropTime).SetEase(dropCurve).SetDelay(delayTime);

        yield return new WaitForSeconds(delayTime + dropTime + idleTime);

        canClose = true;
        CloseInternal();
    }

	public override void Close(bool forceDestroying = true)
	{
        bgTransform.DOAnchorPos(sourcePos, liftTime).SetEase(liftCurve).OnComplete(() => 
        {
            PostAnimateHideEvent?.Invoke();
            TerminateInternal(forceDestroying);
        });        
    }
}
