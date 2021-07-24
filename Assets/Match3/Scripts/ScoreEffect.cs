using DG.Tweening;
using PathologicalGames;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ScoreEffect : MonoBehaviour
{
	public static readonly float TweenTimeValue = 1.2f;

	private readonly float TweenPosYValue = 60f;

	public Image[] ImageScoresDigit;

	public LayoutElement[] LayoutElementScoreDigit;

	private readonly StringBuilder m_strBuilder = new StringBuilder();

	public Sprite[] SpriteImageFontBlue;

	public Sprite[] SpriteImageFontGreen;

	public Sprite[] SpriteImageFontOrange;

	public Sprite[] SpriteImageFontPurple;

	public Sprite[] SpriteImageFontRed;

	public Sprite[] SpriteImageFontYellow;

	public void StartScoreEffect(int score, int colorId)
	{
		for (int i = 0; i < ImageScoresDigit.Length; i++)
		{
			ImageScoresDigit[i].gameObject.SetActive(value: false);
		}
		m_strBuilder.Length = 0;
		m_strBuilder.Append(score);
		Sprite[] array = SpriteImageFontRed;
		switch (colorId)
		{
		case 0:
			array = SpriteImageFontRed;
			break;
		case 1:
			array = SpriteImageFontOrange;
			break;
		case 2:
			array = SpriteImageFontYellow;
			break;
		case 3:
			array = SpriteImageFontGreen;
			break;
		case 4:
			array = SpriteImageFontBlue;
			break;
		case 5:
			array = SpriteImageFontPurple;
			break;
		}
		int length = m_strBuilder.ToString().Length;
		for (int j = 0; j < length && j < 6; j++)
		{
			int result = -1;
			if (int.TryParse(m_strBuilder[length - j - 1].ToString(), out result))
			{
				ImageScoresDigit[j].gameObject.SetActive(value: true);
				ImageScoresDigit[j].sprite = array[result];
				LayoutElementScoreDigit[j].preferredWidth = array[result].rect.width;
				LayoutElementScoreDigit[j].preferredHeight = array[result].rect.height;
			}
		}
		Transform transform = base.transform;
		Vector3 localPosition = base.transform.localPosition;
		transform.DOLocalMoveY(localPosition.y + TweenPosYValue, TweenTimeValue).SetEase(Ease.OutBack);
	}

	public static void SetScoreEffect(int score, Vector3 pos, int colorId)
	{
		Transform transform = PoolManager.PoolGameEffect.Spawn(SpawnStringEffect.Get(SpawnStringEffectType.ScoreEffect));
		if ((bool)transform)
		{
			GameObject gameObject = transform.gameObject;
			gameObject.transform.SetParent(GameMain.main.GameEffectCamera.transform, worldPositionStays: false);
			gameObject.transform.position = pos;
			gameObject.transform.localScale = ((score < 2000) ? Vector3.one : new Vector3(1.4f, 1.4f, 1f));
			PoolManager.PoolGameEffect.Despawn(gameObject.transform, TweenTimeValue);
			ScoreEffect component = gameObject.GetComponent<ScoreEffect>();
			component.StartScoreEffect(score, colorId);
		}
	}
}
