using UnityEngine;

public class NumberChocolateBlock : BlockInterface
{
	public int colorID = 4;

	public int inOrder;

	public SpriteRenderer spriteRenderer;

	public Sprite[] deactiveBlockSprites = new Sprite[9];

	public override bool EnableBoosterHammer => false;

	public override bool EnableBoosterCandyCrane => false;

	public override bool EnableBoosterCandyPack => true;

	public void Initialize(int id, int inOrder)
	{
		Initialize();
		colorID = id;
		this.inOrder = inOrder;
		blockType = (IBlockType)(70 + colorID);
	}

	public override void Start()
	{
		base.Start();
		spriteRenderer.sprite = deactiveBlockSprites[inOrder - 1];
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

	public void ActiveNumberChocolate(int inOrder)
	{
		if (this.inOrder == inOrder)
		{
			GameObject spawnBlockObject = SpawnStringBlock.GetSpawnBlockObject(SpawnStringBlockType.NumberChocolate);
			spawnBlockObject.transform.position = base.transform.position;
			spawnBlockObject.name = "NumberChocolate_" + slot.x + "x" + slot.y;
			NumberChocolateChip component = spawnBlockObject.GetComponent<NumberChocolateChip>();
			component.Initialize(colorID, inOrder);
			Chip component2 = spawnBlockObject.GetComponent<Chip>();
			slot.SetChip(component2);
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
}
