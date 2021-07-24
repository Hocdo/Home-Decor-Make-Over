using DG.Tweening;
using System.Collections;
using UnityEngine;

public class AnimationAssistant : MonoBehaviour
{
	public static AnimationAssistant main;

	[HideInInspector]
	public bool forceSwap;

	private readonly float swapDuration = 0.2f;

	public bool Swaping
	{
		get;
		private set;
	}

	public AnimationAssistant()
	{
		Swaping = false;
	}

	private void Awake()
	{
		main = this;
	}

	private void Start()
	{
		BombMixEffect.Initialize();
	}

	public void SwapTwoItemNow(Chip a, Chip b)
	{
		if ((bool)a && (bool)b && !(a == b) && !a.parentSlot.slot.GetBlock() && !b.parentSlot.slot.GetBlock())
		{
			a.movementID = (b.movementID = GameMain.main.GetMovementID());
			SlotForChip parentSlot = a.parentSlot;
			SlotForChip parentSlot2 = b.parentSlot;
			parentSlot2.SetChip(a);
			parentSlot.SetChip(b);
		}
	}

	public void SwapTwoItem(Chip a, Chip b, Side swapDirection)
	{
		if (GameMain.main.isPlaying)
		{
			StartCoroutine(SwapTwoItemRoutine(a, b, swapDirection));
		}
	}

	public void ShakeChip(Chip chip)
	{
		if ((bool)chip)
		{
			Sequence s = DOTween.Sequence();
			s.Append(chip.transform.DORotate(new Vector3(0f, 0f, -10f), 0.05f));
			s.Append(chip.transform.DORotate(new Vector3(0f, 0f, 10f), 0.06f).SetLoops(7, LoopType.Yoyo));
			s.Append(chip.transform.DORotate(new Vector3(0f, 0f, 0f), 0.05f));
		}
	}

	private IEnumerator SwapTwoItemRoutine(Chip a, Chip b, Side swapDirection)
	{
		if (Swaping || !a || !b || !a.parentSlot || !b.parentSlot || !a.parentSlot.slot || !b.parentSlot.slot || (bool)a.parentSlot.slot.GetBlock() || (bool)b.parentSlot.slot.GetBlock() || a.chipType == ChipType.MouseChip || b.chipType == ChipType.MouseChip || a.parentSlot.slot.IsControlLock || b.parentSlot.slot.IsControlLock || !BoardManager.main || BoardManager.main.boardData == null || GameMain.main == null || !GameMain.main.CanIAnimate() || MapData.main == null || GameMain.main.MoveCount <= 0 || Popup.PopupSystem.Instance.IsShowingPopup())
		{
			yield break;
		}

#if UNITY_EDITOR
		Debug.Log("Swap (" + a.parentSlot.slot.x + ", " + a.parentSlot.slot.y + ") - (" + b.parentSlot.slot.x + ", " + b.parentSlot.slot.y + ") - " + swapDirection);
#endif

		if (GameMain.main.MoveCount == MapData.main.moveCount)
		{
			AppEventManager.m_TempBox.didAreaTutorialPlay = 0;
			if (BoardManager.main.boardData.tutorial1X != -1 && BoardManager.main.boardData.tutorial1Y != -1 && BoardManager.main.boardData.tutorial2X != -1 && BoardManager.main.boardData.tutorial2Y != -1)
			{
				if ((a.parentSlot.slot.x == BoardManager.main.boardData.tutorial1X && a.parentSlot.slot.y == BoardManager.main.boardData.tutorial1Y && b.parentSlot.slot.x == BoardManager.main.boardData.tutorial2X && b.parentSlot.slot.y == BoardManager.main.boardData.tutorial2Y) || (b.parentSlot.slot.x == BoardManager.main.boardData.tutorial1X && b.parentSlot.slot.y == BoardManager.main.boardData.tutorial1Y && a.parentSlot.slot.x == BoardManager.main.boardData.tutorial2X && a.parentSlot.slot.y == BoardManager.main.boardData.tutorial2Y))
				{
					AppEventManager.m_TempBox.didAreaTutorialPlay = 1;
				}
				else
				{
					AppEventManager.m_TempBox.didAreaTutorialPlay = -1;
				}
			}
		}
		GameMain.main.EventCounter();
		GameMain.main.TurnStart();
		bool mix = BombMixEffect.ContainsPair(a.chipType, b.chipType);
		int move = 0;
		GameMain.main.animate++;
		Swaping = true;
		Vector3 posA = a.parentSlot.transform.position;
		Vector3 posB = b.parentSlot.transform.position;
		float progress = 0f;
		int aPosX = a.parentSlot.slot.x;
		int y = a.parentSlot.slot.y;
		int bPosX = b.parentSlot.slot.x;
		int y2 = b.parentSlot.slot.y;
		Vector3 normal = (aPosX != bPosX) ? Vector3.up : Vector3.right;
		SoundSFX.Play(SFXIndex.BlockSwap);
		while (progress < swapDuration)
		{
			a.transform.position = Vector3.Lerp(posA, posB, progress / swapDuration) + normal * Mathf.Sin(3.14f * progress / swapDuration) * 0.2f;
			if (!mix)
			{
				b.transform.position = Vector3.Lerp(posB, posA, progress / swapDuration) - normal * Mathf.Sin(3.14f * progress / swapDuration) * 0.2f;
			}
			progress += Time.deltaTime;
			yield return 0;
		}
		a.transform.position = posB;
		if (!mix)
		{
			b.transform.position = posA;
		}
		a.movementID = (b.movementID = GameMain.main.GetMovementID());
		if (mix)
		{
			int targetId;
			int subId;
			if (a.chipType == ChipType.RainbowBomb && b.chipType != ChipType.RainbowBomb)
			{
				targetId = b.id;
				subId = a.id;
			}
			else if (b.chipType == ChipType.RainbowBomb && a.chipType != ChipType.RainbowBomb)
			{
				targetId = a.id;
				subId = b.id;
			}
			else if (a.chipType == ChipType.CandyChip || b.chipType == ChipType.CandyChip)
			{
				targetId = Mathf.Max(a.id, b.id);
				subId = Mathf.Min(a.id, b.id);
			}
			else
			{
				targetId = a.id;
				subId = b.id;
			}
			Swaping = false;
			ChipType chipType = a.chipType;
			ChipType chipType2 = b.chipType;
			SlotForChip parentSlot = b.parentSlot;
			SlotForChip parentSlot2 = a.parentSlot;
			a.HideChip();
			b.HideChip();
			GameMain.main.animate--;
			BombMixEffect.Mix(chipType, chipType2, parentSlot, parentSlot2, targetId, subId, swapDirection);
			GameMain.main.TurnEnd();
		}
		else
		{
			if (!a || !b || !a.parentSlot || !b.parentSlot)
			{
				yield break;
			}
			SlotForChip slotA = a.parentSlot;
			SlotForChip slotB = b.parentSlot;
			a.swapDirection = (b.swapDirection = SwapDiagonalType.None);
			if (slotB != null)
			{
				slotB.SetChip(a);
			}
			if (slotA != null)
			{
				slotA.SetChip(b);
			}
			move++;
			int count = 0;
			GameMain.Solution solutionA = slotA.MatchAnaliz();
			if (solutionA != null)
			{
				count += solutionA.count + solutionA.boxCombinationCount;
			}
			GameMain.Solution solutionB = slotB.MatchAnaliz();
			if (solutionB != null)
			{
				count += solutionB.count + solutionB.boxCombinationCount;
			}
			if (count == 0 && !forceSwap)
			{
				SoundSFX.Play(SFXIndex.BlockMismatch);
				AppEventManager.m_TempBox.GameUnMatchedMoveBlockCount++;
				while (progress > 0f)
				{
					a.transform.position = Vector3.Lerp(posA, posB, progress / swapDuration) - normal * Mathf.Sin(3.14f * progress / swapDuration) * 0.2f;
					b.transform.position = Vector3.Lerp(posB, posA, progress / swapDuration) + normal * Mathf.Sin(3.14f * progress / swapDuration) * 0.2f;
					progress -= Time.deltaTime;
					yield return 0;
				}
				a.transform.position = posA;
				b.transform.position = posB;
				a.movementID = (b.movementID = GameMain.main.GetMovementID());
				if ((bool)slotB)
				{
					slotB.SetChip(b);
				}
				if ((bool)slotA)
				{
					slotA.SetChip(a);
				}
				move--;
			}
			else
			{
				GameMain.main.swapEvent++;
			}
			GameMain.main.firstChipGeneration = false;
			if (move > 0 && GameMain.main.CurrentTurn == VSTurn.Player)
			{
				GameMain.main.TurnEnd(passToBossTurn: false);
			}
			GameMain.main.animate--;
			Swaping = false;
		}
	}

	public void Explode(Vector3 center, float radius, float force)
	{
		Chip[] componentsInChildren = BoardManager.main.slotGroup.GetComponentsInChildren<Chip>();
		Chip[] array = componentsInChildren;
		foreach (Chip chip in array)
		{
			if (!((chip.transform.position - center).magnitude > radius))
			{
				Vector3 a = (chip.transform.position - center) * force;
				a *= Mathf.Pow((radius - (chip.transform.position - center).magnitude) / radius, 2f);
				chip.impulse += a;
			}
		}
	}

	public void TeleportChip(Chip chip, Slot target)
	{
		StartCoroutine(TeleportChipRoutine(chip, target));
	}

	private IEnumerator TeleportChipRoutine(Chip chip, Slot target)
	{
		if ((bool)chip.parentSlot)
		{
			TrailRenderer trail = chip.gameObject.GetComponentInChildren<TrailRenderer>();
			float trailTime = 0f;
			if ((bool)trail)
			{
				trailTime = trail.time;
				trail.time = 0f;
			}
			Vector3 localScale = chip.transform.localScale;
			float defScale = localScale.x;
			float scale2 = defScale;
			while (scale2 > 0f)
			{
				scale2 -= Time.deltaTime * 10f;
				chip.transform.localScale = Vector3.one * scale2;
				yield return 0;
			}
			if (!target.GetChip() && (bool)chip && (bool)chip.parentSlot)
			{
				Transform a = chip.parentSlot.transform;
				Transform b = target.transform;
				Color color = (chip.id != Mathf.Clamp(chip.id, 0, 5)) ? Color.white : Chip.colors[chip.id];
				Lightning i = Lightning.CreateLightning(5, a, b, color);
				target.SetChip(chip);
				chip.transform.position = chip.parentSlot.transform.position;
				yield return 0;
				i.end = null;
			}
			yield return 0;
			if ((bool)trail)
			{
				trail.time = trailTime;
			}
			scale2 = 0.2f;
			while (scale2 < defScale)
			{
				scale2 += Time.deltaTime * 10f;
				chip.transform.localScale = Vector3.one * scale2;
				yield return 0;
			}
			chip.transform.localScale = Vector3.one * defScale;
		}
	}
}
