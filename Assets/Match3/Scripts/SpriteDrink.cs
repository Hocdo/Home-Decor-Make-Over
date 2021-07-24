using UnityEngine;

public class SpriteDrink : BlockInterface
{
	public int DrinkID = 1;

	public int Level = 3;

	public SpriteRenderer spriteRenderer;

	public Sprite[] levelSprites = new Sprite[3];

	private int eventCountBorn;

	public override bool EnableBoosterHammer => true;

	public override bool EnableBoosterCandyCrane => false;

	public override bool EnableBoosterCandyPack => true;

	public void Initialize(int id, int level)
	{
		Initialize();
		DrinkID = id;
		Level = level + 1;
		blockType = (IBlockType)(51 + (id * 3 + level));
	}

	public override void Start()
	{
		base.Start();
		spriteRenderer.sprite = levelSprites[Level - 1];
	}

	public override void Initialize()
	{
		slot.gravity = false;
		eventCountBorn = GameMain.main.eventCount;
	}

	public override void BlockCrush(int fromCrushId = -1, int subId = -1)
	{
		if (eventCountBorn != GameMain.main.eventCount)
		{
			eventCountBorn = GameMain.main.eventCount;
			if (DrinkID == fromCrushId || DrinkID == subId)
			{
				CrushSpriteDrink();
			}
		}
	}

	public override bool CanBeCrushedByNearSlot()
	{
		return true;
	}

	private void CrushSpriteDrink()
	{
		Level--;
		if (Level == 0)
		{
			GameMain.main.DecreaseCollect(CollectBlockType.CarbonatedDrink, countPrevValue: true);
			Crush();
			slot.gravity = true;
			slot.SetBlock(null);
			SlotGravity.Reshading();
			UnityEngine.Object.Destroy(base.gameObject);
			GameMain.main.CheckRescueGingerMan(slot.x, slot.y);
		}
		else
		{
			spriteRenderer.sprite = levelSprites[Level - 1];
			blockType = (IBlockType)(51 + (DrinkID * 3 + Level));
		}
	}

	public void CrushByForce()
	{
		CrushSpriteDrink();
	}
}
