using System.Collections.Generic;

public class RealTimeOptionData
{
	public bool IsEnableUserLevelRating;

	public List<int> ListRatingLevel = new List<int>();

	private readonly List<int> listUserRatedLevel = new List<int>();

	public bool AskLevelRating(int level)
	{
		if (IsEnableUserLevelRating && ListRatingLevel.Contains(level) && !listUserRatedLevel.Contains(level))
		{
			listUserRatedLevel.Add(level);
			return true;
		}
		return false;
	}
}
