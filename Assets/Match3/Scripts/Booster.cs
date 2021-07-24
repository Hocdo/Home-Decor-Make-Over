using System;
using System.Collections;
using Match3;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Booster : MonoBehaviour
{
	public BoosterType boosterType;

	public GameObject buttonBuy;

	public GameObject buttonNumberObj;

	[NonSerialized] public BoosterGuideView guideView;

	public Image itemImage;
	
	protected bool onSelect;

	public GameObject selectEffect;

	public Text TextBoosterCount;

	public GameObject cost;

	public Material disabledMaterial;

	protected int uiIndex;

	private string textGuide;

	protected bool locked;

	protected Slot tutorialForceTarget;

	public void SetBoosterType(BoosterType boosterType)
    {
		this.boosterType = boosterType;

		if (boosterType == BoosterType.CandyPack)
			EventDispatcher<GlobalEventId>.Instance.RegisterEvent(GlobalEventId.CandyPackChange, UpdateBoosterCount);
		else if (boosterType == BoosterType.Hammer)
			EventDispatcher<GlobalEventId>.Instance.RegisterEvent(GlobalEventId.HammerChange, UpdateBoosterCount);
	}

	public void SetForceTarget(int x, int y)
    {
		tutorialForceTarget = BoardManager.main.GetSlot(x, y);
	}

	public bool IsLocked() { return locked; }
	
    private void OnEnable()
    {
		
	}

    private void OnDisable()
    {
		if (boosterType == BoosterType.CandyPack)
			EventDispatcher<GlobalEventId>.Instance.RemoveEvent(GlobalEventId.CandyPackChange, UpdateBoosterCount);
		else if (boosterType == BoosterType.Hammer)
			EventDispatcher<GlobalEventId>.Instance.RemoveEvent(GlobalEventId.HammerChange, UpdateBoosterCount);
	}

	private void UpdateBoosterCount(object param)
    {
		int count = PlayerData.current.match3Data.GetIngameBoosterCount(boosterType);

		if (count <= 0)
		{
			buttonBuy.SetActive(value: true);
			buttonNumberObj.gameObject.SetActive(false);
		}
		else
		{
			buttonNumberObj.gameObject.SetActive(true);
			buttonBuy.SetActive(value: false);

			if (TextBoosterCount != null)
			{
				TextBoosterCount.text = count.ToString();
			}
		}
	}

    protected virtual void Caching()
	{
		if (TextBoosterCount == null)
		{
			TextBoosterCount = base.transform.Find("Button_number/Text").GetComponent<Text>();
		}
		if (buttonBuy == null)
		{
			buttonBuy = base.transform.Find("Button_buy").gameObject;
		}
		if (selectEffect == null)
		{
			selectEffect = base.transform.Find("Eff_Item_use").gameObject;
		}
		
		if (itemImage == null)
		{
			itemImage = base.transform.Find("Item").GetComponent<Image>();
		}

		if (buttonNumberObj == null)
		{
			buttonNumberObj = base.transform.Find("Button_number").gameObject;
		}

		cost = transform.Find("Cost").gameObject;
		cost.SetActive(false);
	}

    public void SetTextGuide(string textGuide)
    {
	    this.textGuide = textGuide;
    }
    
    public void SetImage(Sprite boosterIconSprite)
	{
		itemImage.SetNativeSize();
		if (boosterIconSprite != null)
		{
			itemImage.sprite = boosterIconSprite;
			itemImage.SetNativeSize();
			itemImage.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
		}
	}

	public void SetLocked()
    {
		GetComponent<Image>().material = disabledMaterial;
		itemImage.material = disabledMaterial;

		buttonBuy.SetActive(false);
		buttonNumberObj.SetActive(false);
		selectEffect.SetActive(false);

		locked = true;
	}

	protected virtual void Start()
	{
	}

	public virtual void ForceStart()
	{
		Caching();

		if (buttonBuy != null)
		{
			buttonBuy.SetActive(value: false);
		}
		if (guideView != null)
		{
			guideView.RegistBoosterButton(GetComponent<Button>());
			guideView.gameObject.SetActive(value: false);
		}
		if (selectEffect != null)
		{
			//float num = Screen.height;
			//float num2 = Screen.width;
			//if (Screen.width > Screen.height)
			//{
			//	num = Screen.width;
			//	num2 = Screen.height;
			//}
			//float num3 = num2 / num * 3f;
			//selectEffect.gameObject.layer = base.transform.parent.gameObject.layer;
			//ParticleSystem[] componentsInChildren = selectEffect.transform.GetComponentsInChildren<ParticleSystem>();
			//for (int i = 0; i < componentsInChildren.Length; i++)
			//{
			//	componentsInChildren[i].gameObject.layer = base.transform.parent.gameObject.layer;
			//	if (componentsInChildren[i].gameObject.layer == LayerMask.NameToLayer("UIInGame"))
			//	{
			//		componentsInChildren[i].transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
			//	}
			//	else
			//	{
			//		componentsInChildren[i].transform.localScale = new Vector3(num3, num3, num3);
			//	}
			//}
			//GameObject gameObject = selectEffect.transform.Find("Check").gameObject;
			//gameObject.SetActive(value: false);
		}
		
		if (base.transform.parent.gameObject.layer == LayerMask.NameToLayer("UIInGame"))
		{
			MonoSingleton<UIManager>.Instance.eventCancelBooster += CancelBooster;
		}
		
		GameObject gameObject3 = buttonNumberObj.transform.Find("ImageYellow").gameObject;
		buttonNumberObj.transform.localPosition = new Vector3(30f, -21f, 0f);
		gameObject3.SetActive(value: true);
	}

	public void SetUiIndex(int _uiIndex)
	{
		uiIndex = _uiIndex;
	}

	private void OnDestroy()
	{
		if (base.transform.parent.gameObject.layer == LayerMask.NameToLayer("UIInGame"))
		{
			MonoSingleton<UIManager>.Instance.eventCancelBooster -= CancelBooster;
		}
	}

	protected virtual void Update()
	{
		if ((Application.isEditor || Application.platform == RuntimePlatform.Android) && UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		{
			CancelBooster();
		}
	}

	public virtual void UseBooster(bool isTutorial = false)
	{
		guideView.textGuide.text = CustomLocalization.Get(textGuide);
		guideView.textGuide.font = CustomLocalization.GetFont();
		Match3GameUI.Instance.isPlayingBoosterItem = true;
		if (isTutorial)
		{
			onSelect = false;
		}
	}

	public virtual void CancelBooster()
	{
		Match3GameUI.Instance.isPlayingBoosterItem = false;
		Match3GameUI.Instance.UpdateTextBoosterCount();
	}
	
	protected bool CheckEnableBooster(bool isTutorial = false)
	{
		if (locked)
        {
			var str = CustomLocalization.Get("booster_lock");
			PopupUtility.OpenPopupLiteMesage(str.Replace("xxx", BoosterUtility.GetIngameUnlockLevel(boosterType).ToString()));

			return false;
        }

		if (!isTutorial)
		{
			if (!GameMain.main.isPlaying)
			{
				return false;
			}
			if (!GameMain.main.CanIWait())
			{
				return false;
			}
			if (GameMain.main.isGameResult)
			{
				return false;
			}
			if (GameMain.main.CurrentTurn == VSTurn.CPU)
			{
				return false;
			}
			if (GameMain.main.isConnectedSweetRoad)
			{
				return false;
			}
		}
		if (PlayerData.current.match3Data.GetIngameBoosterCount(boosterType) > 0)
		{
			return true;
		}
        else
        {
            var popupPurchaseBooster = Popup.PopupSystem.GetOpenBuilder()
               .SetType(PopupType.PopupPurchaseBooster)
               .SetCurrentPopupBehaviour(Popup.CurrentPopupBehaviour.Close)
               .Open<PopupPurchaseBooster>();

			if (boosterType == BoosterType.CandyPack)
			{
				popupPurchaseBooster.SetData((int)boosterType, CustomLocalization.Get("bomb_name"), CustomLocalization.Get("bomb_guide"),
					100, 3, itemImage.sprite);
			}
			else if (boosterType == BoosterType.Hammer)
			{
				popupPurchaseBooster.SetData((int)boosterType, CustomLocalization.Get("hammer_name"), CustomLocalization.Get("hammer_guide"),
					60, 3, itemImage.sprite);
			}
			else if (boosterType == BoosterType.HBomb)
			{
				popupPurchaseBooster.SetData((int)boosterType, CustomLocalization.Get("h_rocket_name"), CustomLocalization.Get("h_rocket_guide"),
					60, 3, itemImage.sprite);
			}
			else if (boosterType == BoosterType.VBomb)
			{
				popupPurchaseBooster.SetData((int)boosterType, CustomLocalization.Get("v_rocket_name"), CustomLocalization.Get("v_rocket_guide"),
					60, 3, itemImage.sprite);
			}

			popupPurchaseBooster.AcceptEvent = (param) => Match3GameUI.Instance.ThrowPurchasedBoosterItemEffect(boosterType, 3);

			return false;
        }
	}

	public virtual void UpdateTextBoosterCount()
	{
		if (locked) return;

		int count = PlayerData.current.match3Data.GetIngameBoosterCount(boosterType);

		if (count <= 0)
		{
			buttonBuy.SetActive(value: true);
			buttonNumberObj.gameObject.SetActive(false);
		}
		else 
		{
			buttonNumberObj.gameObject.SetActive(true);
			buttonBuy.SetActive(value: false);

			if (TextBoosterCount != null)
			{
				TextBoosterCount.text = count.ToString();
			}
		}
	}

	protected void CompleteUseBooster()
	{
		GameMain.main.IsGettingItem = true;
		int boosterTypeInt = (int)boosterType;
		GameMain.main.IsUseBooster = true;
		if (GameMain.main.UsedBoosterCountForSync != null && (int)boosterType < GameMain.main.UsedBoosterCountForSync.Length)
		{
			GameMain.main.UsedBoosterCountForSync[boosterTypeInt]++;
		}
		if (boosterTypeInt <= 2)
		{
			GameMain.main.UsedBoosterCount[boosterTypeInt]++;
		}
		
		//AppEventManager.m_TempBox.listUsedBoosterOrder.Add(boosterType);
		Match3GameUI.Instance.isPlayingBoosterItem = false;
		PlayerData.current.match3Data.AddIngameBooster(boosterType, -1);
		//MonoSingleton<PlayerDataManager>.Instance.SaveBoosterData();
		//if (Model.Instance.playerData.match3Data.ingameBooster[boosterTypeInt] == 0)
		//{
		//	buttonBuy.SetActive(value: true);
		//}
		//else
		//{
		//	buttonBuy.SetActive(value: false);
		//}
		//if (boosterType == BoosterType.Shuffle)
		//{
		//	buttonBuy.SetActive(value: false);
		//}
		//TextBoosterCount.text = Model.Instance.playerData.match3Data.ingameBooster[boosterTypeInt].ToString();

		UpdateTextBoosterCount();

		GameMain.main.EventCounter();
		CompleteUseItem();
	}

	private void CompleteUseItem()
	{
		if (GameMain.main.UsedBoosterCountForSync != null && (int)boosterType < GameMain.main.UsedBoosterCountForSync.Length)
		{
			GameMain.main.UsedBoosterCountForSync[(int)boosterType]--;
		}
	}

	//protected IEnumerator EffectSelectFail(Transform targetPos)
	//{
	//	GameObject effSelectFail = UnityEngine.Object.Instantiate(selectFail);
	//	effSelectFail.SetActive(value: true);
	//	Transform transform = effSelectFail.transform;
	//	Vector3 position = targetPos.transform.position;
	//	float x = position.x;
	//	Vector3 position2 = targetPos.transform.position;
	//	transform.position = new Vector2(x, position2.y);
	//	yield return new WaitForSeconds(0.5f);
	//	UnityEngine.Object.Destroy(effSelectFail);
	//}
}
