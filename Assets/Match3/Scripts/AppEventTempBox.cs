using System;
using System.Collections.Generic;

public class AppEventTempBox
{
	public AppEventManager.AdAccessedBy adAccessedBy = AppEventManager.AdAccessedBy.Etc;

	public AppEventManager.LifeAccessedBy askLifeAccessedBy = AppEventManager.LifeAccessedBy.Etc;

	public AppEventManager.StageClearPopupClosedAction stageClearedPopupClosedAction;

	public bool AutoDailyRewardPopup;

	public bool AutoStepSpinPopup;

	public AppEventManager.CoinCategory coinCategory;

	public AppEventManager.CoinPurchasedProductType coinProductType;

	public int countFailStreak;

	public DateTime dateTimeGameStart = DateTime.Now;

	public DateTime dateTimeLoadGame = DateTime.Now;

	public DateTime dateTimeSpendStart = DateTime.Now;

	public int didAreaTutorialPlay;

	public int didClearLastPlayedLevel = -1;

	public int didPlayerQuit;

	public AppEventManager.FacebookLoginFromWhere fbLoginWhere = AppEventManager.FacebookLoginFromWhere.ETC;

	public bool forceUpdate;

	public int gameContinueCount;

	public int GameUnMatchedMoveBlockCount;

	public int howManyMakeBlock2x2;

	public int howManyMakeBlock3x3;

	public int howManyMakeBlock4x1;

	public int howManyMakeBlock5x1;

	public int inviteActionType;

	public int isExistFacebookAcount;

	public int isExistPlayerAccount;

	public bool IsFirstLevelFail;

	public bool IsFirstLifeZero;

	public bool isOpenedUpdatePopup;

	public bool isPurchaseInGameOver;

	public bool isRewaredFacebookLoginReward;

	public bool isThisLevelClearedBefore;

	public int isUseDeviceIDForADID;
	
	public int lastRewardCount;

	public int lastRewardItemIndex;
	
	public int loggedInType;

	public AppEventManager.MetaGameAccessedBy metaGameAccessedBy;

	public int NumOfItemsAlreadyUsedInLevel;

	public AppEventManager.GetPermissionAccessedBy permissionAccessedBy = AppEventManager.GetPermissionAccessedBy.ETC;

	public bool permissionPublishActions;

	public bool permissionUserFriends;

	public int prevBonusGameBestScore;

	public int prevBonusGameClearedCount;

	public int prevBonusGameFailCount;

	public int prevBonusGamePlayCount;

	public int prevBonusGameStar;

	public int prevGameBestScore;

	public int prevGameClearCount;

	public int prevGameFailCount;

	public int prevGamePlayCount;

	public int prevGameStar;

	public int levelRankWithFriends;

	public AppEventManager.SessionStartedAs remotePushAs = AppEventManager.SessionStartedAs.Relaunched;

	public DateTime PurchaseFunnelStepElapsedTime = DateTime.Now;

	public AppEventManager.LifeAccessedBy sendLifeAccessedBy = AppEventManager.LifeAccessedBy.Etc;

	public int sessionGameClearCount;

	public int sessionGameFailCount;

	public int sessionGamePlayCount;

	public int totalCollectCount;

	public int TotalSpendInGameItemsCount;

	public int TotalSpentCoin;

	public int UsedMoveCount;

	public int GetTokenCountInLevel;

	public bool CompleteToGetToken;

	public int DungeonOpenCount;

	public int FirstGetTokenTime;

	public bool TokenCountSwitch;

	public int LevelCountForTokenTime;

	public int DungeonLevelPlayCount;

	public int DungeonLevelFailCount;

	public bool IsUsedRetryChance;

	public int MidRewardCountInDungeon;

	public int MidRewardCountInEvent;

	public int MidRewardFailedCount;

	public bool firstSessionFailCheck = true;

	public void InitGamePlay()
	{
		dateTimeGameStart = DateTime.Now;
		totalCollectCount = 0;
		gameContinueCount = 0;
		NumOfItemsAlreadyUsedInLevel = 0;
		GameUnMatchedMoveBlockCount = 0;
		UsedMoveCount = 0;
		howManyMakeBlock2x2 = (howManyMakeBlock3x3 = (howManyMakeBlock4x1 = (howManyMakeBlock5x1 = 0)));
		GetTokenCountInLevel = 0;
		CompleteToGetToken = false;
	}

	public void SetTotalCollectCount(int[] countOfEachTargetCount)
	{
		totalCollectCount = 0;
		for (int i = 0; i < countOfEachTargetCount.Length; i++)
		{
			totalCollectCount += countOfEachTargetCount[i];
		}
	}
}
