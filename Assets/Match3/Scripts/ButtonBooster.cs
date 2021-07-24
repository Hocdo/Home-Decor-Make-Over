using UnityEngine;
using Match3;

public class ButtonBooster : MonoBehaviour
{
	public Booster booster;

	public void ForceStart(BoosterType type)
	{
		AddBoosterComponent(type);
	}

	private void AddBoosterComponent(BoosterType type)
	{
		string empty = string.Empty;
		switch (type)
		{
		case BoosterType.Hammer:
			booster = base.gameObject.AddComponent<BoosterMagicHammer>();
			break;
		case BoosterType.CandyPack:
			booster = base.gameObject.AddComponent<BoosterCandyPack>();
			break;
		case BoosterType.Shuffle:
			booster = base.gameObject.AddComponent<BoosterShuffle>();
			break;
		case BoosterType.HBomb:
			booster = base.gameObject.AddComponent<BoosterHBomb>();
			break;
		case BoosterType.VBomb:
			booster = base.gameObject.AddComponent<BoosterVBomb>();
			break;
		default:
			booster = base.gameObject.AddComponent<BoosterMagicHammer>();
			break;
		}

		booster.SetBoosterType(type);
	}

	public void UseBoosterInterface()
	{
		booster.UseBooster();
	}
}
