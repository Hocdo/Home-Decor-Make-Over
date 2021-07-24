public class ScoreManager : MonoSingleton<ScoreManager>
{
	public static readonly int baseBlockScore = 20;

	private readonly int[] scoreArray = new int[22]
	{
		60,
		60,
		120,
		200,
		120,
		200,
		120,
		200,
		120,
		200,
		240,
		240,
		320,
		520,
		320,
		240,
		520,
		400,
		600,
		800,
		3000,
		4000
	};

	public int GetScoreUnit(ScoreType scoreType)
	{
		if (scoreType == ScoreType.None)
		{
			return 0;
		}
		return scoreArray[(int)scoreType];
	}
}
