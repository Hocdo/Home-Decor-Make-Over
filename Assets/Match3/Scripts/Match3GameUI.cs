using DG.Tweening;
using PathologicalGames;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Match3GameUI : MonoBehaviour
{
	private static Match3GameUI _instance;

	private int _currentMoveCount;

	private int _currentScore;

	public bool isPlayingBoosterItem;

	private float lerpScoreTimeDuration;

	private int targetScore;

	private int oldScore;

	public Match3.GameProgressInfoView progressInfoView;

	public Match3.BoosterPanelView boosterPanelView;

	public Match3.BoosterGuideView boosterGuideView;

	public Button settingButton;

	public Text skipFestivalText;

    private UIEdgeSnapPosition progressInfoViewSnap;

    private UIEdgeSnapPosition boosterPanelViewSnap;

	private UIEdgeSnapPosition boosterGuideViewSnap;

	private UIEdgeSnapPosition settingButtonSnap;

    public static Match3GameUI Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = (Object.FindObjectOfType(typeof(Match3GameUI)) as Match3GameUI);
				if (!(_instance == null))
				{
				}
			}
			return _instance;
		}
	}

	private int currentScore
	{
		get
		{
			return _currentScore;
		}
		set
		{
			_currentScore = value;			
			progressInfoView.UpdateTextScore(value);
		}
	}

	private int currentMoveCount
	{
		get
		{
			return _currentMoveCount;
		}
		set
		{
			_currentMoveCount = value;
			progressInfoView.TextMoveCount.text = _currentMoveCount.ToString();
			if (_currentMoveCount <= 5 && !GameMain.main.isBonusTime)
			{
                Sequence sequence = DOTween.Sequence();
				sequence.Append(progressInfoView.TextMoveCount.transform.DOScale(2f, 0.2f));
				sequence.Append(progressInfoView.TextMoveCount.transform.DOScale(1f, 0.2f));
				sequence.Play();
			}
		}
	}

	public void Awake()
	{
		_instance = this;
	}

	public void Start()
	{
        progressInfoViewSnap = new UIEdgeSnapPosition(progressInfoView.GetComponent<RectTransform>(), new Vector2(-1.2f, 0f));
        boosterPanelViewSnap = new UIEdgeSnapPosition(boosterPanelView.GetComponent<RectTransform>(), new Vector2(1.2f, 0f));
		boosterGuideViewSnap = new UIEdgeSnapPosition(boosterGuideView.GetComponent<RectTransform>(), new Vector2(-1.2f, 0f));
		settingButtonSnap = new UIEdgeSnapPosition(settingButton.GetComponent<RectTransform>(), new Vector2(0, -1.2f));

    settingButton.onClick.AddListener(() => 
		{
			Popup.PopupSystem.Instance.ShowPopup(PopupType.PopupSelectJewel, Popup.CurrentPopupBehaviour.Close, true, true);
		});
	}

	public void Update()
	{
		//if (oldScore < targetScore)
		//{
		//	currentScore = (int)Mathf.Lerp(oldScore, targetScore, lerpScoreTimeDuration);
		//	lerpScoreTimeDuration += Time.deltaTime * 4f;			
		//}

		if (Input.GetKeyDown(KeyCode.Escape) && !Popup.PopupSystem.Instance.IsShowingPopup()) 
		{
			Popup.PopupSystem.Instance.ShowPopup(PopupType.PopupSetting, Popup.CurrentPopupBehaviour.Close, true, true);
		}			
	}

	public void Reset(int startScore, int moveCount, MapDataCollectBlock[] collectBlocks, int gid)
	{
		currentScore = (targetScore = (oldScore = startScore));
		SetMoveCount(moveCount);
		progressInfoView.Reset(collectBlocks, gid);
	}

	public void SetTargetScore(int targetScore)
	{
		oldScore = currentScore;
		this.targetScore = targetScore;
		lerpScoreTimeDuration = 0f;
	}

	public void OnBoosterSelected()
    {
		Material mat = GameMain.main.bgRenderer.material;
		mat.DOKill();
		mat.DOColor(new Color(0.2f, 0.2f, 0.2f, 1f), 0.35f);
    }

	public void OnBoosterDeselected()
    {
		Material mat = GameMain.main.bgRenderer.material;
		mat.DOKill();
		mat.DOColor(new Color(0.8f, 0.8f, 0.8f, 1f), 0.35f);
	}

	public void SetMoveCount(int moveCount)
	{
		currentMoveCount = moveCount;
	}

	public void UpdateTextBoosterCount()
	{
		boosterPanelView.UpdateTextBoosterCount();
	}

	public void UpdateCollect(CollectBlockType collectBlockType, int count)
	{
		progressInfoView.UpdateCollect(collectBlockType, count);
	}

	public Vector3 GetCollectObjectPosition(CollectBlockType collectBlockType)
	{
		return progressInfoView.GetCollectObjectPosition(collectBlockType);
	}

	public Vector3 GetCollectObjectGameCameraPosition(CollectBlockType collectBlockType)
	{
		Vector3 viewportPoint = GameMain.main.UIGameCamera.WorldToViewportPoint(GetCollectObjectPosition(collectBlockType));

		Vector3 worldPoint = GameMain.main.GameEffectCamera.ViewportToWorldPoint(viewportPoint);
		//return GameMain.main.GameEffectCamera.ViewportToWorldPoint(GameMain.main.UIGameCamera.WorldToViewportPoint(GetCollectObjectPosition(collectBlockType)));
		return worldPoint;
		//return progressInfoView.GetCollectObjectPosition(collectBlockType);
	}

    public Vector3 GetBonusCoinGameCameraPosition()
    {
        Vector3 viewportPoint = GameMain.main.UIGameCamera.WorldToViewportPoint(progressInfoView.bonusCoinImage.transform.position);

        Vector3 worldPoint = GameMain.main.GameEffectCamera.ViewportToWorldPoint(viewportPoint);
        //return GameMain.main.GameEffectCamera.ViewportToWorldPoint(GameMain.main.UIGameCamera.WorldToViewportPoint(GetCollectObjectPosition(collectBlockType)));
        return worldPoint;
    }


    public void ThrowPurchasedItemEffectForDigging(SpawnStringEffectType EffectType, int NumOfItem, Vector3 startPos)
	{
		StartCoroutine(progressInfoView.BuyEffect(EffectType, progressInfoView.throwingTarget, NumOfItem, startPos));
	}

	public void ThrowPurchasedItemEffect(SpawnStringEffectType EffectType, int NumOfItem)
	{
		StartCoroutine(progressInfoView.BuyEffect(EffectType, progressInfoView.throwingTarget, NumOfItem));
	}

	public void ThrowPurchasedBoosterItemEffect(Match3.BoosterType boosterType, int NumOfItem)
	{
		SpawnStringEffectType effectType = SpawnStringEffectType.SuccessBuyCandyPack;

		if (boosterType == Match3.BoosterType.CandyPack)
		{
			effectType = SpawnStringEffectType.SuccessBuyCandyPack;
		}
		else if (boosterType == Match3.BoosterType.Hammer)
		{
			effectType = SpawnStringEffectType.SuccessBuyMagicHammer;
		}
		else if (boosterType == Match3.BoosterType.HBomb)
		{
			effectType = SpawnStringEffectType.SuccessBuyBoosterHBomb;
		}
		else if (boosterType == Match3.BoosterType.VBomb)
		{
			effectType = SpawnStringEffectType.SuccessBuyBoosterVBomb;
		}
        else
        {
			return;
        }

		StartCoroutine(progressInfoView.BuyEffect(effectType, boosterPanelView.GetBoosterButtonWithType(boosterType), NumOfItem));
	}

	public static string GetCollectIconNormalBlockName(string blockTypeName)
	{
		return "UI/CollectIcon/" + blockTypeName;
	}

	public void SkipFestival()
	{
		GameMain.main.SkipFestival();
	}

	public void OnFestivalStart()
    {
		progressInfoView.OnFestivalStart();
		skipFestivalText.color = new Color(1f, 1f, 1f, 0f);
		skipFestivalText.transform.parent.gameObject.SetActive(true);
		skipFestivalText.DOColor(Color.white, 0.35f);
	}

	public void OnFestivalEnd()
	{
		Hide(true);
		
		skipFestivalText.DOColor(new Color(1f, 1f, 1f, 0f), 0.35f).OnComplete(() => skipFestivalText.transform.parent.gameObject.SetActive(false));
	}

	public void Show(bool animation)
    {
        if (animation)
        {
            progressInfoViewSnap.SetPositionVisibility(false);
            boosterPanelViewSnap.SetPositionVisibility(false);
            settingButtonSnap.SetPositionVisibility(false);
            progressInfoViewSnap.target.DOAnchorPos(progressInfoViewSnap.showPosition, 0.35f);
            boosterPanelViewSnap.target.DOAnchorPos(boosterPanelViewSnap.showPosition, 0.35f);
            settingButtonSnap.target.DOAnchorPos(settingButtonSnap.showPosition, 0.35f);
        }
        else
        {
            progressInfoViewSnap.SetPositionVisibility(true);
            boosterPanelViewSnap.SetPositionVisibility(true);
            settingButtonSnap.SetPositionVisibility(true);
        }
    }

    public void Hide(bool animation)
    {
        if (animation)
        {
            progressInfoViewSnap.SetPositionVisibility(true);
            boosterPanelViewSnap.SetPositionVisibility(true);
            settingButtonSnap.SetPositionVisibility(true);
            progressInfoViewSnap.target.DOAnchorPos(progressInfoViewSnap.hidePosition, 0.35f);
            boosterPanelViewSnap.target.DOAnchorPos(boosterPanelViewSnap.hidePosition, 0.35f);
            settingButtonSnap.target.DOAnchorPos(settingButtonSnap.hidePosition, 0.35f);
        }
        else
        {
            progressInfoViewSnap.SetPositionVisibility(false);
            boosterPanelViewSnap.SetPositionVisibility(false);
            settingButtonSnap.SetPositionVisibility(false);
        }
    }
}
