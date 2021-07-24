using System.Collections;
using UnityEngine;
using Match3;

public class BoosterMagicHammer : Booster
{
	public static bool isMagicHammerUsing;

	public GameObject prefabHammer;

	private bool doingShake;

	protected override void Start()
	{
		base.Start();
		doingShake = false;
	}

	protected override void Update()
	{
		base.Update();
	}

	public override void ForceStart()
	{
		prefabHammer = (Resources.Load("Eff_item_hammer") as GameObject);
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
			SoundSFX.Play(SFXIndex.GameItemButtonClickHammer);
			StartCoroutine(UseMagicHammer());
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

	private IEnumerator UseMagicHammer()
	{
		isMagicHammerUsing = true;
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
			if (GameMain.main.isTutorial == false && Input.GetMouseButtonDown(0) || (UnityEngine.Input.touchCount > 0 && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Ended))
			{
				targetSlot = ControlAssistant.main.GetSlotFromTouch();
			}

			if (GameMain.main.isTutorial && tutorialForceTarget)
            {
				targetSlot = tutorialForceTarget;
				tutorialForceTarget = null;
			}

			if (targetSlot != null)
			{
				if (CheckCanCrush(targetSlot))
				{
					GameMain.main.isPlaying = false;
					GameObject hammer = Object.Instantiate(prefabHammer);
					Transform transform = hammer.transform;
					Vector3 position = targetSlot.transform.position;
					float x = position.x;
					Vector3 position2 = targetSlot.transform.position;
					transform.position = new Vector2(x, position2.y);
					yield return new WaitForSeconds(0.5f);
					BoardManager.main.SlotCrush(targetSlot.x, targetSlot.y, radius: false, ScoreType.ChipCrushByItemBlock);
					SoundSFX.Play(SFXIndex.GameItemUseHammer);
					StartCoroutine(ProcessWorkingBlock(targetSlot));
					selectEffect.SetActive(value: false);
					guideView.gameObject.SetActive(value: false);
					ControlAssistant.main.ReleasePressedChip();
					CompleteUseBooster();
					StartCoroutine(DestroyHammer(hammer));
					GameMain.main.isPlaying = true;
					GameMain.main.TurnEndAfterUsingBooster();

					GlobalEventObserver.InvokeUseIngameBoosterEvent(boosterType);

					break;
				}
				//StartCoroutine(EffectSelectFail(targetSlot.transform));
				targetSlot = null;
				ControlAssistant.main.ReleasePressedChip();
				yield return null;
			}
			else
			{
				yield return 0;
			}
		}
		ControlAssistant.main.ReleasePressedChip();
		isMagicHammerUsing = false;
		onSelect = false;
	}

	private IEnumerator DestroyHammer(GameObject hammer)
	{
		yield return new WaitForSeconds(0.8f);
		UnityEngine.Object.Destroy(hammer);
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
			return targetSlot.GetBlock().EnableBoosterHammer;
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
			return true;
		}
		return false;
	}

	private IEnumerator ProcessWorkingBlock(Slot targetSlot)
	{
		BlockInterface block = targetSlot.GetBlock();
		if (!block)
		{
			yield break;
		}
		if (block.blockType >= IBlockType.CandyFactory_1 && block.blockType <= IBlockType.CandyFactory_6)
		{
			CandyFactory candyFactory = block as CandyFactory;
			if ((bool)candyFactory)
			{
				candyFactory.SpewCandyByBooster(boosterType);
			}
		}
		else if (block.blockType == IBlockType.PastryBag)
		{
			PastryBag pastryBag = block as PastryBag;
			if ((bool)pastryBag)
			{
				pastryBag.CrushByBooster(boosterType);
			}
		}
		else if (block.blockType == IBlockType.GreenSlime)
		{
			GreenSlimeParent greenSlimeParent = block as GreenSlimeParent;
			if ((bool)greenSlimeParent)
			{
				greenSlimeParent.CrushByBooster(boosterType);
			}
		}
	}
}
