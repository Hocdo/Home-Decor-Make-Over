using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3;

public class CandyFactory : BlockInterface
{
	private int eventCountBorn;

	public bool completeBombCandyFactory = true;

	public int CreateCount = 3;

	private readonly List<GameObject> listObjDropEffect = new List<GameObject>();

	public int needCreateCandyCount;

	public int SpewCandyCount = 3;

	public int SpewCandyID = 5;

	public float SpewCandyMoveDuration = 0.4f;

	public override bool EnableBoosterHammer => true;

	public override bool EnableBoosterCandyCrane => false;

	public override bool EnableBoosterCandyPack => false;

	private void OnDestroy()
	{
		foreach (GameObject item in listObjDropEffect)
		{
			if ((bool)item && PoolManager.PoolGameEffect.IsSpawned(item.transform))
			{
				PoolManager.PoolGameEffect.Despawn(item.transform);
			}
		}
		listObjDropEffect.Clear();
	}

	public void CrushedByNearSlot()
	{
		if (!GameMain.main.isGameResult)
		{
			needCreateCandyCount--;
			if (needCreateCandyCount == 0)
			{
				animator.SetTrigger("InputJam");
				SoundSFX.PlayCap(SFXIndex.JamFilling);
				StartCoroutine(SpreadJam(isExplode: false));
			}
			else if (needCreateCandyCount > 0)
			{
				animator.SetTrigger("InputJam");
				SoundSFX.PlayCap(SFXIndex.JamFilling);
			}
		}
	}

	public IEnumerator SpreadJam(bool isExplode, bool beInterval = false)
	{
		int multiply = 1;
		if (isExplode)
		{
			needCreateCandyCount = 0;
			multiply = 2;
		}
		completeBombCandyFactory = false;
		if ((bool)animator)
		{
			animator.ResetTrigger("InputJam");
			animator.SetTrigger("ExplodeJam");
		}
		if (!GameMain.main || !BoardManager.main || !BoardManager.main.slotGroup)
		{
			yield break;
		}
		while (!GameMain.main.canBombCandyFactory)
		{
			yield return null;
		}
		yield return StartCoroutine(Utils.WaitFor(GameMain.main.CanIWait, 0.1f));
		yield return StartCoroutine(Utils.WaitFor(GameMain.main.CanISpewCandyFactory, 0.1f));
		if ((bool)animator)
		{
			animator.SetTrigger("EndWating");
		}
		GameMain.main.SpewingCandyFactoryCount++;
		SimpleChip[] allChips = BoardManager.main.slotGroup.GetComponentsInChildren<SimpleChip>();
		List<SimpleChip> changeableChips = new List<SimpleChip>();
		SimpleChip[] array = allChips;
		foreach (SimpleChip simpleChip in array)
		{
			if ((bool)simpleChip && simpleChip.id != SpewCandyID && simpleChip.parentSlot != null && simpleChip.parentSlot.slot != null && simpleChip.parentSlot.slot.GetBlock() == null && !simpleChip.parentSlot.slot.isSafeObs)
			{
				changeableChips.Add(simpleChip);
			}
		}
		yield return new WaitForSeconds(Random.Range(0.01f, 0.1f));
		Utils.Shuffle(changeableChips);
		Coroutine waitThisCoroutine = null;
		for (int i = 0; i < SpewCandyCount * multiply && i < changeableChips.Count; i++)
		{
			if ((bool)changeableChips[i] && (bool)changeableChips[i].parentSlot && (bool)changeableChips[i].parentSlot.slot)
			{
				Slot targetSlot = changeableChips[i].parentSlot.slot;
				waitThisCoroutine = StartCoroutine(ThrowJamDrop(targetSlot));
				if (beInterval)
				{
					yield return new WaitForSeconds(0.1f);
				}
			}
		}
		if (waitThisCoroutine != null)
		{
			yield return waitThisCoroutine;
		}
		GameMain.main.SpewingCandyFactoryCount--;
		needCreateCandyCount = CreateCount;
		yield return StartCoroutine(Utils.WaitFor(GameMain.main.CanIWait, 0.1f));
		if (needCreateCandyCount != 0)
		{
			completeBombCandyFactory = true;
		}
		GameMain.main.SetBestMovesNull();
	}

	private IEnumerator ThrowJamDrop(Slot targetSlot)
	{
		if (!targetSlot || !targetSlot.GetChip())
		{
			yield break;
		}
		Vector3 targetPos = targetSlot.transform.position;
		GameObject objDropEffect = SpawnStringEffect.GetSpawnEffectObjectJamShoot(SpewCandyID);
		listObjDropEffect.Add(objDropEffect);
		Vector3 vector = base.transform.position + new Vector3(0f, 30f, -1f);
		objDropEffect.transform.position = vector;
		Vector3 startPos = vector;
		SoundSFX.PlayCap(SFXIndex.JamThrowing);
		float elapse_time = 0f;
		while (elapse_time < 1f)
		{
			vector = base.transform.position;
			float x = (vector.x + targetPos.x) * 0.5f;
			float y = targetPos.y;
			Vector3 position = base.transform.position;
			float y2;
			if (y <= position.y)
			{
				Vector3 position2 = base.transform.position;
				y2 = position2.y + 300f;
			}
			else
			{
				y2 = targetPos.y + 300f;
			}
			Vector3 pointPos = new Vector3(x, y2, 0f);
			objDropEffect.transform.position = Utils.Bezier(elapse_time, startPos, pointPos, targetPos);
			elapse_time += Time.deltaTime;
			yield return null;
		}
		PoolManager.PoolGameEffect.Despawn(objDropEffect.transform, 0.5f);
		if ((bool)targetSlot)
		{
			GameObject spawnEffectObjectJamSpread = SpawnStringEffect.GetSpawnEffectObjectJamSpread(SpewCandyID);
			spawnEffectObjectJamSpread.transform.position = targetSlot.transform.position;
			PoolManager.PoolGameEffect.Despawn(spawnEffectObjectJamSpread.transform, 1.9f);
			SoundSFX.PlayCap(SFXIndex.JamDrop);
			Chip chip = targetSlot.GetChip();
			Chip newSimpleChip = BoardManager.main.GetNewSimpleChip(targetSlot.x, targetSlot.y, base.transform.position, SpewCandyID);
			newSimpleChip.transform.position = targetSlot.transform.position;
			if ((bool)chip && chip.gameObject.activeSelf)
			{
				PoolManager.PoolGameBlocks.Despawn(chip.gameObject.transform);
			}
		}
	}

	public void SpewCandyByBooster(BoosterType boosterType)
	{
		switch (boosterType)
		{
		case BoosterType.Hammer:
			StartCoroutine(SpreadJam(isExplode: true, beInterval: true));
			break;
		case BoosterType.CandyPack:
			StartCoroutine(SpreadJam(isExplode: true));
			break;
		}
	}

	public override void Initialize()
	{
		slot.gravity = false;
		needCreateCandyCount = CreateCount;
		eventCountBorn = GameMain.main.eventCount;
	}

	public override void BlockCrush(int fromCrushId = -1, int subId = -1)
	{
		if (eventCountBorn != GameMain.main.eventCount)
		{
			eventCountBorn = GameMain.main.eventCount;
			CrushedByNearSlot();
		}
	}

	public override bool CanBeCrushedByNearSlot()
	{
		return true;
	}
}
