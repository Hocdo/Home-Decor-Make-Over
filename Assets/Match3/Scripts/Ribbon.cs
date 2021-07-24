using DG.Tweening;
using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ribbon : BlockInterface
{
	public bool CanRemoveRibbon;

	public bool checkRemove;

	public string curRibbonName;

	private readonly Dictionary<int, GameObject> dicKnot = new Dictionary<int, GameObject>();

	public int knotLevel;

	private AudioSource loopSound;

	public Ribbon nextRibbon;

	public Ribbon prevRibbon;

	public Side startDir;

	public override bool EnableBoosterHammer
	{
		get
		{
			if (knotLevel > 0)
			{
				return true;
			}
			return false;
		}
	}

	public override bool EnableBoosterCandyCrane => false;

	public override bool EnableBoosterCandyPack => false;

	public void Initialize(string ribbonName)
	{
		slot.gravity = false;
		prevRibbon = (nextRibbon = null);
		curRibbonName = ribbonName;
	}

	public Ribbon GetOtherRibbon(Ribbon startRibbon)
	{
		if (prevRibbon != startRibbon)
		{
			return prevRibbon;
		}
		if (nextRibbon != startRibbon)
		{
			return nextRibbon;
		}
		return null;
	}

	public void CreateKnot(int level, string ribbonName)
	{
		if (level == 0 || level > 3)
		{
			return;
		}
		knotLevel = level;
		curRibbonName = ribbonName;
		if (!dicKnot.ContainsKey(knotLevel))
		{
			GameObject item = ContentAssistant.main.GetItem("Obstacle_Knot_" + level);
			if ((bool)item)
			{
				dicKnot[level] = item;
				item.transform.parent = base.transform;
				item.transform.localPosition = Vector3.zero;
				item.transform.SetAsLastSibling();
				RotateKnot(item);
			}
		}
	}

	private IEnumerator RemoveKnotEffect()
	{
		if (knotLevel == 0 && BoardManager.main.CheckCanRemoveRibbonBlock(slot.x, slot.y, this))
		{
			BoardManager.main.ReplaceRibbonLink(slot.x, slot.y, this);
			SpriteRenderer renderer = GetComponent<SpriteRenderer>();
			if ((bool)renderer)
			{
				renderer.enabled = false;
			}
			GameObject effect2 = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.RemoveRibbonStart);
			if ((bool)effect2)
			{
				effect2.transform.parent = base.gameObject.transform.parent;
				effect2.transform.localPosition = new Vector3(0f, 0f, -10f);
				effect2.transform.SetAsLastSibling();
				RotateKnot(effect2);
				PoolManager.PoolGameEffect.Despawn(effect2.transform, 1.167f);
			}
			SoundSFX.PlayCap(SFXIndex.RibbonCut);
			yield return new WaitForSeconds(1.167f);
			RemoveNextRibbon();
			RemoveCurrentRibbon();
		}
		else
		{
			float aniTime = (knotLevel != 0) ? 0.75f : 1f;
			GameObject effect = SpawnStringEffect.GetSpawnEffectObject((SpawnStringEffectType)(125 + knotLevel));
			if ((bool)effect)
			{
				effect.transform.parent = base.gameObject.transform.parent;
				effect.transform.localPosition = new Vector3(0f, 0f, -10f);
				effect.transform.SetAsLastSibling();
				RotateKnot(effect);
				PoolManager.PoolGameEffect.Despawn(effect.transform, aniTime);
			}
			SoundSFX.PlayCap(SFXIndex.RibbonCut);
			yield return new WaitForSeconds(aniTime);
			if (knotLevel > 0)
			{
				CreateKnot(knotLevel, curRibbonName);
			}
		}
	}

	private IEnumerator RemoveRibbonEffect()
	{
		SpriteRenderer renderer = GetComponent<SpriteRenderer>();
		if ((bool)renderer)
		{
			renderer.enabled = false;
		}
		float aniTime = 0.233f;
		GameObject ribbonEffect;
		if (curRibbonName.Equals("H") || curRibbonName.Equals("V"))
		{
			if (prevRibbon == null && nextRibbon == null)
			{
				ribbonEffect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.RemoveRibbonEnd);
				aniTime = 0.5f;
			}
			else
			{
				ribbonEffect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.RemoveRibbonHV);
			}
		}
		else
		{
			ribbonEffect = (curRibbonName.Contains("S") ? SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.RemoveRibbonEdge) : (((!curRibbonName.Equals("BL") || startDir != Side.Bottom) && (!curRibbonName.Equals("TL") || startDir != Side.Left) && (!curRibbonName.Equals("TR") || startDir != Side.Top) && (!curRibbonName.Equals("BR") || startDir != Side.Right)) ? SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.RemoveRibbonCorner) : SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.RemoveRibbonCornerReverse)));
		}
		if ((bool)ribbonEffect)
		{
			ribbonEffect.transform.parent = base.gameObject.transform.parent;
			ribbonEffect.transform.localPosition = new Vector3(0f, 0f, -10f);
			ribbonEffect.transform.SetAsLastSibling();
			RotateRibbonEffect(ribbonEffect);
			PoolManager.PoolGameEffect.Despawn(ribbonEffect.transform, aniTime);
		}
		if ((bool)prevRibbon || (bool)nextRibbon)
		{
			GameObject rollerEffect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.RemoveRibbonRolling);
			if ((bool)rollerEffect)
			{
				rollerEffect.transform.parent = base.gameObject.transform.parent;
				rollerEffect.transform.localPosition = new Vector3(0f, 0f, -10f);
				rollerEffect.transform.SetAsLastSibling();
				RotateRolller(rollerEffect);
				PoolManager.PoolGameEffect.Despawn(rollerEffect.transform, aniTime);
				DoMoveRoller(rollerEffect, aniTime);
			}
		}
		loopSound = SoundSFX.Play(SFXIndex.RibbonRoll, loop: true);
		yield return new WaitForSeconds(aniTime);
		if ((bool)loopSound)
		{
			loopSound.Stop();
		}
		if (curRibbonName.Contains("S"))
		{
			GameObject spawnEffectObject = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.RemoveRibbonRollingEnd);
			if ((bool)spawnEffectObject)
			{
				spawnEffectObject.transform.parent = base.gameObject.transform.parent;
				SetRollerEndPosition(spawnEffectObject);
				spawnEffectObject.transform.SetAsLastSibling();
				RotateRolller(spawnEffectObject);
				PoolManager.PoolGameEffect.Despawn(spawnEffectObject.transform, 0.5f);
			}
			RemoveCurrentRibbon();
		}
		else
		{
			RemoveNextRibbon();
			RemoveCurrentRibbon();
		}
	}

	private void RemoveNextRibbon()
	{
		if ((bool)prevRibbon && !prevRibbon.checkRemove)
		{
			prevRibbon.startDir = GetDirectionStartEffect(prevRibbon.slot);
			if (BoardManager.main.CheckCanRemoveRibbonBlock(slot.x, slot.y, this))
			{
				prevRibbon.CanRemoveRibbon = true;
			}
			prevRibbon.BlockCrush();
			prevRibbon.checkRemove = true;
		}
		if ((bool)nextRibbon && !nextRibbon.checkRemove)
		{
			nextRibbon.startDir = GetDirectionStartEffect(nextRibbon.slot);
			if (BoardManager.main.CheckCanRemoveRibbonBlock(slot.x, slot.y, this))
			{
				nextRibbon.CanRemoveRibbon = true;
			}
			nextRibbon.BlockCrush();
			nextRibbon.checkRemove = true;
		}
	}

	private void RemoveCurrentRibbon()
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

	private Side GetDirectionStartEffect(Slot target)
	{
		if (slot.x > target.x)
		{
			return Side.Right;
		}
		if (slot.x < target.x)
		{
			return Side.Left;
		}
		if (slot.y > target.y)
		{
			return Side.Top;
		}
		if (slot.y < target.y)
		{
			return Side.Bottom;
		}
		return Side.Null;
	}

	private void RotateKnot(GameObject obj)
	{
		if (curRibbonName.Contains("H"))
		{
			obj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		}
		else if (curRibbonName.Contains("V"))
		{
			obj.transform.localRotation = Quaternion.Euler(0f, 0f, 270f);
		}
	}

	private void RotateRolller(GameObject obj)
	{
		if (startDir == Side.Right)
		{
			obj.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
		}
		if (startDir == Side.Left)
		{
			obj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		}
		if (startDir == Side.Top)
		{
			obj.transform.localRotation = Quaternion.Euler(0f, 180f, 270f);
		}
		if (startDir == Side.Bottom)
		{
			obj.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
		}
	}

	private void DoMoveRoller(GameObject effect, float duration)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		Vector3 endValue = default(Vector3);
		if (curRibbonName.Equals("H") || curRibbonName.Equals("V") || curRibbonName.Contains("S"))
		{
			if (startDir == Side.Right)
			{
				num = 34;
				num2 = 0;
			}
			else if (startDir == Side.Left)
			{
				num = -34;
				num2 = 0;
			}
			else if (startDir == Side.Top)
			{
				num = 0;
				num2 = 34;
			}
			else if (startDir == Side.Bottom)
			{
				num = 0;
				num2 = -34;
			}
		}
		else if (curRibbonName.Equals("BL"))
		{
			num3 = -34;
			num4 = -34;
			if (startDir == Side.Left)
			{
				num = 0;
				num2 = 34;
				endValue = new Vector3(0f, 0f, 270f);
			}
			if (startDir == Side.Bottom)
			{
				num = 34;
				num2 = 0;
				endValue = new Vector3(0f, 0f, 90f);
			}
		}
		else if (curRibbonName.Equals("TL"))
		{
			num3 = -34;
			num4 = 34;
			if (startDir == Side.Left)
			{
				num = 0;
				num2 = -34;
				endValue = new Vector3(0f, 0f, 90f);
			}
			if (startDir == Side.Top)
			{
				num = 34;
				num2 = 0;
				endValue = new Vector3(0f, 0f, 270f);
			}
		}
		else if (curRibbonName.Equals("TR"))
		{
			num3 = 34;
			num4 = 34;
			if (startDir == Side.Right)
			{
				num = 0;
				num2 = -34;
				endValue = new Vector3(0f, 0f, 270f);
			}
			if (startDir == Side.Top)
			{
				num = -34;
				num2 = 0;
				endValue = new Vector3(0f, 0f, 90f);
			}
		}
		else if (curRibbonName.Equals("BR"))
		{
			num3 = 34;
			num4 = -34;
			if (startDir == Side.Right)
			{
				num = 0;
				num2 = 34;
				endValue = new Vector3(0f, 0f, 90f);
			}
			if (startDir == Side.Bottom)
			{
				num = -34;
				num2 = 0;
				endValue = new Vector3(0f, 0f, 270f);
			}
		}
		if (!curRibbonName.Equals("H") && !curRibbonName.Equals("V") && !curRibbonName.Contains("S"))
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "PivotParent";
			gameObject.transform.SetParent(base.gameObject.transform);
			gameObject.transform.localPosition = new Vector3(num3, num4, 0f);
			gameObject.transform.localScale = Vector3.one;
			effect.transform.SetParent(gameObject.transform);
			effect.transform.localPosition = new Vector3(num, num2, -10f);
			gameObject.transform.DOLocalRotate(endValue, duration).SetEase(Ease.Linear);
		}
		else
		{
			effect.transform.localPosition = new Vector3(num, num2, -10f);
			effect.transform.DOLocalMove(new Vector3(-num, -num2, 0f), duration).SetEase(Ease.Linear);
		}
	}

	private void SetRollerEndPosition(GameObject endEffect)
	{
		int num = 0;
		int num2 = 0;
		if (startDir == Side.Right)
		{
			num = -34;
			num2 = 0;
		}
		else if (startDir == Side.Left)
		{
			num = 34;
			num2 = 0;
		}
		else if (startDir == Side.Top)
		{
			num = 0;
			num2 = -34;
		}
		else if (startDir == Side.Bottom)
		{
			num = 0;
			num2 = 34;
		}
		endEffect.transform.localPosition = new Vector3(num, num2, 0f);
	}

	private void RotateRibbonEffect(GameObject obj)
	{
		if (curRibbonName.Equals("H"))
		{
			if (startDir == Side.Right)
			{
				obj.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
			}
			if (startDir == Side.Left)
			{
				obj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			}
		}
		else if (curRibbonName.Equals("V"))
		{
			if (startDir == Side.Top)
			{
				obj.transform.localRotation = Quaternion.Euler(0f, 0f, 270f);
			}
			if (startDir == Side.Bottom)
			{
				obj.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
			}
		}
		else if (curRibbonName.Equals("BL"))
		{
			obj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		}
		else if (curRibbonName.Equals("TL"))
		{
			obj.transform.localRotation = Quaternion.Euler(0f, 0f, 270f);
		}
		else if (curRibbonName.Equals("TR"))
		{
			obj.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
		}
		else if (curRibbonName.Equals("BR"))
		{
			obj.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
		}
		else if (curRibbonName.Equals("ST"))
		{
			obj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		}
		else if (curRibbonName.Equals("SR"))
		{
			obj.transform.localRotation = Quaternion.Euler(0f, 0f, 270f);
		}
		else if (curRibbonName.Equals("SB"))
		{
			obj.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
		}
		else if (curRibbonName.Equals("SL"))
		{
			obj.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
		}
	}

	public override void Initialize()
	{
	}

	public override void BlockCrush(int fromCrushId = -1, int subId = -1)
	{
		if (knotLevel == 0 && CanRemoveRibbon)
		{
			CanRemoveRibbon = false;
			StartCoroutine(RemoveRibbonEffect());
		}
		if (knotLevel > 0)
		{
			if (dicKnot.ContainsKey(knotLevel))
			{
				UnityEngine.Object.Destroy(dicKnot[knotLevel]);
				dicKnot.Remove(knotLevel);
			}
			knotLevel--;
			StartCoroutine(RemoveKnotEffect());
		}
	}

	public override bool CanBeCrushedByNearSlot()
	{
		if (knotLevel > 0)
		{
			return true;
		}
		return false;
	}
}
