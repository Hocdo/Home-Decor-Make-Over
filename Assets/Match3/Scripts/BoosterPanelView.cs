using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Match3
{
	public class BoosterPanelView : MonoBehaviour
	{
		public Material boosterDisabledMaterial;

		public GameObject boosterButtonPrefab;

		public Transform boosterGroupTransform;

		public BoosterGuideView guideView;

		public BoosterInfoTable infoTable;

		public CanvasScaler canvasScaler;
	
		//[NonSerialized]
		public Booster[] boosterUI;

		private BoosterType[] boosterTypes = new BoosterType[5]
			{
				BoosterType.Shuffle,
				BoosterType.Hammer,				
				BoosterType.HBomb,
				BoosterType.VBomb,
				BoosterType.CandyPack
			};

		public GameObject GetBoosterButtonWithType(BoosterType boosterType)
        {
			for (int i = 0; i < boosterTypes.Length; i++)
            {
				if (boosterType == boosterTypes[i])
                {
					return boosterUI[i].gameObject;
                }
            }

			return null;
        }

		public virtual void Start()
		{
			
			boosterUI = new Booster[boosterTypes.Length];
			PlayerData playerData = PlayerData.current;
			
			for (int i = 0; i < boosterUI.Length; i++)
			{
				ButtonBooster component = Instantiate(boosterButtonPrefab).GetComponent<ButtonBooster>();
				component.ForceStart(boosterTypes[i]);
				boosterUI[i] = component.booster;
				boosterUI[i].disabledMaterial = boosterDisabledMaterial;
				boosterUI[i].gameObject.SetActive(value: true);
				boosterUI[i].transform.SetParent(boosterGroupTransform);
				boosterUI[i].transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
				boosterUI[i].transform.localPosition = Vector3.zero;
				boosterUI[i].guideView = guideView;
				boosterUI[i].ForceStart();
				boosterUI[i].SetUiIndex(i);

				BoosterInfoTable.BoosterInfo boosterInfo = infoTable.Find(boosterUI[i].boosterType);
				
				boosterUI[i].SetImage(boosterInfo.sprite);
				boosterUI[i].SetTextGuide(boosterInfo.guide);

				int unlockLevel = BoosterUtility.GetIngameUnlockLevel(boosterTypes[i]);

				if (playerData.match3Data.level < unlockLevel)
					boosterUI[i].SetLocked();
			}

			UpdateTextBoosterCount();

			this.ExecuteNextFrame(() => 
			{
				var canvasSize = UIUtility.GetCanvasSize(canvasScaler);
				float totalBoosterHeight = Mathf.Abs(boosterUI[boosterUI.Length - 1].GetComponent<RectTransform>().anchoredPosition.y
					- boosterUI[0].GetComponent<RectTransform>().anchoredPosition.y)
					+ boosterUI[0].GetComponent<RectTransform>().sizeDelta.y * boosterUI[0].transform.localScale.y;

				if (canvasSize.y < totalBoosterHeight * 1.1f) transform.localScale = canvasSize.y / (totalBoosterHeight * 1.1f) * Vector3.one;
			});	
		}

		public void UpdateTextBoosterCount()
		{
			if (boosterUI != null)
			{
				for (int i = 0; i < boosterUI.Length; i++)
				{
					boosterUI[i].UpdateTextBoosterCount();
				}
			}
		}
	}

}
