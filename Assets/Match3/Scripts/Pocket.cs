using PathologicalGames;
using System.Collections;
using UnityEngine;

public class Pocket : BlockInterface
{
	public override bool EnableBoosterHammer => true;

	public override bool EnableBoosterCandyCrane => false;

	public override bool EnableBoosterCandyPack => true;

	public override void Awake()
	{
		base.Awake();
		blockType = IBlockType.Pocket;
	}

	public override void Initialize()
	{
		slot.gravity = false;
	}

	public override void BlockCrush(int fromCrushId = -1, int subId = -1)
	{
	}

	public override bool CanBeCrushedByNearSlot()
	{
		return true;
	}

	public void GoalIn(Side direction)
	{
		StartCoroutine(ProcessGoalInEffect(direction));
	}

	private IEnumerator ProcessGoalInEffect(Side direction)
	{
		GameMain.main.PrevThrowCollectItem(CollectBlockType.PocketCandy);
		GameObject objGoalEffect = null;
		switch (direction)
		{
		case Side.Left:
			objGoalEffect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.PocketGoalInRight);
			break;
		case Side.Right:
			objGoalEffect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.PocketGoalInLeft);
			break;
		case Side.Top:
			objGoalEffect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.PocketGoalInBottom);
			break;
		case Side.Bottom:
			objGoalEffect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.PocketGoalInTop);
			break;
		}
		objGoalEffect.transform.position = base.transform.position;
		PoolManager.PoolGameEffect.Despawn(objGoalEffect.transform, 1.667f);
		Vector3 targetPos = Match3GameUI.Instance.GetCollectObjectGameCameraPosition(CollectBlockType.PocketCandy);
		SoundSFX.PlayCap(SFXIndex.PocketballGoalIn);
		GameObject collectEffect = SpawnStringGamePlaying.GetSpawnObject(SpawnStringGamePlayingType.PocketCollect);
		collectEffect.transform.position = base.transform.position;
		collectEffect.transform.localScale = Vector3.one;
		float throwingTime = GameMain.main.CalculateThrowingTime(collectEffect.transform.position, targetPos);
		SoundSFX.PlayCapDelay(this, throwingTime - 0.15f, SFXIndex.PocketballReachedTarget);
		GameMain.main.ThrowCollectItem(collectEffect, CollectBlockType.PocketCandy, 0.1f, PoolManager.PoolGamePlaying);
		yield return new WaitForSeconds(throwingTime);
		GameObject collectEffectFinish = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.PocketCollectFinish);
		collectEffectFinish.transform.position = targetPos;
		collectEffectFinish.transform.localScale = Vector3.one;
		PoolManager.PoolGameEffect.Despawn(collectEffectFinish.transform, 0.8f);
	}
}
