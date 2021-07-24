using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Match3;

public class BoosterShuffle : Booster
{
	public static int shuffleEffectType = 2;

	private bool usingBooster;

	protected override void Start()
	{
		base.Start();
	}

	protected override void Caching()
	{
		base.Caching();
	}

	public override void ForceStart()
	{
		base.ForceStart();
	}

	public override void UseBooster(bool isTutorial = false)
	{
		if (CheckEnableBoosterForShuffle(isTutorial))
		{
			base.UseBooster(isTutorial);
		}
	}

	public override void UpdateTextBoosterCount()
	{
		base.UpdateTextBoosterCount();

        if (PlayerData.current.match3Data.ingameBooster[(int)boosterType] == 0)
        {
			cost.gameObject.SetActive(true);
        }
        else
        {
			cost.gameObject.SetActive(false);
		}

        buttonBuy.SetActive(false);
    }

	protected bool CheckEnableBoosterForShuffle(bool isTutorial = false)
	{
		if (!isTutorial)
		{
			if (!GameMain.main.CanINextTurn())
			{
				return false;
			}
			if (!GameMain.main.isPlaying)
			{
				return false;
			}
			if (!GameMain.main.CanIWait())
			{
				return false;
			}
			if (GameMain.main.isGameResult)
			{
				return false;
			}
			if (GameMain.main.CurrentTurn == VSTurn.CPU)
			{
				return false;
			}
			if (GameMain.main.isConnectedSweetRoad)
			{
				return false;
			}
			if (usingBooster)
			{
				return false;
			}
		}

		var playerData = PlayerData.current;
		if (playerData.match3Data.ingameBooster[(int)boosterType] > 0)
		{
			StartCoroutine(UseBooster());
			return true;
		}
        else
        {
			if (playerData.gemCount >= 10)
            {
				playerData.AddGem(10);
				SoundSFX.Play(SFXIndex.BuyBooster);
				StartCoroutine(UseBooster());
			}
            else
            {
				Popup.PopupSystem.Instance.ShowPopup(PopupType.PopupGetMoreGems, Popup.CurrentPopupBehaviour.Close, true, true);
			}			
		}

		return false;
	}

	public override void CancelBooster()
	{
		base.CancelBooster();
		ControlAssistant.main.ReleasePressedChip();
		onSelect = false;
		selectEffect.SetActive(value: false);
		guideView.gameObject.SetActive(value: false);
	}

	private IEnumerator UseBooster()
	{
		usingBooster = true;
		UpdateTextBoosterCount();
		selectEffect.SetActive(value: true);
		guideView.gameObject.SetActive(value: true);
		guideView.TurnOnOnlyOneBoosterUI(uiIndex);
		guideView.textGuide.text = string.Empty;
		guideView.SetVisualize(false);

		GlobalEventObserver.InvokeUseIngameBoosterEvent(boosterType);

		yield return StartCoroutine(Utils.WaitFor(GameMain.main.CanIWait, 0.1f));
		GameMain.main.isPlaying = true;
		SoundSFX.Play(SFXIndex.Shuffle);
		yield return StartCoroutine(GameMain.main.ShuffleBooster());
		CompleteUseBooster();
		selectEffect.SetActive(value: false);
		guideView.gameObject.SetActive(value: false);
		guideView.SetVisualize(true);
		UpdateTextBoosterCount();
		ControlAssistant.main.ReleasePressedChip();
		onSelect = false;
		usingBooster = false;
	}
}
