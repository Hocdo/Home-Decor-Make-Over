using PathologicalGames;
using System.Collections;
using UnityEngine;

public class BringDownChip : Chip
{
	public enum BringDownSpriteType
	{
		Hat,
		Bag,
		Shoes
	}

	public BringDownSpriteType spriteType;

	public override void Awake()
	{
		base.Awake();
		Utils.EnableAllSpriteRenderer(base.gameObject);
		chipType = ChipType.BringDown;
		destroyable = false;
	}

	public override void Update()
	{
		base.Update();
		if (!destroying && !move && (bool)parentSlot && GameMain.main.isPlaying && !AnimationAssistant.main.Swaping && parentSlot.slot.bringDownEndSlot)
		{
			destroying = true;
			StartCoroutine(CollectChip());
		}
	}

	private IEnumerator CollectChip()
	{
		GameMain.main.targetBringDownRemainCount--;
		GameMain.main.PrevThrowCollectItem(CollectBlockType.BringDown);
		canMove = false;
		base.IsMatching = true;
		Utils.DisableAllSpriteRenderer(base.gameObject);
		GameObject objLight = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.BringDownEffectLight);
		objLight.transform.position = parentSlot.transform.position;
		GameObject objBringDownSwallow = SpawnStringEffect.GetSpawnEffectObjectBringDownSwallow((int)spriteType);
		objBringDownSwallow.transform.position = parentSlot.transform.position;
		objBringDownSwallow.transform.localScale = Chip.baseScale;
		//objBringDownSwallow.GetComponentInChildren<SpriteRenderer>().gameObject.layer = base.gameObject.layer;
		objBringDownSwallow.GetComponentInChildren<SpriteRenderer>().color = Color.white;
		yield return new WaitForSeconds(0.5f);
		canMove = true;
		PoolManager.PoolGameEffect.Despawn(objLight.transform);
		ParentRemove();
		yield return null;
		//GameObject objTrail = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.BringDownEffectTrail);
		//objTrail.transform.parent = objBringDownSwallow.transform;
		//objTrail.transform.localPosition = Vector3.zero;
		////float throwingTime = GameMain.main.CalculateThrowingTime(objTrail.transform.position, CPanelGameUI.Instance.GetCollectObjectGameCameraPosition(CollectBlockType.BringDown));
		float throwingTime = 1f;
		//PoolManager.PoolGameEffect.Despawn(objTrail.transform, throwingTime);
		//objBringDownSwallow.GetComponentInChildren<SpriteRenderer>().gameObject.layer = objTrail.layer;
		GameMain.main.ThrowCollectItem(objBringDownSwallow, CollectBlockType.BringDown, 0f, PoolManager.PoolGameEffect);
		PoolManager.PoolGameBlocks.Despawn(base.transform, throwingTime);
	}
}
