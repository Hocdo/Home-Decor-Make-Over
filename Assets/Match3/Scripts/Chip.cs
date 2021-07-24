//@TODO ENABLE_MEC

using DG.Tweening;
#if ENABLE_MEC
using MEC;
using MovementEffects;
#endif
using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chip : AnimationObject
{
	private static readonly float velocityLimit = 1000f;

	public static Color[] colors = new Color[6]
	{
		new Color(0.75f, 0.3f, 0.3f),
		new Color(0.3f, 0.75f, 0.3f),
		new Color(0.3f, 0.5f, 0.75f),
		new Color(0.75f, 0.75f, 0.3f),
		new Color(0.75f, 0.3f, 0.75f),
		new Color(0.75f, 0.6f, 0.3f)
	};

	public static float combineBombDuration = 0.2f;

	public static WaitForSeconds waitForCombineBomb = new WaitForSeconds(combineBombDuration);

	public static Vector3 dropScale = new Vector3(1.05f, 1.2f, 1f);

	public static Vector3 baseScale = new Vector3(1.1f, 1.1f, 1f);

	protected static string idleAnimationName = "Idle";

	private static readonly string EndOfClickAnimationMethodName = "EndOfClickAnimation";

	private readonly float acceleration = 2000f;

	protected Animator aniController;

	public bool canMove = true;

	[HideInInspector]
	public ChipType chipType;

	public bool destroying;

	public bool destroyable = true;

	public int id;

	public Vector3 impulse = Vector3.zero;

	private Vector3 impulsParent = new Vector3(0f, 0f, -1f);

	protected bool isBouncePlaying;

	public bool isCandyMix;

	public bool isCombineBomb;

	public bool isLockOnCrowMove;

	private bool isMatching;

	protected bool isMuteki;

	private bool isPlayingClickAnimation;

	protected bool isWaitCreateEffect;

	private Vector3 lastPosition;

	public bool move;

	public int movementID;

	private Vector3 moveVector;

	private float mutekiDurationTime;

	private float mutekiSetTime;

	public SlotForChip parentSlot;

	public int powerId;

	private Vector3 startPosition;

	[HideInInspector]
	public SwapDiagonalType swapDirection;

	[HideInInspector]
	public float collectDelayTime = 0f;

	public MagicalCrow targetAttakMagicCrow;

	public float velocity;

	private Vector3 zVector;

    protected bool isCollected = false;

    public System.Action CrushAction;

	protected bool IsMatching
	{
		get
		{
			return isMatching;
		}
		set
		{
			if (value)
			{
				GameMain.main.matching++;
			}
			else
			{
				GameMain.main.matching = Mathf.Max(0, GameMain.main.matching - 1);
			}
			isMatching = value;
		}
	}

	public bool IsMuteki
	{
		get
		{
			return isMuteki;
		}
		private set
		{
		}
	}

	public virtual void OnSpawned()
	{
		CrushAction = null;
		movementID = 0;
		velocity = 1f;
		isMatching = false;
		move = true;
		canMove = true;
		base.transform.localScale = Vector3.one;
		destroying = false;
		isBouncePlaying = false;
		isCombineBomb = false;
		isCandyMix = false;
		GameMain.main.gravity++;
		isPlayingClickAnimation = false;
		isLockOnCrowMove = false;
		targetAttakMagicCrow = null;
		impulse = Vector3.zero;
		Utils.EnableAllSpriteRenderer(base.gameObject);
	}

	public virtual void OnDespawned()
	{
		CrushAction?.Invoke();
		StopBounceEffect();
		if (IsMatching)
		{
			IsMatching = false;
		}
		if (move)
		{
			GameMain.main.gravity--;
		}
		base.transform.localScale = baseScale;
		SpriteRenderer component = GetComponent<SpriteRenderer>();
		if ((bool)component)
		{
			component.color = Color.white;
		}
	}

	public virtual void Awake()
	{
		velocity = 1f;
		move = true;
		aniController = GetComponent<Animator>();
	}

	public virtual void ShowCreateEffect()
	{
		base.transform.localScale = baseScale;
		Utils.DisableAllSpriteRenderer(base.gameObject);
	}

	public bool IsMatchable()
	{
		if (id < 0)
		{
			return false;
		}
		if (destroying)
		{
			return false;
		}
		if (isCombineBomb)
		{
			return false;
		}
		if (!canMove)
		{
			return false;
		}
		if (GameMain.main == null)
		{
			return false;
		}
		if (GameMain.main.gravity == 0)
		{
			return true;
		}
		if (parentSlot == null)
		{
			return false;
		}
		if (move)
		{
			return false;
		}
		if (base.transform.position != parentSlot.transform.position)
		{
			return false;
		}
		if (velocity != 0f)
		{
			return false;
		}
		Side side = Side.Null;
		for (int i = 0; i < 4; i++)
		{
			side = Utils.straightSides[i];
			if ((bool)parentSlot[side] && parentSlot[side].gravity && !parentSlot[side].GetShadow() && !parentSlot[side].GetChip())
			{
				return false;
			}
		}
		return true;
	}

	public virtual void Update()
	{
		RunDrop();
		if (!isMuteki)
		{
			return;
		}
		if (mutekiDurationTime < mutekiSetTime)
		{
			if (GameMain.main.CanIMatch())
			{
				isMuteki = false;
			}
			else
			{
				mutekiDurationTime += Time.deltaTime;
			}
		}
		else
		{
			isMuteki = false;
		}
	}

	public void RunDrop()
	{
		if (destroying || isCombineBomb || GameMain.main == null || (!GameMain.main.isPlaying && !GameMain.main.isGameResult) || GameMain.main.doingStartCameraTween)
		{
			return;
		}
		if (impulse != Vector3.zero && ((bool)parentSlot || impulsParent.z != -1f))
		{
			if (impulsParent.z == -1f)
			{
				if (!parentSlot)
				{
					impulse = Vector3.zero;
					return;
				}
				if (!move)
				{
					GameMain.main.gravity++;
				}
				move = true;
				StopBounceEffect();
				impulsParent = parentSlot.transform.position;
			}
			base.transform.position += impulse * Time.deltaTime;
			base.transform.position += (impulsParent - base.transform.position) * Time.deltaTime;
			impulse -= impulse * Time.deltaTime;
			impulse -= base.transform.position - impulsParent;
			impulse *= 1f - 6f * Time.deltaTime;
			if ((base.transform.position - impulsParent).magnitude < 2f * Time.deltaTime && impulse.magnitude < 2f)
			{
				impulse = Vector3.zero;
				base.transform.position = impulsParent;
				impulsParent.z = -1f;
				if (move)
				{
					GameMain.main.gravity--;
				}
				move = false;
				base.transform.localScale = baseScale;
			}
		}
		else
		{
			if (!GameMain.main.CanIGravity() || !canMove)
			{
				return;
			}
			moveVector.x = 0f;
			moveVector.y = 0f;
			moveVector.z = 0f;
			if ((bool)parentSlot && base.transform.position != parentSlot.transform.position)
			{
				if (!move && Vector3.Distance(base.transform.position, parentSlot.transform.position) > 0.1f)
				{
					move = true;
					velocity = 2f;
					base.transform.localScale = dropScale;
					GameMain.main.gravity++;
				}
				velocity += acceleration * Time.deltaTime;
				if (velocity > velocityLimit)
				{
					velocity = velocityLimit;
				}
				lastPosition = base.transform.position;
				Vector3 position = base.transform.position;
				float x = position.x;
				Vector3 position2 = parentSlot.transform.position;
				if (Mathf.Abs(x - position2.x) < velocity * Time.deltaTime)
				{
					zVector = base.transform.position;
					ref Vector3 reference = ref zVector;
					Vector3 position3 = parentSlot.transform.position;
					reference.x = position3.x;
					base.transform.position = zVector;
				}
				Vector3 position4 = base.transform.position;
				float y = position4.y;
				Vector3 position5 = parentSlot.transform.position;
				if (Mathf.Abs(y - position5.y) < velocity * Time.deltaTime)
				{
					zVector = base.transform.position;
					ref Vector3 reference2 = ref zVector;
					Vector3 position6 = parentSlot.transform.position;
					reference2.y = position6.y;
					base.transform.position = zVector;
				}
				if (base.transform.position == parentSlot.transform.position)
				{
					parentSlot.slot.slotGravity.GravityReaction();
					if (base.transform.position != parentSlot.transform.position)
					{
						base.transform.position = lastPosition;
					}
				}
				Vector3 position7 = base.transform.position;
				float x2 = position7.x;
				Vector3 position8 = parentSlot.transform.position;
				if (x2 < position8.x)
				{
					moveVector.x = 10f;
				}
				Vector3 position9 = base.transform.position;
				float x3 = position9.x;
				Vector3 position10 = parentSlot.transform.position;
				if (x3 > position10.x)
				{
					moveVector.x = -10f;
				}
				Vector3 position11 = base.transform.position;
				float y2 = position11.y;
				Vector3 position12 = parentSlot.transform.position;
				if (y2 < position12.y)
				{
					moveVector.y = 10f;
				}
				Vector3 position13 = base.transform.position;
				float y3 = position13.y;
				Vector3 position14 = parentSlot.transform.position;
				if (y3 > position14.y)
				{
					moveVector.y = -10f;
				}
				moveVector = moveVector.normalized * velocity;
				base.transform.position += moveVector * Time.deltaTime;
			}
			else if (move)
			{
				move = false;
				base.transform.localScale = baseScale;
				velocity = 0f;
				GameMain.main.gravity--;
				SetBoundEffect();
			}
		}
	}

	private void SetBoundEffect()
	{
		if ((bool)aniController && !isBouncePlaying && base.gameObject.activeSelf)
		{
			if (!(this is BombMixEffect))
			{
				aniController.SetTrigger("setBounce");
			}
#if ENABLE_MEC
			Timing.RunCoroutine(waitBounceEnd());
#else
            StartCoroutine(waitBounceEnd());
#endif
            isBouncePlaying = true;
		}
		SoundSFX.PlayCap(SFXIndex.BlockDrop);
	}

	private IEnumerator<float> waitBounceEnd()
	{
#if ENABLE_MEC
        yield return Timing.WaitForSeconds(GameMain.BounceTweenTime);
#else
        yield return GameMain.BounceTweenTime;
#endif
        isBouncePlaying = false;
	}

	public bool StopBounceEffect()
	{
		if ((bool)aniController && isBouncePlaying)
		{
#if ENABLE_MEC
			Timing.RunCoroutine(waitBounceEnd());
#else
            StartCoroutine(waitBounceEnd());
#endif
            isBouncePlaying = false;
			return true;
		}
		return false;
	}

	public int GetPotential()
	{
		return GetPotential(powerId);
	}

	public static int GetPotential(int i)
	{
		switch (i)
		{
		case 0:
			return 1;
		case 1:
			return 7;
		case 2:
			return 12;
		case 3:
			return 5;
		default:
			return 0;
		}
	}

	public virtual void OnClick()
	{
		if ((bool)aniController && !isPlayingClickAnimation && !Popup.PopupSystem.Instance.IsShowingPopup())
		{
			aniController.SetTrigger("setClick");
			isPlayingClickAnimation = true;
			GameMain.main.EventCounter();
			GameMain.main.shuffleOrder++;
			Invoke(EndOfClickAnimationMethodName, 0.5f);
			SoundSFX.Play(SFXIndex.ClickBlock);
		}
	}

	private void EndOfClickAnimation()
	{
		isPlayingClickAnimation = false;
	}

	public void Swap(Side side)
	{
		if ((bool)parentSlot && (bool)parentSlot[side])
		{
			AnimationAssistant.main.SwapTwoItem(this, parentSlot[side].GetChip(), side);
		}
	}

	public void ParentRemove()
	{
		if ((bool)parentSlot)
		{
			parentSlot.chip = null;
			parentSlot = null;
		}
	}

	private void CommonCrush(bool enableCollect = true)
	{
		if (enableCollect && id >= 0)
		{          
            if (GameMain.main.IdNeedCollected(id))
            {
                GameMain.main.DecreaseCollectCandy(base.transform.position, id, collectDelayTime);
                isCollected = true;
            }
		}
	}

	public void CombineBomb(int toX, int toY)
	{
		Vector3 slotPosition = BoardManager.main.GetSlotPosition(toX, toY);
		isCombineBomb = true;
		//CommonCrush();
		base.transform.DOMove(slotPosition, combineBombDuration);
#if ENABLE_MEC
		Timing.RunCoroutine(WaitCombineBomb());
#else
        StartCoroutine(WaitCombineBomb());
#endif
    }

	public static float CombineBombDuration = 0.175f;

	private IEnumerator WaitCombineBomb()
	{
		ParentRemove();
#if ENABLE_MEC
        yield return Timing.WaitForSeconds(CombineBombDuration);
#else
		yield return new WaitForSeconds(CombineBombDuration);
#endif
		//ParentRemove();
		HideChip(enableCollect: true);
	}

//	private IEnumerator WaitCombineBomb()
//	{
//#if ENABLE_MEC
//        yield return Timing.WaitForSeconds(GameMain.BlockDropDelayTime);
//#else
//        yield return waitForCombineBomb;
//#endif
//        ParentRemove();
//		HideChip();
//	}

	public void DestroyChip(float delayTime, bool showScoreEffect = false)
	{
#if ENABLE_MEC
		Timing.RunCoroutine(waitDestroyDelay(delayTime, showScoreEffect));
#else
        StartCoroutine(waitDestroyDelay(delayTime, showScoreEffect));
#endif
    }

	private IEnumerator<float> waitDestroyDelay(float delayTime, bool showScoreEffect)
	{
#if ENABLE_MEC
		yield return Timing.WaitForSeconds(delayTime);
#else
        yield return delayTime;
#endif
        if (showScoreEffect && (bool)parentSlot)
		{
			BoardManager.main.SetScore(parentSlot.slot.x, parentSlot.slot.y, ScoreType.ChipCrushByItemBlock, id);
		}
		DestroyChip();
	}

	public void DestroyChip(int fromCrushId = -1, Side crushDir = Side.Null, bool ignoreBlockCrush = false)
	{
		if (!destroyable || destroying || !parentSlot || !parentSlot.slot)
		{
			return;
		}
		StopBounceEffect();
		if (!ignoreBlockCrush && (bool)parentSlot.slot.GetBlock() && !parentSlot.slot.GetBlock().destroying)
		{
			parentSlot.slot.GetBlock().BlockCrush(fromCrushId);
		}
		else
		{
			if (chipType == ChipType.BringDown)
			{
				return;
			}
			if (chipType == ChipType.CandyChip)
			{
				CandyChip candyChip = parentSlot.GetChip() as CandyChip;
				if ((bool)candyChip)
				{
					int x = candyChip.parentSlot.slot.x;
					int y = candyChip.parentSlot.slot.y;
					candyChip.CrushCandyChip();
					Transform transform = SpawnStringBlock.GetSpawnBlockObject(SpawnStringBlockType.SmallCandyMixEffect).transform;
					transform.transform.position = base.transform.position;
					transform.transform.localPosition += new Vector3(0f, 0f, -1f);
					SmallCandyMixEffect component = transform.GetComponent<SmallCandyMixEffect>();
					BoardManager.main.GetSlot(x, y).SetChip(component);
					switch (crushDir)
					{
					case Side.Null:
						crushDir = (Side)Random.Range(1, 5);
						break;
					case Side.Right:
					case Side.Left:
						crushDir = (Side)Random.Range(1, 3);
						break;
					case Side.Top:
					case Side.Bottom:
						crushDir = (Side)Random.Range(3, 5);
						break;
					}
					component.Rolling(crushDir);
				}
			}
			else if (chipType == ChipType.RainbowBomb)
			{
				Slot slot = parentSlot.slot;
				HideChip();
				Transform transform2 = SpawnStringBlock.GetSpawnBlockObject(SpawnStringBlockType.RainbowChipMixEffect).transform;
				RainbowChipMixEffect component2 = transform2.GetComponent<RainbowChipMixEffect>();
				slot.SetChip(component2);
				component2.canMove = false;
				component2.transform.position = base.transform.position;
				component2.targetId = fromCrushId;
				component2.StartMixEffect();
			}
			else
			{
				destroying = true;
				CommonCrush();
				swapDirection = SwapDiagonalType.None;
				StartCoroutine(DestroyChipFunction());
				SoundSFX.PlayCap(SFXIndex.BlockRemove);
			}
		}
	}

	public void HideChip(bool enableCollect = true)
	{
		if (!destroying)
		{
			destroying = true;
			CommonCrush(enableCollect);
			ParentRemove();
			PoolManager.PoolGameBlocks.Despawn(base.transform);
		}
	}

	public void Flashing(int eventCount)
	{
#if ENABLE_MEC
		Timing.RunCoroutine(FlashingUntil(eventCount));
#else
        StartCoroutine(FlashingUntil(eventCount));
#endif
    }

	private IEnumerator<float> FlashingUntil(int eventCount)
	{
		if (!(aniController == null))
		{
			aniController.SetBool("isHint", value: true);
			while (eventCount == GameMain.main.eventCount)
			{
#if ENABLE_MEC
				yield return Timing.WaitForOneFrame;
#else
                yield return 0.1f;
#endif
            }
			if (!this)
			{
				aniController.SetBool("isHint", value: false);
			}
			else
			{
				aniController.SetBool("isHint", value: false);
			}
		}
	}

	public void LookAtMe(int eventCount)
	{
#if ENABLE_MEC
		Timing.RunCoroutine(processLookAtMe(eventCount));
#else
        StartCoroutine(processLookAtMe(eventCount));
#endif
    }

	private IEnumerator<float> processLookAtMe(int eventCount)
	{
		Vector3 originScale = base.transform.localScale;
		Quaternion originRotation = base.transform.localRotation;
		Sequence sqnce = DOTween.Sequence();
		sqnce.Append(base.transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.3f));
		sqnce.Append(base.transform.DORotate(new Vector3(0f, 0f, -15f), 0.1f));
		sqnce.Append(base.transform.DORotate(new Vector3(0f, 0f, 15f), 0.2f).SetLoops(8, LoopType.Yoyo));
		sqnce.Append(base.transform.DORotate(new Vector3(0f, 0f, 0f), 0.1f));
		sqnce.Append(base.transform.DOScale(originScale, 0.3f));
		while (eventCount == GameMain.main.eventCount)
		{
#if ENABLE_MEC
            yield return Timing.WaitForOneFrame;
#else
            yield return 0.1f;
#endif
		}
		base.transform.localScale = originScale;
		base.transform.localRotation = originRotation;
		sqnce.Kill();
	}

	public void CloseToYou(int eventCount, Vector3 dir)
	{
		StartCoroutine(processCloseToYou(eventCount, dir));
	}

	private IEnumerator processCloseToYou(int eventCount, Vector3 dir)
	{
		Vector3 originScale = base.transform.localScale;
		Quaternion originRotation = base.transform.localRotation;
		Vector3 originPos = base.transform.localPosition;
		Vector3 targetPos = originPos + dir * 14f;
		GameMain.main.isLockDrop = true;
		Sequence sqnce = DOTween.Sequence();
		sqnce.Append(base.transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.3f));
		sqnce.Append(base.transform.DOLocalMove(targetPos, 0.5f).SetLoops(6, LoopType.Yoyo));
		sqnce.Append(base.transform.DOScale(originScale, 0.3f));
		while (eventCount == GameMain.main.eventCount)
		{
			yield return 0;
		}
		sqnce.Kill();
		GameMain.main.isLockDrop = false;
		base.transform.localScale = originScale;
		base.transform.localRotation = originRotation;
	}

	public virtual IEnumerator DestroyChipFunction()
	{
		yield break;
	}

	public void SetMutekiTime(float sec)
	{
		isMuteki = true;
		mutekiDurationTime = 0f;
		mutekiSetTime = sec;
	}
}
