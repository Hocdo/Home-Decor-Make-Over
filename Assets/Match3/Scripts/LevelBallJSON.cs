public class LevelBallJSON
{
	public int[] bonus;

	public int[] normal;

	public LevelBallJSON(int normalCount = 45, int bonusCount = 3)
	{
		normal = new int[normalCount * 2];
		bonus = new int[bonusCount * 2];
	}
}
