using PathologicalGames;
using System.Collections;
using UnityEngine;

public class NumberChocolateChip : Chip
{
	public int colorID = 4;

	public int inOrder;

	public SpriteRenderer spriteRenderer;

	public Sprite[] activeBlockSprites = new Sprite[54];

	public override void Awake()
	{
		base.Awake();
		Utils.EnableAllSpriteRenderer(base.gameObject);
		chipType = ChipType.NumberChocolate;
	}

	public void Initialize(int id, int inOrder)
	{
		base.id = id;
		colorID = id;
		this.inOrder = inOrder;
		spriteRenderer.sprite = activeBlockSprites[colorID * 9 + (inOrder - 1)];
	}

	public override IEnumerator DestroyChipFunction()
	{
		canMove = false;
		base.IsMatching = true;
		yield return new WaitForSeconds(0.1f);
		GameMain.main.ActiveNumberChocolate(inOrder + 1);
		GameMain.main.DecreaseCollect(CollectBlockType.NumberChocolate, countPrevValue: true);
		PoolManager.PoolGameBlocks.Despawn(base.transform);
		canMove = true;
		ParentRemove();
		base.IsMatching = false;
	}
}
