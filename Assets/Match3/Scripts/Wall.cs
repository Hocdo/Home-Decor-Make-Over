using UnityEngine;

public class Wall : MonoBehaviour
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
				BoardManager.main.GetNearSlot(x, y, Side.TopLeft).SetWall(Side.BottomRight);
				BoardManager.main.GetNearSlot(x, y, Side.TopRight).SetWall(Side.BottomLeft);
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
				BoardManager.main.GetNearSlot(x, y, Side.Top).SetWall(Side.BottomRight);
				BoardManager.main.GetNearSlot(x, y, Side.Bottom).SetWall(Side.TopRight);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
