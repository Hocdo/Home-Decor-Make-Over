using System.Collections;
using UnityEngine;
using Match3;

public class BoosterHBomb : Booster
{
	protected override void Start()
	{
		base.Start();
	}

	public override void ForceStart()
	{
		base.ForceStart();
	}

	public override void UseBooster(bool isTutorial = false)
	{
		if (CheckEnableBooster(isTutorial))
		{
			base.UseBooster(isTutorial);
			selectEffect.SetActive(value: true);
			guideView.gameObject.SetActive(value: true);
			guideView.TurnOnOnlyOneBoosterUI(uiIndex);

			if ((bool)guideView)
			{
				guideView.SetIconImage(boosterType);
			}
			if (onSelect)
			{
				CancelBooster();
				return;
			}
			SoundSFX.Play(SFXIndex.GameItemButtonClickCandyBomb);
			StartCoroutine(UseCandyPack());
		}		
	}

	public override void CancelBooster()
	{
		base.CancelBooster();
		ControlAssistant.main.ReleasePressedChip();
		onSelect = false;
		selectEffect.SetActive(value: false);
		guideView.gameObject.SetActive(value: false);
	}

	private IEnumerator UseCandyPack()
	{		
		onSelect = true;
		yield return StartCoroutine(Utils.WaitFor(GameMain.main.CanIWait, 0.1f));
		Slot targetSlot = null;
		while (onSelect)
		{
			if (!GameMain.main.CanIWait())
			{
				CancelBooster();
			}
			if (GameMain.main.CurrentTurn == VSTurn.CPU)
			{
				CancelBooster();
			}
			if (Input.GetMouseButtonDown(0) || (UnityEngine.Input.touchCount > 0 && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Ended))
			{
				targetSlot = ControlAssistant.main.GetSlotFromTouch();
			}
			if (targetSlot != null)
			{
				GameMain.main.isPlaying = false;
				selectEffect.SetActive(value: false);
				guideView.gameObject.SetActive(value: false);
				SoundSFX.Play(SFXIndex.GameItemUseHammer);
				Chip chip = null;
				Chip chip2 = null;
				int chipID = 0;
				if (targetSlot.GetBlock() == null)
				{
					if (targetSlot.GetChip() != null)
					{
						chipID = targetSlot.GetChip().id;
						if (targetSlot.GetChip().chipType == ChipType.BringDown)
						{
							chip2 = targetSlot.GetChip();
						}
						else
						{
							targetSlot.GetChip().DestroyChip();
						}
					}
				}
				else if (targetSlot.GetBlock() != null)
				{
					BoardManager.main.BlockCrush(targetSlot.x, targetSlot.y, radius: false);
					if (targetSlot.GetChip() != null)
					{
						chip = targetSlot.GetChip();
					}
				}

				Transform transform = SpawnStringBlock.GetSpawnBlockObjectHBomb(chipID).transform;
				ColorHBomb component = transform.GetComponent<ColorHBomb>();
				targetSlot.SetChip(component);
				component.transform.localPosition = Vector3.zero;
				component.DestroyChip();
				
				if ((bool)chip)
				{
					targetSlot.SetChip(chip);
				}
				else if ((bool)chip2)
				{
					targetSlot.SetChip(chip2);
				}
				CompleteUseBooster();
				ControlAssistant.main.ReleasePressedChip();
				GameMain.main.isPlaying = true;
				GameMain.main.TurnEndAfterUsingBooster();

				GlobalEventObserver.InvokeUseIngameBoosterEvent(boosterType);

				break;
			}
			yield return 0;
		}
		ControlAssistant.main.ReleasePressedChip();
		onSelect = false;
	}

	private bool CheckCanCrush(Slot targetSlot)
	{
		if (!targetSlot)
		{
			return false;
		}
		if (!targetSlot.canBeCrush)
		{
			return false;
		}
		if (targetSlot.GetBlock() != null)
		{
			return targetSlot.GetBlock().EnableBoosterCandyPack;
		}
		if (targetSlot.GetChip() != null)
		{
			if (targetSlot.GetChip().chipType == ChipType.BringDown)
			{
				return false;
			}
			if (targetSlot.GetChip().chipType == ChipType.CandyChip)
			{
				return false;
			}
			if (targetSlot.GetChip().chipType == ChipType.SimpleChip)
			{
				return true;
			}
			return false;
		}
		return false;
	}
}
