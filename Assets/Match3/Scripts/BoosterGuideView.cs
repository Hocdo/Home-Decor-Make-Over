using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Match3
{
	public class BoosterGuideView : MonoBehaviour
	{
		public Text title;

		public GameObject bg;

		public List<Button> boosterButtons = new List<Button>();

		public Image boosterIcon;

		private bool registComplete;

		public Sprite[] SpriteBoosterIcon;

		private int tempDir = 1;

		public Text textGuide;

        public void Awake()
        {
			//boosterGuideViewSnap = new UIEdgeSnapPosition(boosterGuideView.GetComponent<RectTransform>(), new Vector2(-1.2f, 0f));
		}

        private void OnEnable()
		{
			if (registComplete)
			{
				for (int i = 0; i < boosterButtons.Count; i++)
				{
					boosterButtons[i].transform.Find("Button_number").gameObject.SetActive(value: false);
				}
			}

			Match3GameUI.Instance.OnBoosterSelected();
		}

		private void OnDisable()
		{
			if (!registComplete)
			{
				return;
			}
			for (int i = 0; i < boosterButtons.Count; i++)
			{
				boosterButtons[i].interactable = true;
				Booster component = boosterButtons[i].GetComponent<Booster>();
	
				if (component.IsLocked()) continue;

				GameObject gameObject = component.buttonNumberObj;

				if (component.boosterType == BoosterType.Shuffle)
				{
					if (PlayerData.current.match3Data.ingameBooster[(int)component.boosterType] == 0)
					{
						gameObject.SetActive(value: false);
					}
					else
					{
						gameObject.SetActive(value: true);
					}
				}
				else
				{
					if (PlayerData.current.match3Data.ingameBooster[(int)component.boosterType] == 0)
					{
						gameObject.SetActive(value: false);
					}
					else
					{
						gameObject.SetActive(value: true);
					}
				}
			}

			Match3GameUI.Instance.OnBoosterDeselected();
		}

		public void RegistBoosterButton(Button _boosterButton)
		{
			registComplete = true;
			boosterButtons.Add(_boosterButton);
		}

		public void SetIconImage(BoosterType boosterType)
		{
			Sprite sprite = SpriteBoosterIcon[(int)boosterType];
			if (sprite != null)
			{
				if ((bool)boosterIcon)
				{
					boosterIcon.sprite = sprite;
					boosterIcon.SetNativeSize();
				}
			}
		}

		public void SetVisualize(bool flag)
		{				
			boosterIcon.enabled = flag;
			title.enabled = flag;
			bg.SetActive(flag);
		}

		public void TurnOnOnlyOneBoosterUI(int uiIndex)
		{
			boosterIcon.gameObject.SetActive(value: true);
			Vector3 localPosition = textGuide.transform.localPosition;
			localPosition.x += 0.001f * (float)tempDir;
			tempDir = -tempDir;
			textGuide.transform.localPosition = localPosition;
			for (int i = 0; i < boosterButtons.Count; i++)
			{
				boosterButtons[i].interactable = false;
			}
			boosterButtons[uiIndex].interactable = true;
		}
	}

}

