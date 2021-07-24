using PathologicalGames;
using System.Collections;
using UnityEngine;

public class Dig : BlockInterface
{
	private Animator changeAnimator;

	private int eventCountBorn;

	public int level = 1;

	public override bool EnableBoosterHammer => true;

	public override bool EnableBoosterCandyCrane => false;

	public override bool EnableBoosterCandyPack => false;

	private Side FindRollingDirection(int x, int y)
	{
		Side[] array = new Side[2];
		if (x > 0)
		{
			array[0] = Side.Right;
		}
		else if (x < 0)
		{
			array[0] = Side.Left;
		}
		else
		{
			array[0] = Side.Null;
		}
		if (y > 0)
		{
			array[1] = Side.Top;
		}
		else if (y < 0)
		{
			array[1] = Side.Bottom;
		}
		else
		{
			array[1] = Side.Null;
		}
		if (Mathf.Abs(x) == Mathf.Abs(y))
		{
			if (Random.Range(0, 2) == 0)
			{
				return array[0];
			}
			return array[1];
		}
		return (Mathf.Abs(x) >= Mathf.Abs(y)) ? array[0] : array[1];
	}

	public override void Initialize()
	{
		slot.gravity = false;
		eventCountBorn = GameMain.main.eventCount;
		changeAnimator = GetComponent<Animator>();
	}

	public override void BlockCrush(int fromCrushId = -1, int subId = -1)
	{
		if (eventCountBorn != GameMain.main.eventCount)
		{
			eventCountBorn = GameMain.main.eventCount;
			level--;
			PlayCrushSound(blockType);
			AnimatorStateInfo currentAnimatorStateInfo = changeAnimator.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.IsName("Obstacle_D_normal_lv3_Candy") 
				|| currentAnimatorStateInfo.IsName("Obstacle_D_normal_lv2_Candy") 
				|| currentAnimatorStateInfo.IsName("Obstacle_D_normal_lv3")
				|| currentAnimatorStateInfo.IsName("Obstacle_D_normal_lv2"))
			{
				changeAnimator.SetTrigger("SetSkip");
			}
			else
			{
				changeAnimator.SetTrigger("SetLevelDown");
			}
			changeAnimator.SetTrigger("SetLevelDownComplete");
			if (level == 0 && !destroying)
			{
				destroying = true;
				StartCoroutine(BlockCrush_Coroutine());
			}
		}
	}

	private IEnumerator BlockCrush_Coroutine()
	{
		int sx = slot.x;
		int sy = slot.y;
		bool isPaintJelly = slot.IsPaintedJelly;
		int id = 0;
		switch (blockType)
		{
		case IBlockType.Digging_HP1_Collect:
		case IBlockType.Digging_HP2_Collect:
		case IBlockType.Digging_HP3_Collect:
		{
			GameObject objDiggingCollect = SpawnStringGamePlaying.GetSpawnObject(SpawnStringGamePlayingType.EffectDiggingCollect);
			objDiggingCollect.transform.position = base.transform.position;
			objDiggingCollect.transform.localScale = Vector3.one;
			//objDiggingCollect.GetComponentInChildren<SpriteRenderer>().gameObject.layer = base.gameObject.layer;
			yield return new WaitForSeconds(0.5f);

			//GameObject objTrail = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.BringDownEffectTrail);
			//float throwingTime = GameMain.main.CalculateThrowingTime(objTrail.transform.position, CPanelGameUI.Instance.GetCollectObjectGameCameraPosition(CollectBlockType.DiggingCandy));
			//PoolManager.PoolGameEffect.Despawn(objTrail.transform, throwingTime);
			//objTrail.transform.parent = objDiggingCollect.transform;
			//objTrail.transform.localPosition = Vector3.zero;
			//objDiggingCollect.GetComponentInChildren<SpriteRenderer>().gameObject.layer = objTrail.layer;

			GameMain.main.PrevThrowCollectItem(CollectBlockType.DiggingCandy);
			GameMain.main.ThrowCollectItem(objDiggingCollect, CollectBlockType.DiggingCandy, 0f, PoolManager.PoolGamePlaying);
			break;
		}
		case IBlockType.Digging_HP1_Bomb1:
		case IBlockType.Digging_HP2_Bomb1:
		case IBlockType.Digging_HP3_Bomb1:
			if (id != -1)
			{
				GameObject spawnEffectObjectSimpleBombCrush = SpawnStringEffect.GetSpawnEffectObjectSimpleBombCrush(id);
				spawnEffectObjectSimpleBombCrush.transform.position = base.transform.position;
				PoolManager.PoolGameEffect.Despawn(spawnEffectObjectSimpleBombCrush.transform, 1f);
				GameObject spawnEffectObjectSmallCandyBombMixRemove = SpawnStringEffect.GetSpawnEffectObjectSmallCandyBombMixRemove(id);
				spawnEffectObjectSmallCandyBombMixRemove.transform.position = base.transform.position;
				PoolManager.PoolGameEffect.Despawn(spawnEffectObjectSmallCandyBombMixRemove.transform, 1f);
			}
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					int x = sx + j;
					int y = sy + i;
					SlotClushDig(x, y, id, isPaintJelly);
				}
			}
			SlotClushDig(sx + 2, sy, id, isPaintJelly);
			SlotClushDig(sx - 2, sy, id, isPaintJelly);
			SlotClushDig(sx, sy + 2, id, isPaintJelly);
			SlotClushDig(sx, sy - 2, id, isPaintJelly);
			BoardManager.main.SlotCrush(sx, sy, radius: false, ScoreType.CrushItem3x3, includeCrushChip: false);
			break;
		case IBlockType.Digging_HP1_Bomb2:
		case IBlockType.Digging_HP2_Bomb2:
		case IBlockType.Digging_HP3_Bomb2:
			Match3GameUI.Instance.ThrowPurchasedItemEffectForDigging(SpawnStringEffectType.SuccessBuyMove, 5, BoardManager.main.GetSlotPosition(sx, sy));
			GameMain.main.IsGettingItem = true;
			break;
		case IBlockType.Digging_HP1_Bomb3:
		case IBlockType.Digging_HP2_Bomb3:
		case IBlockType.Digging_HP3_Bomb3:
		{
			int height = BoardManager.main.boardData.height;
			if (id != -1)
			{
				GameObject spawnEffectObjectVBombCrush = SpawnStringEffect.GetSpawnEffectObjectVBombCrush(id);
				spawnEffectObjectVBombCrush.transform.position = base.transform.position;
				PoolManager.PoolGameEffect.Despawn(spawnEffectObjectVBombCrush.transform, 1.2f);
			}
			for (int k = 1; k < height; k++)
			{
				int num = sy + k;
				int num2 = sy - k;
				if (num >= 0 && num < height)
				{
					BoardManager.main.SlotCrush(sx, num, radius: false, ScoreType.ChipCrushByItemBlock, includeCrushChip: true, 0, id, Side.Top, isPaintJelly);
				}
				if (num2 >= 0 && num2 < height)
				{
					BoardManager.main.SlotCrush(sx, num2, radius: false, ScoreType.ChipCrushByItemBlock, includeCrushChip: true, 0, id, Side.Bottom, isPaintJelly);
				}
			}
			BoardManager.main.SlotCrush(sx, sy, radius: false, ScoreType.CrushItem4x1, includeCrushChip: false);
			break;
		}
		case IBlockType.Digging_HP1_Treasure_G1:
		case IBlockType.Digging_HP2_Treasure_G1:
		case IBlockType.Digging_HP3_Treasure_G1:
		case IBlockType.Digging_HP1_Treasure_G2:
		case IBlockType.Digging_HP2_Treasure_G2:
		case IBlockType.Digging_HP3_Treasure_G2:
		case IBlockType.Digging_HP1_Treasure_G3:
		case IBlockType.Digging_HP2_Treasure_G3:
		case IBlockType.Digging_HP3_Treasure_G3:
		{
			SpawnStringGamePlayingType treasureGreadeEffectType = SpawnStringGamePlayingType.EffectDiggingCollectTreasureG3;
			switch (blockType)
			{
			case IBlockType.Digging_HP1_Treasure_G1:
			case IBlockType.Digging_HP2_Treasure_G1:
			case IBlockType.Digging_HP3_Treasure_G1:
				treasureGreadeEffectType = SpawnStringGamePlayingType.EffectDiggingCollectTreasureG1;
				break;
			case IBlockType.Digging_HP1_Treasure_G2:
			case IBlockType.Digging_HP2_Treasure_G2:
			case IBlockType.Digging_HP3_Treasure_G2:
				treasureGreadeEffectType = SpawnStringGamePlayingType.EffectDiggingCollectTreasureG2;
				break;
			case IBlockType.Digging_HP1_Treasure_G3:
			case IBlockType.Digging_HP2_Treasure_G3:
			case IBlockType.Digging_HP3_Treasure_G3:
				treasureGreadeEffectType = SpawnStringGamePlayingType.EffectDiggingCollectTreasureG3;
				break;
			}
			GameObject objCollect = SpawnStringGamePlaying.GetSpawnObject(treasureGreadeEffectType);
			objCollect.transform.position = base.transform.position;
			objCollect.transform.localScale = Vector3.one;
			//objCollect.GetComponentInChildren<SpriteRenderer>().gameObject.layer = base.gameObject.layer;
			yield return new WaitForSeconds(0.5f);

			//GameObject trail = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.BringDownEffectTrail);
			//float time = GameMain.main.CalculateThrowingTime(trail.transform.position, CPanelGameUI.Instance.GetCollectObjectGameCameraPosition(CollectBlockType.DiggingCandy));
			//PoolManager.PoolGameEffect.Despawn(trail.transform, time);
			//trail.transform.parent = objCollect.transform;
			//trail.transform.localPosition = Vector3.zero;
			//objCollect.GetComponentInChildren<SpriteRenderer>().gameObject.layer = trail.layer;

			CollectBlockType treasureCollectType = CollectBlockType.DiggingTreasure_G3;
			switch (blockType)
			{
			case IBlockType.Digging_HP1_Treasure_G1:
			case IBlockType.Digging_HP2_Treasure_G1:
			case IBlockType.Digging_HP3_Treasure_G1:
				treasureCollectType = CollectBlockType.DiggingTreasure_G1;
				break;
			case IBlockType.Digging_HP1_Treasure_G2:
			case IBlockType.Digging_HP2_Treasure_G2:
			case IBlockType.Digging_HP3_Treasure_G2:
				treasureCollectType = CollectBlockType.DiggingTreasure_G2;
				break;
			case IBlockType.Digging_HP1_Treasure_G3:
			case IBlockType.Digging_HP2_Treasure_G3:
			case IBlockType.Digging_HP3_Treasure_G3:
				treasureCollectType = CollectBlockType.DiggingTreasure_G3;
				break;
			}
			GameMain.main.PrevThrowCollectItem(treasureCollectType);
			GameMain.main.ThrowCollectItem(objCollect, treasureCollectType, 0f, PoolManager.PoolGamePlaying);
			break;
		}
		}
		Crush();
		slot.gravity = true;
		slot.SetBlock(null);
		SlotGravity.Reshading();
		UnityEngine.Object.Destroy(base.gameObject, 2f);
	}

	private void SlotClushDig(int x, int y, int id, bool isPaintJelly)
	{
		if (x >= 0 && x < MapData.MaxWidth && y >= 0 && y < MapData.MaxHeight)
		{
			BoardManager main = BoardManager.main;
			bool radius = false;
			ScoreType scoreType = ScoreType.ChipCrushByItemBlock;
			Side crushDir = FindRollingDirection(x, y);
			main.SlotCrush(x, y, radius, scoreType, includeCrushChip: true, 0, id, crushDir, isPaintJelly);
		}
	}

	private void PlayCrushSound(IBlockType type)
	{
		if (level == 2)
		{
			if (type == IBlockType.Digging_HP3_Bomb2)
			{
				SoundSFX.PlayCap(SFXIndex.Dig5Move);
			}
			else
			{
				SoundSFX.PlayCap(SFXIndex.DigBlock);
			}
		}
		else if (level == 1)
		{
			SoundSFX.PlayCap(SFXIndex.DigBlock);
		}
		else if (level == 0)
		{
			SoundSFX.PlayCap(SFXIndex.DigLastBlock);
		}
	}

	public override bool CanBeCrushedByNearSlot()
	{
		return true;
	}
}
