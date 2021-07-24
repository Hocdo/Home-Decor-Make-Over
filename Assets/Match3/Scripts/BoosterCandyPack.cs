using DG.Tweening;
using System.Collections;
using UnityEngine;
using Match3;

public class BoosterCandyPack : Booster
{
	public GameObject prefabCandyPack;

	protected override void Start()
	{
		base.Start();
	}

	public override void ForceStart()
	{
		prefabCandyPack = (Resources.Load("Eff_item_candypack_bomb") as GameObject);
		base.ForceStart();

		itemImage.rectTransform.anchoredPosition = new Vector2(-0.2f, 3f);
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
				GameObject candyPack = Object.Instantiate(prefabCandyPack);
				if (!candyPack)
				{
					break;
				}
				selectEffect.SetActive(value: false);
				guideView.gameObject.SetActive(value: false);
				SoundSFX.Play(SFXIndex.GameItemUseCandyBomb);
				Transform transform = candyPack.transform;
				Vector3 position = targetSlot.transform.position;
				float x = position.x;
				Vector3 position2 = targetSlot.transform.position;
				float y = position2.y + 107f;
				Vector3 position3 = targetSlot.transform.position;
				transform.position = new Vector3(x, y, position3.z);
				yield return new WaitForSeconds(0.16f);
				Camera.main.transform.DOShakePosition(0.3f, 3f);
				yield return new WaitForSeconds(2.14f);
				Chip chipBackupOverObstacle = null;
				Chip chipBringDown = null;

				if (targetSlot.GetBlock() == null)
				{
					if (targetSlot.GetChip() != null)
					{
						if (targetSlot.GetChip().chipType == ChipType.BringDown)
						{
							chipBringDown = targetSlot.GetChip();
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
						chipBackupOverObstacle = targetSlot.GetChip();
					}
				}
				Transform t = SpawnStringBlock.GetSpawnBlockObject(SpawnStringBlockType.RainbowRainbowMixEffect).transform;
				RainbowRainbowMixEffect effect = t.GetComponent<RainbowRainbowMixEffect>();
				targetSlot.SetChip(effect);
				effect.transform.localPosition = Vector3.zero;
				yield return StartCoroutine(effect.BoosterKingCandyEffect());
				if ((bool)chipBackupOverObstacle)
				{
					targetSlot.SetChip(chipBackupOverObstacle);
				}
				else if ((bool)chipBringDown)
				{
					targetSlot.SetChip(chipBringDown);
				}
				CompleteUseBooster();
				ControlAssistant.main.ReleasePressedChip();
				StartCoroutine(DestroyCandyPack(candyPack));
				GameMain.main.isPlaying = true;
				GameMain.main.TurnEndAfterUsingBooster();

				


				break;
			}
			yield return 0;
		}
		ControlAssistant.main.ReleasePressedChip();
		onSelect = false;
	}

	private IEnumerator DestroyCandyPack(GameObject candyPack)
	{
		yield return new WaitForSeconds(1f);
		UnityEngine.Object.Destroy(candyPack);
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
