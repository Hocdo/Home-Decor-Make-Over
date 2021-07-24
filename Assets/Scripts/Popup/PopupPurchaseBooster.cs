using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using Popup;

public class PopupPurchaseBooster : PopupBase
{
	public Transform main;

	public Text titleText;

	public Text descText;

	public Text costText;

	public Text amountText;

	public Image boosterImage;

	public RectTransform gem;

	public Text gemCountText;

	private UIEdgeSnapPosition gemSnapPos;

	private int rewardCount;

	private int gemCost;

	private int index;

	private PlayerData playerData;

    private void Awake()
    {
		playerData = PlayerData.current;
		gemSnapPos = new UIEdgeSnapPosition(gem, new Vector2(0f, 1.2f));		
	}

    public override void Show()
	{
		gemCountText.text = playerData.gemCount.ToString();
		gemSnapPos.SetPositionVisibility(false);
		gemSnapPos.Show(0.35f).SetEase(Ease.OutBack).OnComplete(() => canClose = true);
		
		canClose = false;
		PopupAnimationUtility.AnimateScale(main, Ease.OutBack, 0.25f, 1f, 0.25f, 0f);
	}

	public override void Close(bool forceDestroying = true)
	{
		TerminateInternal(forceDestroying);
	}

	public void SetData(int idx, string title, string desc, int cost, int amount, Sprite boosterSprite)
    {
		rewardCount = amount;
		gemCost = cost;
		index = idx;

		titleText.text = title;
		descText.text = desc;
		descText.font = CustomLocalization.GetFont();

		costText.text = cost.ToString();
		amountText.text = "x" + amount.ToString();
		boosterImage.sprite = boosterSprite;
    }

	public void BuyButtonPressed()
    {
		if (playerData.gemCount >= gemCost)
        {
			playerData.AddGem(-gemCost);
			gemCountText.text = playerData.gemCount.ToString();
			SoundSFX.Play(SFXIndex.BuyBooster);

			playerData.match3Data.AddIngameBooster((Match3.BoosterType)index, rewardCount);

			AcceptEvent?.Invoke(null);

			CloseInternal();
		}
        else
        {
			PopupSystem.Instance.ShowPopup(PopupType.PopupGetMoreGems, CurrentPopupBehaviour.Close, true, true);
        }
    }
}
