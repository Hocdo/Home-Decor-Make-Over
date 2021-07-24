using PathologicalGames;
using System.Collections;
using UnityEngine;

public class SimpleChip : Chip
{
	private static readonly Vector3 CrushEffectOffsetPosition = new Vector3(0f, -32.5f, 0f);

	private readonly float chameleonChangeDelay = 1f;

	private bool haveOCCToken;

	public bool increaseMoveCount;

	private GameObject ObjChameleonIdle;

	public GameObject OCCToken;

	private SimpleChipSubType subType;

	public GameObject glow;

	public override void Awake()
	{
		base.Awake();
		chipType = ChipType.SimpleChip;
		subType = SimpleChipSubType.None;
		haveOCCToken = false;
	}

	public override void OnDespawned()
	{
		base.OnDespawned();
		subType = SimpleChipSubType.None;
		if ((bool)ObjChameleonIdle)
		{
			ObjChameleonIdle.transform.parent = PoolManager.PoolGameEffect.transform;
			if (PoolManager.PoolGameEffect.IsSpawned(ObjChameleonIdle.transform))
			{
				PoolManager.PoolGameEffect.Despawn(ObjChameleonIdle.transform);
			}
			ObjChameleonIdle = null;
		}
	}

	public override void PlayIdleAnimation(AnimationController.IdleAnimationType type, AnimationProperty prop)
	{
		if ((bool)ObjChameleonIdle && ObjChameleonIdle.GetComponent<Animation>() != null)
		{
			ObjChameleonIdle.GetComponent<Animation>().Play();
		}
	}

	public void SetPowerupChameleon()
	{
		subType = SimpleChipSubType.Chameleon;
		aniType = AnimationController.IdleAnimationType.Chameleon;
		ObjChameleonIdle = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.ChameleonChipIdle);
		if ((bool)ObjChameleonIdle)
		{
			ObjChameleonIdle.transform.parent = base.transform;
			ObjChameleonIdle.transform.localPosition = Vector3.zero;
			ObjChameleonIdle.transform.eulerAngles = new Vector3(-20f, 0f, 0f);
			ObjChameleonIdle.transform.localScale = new Vector3(35f, 35f, 35f);
			if (!MonoSingleton<AnimationController>.Instance.IsRegistedLoopAnimation(AnimationController.IdleAnimationType.Chameleon))
			{
				ObjChameleonIdle.GetComponent<Animation>().Stop();
			}
		}
	}

	public void AttachOCCToken()
	{
	}

	public override IEnumerator DestroyChipFunction()
	{
		canMove = false;
		base.IsMatching = true;
		if (increaseMoveCount)
		{
			GameMain.main.IncreaseMoveCount(5);
		}
		if (isBouncePlaying)
		{
			StopBounceEffect();
		}
		Utils.DisableAllSpriteRenderer(base.gameObject);

        if (isCollected == false)
        {
            GameObject crushEffect = SpawnStringEffect.GetSpawnEffectObjectSimpleChipCrush(id);
            crushEffect.transform.position = base.transform.position;
            PoolManager.PoolGameEffect.Despawn(crushEffect.transform, 1f);
        }

        isCollected = false;

        yield return new WaitForSeconds(GameMain.BlockDropDelayTime);
		canMove = false;
		ParentRemove();
		base.IsMatching = false;

		PoolManager.PoolGameBlocks.Despawn(base.transform);
	}

	public virtual void DoDynamic()
	{
		if (subType == SimpleChipSubType.Chameleon)
		{
			StartCoroutine(ChameleonChangeRandomColor());
		}
	}

	private IEnumerator ChameleonChangeRandomColor()
	{
		GameMain.main.chameleonColorChanged++;
		GameObject objChangeEffect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.ChameleonChipAction);
		if ((bool)objChangeEffect)
		{
			objChangeEffect.transform.position = base.transform.position + new Vector3(0f, -22f, 0f);
			objChangeEffect.transform.eulerAngles = new Vector3(0f, 180f, 0f);
			PoolManager.PoolGameEffect.Despawn(objChangeEffect.transform, chameleonChangeDelay);
		}
		SoundSFX.PlayCap(SFXIndex.ChameleonChangeColor);
		yield return new WaitForSeconds(chameleonChangeDelay - 0.4f);
		int newColor = BoardManager.main.boardData.GetRandomChipExceptOneColor(id);
		if ((bool)parentSlot && (bool)parentSlot.slot)
		{
			BoardManager.main.AddPowerup(parentSlot.slot.x, parentSlot.slot.y, Powerup.Chameleon, newColor);
		}
	}
}