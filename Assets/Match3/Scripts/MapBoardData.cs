using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MapBoardData
{
	public IBlockType[,] blocks;

	public bool[,] bringDownGenerator;

	public bool[,] bringDownGoal;

	public int chipCount;

	public int[,] chips;

	public int[,] clothButton;

	public Dictionary<BoardPosition, MapDataGeneratorDrop> dicGeneratorDropBlock = new Dictionary<BoardPosition, MapDataGeneratorDrop>();

	public Dictionary<BoardPosition, MapDataGeneratorSpecialDrop> dicGeneratorSpecialDropBlock = new Dictionary<BoardPosition, MapDataGeneratorSpecialDrop>();

	public Dictionary<BoardPosition, BoardPosition> dicRailNextPosition = new Dictionary<BoardPosition, BoardPosition>();

	public int[] dropBlockProb = new int[MapData.MaxBlockColor];

	public DropDirection[,] dropDirection;

	public bool[,] dropLock;

	public int[] dropRandomChipColor;

	public bool[,] dropWallsH;

	public bool[,] dropWallsV;

	public int gateEnterX;

	public int gateEnterY;

	public int gateExitX;

	public int gateExitY;

	public int height;

	public int[,] inOrder;

	public bool isNewMap;

	public bool[,] jellyTile;

	public int[,] knot;

	public List<MapDataMovingSlot> listMovingSlot = new List<MapDataMovingSlot>();

	public List<MapDataRotationSlot> listRotationSlot = new List<MapDataRotationSlot>();

	public bool[,] milkTile;

	public IBlockType[,] orgBlocks;

	public int[,] orgChips;

	public RescueGingerManSize[,] orgRescueGingerManSize;

	public bool[,] orgSlots;

	public int[,] orgTunnel;

	public int[,] param1;

	public int[,] param2;

	public int[,] powerUps;

	public bool[,] rail;

	public int[,] railImage;

	public RescueGingerManSize[,] rescueGinerManSize;

	public int[,] rescueGingerManSubPosX;

	public int[,] rescueGingerManSubPosY;

	public int[,] rescueMouseRoadType;

	public int[,] ribbon;

	public int[,] rockCandyTile;

	public bool[,] safeObsSlot;

	public bool[,] slots;

	public int[,] teleport;

	public int[,] tunnel;

	public int tutorial1X = -1;

	public int tutorial1Y = -1;

	public int tutorial2X = -1;

	public int tutorial2Y = -1;

	public bool[,] wallsH;

	public bool[,] wallsV;

	public int width;

	public bool[,] yarn;

	private static readonly string[] blockListNames = new string[27]
		{
			"N1",
			"N2",
			"N3",
			"N4",
			"N5",
			"N6",
			"N1H",
			"N2H",
			"N3H",
			"N4H",
			"N5H",
			"N6H",
			"N1V",
			"N2V",
			"N3V",
			"N4V",
			"N5V",
			"N6V",
			"N1B",
			"N2B",
			"N3B",
			"N4B",
			"N5B",
			"N6B",
			"N0W",
			"N0L",
			"N0R"
		};

	public static int GetGeneratorSpecialBlockIndex(string strBlockName)
	{
		for (int i = 0; i < blockListNames.Length; i++)
		{
			if (blockListNames[i] == strBlockName)
			{
				return i;
			}
		}
		return -1;
	}

	public MapBoardData()
	{
		width = MapData.MaxWidth;
		height = MapData.MaxHeight;
		dropBlockProb[0] = (dropBlockProb[1] = (dropBlockProb[2] = (dropBlockProb[3] = 25)));
		dropBlockProb[4] = (dropBlockProb[5] = 0);
		slots = new bool[width, height];
		gateEnterX = (gateEnterY = (gateExitX = (gateExitY = -1)));
		teleport = new int[width, height];
		chips = new int[width, height];
		powerUps = new int[width, height];
		blocks = new IBlockType[width, height];
		wallsV = new bool[width, height];
		wallsH = new bool[width, height];
		rockCandyTile = new int[width, height];
		jellyTile = new bool[width, height];
		milkTile = new bool[width, height];
		rescueGinerManSize = new RescueGingerManSize[width, height];
		rescueGingerManSubPosX = new int[width, height];
		rescueGingerManSubPosY = new int[width, height];
		rescueMouseRoadType = new int[width, height];
		dropWallsH = new bool[width, height];
		dropWallsV = new bool[width, height];
		rail = new bool[width, height];
		railImage = new int[width, height];
		dropDirection = new DropDirection[width, height];
		dropLock = new bool[width, height];
		bringDownGenerator = new bool[width, height];
		bringDownGoal = new bool[width, height];
		safeObsSlot = new bool[width, height];
		tunnel = new int[width, height];
		ribbon = new int[width, height];
		knot = new int[width, height];
		yarn = new bool[width, height];
		clothButton = new int[width, height];
		inOrder = new int[width, height];
		param1 = new int[width, height];
		param2 = new int[width, height];
		dicGeneratorDropBlock.Clear();
		dicGeneratorSpecialDropBlock.Clear();
		listRotationSlot.Clear();
		listMovingSlot.Clear();
		dicRailNextPosition.Clear();
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				slots[i, j] = true;
				dropDirection[i, j] = DropDirection.Down;
				chips[i, j] = 0;
			}
		}
	}

	public MapBoardData(PacketGameMapData packetData)
		: this()
	{
		#region NotExecuted
		if (!string.IsNullOrEmpty(packetData.road) && packetData.road != "NULL")
		{
			string[] array = packetData.road.Split(':');
			if (array[0].Length <= 2)
			{
				int.TryParse(array[0].Substring(0, 1), out gateEnterX);
				int.TryParse(array[0].Substring(1, 1), out gateEnterY);
			}
			if (array[1].Length <= 2)
			{
				int.TryParse(array[1].Substring(0, 1), out gateExitX);
				int.TryParse(array[1].Substring(1, 1), out gateExitY);
			}
		}
		#endregion

		string text = (!string.IsNullOrEmpty(packetData.board_data)) ? packetData.board_data : string.Empty;
		if (!string.IsNullOrEmpty(text) && text != "NULL")
		{
			string[] array2 = text.Split(',');

			if (array2.Length != MapData.MaxWidth * MapData.MaxHeight)
			{
			}

			for (int i = 0; i < array2.Length; i++)
			{
				int num = i % MapData.MaxWidth;
				int num2 = i / MapData.MaxWidth;
				string[] array3 = array2[i].Split(':');
				string text2 = array3[0];

				if (text2 == "0")
				{
					slots[num, num2] = false;
					continue;
				}

				slots[num, num2] = true;

				if (text2[0] == 'E')
				{
					chips[num, num2] = -1;
				}
				else if (text2[0] == 'N')
				{
					if (text2.Length == 3 && text2.Substring(1, 2) == "-1")
					{
						chips[num, num2] = 0;
					}
					else
					{
						chips[num, num2] = int.Parse(text2[1].ToString());

						//ChipType chipType = (ChipType)chips[num, num2];

						//if (chipType == ChipType.SimpleBomb)
      //                  {
						//	Debug.Log(packetData.gid + "-" + chipType + "-" + num + "-" + num2);
						//}                                           
                    }

					if (text2.Length > 2)
					{
						if (text2[2] == 'H')
						{
							powerUps[num, num2] = 4;
						}
						else if (text2[2] == 'V')
						{
							powerUps[num, num2] = 5;
						}
						else if (text2[2] == 'B')
						{
							powerUps[num, num2] = 1;
						}
						else if (text2[2] == 'W')
						{
							powerUps[num, num2] = 3;
						}
						else if (text2[2] == 'L')
						{
							powerUps[num, num2] = 10;
						}
						else if (text2[2] == 'D')
						{
							powerUps[num, num2] = 11;
						}
						else if (text2[2] == 'R')
						{
							powerUps[num, num2] = 12;
						}
						else if (text2[2] == 'O')
						{
							powerUps[num, num2] = 13;
						}
					}
				}
				else if (text2[0] == 'O')
				{
					IBlockType blockTypeFromString = MapData.GetBlockTypeFromString(array3[0]);
					blocks[num, num2] = blockTypeFromString;

					if (array3.Length > 1 && MapData.IsNumberChocolate(blockTypeFromString))
					{
						int.TryParse(array3[1], out inOrder[num, num2]);
					}
				}

				
				for (int j = 1; j < array3.Length; j++)
				{
					bool flag = false;

					if (array3[j].StartsWith("RBN"))
					{
						flag = true;
						int result = -1;
						int.TryParse(array3[j].Substring(3, 2), out result);

						if (result != -1)
						{
							ribbon[num, num2] = result;
						}

						if (array3[j].Length == 7)
						{
							result = -1;
							int.TryParse(array3[j].Substring(6, 1), out result);

							if (result != -1)
							{
								knot[num, num2] = result;
							}
						}
					}
					else if (array3[j].StartsWith("YARN"))
					{
						flag = true;
						yarn[num, num2] = true;
						int result2 = -1;

						if (array3[j].Length == 6)
						{
							int.TryParse(array3[j].Substring(5, 1), out result2);

							if (result2 != -1)
							{
								clothButton[num, num2] = result2;
							}
						}
					}
					else if (array3[j].StartsWith("RAIL"))
					{
						flag = true;
						int result3 = -1;
						int.TryParse(array3[j][4].ToString(), out int result4);
						int.TryParse(array3[j][5].ToString(), out result3);

						if (result4 != -1 && result3 != -1)
						{
							SetRailNextPosition(num, num2, result4, result3);
						}

						if (array3[j].Length == 8)
						{
							int result5 = -1;
							int.TryParse(array3[j].Substring(6, 2), out result5);

							if (result5 != -1)
							{
								railImage[num, num2] = result5;
							}
						}
					}
					else if (array3[j].StartsWith("TN"))
					{
						flag = true;
						int result6 = -1;
						int.TryParse(array3[j].Substring(2, 1), out result6);

						if (result6 != -1)
						{
							tunnel[num, num2] = result6;
						}
					}
					else if (array3[j].StartsWith("SAFE"))
					{
						flag = true;
						safeObsSlot[num, num2] = true;
					}
					else if (array3[j] == "MK")
					{
						milkTile[num, num2] = true;
					}
					else if (array3[j] == "WH")
					{
						wallsH[num, num2] = true;
					}
					else if (array3[j] == "WV")
					{
						wallsV[num, num2] = true;
					}

					if (flag)
					{
						continue;
					}

					switch (array3[j][0])
					{
					case 'D':
						if (array3[j][1] == 'R')
						{
							dropDirection[num, num2] = DropDirection.Right;
						}
						else if (array3[j][1] == 'L')
						{
							dropDirection[num, num2] = DropDirection.Left;
						}
						else if (array3[j][1] == 'U')
						{
							dropDirection[num, num2] = DropDirection.Up;
						}
						else if (array3[j][1] == 'D')
						{
							dropDirection[num, num2] = DropDirection.Down;
						}
						if (array3[j].Length == 3 && array3[j][2] == 'L')
						{
							dropLock[num, num2] = true;
						}
						break;
					case 'J':
						jellyTile[num, num2] = true;
						break;
					case 'G':
						if (array3[j].Length == 1)
						{
							AddGeneratorDropList(new BoardPosition(num, num2));
						}
						else if (array3[j].Length > 1 && array3[j] == "GS")
						{
							AddGeneratorSpecialDropList(new BoardPosition(num, num2));
						}
						break;
					case 'O':
						blocks[num, num2] = MapData.GetBlockTypeFromString(array3[j]);

						if (blocks[num, num2] == IBlockType.PastryBag)
						{
							string[] array4 = array3[j].Split('.');
							int result7 = 1;
							int.TryParse(array4[1], out result7);
							int result8 = 1;
							int.TryParse(array4[2], out result8);
							param1[num, num2] = result7;
							param2[num, num2] = result8;
						}
						break;
					case 'R':
						if (array3[j].Length == 4)
						{
							int result9 = 0;
							int.TryParse(array3[j][2].ToString(), out int result10);
							int.TryParse(array3[j][3].ToString(), out result9);
							if (result10 != 0 && result9 != 0)
							{
								rescueGinerManSize[num, num2] = Utils.GetRescueGingerManSize(result10, result9);
							}
						}
						break;
					default:
						if (array3[j] == "B1")
						{
							rockCandyTile[num, num2] = 1;
						}
						else if (array3[j] == "B2")
						{
							rockCandyTile[num, num2] = 2;
						}
						else if (array3[j] == "BDS")
						{
							bringDownGenerator[num, num2] = true;
						}
						else if (array3[j] == "BDE")
						{
							bringDownGoal[num, num2] = true;
						}
						break;
					}
				}
			}
		}

        #region NotExecuted
        string text3 = (!string.IsNullOrEmpty(packetData.moving)) ? packetData.moving : string.Empty;	
		if (!string.IsNullOrEmpty(text3) && text3 != "NULL")
		{
			string[] array5 = text3.Split(',');
			for (int k = 0; k < array5.Length; k++)
			{
				string[] array6 = array5[k].Split(':');
				int.TryParse(array6[0].Substring(0, 1), out int result11);
				int.TryParse(array6[0].Substring(1, 1), out int result12);
				int.TryParse(array6[0].Substring(2, 1), out int result13);
				int.TryParse(array6[0].Substring(3, 1), out int result14);
				int.TryParse(array6[1].Substring(0, 1), out int result15);
				int.TryParse(array6[1].Substring(1, 1), out int result16);
				listMovingSlot.Add(new MapDataMovingSlot(result11, result12, result13, result14, result15, result16));
			}
		}
		#endregion

		#region NotExecuted
		string text4 = (!string.IsNullOrEmpty(packetData.rotation)) ? packetData.rotation : string.Empty;
		if (!string.IsNullOrEmpty(text4) && text4 != "NULL")
		{
			string[] array7 = text4.Split(',');
			bool flag2 = false;
			for (int l = 0; l < array7.Length; l++)
			{
				string[] array8 = array7[l].Split(':');
				int.TryParse(array8[0].Substring(0, 1), out int result17);
				int.TryParse(array8[0].Substring(1, 1), out int result18);
				int.TryParse(array8[1].Substring(0, 1), out int result19);
				flag2 = ((array8[1][1] == 'C') ? true : false);
				listRotationSlot.Add(new MapDataRotationSlot(result19 % 2 == 0, result17, result18, result19, flag2));
			}
		}
        #endregion

        string text5 = (!string.IsNullOrEmpty(packetData.drop_block_prob)) ? packetData.drop_block_prob : string.Empty;
		if (!string.IsNullOrEmpty(text5) && text5 != "NULL")
		{
			string[] array9 = text5.Split(',');
			for (int m = 0; m < array9.Length; m++)
			{
				int.TryParse(array9[m], out dropBlockProb[m]);
			}
		}

		#region NotExecuted
		string text6 = (!string.IsNullOrEmpty(packetData.drop)) ? packetData.drop : string.Empty;
		if (!string.IsNullOrEmpty(text6) && text6 != "NULL")
		{
			string[] array10 = text6.Split(',');
			BoardPosition key = default(BoardPosition);
			for (int n = 0; n < array10.Length; n++)
			{
				if (array10[n].Length <= 3)
				{
					continue;
				}
				string a = array10[n].Substring(0, 3);
				if (a == "GS|")
				{
					int.TryParse(array10[n].Substring(3, 1), out key.x);
					int.TryParse(array10[n].Substring(4, 1), out key.y);
					if (dicGeneratorSpecialDropBlock.ContainsKey(key))
					{
						string[] array11 = array10[n].Split(':');
						if (array11.Length > 0)
						{
							array11[0] = array11[0].Substring(6);
						}
						for (int num3 = 0; num3 < array11.Length; num3++)
						{
							string[] array12 = array11[num3].Split('-');
							int generatorSpecialBlockIndex = GetGeneratorSpecialBlockIndex(array12[0]);
							dicGeneratorSpecialDropBlock[key].dropBlocks[generatorSpecialBlockIndex].chipType = MapData.GetChipTypeFromString(array12[0]);
							dicGeneratorSpecialDropBlock[key].dropBlocks[generatorSpecialBlockIndex].chipColor = MapData.GetChipColorFromString(array12[0]);
							dicGeneratorSpecialDropBlock[key].dropBlocks[generatorSpecialBlockIndex].prob = 0;
							int.TryParse(array12[1], out dicGeneratorSpecialDropBlock[key].dropBlocks[generatorSpecialBlockIndex].prob);
						}
					}
					continue;
				}
				int.TryParse(array10[n].Substring(0, 1), out key.x);
				int.TryParse(array10[n].Substring(1, 1), out key.y);
				if (!dicGeneratorDropBlock.ContainsKey(key))
				{
					continue;
				}
				string[] array13 = array10[n].Split(':');
				if (array13.Length > 0)
				{
					array13[0] = array13[0].Substring(3);
				}
				for (int num4 = 0; num4 < array13.Length; num4++)
				{
					if (array13[num4] != "0")
					{
						dicGeneratorDropBlock[key].dropBlocks[num4].chipType = MapData.GetChipTypeFromString(array13[num4]);
						dicGeneratorDropBlock[key].dropBlocks[num4].chipColor = MapData.GetChipColorFromString(array13[num4]);
					}
				}
			}
		}

		string text7 = (!string.IsNullOrEmpty(packetData.map_param)) ? packetData.map_param : string.Empty;
		if (string.IsNullOrEmpty(text7) || !(text7 != "NULL"))
		{
			return;
		}
        

        string[] array14 = text7.Split('/');
		for (int num5 = 0; num5 < array14.Length; num5++)
		{
			if (!array14[num5].StartsWith("Tutorial"))
			{
				continue;
			}
			string[] array15 = array14[num5].Split(':');
			if (array15.Length == 2)
			{
				string[] array16 = array15[1].Split(',');
				if (array16.Length == 4)
				{
					int.TryParse(array16[0], out tutorial1X);
					int.TryParse(array16[1], out tutorial1Y);
					int.TryParse(array16[2], out tutorial2X);
					int.TryParse(array16[3], out tutorial2Y);
				}
			}
		}
		#endregion
	}

	public bool GetSlot(int x, int y)
	{
		if (x >= 0 && x < width && y >= 0 && y < height)
		{
			return slots[x, y];
		}
		return false;
	}

	public int GetChip(int x, int y)
	{
		if (x >= 0 && x < width && y >= 0 && y < height)
		{
			return chips[x, y];
		}
		return 0;
	}

	public int GetChipDig(int digGridX, int digGridY)
	{
		if (digGridX >= 0 && digGridX < MapData.main.widthDig && digGridY >= 0 && digGridY < MapData.main.heightDig)
		{
			return MapData.main.chipsDig[digGridX, digGridY];
		}
		return 0;
	}

	public void GenerateRandomList(ref int[] randomList, int[] objProb)
	{
		int num = 0;
		for (int i = 0; i < objProb.Length; i++)
		{
			num += objProb[i];
		}
		randomList = new int[num];
		int num2 = 0;
		for (int j = 0; j < objProb.Length; j++)
		{
			for (int k = 0; k < objProb[j]; k++)
			{
				randomList[num2++] = j;
			}
		}
	}

	public void NewRandomChipFirstGeneration(int x, int y, bool unmatching)
	{
		if (chips[x, y] == -1)
		{
			chips[x, y] = 0;
		}
		chips[x, y] = GetRandomChip() + 1;
		if (unmatching)
		{
			int num = 0;
			while ((GetChip(x, y) == GetChip(x, y + 1) && GetChip(x, y) == GetChip(x, y + 2)) || (GetChip(x, y) == GetChip(x + 1, y) && GetChip(x, y) == GetChip(x + 2, y)) || (GetChip(x, y) == GetChip(x, y - 1) && GetChip(x, y) == GetChip(x, y - 2)) || (GetChip(x, y) == GetChip(x - 1, y) && GetChip(x, y) == GetChip(x - 2, y)) || (GetChip(x, y) == GetChip(x - 1, y) && GetChip(x, y) == GetChip(x + 1, y)) || (GetChip(x, y) == GetChip(x, y - 1) && GetChip(x, y) == GetChip(x, y + 1)) || (GetChip(x, y) == GetChip(x - 1, y + 1) && GetChip(x, y) == GetChip(x, y + 1) && GetChip(x, y) == GetChip(x - 1, y)) || (GetChip(x, y) == GetChip(x, y + 1) && GetChip(x, y) == GetChip(x + 1, y + 1) && GetChip(x, y) == GetChip(x + 1, y)) || (GetChip(x, y) == GetChip(x - 1, y) && GetChip(x, y) == GetChip(x - 1, y - 1) && GetChip(x, y) == GetChip(x, y - 1)) || (GetChip(x, y) == GetChip(x, y - 1) && GetChip(x, y) == GetChip(x + 1, y - 1) && GetChip(x, y) == GetChip(x + 1, y) && num++ < 100))
			{
				chips[x, y] = GetRandomChip() + 1;
			}
		}
	}

	public int GetRandomChip()
	{
		return dropRandomChipColor[Random.Range(0, dropRandomChipColor.Length)];
	}

	public void AddChipTypeInRandomList()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < MapData.MaxBlockColor; i++)
		{
			list.Add(i);
		}
		for (int j = 0; j < dropRandomChipColor.Length; j++)
		{
			for (int k = 0; k < MapData.MaxBlockColor; k++)
			{
				if (dropRandomChipColor[j] == k && list.Contains(k))
				{
					list.Remove(k);
				}
			}
		}
		if (list.Count > 0)
		{
			int item = list[Random.Range(0, list.Count)];
			List<int> list2 = dropRandomChipColor.ToList();
			for (int l = 0; l < 20; l++)
			{
				list2.Add(item);
			}
			dropRandomChipColor = list2.ToArray();
		}
		if (BoardManager.main.dicGeneratorSpecialDrop.Count > 0)
		{
			list.Clear();
			for (int m = 1; m <= MapData.MaxBlockColor; m++)
			{
				list.Add(m);
			}
			foreach (MapDataGeneratorSpecialDrop value in BoardManager.main.dicGeneratorSpecialDrop.Values)
			{
				for (int n = 0; n < value.dropBlocks.Length; n++)
				{
					if (value.dropBlocks[n].prob > 0 && list.Contains(value.dropBlocks[n].chipColor))
					{
						list.Remove(value.dropBlocks[n].chipColor);
					}
				}
			}
			if (list.Count > 0)
			{
				int num = list[Random.Range(0, list.Count)];
				foreach (MapDataGeneratorSpecialDrop value2 in BoardManager.main.dicGeneratorSpecialDrop.Values)
				{
					value2.totalProb += 30;
					value2.dropBlocks[num].prob = 30;
					value2.dropBlocks[num].chipColor = num;
					value2.dropBlocks[num].chipType = ChipType.SimpleChip;
				}
			}
		}
	}

	public int GetRandomChipExceptOneColor(int exceptColorId)
	{
		if (dropRandomChipColor.Length == 1)
		{
			return exceptColorId;
		}
		int num = -1;
		do
		{
			num = GetRandomChip();
		}
		while (num == exceptColorId);
		return num;
	}

	public int GetRandomBringDown()
	{
		return MapData.main.dropRandomBringDown[Random.Range(0, MapData.main.dropRandomBringDown.Length)];
	}

	public void FirstChipGeneration()
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				if (!slots[i, j] || (blocks[i, j] != 0 && !MapData.IsBlockTypeIncludingChip(blocks[i, j])))
				{
					chips[i, j] = -1;
				}
				if (MapData.IsBlockTypeIncludingChip(blocks[i, j]) && chips[i, j] == -1)
				{
					chips[i, j] = 0;
				}
				if (MapData.IsNumberChocolate(blocks[i, j]) && inOrder[i, j] == 1)
				{
					chips[i, j] = (int)(blocks[i, j] - 70 + 1);
				}
			}
		}
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				if (chips[i, j] == 0 && chips[i, j] != 9 && chips[i, j] != 10)
				{
					NewRandomChipFirstGeneration(i, j, unmatching: true);
				}
			}
		}
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				if (chips[i, j] > 0 && chips[i, j] != 9 && chips[i, j] != 10)
				{
					chips[i, j]--;
				}
			}
		}
	}

	public bool IsExitPosition(int x, int y)
	{
		if (x == gateExitX && y == gateExitY)
		{
			return true;
		}
		return false;
	}

	public void SetRescueBear(int x, int y, int sizeW, int sizeH)
	{
		rescueGinerManSize[x, y] = Utils.GetRescueGingerManSize(sizeW, sizeH);
	}

	public void RemoveRescueBear(int x, int y)
	{
		rescueGinerManSize[x, y] = RescueGingerManSize.Null;
	}

	public void SetChip(int x, int y, ChipType _chipType, int _chipID)
	{
		switch (_chipType)
		{
		case ChipType.Empty:
			chips[x, y] = -1;
			powerUps[x, y] = 0;
			break;
		case ChipType.None:
			chips[x, y] = 0;
			powerUps[x, y] = 0;
			break;
		case ChipType.SimpleChip:
			chips[x, y] = _chipID;
			powerUps[x, y] = 0;
			break;
		case ChipType.HBomb:
			chips[x, y] = _chipID;
			powerUps[x, y] = 4;
			break;
		case ChipType.VBomb:
			chips[x, y] = _chipID;
			powerUps[x, y] = 5;
			break;
		case ChipType.SimpleBomb:
			chips[x, y] = _chipID;
			powerUps[x, y] = 1;
			break;
		case ChipType.RainbowBomb:
			chips[x, y] = 0;
			powerUps[x, y] = 3;
			break;
		case ChipType.CandyChip:
			chips[x, y] = 0;
			powerUps[x, y] = 10;
			break;
		case ChipType.BringDown:
			chips[x, y] = 0;
			powerUps[x, y] = 11;
			break;
		case ChipType.ChamelonChip:
			chips[x, y] = 0;
			powerUps[x, y] = 12;
			break;
		case ChipType.OreoCracker:
			chips[x, y] = 0;
			powerUps[x, y] = 13;
			break;
		}
		if (MapData.GetBlockLayerNo(blocks[x, y]) != 2)
		{
			blocks[x, y] = IBlockType.None;
		}
	}

	public void SetBlock(int x, int y, IBlockType _iblockType)
	{
		blocks[x, y] = _iblockType;
	}

	public void SetRailNextPosition(int x, int y, int nextX, int nextY)
	{
		BoardPosition key = new BoardPosition(x, y);
		if (dicRailNextPosition.ContainsKey(key))
		{
			BoardPosition value = dicRailNextPosition[key];
			value.x = nextX;
			value.y = nextY;
			dicRailNextPosition[key] = value;
		}
		else
		{
			dicRailNextPosition.Add(key, new BoardPosition(nextX, nextY));
		}
		rail[x, y] = true;
	}

	public void RemoveRail(int x, int y)
	{
		BoardPosition key = new BoardPosition(x, y);
		if (dicRailNextPosition.ContainsKey(key))
		{
			dicRailNextPosition.Remove(key);
		}
		rail[x, y] = false;
		railImage[x, y] = 0;
	}

	public void InitForGamePlay()
	{
		chipCount = GetChipCount();
		GenerateRandomList(ref dropRandomChipColor, dropBlockProb);
		GenerateRandomList(ref MapData.main.dropRandomBringDown, MapData.main.dropBringDownProb);
		for (int i = 0; i < listMovingSlot.Count; i++)
		{
			listMovingSlot[i].InitForGamePlay();
		}
		for (int j = 0; j < listRotationSlot.Count; j++)
		{
			listRotationSlot[j].InitForGamePlay();
		}
		if (MapData.main.target == GoalTarget.Digging)
		{
			MapData.main.InitForDiggingMode();
		}
	}

	public int GetChipCount()
	{
		int num = 0;
		for (int i = 0; i < dropBlockProb.Length; i++)
		{
			if (dropBlockProb[i] > 0)
			{
				num++;
			}
		}
		return num;
	}

	public void AddGeneratorDropList(BoardPosition pos)
	{
		if (!dicGeneratorDropBlock.ContainsKey(pos))
		{
			dicGeneratorDropBlock.Add(pos, new MapDataGeneratorDrop(pos.x, pos.y));
		}
	}

	public void RemoveGeneratorDropList(BoardPosition pos)
	{
		if (dicGeneratorDropBlock.ContainsKey(pos))
		{
			dicGeneratorDropBlock.Remove(pos);
		}
	}

	public void SetGeneratorDropBlock(BoardPosition pos, int index, string blockName)
	{
		if (!dicGeneratorDropBlock.ContainsKey(pos))
		{
			AddGeneratorDropList(pos);
		}
		dicGeneratorDropBlock[pos].dropBlocks[index].chipType = MapData.GetChipTypeFromString(blockName);
		dicGeneratorDropBlock[pos].dropBlocks[index].chipColor = MapData.GetChipColorFromString(blockName);
	}

	public void AddGeneratorSpecialDropList(BoardPosition pos)
	{
		if (!dicGeneratorSpecialDropBlock.ContainsKey(pos))
		{
			dicGeneratorSpecialDropBlock.Add(pos, new MapDataGeneratorSpecialDrop(pos.x, pos.y));
		}
	}

	public void RemoveGeneratorSpecialDropList(BoardPosition pos)
	{
		if (dicGeneratorSpecialDropBlock.ContainsKey(pos))
		{
			dicGeneratorSpecialDropBlock.Remove(pos);
		}
	}

	public void SetGeneratorSpecialDropBlock(BoardPosition pos, int index, string blockName, int prob)
	{
		if (!dicGeneratorSpecialDropBlock.ContainsKey(pos))
		{
			AddGeneratorSpecialDropList(pos);
		}
		dicGeneratorSpecialDropBlock[pos].dropBlocks[index].chipType = MapData.GetChipTypeFromString(blockName);
		dicGeneratorSpecialDropBlock[pos].dropBlocks[index].chipColor = MapData.GetChipColorFromString(blockName);
		dicGeneratorSpecialDropBlock[pos].dropBlocks[index].prob = prob;
	}

	public void AddRotationSlot(bool isGrid, int cx, int cy, int size, bool isClockwork)
	{
		if (listRotationSlot != null && listRotationSlot.Count > 0)
		{
			MapDataRotationSlot mapDataRotationSlot = listRotationSlot.SingleOrDefault((MapDataRotationSlot c) => c.centerX == cx && c.centerY == cy);
			if (mapDataRotationSlot != null)
			{
				return;
			}
		}
		listRotationSlot.Add(new MapDataRotationSlot(isGrid, cx, cy, size, isClockwork));
	}

	public void RemoveRotationSlot(int cx, int cy)
	{
		if (listRotationSlot != null && listRotationSlot.Count > 0)
		{
			MapDataRotationSlot mapDataRotationSlot = listRotationSlot.SingleOrDefault((MapDataRotationSlot c) => c.centerX == cx && c.centerY == cy);
			if (mapDataRotationSlot != null)
			{
				listRotationSlot.Remove(mapDataRotationSlot);
			}
		}
	}

	//public void AddMovingSlot(int sx, int sy, MapToolMainMenuSlotSubMenuMoving.MovingDirection movingDirection, int sizeWidth, int sizeHeight, int moveCount)
	//{
	//	if (listMovingSlot != null && listMovingSlot.Count > 0)
	//	{
	//		MapDataMovingSlot mapDataMovingSlot = listMovingSlot.SingleOrDefault((MapDataMovingSlot c) => c.startX == sx && c.startY == sy);
	//		if (mapDataMovingSlot != null)
	//		{
	//			return;
	//		}
	//	}
	//	int num = sx;
	//	int num2 = sy;
	//	switch (movingDirection)
	//	{
	//	case MapToolMainMenuSlotSubMenuMoving.MovingDirection.Right:
	//		num += moveCount;
	//		break;
	//	case MapToolMainMenuSlotSubMenuMoving.MovingDirection.Left:
	//		num -= moveCount;
	//		break;
	//	case MapToolMainMenuSlotSubMenuMoving.MovingDirection.Down:
	//		num2 -= moveCount;
	//		break;
	//	case MapToolMainMenuSlotSubMenuMoving.MovingDirection.Up:
	//		num2 += moveCount;
	//		break;
	//	}
	//	listMovingSlot.Add(new MapDataMovingSlot(sx, sy, num, num2, sizeWidth, sizeHeight));
	//}

	public void RemoveMovingSlot(int sx, int sy)
	{
		if (listMovingSlot != null && listMovingSlot.Count > 0)
		{
			MapDataMovingSlot mapDataMovingSlot = listMovingSlot.SingleOrDefault((MapDataMovingSlot c) => c.startX == sx && c.startY == sy);
			if (mapDataMovingSlot != null)
			{
				listMovingSlot.Remove(mapDataMovingSlot);
			}
		}
	}

	public string GetSlotInfoJsonFormat(int x, int y, GoalTarget goalTarget)
	{
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		StringBuilder stringBuilder3 = new StringBuilder();
		StringBuilder stringBuilder4 = new StringBuilder();
		StringBuilder stringBuilder5 = new StringBuilder();
		StringBuilder stringBuilder6 = new StringBuilder();
		StringBuilder stringBuilder7 = new StringBuilder();
		StringBuilder stringBuilder8 = new StringBuilder();
		StringBuilder stringBuilder9 = new StringBuilder();
		StringBuilder stringBuilder10 = new StringBuilder();
		BoardPosition key = new BoardPosition(x, y);
		if (!slots[x, y])
		{
			stringBuilder.Append("0");
		}
		else
		{
			if (powerUps[x, y] == 0 && (chips[x, y] == 0 || chips[x, y] == -1))
			{
				if (chips[x, y] == -1)
				{
					stringBuilder.Append("E");
				}
				else
				{
					stringBuilder.Append("R");
				}
			}
			else
			{
				ChipType chipTypeFromPower = MapData.GetChipTypeFromPower((Powerup)powerUps[x, y]);
				stringBuilder.Append(MapData.GetChipJsonFormat(chipTypeFromPower, chips[x, y]));
			}
			if (dicGeneratorSpecialDropBlock.ContainsKey(key))
			{
				stringBuilder3.Append("GS");
			}
			else if (dicGeneratorDropBlock.ContainsKey(key))
			{
				stringBuilder3.Append("G");
			}
			if (dropDirection[x, y] == DropDirection.Left)
			{
				stringBuilder4.Append("DL");
			}
			else if (dropDirection[x, y] == DropDirection.Right)
			{
				stringBuilder4.Append("DR");
			}
			else if (dropDirection[x, y] == DropDirection.Up)
			{
				stringBuilder4.Append("DU");
			}
			if (dropLock[x, y])
			{
				if (dropDirection[x, y] == DropDirection.Down)
				{
					stringBuilder4.Append("DD");
				}
				stringBuilder4.Append("L");
			}
			if (blocks[x, y] != 0 && blocks[x, y] != IBlockType.Ribbon)
			{
				if (MapData.GetBlockLayerNo(blocks[x, y]) == 1)
				{
					stringBuilder.Remove(0, stringBuilder.Length);
					if (MapData.IsNumberChocolate(blocks[x, y]))
					{
						stringBuilder.Append(MapData.GetBlockJsonFormat(blocks[x, y]) + ":" + inOrder[x, y]);
					}
					else
					{
						stringBuilder.Append(MapData.GetBlockJsonFormat(blocks[x, y]));
					}
				}
				else
				{
					stringBuilder2.Append(MapData.GetBlockJsonFormat(blocks[x, y]));
				}
			}
			if (rescueGinerManSize[x, y] != 0)
			{
				stringBuilder5.Append("RB" + Utils.GetRescueGingerManSizeWidth(rescueGinerManSize[x, y]) + Utils.GetRescueGingerManSizeHeight(rescueGinerManSize[x, y]));
			}
			if (goalTarget == GoalTarget.BringDown || goalTarget == GoalTarget.CollectCracker)
			{
				if (bringDownGenerator[x, y])
				{
					stringBuilder6.Append("BDS");
				}
				else if (bringDownGoal[x, y])
				{
					stringBuilder6.Append("BDE");
				}
			}
			if (dicRailNextPosition.ContainsKey(key))
			{
				StringBuilder stringBuilder11 = stringBuilder7;
				BoardPosition boardPosition = dicRailNextPosition[key];
				object arg = boardPosition.x;
				BoardPosition boardPosition2 = dicRailNextPosition[key];
				stringBuilder11.AppendFormat("RAIL{0}{1}", arg, boardPosition2.y);
				if (railImage[x, y] > 0)
				{
					stringBuilder7.AppendFormat("{0:00}", railImage[x, y]);
				}
			}
			if (tunnel[x, y] > 0)
			{
				stringBuilder8.Append("TN" + tunnel[x, y]);
			}
			if (ribbon[x, y] > 0)
			{
				stringBuilder9.AppendFormat("RBN{0:00}", ribbon[x, y]);
				if (knot[x, y] > 0)
				{
					stringBuilder9.Append("." + knot[x, y]);
				}
			}
			if (yarn[x, y])
			{
				stringBuilder10.Append("YARN");
				if (clothButton[x, y] > 0)
				{
					stringBuilder10.Append("." + clothButton[x, y]);
				}
			}
			if (safeObsSlot[x, y])
			{
				stringBuilder7.Append("SAFE");
			}
		}
		if (stringBuilder3.Length > 0)
		{
			stringBuilder.Append(":").Append(stringBuilder3);
		}
		if (stringBuilder4.Length > 0)
		{
			stringBuilder.Append(":").Append(stringBuilder4);
		}
		if (stringBuilder2.Length > 0)
		{
			stringBuilder.Append(":").Append(stringBuilder2);
		}
		if (stringBuilder5.Length > 0)
		{
			stringBuilder.Append(":").Append(stringBuilder5);
		}
		if (rockCandyTile[x, y] > 0)
		{
			stringBuilder.Append(":B" + rockCandyTile[x, y]);
		}
		if (jellyTile[x, y])
		{
			stringBuilder.Append(":J");
		}
		if (milkTile[x, y])
		{
			stringBuilder.Append(":MK");
		}
		if (stringBuilder6.Length > 0)
		{
			stringBuilder.Append(":").Append(stringBuilder6);
		}
		if (stringBuilder7.Length > 0)
		{
			stringBuilder.Append(":").Append(stringBuilder7);
		}
		if (stringBuilder8.Length > 0)
		{
			stringBuilder.Append(":").Append(stringBuilder8);
		}
		if (stringBuilder9.Length > 0)
		{
			stringBuilder.Append(":").Append(stringBuilder9);
		}
		if (stringBuilder10.Length > 0)
		{
			stringBuilder.Append(":").Append(stringBuilder10);
		}
		if (wallsH[x, y])
		{
			stringBuilder.Append(":").Append("WH");
		}
		if (wallsV[x, y])
		{
			stringBuilder.Append(":").Append("WV");
		}
		return stringBuilder.ToString();
	}

	public string GetBoardJsonFormat(GoalTarget goalTarget)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < MapData.MaxHeight; i++)
		{
			for (int j = 0; j < MapData.MaxWidth; j++)
			{
				if (j != 0 || i != 0)
				{
					stringBuilder.Append(",");
				}
				stringBuilder.Append(GetSlotInfoJsonFormat(j, i, goalTarget));
			}
		}
		return stringBuilder.ToString();
	}

	public string GetRoadJsonFormat()
	{
		return $"{gateEnterX}{gateEnterY}:{gateExitX}{gateExitY}";
	}

	public string GetGeneratorDropJsonFormat()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (MapDataGeneratorDrop value in dicGeneratorDropBlock.Values)
		{
			if (!value.IsAllRandomValue())
			{
				stringBuilder.Append(value.GetJsonFormat());
				stringBuilder.Append(",");
			}
		}
		if (stringBuilder.Length > 0 && stringBuilder[stringBuilder.Length - 1] == ',')
		{
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
		}
		return stringBuilder.ToString();
	}

	public string GetGeneratorSpecialDropJsonFormat()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (MapDataGeneratorSpecialDrop value in dicGeneratorSpecialDropBlock.Values)
		{
			stringBuilder.Append(value.GetJsonFormat());
			stringBuilder.Append(",");
		}
		if (stringBuilder.Length > 0 && stringBuilder[stringBuilder.Length - 1] == ',')
		{
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
		}
		return stringBuilder.ToString();
	}

	public string GetMapParamJsonFormat()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (tutorial1X != -1 && tutorial1Y != -1 && tutorial2X != -1 && tutorial2Y != -1)
		{
			stringBuilder.Append("Tutorial:");
			stringBuilder.AppendFormat("{0},{1},{2},{3}", tutorial1X, tutorial1Y, tutorial2X, tutorial2Y);
		}
		return stringBuilder.ToString();
	}

	public string GetDropBlockProbJsonFormat()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < MapData.MaxBlockColor; i++)
		{
			stringBuilder.Append(dropBlockProb[i]);
			if (i < MapData.MaxBlockColor - 1)
			{
				stringBuilder.Append(",");
			}
		}
		return stringBuilder.ToString();
	}

	public string GetMovingJsonFormat()
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		foreach (MapDataMovingSlot item in listMovingSlot)
		{
			stringBuilder.Append(item.GetJsonFormat());
			if (++num < listMovingSlot.Count)
			{
				stringBuilder.Append(",");
			}
		}
		return stringBuilder.ToString();
	}

	public string GetRotaitonJsonFormat()
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		foreach (MapDataRotationSlot item in listRotationSlot)
		{
			stringBuilder.Append(item.GetJsonFormat());
			if (++num < listRotationSlot.Count)
			{
				stringBuilder.Append(",");
			}
		}
		return stringBuilder.ToString();
	}
}
