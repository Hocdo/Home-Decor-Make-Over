using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Popup;
using DG.Tweening;

public class PopupReward : PopupBase
{
	public Image darkBgImage;

    public RewardItemTable rewardItemTable;

    private List<RewardItemController> currencyRewards = new List<RewardItemController>();

    private List<RewardItemController> playRewards = new List<RewardItemController>();

    private bool canCollect = true;

    public bool playCollectAnimation = true;

    public RewardItemController Add(RewardType type, int count)
    {
        var rewardItem = rewardItemTable.Get(type).GetComponent<RewardItemController>();

        if (type == RewardType.CurrencyStamina || type == RewardType.CurrencyStaminaInf)
        {            
            if (count >= 3600)
            {
                rewardItem.SetCount((count / 3600).ToString() + "h");
            }
            else
            {
                rewardItem.SetCount((count / 60).ToString() + "m");
            }
        }
        else
        {
            rewardItem.SetCount("x" + count.ToString());
        }

        if (type == RewardType.CurrencyCoin || type == RewardType.CurrencyGem || type == RewardType.CurrencyStamina || type == RewardType.CurrencyStaminaInf)
        {
            currencyRewards.Add(rewardItem);
        }
        else
        {
            playRewards.Add(rewardItem);
        }

        rewardItem.type = type;

        return rewardItem;
    }

    public override void Show()
	{
		canClose = false;

		Color color = darkBgImage.color;
		darkBgImage.color = new Color(0f, 0f, 0f, 0f);

		Sequence sequence = DOTween.Sequence();
        sequence
            .Append(darkBgImage.DOColor(color, 0.5f).OnComplete(() => canClose = true));	
	}

	public override void Close(bool forceDestroying = true)
	{
		TerminateInternal(forceDestroying);

        PostAnimateHideEvent?.Invoke();
    }

    public void PlayCollectItems()
    {
        if (CanClose() == false) return;

        if (canCollect == false) return;

        canCollect = false;

        if (playCollectAnimation) 
        {
            var halfCanvasSize = UIUtility.GetCanvasSize(transform.parent.GetComponent<CanvasScaler>()) * 0.5f;
            var camera = PopupSystem.Instance.popupCamera;
            float hx = camera.aspect * camera.orthographicSize;
            float hy = camera.orthographicSize;

            PopupSystem.Instance.StartCoroutine(PlayCollectItemsCoroutine(halfCanvasSize, hx, hy));
        }
        else 
        {
            for (int i = 0; i < currencyRewards.Count; i++)
            {
                var currencyReward = currencyRewards[i];
                currencyReward.SetAsCollectedMode();
                currencyReward.transform.DOScale(0f, 0.35f).OnComplete(() => 
                { 
                    currencyReward.CompleteAction?.Invoke(); 
                    Destroy(currencyReward.gameObject); 
                });
            }

            for (int i = playRewards.Count - 1; i >= 0; i--)
            {
                var playReward = playRewards[i];
                playReward.SetAsCollectedMode();
                playReward.transform.DOScale(0f, 0.35f).OnComplete(() => 
                {
                    playReward.CompleteAction?.Invoke();
                    Destroy(playReward.gameObject);
                });
            }
        }
            
        CloseInternal();
    }

    private IEnumerator PlayCollectItemsCoroutine(Vector2 halfCanvasSize, float hx, float hy)
    {
        Debug.Log("Collect");

        for (int i = 0; i < currencyRewards.Count; i++)
        {
            currencyRewards[i].SetAsCollectedMode();
        }

        for (int i = playRewards.Count - 1; i >= 0; i--)
        {
            playRewards[i].SetAsCollectedMode();
        }

        for (int i = 0; i < currencyRewards.Count; i++)
        {
            Vector2 ratio;

            if (currencyRewards[i].type == RewardType.CurrencyCoin)
            {
                ratio = new Vector2((357f - halfCanvasSize.x) / halfCanvasSize.x, (halfCanvasSize.y - 23f) / halfCanvasSize.y);
            }
            else if (currencyRewards[i].type == RewardType.CurrencyGem)
            {
                ratio = new Vector2((195f - halfCanvasSize.x) / halfCanvasSize.x, (halfCanvasSize.y - 23f) / halfCanvasSize.y);
            }
            else
            {
                ratio = new Vector2((32f - halfCanvasSize.x) / halfCanvasSize.x, (halfCanvasSize.y - 23f) / halfCanvasSize.y);
            }

            currencyRewards[i].SetAsCollectedMode();
            currencyRewards[i].CollectWithTarget(ratio * new Vector2(hx, hy), 0f);      
        }

        for (int i = playRewards.Count - 1; i >= 0 ; i--)
        {
            playRewards[i].SetAsCollectedMode();
            playRewards[i].CollectWithTarget(new Vector2((halfCanvasSize.x - 58f) / halfCanvasSize.x, (52f - halfCanvasSize.y) / halfCanvasSize.y) * new Vector2(hx, hy), 0f);

            yield return new WaitForSeconds(0.15f);
        }

        yield return null;
    }

    public void Align(float titleX, float titleY)
    {
        for (int i = 0; i < currencyRewards.Count; i++)
        {
            float x = (i - (currencyRewards.Count - 1) * 0.5f) * titleX;
            float y = titleY * 0.5f;
            currencyRewards[i].transform.position = new Vector3(x, y, 0f);
        }

        for (int i = 0; i < playRewards.Count; i++)
        {
            float x = (i - (playRewards.Count - 1) * 0.5f) * titleX;
            float y = -titleY * 0.5f;
            playRewards[i].transform.position = new Vector3(x, y, 0f);
        }

        for (int i = 0; i < currencyRewards.Count; i++)
        {
            float scale = currencyRewards[i].transform.localScale.x;
            currencyRewards[i].transform.localScale = Vector3.zero;
            currencyRewards[i].transform.DOScale(scale, 0.5f).SetEase(Ease.OutCubic);
        }

        for (int i = 0; i < playRewards.Count; i++)
        {
            float scale = playRewards[i].transform.localScale.x;
            playRewards[i].transform.localScale = Vector3.zero;
            playRewards[i].transform.DOScale(scale, 0.5f).SetEase(Ease.OutCubic);
        }
    }

    //public IEnumerator PlayAddGemSFXCoroutine()
    //{
    //	var waitForInterval = new WaitForSeconds(0.15f);

    //	canPlayAddCoinSFX = true;
    //	while (canPlayAddCoinSFX)
    //	{
    //		AudioManager.Instance.PlaySFX(AudioClipId.AddCoin);

    //		yield return waitForInterval;
    //	}
    //}
}