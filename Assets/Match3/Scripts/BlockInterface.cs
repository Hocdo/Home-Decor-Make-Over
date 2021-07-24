using PathologicalGames;
using UnityEngine;

public abstract class BlockInterface : AnimationObject
{
	public void SetVisualization(CharacterController character)
	{
		character.center = new Vector3(5f, 5f, 5f);
	}
	
	public IBlockType blockType;

	public bool destroying;

	[SerializeField]
	protected GameObject PrefabCrushPoolObject;

	public Slot slot;

	public abstract bool EnableBoosterHammer
	{
		get;
	}

	public abstract bool EnableBoosterCandyCrane
	{
		get;
	}

	public abstract bool EnableBoosterCandyPack
	{
		get;
	}

	public abstract void Initialize();

	public abstract void BlockCrush(int fromCrushId = -1, int subId = -1);

	public abstract bool CanBeCrushedByNearSlot();

	public virtual void Awake()
	{
		if (PrefabCrushPoolObject != null && BoardManager.main.CurrentMapIndex <= 0)
		{
			GameObject gameObject = Object.Instantiate(PrefabCrushPoolObject);
			if ((bool)gameObject)
			{
				PoolManager.PoolGamePlaying.Add(gameObject.transform, PrefabCrushPoolObject.gameObject.name, despawn: true, parent: true);
			}
		}
	}

	protected void Crush()
	{
		destroying = true;
		if (slot != null)
		{
			BoardManager.main.boardData.blocks[slot.x, slot.y] = IBlockType.None;
		}
	}

	private void OnDespawned()
	{
		destroying = false;
		SpriteRenderer component = GetComponent<SpriteRenderer>();
		if ((bool)component)
		{
			component.color = Color.white;
		}
	}

	protected GameObject GetCrushPoolObject()
	{
		if ((bool)PrefabCrushPoolObject)
		{
			Transform transform = PoolManager.PoolGamePlaying.Spawn(PrefabCrushPoolObject.gameObject.name);
			if ((bool)transform)
			{
				return transform.gameObject;
			}
		}
		return null;
	}

	protected void DespawnCrushPoolObject(GameObject despawnObj, float despawnTime = 0f)
	{
		if ((bool)despawnObj && (bool)PrefabCrushPoolObject)
		{
			if (despawnTime > 0f)
			{
				PoolManager.PoolGamePlaying.Despawn(despawnObj.transform, despawnTime);
			}
			else
			{
				PoolManager.PoolGamePlaying.Despawn(despawnObj.transform);
			}
		}
	}
}
