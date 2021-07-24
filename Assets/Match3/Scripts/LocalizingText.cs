using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizingText : MonoBehaviour
{
	[SerializeField]
	public string StringKey = string.Empty;

	private void Start()
	{
		Refresh();
	}

	public void Refresh()
	{
		Text component = base.gameObject.GetComponent<Text>();
		if ((bool)component)
		{
			string langValue = MonoSingleton<ServerDataTable>.Instance.GetLangValue(StringKey);
			if (!string.IsNullOrEmpty(langValue))
			{
				component.text = langValue;
			}
			component.lineSpacing = 0.8f;
		}
	}
}
