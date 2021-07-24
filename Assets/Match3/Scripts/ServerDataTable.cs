using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerDataTable : MonoSingleton<ServerDataTable>
{
	public static int MAX_LEVEL = 40;

	public static string UpdateURL = string.Empty;

	public Dictionary<int, PacketDataSpecCoin> m_dicTableCoin = new Dictionary<int, PacketDataSpecCoin>();

	public Dictionary<int, PacketDataSpecItemData> m_dicTableItemShop = new Dictionary<int, PacketDataSpecItemData>();

	public Dictionary<string, string> m_dicTableOption = new Dictionary<string, string>();

	public Dictionary<string, PacketDataLanguageData> m_dicTableLang = new Dictionary<string, PacketDataLanguageData>();

	public bool EnableIOSInAppRating = true;

	public int InterstitalGameClearCount = 5;

	public string faqCsUrl = "https://sweetroad.zendesk.com/hc/en-us";

	private readonly List<string> listLoadedTable = new List<string>();

	private ResPacketLoadGameTable serverTable;

	public int SessionTimeMinute = 10;
	
	public int[] TutorialOpenLevel = new int[8];

	private readonly List<string> listLangConvertFormatType = new List<string>
	{
		"[LEVEL]",
		"[POINT]",
		"[STAR]",
		"[COUNT]",
		"[EPISODE]",
		"[STAGE]",
		"[NAME]",
		"[USER_NAME]",
		"[ITEM_NAME]",
		"[CURRENCY]",
		"[TIME]",
		"[SALE]",
		"[COIN]",
		"[TOKEN]"
	};

	private List<int> listEnabledLocalPushIndex;

	public void InitTable()
	{
		listLoadedTable.Clear();
	}

	public void SetTable(ResPacketLoadGameTable res)
	{
		if (res != null)
		{
			serverTable = res;
			LoadTableSpecCoin(res.m_SPEC_COIN);
			LoadTableSpecItem(res.m_SPEC_ITEM);
			LoadTableSpecOption(res.m_SPEC_OPTION);
			LoadTableSpecGameLevelData(res.m_SPEC_GAME);
			LoadTableSpecGameMapData(res.m_SPEC_MAP);
		}
	}

	public static string GetLangCode(SystemLanguage langCode)
	{
		switch (langCode)
		{
		case SystemLanguage.Korean:
			return "KR";
		case SystemLanguage.Japanese:
			return "JP";
		case SystemLanguage.German:
			return "GE";
		case SystemLanguage.ChineseSimplified:
			return "CH";
		case SystemLanguage.French:
			return "FR";
		case SystemLanguage.Italian:
			return "IT";
		case SystemLanguage.Portuguese:
			return "PO";
		case SystemLanguage.Spanish:
			return "SP";
		default:
			return "EN";
		}
	}

	public void LoadLangTable(PacketDataLanguageData[] res)
	{
		m_dicTableLang.Clear();
		foreach (PacketDataLanguageData packetDataLanguageData in res)
		{
			if (!string.IsNullOrEmpty(packetDataLanguageData.lang_key) && !m_dicTableLang.ContainsKey(packetDataLanguageData.lang_key))
			{
				m_dicTableLang.Add(packetDataLanguageData.lang_key, packetDataLanguageData);
			}
		}
	}

	private void LoadTableSpecItem(PacketDataSpecItem items)
	{
		if (items == null || items.shop_ingame == null)
		{
			return;
		}
		m_dicTableItemShop.Clear();
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		List<int> list3 = new List<int>();
		List<int> list4 = new List<int>();
		PacketDataSpecItemData[] shop_ingame = items.shop_ingame;
		foreach (PacketDataSpecItemData packetDataSpecItemData in shop_ingame)
		{
			m_dicTableItemShop.Add(packetDataSpecItemData.iid, packetDataSpecItemData);
			if (packetDataSpecItemData.iid_seq == 7)
			{
				if (!list.Contains(packetDataSpecItemData.iid))
				{
					list.Add(packetDataSpecItemData.iid);
				}
			}
			else if (packetDataSpecItemData.iid_seq == 8)
			{
				if (!list2.Contains(packetDataSpecItemData.iid))
				{
					list2.Add(packetDataSpecItemData.iid);
				}
			}
			else if (packetDataSpecItemData.iid_seq == 17)
			{
				if (!list3.Contains(packetDataSpecItemData.iid))
				{
					list3.Add(packetDataSpecItemData.iid);
				}
			}
			else if (packetDataSpecItemData.iid_seq == 18 && !list4.Contains(packetDataSpecItemData.iid))
			{
				list4.Add(packetDataSpecItemData.iid);
			}
		}
		//MonoSingleton<PlayerDataManager>.Instance.dicBoosterItemList.Clear();
		//MonoSingleton<PlayerDataManager>.Instance.dicBoosterItemList.Add(7, list);
		//MonoSingleton<PlayerDataManager>.Instance.dicBoosterItemList.Add(8, list2);
		//MonoSingleton<PlayerDataManager>.Instance.dicBoosterItemList.Add(17, list3);
		//MonoSingleton<PlayerDataManager>.Instance.dicBoosterItemList.Add(18, list4);
		listLoadedTable.Add(LocalDataManager.FILE_NAME_TABLE_SPEC_ITEM);
	}

	public void LoadTableFromLocalFile()
	{
		if (!listLoadedTable.Contains(LocalDataManager.FILE_NAME_TABLE_SPEC_COIN))
		{
			string fileContents = LocalDataManager.GetFileContents(LocalDataManager.FILE_NAME_TABLE_SPEC_COIN);
			PacketDataSpecCoin[] lists = JsonConvert.DeserializeObject<PacketDataSpecCoin[]>(fileContents);
			LoadTableSpecCoin(lists);
		}
		else if (serverTable != null)
		{
			LocalDataManager.SaveFile(isResourcesFolder: false, LocalDataManager.FILE_NAME_TABLE_SPEC_COIN, JsonConvert.SerializeObject(serverTable.m_SPEC_COIN));
		}
		if (!listLoadedTable.Contains(LocalDataManager.FILE_NAME_TABLE_OPTION))
		{
			LoadTableSpecOption();
		}
		else if (serverTable != null)
		{
			LocalDataManager.SaveFile(isResourcesFolder: false, LocalDataManager.FILE_NAME_TABLE_OPTION, JsonConvert.SerializeObject(serverTable.m_SPEC_OPTION));
		}
		if (!listLoadedTable.Contains(LocalDataManager.FILE_NAME_TABLE_SPEC_ITEM))
		{
			string fileContents2 = LocalDataManager.GetFileContents(LocalDataManager.FILE_NAME_TABLE_SPEC_ITEM);
			PacketDataSpecItem items = JsonConvert.DeserializeObject<PacketDataSpecItem>(fileContents2);
			LoadTableSpecItem(items);
		}
		else if (serverTable != null)
		{
			LocalDataManager.SaveFile(isResourcesFolder: false, LocalDataManager.FILE_NAME_TABLE_SPEC_ITEM, JsonConvert.SerializeObject(serverTable.m_SPEC_ITEM));
		}
		if (!listLoadedTable.Contains(LocalDataManager.FILE_NAME_TABLE_LEVEL_DATA))
		{
			LoadGameLevelDataFromLocalFile();
		}
		if (!listLoadedTable.Contains(LocalDataManager.FILE_NAME_TABLE_MAP_DATA))
		{
			LoadGameMapDataFromLocalFile();
		}
		serverTable = null;
	}

	public bool LoadGameLevelDataFromLocalFile()
	{
        Debug.Log("JungleGame ServerDataTable.LoadGameLevelDataFromLocalFile");

		string fileContents = LocalDataManager.GetFileContents(LocalDataManager.FILE_NAME_TABLE_LEVEL_DATA);

        if (!string.IsNullOrEmpty(fileContents))
		{
			if (fileContents.Length > 10 && fileContents.Substring(0, 10).StartsWith("{\"game"))
			{
				try
				{
					ResPacketLoadGameData resPacketLoadGameData = JsonConvert.DeserializeObject<ResPacketLoadGameData>(fileContents);
					if (resPacketLoadGameData != null)
					{
						MapData.ReceiveServerGameData(resPacketLoadGameData.game);
                        Debug.Log("JungleGame ServerDataTable.LoadGameLevelDataFromLocalFile totalLevel1:" + resPacketLoadGameData.game.Length);
                    }
                }
				catch (Exception ex)
				{
                    Debug.Log("JungleGame ServerDataTable.LoadGameLevelDataFromLocalFile ex:" + ex.Message);
                }
            }
			else
			{
				PacketGameLevelData[] array = JsonConvert.DeserializeObject<PacketGameLevelData[]>(fileContents);
				if (array != null)
				{
					MapData.ReceiveServerGameData(array);
                    Debug.Log("JungleGame ServerDataTable.LoadGameLevelDataFromLocalFile totalLevel2:" + array.Length);
                }
            }
			return true;
		}
		return false;
	}

	public bool LoadGameMapDataFromLocalFile()
	{
        Debug.Log("JungleGame ServerDataTable.LoadGameMapDataFromLocalFile");

        string fileContents = LocalDataManager.GetFileContents(LocalDataManager.FILE_NAME_TABLE_MAP_DATA);
		if (!string.IsNullOrEmpty(fileContents))
		{
			if (fileContents.Length > 10 && fileContents.Substring(0, 10).StartsWith("{\"map"))
			{
				try
				{
					ResPacketLoadMapData resPacketLoadMapData = JsonConvert.DeserializeObject<ResPacketLoadMapData>(fileContents);
					MapData.ReceiveServerMapData(resPacketLoadMapData.map);
                }
                catch (Exception ex)
				{
                    Debug.Log("JungleGame ServerDataTable.LoadGameMapDataFromLocalFile ex:" + ex.Message);
                }
            }
			else
			{
				PacketGameMapData[] lists = JsonConvert.DeserializeObject<PacketGameMapData[]>(fileContents);
				MapData.ReceiveServerMapData(lists);
            }
            return true;
		}
		return false;
	}

	private void LoadTableSpecCoin(PacketDataSpecCoin[] lists)
	{
		if (lists != null)
		{
			m_dicTableCoin.Clear();
			foreach (PacketDataSpecCoin packetDataSpecCoin in lists)
			{
				m_dicTableCoin.Add(packetDataSpecCoin.lid, packetDataSpecCoin);
			}
			listLoadedTable.Add(LocalDataManager.FILE_NAME_TABLE_SPEC_COIN);
		}
	}

	public void LoadTableSpecOption()
	{
		string fileContents = LocalDataManager.GetFileContents(LocalDataManager.FILE_NAME_TABLE_OPTION);
		PacketDataOption[] lists = JsonConvert.DeserializeObject<PacketDataOption[]>(fileContents);
		LoadTableSpecOption(lists);
	}

	private void LoadTableSpecOption(PacketDataOption[] lists)
	{
		if (lists != null)
		{
			m_dicTableOption.Clear();
			listEnabledLocalPushIndex = null;
			foreach (PacketDataOption packetDataOption in lists)
			{
				m_dicTableOption.Add(packetDataOption.option_id, packetDataOption.option_value);
			}
			listLoadedTable.Add(LocalDataManager.FILE_NAME_TABLE_OPTION);
			SetOptionValue("MAX_LEVEL", ref MAX_LEVEL);
			SetOptionValue("SESSION_TIME_MINUTE", ref SessionTimeMinute);
			for (int j = 1; j <= 8; j++)
			{
				SetOptionValue("TUTORIAL_LV_" + j, ref TutorialOpenLevel[j - 1]);
			}
			SetOptionValue("Interstitial_GameClear_Count", ref InterstitalGameClearCount);
		}
	}

	public int GetTutorialIndex(int level)
	{
		for (int i = 0; i < TutorialOpenLevel.Length; i++)
		{
			if (TutorialOpenLevel[i] == level)
			{
				return i;
			}
		}
		return -1;
	}

	private void LoadTableSpecGameLevelData(PacketGameLevelData[] lists)
	{
		if (lists != null)
		{
			MapData.ReceiveServerGameData(lists);
			LocalDataManager.SaveFile(isResourcesFolder: false, LocalDataManager.FILE_NAME_TABLE_LEVEL_DATA, JsonConvert.SerializeObject(lists));
			listLoadedTable.Add(LocalDataManager.FILE_NAME_TABLE_LEVEL_DATA);
		}
	}

	private void LoadTableSpecGameMapData(PacketGameMapData[] lists)
	{
		if (lists != null)
		{
			MapData.ReceiveServerMapData(lists);
			LocalDataManager.SaveFile(isResourcesFolder: false, LocalDataManager.FILE_NAME_TABLE_MAP_DATA, JsonConvert.SerializeObject(lists));
			listLoadedTable.Add(LocalDataManager.FILE_NAME_TABLE_MAP_DATA);
		}
	}
	
	public void SetOptionValue(string key, ref int t)
	{
		if (m_dicTableOption.ContainsKey(key))
		{
			int.TryParse(m_dicTableOption[key], out t);
		}
	}

	public string GetLangValue(string key)
	{
		if (!m_dicTableLang.ContainsKey(key))
		{
			return string.Empty;
		}
		return m_dicTableLang[key].lang_value.Trim().Replace("\\n", "\n");
	}

	public void SetFont(Text objText)
	{
	}

	public void SetLangValue(Text objText, string key)
	{
		SetFont(objText);
		objText.text = GetLangValue(key);
	}

	public string GetLangValueParam(string key, int level = 0, int point = 0, int star = 0, int count = 0, int episode = 0, int stage = 0, string name = null, string user_name = null, string item_name = null, string time = null, int sale = 0, int coin = 0, int token = 0)
	{
		string text = GetLangValue(key);
		if (!string.IsNullOrEmpty(text))
		{
			for (int i = 0; i < listLangConvertFormatType.Count; i++)
			{
				if (text.Contains(listLangConvertFormatType[i]))
				{
					switch (i)
					{
					case 0:
						text = text.Replace(listLangConvertFormatType[i], level.ToString());
						break;
					case 1:
						text = text.Replace(listLangConvertFormatType[i], point.ToString());
						break;
					case 2:
						text = text.Replace(listLangConvertFormatType[i], star.ToString());
						break;
					case 3:
						text = text.Replace(listLangConvertFormatType[i], count.ToString());
						break;
					case 4:
						text = text.Replace(listLangConvertFormatType[i], episode.ToString());
						break;
					case 5:
						text = text.Replace(listLangConvertFormatType[i], stage.ToString());
						break;
					case 6:
						text = text.Replace(listLangConvertFormatType[i], name);
						break;
					case 7:
						text = text.Replace(listLangConvertFormatType[i], user_name);
						break;
					case 8:
						text = text.Replace(listLangConvertFormatType[i], item_name);
						break;
					case 10:
						text = text.Replace(listLangConvertFormatType[i], time);
						break;
					case 11:
						text = text.Replace(listLangConvertFormatType[i], sale.ToString());
						break;
					case 12:
						text = text.Replace(listLangConvertFormatType[i], coin.ToString());
						break;
					case 13:
						text = text.Replace(listLangConvertFormatType[i], token.ToString());
						break;
					}
				}
			}
		}
		return text;
	}

	public void SetLangValueParam(Text objText, string key, int level = 0, int point = 0, int star = 0, int count = 0, int episode = 0, int stage = 0, string name = null, string user_name = null, string item_name = null, string time = null, int sale = 0, int coin = 0, int token = 0)
	{
		SetFont(objText);
		objText.text = GetLangValueParam(key, level, point, star, count, episode, stage, name, user_name, item_name, time, sale, coin, token);
	}
}
