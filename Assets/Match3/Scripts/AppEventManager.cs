using System;
using System.Collections.Generic;
using UnityEngine;

public class AppEventManager : MonoSingleton<AppEventManager>
{
	public enum FacebookLoginFromWhere
	{
		Auto_Popup__x__Cleared_Level_,
		Auto_Popup__x__No_More_Lives,
		Title_Screen,
		Message_Center,
		Heart_Request_Popup,
		Bonus_Level_Popup,
		Leader_Board__x__Game_Start_Popup,
		Leader_Board__x__Game_Result_Popup,
		Leader_Board__x__Lobby_Popup,
		Option_Popup,
		Get_Help_Request_Popup,
		ETC
	}

	public enum SessionStartedAs
	{
		Resumed,
		Relaunched,
		First_Launch
	}

	public enum CoinCategory
	{
		Null,
		InGameItem,
		Life,
		Move,
		DailySpin,
		PlayBonusGame,
		EventPopup,
		AutomaticPopup,
		PreBooster,
		DailyBonusLevel,
		RandomBox
	}

	public enum CoinPurchasedProductType
	{
		Regular_Coin_Pack,
		Starter_Pack_1,
		Cohort_Sale_Pack,
		Cohort_Bonus_Item_Pack,
		Unlimited_Life_Pack,
		Buyback_Pack,
		Ingame_Item_Pack,
		Push__x__1_More_Expensive_Proposal,
		Push__x__1_More_Ingame_Item_Pack,
		Push__x__1_Regular_Coin_Pack,
		Double_Coin_Pack,
		Step_Double_Coin_Pack,
		Super_Sale_Coin_Pack,
		BoosterPackBomb,
		BoosterPackHammer,
		BoosterPackHBomb,
		BoosterPackVBomb,
		BoosterPackBomb2,
		PeriodEventChristmasPack,
		Etc
	}

	public enum ItemEarnedBy
	{
		Initial_Coin_for_New_Player,
		Timed_Replenish__x__While_Gone,
		Timed_Replenish__x__While_Playing,
		Free_Replenish__x__First_Time_No_More_Lives,
		Free_Lives_for_New_Player,
		Free_Replenish,
		Initial_Water_for_New_Player,
		Initial_Items_When_Unlocked,
		Initial_Star_Candy_for_New_Player,
		Level_Clear_Reward,
		Viral_Alien,
		Daily_Bonus_Level_Reward,
		Purchased_with_Real_Money,
		Purchased_with_Coins,
		Bonus_Level_Reward,
		Daily_Bonus,
		Daily_Bonus_Double_Watch_AD,
		Event_Reward,
		CS_Reward,
		Fanpange_Reward,
		Welcome_Back_Bonus,
		Become_a_Hero_Reward,
		Facebook_Login_Reward,
		Daily_Spin_Bonus,
		AD_Watching_Reward,
		Package_Product,
		Fast_Level_Achievement_Reward,
		Friends_Help,
		Hard_Level_Reward,
		Metagame__x__Tree_Reward,
		Rating_Reward,
		OneCoin,
		Step_Spin_Bonus,
		Reward_Life_Message,
		Guest_Bonus,
		Random_Box,
		League,
		Second_Level_Clear_Reward,
		Quest_Reward,
		Quest_All_Clear_Reward,
		Step_Spin_Tutorial,
		Other_Reward
	}

	public enum StageClearPopupClosedAction
	{
		None,
		Next,
		Invite,
		Close
	}

	public enum LifeAccessedBy
	{
		Life_Icon_from_Lobby,
		No_more_life_Automatic_Popup,
		Message_Center,
		Friends_Ranking_UI,
		Etc
	}

	public enum AdAccessedBy
	{
		Life_Icon_from_Lobby,
		No_more_life_Popup,
		No_more_life_Automatic_Popup,
		BounsLevelBall_from_Lobby,
		BounsLevelBall_Automatic_Popup,
		Levelball_from_Lobby,
		GameStart_Automatic_Popup,
		Coin_Store_Popup,
		Coin_Store_Automatic_Popup,
		Coin_ADReward_Popup,
		AD_Booster_Icon_from_Ingame,
		AD_Package_Popup,
		AlienMessage,
		OneCoin_RewardAD_Retry_popup,
		AD_Help_Popup,
		Level_Result_Popup,
		LevelReward_Popup,
		Guest_GameStartPopup_RankingUI,
		LifeMessage,
		StepSpinPopup,
		StepSpinTutorialPopup,
		Level_Clear_Click_Next_Button,
		Etc
	}

	public enum AdCompletedStepReached
	{
		None,
		Popup_Opened,
		Clicked_ADWatching_Button,
		Completed
	}

	public enum GetPermissionActionType
	{
		Impression,
		Response_Confirm,
		Response_Later
	}

	public enum GetPermissionAccessedBy
	{
		AcceptDialog,
		AutomaticPopup,
		InvitePopup,
		LeaderBoard_GameStart,
		LeaderBoard_GameEnd,
		LeaderBoard_LobbyIcon,
		GetHelp,
		AskForLives,
		MessageCenter,
		ETC
	}

	public enum PurchaseTypeOfPopup
	{
		Regual_Coin_Store,
		Starter_Pack_Popup,
		Cohort_Sale_Popup__x__2nd_Purchase,
		Unlimited_Life_Pack,
		Buyback_Pack,
		Ingame_Item_Pack,
		Push__x__1_More_Expensive_Proposal,
		Push__x__1_More_Ingame_Item_Pack,
		Push__x__1_Regular_Coin_Store,
		Double_Coin_Store,
		Step_Double_Coin_Store,
		BoosterPackBomb,
		BoosterPackHammer,
		BoosterPackHBomb,
		BoosterPackVBomb,
		BoosterPackBomb2,
		PeriodEventChristmasPack,
		Others
	}

	public enum PurchaseReachedStep
	{
		PopupOpened,
		ClickedPurchaseButton,
		Purchased
	}

	public enum MetaGameAccessedBy
	{
		Lobby_Icon,
		Automatic_Event
	}

	public static AppEventTempBox m_TempBox = new AppEventTempBox();

	private void Start()
	{
        m_TempBox.IsFirstLevelFail = ((PlayerPrefs.GetInt("FirstLevelFail", 0) == 1) ? true : false);
        m_TempBox.IsFirstLifeZero = ((PlayerPrefs.GetInt("FirstLifeZero", 0) == 1) ? true : false);
        m_TempBox.countFailStreak = PlayerPrefs.GetInt("CountFailStreak", 0);
        m_TempBox.DungeonOpenCount = PlayerPrefs.GetInt(LocalDataString.GetString(StringIndex.OCCDungeonOpenCount), 0);
        m_TempBox.FirstGetTokenTime = PlayerPrefs.GetInt(LocalDataString.GetString(StringIndex.OCCFirstGetTokenTime), 0);
        m_TempBox.TokenCountSwitch = ((PlayerPrefs.GetInt(LocalDataString.GetString(StringIndex.OCCTokenCountSwitch), 0) == 1) ? true : false);
        m_TempBox.LevelCountForTokenTime = PlayerPrefs.GetInt(LocalDataString.GetString(StringIndex.OCCLevelCountForTokenTime), 1);
        m_TempBox.DungeonLevelPlayCount = PlayerPrefs.GetInt(LocalDataString.GetString(StringIndex.OCCDungeonLevelPlayCount), 0);
        m_TempBox.DungeonLevelFailCount = PlayerPrefs.GetInt(LocalDataString.GetString(StringIndex.OCCDungeonLevelFailCount), 0);
        m_TempBox.MidRewardCountInDungeon = PlayerPrefs.GetInt(LocalDataString.GetString(StringIndex.OCCMidRewardCountInDungeon), 0);
        m_TempBox.MidRewardCountInEvent = PlayerPrefs.GetInt(LocalDataString.GetString(StringIndex.OCCMidRewardCountInEvent), 0);
        m_TempBox.MidRewardFailedCount = PlayerPrefs.GetInt(LocalDataString.GetString(StringIndex.OCCMidRewardFailedCount), 0);
    }
}
