public static class LocalDataString
{
	private static readonly DictEnumKeyGenericVal<string> dicString;

	static LocalDataString()
	{
		dicString = new DictEnumKeyGenericVal<string>();
		dicString.Add(0, "OCC_TokenCount");
		dicString.Add(1, "OCC_EnterDungeon");
		dicString.Add(2, "OCC_LastDungeonOpenTime");
		dicString.Add(3, "OCC_ID");
		dicString.Add(4, "OCC_DidAnyBoosterUse");
		dicString.Add(5, "OCC_DidUseRetryChance");
		dicString.Add(6, "OCC_IsClearOneCoinChallenge");
		dicString.Add(7, "OCC_IsOpeningDungeon");
		dicString.Add(8, "OCC_CurrentLevel");
		dicString.Add(9, "OCC_DungeonOpenCount");
		dicString.Add(10, "OCC_FirstGetTokenTime");
		dicString.Add(11, "OCC_TokenCountSwitch");
		dicString.Add(12, "OCC_LevelCountForTokenTime");
		dicString.Add(13, "OCC_DungeonLevelFailCount");
		dicString.Add(14, "OCC_DungeonLevelPlayCount");
		dicString.Add(15, "OCC_ClearCountPerDungeonLevel");
		dicString.Add(16, "OCC_FailCountPerDungeonLevel");
		dicString.Add(17, "OCC_MidRewardCountInDungeon");
		dicString.Add(18, "OCC_MidRewardCountInEvent");
		dicString.Add(19, "OCC_MidRewardFailedCount");
		dicString.Add(20, "AbnormalTermination");
		dicString.Add(21, "IsDecreasingLife");
		dicString.Add(22, "IsResetOneCoinChallenge");
		dicString.Add(23, "LastPlayedGid");
		dicString.Add(24, "LastQuestResetTime");
		dicString.Add(25, "HowManyTimesInAWeekResetQuest");
		dicString.Add(26, "LastResetQuestCountTime");
	}

	public static string GetString(StringIndex index)
	{
		return dicString[(int)index];
	}
}
