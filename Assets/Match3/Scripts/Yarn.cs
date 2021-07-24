using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yarn : BlockInterface
{
	public int clothButtonLevel;

	private readonly Dictionary<int, GameObject> dicClothButton = new Dictionary<int, GameObject>();

	public Dictionary<Side, Yarn> dicYarn = new Dictionary<Side, Yarn>();

	public Dictionary<Side, GameObject> dicYarnImg = new Dictionary<Side, GameObject>();

	public override bool EnableBoosterHammer
	{
		get
		{
			if (clothButtonLevel > 0)
			{
				return true;
			}
			return false;
		}
	}

	public override bool EnableBoosterCandyCrane => false;

	public override bool EnableBoosterCandyPack => false;

	public override void BlockCrush(int fromCrushId = -1, int subId = -1)
	{
		if (clothButtonLevel > 0)
		{
			if (dicClothButton.ContainsKey(clothButtonLevel))
			{
				UnityEngine.Object.Destroy(dicClothButton[clothButtonLevel]);
				dicClothButton.Remove(clothButtonLevel);
			}
			clothButtonLevel--;
			StartCoroutine(ProcessCrushClothButton());
		}
	}

	public override bool CanBeCrushedByNearSlot()
	{
		if (clothButtonLevel > 0)
		{
			return true;
		}
		return false;
	}

	public override void Initialize()
	{
		slot.gravity = false;
	}

	public void CreateYarn(Side side)
	{
		if (!dicYarnImg.ContainsKey(side))
		{
			GameObject item = ContentAssistant.main.GetItem("YarnImg");
			item.transform.SetParent(base.transform);
			switch (side)
			{
			case Side.Top:
				item.transform.localPosition = new Vector3(0f, 17f, 0f);
				item.transform.localRotation = Quaternion.Euler(0f, 0f, 270f);
				break;
			case Side.Bottom:
				item.transform.localPosition = new Vector3(0f, -17f, 0f);
				item.transform.localRotation = Quaternion.Euler(0f, 0f, 270f);
				break;
			case Side.Left:
				item.transform.localPosition = new Vector3(-17f, 0f, 0f);
				break;
			case Side.Right:
				item.transform.localPosition = new Vector3(17f, 0f, 0f);
				break;
			}
			dicYarnImg.Add(side, item);
		}
	}

	public void SetSideYarn(Side side, Yarn yarn)
	{
		dicYarn.Add(side, yarn);
	}

	public void CreateClothButton(int level)
	{
		if (level == 0 || level > 3)
		{
			return;
		}
		clothButtonLevel = level;
		if (!dicClothButton.ContainsKey(clothButtonLevel))
		{
			GameObject item = ContentAssistant.main.GetItem("ribbon_0" + level + "_before");
			if ((bool)item)
			{
				dicClothButton[level] = item;
				item.transform.parent = base.transform;
				item.transform.localPosition = Vector3.zero;
				item.transform.SetAsLastSibling();
			}
		}
	}

	private IEnumerator ProcessCrushClothButton()
	{
		SoundSFX.PlayCap(SFXIndex.ClothButtonBreak);
		float aniTime = (clothButtonLevel != 0) ? 1f : 1.5f;
		GameObject crushEffect = SpawnStringEffect.GetSpawnEffectObject((SpawnStringEffectType)(128 + clothButtonLevel));
		if ((bool)crushEffect)
		{
			crushEffect.transform.parent = base.gameObject.transform.parent;
			crushEffect.transform.localPosition = new Vector3(0f, 0f, -10f);
			crushEffect.transform.SetAsLastSibling();
			PoolManager.PoolGameEffect.Despawn(crushEffect.transform, aniTime);
		}
		float yarnCrushWait = 0f;
		if (clothButtonLevel == 0)
		{
			yarnCrushWait = 0.5f;
			yield return new WaitForSeconds(yarnCrushWait);
			SoundSFX.PlayCap(SFXIndex.ClothButtonBreakFinal);
			Side[] straightSides = Utils.straightSides;
			foreach (Side side in straightSides)
			{
				RemoveYarnPath(side);
			}
		}
		yield return new WaitForSeconds(aniTime - yarnCrushWait);
		if (clothButtonLevel > 0)
		{
			CreateClothButton(clothButtonLevel);
		}
	}

	public void CrushClothButtonAllLevel()
	{
		StartCoroutine(ProcessCrushClothButtonAllLevel());
	}

	private IEnumerator ProcessCrushClothButtonAllLevel()
	{
		foreach (GameObject value in dicClothButton.Values)
		{
			UnityEngine.Object.Destroy(value);
		}
		dicClothButton.Clear();
		float aniTime = (clothButtonLevel != 3) ? 1.5f : 1.433f;
		SpawnStringEffectType type = (clothButtonLevel == 1) ? SpawnStringEffectType.RemoveClothButtonHP1 : ((clothButtonLevel != 2) ? SpawnStringEffectType.RemoveClothButtonHP3Finish : SpawnStringEffectType.RemoveClothButtonHP2Finish);
		GameObject crushEffect = SpawnStringEffect.GetSpawnEffectObject(type);
		if ((bool)crushEffect)
		{
			crushEffect.transform.parent = base.gameObject.transform.parent;
			crushEffect.transform.localPosition = new Vector3(0f, 0f, -10f);
			crushEffect.transform.SetAsLastSibling();
			PoolManager.PoolGameEffect.Despawn(crushEffect.transform, aniTime);
		}
		clothButtonLevel = 0;
		yield return new WaitForSeconds(0.5f);
		SoundSFX.PlayCap(SFXIndex.ClothButtonBreakFinal);
		yield return new WaitForSeconds(0.2f);
		DestroyBlock();
	}

	public void RemoveYarnPath(Side side)
	{
		StartCoroutine(ProcessRemoveYarnPath(side));
	}

	public IEnumerator ProcessRemoveYarnPath(Side side)
	{
		Side mirrorSide = Utils.MirrorSide(side);
		yield return new WaitForSeconds(0.1f);
		if (dicYarnImg.ContainsKey(side) && (bool)dicYarn[side])
		{
			GameObject spawnEffectObject = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.RemoveYarn);
			if ((bool)spawnEffectObject)
			{
				spawnEffectObject.transform.position = dicYarnImg[side].transform.position;
				PoolManager.PoolGameEffect.Despawn(spawnEffectObject.transform, 5f);
			}
			SoundSFX.PlayCap(SFXIndex.YarnBreak);
			UnityEngine.Object.Destroy(dicYarnImg[side]);
			dicYarnImg.Remove(side);
		}
		yield return new WaitForSeconds(0.1f);
		if (dicYarn.ContainsKey(side) && dicYarn[side] != null && dicYarn[side].dicYarnImg != null && dicYarn[side].dicYarnImg.ContainsKey(mirrorSide) && dicYarn[side].dicYarnImg[mirrorSide] != null)
		{
			GameObject spawnEffectObject2 = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.RemoveYarn);
			if ((bool)spawnEffectObject2)
			{
				spawnEffectObject2.transform.position = dicYarn[side].dicYarnImg[mirrorSide].transform.position;
				PoolManager.PoolGameEffect.Despawn(spawnEffectObject2.transform, 5f);
			}
			SoundSFX.PlayCap(SFXIndex.YarnBreak);
			UnityEngine.Object.Destroy(dicYarn[side].dicYarnImg[mirrorSide]);
			dicYarn[side].dicYarnImg.Remove(mirrorSide);
		}
		if (!dicYarn.ContainsKey(side))
		{
			yield break;
		}
		DestroyBlock();
		if (!dicYarn[side])
		{
			yield break;
		}
		if (dicYarn[side].clothButtonLevel == 0)
		{
			if ((bool)dicYarn[side])
			{
				dicYarn[side].RemoveYarnPath(side);
			}
		}
		else if (dicYarn[side].dicYarnImg != null && dicYarn[side].dicYarnImg.Count == 0)
		{
			dicYarn[side].CrushClothButtonAllLevel();
		}
		else
		{
			dicYarn[side].dicYarn.Remove(mirrorSide);
		}
	}

	private void DestroyBlock()
	{
		if (!destroying)
		{
			destroying = true;
			Crush();
			slot.gravity = true;
			slot.SetBlock(null);
			SlotGravity.Reshading();
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
