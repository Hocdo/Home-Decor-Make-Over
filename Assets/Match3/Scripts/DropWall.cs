using UnityEngine;

public class DropWall : MonoBehaviour
{
	public bool h;

	public int x = 4;

	public int y = 3;

	public void Initialize()
	{
		if (h)
		{
			if (BoardManager.main.boardData.slots[x, y] && BoardManager.main.boardData.slots[x, y + 1])
			{
				BoardManager.main.GetNearSlot(x, y, Side.Top).SetWall(Side.Bottom);
				BoardManager.main.GetSlot(x, y).SetWall(Side.Top);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
		if (!h)
		{
			if (BoardManager.main.boardData.slots[x, y] && BoardManager.main.boardData.slots[x + 1, y])
			{
				BoardManager.main.GetNearSlot(x, y, Side.Right).SetWall(Side.Left);
				BoardManager.main.GetSlot(x, y).SetWall(Side.Right);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
